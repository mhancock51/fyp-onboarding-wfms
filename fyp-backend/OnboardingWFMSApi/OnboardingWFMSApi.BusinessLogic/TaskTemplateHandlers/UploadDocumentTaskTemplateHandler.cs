using OnboardingWFMSApi.DataAccess.Repositories.Task_Repositories;
using OnboardingWFMSApi.DataModels;
using OnboardingWFMSApi.DataModels.Tables.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace OnboardingWFMSApi.BusinessLogic.TaskTemplateHandlers
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

        public async Task<ServerResponse<string, string>> CreateTaskTypeMetaData(object taskTypeData, string taskTemplateId)
        {
            if (taskTypeData == null)
            {
                return new ServerResponse<string, string>() { Success = false, Error = "Task type data is null" };
            }
            // cast object
            FileUploadTaskTemplateTable fileUploadTaskData = CastObjectToType(taskTypeData);

            // validate
            var validationResult = await ValidateTaskTypeData(taskTypeData);
            if (!validationResult.Success)
            {
                return new ServerResponse<string, string>() { Success = false, Error = validationResult.Error };
            }

            fileUploadTaskData.TaskTemplateId = taskTemplateId;
            try
            {
                await _repository.AddAsync(fileUploadTaskData);                
                return new ServerResponse<string, string>() { Success = true };
            }
            catch (Exception ex)
            {
                return new ServerResponse<string, string>() { Success = false, Error = "Failed to insert checklist data" };
            }
        }

        public string GetTaskTypeId()
        {
            return "upload-document";
        }

        public override async Task<ServerResponse<string, string>> ValidateTaskTypeData(object taskTypeData)
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
    }
}
