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

        public override async Task<ServerResponse<string, string>> ValidateTaskTemplateData(object taskTypeData)
        {            
            ChecklistTaskTemplateTable checklistTaskData = CastObjectToType(taskTypeData);

            if (checklistTaskData.Items.Length == 0)
            {
                return new ServerResponse<string, string>() { Success = false, Error = "Checklist must include at least one item" };
            }
            return new ServerResponse<string, string>() { Success = true, Data = "Task Type metadata validated successfully" };
        }
    }
}
