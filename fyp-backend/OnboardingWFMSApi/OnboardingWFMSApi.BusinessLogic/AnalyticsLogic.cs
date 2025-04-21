using OnboardingWFMSApi.DataAccess.Repositories;
using OnboardingWFMSApi.DataAccess.Repositories.Workflow_Repositories;
using OnboardingWFMSApi.DataModels;
using OnboardingWFMSApi.DataModels.DTOs;
using System;
using System.Collections.Generic;
using System.Globalization;
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
        public Task<HTTPResponse<OnboardedEmployeesTimelineDTO, string>> GetOnboardedEmployeesTimeline();
        public Task<HTTPResponse<ReportedIssuesAnalyticsDTO, string>> GetReportedIssuesAnalytics(DateTime? from);

    }

    public class AnalyticsLogic : IAnalyticsLogic
    {
        private readonly IWorkflowInstanceRepository _workflowInstanceRepository;
        private readonly IReportedIssueRepository _reportedIssueRepository;

        public AnalyticsLogic(IWorkflowInstanceRepository workflowInstanceRepository, IReportedIssueRepository reportedIssueRepository)
        {
            _workflowInstanceRepository = workflowInstanceRepository;
            _reportedIssueRepository = reportedIssueRepository;
        }

        public async Task<double> GetAverageTimeToOnboard()
        {
            var completedWorkflowInstances = await _workflowInstanceRepository.GetCompletedOnboardingWorkflowInstances(null, null);
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
            var completedWorkflowInstances = await _workflowInstanceRepository.GetCompletedOnboardingWorkflowInstances(from, null);
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

        public async Task<HTTPResponse<OnboardedEmployeesTimelineDTO, string>> GetOnboardedEmployeesTimeline()
        {
            List<OnboardedEmployeesTimelineItem> timeline = new List<OnboardedEmployeesTimelineItem>();
            // get number of employees onboarded by each month of the current year
            int year = DateTime.Now.Year;
            DateTime yearStart = new DateTime(year, 1, 1);
            DateTime monthStart = yearStart;
            for (int i = 0; i < 12; i++)
            {
                var instancesCompleted = await _workflowInstanceRepository.GetCompletedOnboardingWorkflowInstances(monthStart, monthStart.AddMonths(1).Subtract(TimeSpan.FromSeconds(1)));
                var timelineItem = new OnboardedEmployeesTimelineItem(i, DateTimeFormatInfo.CurrentInfo.GetMonthName(i), instancesCompleted.Count);
                timeline.Add(timelineItem);
                monthStart.AddMonths(1);
            }
            return new HTTPResponse<OnboardedEmployeesTimelineDTO, string>() { Success = true, HttpCode = 200, Data = new OnboardedEmployeesTimelineDTO(timeline) };
        }

        public async Task<HTTPResponse<ReportedIssuesAnalyticsDTO, string>> GetReportedIssuesAnalytics(DateTime? from)
        {
            ReportedIssuesAnalyticsDTO analytics = new ReportedIssuesAnalyticsDTO();
            var openTaskIssues = await _reportedIssueRepository.GetAllOpenIssues();
            if (from != null)
            {
                openTaskIssues = openTaskIssues.Where(i => i.IssueLoggedTimestamp > from).ToList();
            }
            analytics.OpenTaskIssues = openTaskIssues.Count;

            return new HTTPResponse<ReportedIssuesAnalyticsDTO, string>() { Success = true, HttpCode = 200, Data = analytics };
        }
    }
}
