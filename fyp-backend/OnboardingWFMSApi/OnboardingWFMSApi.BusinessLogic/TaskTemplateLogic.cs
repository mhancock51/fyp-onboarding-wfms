using AutoMapper;
using Microsoft.Extensions.Logging;
using OnboardingWFMSApi.BusinessLogic.Factories;
using OnboardingWFMSApi.DataAccess.Repositories.Task_Repositories;
using OnboardingWFMSApi.DataModels;
using OnboardingWFMSApi.DataModels.Models;
using OnboardingWFMSApi.DataModels.Payloads;
using OnboardingWFMSApi.DataModels.Tables;
using OnboardingWFMSApi.DataModels.Tables.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace OnboardingWFMSApi.BusinessLogic
{
    public interface ITaskTemplateLogic
    {
        public Task<HTTPResponse<List<TaskTemplate>, string>> GetAllTaskTemplates(string? status);
        public Task<HTTPResponse<string, string>> CreateTaskTemplate(CreateTaskTemplatePayload payload, string accountId);
        public Task<HTTPResponse<TaskTemplate, string>> GetTaskTemplateById(string id);
        public Task<HTTPResponse<List<TaskType>, string>> GetAllTaskTypes();
        public Task<HTTPResponse<string, string>> ArchiveTaskTemplate(string taskTemplateId);
        public Task<HTTPResponse<string, string>> UpdateTaskTemplate(UpdateTaskTemplatePayload payload);
    }

    public class TaskTemplateLogic : ITaskTemplateLogic
    {
        public const string ACTIVE_TASK_TEMPLATE_STATUS = "active";
        public const string ARCHIVED_TASK_TEMPLATE_STATUS = "archived";

        // TODO: separate these const from this class, put in constants class
        public const string UPLOAD_DOCUMENT_TASK_TYPE_ID = "upload-document";
        public const string READ_DOCUMENT_TASK_TYPE_ID = "read-document";
        public const string CHECKLIST_TASK_TYPE_ID = "checklist";

        private readonly ITaskTemplateRepository _taskTemplateRepository;
        private readonly ITaskTypeRepository _taskTypeRepository;
        private readonly ITaskInstanceRepository _taskInstanceRepository;

        private readonly ITaskTemplateHandlerFactory _taskTemplateHandlerFactory;

        private readonly IMapper _mapper;
        private readonly ILogger<TaskTemplateLogic> _logger;

        public TaskTemplateLogic(ITaskTemplateRepository taskTemplateRepository, IMapper mapper,
            ITaskTypeRepository taskTypeRepository, ITaskTemplateHandlerFactory taskTemplateHandlerFactory, ILogger<TaskTemplateLogic> logger, ITaskInstanceRepository taskInstanceRepository)
        {
            _mapper = mapper;
            _taskTemplateRepository = taskTemplateRepository;
            _taskTypeRepository = taskTypeRepository;
            _taskTemplateHandlerFactory = taskTemplateHandlerFactory;
            _logger = logger;
            _taskInstanceRepository = taskInstanceRepository;
        }

        public async Task<HTTPResponse<string, string>> ArchiveTaskTemplate(string taskTemplateId)
        {
            // get task template
            var taskTemplate = await _taskTemplateRepository.GetById(taskTemplateId);
            if (taskTemplate == null)
            {
                return new HTTPResponse<string, string>() { Success = false, HttpCode = 400, Error = "No task template found" };
            }
            if (taskTemplate.Status == ARCHIVED_TASK_TEMPLATE_STATUS)
            {
                return new HTTPResponse<string, string>() { Success = false, HttpCode = 400, Error = "Task template is already archived" };
            }

            taskTemplate.Status = ARCHIVED_TASK_TEMPLATE_STATUS;
            try
            {
                await _taskTemplateRepository.UpdateAsync(taskTemplate);
                return new HTTPResponse<string, string>() { Success = false, HttpCode = 200, Data = "Successfully archived task template" };
            }
            catch (Exception ex)
            {
                return new HTTPResponse<string, string>() { Success = false, HttpCode = 500, Error = "Failed to archive task template" };
            }
        }

        public async Task<HTTPResponse<string, string>> CreateTaskTemplate(CreateTaskTemplatePayload payload, string accountId)
        {           
            var taskTemplate = await _taskTemplateRepository.AddAsync(new TaskTemplateTable() { 
                CreatorAccountId = accountId, 
                Name = payload.Name, 
                Description = payload.Description, 
                DateCreated = DateTime.Now, 
                TaskTypeId = payload.TaskTypeId,
                Status = ACTIVE_TASK_TEMPLATE_STATUS
            });          

            HTTPResponse<string, string> invalidTaskDataResponse = new HTTPResponse<string, string>() { Success = false, HttpCode = 400, Data = "Invalid task data" };

            var handler = _taskTemplateHandlerFactory.GetHandler(payload.TaskTypeId);
            if (handler == null)
            {
                // delete task template
                await _taskTemplateRepository.DeleteAsync(taskTemplate);
                return new HTTPResponse<string, string>() { Success = false, HttpCode = 400, Error = "Invalid task type provided" };
            }

            // handle the insertion of task type meta data, i.e. checklist data, file upload data, etc
            try
            {
                var response = await handler.CreateTaskTemplateData(payload.TaskTypeData, taskTemplate.Id);
                if (response.Success == false)
                {
                    // delete task template
                    await _taskTemplateRepository.DeleteAsync(taskTemplate);
                    return new HTTPResponse<string, string>() { Success = false, HttpCode = 500, Error = "Failed to insert task type meta data" };
                }
            }
            catch(Exception ex)
            {
                _logger.LogError($"Error inserting task template data: {ex.Message}");
            }
            
            return new HTTPResponse<string, string>() { Success = true, Data = "Created task template", HttpCode = 200 };
        }

        public async Task<HTTPResponse<List<TaskTemplate>, string>> GetAllTaskTemplates(string? status)
        {
            List<TaskTemplate> taskTemplates = _mapper.Map<List<TaskTemplate>>(await _taskTemplateRepository.GetAll());
            if (!string.IsNullOrEmpty(status))
            {
                taskTemplates = taskTemplates.Where(t => t.Status == status).ToList();
            }
            for (int i = 0; i < taskTemplates.Count; i++)
            {
                var response = await GetTaskTemplateById(taskTemplates[i].Id);
                if (response.Success)
                {
                    taskTemplates[i] = response.Data;
                }
            }            
            return new HTTPResponse<List<TaskTemplate>, string>() { Success = true, HttpCode = 200, Data = taskTemplates };
        }

        public async Task<HTTPResponse<List<TaskType>, string>> GetAllTaskTypes()
        {            
            var taskTypes = _mapper.Map<List<TaskType>>(await _taskTypeRepository.GetAll());
            return new HTTPResponse<List<TaskType>, string>() { Success = true, Data = taskTypes, HttpCode = 200 };            
        }

        public async Task<HTTPResponse<TaskTemplate, string>> GetTaskTemplateById(string id)
        {
            TaskTemplate taskTemplate = _mapper.Map<TaskTemplate>(await _taskTemplateRepository.GetById(id));
            if (taskTemplate == null)
            {
                return new HTTPResponse<TaskTemplate, string>() { Success = false, Error = "Task template doesn't exist", HttpCode = 400 };
            }
            else
            {
                // retrieve task type meta data using handler
                var handler = _taskTemplateHandlerFactory.GetHandler(taskTemplate.TaskTypeId);
                if (handler == null)
                {
                    throw new InvalidOperationException("Invalid task type associated with task template");
                }
                var response = await handler.FetchTaskTemplateData(taskTemplate.Id);
                if (!response.Success) 
                {
                    return new HTTPResponse<TaskTemplate, string>() { Success = false, Error = response.Error, HttpCode = 500 };
                }
                taskTemplate.TaskTypeData = response.Data;
            }
            taskTemplate.taskType = _mapper.Map<TaskType>(await _taskTypeRepository.GetById(taskTemplate.TaskTypeId));

            return new HTTPResponse<TaskTemplate, string>() { Success = true, Data = taskTemplate, HttpCode = 200 };
        }

        public async Task<HTTPResponse<string, string>> UpdateTaskTemplate(UpdateTaskTemplatePayload payload)
        {
            // get task template
            var taskTemplateDTO = (await GetTaskTemplateById(payload.Id)).Data ?? null;
            if (taskTemplateDTO == null) 
            {
                return new HTTPResponse<string, string>() { Success = false, HttpCode = 400, Error = "Task template doesn't exist" };
            }            
            
            // check that it hasn't been archived
            if (taskTemplateDTO.Status != ACTIVE_TASK_TEMPLATE_STATUS)
            {
                return new HTTPResponse<string, string>() { Success = false, HttpCode = 400, Error = "Task template isn't active" };
            }

            // figure out if instances of the task exist
            var taskInstances = await _taskInstanceRepository.GetTaskInstancesByTaskTemplateId(payload.Id);
            var hasActiveInstances = false;
            foreach (var taskInstance in taskInstances)
            {
                if (taskInstance.Status != TaskInstanceLogic.COMPLETED_TASK_STATUS)
                {
                    hasActiveInstances = true;
                }
            }

            // update task type data using handler
            var handler = _taskTemplateHandlerFactory.GetHandler(taskTemplateDTO.TaskTypeId);
            if (handler == null)
            {
                _logger.LogError($"Task template {taskTemplateDTO.Id} is not associated with a valid task type Id");
                return new HTTPResponse<string, string>() { Success = false, HttpCode = 500, Error = "Failed to update task type data" };
            }
            var response = await handler.UpdateTaskTemplateData(payload.UpdateTaskTypeData, taskTemplateDTO.TaskTypeData, hasActiveInstances);
            if (!response.Success)
            {
                return new HTTPResponse<string, string>() { Success = false, Error = response.Error, HttpCode = 500 };
            }
            // retrive current record
            var taskTemplateRow = await _taskTemplateRepository.GetById(taskTemplateDTO.Id);

            // update description if changed
            if (!string.IsNullOrEmpty(payload.UpdatedDescription))
            {
                taskTemplateRow.Description = payload.UpdatedDescription;
            }
            // update last modified
            taskTemplateRow.LastModifiedTimestamp = DateTime.Now;
            try
            {
                await _taskTemplateRepository.UpdateAsync(taskTemplateRow);
                return new HTTPResponse<string, string>() { Success = true, Data = "Successfully updated task template", HttpCode = 200 };
            }
            catch(Exception ex)
            {
                _logger.LogError("Failed to update task template");
                return new HTTPResponse<string, string>() { Success = false, Error = "Failed to update task template", HttpCode = 500 };
            }
        }
    }

}
