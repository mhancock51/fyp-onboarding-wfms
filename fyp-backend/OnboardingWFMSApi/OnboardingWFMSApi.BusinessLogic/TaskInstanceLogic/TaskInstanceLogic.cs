using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using OnboardingWFMSApi.BusinessLogic.Factories;
using OnboardingWFMSApi.BusinessLogic.Handlers.MediatRHandlers;
using OnboardingWFMSApi.BusinessLogic.Handlers.TaskTemplateHandlers;
using OnboardingWFMSApi.BusinessLogic.TaskTemplateLogic;
using OnboardingWFMSApi.BusinessLogic.WorkflowInstanceLogic;
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

namespace OnboardingWFMSApi.BusinessLogic.TaskInstanceLogic
{
    public interface ITaskInstanceLogic
    {
        public Task<HTTPResponse<string, string>> CreateInstance(CreateTaskInstancePayload payload);
        public Task<HTTPResponse<List<TaskInstanceDTO>, string>> GetUsersAssignedTask(string accountId);
        public Task<HTTPResponse<TaskInstanceDTO, string>> GetTaskInstance(string tasksInstanceId);
        public Task<HTTPResponse<string, string>> UpdateInstanceState(UpdateInstanceStatePayload payload, string accountId);
        public Task<HTTPResponse<string, string>> CompleteTaskInstance(string taskInstanceId, string accountId);
        public Task<List<TaskInstanceDTO>> GetTaskInstancesByWorkflowInstance(string workflowInstanceId);
        public Task<HTTPResponse<List<TaskInstanceDTO>, string>> GetWorkflowInstanceCompletedFeedbackTasks(string workflowInstanceId);
    }
    public class TaskInstanceLogic : ITaskInstanceLogic
    {
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
        private readonly ILogger<TaskInstanceLogic> _logger;

        private readonly ITaskTemplateLogic _taskTemplateLogic;

        private readonly ITaskInstanceRepository _taskInstanceRepository;
        private readonly ITaskTemplateRepository _taskTemplateRepository;
        private readonly IAccountRepository _accountRepository;

        private readonly ITaskInstanceHandlerFactory _taskInstanceHandlerFactory;

        public TaskInstanceLogic(ITaskInstanceRepository taskInstanceRepository, IMapper mapper, ITaskTemplateLogic taskTemplateLogic,
            IAccountRepository accountRepository, ITaskTemplateRepository taskTemplateRepository, ITaskInstanceHandlerFactory taskInstanceHandlerFactory,
            IMediator mediator, ILogger<TaskInstanceLogic> logger)
        {
            _taskInstanceRepository = taskInstanceRepository;
            _mapper = mapper;
            _taskTemplateLogic = taskTemplateLogic;
            _accountRepository = accountRepository;
            _taskTemplateRepository = taskTemplateRepository;
            _taskInstanceHandlerFactory = taskInstanceHandlerFactory;
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
                CreationTimestamp = DateTime.UtcNow,
                Status = TaskInstanceConstants.OPEN_TASK_STATUS,
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
                var response = await _taskTemplateLogic.GetTaskTemplateDTOById(instance.TaskTemplateId);
                taskTemplate = response.Data;
                if (taskTemplate == null)
                {
                    _logger.LogError("Failed to load task template DTO for TaskTemplateId={TaskTemplateId}", instance.TaskTemplateId);
                    await _taskInstanceRepository.DeleteAsync(taskInstance);
                    return serverErrorResponse;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create task instance row or load template for TaskTemplateId={TaskTemplateId}", payload.TaskTemplateId);
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

                // create audit log
                if (payload.WorkflowInstanceId != null)
                {
                    var log = new CreateWorkflowInstanceAuditLogPayload()
                    {
                        WorkflowInstanceId = payload.WorkflowInstanceId,
                        Log = $"'{taskTemplate.Name}' Task asssigned",
                        AccountId = taskInstance.AssigneeAccountId,
                    };
                    await _mediator.Send(new CreateWorkflowInstanceAuditLogRequest(log));
                }
                return new HTTPResponse<string, string>() { Success = true, HttpCode = 200, Data = "Successfully created task instance" };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create task type instance data for TaskInstanceId={TaskInstanceId} TaskTypeId={TaskTypeId}", taskInstance?.Id, taskTemplate?.TaskTypeId);
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
            var response = await _taskTemplateLogic.GetTaskTemplateDTOById(taskInstance.TaskTemplateId);
            if (response.Success)
            {
                taskInstance.Template = response.Data;
            }
            else
            {
                throw new Exception("Task instance isn't associated with a valid task template");
            }
            // get task type data            
            var handler = _taskInstanceHandlerFactory.GetHandler(taskInstance.Template.TaskTypeId);
            if (handler == null)
            {
                throw new Exception("Task instance is associated with an invalid task type id");
            }
            try
            {
                var dataResponse = await handler.FetchTaskInstanceData(taskInstance.Id);
                taskInstance.InstanceData = dataResponse.Data;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching task type data: {ex.Message}");
                return new HTTPResponse<TaskInstanceDTO, string>() { Success = false, HttpCode = 500, Error = "Failed to create task instance data" };
            }

            // get task instance DTO
            if (taskInstance.WorkflowInstanceId != null)
            {
                var mediatorReponse = await _mediator.Send(new RetrieveWorkflowInstanceRequest(taskInstance.WorkflowInstanceId));
                if (mediatorReponse.Success == false)
                {
                    _logger.LogWarning($"Failed to retrieve workflow instance {taskInstance.WorkflowInstanceId} through mediator");
                }
                else
                {
                    taskInstance.WorkflowInstance = mediatorReponse.Data;
                }
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
            if (taskInstance.Status != TaskInstanceConstants.OPEN_TASK_STATUS)
            {
                return new HTTPResponse<string, string>() { Success = false, HttpCode = 400, Error = "Task isn't open" };
            }
            // check account is assigned to task instance
            if (taskInstance.AssigneeAccountId != accountId)
            {
                return new HTTPResponse<string, string>() { Success = false, HttpCode = 400, Error = "User doesn't have access to update this resource" };
            }

            // make sure task meets conditions to be complete using handler
            var handler = _taskInstanceHandlerFactory.GetHandler(taskInstance.Template.TaskTypeId);
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
            taskInstance.Status = TaskInstanceConstants.COMPLETED_TASK_STATUS;
            taskInstance.CompletionTimestamp = DateTime.Now;

            await _taskInstanceRepository.UpdateAsync(taskInstance);

            var mediatorResponse = await _mediator.Send(new TaskCompletedRequest(taskInstance));

            // create audit log
            if (taskInstance.WorkflowInstanceId != null)
            {
                var taskTemplate = (await _taskTemplateLogic.GetTaskTemplateDTOById(taskInstance.TaskTemplateId)).Data;
                var log = new CreateWorkflowInstanceAuditLogPayload()
                {
                    WorkflowInstanceId = taskInstance.WorkflowInstanceId,
                    Log = $"'{taskTemplate.Name}' Task completed",
                    AccountId = accountId
                };
                await _mediator.Send(new CreateWorkflowInstanceAuditLogRequest(log));
            }                    

            return new HTTPResponse<string, string>() { Success = true, HttpCode = 200, Data = "Successfully completed task" };
        }

        public async Task<List<TaskInstanceDTO>> GetTaskInstancesByWorkflowInstance(string workflowInstanceId)
        {
            var instanceRows = await _taskInstanceRepository.GetTaskInstancesByWorkflowInstance(workflowInstanceId);
            List<TaskInstanceDTO> taskInstances = new List<TaskInstanceDTO>();
            foreach (var instanceRow in instanceRows)
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
            if (taskInstance.Status != TaskInstanceConstants.OPEN_TASK_STATUS)
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

        public async Task<HTTPResponse<List<TaskInstanceDTO>, string>> GetWorkflowInstanceCompletedFeedbackTasks(string workflowInstanceId)
        {
            var feedbackTasks = (await GetTaskInstancesByWorkflowInstance(workflowInstanceId)).Where(i => i.Status == TaskInstanceConstants.COMPLETED_TASK_STATUS && i.Template.TaskTypeId == "feedback").ToList();
            return new HTTPResponse<List<TaskInstanceDTO>, string>() { Success = true, Data = feedbackTasks, HttpCode = 200 };
        }
    }

}
