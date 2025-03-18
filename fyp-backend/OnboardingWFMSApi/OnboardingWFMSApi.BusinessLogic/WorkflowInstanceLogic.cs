using AutoMapper;
using Microsoft.Extensions.Logging;
using OnboardingWFMSApi.DataAccess.Repositories.Workflow_Repositories;
using OnboardingWFMSApi.DataModels;
using OnboardingWFMSApi.DataModels.DTOs;
using OnboardingWFMSApi.DataModels.Models;
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
        public Task<HTTPResponse<WorkflowInstanceDTO, string>> GetWorkflowInstance(string workflowInstanceId);
        public Task<HTTPResponse<string, string>> HandleTaskInstanceCompletion(TaskInstance taskInstance);
        public Task<HTTPResponse<string, string>> HandleOnboarderRegistration(string accountId, string emailAddress);
        public Task<HTTPResponse<List<WorkflowInstanceDTO>, string>> GetAccountsWorkflowInstances(string accountId);
    }
    public class WorkflowInstanceLogic : IWorkflowInstanceLogic
    {
        public const string WORKFLOW_INSTANCE_PREFLOW_STATUS = "PREFLOW";
        public const string WORKFLOW_INSTANCE_MAINFLOW_STATUS = "MAINFLOW";
        public const string WORKFLOW_INSTANCE_COMPLETE_STATUS = "COMPLETE";

        private readonly ILogger<WorkflowInstanceLogic> _logger;
        private readonly IMapper _mapper;
        private readonly IUtility _utility;

        private readonly IWorkflowTemplateLogic _workflowTemplateLogic;
        private readonly ITaskInstanceLogic _taskInstanceLogic;

        private readonly IWorkflowInstanceRepository _workflowInstanceRepository;

        public WorkflowInstanceLogic(IWorkflowInstanceRepository workflowInstanceRepository, IWorkflowTemplateLogic workflowTemplateLogic, ITaskInstanceLogic taskInstanceLogic, ILogger<WorkflowInstanceLogic> logger, IMapper mapper, IUtility utility)
        {
            _workflowInstanceRepository = workflowInstanceRepository;
            _workflowTemplateLogic = workflowTemplateLogic;
            _taskInstanceLogic = taskInstanceLogic;
            _logger = logger;
            _mapper = mapper;
            _utility = utility;
        }

        public async Task<HTTPResponse<string, string>> CreateWorkflowInstance(CreateWorkflowInstancePayload payload)
        {
            // make sure an instance with the same template Id and account Ids don't exists
            var workflowInstances = await _workflowInstanceRepository.GetInstancesByTemplateId(payload.workflowTeamplateId);
            if (workflowInstances.FirstOrDefault(i => i.SupervisorAccountId == payload.supervisorAccountId && i.OnboarderEmailAddress == payload.onboarderEmailAddress) != null)
            {
                return new HTTPResponse<string, string>() { Success = false, HttpCode = 400, Error = "A similar workflow instance already exists" };
            }
            // fetch workflow template - make sure it exists
            var workflowTemplate = (await _workflowTemplateLogic.GetWorkflowTemplate(payload.workflowTeamplateId)).Data ?? null;
            if (workflowTemplate == null)
            {
                return new HTTPResponse<string, string>() { Success = false, HttpCode = 400, Error = "Workflow template doesn't exist" };
            }
            
            if (workflowTemplate.IsOnboardingWF && (payload.onboarderEmailAddress == null || payload.supervisorAccountId == null))
            {
                return new HTTPResponse<string, string>() { Success = false, HttpCode = 400, Error = "Onboarder and Supervisor account must be selected" };
            }

            // TODO: ensure supervisor account is active            

            var instance = new WorkflowInstanceTable()
            {
                Id = "",
                WorkflowTemplateId = payload.workflowTeamplateId,
                OnboarderAccountId = null,
                OnboarderEmailAddress = payload.onboarderEmailAddress,
                SupervisorAccountId = payload.supervisorAccountId,
                CreationTimestamp = DateTime.UtcNow,
            };
            instance = await _workflowInstanceRepository.AddAsync(instance);

            // assign first tasks (ones with no dependencies in the preflow (if onboarding or mainflow))
            var nodeswithNoDependencies = new List<WorkflowTemplateNodeDTO>();
            if (workflowTemplate.IsOnboardingWF)
            {
                // find preflow tasks with no dependencies
                nodeswithNoDependencies = workflowTemplate.PreflowTasks.Where(n => n.DependencyNodeIds.Count == 0).ToList();
            }
            else
            {
                nodeswithNoDependencies = workflowTemplate.MainflowTasks.Where(n => n.DependencyNodeIds.Count == 0).ToList();
            }
            // assign tasks
            foreach (var node in nodeswithNoDependencies) 
            {
                var result = await _taskInstanceLogic.CreateInstance(
                    new CreateTaskInstancePayload() { 
                        TaskTemplateId = node.TaskTemplateId,
                        AssigneeAccountId = _utility.ReplaceAccountIdPlaceholder(node.AssigneeId, instance),
                        AssignerAccountId = _utility.ReplaceAccountIdPlaceholder(instance.SupervisorAccountId, instance),
                        WorkflowInstanceId = instance.Id,
                        WorkflowNodeId = node.Id
                    }
                );
                if (!result.Success)
                {
                    _logger.LogWarning($"Failed to instantiate task instance for workflow, error: {result.Error}");
                }
            }

            return new HTTPResponse<string, string>() { Success = true, HttpCode = 400, Data = "Successfully instantiated workflow instance" };
        }

        public async Task<HTTPResponse<WorkflowInstanceDTO, string>> GetWorkflowInstance(string workflowInstanceId)
        {
            var workflowInstance = await _workflowInstanceRepository.GetById(workflowInstanceId);
            if (workflowInstance == null)
            {
                return new HTTPResponse<WorkflowInstanceDTO, string>() { Success = false, HttpCode = 400, Error = "Workflow instance doesn't exist" };
            }
            var workflowInstanceDTO = _mapper.Map<WorkflowInstanceDTO>(workflowInstance);
            // retrieve workflow template DTO
            var result = await _workflowTemplateLogic.GetWorkflowTemplate(workflowInstance.WorkflowTemplateId);
            if (!result.Success || !result.HasData)
            {
                return new HTTPResponse<WorkflowInstanceDTO, string>() { Success = false, HttpCode = 500, Error = "Failed to retrieve workflow template" };
            }
            // retrieve the number of completed tasks
            workflowInstanceDTO.CompletedTasks = (await _taskInstanceLogic.GetTaskInstancesByWorkflowInstance(workflowInstance.Id)).Where(i => i.Status == TaskInstanceLogic.COMPLETED_TASK_STATUS).Count();

            workflowInstanceDTO.Status = await GetWorkflowInstanceStatus(workflowInstanceDTO);

            return new HTTPResponse<WorkflowInstanceDTO, string>() { Success = false, HttpCode = 200, Data = workflowInstanceDTO };
        }       

        public async Task<HTTPResponse<string, string>> HandleTaskInstanceCompletion(TaskInstance taskInstance)
        {
            _logger.LogDebug($"Handling task completion event for task instance: {taskInstance.Id} ({taskInstance.template.Name}, {taskInstance.AssigneeAccountId})");
            if (string.IsNullOrEmpty(taskInstance.WorkflowInstanceId))
            {
                throw new Exception("Task instance isn't associated to a workflow instance");                
            }

            var workflowInstance = (await GetWorkflowInstance(taskInstance.WorkflowInstanceId)).Data ?? null;
            if (workflowInstance == null)
            {
                return new HTTPResponse<string, string>() { Success = false, HttpCode = 500, Error = "Failed to retrieve workflow instance" };
            }

            if (workflowInstance.WorkflowTemplate.IsOnboardingWF)
            {
                // check if all preflow tasks have been completed
                var isPreflowSectionComplete = await AreAllTasksInWorkflowSectionComplete(workflowInstance.WorkflowTemplate.PreflowTasks, workflowInstance.Id);
                if (isPreflowSectionComplete)
                {
                    // TODO: invite onboarder (check onboarder hasn't already been invited) - once onboarder has registed, mainflow tasks should begin                    

                    //return new HTTPResponse<string, string>() { Success = true, HttpCode = 200, Data = "Onboarder invited" };
                }
            }

            // find associated workflow node
            var node = workflowInstance.WorkflowTemplate.PreflowTasks.Concat(workflowInstance.WorkflowTemplate.MainflowTasks)
                .FirstOrDefault(n => n.TaskTemplateId == taskInstance.TaskTemplateId);  

            // ASSIGN TASKS THAT WERE DEPENDENT ON THIS TASK AND ARE NOW SATISFIED
            // get all nodes in workflow dependent on this node, if the nodes's dependencies are now statisfied, assign it 
            var dependentNodes = workflowInstance.WorkflowTemplate.PreflowTasks.Concat(workflowInstance.WorkflowTemplate.MainflowTasks)
                                    .Where(i => i.DependencyNodeIds.Contains(node.Id))
                                    .ToList();                        
            foreach(var dependentNode in dependentNodes)
            {
                if (await AreNodesDependenciesSatisfied(dependentNode, workflowInstance, node.Id))
                {
                    // all dependencies have been satisfied, instantiate this task
                    await _taskInstanceLogic.CreateInstance(
                        new CreateTaskInstancePayload()
                        {
                            TaskTemplateId = dependentNode.TaskTemplateId,
                            AssigneeAccountId = _utility.ReplaceAccountIdPlaceholder(dependentNode.AssigneeId, workflowInstance),
                            AssignerAccountId = _utility.ReplaceAccountIdPlaceholder(workflowInstance.SupervisorAccountId, workflowInstance),
                            WorkflowInstanceId = workflowInstance.Id,
                            WorkflowNodeId = dependentNode.Id,
                        }
                    );
                }
            }
            return new HTTPResponse<string, string>() { Success = true, HttpCode = 200 };
        }

        private async Task<bool> AreNodesDependenciesSatisfied(WorkflowTemplateNodeDTO node, WorkflowInstanceDTO workflowInstance, string currentNodeId)
        {
            var workflowTaskInstances = await _taskInstanceLogic.GetTaskInstancesByWorkflowInstance(workflowInstance.Id);
            // check every other task dependency accept the current task (assuming it will be completed soon)
            node.DependencyNodeIds.Remove(currentNodeId);
            // check that each dependency task has been created and completed
            foreach (var dependencyTaskId in node.DependencyNodeIds)
            {
                // get taskTemplateId
                var taskInstance = workflowTaskInstances.FirstOrDefault(i => i.WorkflowNodeId == currentNodeId);
                if (taskInstance == null)
                {
                    // task instance hasn't even been created yet
                    return false;
                }
                if (taskInstance.Status != TaskInstanceLogic.COMPLETED_TASK_STATUS)
                {
                    // task exists, but hasn't been complete
                    return false;
                }
            }
            return true;
        }

        private async Task<bool> AreAllTasksInWorkflowSectionComplete(List<WorkflowTemplateNodeDTO> workflowSectionNodes, string workflowInstanceId)
        {
            var taskTemplateIds = workflowSectionNodes.Select(t => t.TaskTemplateId);
            var workflowTaskInstances = await _taskInstanceLogic.GetTaskInstancesByWorkflowInstance(workflowInstanceId);
            foreach (var taskTemplateId in taskTemplateIds)
            {
                var taskInstance = workflowTaskInstances.FirstOrDefault(i => i.TaskTemplateId == taskTemplateId);
                if (taskInstance == null)
                {
                    // a task instance hasn't been created for this task yet 
                    return false;
                }
                if (taskInstance.Status != TaskInstanceLogic.COMPLETED_TASK_STATUS)
                {
                    // task exists, but hasn't been complete
                    return false;
                }
            }
            return true;
        }

        public async Task<HTTPResponse<string, string>> HandleOnboarderRegistration(string accountId, string emailAddress)
        {
            // find active worklow instance where onboarder email address is the same
            var matchingWorkflowInstance = (await _workflowInstanceRepository.GetAll()).FirstOrDefault(i => i.OnboarderEmailAddress == emailAddress);
            if (matchingWorkflowInstance == null)
            {
                // account isn't associated with an existing user
                return new HTTPResponse<string, string>() { Success = true, HttpCode = 200, Data = "Account isn't associated with a workflow instance where this user is an onboarder" };
            }
            if (!string.IsNullOrEmpty(matchingWorkflowInstance.OnboarderAccountId))
            {
                throw new Exception("OnboarderAccountId for worklow instance has already been set!");                
            }
            // update instance to include onboarder's account id
            matchingWorkflowInstance.OnboarderAccountId = accountId;
            await _workflowInstanceRepository.UpdateAsync(matchingWorkflowInstance);

            var workflowInstance = (await GetWorkflowInstance(matchingWorkflowInstance.Id)).Data ?? null;
            if (workflowInstance == null)
            {
                throw new Exception("Failed to retrieve DTO for workflow instance");
            }
            // start mainflow tasks
            _logger.LogDebug($"Handling onboarding registration for onboarder (${accountId}) in workflow instance: {workflowInstance.Id} ({workflowInstance.WorkflowTemplate.Name})");
            
            var nodesWithNoDependencies = workflowInstance?.WorkflowTemplate.MainflowTasks.Where(n => n.DependencyNodeIds.Count == 0).ToList();
            foreach(var node in nodesWithNoDependencies)
            {
                await _taskInstanceLogic.CreateInstance(
                    new CreateTaskInstancePayload()
                    {
                        TaskTemplateId = node.TaskTemplateId,
                        AssigneeAccountId = _utility.ReplaceAccountIdPlaceholder(node.AssigneeId, workflowInstance),
                        AssignerAccountId = _utility.ReplaceAccountIdPlaceholder(workflowInstance.SupervisorAccountId, workflowInstance),
                        WorkflowInstanceId = workflowInstance.Id,
                        WorkflowNodeId = node.Id,
                    }
                );                
            }

            return new HTTPResponse<string, string>() { Success = true, HttpCode = 200 };
        }

        public async Task<HTTPResponse<List<WorkflowInstanceDTO>, string>> GetAccountsWorkflowInstances(string accountId)
        {
            // find all workflows where user is the supervisor
            var workflowInstances = await _workflowInstanceRepository.GetInstancesBySupervisorAccountId(accountId);
            // find all workflows where user is the onboarder
            workflowInstances.AddRange(await _workflowInstanceRepository.GetInstancesByOnboarderAccountId(accountId));

            var workflowInstanceDTOs = new List<WorkflowInstanceDTO>();
            foreach (var workflowInstance in workflowInstances)
            {
                var dto = (await GetWorkflowInstance(workflowInstance.Id)).Data ?? null;
                if (dto != null) workflowInstanceDTOs.Add(dto);
            }

            return new HTTPResponse<List<WorkflowInstanceDTO>, string>() { Success = true, HttpCode = 200, Data = workflowInstanceDTOs }; 
        }

        private async Task<string> GetWorkflowInstanceStatus(WorkflowInstanceDTO workflowInstance)
        {
            if (workflowInstance.CompletedTasks == workflowInstance.WorkflowTemplate.NumberOfTasks)
            {
                return WORKFLOW_INSTANCE_COMPLETE_STATUS;
            }

            var preflowTasksCompleted = await AreAllTasksInWorkflowSectionComplete(workflowInstance.WorkflowTemplate.PreflowTasks, workflowInstance.Id);
            if (!preflowTasksCompleted && workflowInstance.WorkflowTemplate.IsOnboardingWF) return WORKFLOW_INSTANCE_PREFLOW_STATUS;
            
            var mainflowTasksCompleted = await AreAllTasksInWorkflowSectionComplete(workflowInstance.WorkflowTemplate.MainflowTasks, workflowInstance.Id);
            if (!mainflowTasksCompleted) return WORKFLOW_INSTANCE_MAINFLOW_STATUS;

            throw new Exception("Invalid condition met");
        }
    }
}
