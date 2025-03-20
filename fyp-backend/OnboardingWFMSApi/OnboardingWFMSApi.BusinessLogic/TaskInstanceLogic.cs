using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Diagnostics;
using OnboardingWFMSApi.BusinessLogic.MediatRHandlers;
using OnboardingWFMSApi.BusinessLogic.TaskInstanceHandlers;
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

        public Task<HTTPResponse<string, string>> UpdateChecklistInstanceState(UpdateInstanceStateChecklistPayload payload, string accountId);
        public Task<HTTPResponse<string, string>> UpdateReadDocumentInstanceState(UpdateInstanceStateReadDocPayload payload, string accountId);
        public Task<HTTPResponse<string, string>> UpdateFileUploadInstanceState(UpdateInstanceStateFileUploadPayload payload, string accountId);
        public Task<HTTPResponse<string, string>> CompleteTaskInstance(string taskInstanceId, string accountId);

        public Task<List<TaskInstanceDTO>> GetTaskInstancesByWorkflowInstance(string workflowInstanceId);
    }
    public class TaskInstanceLogic : ITaskInstanceLogic
    {
        public const string OPEN_TASK_STATUS = "open";
        public const string COMPLETED_TASK_STATUS = "complete";

        private readonly IMapper _mapper;
        private readonly IMediator _mediator;

        private readonly ITaskTemplateLogic _taskTemplateLogic;
        private readonly IDocumentLogic _documentLogic;

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
            IDocumentLogic documentLogic, IFileUploadTaskInstanceRepository uploadTaskInstanceRepository, ITaskInstanceHandlerFactory taskInstanceHandlerFactory, 
            IWorkflowTemplateRepository workflowTemplateRepository, IWorkflowInstanceRepository workflowInstanceRepository, IMediator mediator
        )
        {
            _taskInstanceRepository = taskInstanceRepository;
            _mapper = mapper;
            _taskTemplateLogic = taskTemplateLogic;
            _accountRepository = accountRepository;
            _checklistTaskInstanceRepository = checklistTaskInstanceRepository;
            _taskTemplateRepository = taskTemplateRepository;
            _readDocumentTaskInstanceRepository = readDocumentTaskInstanceRepository;
            _documentLogic = documentLogic;
            _uploadTaskInstanceRepository = uploadTaskInstanceRepository;
            _taskInstanceHandlerFactory = taskInstanceHandlerFactory;
            _workflowTemplateRepository = workflowTemplateRepository;
            _workflowInstanceRepository = workflowInstanceRepository;
            _mediator = mediator;
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
                WorkflowNodeId = payload.WorkflowNodeId ?? null,
                CreationTimestamp = DateTime.Now,
                Status = OPEN_TASK_STATUS,
                DueDate = payload.DueInXDays != null ? DateTime.Now.AddDays(payload.DueInXDays ?? 0.0) : null,
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
                // create instance of task type data
                // i.e. checklist task -> create row in ChecklistTaskInstance repo
            }
            catch (Exception ex)
            {
                return serverErrorResponse;
            }

            try
            {
                var handler = _taskInstanceHandlerFactory.GetHandler(taskTemplate.TaskTypeId);
                if (handler == null)
                {
                    throw new Exception("Tasks template has an invalid task type");
                }
                // insert task instance meta data using handler
                var result = await handler.InsertTaskInstanceMetaData(taskTemplate.TaskTypeData, taskInstance.Id);
                if (!result.Success)
                {
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
            var dataResponse = await handler.GetTaskInstanceMetaData(taskInstance.Id);
            if (!dataResponse.Success) return new HTTPResponse<TaskInstanceDTO, string>() { Success = false, HttpCode = 500, Error = dataResponse.Error };
            taskInstance.InstanceData = dataResponse.Data;

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

        public async Task<HTTPResponse<string, string>> UpdateChecklistInstanceState(UpdateInstanceStateChecklistPayload payload, string accountId)
        {
            // check task is a checklist task
            var response = await GetTaskInstance(payload.TaskInstanceId);
            if (!response.Success || response.Data == null)
            {
                return new HTTPResponse<string, string>() { Success = false, HttpCode = 400, Error = "Task instance doesn't exist" };
            }
            var taskInstance = response.Data;
            if (taskInstance.template.TaskTypeId != TaskTemplateLogic.CHECKLIST_TASK_TYPE_ID)
            {
                return new HTTPResponse<string, string>() { Success = false, HttpCode = 400, Error = "Task instance isn't a checklist task" };
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

            // VALIDATE STATE
            // check that checklist status length is correct
            var taskItems = (taskInstance.template.TaskTypeData as ChecklistTaskTemplateTable).Items;
            if (payload.ItemCompletionStatuses.Length != taskItems.Length)
            {
                return new HTTPResponse<string, string>() { Success = false, HttpCode = 400, Error = "Invalid checklist state" };
            }

            // update state
            var checklistInstance = await _checklistTaskInstanceRepository.GetByTaskInstanceId(taskInstance.Id);
            checklistInstance.ItemCompletionStatuses = payload.ItemCompletionStatuses;
            await _checklistTaskInstanceRepository.UpdateAsync(checklistInstance);

            return new HTTPResponse<string, string>() { Success = true, HttpCode = 200, Message = "Successfully updated state" };
        }

        public async Task<HTTPResponse<string, string>> UpdateReadDocumentInstanceState(UpdateInstanceStateReadDocPayload payload, string accountId)
        {
            // check task is a checklist task
            var response = await GetTaskInstance(payload.TaskInstanceId);
            if (!response.Success || response.Data == null)
            {
                return new HTTPResponse<string, string>() { Success = false, HttpCode = 400, Error = "Task instance doesn't exist" };
            }
            var taskInstance = response.Data;
            if (taskInstance.template.TaskTypeId != TaskTemplateLogic.READ_DOCUMENT_TASK_TYPE_ID)
            {
                return new HTTPResponse<string, string>() { Success = false, HttpCode = 400, Error = "Task instance isn't a read document task" };
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

            // VALIDATE STATE

            // update state
            var readDocInstance = await _readDocumentTaskInstanceRepository.GetByTaskInstanceId(taskInstance.Id);
            readDocInstance.LinkClicked = payload.LinkClicked;
            readDocInstance.CheckboxChecked = payload.CheckboxChecked;
            await _readDocumentTaskInstanceRepository.UpdateAsync(readDocInstance);

            return new HTTPResponse<string, string>() { Success = true, HttpCode = 200, Message = "Successfully updated state" };
        }

        public async Task<HTTPResponse<string, string>> UpdateFileUploadInstanceState(UpdateInstanceStateFileUploadPayload payload, string accountId)
        {
            // check task is a "file upload" task
            var response = await GetTaskInstance(payload.TaskInstanceId);
            if (!response.Success || response.Data == null)
            {
                return new HTTPResponse<string, string>() { Success = false, HttpCode = 400, Error = "Task instance doesn't exist" };
            }
            var taskInstance = response.Data;
            if (taskInstance.template.TaskTypeId != TaskTemplateLogic.UPLOAD_DOCUMENT_TASK_TYPE_ID)
            {
                return new HTTPResponse<string, string>() { Success = false, HttpCode = 400, Error = "Task instance isn't a read document task" };
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

            // validate state           
            // ensure file contains data
            if (payload.File == null)
            {
                return new HTTPResponse<string, string>() { Success = false, HttpCode = 400, Error = "Document was not uploaded" };
            }
            // ensure file extension is valid
            var taskTypeTemplateData = taskInstance.template.TaskTypeData as FileUploadTaskTemplateTable;               
            var fileExtension = Path.GetExtension(payload.File.FileName);
            if (!taskTypeTemplateData.SupportedDocumentType.Split(";").Contains(fileExtension))
            {
                return new HTTPResponse<string, string>() { Success = false, HttpCode = 400, Error = "Invalid file extension" };
            }

            // attempt to upload document            
            var result = await _documentLogic.UploadDocument(
                new UploadDocumentPayload()
                {
                    TaskInstanceId = taskInstance.Id,
                    WorkflowId = "---",
                    File = payload.File,
                    DocumentName = taskTypeTemplateData.DocumentName,
                },
                accountId
            );
            if (!result.Success)
            {
                return new HTTPResponse<string, string>() { Success = false, HttpCode = 500, Error = "Failed to upload document" };
            }
            else
            {
                var document = result.Data;
                var uploadDocInstance = await _uploadTaskInstanceRepository.GetByTaskInstanceId(taskInstance.Id);
                uploadDocInstance.DocumentId = document.Id;
                uploadDocInstance.TaskInstanceId = taskInstance.Id;
                uploadDocInstance.UploadedTimestamp = DateTime.UtcNow;

                await _uploadTaskInstanceRepository.UpdateAsync(uploadDocInstance);
                return new HTTPResponse<string, string>() { Success = true, HttpCode = 200, Data = "Successfully upload document" };
            }                      
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
    }

}
