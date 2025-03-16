using Microsoft.Extensions.Logging;
using OnboardingWFMSApi.DataAccess.Repositories.Workflow_Repositories;
using OnboardingWFMSApi.DataModels;
using OnboardingWFMSApi.DataModels.DTOs;
using OnboardingWFMSApi.DataModels.Payloads;
using OnboardingWFMSApi.DataModels.Tables.Workflows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnboardingWFMSApi.BusinessLogic
{
    public interface IWorkflowInstanceLogic
    {
        public Task<HTTPResponse<string, string>> CreateWorkflowInstance(CreateWorkflowInstancePayload payload);
    }
    public class WorkflowInstanceLogic : IWorkflowInstanceLogic
    {
        private readonly ILogger<WorkflowInstanceLogic> _logger;

        private readonly IWorkflowTemplateLogic _workflowTemplateLogic;
        private readonly ITaskInstanceLogic _taskInstanceLogic;

        private readonly IWorkflowInstanceRepository _workflowInstanceRepository;

        public WorkflowInstanceLogic(IWorkflowInstanceRepository workflowInstanceRepository, IWorkflowTemplateLogic workflowTemplateLogic, ITaskInstanceLogic taskInstanceLogic, ILogger<WorkflowInstanceLogic> logger)
        {
            _workflowInstanceRepository = workflowInstanceRepository;
            _workflowTemplateLogic = workflowTemplateLogic;
            _taskInstanceLogic = taskInstanceLogic;
            _logger = logger;
        }

        public async Task<HTTPResponse<string, string>> CreateWorkflowInstance(CreateWorkflowInstancePayload payload)
        {
            // make sure an instance with the same template Id and account Ids don't exists
            var workflowInstances = await _workflowInstanceRepository.GetInstancesByTemplateId(payload.workflowTeamplateId);
            if (workflowInstances.FirstOrDefault(i => i.SupervisorAccountId == payload.supervisorAccountId && i.OnboarderAccountId == payload.onboarderAccountId) != null)
            {
                return new HTTPResponse<string, string>() { Success = false, HttpCode = 400, Error = "A similar workflow instance already exists" };
            }
            // fetch workflow template - make sure it exists
            var workflowTemplate = (await _workflowTemplateLogic.GetWorkflowTemplate(payload.workflowTeamplateId)).Data ?? null;
            if (workflowTemplate == null)
            {
                return new HTTPResponse<string, string>() { Success = false, HttpCode = 400, Error = "Workflow template doesn't exist" };
            }
            
            if (workflowTemplate.IsOnboardingWF && (payload.onboarderAccountId == null || payload.supervisorAccountId == null))
            {
                return new HTTPResponse<string, string>() { Success = false, HttpCode = 400, Error = "Onboarder and Supervisor account must be selected" };
            }

            // TODO: ensure supervisor account is active            

            var instance = new WorkflowInstanceTable()
            {
                Id = "",
                WorkflowTemplateId = payload.workflowTeamplateId,
                OnboarderAccountId = payload.onboarderAccountId ?? "",
                SupervisorAccountId = payload.supervisorAccountId ?? "",
                CreationTimestamp = DateTime.UtcNow,
            };
            instance = await _workflowInstanceRepository.AddAsync(instance);

            // assign first tasks (ones with no dependencies in the preflow (if onboarding or mainflow))
            var tasks = new List<WorkflowTemplateNodeDTO>();
            if (workflowTemplate.IsOnboardingWF)
            {
                // find preflow tasks with no dependencies
                tasks = workflowTemplate.PreflowTasks.Where(n => n.DependencyTaskTemplateIds.Count == 0).ToList();
            }
            else
            {
                tasks = workflowTemplate.MainflowTasks.Where(n => n.DependencyTaskTemplateIds.Count == 0).ToList();
            }
            // assign tasks
            foreach (var task in tasks) 
            {
                var result = await _taskInstanceLogic.CreateInstance(
                    new CreateTaskInstancePayload() { TaskTemplateId = task.TaskTemplateId, AssigneeAccountId = task.AssigneeId, AssignerAccountId = instance.SupervisorAccountId }
                );
                if (!result.Success)
                {
                    _logger.LogWarning($"Failed to instantiate task instance for workflow, error: {result.Error}");
                }
            }

            return new HTTPResponse<string, string>() { Success = true, HttpCode = 400, Data = "Successfully instantiated workflow instance" };
        }
    }
}
