using Microsoft.Extensions.Logging;
using OnboardingWFMSApi.DataAccess.Repositories.Task_Repositories.Interfaces;
using OnboardingWFMSApi.DataModels;
using OnboardingWFMSApi.DataModels.Tables.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnboardingWFMSApi.BusinessLogic.Handlers.TaskInstanceHandlers
{
    public interface IDecisionTaskInstanceHandler : ITaskInstanceHandler
    {

    }
    public class DecisionTaskInstanceHandler : BaseTaskTemplateHandler<DecisionTaskInstanceTable>, IDecisionTaskInstanceHandler
    {
        public DecisionTaskInstanceHandler(ITaskTypeInstanceRepository<DecisionTaskInstanceTable> repository, ILogger<BaseTaskTemplateHandler<DecisionTaskInstanceTable>> logger) : base(repository, logger)
        {
        }

        public override async Task<ServerResponse<string, string>> CreateTaskInstanceData(object taskTemplateMetaData, string taskInstanceId)
        {
            await _repository.AddAsync(new DecisionTaskInstanceTable() { Id = "", Answer = null, TaskInstanceId = taskInstanceId });
            return new ServerResponse<string, string>() { Success = true };
        }

        public string GetTaskTypeId()
        {
            return "decision";
        }

        public override async Task<ServerResponse<string, string>> IsTaskInstanceCompleteable(object taskInstanceMetaData)
        {
            DecisionTaskInstanceTable decisionInstance = CastObjectToType(taskInstanceMetaData);
            if (decisionInstance.Answer == null || (decisionInstance.Answer != 1 && decisionInstance.Answer != 2))
            {
                return new ServerResponse<string, string>() { Success = false, Error = "Answer not provided" };
            }
            return new ServerResponse<string, string>() { Success = true };
        }

        public override async Task<ServerResponse<string, string>> ValidateTaskInstanceData(object taskInstanceData)
        {
            // cast object
            DecisionTaskInstanceTable taskInstance = CastObjectToType(taskInstanceData);
            if (taskInstance == null) return new ServerResponse<string, string>() { Success = false, Error = "Failed to cast task instance data" };

            // no validation to be done
            return new ServerResponse<string, string>() { Success = true };
        }
    }
}
