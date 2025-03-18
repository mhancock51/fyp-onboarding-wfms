using AutoMapper;
using OnboardingWFMSApi.DataAccess.Repositories;
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
    public interface IWorkflowTemplateLogic
    {
        public Task<HTTPResponse<string, string>> CreateWorkflowTemplate(WorkflowTemplateDTO payload, string accountId);
        public Task<HTTPResponse<WorkflowTemplateDTO, string>> GetWorkflowTemplate(string id);
    }

    public class WorkflowTemplateLogic : IWorkflowTemplateLogic
    {        
        private readonly IWorkflowTemplateRepository _workflowTemplateRepository;
        private readonly IWorkflowTemplateNodeRepository _workflowTemplateNodeRepository;
        private readonly INodeTaskDependencyRepository _nodeTaskDependencyRepository;
        private readonly IAccountRepository _accountRepository;

        private readonly IMapper _mapper;

        public WorkflowTemplateLogic(IWorkflowTemplateRepository workflowTemplateRepository, IWorkflowTemplateNodeRepository workflowTemplateNodeRepository,
            INodeTaskDependencyRepository nodeTaskDependencyRepository, IMapper mapper, IAccountRepository accountRepository)
        {
            _workflowTemplateRepository = workflowTemplateRepository;
            _workflowTemplateNodeRepository = workflowTemplateNodeRepository;
            _nodeTaskDependencyRepository = nodeTaskDependencyRepository;
            _mapper = mapper;
            _accountRepository = accountRepository;
        }

        public async Task<HTTPResponse<string, string>> CreateWorkflowTemplate(WorkflowTemplateDTO payload, string accountId)
        {
            // ensure there isn't another template with the same name
            var existingTemplate = await _workflowTemplateRepository.GetWorkflowTemplateByName(payload.Name);
            if (existingTemplate != null) return new HTTPResponse<string, string>() { Success = false, HttpCode = 400, Error = "Template name must be unique" };

            if (payload.PreflowTasks.Count > 0 && !payload.IsOnboardingWF) return new HTTPResponse<string, string>() { Success = false, HttpCode = 400, Error = "None onboarding workflows can't have preflow tasks" };
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

            var nodes = new List<WorkflowTemplateNodeTable>();
            var dependencies = new List<NodeTaskDependencyTable>();            
            try
            {
                // add nodes and dependencies to tables
                int index = 0;
                foreach (var preflowTask in payload.PreflowTasks)
                {                
                    // TODO implement validation
                    // ensure preflow node (preboarding) isn't refererencing onboarder placeholder name before the onboarder has been invited
                    if (preflowTask.AssigneeId == Utility.ONBOARDER_ACCOUNT_ID_PLACEHOLDER)
                    {
                        throw new Exception("Onboarder placeholder account Id used in preflow task");
                    }
                    var node = new WorkflowTemplateNodeTable()
                    {
                        Id = Guid.NewGuid().ToString(),
                        Order = index,
                        TaskTemplateId = preflowTask.TaskTemplateId,
                        AssigneeId = preflowTask.AssigneeId,
                        WorkflowSection = "preflowtasks",
                        WorkflowTemplateId = workflowTemplate.Id
                    };
                    nodes.Add(node);
                    // TODO implement validation                                    

                    foreach (var nodeDependency in preflowTask.DependencyNodeIds)
                    {
                        var dependency = new NodeTaskDependencyTable()
                        {
                            Id = Guid.NewGuid().ToString(),
                            NodeId = node.Id,
                            DependencyNodeId = nodes.FirstOrDefault(n => n.TaskTemplateId == nodeDependency).Id,
                            WorkflowTemplateId = workflowTemplate.Id
                        };
                        dependencies.Add(dependency);
                    }
                    index++;
                }
                foreach (var mainflowTask in payload.MainflowTasks)
                {
                    // TODO implement validation
                    var node = new WorkflowTemplateNodeTable()
                    {
                        Id = Guid.NewGuid().ToString(),
                        Order = index,
                        TaskTemplateId = mainflowTask.TaskTemplateId,
                        AssigneeId = mainflowTask.AssigneeId,
                        WorkflowSection = "mainflowtasks",
                        WorkflowTemplateId = workflowTemplate.Id
                    };
                    nodes.Add(node);
                    // TODO implement validation                                    
                    // remove null values
                    foreach (var nodeDependency in mainflowTask.DependencyNodeIds)
                    {
                        if (nodeDependency == null) continue;
                        var dependency = new NodeTaskDependencyTable()
                        {
                            Id = Guid.NewGuid().ToString(),
                            NodeId = node.Id,
                            DependencyNodeId = nodes.FirstOrDefault(n => n.TaskTemplateId == nodeDependency).Id,
                            WorkflowTemplateId = workflowTemplate.Id
                        };
                        dependencies.Add(dependency);
                    }
                    index++;
                }
            }
            catch(Exception ex)
            {
                await _workflowTemplateRepository.DeleteAsync(workflowTemplate);
                return new HTTPResponse<string, string>() { Success = false, Error = "Failed to process nodes and dependencies", HttpCode = 500 };
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

        public async Task<HTTPResponse<WorkflowTemplateDTO, string>> GetWorkflowTemplate(string id)
        {
            var template = await _workflowTemplateRepository.GetById(id);
            if (template == null)
            {
                return new HTTPResponse<WorkflowTemplateDTO, string>() { Success = false, HttpCode = 400, Error = "Workflow template doesn't exist" };
            }

            WorkflowTemplateDTO workflowTemplate = _mapper.Map<WorkflowTemplateDTO>(template);

            // retrive nodes and dependencies
            var nodes = await _workflowTemplateNodeRepository.GetAllNodesByWorkflowTemplateId(template.Id);
            var dependencies = await _nodeTaskDependencyRepository.GetAllNodeDependenciesByWorkflowTemplateId(template.Id);
            
            // assign dependencies to nodes
            var preflowTasks = _mapper.Map<List<WorkflowTemplateNodeDTO>>(nodes.Where(i => i.WorkflowSection == "preflowtasks"));
            for (int i = 0; i < preflowTasks.Count; i++) 
            {
                preflowTasks[i].DependencyNodeIds = dependencies.Where(n => n.NodeId == preflowTasks[i].Id && n.WorkflowTemplateId == template.Id).Select(i => i.DependencyNodeId).ToList() ?? new List<string>();                
            }
            var mainflowTasks = _mapper.Map<List<WorkflowTemplateNodeDTO>>(nodes.Where(i => i.WorkflowSection == "mainflowtasks"));
            for (int i = 0; i < mainflowTasks.Count; i++)
            {
                mainflowTasks[i].DependencyNodeIds = dependencies.Where(n => n.NodeId == mainflowTasks[i].Id && n.WorkflowTemplateId == template.Id).Select(i => i.DependencyNodeId).ToList() ?? new List<string>();
            }
            workflowTemplate.PreflowTasks = preflowTasks.ToList();
            workflowTemplate.MainflowTasks = mainflowTasks.ToList();
            return new HTTPResponse<WorkflowTemplateDTO, string>() { Success = true, HttpCode = 200, Data =  workflowTemplate };
        }        
    }
}
