using Microsoft.Extensions.Logging;
using OnboardingWFMSApi.BusinessLogic.Handlers.TaskInstanceHandlers;
using OnboardingWFMSApi.DataAccess.Repositories.Task_Repositories;
using OnboardingWFMSApi.DataAccess.Repositories.Task_Repositories.Interfaces;
using OnboardingWFMSApi.DataModels;
using OnboardingWFMSApi.DataModels.Tables.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnboardingWFMSApi.BusinessLogic.Handlers.TaskTemplateHandlers
{
    public interface IDecisionTaskTemplateHandler : ITaskTemplateHandler
    {

    }

    public class DecisionTaskTemplateHandler : BaseTaskTemplateHandler<DecisionTaskTemplateTable>, IDecisionTaskTemplateHandler
    {
        public DecisionTaskTemplateHandler(ITaskTemplateRepository<DecisionTaskTemplateTable> repository) : base(repository)
        {
        }

        public override async Task<ServerResponse<string, string>> UpdateTaskTemplateData(object updatedData, object existingData, bool hasActiveInstances)
        {
            // context related checks
            DecisionTaskTemplateTable updatedDecisionData = CastObjectToType(updatedData);
            DecisionTaskTemplateTable decisionData = CastObjectToType(existingData);
            
            // run base method to do base validation checks and then update record
            return await base.UpdateTaskTemplateData(updatedData, existingData, hasActiveInstances);
        }

        public string GetTaskTypeId()
        {
            return "decision";
        }

        public override async Task<ServerResponse<string, string>> ValidateTaskTemplateData(object taskTypeData)
        {
            DecisionTaskTemplateTable decisionData = CastObjectToType(taskTypeData);
            if (decisionData == null) return new ServerResponse<string, string>() { Success = false, Error = "Failed to cast task template" };

            if (string.IsNullOrEmpty(decisionData.Question)) return new ServerResponse<string, string>() { Success = false, Error = "Question must be provided" };

            if (string.IsNullOrEmpty(decisionData.AnswerA)) return new ServerResponse<string, string>() { Success = false, Error = "Answer A must be provided" };

            if (string.IsNullOrEmpty(decisionData.AnswerB)) return new ServerResponse<string, string>() { Success = false, Error = "Answer B must be provided" };

            return new ServerResponse<string, string>() { Success = true };
        }
    }
}
