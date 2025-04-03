using Microsoft.Extensions.Logging;
using OnboardingWFMSApi.DataAccess.Repositories.Workflow_Repositories;
using OnboardingWFMSApi.DataModels;
using OnboardingWFMSApi.DataModels.Payloads;
using OnboardingWFMSApi.DataModels.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnboardingWFMSApi.BusinessLogic
{
    public interface IWorkflowInstanceAuditLogic
    {
        public Task<ServerResponse<string, string>> CreateLog(CreateWorkflowInstanceAuditLogPayload payload);
        public Task<HTTPResponse<List<WorkflowInstanceAuditLogTable>, string>> GetAuditLogsByWorkflowInstanceId(string workflowInstanceId);
    }

    public class WorkflowInstanceAuditLogic : IWorkflowInstanceAuditLogic
    {
        private readonly IWorkflowInstanceAuditLogRepository _workflowInstanceAuditLogRepository;

        private readonly ILogger<WorkflowInstanceAuditLogic> _logger;

        public WorkflowInstanceAuditLogic(IWorkflowInstanceAuditLogRepository workflowInstanceAuditLogRepository, ILogger<WorkflowInstanceAuditLogic> logger)
        {
            _workflowInstanceAuditLogRepository = workflowInstanceAuditLogRepository;
            _logger = logger;
        }

        public async Task<ServerResponse<string, string>> CreateLog(CreateWorkflowInstanceAuditLogPayload payload)
        {
            if (string.IsNullOrEmpty(payload.Log))
            {
                return new ServerResponse<string, string>() { Success = false, Error = "A log must be provided" };
            }

            var log = new WorkflowInstanceAuditLogTable() { 
                Id = "", 
                WorkflowInstanceId = payload.WorkflowInstanceId,
                Log = payload.Log,
                Timestamp = DateTime.Now,
                AccountId = payload.AccountId,
            };
            try
            {
                await _workflowInstanceAuditLogRepository.AddAsync(log);
                _logger.LogDebug($"Successfully inserted log for workflow instance {log.WorkflowInstanceId}");
                return new ServerResponse<string, string>() { Success = true };
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to insert log");
                return new ServerResponse<string, string>() { Success = false, Error = "Failed to insert log" };
            }
        }

        public async Task<HTTPResponse<List<WorkflowInstanceAuditLogTable>, string>> GetAuditLogsByWorkflowInstanceId(string workflowInstanceId)
        {
            var logs = await _workflowInstanceAuditLogRepository.GetAuditLogsByWorkflowInstanceId(workflowInstanceId);
            return new HTTPResponse<List<WorkflowInstanceAuditLogTable>, string>() { Success = true, Data = logs, HttpCode = 200 };
        }
    }
}
