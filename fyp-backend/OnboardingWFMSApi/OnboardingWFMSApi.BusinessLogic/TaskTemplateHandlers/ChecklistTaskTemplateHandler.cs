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

    public class ChecklistTaskTemplateHandler : BaseTaskTemplateHandler<ChecklistTaskTemplateTable>, IChecklistTaskTemplateHandler
    {
        public ChecklistTaskTemplateHandler(IChecklistTaskTemplateRepository repository) : base(repository)
        {

        }

        public async Task<ServerResponse<string, string>> CreateTaskTypeMetaData(object taskTypeData, string taskTemplateId)
        {
            if (taskTypeData == null)
            {
                return new ServerResponse<string, string>() { Success = false, Error = "Task type data is null" };
            }
            // cast object
            ChecklistTaskTemplateTable checklistTaskData = CastObjectToType(taskTypeData);
            // validate 
            var validationResult = await ValidateTaskTypeMetaData(checklistTaskData);
            if (!validationResult.Success)
            {
                return new ServerResponse<string, string>() { Success = false, Error = validationResult.Error };
            }

            checklistTaskData.TaskTemplateId = taskTemplateId;
            try
            {
                await _repository.AddAsync(checklistTaskData);
                return new ServerResponse<string, string>() { Success = true };
            }
            catch (Exception ex) 
            {
                return new ServerResponse<string, string>() { Success = false, Error = "Failed to insert checklist data" };
            }
        }

        public string GetTaskTypeId()
        {
            return "checklist";
        }

        public async Task<ServerResponse<string, string>> ValidateTaskTypeMetaData(object taskTypeData)
        {            
            ChecklistTaskTemplateTable checklistTaskData = CastObjectToType(taskTypeData);

            if (checklistTaskData.Items.Length == 0)
            {
                return new ServerResponse<string, string>() { Success = false, Error = "Checklist must include at least one item" };
            }
            return new ServerResponse<string, string>() { Success = true, Data = "Task Type metadata validated successfully" };
        }

        public ChecklistTaskTemplateTable CastObjectToType(object taskTypeData)
        {
            var checklistTaskData = taskTypeData as ChecklistTaskTemplateTable;
            if (checklistTaskData == null)
            {
                JsonElement jsonElement = (JsonElement)taskTypeData;
                checklistTaskData = jsonElement.Deserialize<ChecklistTaskTemplateTable>();
            }
            return checklistTaskData;
        }
    }
}
