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
    public interface IChecklistTaskInstanceHandler : ITaskInstanceHandler
    {
    }

    public class ChecklistTaskInstanceHandler : IChecklistTaskInstanceHandler
    {
        private readonly ILogger<ChecklistTaskInstanceHandler> _logger;

        private readonly IChecklistTaskInstanceRepository _checklistTaskInstanceRepository;

        public ChecklistTaskInstanceHandler(IChecklistTaskInstanceRepository checklistTaskInstanceRepository, ILogger<ChecklistTaskInstanceHandler> logger)
        {
            _checklistTaskInstanceRepository = checklistTaskInstanceRepository;
            _logger = logger;
        }

        public async Task<ServerResponse<object, string>> GetTaskInstanceMetaData(string taskInstanceId)
        {
            var taskInstanceMetaData = await _checklistTaskInstanceRepository.GetByTaskInstanceId(taskInstanceId);
            return taskInstanceMetaData == null ? new ServerResponse<object, string>() { Success = false, Error = "Failed to retrieve task instance metadata" }
                : new ServerResponse<object, string>() { Success = true, Data = taskInstanceMetaData };
        }

        public string GetTaskTypeId()
        {
            return "checklist";
        }

        public async Task<ServerResponse<string, string>> InsertTaskInstanceMetaData(object taskTemplateMetaData, string taskInstanceId)
        {
            var checklistItems = (taskTemplateMetaData as ChecklistTaskTemplateTable).Items;
            await _checklistTaskInstanceRepository.AddAsync(new ChecklistTaskInstanceTable() { ItemCompletionStatuses = new bool[checklistItems.Length], TaskInstanceId = taskInstanceId });
            return new ServerResponse<string, string>() { Success = true };
        }

        public async Task<ServerResponse<string, string>> IsTaskInstanceCompleteable(object taskInstanceMetaData)
        {            
            var taskInstance = taskInstanceMetaData as ChecklistTaskInstanceTable;            
            if (taskInstance == null)
            {
                return new ServerResponse<string, string>() { Success = false, Error = "Failed to cast task instance data" };
            }

            // ensure all checklist items are complete            
            foreach (var itemStatus in taskInstance.ItemCompletionStatuses)
            {
                if (itemStatus == false)
                {
                    return new ServerResponse<string, string>() { Success = false, Error = "Incomplete checklist item" };
                }
            }
            return new ServerResponse<string, string>() { Success = true };
        }

        public async Task<ServerResponse<string, string>> UpdateTaskInstanceMetaData(object updatedTaskInstanceMetaData, object taskTemplateMetaData, string taskInstanceId)
        {
            var taskInstance = updatedTaskInstanceMetaData as ChecklistTaskInstanceTable;
            // validate updated meta data before inserting
            var validationResponse = await ValidateTaskInstanceMetaData(updatedTaskInstanceMetaData, taskTemplateMetaData);
            if (!validationResponse.Success)
            {
                return new ServerResponse<string, string>() { Success = false, Data = validationResponse.Data };
            }
            // update task instance
            var checklistInstance = await _checklistTaskInstanceRepository.GetByTaskInstanceId(taskInstance.Id);
            checklistInstance.ItemCompletionStatuses = taskInstance.ItemCompletionStatuses;
            await _checklistTaskInstanceRepository.UpdateAsync(checklistInstance);
            return new ServerResponse<string, string>() { Success = true };
        }

        public async Task<ServerResponse<string, string>> ValidateTaskInstanceMetaData(object taskInstanceMetaData, object taskTemplateMetaData)
        {
            var taskInstance = taskInstanceMetaData as ChecklistTaskInstanceTable;
            if (taskInstance == null)
            {
                return new ServerResponse<string, string>() { Success = false, Error = "Failed to cast task instance data" };
            }
            var taskTemplate = taskTemplateMetaData as ChecklistTaskTemplateTable;
            if (taskTemplate == null)
            {
                return new ServerResponse<string, string>() { Success = false, Error = "Failed to cast task template data" };
            }

            // ensure the number of items in instance match number in template
            if (taskInstance.ItemCompletionStatuses.Length != taskTemplate.Items.Length)
            {
                return new ServerResponse<string, string>() { Success = false,  Error = "Invalid checklist state" };
            }
            return new ServerResponse<string, string>() { Success = true };
        }
    }
}
