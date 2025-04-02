using AutoMapper;
using Microsoft.Extensions.Logging;
using OnboardingWFMSApi.DataAccess.Repositories;
using OnboardingWFMSApi.DataAccess.Repositories.Task_Repositories;
using OnboardingWFMSApi.DataModels;
using OnboardingWFMSApi.DataModels.DTOs;
using OnboardingWFMSApi.DataModels.Payloads;
using OnboardingWFMSApi.DataModels.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnboardingWFMSApi.BusinessLogic
{
    public interface IReportedIssuesLogic
    {
        public Task<HTTPResponse<string, string>> CreateIssue(CreateIssuePayload payload, string accountId);
        public Task<HTTPResponse<List<IssueDTO>, string>> GetAllIssues();
        public Task<IssueDTO> GetIssue(string issueId);
        public Task<HTTPResponse<string, string>> UpdateIssueStatus(UpdateIssueStatusPayload payload, string accountId);
    }
    public class ReportedIssuesLogic : IReportedIssuesLogic
    {
        public const string ISSUE_OPEN_STATUS = "open";
        public const string ISSUE_RESOLVED_STATUS = "resolved";
        public const string ISSUE_CLOSED_STATUS = "closed";

        private readonly ITaskTemplateRepository _taskTemplateRepository;
        private readonly ITaskInstanceRepository _taskInstanceRepository;
        private readonly IReportedIssueRepository _reportedIssueRepository;

        private readonly IAccountLogic _accountLogic;
        private readonly ITaskInstanceLogic _taskInstanceLogic;

        private readonly ILogger<ReportedIssuesLogic> _logger;
        private readonly IMapper _mapper;

        public ReportedIssuesLogic(ITaskTemplateRepository taskTemplateRepository, ITaskInstanceRepository taskInstanceRepository,
            ILogger<ReportedIssuesLogic> logger, IReportedIssueRepository reportedIssueRepository, IMapper mapper, IAccountLogic accountLogic, ITaskInstanceLogic taskInstanceLogic)
        {
            _taskTemplateRepository = taskTemplateRepository;
            _taskInstanceRepository = taskInstanceRepository;
            _logger = logger;
            _reportedIssueRepository = reportedIssueRepository;
            _mapper = mapper;
            _accountLogic = accountLogic;
            _taskInstanceLogic = taskInstanceLogic;
        }

        public async Task<HTTPResponse<string, string>> CreateIssue(CreateIssuePayload payload, string accountId)
        {
            // ensure text is given
            if (string.IsNullOrEmpty(payload.SuggestedChanges))
            {
                return new HTTPResponse<string, string>() { Success = false, HttpCode = 400, Error = "Please provide suggested changes" };
            }
            if (string.IsNullOrEmpty(payload.Description))
            {
                return new HTTPResponse<string, string>() { Success = false, HttpCode = 400, Error = "Please provide a description of the issue" };
            }
            // check task instance exists
            var taskInstance = await _taskInstanceRepository.GetById(payload.TaskInstanceId);
            if (taskInstance == null)
            {
                return new HTTPResponse<string, string>() { Success = false, HttpCode = 400, Error = "Task instance doesn't exist" };
            }
            // ensure account is the assignee
            if (taskInstance.AssigneeAccountId != accountId)
            {
                return new HTTPResponse<string, string>() { Success = false, HttpCode = 400, Error = "Users can only report issues about tasks assigned to themselves" };
            }
            // get the task template
            var taskTemplate = await _taskTemplateRepository.GetById(taskInstance.TaskTemplateId);
            if (taskTemplate == null)
            {
                _logger.LogError($"Task instance ({taskInstance.Id}) isn't associated with a task template");
                return new HTTPResponse<string, string>() { Success = false, HttpCode = 500, Error = "Task instance isn't associated with a task template" };
            }

            // insert the issue
            var issue = new ReportedIssueTable()
            {
                Id = "",
                TaskTemplateId = taskTemplate.Id,
                TaskInstanceId = taskInstance.Id,
                IssueCreatorId = accountId,
                IssueLoggedTimestamp = DateTime.UtcNow,
                Description = payload.Description,
                SuggestedChanges = payload.SuggestedChanges,
                Status = ISSUE_OPEN_STATUS
            };
            try
            {                    
                await _reportedIssueRepository.AddAsync(issue);
                return new HTTPResponse<string, string>() { Success = true, HttpCode = 200, Data = "Successfully created issue" };
            }
            catch (Exception ex)
            {
                return new HTTPResponse<string, string>() { Success = false, HttpCode = 500, Error = "Failed to create issue" };
            }
        }

        public async Task<HTTPResponse<List<IssueDTO>, string>> GetAllIssues()
        {
            var issueIds = (await _reportedIssueRepository.GetAll()).Select(i => i.Id).ToList();
            var issueDTOs = new List<IssueDTO>();
            foreach(var id in issueIds)
            {
                var dto = await GetIssue(id);
                if (dto != null)
                {
                    issueDTOs.Add(dto);
                }
            }
            return new HTTPResponse<List<IssueDTO>, string>() { Success = true, HttpCode = 200, Data = issueDTOs };
        }

        public async Task<IssueDTO> GetIssue(string issueId)
        {
            var issue = await _reportedIssueRepository.GetById(issueId);
            if (issue == null) return null;

            var issueDTO = _mapper.Map<IssueDTO>(issue);
            var creatorAccount = await _accountLogic.GetDirectoryByAccountId(issue.IssueCreatorId);
            if (creatorAccount == null)
            {
                _logger.LogWarning($"Couldn't retrieve account directory DTO for account {issue.IssueCreatorId}");
                return null;
            }                
            issueDTO.IssueCreatorAccount = creatorAccount;

            var taskInstance = (await _taskInstanceLogic.GetTaskInstance(issue.TaskInstanceId)).Data;
            if (taskInstance == null)
            {
                _logger.LogWarning($"Failed to retrieve task instance DTO for instance {issue.TaskInstanceId}");
                return null;
            }
            issueDTO.TaskInstance = taskInstance;
            return issueDTO;
        }

        public async Task<HTTPResponse<string, string>> UpdateIssueStatus(UpdateIssueStatusPayload payload, string accountId)
        {
            // validate status
            if (payload.Status != ISSUE_OPEN_STATUS && payload.Status != ISSUE_RESOLVED_STATUS && payload.Status != ISSUE_CLOSED_STATUS)
            {
                return new HTTPResponse<string, string>() { Success = false, HttpCode = 400, Error = "Invalid status given" };
            }

            // retrieve issue and make sure it exists
            var issue = await _reportedIssueRepository.GetById(payload.IssueId);
            if (issue == null)
            {
                return new HTTPResponse<string, string>() { Success = false, HttpCode = 200, Error = "Issue doesn't exist" };
            }
            // update status
            issue.Status = payload.Status;
            issue.Remark = payload.Remark;
            try
            {
                await _reportedIssueRepository.UpdateAsync(issue);
                return new HTTPResponse<string, string>() { Success = true, HttpCode = 200, Data = "Successfully updated issue" };
            }
            catch(Exception ex)
            {
                _logger.LogWarning("Failed to update issue");
                return new HTTPResponse<string, string>() { Success = false, HttpCode = 500, Data = "Failed to update issue" };
            }
        }
    }
}
