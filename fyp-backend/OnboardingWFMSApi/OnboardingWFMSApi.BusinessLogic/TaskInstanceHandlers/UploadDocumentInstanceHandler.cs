using Microsoft.Extensions.Logging;
using OnboardingWFMSApi.DataAccess.Repositories.Task_Repositories;
using OnboardingWFMSApi.DataModels;
using OnboardingWFMSApi.DataModels.Models;
using OnboardingWFMSApi.DataModels.Tables.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace OnboardingWFMSApi.BusinessLogic.TaskInstanceHandlers
{
    public interface IUploadDocumentInstanceHandler : ITaskInstanceHandler
    {

    }
    public class UploadDocumentInstanceHandler : BaseTaskInstanceHandler<FileUploadTaskInstanceTable>, IUploadDocumentInstanceHandler
    {        
        private readonly IFileUploadTaskTemplateRepository _fileUploadTaskTemplateRepository;

        public UploadDocumentInstanceHandler(IFileUploadTaskInstanceRepository repository, IFileUploadTaskTemplateRepository fileUploadTaskTemplateRepository,
            ILogger<UploadDocumentInstanceHandler> logger) : base(repository, logger)
        {            
            _fileUploadTaskTemplateRepository = fileUploadTaskTemplateRepository;
        }

        public string GetTaskTypeId()
        {
            return "upload-document";
        }

        public override async Task<ServerResponse<string, string>> CreateTaskInstanceData(object taskTemplateMetaData, string taskInstanceId)
        {
            await _repository.AddAsync(new FileUploadTaskInstanceTable() { TaskInstanceId = taskInstanceId, DocumentId = "", UploadedTimestamp = DateTime.MinValue });
            return new ServerResponse<string, string>() { Success = true };
        }

        public override async Task<ServerResponse<string, string>> IsTaskInstanceCompleteable(object taskInstanceMetaData)
        {
            var taskInstance = CastObjectToType(taskInstanceMetaData);
            if (taskInstance == null)
            {
                return new ServerResponse<string, string>() { Success = false, Error = "Failed to cast task instance data" };
            }            
            // ensure document has been uploaded            
            if (taskInstance.DocumentId == "")
            {
                return new ServerResponse<string, string>() { Success = false, Error = "No document uploaded" };
            }
            if (taskInstance.UploadedTimestamp == DateTime.MinValue)
            {
                return new ServerResponse<string, string>() { Success = false, Error = "No document uploaded" };
            }
            return new ServerResponse<string, string>() { Success = true };
        }

        public override async Task<ServerResponse<string, string>> ValidateTaskInstanceData(object taskInstanceMetaData)
        {
            // cast object
            FileUploadTaskInstanceTable taskInstance = CastObjectToType(taskInstanceMetaData);
            if (taskInstance == null)
            {
                return new ServerResponse<string, string>() { Success = false, Error = "Failed to cast task instance data" };
            }
            var taskTemplate = await _fileUploadTaskTemplateRepository.GetByTaskInstanceId(taskInstance.TaskInstanceId);           
            if (taskTemplate == null)
            {
                return new ServerResponse<string, string>() { Success = false, Error = "Failed to cast task template data" };
            }

            // TODO implement validation for future properties

            return new ServerResponse<string, string>() { Success = true };
        }
    }
}
