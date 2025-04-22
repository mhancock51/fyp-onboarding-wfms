using OnboardingWFMSApi.DataAccess.Repositories.Task_Repositories;
using OnboardingWFMSApi.DataModels;
using OnboardingWFMSApi.DataModels.Tables.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnboardingWFMSApi.BusinessLogic.TaskTemplateHandlers
{
    public interface IFeedbackTaskTemplateHandler : ITaskTemplateHandler
    {

    }

    public class FeedbackTaskTemplateHandler : BaseTaskTemplateHandler<FeedbackTaskTemplateTable>, IFeedbackTaskTemplateHandler
    {
        public FeedbackTaskTemplateHandler(IFeedbackTaskTemplateRepository repository) : base(repository)
        {
        }

        public string GetTaskTypeId()
        {
            return "feedback";
        }

        public override async Task<ServerResponse<string, string>> UpdateTaskTemplateData(object updatedData, object existingData, bool hasActiveInstances)
        {
            // context related checks
            FeedbackTaskTemplateTable updatedFeedbackData = CastObjectToType(updatedData);
            FeedbackTaskTemplateTable feedbackData = CastObjectToType(existingData);
            if (hasActiveInstances)

            if (hasActiveInstances && updatedFeedbackData.Questions.Length != feedbackData.Questions.Length)
            {
                // length changes but has active instances, don't allow this
                return new ServerResponse<string, string>() { Success = false, Error = "Number of items can't be changed" };
            }

            // run base method to do base validation checks and then update record
            return await base.UpdateTaskTemplateData(updatedData, existingData, hasActiveInstances);
        }

        public override async Task<ServerResponse<string, string>> ValidateTaskTemplateData(object taskTypeData)
        {
            FeedbackTaskTemplateTable data = CastObjectToType(taskTypeData);

            // ensure there are questions
            if (data.Questions.Length == 0) return new ServerResponse<string, string>() { Success = false, Error = "At least one question must exist" };
            // ensure questions contain a question and valid likert scale
            foreach(var question in data.Questions)
            {
                if (string.IsNullOrEmpty(question.Question))
                {
                    return new ServerResponse<string, string>() { Success = false, Error = "All likert questions must provide a valid question" };
                }
                if (question.LikertScale.Contains("") || question.LikertScale.Contains(null))
                {
                    return new ServerResponse<string, string>() { Success = false, Error = "Invalid likert scale" };
                }
            }

            return new ServerResponse<string, string>() { Success = true };

        }
    }
}
