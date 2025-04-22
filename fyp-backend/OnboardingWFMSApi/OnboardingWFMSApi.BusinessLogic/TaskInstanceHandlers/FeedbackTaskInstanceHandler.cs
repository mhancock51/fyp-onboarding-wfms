using Microsoft.Extensions.Logging;
using OnboardingWFMSApi.DataAccess.Repositories.Task_Repositories;
using OnboardingWFMSApi.DataAccess.Repositories.Task_Repositories.Interfaces;
using OnboardingWFMSApi.DataModels;
using OnboardingWFMSApi.DataModels.Tables.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnboardingWFMSApi.BusinessLogic.TaskInstanceHandlers
{
    public interface IFeedbackTaskInstanceHandler : ITaskInstanceHandler
    {

    }

    public class FeedbackTaskInstanceHandler : BaseTaskInstanceHandler<FeedbackTaskInstanceTable>, IFeedbackTaskInstanceHandler
    {
        public FeedbackTaskInstanceHandler(IFeedbackTaskInstanceRepository repository, ILogger<BaseTaskInstanceHandler<FeedbackTaskInstanceTable>> logger) : base(repository, logger)
        {
        }

        public string GetTaskTypeId()
        {
            return "feedback";
        }

        public override async Task<ServerResponse<string, string>> CreateTaskInstanceData(object taskTemplateMetaData, string taskInstanceId)
        {
            var questions = (taskTemplateMetaData as FeedbackTaskTemplateTable).Questions;
            var emptyResponses = Enumerable.Repeat(-1, questions.Count).ToArray();
            await _repository.AddAsync(new FeedbackTaskInstanceTable() { Id = "", TaskInstanceId = taskInstanceId, Responses = emptyResponses });
            return new ServerResponse<string, string>() { Success = true };
        }

        public override async Task<ServerResponse<string, string>> IsTaskInstanceCompleteable(object taskInstanceMetaData)
        {
            FeedbackTaskInstanceTable taskInstance = CastObjectToType(taskInstanceMetaData);
            if (taskInstance == null)
            {
                return new ServerResponse<string, string>() { Success = false, Error = "Failed to cast task instance data" };
            }

            // ensure all questions are answered
            foreach(var response in taskInstance.Responses)
            {
                if (response == -1) return new ServerResponse<string, string>() { Success = false, Error = "A question hasn't been answered" };
            }

            return new ServerResponse<string, string>() { Success = true };
        }

        public override async Task<ServerResponse<string, string>> ValidateTaskInstanceData(object taskInstanceData)
        {
            // no validation to be done
            return new ServerResponse<string, string>() { Success = true };
        }
    }
}
