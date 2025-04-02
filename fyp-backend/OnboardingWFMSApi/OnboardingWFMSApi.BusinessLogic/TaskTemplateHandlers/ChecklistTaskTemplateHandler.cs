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

        public string GetTaskTypeId()
        {
            return "checklist";
        }

        public override async Task<ServerResponse<string, string>> UpdateTaskTemplateData(object updatedData, object existingData, bool hasActiveInstances)
        {
            // call method for base checks
            await base.UpdateTaskTemplateData(updatedData, existingData, hasActiveInstances);

            ChecklistTaskTemplateTable updatedChecklistData = CastObjectToType(updatedData);
            ChecklistTaskTemplateTable checklistData = CastObjectToType(existingData);
            if (hasActiveInstances && updatedChecklistData.Items.Length != checklistData.Items.Length)
            {
                // length change but has active instances, don't allow this
                return new ServerResponse<string, string>() { Success = false, Error = "Number of items can't be changed" };
            }
            // check that new version passes validation checks
            var validationResponse = await ValidateTaskTemplateData(updatedChecklistData);
            // passed all checks, update record
            await _repository.UpdateAsync(updatedChecklistData);

            return new ServerResponse<string, string>() { Success = true };
        }
        

        public override async Task<ServerResponse<string, string>> ValidateTaskTemplateData(object taskTypeData)
        {            
            ChecklistTaskTemplateTable checklistTaskData = CastObjectToType(taskTypeData);

            if (checklistTaskData.Items.Length == 0)
            {
                return new ServerResponse<string, string>() { Success = false, Error = "Checklist must include at least one item" };
            }
            foreach(var item in checklistTaskData.Items)
            {
                if (string.IsNullOrEmpty(item))
                {
                    return new ServerResponse<string, string>() { Success = false, Error = "Items can't be empty" };
                }
            }
            return new ServerResponse<string, string>() { Success = true, Data = "Task Type metadata validated successfully" };
        }
    }
}
