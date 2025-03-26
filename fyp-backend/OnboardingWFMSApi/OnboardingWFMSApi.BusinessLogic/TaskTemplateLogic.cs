using AutoMapper;
using Microsoft.Extensions.Logging;
using OnboardingWFMSApi.BusinessLogic.TaskTemplateHandlers;
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
        public Task<HTTPResponse<List<TaskTemplate>, string>> GetAllTaskTemplates();
        public Task<HTTPResponse<string, string>> CreateTaskTemplate(CreateTaskTemplatePayload payload, string accountId);
        public Task<HTTPResponse<TaskTemplate, string>> GetTaskTemplateById(string id);
        public Task<HTTPResponse<List<TaskType>, string>> GetAllTaskTypes();
    }

    public class TaskTemplateLogic : ITaskTemplateLogic
    {
        // TODO: separate these const from this class, put in constants class
        public const string UPLOAD_DOCUMENT_TASK_TYPE_ID = "upload-document";
        public const string READ_DOCUMENT_TASK_TYPE_ID = "read-document";
        public const string CHECKLIST_TASK_TYPE_ID = "checklist";

        private readonly ITaskTemplateRepository _taskTemplateRepository;
        private readonly ITaskTypeRepository _taskTypeRepository;

        private readonly ITaskTemplateHandlerFactory _taskTemplateHandlerFactory;

        private readonly IMapper _mapper;
        private readonly ILogger<TaskTemplateLogic> _logger;

        public TaskTemplateLogic(ITaskTemplateRepository taskTemplateRepository, IMapper mapper,
            ITaskTypeRepository taskTypeRepository, ITaskTemplateHandlerFactory taskTemplateHandlerFactory, ILogger<TaskTemplateLogic> logger)
        {
            _mapper = mapper;
            _taskTemplateRepository = taskTemplateRepository;
            _taskTypeRepository = taskTypeRepository;
            _taskTemplateHandlerFactory = taskTemplateHandlerFactory;
            _logger = logger;
        }

        public async Task<HTTPResponse<string, string>> CreateTaskTemplate(CreateTaskTemplatePayload payload, string accountId)
        {           
            var taskTemplate = await _taskTemplateRepository.AddAsync(new TaskTemplateTable() { CreatorAccountId = accountId, Name = payload.Name, Description = payload.Description, DateCreated = DateTime.Now, TaskTypeId = payload.TaskTypeId });          

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

        public async Task<HTTPResponse<List<TaskTemplate>, string>> GetAllTaskTemplates()
        {
            List<TaskTemplate> taskTemplates = _mapper.Map<List<TaskTemplate>>(await _taskTemplateRepository.GetAll());
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
    }

}
