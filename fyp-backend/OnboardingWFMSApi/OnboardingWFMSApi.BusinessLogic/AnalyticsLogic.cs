using OnboardingWFMSApi.DataAccess.Repositories.Workflow_Repositories;
using OnboardingWFMSApi.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnboardingWFMSApi.BusinessLogic
{
    public interface IAnalyticsLogic
    {
        public Task<HTTPResponse<int, string>> GetCompletedWorkflowInstances(DateTime? from);
        public Task<HTTPResponse<int, string>> GetOpenWorkflowInstances();
        public Task<HTTPResponse<double, string>> GetAverageTimeToOnboard();
    }

    public class AnalyticsLogic : IAnalyticsLogic
    {
        private readonly IWorkflowInstanceRepository _workflowInstanceRepository;

        public AnalyticsLogic(IWorkflowInstanceRepository workflowInstanceRepository)
        {
            _workflowInstanceRepository = workflowInstanceRepository;
        }

        public async Task<HTTPResponse<double, string>> GetAverageTimeToOnboard()
        {
            var completedWorkflowInstances = await _workflowInstanceRepository.GetCompletedWorkflowInstances();
            var totalDays = 0;            
            foreach(var instance in completedWorkflowInstances)
            {
                if (instance.CompletionTimestamp != null) 
                {
                    var span = instance.CompletionTimestamp.Value.Subtract(instance.CreationTimestamp);
                    totalDays += span.Days;
                }
            }
            var average = (double)totalDays / completedWorkflowInstances.Count;
            average = Math.Truncate(100 * average) / 100;
            return new HTTPResponse<double, string>() { Success = true, HttpCode = 200, Data = average };

        }

        public async Task<HTTPResponse<int, string>> GetCompletedWorkflowInstances(DateTime? from)
        {
            var completedWorkflowInstances = await _workflowInstanceRepository.GetCompletedWorkflowInstances();
            if (from != null)
            {
                // get the workflow instance completed in the last month, week, etc
                completedWorkflowInstances = completedWorkflowInstances.Where(i => i.CompletionTimestamp > from).ToList();
            }
            return new HTTPResponse<int, string>() { Success = true, HttpCode = 200, Data = completedWorkflowInstances.Count };
        }

        public async Task<HTTPResponse<int, string>> GetOpenWorkflowInstances()
        {
            var openWorkflowInstances = await _workflowInstanceRepository.GetOpenWorkflowInstances();
            return new HTTPResponse<int, string>() { Success = true, HttpCode = 200, Data = openWorkflowInstances.Count };
        }
    }
}
