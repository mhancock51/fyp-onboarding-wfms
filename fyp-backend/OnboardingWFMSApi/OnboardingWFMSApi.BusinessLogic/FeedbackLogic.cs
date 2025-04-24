using OnboardingWFMSApi.BusinessLogic.TaskInstanceLogic;
using OnboardingWFMSApi.BusinessLogic.WorkflowInstanceLogic;
using OnboardingWFMSApi.DataAccess.Repositories.Workflow_Repositories;
using OnboardingWFMSApi.DataModels;
using OnboardingWFMSApi.DataModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnboardingWFMSApi.BusinessLogic
{
    public interface IFeedbackLogic
    {
        public Task<HTTPResponse<List<TaskInstanceDTO>, string>> GetFeedbackTaskInstancesByWorkflowTemplate(string workflowTemplateId);
    }
    public class FeedbackLogic : IFeedbackLogic
    {
        private readonly IWorkflowInstanceRepository _workflowInstanceRepository;
        private readonly ITaskInstanceLogic _taskInstanceLogic;

        public FeedbackLogic(IWorkflowInstanceRepository workflowInstanceRepository, ITaskInstanceLogic taskInstanceLogic)
        {
            _workflowInstanceRepository = workflowInstanceRepository;
            _taskInstanceLogic = taskInstanceLogic;
        }

        public async Task<HTTPResponse<List<TaskInstanceDTO>, string>> GetFeedbackTaskInstancesByWorkflowTemplate(string workflowTemplateId)
        {
            // find workflow instance associated with workflow template
            var workflowInstances = await _workflowInstanceRepository.GetInstancesByTemplateId(workflowTemplateId);
            var feedbackTaskInstances = new List<TaskInstanceDTO>();
            foreach(var workflowInstance in workflowInstances)
            {
                // find feedback task instances in workflow instance
                var response = await _taskInstanceLogic.GetWorkflowInstanceCompletedFeedbackTasks(workflowInstance.Id);
                if (response.HasData)
                {
                    feedbackTaskInstances.AddRange(response.Data);
                }
            }
            return new HTTPResponse<List<TaskInstanceDTO>, string>() { Success = true, HttpCode = 200, Data = feedbackTaskInstances };
        }
    }
}
