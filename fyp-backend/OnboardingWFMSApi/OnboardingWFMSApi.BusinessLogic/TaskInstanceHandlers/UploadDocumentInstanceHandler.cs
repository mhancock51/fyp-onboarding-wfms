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
    public class UploadDocumentInstanceHandler : IUploadDocumentInstanceHandler
    {
        private readonly IFileUploadTaskInstanceRepository _fileUploadTaskInstanceRepository;
        private readonly IFileUploadTaskTemplateRepository _fileUploadTaskTemplateRepository;

        public UploadDocumentInstanceHandler(IFileUploadTaskInstanceRepository fileUploadTaskInstanceRepository)
        {
            _fileUploadTaskInstanceRepository = fileUploadTaskInstanceRepository;
        }

        public async Task<ServerResponse<object, string>> GetTaskInstanceMetaData(string taskInstanceId)
        {
            var taskInstanceMetaData = await _fileUploadTaskInstanceRepository.GetByTaskInstanceId(taskInstanceId);
            return taskInstanceMetaData == null ? new ServerResponse<object, string>() { Success = false, Error = "Failed to retrieve task metadata" }
                : new ServerResponse<object, string>() { Success = true, Data = taskInstanceMetaData };
        }

        public string GetTaskTypeId()
        {
            return "upload-document";
        }

        public async Task<ServerResponse<string, string>> InsertTaskInstanceMetaData(object taskTemplateMetaData, string taskInstanceId)
        {
            await _fileUploadTaskInstanceRepository.AddAsync(new FileUploadTaskInstanceTable() { TaskInstanceId = taskInstanceId, DocumentId = "", UploadedTimestamp = DateTime.MinValue });
            return new ServerResponse<string, string>() { Success = true };
        }

        public async Task<ServerResponse<string, string>> IsTaskInstanceCompleteable(object taskInstanceMetaData)
        {
            var taskInstance = taskInstanceMetaData as FileUploadTaskInstanceTable;
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

        public async Task<ServerResponse<string, string>> UpdateTaskInstanceMetaData(object updatedTaskInstanceMetaData)
        {
            // cast object
            JsonElement jsonElement = (JsonElement)updatedTaskInstanceMetaData;
            FileUploadTaskInstanceTable updatedTaskInstance = jsonElement.Deserialize<FileUploadTaskInstanceTable>();

            // validate updated meta data before inserting
            var validationResponse = await ValidateTaskInstanceMetaData(updatedTaskInstanceMetaData);
            if (!validationResponse.Success)
            {
                return new ServerResponse<string, string>() { Success = false, Data = validationResponse.Data };
            }
            // update task instance
            var taskInstance = await _fileUploadTaskInstanceRepository.GetByTaskInstanceId(updatedTaskInstance.Id);           

            await _fileUploadTaskInstanceRepository.UpdateAsync(taskInstance);
            return new ServerResponse<string, string>() { Success = true };
        }

        public async Task<ServerResponse<string, string>> ValidateTaskInstanceMetaData(object taskInstanceMetaData)
        {
            JsonElement jsonElement = (JsonElement)taskInstanceMetaData;
            FileUploadTaskInstanceTable taskInstance = jsonElement.Deserialize<FileUploadTaskInstanceTable>();

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
