using OnboardingWFMSApi.DataAccess.Repositories.Task_Repositories;
using OnboardingWFMSApi.DataModels;
using OnboardingWFMSApi.DataModels.Tables.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace OnboardingWFMSApi.BusinessLogic.Handlers.TaskTemplateHandlers
{
    public interface IUploadDocumentTaskTemplateHandler : ITaskTemplateHandler
    {

    }

    public class UploadDocumentTaskTemplateHandler : BaseTaskTemplateHandler<FileUploadTaskTemplateTable>, IUploadDocumentTaskTemplateHandler
    {
        private readonly string[] ALLOWED_FILE_EXTENSIONS =
        {
            ".pdf",
            ".jpeg",
            ".png",
            ".docx",
        };

        public UploadDocumentTaskTemplateHandler(IFileUploadTaskTemplateRepository repository) : base(repository)
        {
        }

        public string GetTaskTypeId()
        {
            return "upload-document";
        }

        public override async Task<ServerResponse<string, string>> ValidateTaskTemplateData(object taskTypeData)
        {
            // cast object           
            FileUploadTaskTemplateTable fileUploadTaskData = CastObjectToType(taskTypeData);

            if (fileUploadTaskData == null)
            {
                return new ServerResponse<string, string>() { Success = false, Error = "Failed to cast task type data" };
            }

            if (string.IsNullOrEmpty(fileUploadTaskData.DocumentName))
            {
                return new ServerResponse<string, string>() { Success = false, Error = "Document name must be set" };
            }
            if (string.IsNullOrEmpty(fileUploadTaskData.SupportedDocumentType))
            {
                return new ServerResponse<string, string>() { Success = false, Error = "At least one supported document type must be provided" };
            }
            // ensure all supported document types are allowed
            foreach (var fileExtension in fileUploadTaskData.SupportedDocumentType.Split(";"))
            {
                if (!ALLOWED_FILE_EXTENSIONS.Contains(fileExtension))
                {
                    return new ServerResponse<string, string>() { Success = false, Error = $"{fileExtension} is not an allowed extension" };
                }
            }

            return new ServerResponse<string, string>() { Success = true, Data = "Task Type metadata validated successfully" };
        }

        public override async Task<ServerResponse<string, string>> UpdateTaskTemplateData(object updatedData, object existingData, bool hasActiveInstances)
        {
            var updatedTaskData = CastObjectToType(updatedData);
            var existingTaskData = CastObjectToType(existingData);

            // No contextual validation to be done

            // run base method to do base validation checks and then update record
            return await base.UpdateTaskTemplateData(updatedData, existingData, hasActiveInstances);
        }
    }
}
