using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using OnboardingWFMSApi.BusinessLogic.MediatRHandlers;
using OnboardingWFMSApi.BusinessLogic.WorkflowTemplateLogic;
using OnboardingWFMSApi.DataAccess.Repositories;
using OnboardingWFMSApi.DataAccess.Repositories.Task_Repositories;
using OnboardingWFMSApi.DataAccess.Repositories.Workflow_Repositories;
using OnboardingWFMSApi.DataModels;
using OnboardingWFMSApi.DataModels.DTOs;
using OnboardingWFMSApi.DataModels.Models;
using OnboardingWFMSApi.DataModels.Payloads;
using OnboardingWFMSApi.DataModels.Tables;
using OnboardingWFMSApi.DataModels.Tables.Workflows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace OnboardingWFMSApi.BusinessLogic
{
    public interface IWorkflowInstanceLogic
    {
        public Task<HTTPResponse<string, string>> CreateWorkflowInstance(CreateWorkflowInstancePayload payload);
        public Task<HTTPResponse<WorkflowInstanceDTO, string>> GetWorkflowInstance(string workflowInstanceId);
        public Task<HTTPResponse<string, string>> HandleTaskInstanceCompletion(TaskInstanceDTO taskInstance);
        public Task<HTTPResponse<string, string>> HandleOnboarderRegistration(string accountId, string emailAddress);
        public Task<HTTPResponse<List<WorkflowInstanceDTO>, string>> GetAccountsWorkflowInstances(string accountId);        
        public Task<HTTPResponse<List<WorkflowInstanceDTO>, string>> GetAlllOnboardingWorkflowInstances(DateTime? from, DateTime? to);
        public Task<WorkflowInstanceDTO?> GetOnboardersWorkflowInstance(string onboarderAccountId);
    }
    public class WorkflowInstanceLogic : IWorkflowInstanceLogic
    {
        public const string WORKFLOW_INSTANCE_PREFLOW_STATUS = "PREFLOW";
        public const string WORKFLOW_INSTANCE_MAINFLOW_STATUS = "MAINFLOW";
        public const string WORKFLOW_INSTANCE_COMPLETE_STATUS = "COMPLETE";

        private readonly ILogger<WorkflowInstanceLogic> _logger;
        private readonly IMapper _mapper;
        private readonly IUtility _utility;
        private readonly IMediator _mediator;


        private readonly IWorkflowTemplateLogic _workflowTemplateLogic;

        private readonly IWorkflowInstanceRepository _workflowInstanceRepository;
        private readonly IWorkflowNodeInstanceRepository _workflowNodeInstanceRepository;
        private readonly IWorkflowTemplateNodeRepository _workflowTemplateNodeRepository;

        private readonly IOnboardingEmployeeDetailsRepository _onboardingEmployeeDetailsRepository;
        private readonly IAccountRepository _accountRepository;

        private readonly ITaskTemplateRepository _taskTemplateRepository;
        private readonly ITaskInstanceRepository _taskInstanceRepository;

        public WorkflowInstanceLogic(IWorkflowInstanceRepository workflowInstanceRepository, IWorkflowTemplateLogic workflowTemplateLogic,
            ILogger<WorkflowInstanceLogic> logger, IMapper mapper, IUtility utility,
            IOnboardingEmployeeDetailsRepository onboardingEmployeeDetailsRepository, IMediator mediator, IWorkflowNodeInstanceRepository workflowNodeInstanceRepository, IWorkflowTemplateNodeRepository workflowTemplateNodeRepository, ITaskTemplateRepository taskTemplateRepository, ITaskInstanceRepository taskInstanceRepository, IAccountRepository accountRepository)
        {
            _workflowInstanceRepository = workflowInstanceRepository;
            _workflowTemplateLogic = workflowTemplateLogic;
            _logger = logger;
            _mapper = mapper;
            _utility = utility;
            _onboardingEmployeeDetailsRepository = onboardingEmployeeDetailsRepository;
            _mediator = mediator;
            _workflowNodeInstanceRepository = workflowNodeInstanceRepository;
            _workflowTemplateNodeRepository = workflowTemplateNodeRepository;
            _taskTemplateRepository = taskTemplateRepository;
            _taskInstanceRepository = taskInstanceRepository;
            _accountRepository = accountRepository;
        }

        public async Task<HTTPResponse<string, string>> CreateWorkflowInstance(CreateWorkflowInstancePayload payload)
        {            

            // fetch workflow template - make sure it exists
            var workflowTemplate = (await _workflowTemplateLogic.GetWorkflowTemplate(payload.WorkflowTeamplateId)).Data ?? null;
            if (workflowTemplate == null)
            {
                return new HTTPResponse<string, string>() { Success = false, HttpCode = 400, Error = "Workflow template doesn't exist" };
            }

            if (workflowTemplate.Status == WorkflowTemplateConstants.ARCHIVED_WORKFLOW_TEMPLATE_STATUS)
            {
                return new HTTPResponse<string, string>() { Success = false, HttpCode = 400, Error = "Workflow template is archived" };
            }

            // make sure all task templates in the workflow template are active/not archived
            var taskTemplateIds = workflowTemplate.MainflowNodes.Concat(workflowTemplate.PreflowNodes).Select(n => n.TaskTemplateId).ToList();
            foreach(var id in taskTemplateIds)
            {
                var taskTemplate = await _taskTemplateRepository.GetById(id);
                if (taskTemplate.Status != TaskTemplateLogic.ACTIVE_TASK_TEMPLATE_STATUS)
                {
                    return new HTTPResponse<string, string>() { Success = false, HttpCode = 400, Error = $"Workflow template contains a task template that is archived or inactive ({taskTemplate.Name})" };
                }
            }

            // ensure all assignees in all nodes are registered i.e. account not closed
            foreach(var node in workflowTemplate.PreflowNodes.Concat(workflowTemplate.MainflowNodes))
            {
                // ignore placeholder values
                if (node.AssigneeId == Utility.ONBOARDER_ACCOUNT_ID_PLACEHOLDER || node.AssigneeId == Utility.SUPERVISOR_ACCOUNT_ID_PLACEHOLDER)
                {
                    continue;
                }
                // ensure all really account ids are associated with registered accounts
                var isRegistered = await _accountRepository.IsAccountRegistered(node.AssigneeId);
                if (isRegistered == false)
                {
                    return new HTTPResponse<string, string>() { Success = false, HttpCode = 400, Error = "Workflow template contains a task assigned to a closed account, please update to be able to start an instance" };
                }
            }
            
            if (workflowTemplate.IsOnboardingWF && (payload.OnboardingEmployeeDetails == null || payload.SupervisorAccountId == null))
            {
                return new HTTPResponse<string, string>() { Success = false, HttpCode = 400, Error = "Onboarder and Supervisor account must be selected" };
            }

            if (payload.OnboardingEmployeeDetails != null)
            {
                // ensure that onboarder account isn't already associated with an existing onboarding workflow instance
                var instances = await _workflowInstanceRepository.GetInstancesByOnboarderEmailAddress(payload.OnboardingEmployeeDetails.EmailAddress);
                if (instances.Count > 0) return new HTTPResponse<string, string>() { Success = false, HttpCode = 400, Error = "A similar onboarding workflow instance already exists" };

                // ensure onboarder email isn't already associated with an existing account
                var accountExists = await _accountRepository.DoesAccountExistByEmail(payload.OnboardingEmployeeDetails.EmailAddress);
                if (accountExists) return new HTTPResponse<string, string>() { Success = false, HttpCode = 400, Error = "Onboarder's email address must not be associated with an existing account" };
            }

            // TODO: ensure supervisor account is active            

            var workflowInstance = new WorkflowInstanceTable()
            {
                Id = "",
                WorkflowTemplateId = payload.WorkflowTeamplateId,                    
                SupervisorAccountId = payload.SupervisorAccountId,
                CreationTimestamp = DateTime.UtcNow                
            };
            try
            {
                workflowInstance = await _workflowInstanceRepository.AddAsync(workflowInstance);
            }
            catch (Exception ex)
            {
                await _workflowInstanceRepository.DeleteAsync(workflowInstance);
                return new HTTPResponse<string, string>() { Success = false, HttpCode = 500, Error = "Failed to create workflow instance" };
            }
            // insert employee details record
            if (workflowTemplate.IsOnboardingWF && payload.OnboardingEmployeeDetails != null)
            {
                try
                {
                    var details = new OnboardingEmployeeDetailsTable()
                    {
                        Id = "",
                        WorkflowInstanceId = workflowInstance.Id,
                        DisplayName = payload.OnboardingEmployeeDetails.DisplayName,
                        EmailAddress = payload.OnboardingEmployeeDetails.EmailAddress,
                        DepartmentId = payload.OnboardingEmployeeDetails.DepartmentId,
                    };
                    await _onboardingEmployeeDetailsRepository.AddAsync(details);
                }
                catch (Exception ex)
                {
                    await _workflowInstanceRepository.DeleteAsync(workflowInstance);
                    return new HTTPResponse<string, string>() { Success = false, HttpCode = 500, Error = "Failed to record onboarding employee's details" };
                }
            }

            var nodeInstances = new List<WorkflowInstanceNodeTable>();
            // create workflow node instances for all nodes
            foreach (var templateNode in workflowTemplate.PreflowNodes.Concat(workflowTemplate.MainflowNodes))
            {
                // instantiate instance of the node
                WorkflowInstanceNodeTable nodeInstance = new WorkflowInstanceNodeTable()
                {
                    Id = "",
                    WorkflowInstanceId = workflowInstance.Id,
                    WorkflowTemplateId = workflowTemplate.Id,
                    WorkflowTemplateNodeId = templateNode.Id,
                    TaskTemplateId = templateNode.TaskTemplateId,
                    Status = "unassigned"
                };
                await _workflowNodeInstanceRepository.AddAsync(nodeInstance);
                nodeInstances.Add(nodeInstance);
            }

            // assign first tasks (ones with no dependencies in the preflow (if onboarding or mainflow))
            var nodesWithNoDependencies = new List<WorkflowInstanceNodeTable>();
            
            if (workflowTemplate.IsOnboardingWF)
            {
                // add all node instances that match a template node with no dependencies                
                var templateNodes = workflowTemplate.PreflowNodes.Where(n => n.DependencyNodeIds.Count == 0).ToList();
                nodesWithNoDependencies = nodesWithNoDependencies.Concat(nodeInstances.Where(i => templateNodes.Select(t => t.Id).Contains(i.WorkflowTemplateNodeId))).ToList();
            }
            else
            {
                // add all node instances that match a template node with no dependencies  
                var templateNodes = workflowTemplate.MainflowNodes.Where(n => n.DependencyNodeIds.Count == 0).ToList();
                nodesWithNoDependencies = nodesWithNoDependencies.Concat(nodeInstances.Where(i => templateNodes.Select(t => t.Id).Contains(i.WorkflowTemplateNodeId))).ToList();
            }
            // assign tasks for all node instances
            foreach(var node in nodesWithNoDependencies)
            {
                // get associated node template
                var nodeTemplate = workflowTemplate.PreflowNodes.Concat(workflowTemplate.MainflowNodes).FirstOrDefault(i => i.Id == node.WorkflowTemplateNodeId);
                var taskInstancePayload = new CreateTaskInstancePayload()
                {
                    TaskTemplateId = node.TaskTemplateId,
                    AssigneeAccountId = await _utility.ReplaceAccountIdPlaceholder(nodeTemplate.AssigneeId, workflowInstance),
                    AssignerAccountId = await _utility.ReplaceAccountIdPlaceholder(workflowInstance.SupervisorAccountId, workflowInstance),
                    WorkflowInstanceId = workflowInstance.Id,
                    WorkflowInstanceNodeId = node.Id,
                    DueDate = GetDueDateOfTask(nodeTemplate, workflowInstance)
                };

                var response = await _mediator.Send(new CreateTaskInstanceRequest(taskInstancePayload));                
                if (!response.Success)
                {
                    _logger.LogWarning($"Failed to create task instance through mediator");
                }
                else
                {
                    _logger.LogInformation($"Successfully created task instance through mediator (Task template {node.TaskTemplateId}, assigne {nodeTemplate.AssigneeId})");
                }
                // update node instance to be "open" rather than "unassigned"
                node.Status = "open";
                await _workflowNodeInstanceRepository.UpdateAsync(node);
            }

            var log = new CreateWorkflowInstanceAuditLogPayload()
            {
                WorkflowInstanceId = workflowInstance.Id,
                Log = "Workflow instance started",
            };
            await _mediator.Send(new CreateWorkflowInstanceAuditLogRequest(log));
            return new HTTPResponse<string, string>() { Success = true, HttpCode = 200, Data = "Successfully instantiated workflow instance" };
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
            if (!result.Success || !result.HasData || result.Data == null)
            {
                return new HTTPResponse<WorkflowInstanceDTO, string>() { Success = false, HttpCode = 500, Error = "Failed to retrieve workflow template" };
            }            
            workflowInstanceDTO.WorkflowTemplate = result.Data;
            // retrieve the number of completed tasks
            var completeTaskInstances = (await _taskInstanceRepository.GetTaskInstancesByWorkflowInstance(workflowInstance.Id)).Where(i => i.Status == TaskInstanceLogic.COMPLETED_TASK_STATUS);
            workflowInstanceDTO.CompletedTasks = completeTaskInstances.Count();
            // retrieve the number of node instances (used to determine status)
            var nodeInstances = await _workflowNodeInstanceRepository.GetByWorkflowInstanceId(workflowInstanceId);
            workflowInstanceDTO.NumberOfNodes = nodeInstances.Count;
            // determine status
            workflowInstanceDTO.Status = await GetWorkflowInstanceStatus(workflowInstanceDTO);


            if (workflowInstanceDTO.WorkflowTemplate.IsOnboardingWF)
            {
                // include onboarding employee's details
                var employeeDetails = await _onboardingEmployeeDetailsRepository.GetDetailsByWorkflowInstance(workflowInstanceDTO.Id);
                workflowInstanceDTO.OnboardingEmployeeDetails = _mapper.Map<OnboardingEmployeeDetailsDTO>(employeeDetails);
            }

            return new HTTPResponse<WorkflowInstanceDTO, string>() { Success = true, HttpCode = 200, Data = workflowInstanceDTO };
        }

        public async Task<HTTPResponse<string, string>> HandleTaskInstanceCompletion(TaskInstanceDTO taskInstance)
        {
            _logger.LogDebug($"Handling task completion event for task instance: {taskInstance.Id} ({taskInstance.Template.Name}, {taskInstance.AssigneeAccountId})");
            if (string.IsNullOrEmpty(taskInstance.WorkflowInstanceId))
            {
                throw new Exception("Task instance isn't associated to a workflow instance");
            }

            var workflowInstance = (await GetWorkflowInstance(taskInstance.WorkflowInstanceId)).Data ?? null;
            if (workflowInstance == null)
            {
                return new HTTPResponse<string, string>() { Success = false, HttpCode = 500, Error = "Failed to retrieve workflow instance" };
            }

            // update associated node instance to be marked as "complete"
            var nodeInstance = await _workflowNodeInstanceRepository.GetById(taskInstance.WorkflowInstanceNodeId);
            if (nodeInstance == null && taskInstance.WorkflowInstanceNodeId != null)
            {
                throw new Exception("Task instance is associated to a node instance that can't be found!");
            }
            nodeInstance.Status = "complete";
            await _workflowNodeInstanceRepository.UpdateAsync(nodeInstance);

            // find associated workflow node
            var nodeTemplate = workflowInstance.WorkflowTemplate.PreflowNodes.Concat(workflowInstance.WorkflowTemplate.MainflowNodes)
                .FirstOrDefault(n => n.TaskTemplateId == taskInstance.TaskTemplateId);

            // check if a user needs to be notified
            if (nodeTemplate.AccountsToNotify != null)
            {
                // notify user
                var assigneesAccount = await _mediator.Send(new RetrieveAccountDirectoryRequest(taskInstance.AssigneeAccountId));
                string notificationText = $"'{taskInstance.Template.Name}' task completed by {assigneesAccount.DisplayName ?? "USER"} for workflow {workflowInstance.WorkflowTemplate.Name}";
                if (workflowInstance.WorkflowTemplate.IsOnboardingWF && workflowInstance.OnboardingEmployeeDetails != null)
                {
                    notificationText += $" to onboard {workflowInstance.OnboardingEmployeeDetails.DisplayName}";
                }
                foreach(var accountId in nodeTemplate.AccountsToNotify)
                {
                    var parsedAccountId = await _utility.ReplaceAccountIdPlaceholder(accountId, workflowInstance);
                    var response = await _mediator.Send(new CreateNotificationRequest(new CreateNotificationPayload(parsedAccountId, notificationText, new string[] { "Worfklow Instance", "Onboarding"})));
                    if (response.Success == false)
                    {
                        _logger.LogWarning("Failed to send notification");
                    } 
                }
            }


            // invite onboarder once all preflow (preboarding) tasks have been completed
            if (workflowInstance.WorkflowTemplate.IsOnboardingWF)
            {
                // check if all preflow tasks have been completed
                var isPreflowSectionComplete = await AreAllTasksInWorkflowSectionComplete(workflowInstance.WorkflowTemplateId, workflowInstance.Id, "preflowtasks");
                if (isPreflowSectionComplete && workflowInstance.OnboardingEmployeeDetails.OnboarderAccountId == null)
                {
                    var onboardingEmployeeDetails = await _onboardingEmployeeDetailsRepository.GetDetailsByWorkflowInstance(workflowInstance.Id);
                    if (onboardingEmployeeDetails == null) throw new Exception("No onboarding employee details could be retrieved for an onboarding workflow instance");

                    var result = await _mediator.Send(new InviteAccountRequest(onboardingEmployeeDetails.DisplayName, onboardingEmployeeDetails.EmailAddress, onboardingEmployeeDetails.DepartmentId));                    
                    
                    // create audit log
                    var log = new CreateWorkflowInstanceAuditLogPayload()
                    {
                        WorkflowInstanceId = workflowInstance.Id,
                        Log = $"Onboarder invited to organisation",
                    };
                    await _mediator.Send(new CreateWorkflowInstanceAuditLogRequest(log));                    
                    // send notification to supervisor
                    await _mediator.Send(new CreateNotificationRequest(new CreateNotificationPayload(workflowInstance.SupervisorAccountId, $"Onboarder '{workflowInstance.OnboardingEmployeeDetails.DisplayName}' has been invited to the organisation", ["Workflow", "Onboarding", "Registration"])));
                    return result;
                }
            }

            // mark workflow instance as complete if complete
            if (await AreAllTasksInWorkflowSectionComplete(workflowInstance.WorkflowTemplateId, workflowInstance.Id, "preflowtasks") 
                && await AreAllTasksInWorkflowSectionComplete(workflowInstance.WorkflowTemplateId, workflowInstance.Id, "mainflowtasks"))
            {
                workflowInstance.CompletionTimestamp = DateTime.Now;
                await _workflowInstanceRepository.UpdateAsync(workflowInstance);
                _logger.LogInformation("Workflow instance completed");
                // create audit log
                var log = new CreateWorkflowInstanceAuditLogPayload()
                {
                    WorkflowInstanceId = workflowInstance.Id,
                    Log = $"Workflow instance completed",                    
                };
                await _mediator.Send(new CreateWorkflowInstanceAuditLogRequest(log));

                var accountsToNotify = new List<string>() { workflowInstance.SupervisorAccountId };

                var notificationText = $"'{workflowInstance.WorkflowTemplate.Name}' workflow instance completed!";
                if (workflowInstance.WorkflowTemplate.IsOnboardingWF)
                {
                    // send completion notification to supervisor and onboarder
                    notificationText = $"Workflow to onboard {workflowInstance.OnboardingEmployeeDetails.DisplayName} has been completed";

                }
                foreach (var accountId in accountsToNotify)
                {
                    await _mediator.Send(new CreateNotificationRequest(new CreateNotificationPayload(accountId, notificationText, ["Workflow", "Completed"])));
                }
            }

            // ASSIGN TASKS THAT WERE DEPENDENT ON THIS TASK AND ARE NOW SATISFIED
            var workflowInstancesNodeTemplates = workflowInstance.WorkflowTemplate.PreflowNodes.Concat(workflowInstance.WorkflowTemplate.MainflowNodes).ToList();
            var dependentNodeInstances = (await _workflowNodeInstanceRepository.GetByWorkflowInstanceId(workflowInstance.Id))
                                            .Where(i => workflowInstancesNodeTemplates.Find(j => j.Id == i.WorkflowTemplateNodeId).DependencyNodeIds.Contains(nodeTemplate.Id));

            foreach(var dependentNodeInstance in dependentNodeInstances)
            {
                if (await AreNodeInstancesDependenciesSatisfied(dependentNodeInstance))
                {
                    var dependentNodeTemplate = workflowInstancesNodeTemplates.FirstOrDefault(i => i.Id == dependentNodeInstance.WorkflowTemplateNodeId);
                    var payload = new CreateTaskInstancePayload()
                    {
                        TaskTemplateId = dependentNodeTemplate.TaskTemplateId,
                        AssigneeAccountId = await _utility.ReplaceAccountIdPlaceholder(dependentNodeTemplate.AssigneeId, workflowInstance),
                        AssignerAccountId = await _utility.ReplaceAccountIdPlaceholder(workflowInstance.SupervisorAccountId, workflowInstance),
                        WorkflowInstanceId = workflowInstance.Id,
                        WorkflowInstanceNodeId = dependentNodeInstance.Id,
                        DueDate = GetDueDateOfTask(dependentNodeTemplate, workflowInstance)
                    };

                    var response = await _mediator.Send(new CreateTaskInstanceRequest(payload));
                    if (!response.Success)
                    {
                        _logger.LogWarning($"Failed to create task instance through mediator");
                    }
                    else
                    {
                        _logger.LogInformation($"Successfully created task instance through mediator (Task template {dependentNodeInstance.TaskTemplateId}, assigne {nodeTemplate.AssigneeId})");
                    }
                }
                else
                {
                    _logger.LogDebug("Not all of node's dependencies are satisified");
                }
            }
            return new HTTPResponse<string, string>() { Success = true, HttpCode = 200 };
        }        

        private async Task<bool> AreNodeInstancesDependenciesSatisfied(WorkflowInstanceNodeTable nodeInstance)
        {
            var workflowTemplate = (await _workflowTemplateLogic.GetWorkflowTemplate(nodeInstance.WorkflowTemplateId))?.Data ?? null;
            if (workflowTemplate == null) throw new Exception("Node instance is associated to a workflow template that doesn't exist");

            var nodeTemplate = workflowTemplate.MainflowNodes.Concat(workflowTemplate.PreflowNodes).FirstOrDefault(i => i.Id == nodeInstance.WorkflowTemplateNodeId);
            // get node instances associated with the node's dependency node templates
            var nodeDependencyTemplateIds = workflowTemplate.MainflowNodes.Concat(workflowTemplate.PreflowNodes).Where(i => nodeTemplate.DependencyNodeIds.Contains(i.Id)).Select(i => i.Id).ToList();
            var nodeDependencyInstances = (await _workflowNodeInstanceRepository.GetByWorkflowInstanceId(nodeInstance.WorkflowInstanceId)).Where(i => nodeDependencyTemplateIds.Contains(i.WorkflowTemplateNodeId)).ToList();

            // ensure all node instances that the node depends on are complete
            foreach(var nodeDependencyInstance in nodeDependencyInstances)
            {
                if (nodeDependencyInstance.Status != "complete")
                {
                    return false;
                }
            }
            return true;          
        }

        private async Task<bool> AreAllTasksInWorkflowSectionComplete(string workflowTemplateId, string workflowInstanceId, string section)
        {
            // get ids of all node templates from that section
            var nodeTemplateIds = (await _workflowTemplateNodeRepository.GetAllNodesByWorkflowTemplateId(workflowTemplateId))
                                        .Where(n => n.WorkflowSection.ToLower() == section.ToLower())
                                        .Select(n => n.Id)
                                        .ToList();       

            var nodeInstances = await _workflowNodeInstanceRepository.GetByWorkflowInstanceId(workflowInstanceId);
            // filter node instances to just the one relevant to the section
            nodeInstances = nodeInstances.Where(i => nodeTemplateIds.Contains(i.WorkflowTemplateNodeId)).ToList();
            // find the open/unassigned nodes
            foreach(var nodeInstance in nodeInstances )
            {
                if (nodeInstance.Status.ToLower() != "complete")
                {
                    return false;
                }
            }
            return true;
        }

        public async Task<HTTPResponse<string, string>> HandleOnboarderRegistration(string accountId, string emailAddress)
        {
            // find active worklow instance where onboarder email address is the same
            var matchingWorkflowInstance = (await _workflowInstanceRepository.GetInstancesByOnboarderEmailAddress(emailAddress)).FirstOrDefault();
            if (matchingWorkflowInstance == null)
            {
                // account isn't associated with an existing user
                return new HTTPResponse<string, string>() { Success = true, HttpCode = 200, Data = "Account isn't associated with a workflow instance where this user is an onboarder" };
            }
            var workflowInstanceDTO = (await GetWorkflowInstance(matchingWorkflowInstance.Id))?.Data ?? null;

            if (!string.IsNullOrEmpty(workflowInstanceDTO.OnboardingEmployeeDetails.OnboarderAccountId))
            {
                throw new Exception("OnboarderAccountId for workflow instance has already been set!");                
            }            

            _logger.LogDebug($"Handling onboarding registration for onboarder {accountId} ({emailAddress})");

            // get onboarder details and update it
            var employeeDetails = await _onboardingEmployeeDetailsRepository.GetDetailsByWorkflowInstance(workflowInstanceDTO.Id);
            employeeDetails.OnboarderAccountId = accountId;
            await _onboardingEmployeeDetailsRepository.UpdateAsync(employeeDetails);           
            // update instance to indicate mainflow start         
            matchingWorkflowInstance.MainflowStartTimestamp = DateTime.UtcNow;
            await _workflowInstanceRepository.UpdateAsync(matchingWorkflowInstance);

            var workflowInstance = (await GetWorkflowInstance(matchingWorkflowInstance.Id)).Data ?? null;
            if (workflowInstance == null)
            {
                throw new Exception("Failed to retrieve DTO for workflow instance");
            }

            var workflowTemplate = (await _workflowTemplateLogic.GetWorkflowTemplate(workflowInstance.WorkflowTemplateId)).Data ?? null;
            if (workflowTemplate == null)
            {
                throw new Exception("Failed to retrieve DTO for workflow template");
            }

            // send notification to supervisor
            await _mediator.Send(new CreateNotificationRequest(new CreateNotificationPayload(workflowInstance.SupervisorAccountId, $"Onboarder '{workflowInstance.OnboardingEmployeeDetails.DisplayName}' has registered their account", ["Workflow", "Onboarding", "Registration"])));


            // start mainflow tasks with no dependencies
            var nodeInstances = (await _workflowNodeInstanceRepository.GetByWorkflowInstanceId(workflowInstance.Id));
            var templateNodes = workflowTemplate.MainflowNodes.Where(n => n.DependencyNodeIds.Count == 0).ToList();
            var nodesWithNoDependencies = nodeInstances.Where(i => templateNodes.Select(t => t.Id).Contains(i.WorkflowTemplateNodeId));
            
            foreach (var nodeInstance in nodesWithNoDependencies)
            {
                // get associated node template
                var nodeTemplate = workflowTemplate.MainflowNodes.Find(i => i.Id == nodeInstance.WorkflowTemplateNodeId);
                var taskInstancePayload = new CreateTaskInstancePayload()
                {
                    TaskTemplateId = nodeInstance.TaskTemplateId,
                    AssigneeAccountId = await _utility.ReplaceAccountIdPlaceholder(nodeTemplate.AssigneeId, workflowInstance),
                    AssignerAccountId = await _utility.ReplaceAccountIdPlaceholder(workflowInstance.SupervisorAccountId, workflowInstance),
                    WorkflowInstanceId = workflowInstance.Id,
                    WorkflowInstanceNodeId = nodeInstance.Id,
                    DueDate = GetDueDateOfTask(nodeTemplate, workflowInstance)
                };
                var response = await _mediator.Send(new CreateTaskInstanceRequest(taskInstancePayload));
                if (!response.Success)
                {
                    _logger.LogWarning($"Failed to create task instance through mediator");
                }
                else
                {
                    _logger.LogInformation($"Successfully created task instance through mediator (Task template {nodeInstance.TaskTemplateId}, assigne {nodeTemplate.AssigneeId})");
                }
                // update node instance to be "open" rather than "unassigned"
                nodeInstance.Status = "open";
                await _workflowNodeInstanceRepository.UpdateAsync(nodeInstance);
            }

            // create audit log
            var log = new CreateWorkflowInstanceAuditLogPayload()
            {
                WorkflowInstanceId = workflowInstance.Id,
                Log = $"Onboarder registered their account",
                AccountId = employeeDetails.OnboarderAccountId  
            };
            await _mediator.Send(new CreateWorkflowInstanceAuditLogRequest(log));

            return new HTTPResponse<string, string>() { Success = true, HttpCode = 200 };
        }

        public async Task<HTTPResponse<List<WorkflowInstanceDTO>, string>> GetAccountsWorkflowInstances(string accountId)
        {
            // find all workflows where user is the supervisor
            var workflowInstances = await _workflowInstanceRepository.GetInstancesBySupervisorAccountId(accountId);
            // find all workflows where user is the onboarder
            workflowInstances.AddRange(await _workflowInstanceRepository.GetInstancesByOnboarderAccountId(accountId));
            // find all workflows where user is an assignee of a task
            workflowInstances.AddRange(await _workflowInstanceRepository.GetInstancesWhereAccountIsAssignee(accountId));

            workflowInstances = workflowInstances.Distinct().ToList();

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
            if (workflowInstance.CompletedTasks == workflowInstance.NumberOfNodes)
            {
                return WORKFLOW_INSTANCE_COMPLETE_STATUS;
            }

            var preflowTasksCompleted = await AreAllTasksInWorkflowSectionComplete(workflowInstance.WorkflowTemplateId, workflowInstance.Id, "preflowtasks");
            if (!preflowTasksCompleted && workflowInstance.WorkflowTemplate.IsOnboardingWF) return WORKFLOW_INSTANCE_PREFLOW_STATUS;
            
            var mainflowTasksCompleted = await AreAllTasksInWorkflowSectionComplete(workflowInstance.WorkflowTemplateId, workflowInstance.Id, "mainflowtasks");
            if (!mainflowTasksCompleted) return WORKFLOW_INSTANCE_MAINFLOW_STATUS;

            _logger.LogWarning($"Illegal condition met retrieving the workflow status of workflow instance {workflowInstance.Id}");
            return "ERROR";
        }

        // assign due date depending on the node's section and when the instance was started/mainflow was started
        private DateTime? GetDueDateOfTask(WorkflowTemplateNodeDTO node, WorkflowInstanceTable workflowInstance)
        {
            if (node.DaysUntilDue == null) return null;

            if (node.WorkflowSection == "mainflowtasks" && workflowInstance.MainflowStartTimestamp != null)
            {
                return workflowInstance.MainflowStartTimestamp.Value.AddDays((double)node.DaysUntilDue);
            }
            else if (node.WorkflowSection == "preflowtasks")
            {
                return workflowInstance.CreationTimestamp.AddDays((double)node.DaysUntilDue);
            }
            else
            {
                throw new Exception("Invalid state reached");
            }
        }

        public async Task<HTTPResponse<List<WorkflowInstanceDTO>, string>> GetAlllOnboardingWorkflowInstances(DateTime? from, DateTime? to)
        {
            var workflowInstances = await _workflowInstanceRepository.GetOpenOnboardingWorkflowInstances();
            // filter for "from" and "to" values
            if (from != null) 
            {
                workflowInstances = workflowInstances.Where(i => i.CreationTimestamp >= from).ToList();
            }
            if (to != null)
            {
                workflowInstances = workflowInstances.Where(i => i.CreationTimestamp <= to).ToList();
            }

            // build DTOss
            var workflowInstanceIds = workflowInstances.Select(i => i.Id).ToList();
            var workflowInstanceDTOs = new List<WorkflowInstanceDTO>();
            foreach (var id in workflowInstanceIds)
            {
                var workflowInstance = (await GetWorkflowInstance(id)).Data ?? null;
                if (workflowInstance != null) workflowInstanceDTOs.Add(workflowInstance);
            }
            return new HTTPResponse<List<WorkflowInstanceDTO>, string>() { Success = true, HttpCode = 200, Data = workflowInstanceDTOs };
        }

        public async Task<WorkflowInstanceDTO?> GetOnboardersWorkflowInstance(string onboarderAccountId)
        {
            var workflowInstances = await _workflowInstanceRepository.GetInstancesByOnboarderAccountId(onboarderAccountId);
            if (workflowInstances.Count == 0) return null;

            var workflowInstanceDTO = (await GetWorkflowInstance(workflowInstances[0].Id)).Data ?? null;
            return workflowInstanceDTO;
        }
    }
}
