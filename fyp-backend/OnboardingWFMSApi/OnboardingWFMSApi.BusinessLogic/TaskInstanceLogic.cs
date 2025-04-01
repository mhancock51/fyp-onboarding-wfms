using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using OnboardingWFMSApi.BusinessLogic.Factories;
using OnboardingWFMSApi.BusinessLogic.MediatRHandlers;
using OnboardingWFMSApi.BusinessLogic.TaskTemplateHandlers;
using OnboardingWFMSApi.DataAccess.Repositories;
using OnboardingWFMSApi.DataAccess.Repositories.Task_Repositories;
using OnboardingWFMSApi.DataAccess.Repositories.Workflow_Repositories;
using OnboardingWFMSApi.DataModels;
using OnboardingWFMSApi.DataModels.Models;
using OnboardingWFMSApi.DataModels.Payloads;
using OnboardingWFMSApi.DataModels.Tables;
using OnboardingWFMSApi.DataModels.Tables.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnboardingWFMSApi.BusinessLogic
{
    public interface ITaskInstanceLogic
    {
        public Task<HTTPResponse<string, string>> CreateInstance(CreateTaskInstancePayload payload);
        public Task<HTTPResponse<List<TaskInstanceDTO>, string>> GetUsersAssignedTask(string accountId);
        public Task<HTTPResponse<TaskInstanceDTO, string>> GetTaskInstance(string tasksInstanceId);
        public Task<HTTPResponse<string, string>> UpdateInstanceState(UpdateInstanceStatePayload payload, string accountId);
        public Task<HTTPResponse<string, string>> CompleteTaskInstance(string taskInstanceId, string accountId);
        public Task<List<TaskInstanceDTO>> GetTaskInstancesByWorkflowInstance(string workflowInstanceId);
    }
    public class TaskInstanceLogic : ITaskInstanceLogic
    {
        public const string OPEN_TASK_STATUS = "open";
        public const string COMPLETED_TASK_STATUS = "complete";

        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
        private readonly ILogger<TaskInstanceLogic> _logger;

        private readonly ITaskTemplateLogic _taskTemplateLogic;        

        private readonly ITaskInstanceRepository _taskInstanceRepository;
        private readonly ITaskTemplateRepository _taskTemplateRepository;
        private readonly IAccountRepository _accountRepository;

        private readonly IChecklistTaskInstanceRepository _checklistTaskInstanceRepository;
        private readonly IReadDocumentTaskInstanceRepository _readDocumentTaskInstanceRepository;
        private readonly IFileUploadTaskInstanceRepository _uploadTaskInstanceRepository;

        private readonly IWorkflowInstanceRepository _workflowInstanceRepository;
        private readonly IWorkflowTemplateRepository _workflowTemplateRepository;

        private readonly ITaskInstanceHandlerFactory _taskInstanceHandlerFactory;

        public TaskInstanceLogic(ITaskInstanceRepository taskInstanceRepository, IMapper mapper, ITaskTemplateLogic taskTemplateLogic,
            IAccountRepository accountRepository, ITaskTemplateRepository taskTemplateRepository,
            IChecklistTaskInstanceRepository checklistTaskInstanceRepository, IReadDocumentTaskInstanceRepository readDocumentTaskInstanceRepository,
            IFileUploadTaskInstanceRepository uploadTaskInstanceRepository, ITaskInstanceHandlerFactory taskInstanceHandlerFactory,
            IWorkflowTemplateRepository workflowTemplateRepository, IWorkflowInstanceRepository workflowInstanceRepository, IMediator mediator
, ILogger<TaskInstanceLogic> logger)
        {
            _taskInstanceRepository = taskInstanceRepository;
            _mapper = mapper;
            _taskTemplateLogic = taskTemplateLogic;
            _accountRepository = accountRepository;
            _checklistTaskInstanceRepository = checklistTaskInstanceRepository;
            _taskTemplateRepository = taskTemplateRepository;
            _readDocumentTaskInstanceRepository = readDocumentTaskInstanceRepository;
            _uploadTaskInstanceRepository = uploadTaskInstanceRepository;
            _taskInstanceHandlerFactory = taskInstanceHandlerFactory;
            _workflowTemplateRepository = workflowTemplateRepository;
            _workflowInstanceRepository = workflowInstanceRepository;
            _mediator = mediator;
            _logger = logger;
        }

        public async Task<HTTPResponse<string, string>> CreateInstance(CreateTaskInstancePayload payload)
        {
            var serverErrorResponse = new HTTPResponse<string, string>() { Success = false, HttpCode = 500, Error = "Failed to create instance of task" };

            if (string.IsNullOrEmpty(payload.AssignerAccountId))
            {
                return new HTTPResponse<string, string>() { Success = false, HttpCode = 400, Error = "Please choose an assigner" };
            }
            if (string.IsNullOrEmpty(payload.AssigneeAccountId))
            {
                return new HTTPResponse<string, string>() { Success = false, HttpCode = 400, Error = "Please choose an assignee" };
            }
            if (string.IsNullOrEmpty(payload.TaskTemplateId))
            {
                return new HTTPResponse<string, string>() { Success = false, HttpCode = 400, Error = "Please choose an assignee" };
            }
            
            var instance = new TaskInstanceTable()
            {

                AssigneeAccountId = payload.AssigneeAccountId,
                AssignerAccountId = payload.AssignerAccountId,
                TaskTemplateId = payload.TaskTemplateId,
                WorkflowInstanceId = payload.WorkflowInstanceId ?? null,
                WorkflowInstanceNodeId = payload.WorkflowInstanceNodeId ?? null,                
                CreationTimestamp = DateTime.Now,
                Status = OPEN_TASK_STATUS,
                DueDate = payload.DueDate,
            };
            TaskInstanceTable taskInstance = null;
            TaskTemplate taskTemplate = null;
            try
            {
                taskInstance = await _taskInstanceRepository.AddAsync(instance);
                if (taskInstance == null)
                {
                    return serverErrorResponse;
                }
                // load template
                var response = await _taskTemplateLogic.GetTaskTemplateById(instance.TaskTemplateId);
                taskTemplate = response.Data;                
            }
            catch (Exception ex)
            {
                return serverErrorResponse;
            }

            try
            {
                // create instance of task type data
                // i.e. checklist task -> create row in ChecklistTaskInstance repo
                var handler = _taskInstanceHandlerFactory.GetHandler(taskTemplate.TaskTypeId);
                if (handler == null)
                {
                    throw new Exception("Tasks template has an invalid task type");
                }
                // insert task instance meta data using handler
                var result = await handler.CreateTaskInstanceData(taskTemplate.TaskTypeData, taskInstance.Id);
                if (!result.Success)
                {
                    await _taskInstanceRepository.DeleteAsync(taskInstance);
                    return new HTTPResponse<string, string>() { Success = false, HttpCode = 500, Data = result.Error };
                }
                return new HTTPResponse<string, string>() { Success = true, HttpCode = 200, Data = "Successfully created task instance" };
            }
            catch (Exception ex)
            {
                // rollback task instance creation
                await _taskInstanceRepository.DeleteAsync(taskInstance);
                return serverErrorResponse;
            }
        }

        public async Task<HTTPResponse<List<TaskInstanceDTO>, string>> GetUsersAssignedTask(string accountId)
        {
            var taskInstances = _mapper.Map<List<TaskInstanceDTO>>(await _taskInstanceRepository.GetUsersTaskInstances(accountId));               
            for (var i = 0; i < taskInstances.Count; i++)
            {
                // add template and task type instance data
                var response = await GetTaskInstance(taskInstances[i].Id);
                if (response.Success)
                {
                    taskInstances[i] = response.Data;
                }
                else
                {
                    throw new Exception("Task instance isn't associated with a valid task template or task type instance");
                }
            }
            return new HTTPResponse<List<TaskInstanceDTO>, string>() { Success = true, HttpCode = 200, Data = taskInstances };
        }

        public async Task<HTTPResponse<TaskInstanceDTO, string>> GetTaskInstance(string tasksInstanceId)
        {
            var taskInstance = _mapper.Map<TaskInstanceDTO>(await _taskInstanceRepository.GetById(tasksInstanceId));
            if (taskInstance == null)
            {
                return new HTTPResponse<TaskInstanceDTO, string>() { Success = false, Error = "No task instance found", HttpCode = 400 };
            }

            // get template data
            var response = await _taskTemplateLogic.GetTaskTemplateById(taskInstance.TaskTemplateId);
            if (response.Success)
            {
                taskInstance.template = response.Data;
            }
            else
            {
                throw new Exception("Task instance isn't associated with a valid task template");
            }
            // get task type data            
            var handler = _taskInstanceHandlerFactory.GetHandler(taskInstance.template.TaskTypeId);
            if (handler == null)
            {
                throw new Exception("Task instance is associated with an invalid task type id");
            }
            try
            {
                var dataResponse = await handler.FetchTaskInstanceData(taskInstance.Id);                
                taskInstance.InstanceData = dataResponse.Data;
            }
            catch(Exception ex)
            {
                _logger.LogError($"Error fetching task type data: {ex.Message}");
                return new HTTPResponse<TaskInstanceDTO, string>() { Success = false, HttpCode = 500, Error = "Failed to create task instance data" };
            }


            if (taskInstance.WorkflowInstanceId != null)
            {
                // retrieve name of workflow template
                var workflowInstance = await _workflowInstanceRepository.GetById(taskInstance.WorkflowInstanceId);
                var workflowTemplate = await _workflowTemplateRepository.GetById(workflowInstance.WorkflowTemplateId);
                taskInstance.WorkflowInstanceTemplateName = workflowTemplate.Name;
            }
            else
            {
                taskInstance.WorkflowInstanceTemplateName = "";
            }

            return new HTTPResponse<TaskInstanceDTO, string>() { Success = true, HttpCode = 200, Data = taskInstance }; 
        }

        public async Task<HTTPResponse<string, string>> CompleteTaskInstance(string taskInstanceId, string accountId)
        {
            // check task instance exists and isn't already complete
            var response = await GetTaskInstance(taskInstanceId);
            if (!response.Success || response.Data == null)
            {
                return new HTTPResponse<string, string>() { Success = false, HttpCode = 400, Error = "Task instance doesn't exist" };
            }
            var taskInstance = response.Data;
            if (taskInstance.Status != OPEN_TASK_STATUS)
            {
                return new HTTPResponse<string, string>() { Success = false, HttpCode = 400, Error = "Task isn't open" };
            }            
            // check account is assigned to task instance
            if (taskInstance.AssigneeAccountId != accountId)
            {
                return new HTTPResponse<string, string>() { Success = false, HttpCode = 400, Error = "User doesn't have access to update this resource" }; 
            }

            // make sure task meets conditions to be complete using handler
            var handler = _taskInstanceHandlerFactory.GetHandler(taskInstance.template.TaskTypeId);
            if (handler == null)
            {
                throw new Exception("Task template isn't associated with a valid task type id");
            }
            var completeResponse = await handler.IsTaskInstanceCompleteable(taskInstance.InstanceData);
            if (!completeResponse.Success)
            {
                return new HTTPResponse<string, string>() { Success = false, HttpCode = 400, Error = completeResponse.Error };
            }
            
            // mark task instance as complete
            taskInstance.Status = COMPLETED_TASK_STATUS;
            await _taskInstanceRepository.UpdateAsync(taskInstance);

            var mediatorResponse = await _mediator.Send(new TaskCompletedRequest(taskInstance));

            // TODO implement logic to notify correct users           

            // TODO implement logging of event for workflow

            return new HTTPResponse<string, string>() { Success = true, HttpCode = 200, Data = "Successfully completed task" };
        }

        public async Task<List<TaskInstanceDTO>> GetTaskInstancesByWorkflowInstance(string workflowInstanceId)
        {
            var instanceRows = await _taskInstanceRepository.GetTaskInstancesByWorkflowInstance(workflowInstanceId);
            List<TaskInstanceDTO> taskInstances = new List<TaskInstanceDTO>();
            foreach(var instanceRow in instanceRows)
            {
                var taskInstance = (await GetTaskInstance(instanceRow.Id)).Data ?? null;
                if (taskInstance != null)
                {
                    taskInstances.Add(taskInstance);
                }
            }
            return taskInstances;
        }

        public async Task<HTTPResponse<string, string>> UpdateInstanceState(UpdateInstanceStatePayload payload, string accountId)
        {
            // validate state of task instance            
            var taskInstance = (await GetTaskInstance(payload.TaskInstanceId)).Data ?? null;
            if (taskInstance == null)
            {
                return new HTTPResponse<string, string>() { Success = false, HttpCode = 400, Error = "Task instance doesn't exist" };
            }
            // ensure task is open
            if (taskInstance.Status != OPEN_TASK_STATUS)
            {
                return new HTTPResponse<string, string>() { Success = false, HttpCode = 400, Error = "Task is not open" };
            }
            // check user is assigned to task instance
            if (taskInstance.AssigneeAccountId != accountId)
            {
                return new HTTPResponse<string, string>() { Success = false, HttpCode = 400, Error = "User not authorised to update this resource" };
            }

            // find appropriate handler
            var handler = _taskInstanceHandlerFactory.GetHandler(payload.TaskTypeId);
            // call method to update instance's tasktype data, i.e. checklist items state
            var response = await handler.UpdateTaskInstanceData(payload.UpdateTaskState);

            return new HTTPResponse<string, string>() { Success = response.Success, HttpCode = response.Success ? 200 : 500, Error = response.Error, Data = response.Data };
        }
    }

}
