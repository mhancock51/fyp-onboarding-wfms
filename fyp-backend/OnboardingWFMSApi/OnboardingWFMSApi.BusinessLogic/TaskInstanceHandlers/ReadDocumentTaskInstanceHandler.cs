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
    public interface IReadDocumentTaskInstanceHandler : ITaskInstanceHandler
    {

    }

    public class ReadDocumentTaskInstanceHandler : IReadDocumentTaskInstanceHandler
    {
        private readonly IReadDocumentTaskInstanceRepository _readDocumentTaskInstanceRepository;

        public ReadDocumentTaskInstanceHandler(IReadDocumentTaskInstanceRepository readDocumentTaskInstanceRepository)
        {
            _readDocumentTaskInstanceRepository = readDocumentTaskInstanceRepository;
        }

        public async Task<ServerResponse<object, string>> GetTaskInstanceMetaData(string taskInstanceId)
        {
            var taskInstanceMetaData = await _readDocumentTaskInstanceRepository.GetByTaskInstanceId(taskInstanceId);
            return taskInstanceMetaData == null ? new ServerResponse<object, string>() { Success = false, Error = "Failed to retrieve task metadata" }
                : new ServerResponse<object, string>() { Success = true, Data = taskInstanceMetaData };
        }

        public string GetTaskTypeId()
        {
            return "read-document";
        }

        public async Task<ServerResponse<string, string>> InsertTaskInstanceMetaData(object taskTemplateMetaData, string taskInstanceId)
        {
            await _readDocumentTaskInstanceRepository.AddAsync(new ReadDocumentTaskInstanceTable() { TaskInstanceId = taskInstanceId });
            return new ServerResponse<string, string>() { Success = true };
        }

        public async Task<ServerResponse<string, string>> IsTaskInstanceCompleteable(object taskInstanceMetaData)
        {
            var taskInstance = JsonSerializer.Deserialize<ReadDocumentTaskInstanceTable>(taskInstanceMetaData.ToString());
            if (taskInstance == null)
            {
                return new ServerResponse<string, string>() { Success = false, Error = "Failed to cast task instance data" };
            }

            // ensure document has been opened and checkbox has been checked
            if (!taskInstance.CheckboxChecked)
            {
                return new ServerResponse<string, string>() { Success = false, Error = "Checkbox must be checked" };
            }
            if (!taskInstance.LinkClicked)
            {
                return new ServerResponse<string, string>() { Success = false, Error = "Link must have been clicked" };
            }

            return new ServerResponse<string, string>() { Success = true };
        }

        public async Task<ServerResponse<string, string>> UpdateTaskInstanceMetaData(object updatedTaskInstanceMetaData, object taskTemplateMetaData, string taskInstanceId)
        {
            var updatedTaskInstance = JsonSerializer.Deserialize<ReadDocumentTaskInstanceTable>(updatedTaskInstanceMetaData.ToString());
            // validate update meta data
            var validationResponse = await ValidateTaskInstanceMetaData(updatedTaskInstanceMetaData, taskTemplateMetaData);
            if (!validationResponse.Success)
            {
                return new ServerResponse<string, string>() { Success = false, Data = validationResponse.Data };
            }
            // update task instance
            var checklistInstance = await _readDocumentTaskInstanceRepository.GetByTaskInstanceId(updatedTaskInstance.Id);
            checklistInstance.LinkClicked = updatedTaskInstance.LinkClicked;
            checklistInstance.CheckboxChecked = updatedTaskInstance.CheckboxChecked;
            await _readDocumentTaskInstanceRepository.UpdateAsync(checklistInstance);
            
            return new ServerResponse<string, string>() { Success = true };
        }

        public async Task<ServerResponse<string, string>> ValidateTaskInstanceMetaData(object taskInstanceMetaData, object taskTemplateMetaData)
        {
            var taskInstance = JsonSerializer.Deserialize<ReadDocumentTaskInstanceTable>(taskInstanceMetaData.ToString());
            if (taskInstance == null)
            {
                return new ServerResponse<string, string>() { Success = false, Error = "Failed to cast task instance data" };
            }
            var taskTemplate = JsonSerializer.Deserialize<ReadDocumentTaskTemplateTable>(taskTemplateMetaData.ToString());
            if (taskTemplate == null)
            {
                return new ServerResponse<string, string>() { Success = false, Error = "Failed to cast task template data" };
            }

            // TODO implement validation

            return new ServerResponse<string, string>() { Success = true };
        }
    }
}
