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
        public UploadDocumentInstanceHandler(IFileUploadTaskInstanceRepository repository,
            ILogger<UploadDocumentInstanceHandler> logger) : base(repository, logger)
        {            
        }

        public string GetTaskTypeId()
        {
            return "upload-document";
        }

        public override async Task<ServerResponse<string, string>> CreateTaskInstanceData(object taskTemplateMetaData, string taskInstanceId)
        {
            await _repository.AddAsync(new FileUploadTaskInstanceTable() { TaskInstanceId = taskInstanceId, DocumentId = "" });
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
            return new ServerResponse<string, string>() { Success = true };
        }

        public override async Task<ServerResponse<string, string>> ValidateTaskInstanceData(object taskInstanceMetaData)
        {
            // No validation to be done
            return new ServerResponse<string, string>() { Success = true };
        }
    }
}
