using OnboardingWFMSApi.DataAccess.Repositories.Workflow_Repositories;
using OnboardingWFMSApi.DataModels;
using OnboardingWFMSApi.DataModels.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnboardingWFMSApi.BusinessLogic
{
    public interface IAnalyticsLogic
    {
        public Task<int> GetCompletedOnboardingWorkflowInstances(DateTime? from);
        public Task<int> GetOpenOnboardingWorkflowInstances();
        public Task<double> GetAverageTimeToOnboard();

        public Task<HTTPResponse<OnboardingAnalyticsDTO, string>> GetOnboardingAnalytics(DateTime? from);
    }

    public class AnalyticsLogic : IAnalyticsLogic
    {
        private readonly IWorkflowInstanceRepository _workflowInstanceRepository;

        public AnalyticsLogic(IWorkflowInstanceRepository workflowInstanceRepository)
        {
            _workflowInstanceRepository = workflowInstanceRepository;
        }

        public async Task<double> GetAverageTimeToOnboard()
        {
            var completedWorkflowInstances = await _workflowInstanceRepository.GetCompletedOnboardingWorkflowInstances();
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
            return average;
        }

        public async Task<int> GetCompletedOnboardingWorkflowInstances(DateTime? from)
        {
            var completedWorkflowInstances = await _workflowInstanceRepository.GetCompletedOnboardingWorkflowInstances();
            if (from != null)
            {
                // get the workflow instance completed in the last month, week, etc
                completedWorkflowInstances = completedWorkflowInstances.Where(i => i.CompletionTimestamp > from).ToList();
            }
            return completedWorkflowInstances.Count;
        }

        public async Task<int> GetOpenOnboardingWorkflowInstances()
        {
            var openWorkflowInstances = await _workflowInstanceRepository.GetOpenOnboardingWorkflowInstances();
            return openWorkflowInstances.Count;
        }

        public async Task<HTTPResponse<OnboardingAnalyticsDTO, string>> GetOnboardingAnalytics(DateTime? from)
        {
            var averageTimeToOnboard = await GetAverageTimeToOnboard();
            var completedOnboardingInstances = await GetCompletedOnboardingWorkflowInstances(from);
            var openOnbordingInstances = await GetOpenOnboardingWorkflowInstances();

            OnboardingAnalyticsDTO workflowInstanceAnalytics = new OnboardingAnalyticsDTO(averageTimeToOnboard, openOnbordingInstances, completedOnboardingInstances);
            return new HTTPResponse<OnboardingAnalyticsDTO, string>() { Success = true, Data = workflowInstanceAnalytics, HttpCode = 200 };            
        }
    }
}
