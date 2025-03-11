using OnboardingWFMSApi.DataAccess.Repositories.Workflow_Repositories;
using OnboardingWFMSApi.DataModels;
using OnboardingWFMSApi.DataModels.Payloads;
using OnboardingWFMSApi.DataModels.Tables.Workflows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnboardingWFMSApi.BusinessLogic
{
    public interface IWorkflowTemplateLogic
    {
        public Task<HTTPResponse<string, string>> CreateWorkflowTemplate(CreateWorkflowTemplatePayload payload, string accountId);
    }

    public class WorkflowTemplateLogic : IWorkflowTemplateLogic
    {
        private readonly IWorkflowTemplateRepository _workflowTemplateRepository;
        private readonly IWorkflowTemplateNodeRepository _workflowTemplateNodeRepository;
        private readonly INodeTaskDependencyRepository _nodeTaskDependencyRepository;

        public WorkflowTemplateLogic(IWorkflowTemplateRepository workflowTemplateRepository, IWorkflowTemplateNodeRepository workflowTemplateNodeRepository, 
            INodeTaskDependencyRepository nodeTaskDependencyRepository
        )
        {
            _workflowTemplateRepository = workflowTemplateRepository;
            _workflowTemplateNodeRepository = workflowTemplateNodeRepository;
            _nodeTaskDependencyRepository = nodeTaskDependencyRepository;
        }

        public async Task<HTTPResponse<string, string>> CreateWorkflowTemplate(CreateWorkflowTemplatePayload payload, string accountId)
        {
            // ensure there isn't another template with the same name
            var existingTemplate = await _workflowTemplateRepository.GetWorkflowTemplateByName(payload.Name);
            if (existingTemplate != null) return new HTTPResponse<string, string>() { Success = false, HttpCode = 400, Error = "Template name must be unique" };

            // create workflow template row
            var workflowTemplate = new WorkflowTemplateTable() { Name = payload.Name, Description = payload.Description, IsOnboardingWF = payload.IsOnboardingWF };
            try
            {
                workflowTemplate = await _workflowTemplateRepository.AddAsync(workflowTemplate);                
            }
            catch (Exception ex)
            {
                return new HTTPResponse<string, string>() { Success = false, HttpCode = 500, Error = "Failed to create workflow template" };
            }

            // add nodes and dependencies to tables
            int index = 0;
            var nodes = new List<WorkflowTemplateNodeTable>();
            var dependencies = new List<NodeTaskDependencyTable>();
            foreach (var preflowTask in payload.PreflowTasks)
            {                
                // TODO implement validation
                var node = new WorkflowTemplateNodeTable()
                {
                    Order = index,
                    TaskTemplateId = preflowTask.TaskTemplateId,
                    AssigneeId = preflowTask.AssigneeId,
                    WorkflowSection = "preflowtasks",
                    WorkflowTemplateId = workflowTemplate.Id
                };
                nodes.Add(node);
                // TODO implement validation
                var taskDependencies = new List<NodeTaskDependencyTable>();
                foreach (var taskDependency in preflowTask.DependencyTaskTemplateIds)
                {
                    var dependency = new NodeTaskDependencyTable()
                    {
                        NodeId = preflowTask.TaskTemplateId,
                        DependencyNodeId = taskDependency
                    };
                    taskDependencies.Add(dependency);
                }
                index++;
            }

            // securely insert nodes
            try
            {
                nodes = await _workflowTemplateNodeRepository.AddManyAsync(nodes);
            }
            catch (Exception ex)
            {
                await _workflowTemplateRepository.DeleteAsync(workflowTemplate);
                return new HTTPResponse<string, string>() { Success = false, HttpCode = 500, Error = "Failed to insert nodes" };
            }

            // securely insert dependencies
            try
            {
                dependencies = await _nodeTaskDependencyRepository.AddManyAsync(dependencies);
            }
            catch (Exception ex)
            {
                await _workflowTemplateRepository.DeleteAsync(workflowTemplate);
                // TODO: delete nodes
                return new HTTPResponse<string, string>() { Success = false, HttpCode = 500, Error = "Failed to save dependencies" };
            }
            return new HTTPResponse<string, string>() { Success = true, HttpCode = 200, Data = "Successfully created workflow template" };
        }
    }
}
