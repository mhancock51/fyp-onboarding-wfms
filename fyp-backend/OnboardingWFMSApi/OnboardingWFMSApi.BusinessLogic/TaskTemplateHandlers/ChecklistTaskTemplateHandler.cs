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

namespace OnboardingWFMSApi.BusinessLogic.TaskTemplateHandlers
{
    public interface IChecklistTaskTemplateHandler : ITaskTemplateHandler
    {

    }

    public class ChecklistTaskTemplateHandler : IChecklistTaskTemplateHandler
    {
        private readonly IChecklistTaskTemplateRepository _checklistTaskTemplateRepository;

        public ChecklistTaskTemplateHandler(IChecklistTaskTemplateRepository checklistTaskTemplateRepository)
        {
            _checklistTaskTemplateRepository = checklistTaskTemplateRepository;
        }

        public async Task<ServerResponse<string, string>> CreateTaskTypeMetaData(object taskTypeData, string taskTemplateId)
        {
            if (taskTypeData == null)
            {
                return new ServerResponse<string, string>() { Success = false, Error = "Task type data is null" };
            }

            ChecklistTaskTemplateTable checklistTaskData = JsonSerializer.Deserialize<ChecklistTaskTemplateTable>(taskTypeData.ToString());
            // validate 
            var validationResult = await ValidateTaskTypeMetaData(checklistTaskData);
            if (!validationResult.Success)
            {
                return new ServerResponse<string, string>() { Success = false, Error = validationResult.Error };
            }

            checklistTaskData.TaskTemplateId = taskTemplateId;

            try
            {
                await _checklistTaskTemplateRepository.AddAsync(checklistTaskData);
                return new ServerResponse<string, string>() { Success = true };
            }
            catch (Exception ex) 
            {
                return new ServerResponse<string, string>() { Success = false, Error = "Failed to insert checklist data" };
            }
        }

        public async Task<ServerResponse<object, string>> GetTaskTypeMetaData(string taskTemplateId)
        {
            var checklistTaskData = await _checklistTaskTemplateRepository.GetByTaskTemplateId(taskTemplateId);
            return checklistTaskData == null ?
                new ServerResponse<object, string>() { Success = false, Error = "Failed to retrieve checklist task data" } :
                new ServerResponse<object, string>() { Success = true, Data = checklistTaskData };
        }
        public string GetTaskTypeId()
        {
            return "checklist";
        }

        public async Task<ServerResponse<string, string>> ValidateTaskTypeMetaData(object taskTypeData)
        {
            if (taskTypeData.GetType() != typeof(ChecklistTaskTemplateTable))
            {
                return new ServerResponse<string, string>() { Success = false, Error = "Invalid task type data provided" };
            }

            ChecklistTaskTemplateTable checklistTaskData = taskTypeData as ChecklistTaskTemplateTable;

            if (checklistTaskData.Items.Length == 0)
            {
                return new ServerResponse<string, string>() { Success = false, Error = "Checklist must include at least one item" };
            }
            return new ServerResponse<string, string>() { Success = true, Data = "Task Type metadata validated successfully" };
        }
    }
}
