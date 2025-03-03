using AutoMapper;
using OnboardingWFMSApi.DataAccess.Repositories.Task_Repositories;
using OnboardingWFMSApi.DataModels;
using OnboardingWFMSApi.DataModels.Models;
using OnboardingWFMSApi.DataModels.Payloads;
using OnboardingWFMSApi.DataModels.Tables;
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
        public Task<HTTPResponse<string, string>> CreateTaskTemplate(CreateTaskTemplatePayload payload);
        public Task<HTTPResponse<TaskTemplate, string>> GetTaskTemplateById(string id);
    }

    public class TaskTemplateLogic : ITaskTemplateLogic
    {
        const string UPLOAD_DOCUMENT_TASK_TYPE_ID = "upload-document";        

        private readonly ITaskTemplateRepository _taskTemplateRepository;
        private readonly IFileUploadTaskTemplateRepository _fileUploadTaskTemplateRepository;
        private readonly IMapper _mapper;

        public TaskTemplateLogic(ITaskTemplateRepository taskTemplateRepository, IMapper mapper, IFileUploadTaskTemplateRepository fileUploadTaskTemplateRepository)
        {
            _taskTemplateRepository = taskTemplateRepository;
            _fileUploadTaskTemplateRepository = fileUploadTaskTemplateRepository;
            _mapper = mapper;
        }

        public async Task<HTTPResponse<string, string>> CreateTaskTemplate(CreateTaskTemplatePayload payload)
        {
            // check task type is valid            

            var taskTemplate = await _taskTemplateRepository.AddAsync(new TaskTemplateTable() { CreatorAccountId = payload.CreatorAccountId, Name = payload.Name, Description = payload.Description, DateCreated = DateTime.Now, TaskTypeId = payload.TaskTypeId });

            // make sure task type data can be cast to its type
            
            switch(payload.TaskTypeId)
            {
                case UPLOAD_DOCUMENT_TASK_TYPE_ID:
                    var data = JsonSerializer.Deserialize<FileUploadTaskTemplateTable>(payload.TaskTypeData.ToString());
                    data.TaskTemplateId = taskTemplate.TaskTemplateId;
                    if (data == null)
                    {
                        return new HTTPResponse<string, string>() { Success = false, HttpCode = 400, Data = "Invalid task data" };
                    }
                    else
                    {
                        // insert data into file upload table
                        await _fileUploadTaskTemplateRepository.AddAsync(data);
                    }
                    break;
                default:
                    return new HTTPResponse<string, string>() { Success = false, HttpCode = 400, Data = "Invalid task type" };
            }
            
            return new HTTPResponse<string, string>() { Success = true, Data = "Created task template", HttpCode = 200 };
        }

        public async Task<HTTPResponse<List<TaskTemplate>, string>> GetAllTaskTemplates()
        {
            List<TaskTemplate> taskTemplates = _mapper.Map<List<TaskTemplate>>(await _taskTemplateRepository.GetAll());
            // TODO implement getting task type specific data, i.e. checklist items, upload file type, etc
            return new HTTPResponse<List<TaskTemplate>, string>() { Success = true, HttpCode = 200, Data = taskTemplates };
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
                // load task type data
                switch(taskTemplate.TaskTypeId)
                {
                    case UPLOAD_DOCUMENT_TASK_TYPE_ID:
                        taskTemplate.TaskTypeData = await _fileUploadTaskTemplateRepository.GetByTaskTemplateId(taskTemplate.TaskTemplateId);
                        break;
                    default:
                        throw new InvalidOperationException("Invalid task type associated with task template");
                }
            }
            return new HTTPResponse<TaskTemplate, string>() { Success = true, Data = taskTemplate, HttpCode = 200 };
        }
    }

}
