using AutoMapper;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
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
        public Task<HTTPResponse<string, string>> CreateWorkflowTemplate(CreateWorkflowTemplatePayload payload, string accountId);
        public Task<HTTPResponse<string, string>> UpdateWorkflowTemplate(CreateWorkflowTemplatePayload payload, string accountId);
        public Task<HTTPResponse<WorkflowTemplateDTO, string>> GetWorkflowTemplate(string id);
        public Task<HTTPResponse<List<WorkflowTemplateDTO>, string>> GetAllWorkflowTemplates();
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

        public async Task<HTTPResponse<string, string>> CreateWorkflowTemplate(CreateWorkflowTemplatePayload payload, string accountId)
        {
            // ensure there isn't another template with the same name
            var existingTemplate = await _workflowTemplateRepository.GetWorkflowTemplateByName(payload.Name);
            if (existingTemplate != null) return new HTTPResponse<string, string>() { Success = false, HttpCode = 400, Error = "Template name must be unique" };

            if (payload.PreflowNodes.Count > 0 && !payload.IsOnboardingWF) return new HTTPResponse<string, string>() { Success = false, HttpCode = 400, Error = "None onboarding workflows can't have preflow tasks" };
            // create workflow template row
            var workflowTemplate = new WorkflowTemplateTable() { Id = "", Name = payload.Name, Description = payload.Description, IsOnboardingWF = payload.IsOnboardingWF };

            var nodes = new List<WorkflowTemplateNodeTable>();
            var dependencies = new List<NodeTaskDependencyTable>();            

            // loop through the preflow node list and main flow node list
            // for each node:
            // - create a node table class and add it to the list          
            // - look at dependency nodes and create a dependency entity for each dependency
            int index = 0;
            foreach (var preflowNode in payload.PreflowNodes)
            {
                // validate data
                // ensure preflow node (preboarding) isn't refererencing onboarder placeholder name before the onboarder has been invited
                if (preflowNode.AssigneeId == Utility.ONBOARDER_ACCOUNT_ID_PLACEHOLDER)
                {
                    throw new Exception("Onboarder placeholder account Id used in preflow task");
                }
                var node = _mapper.Map<WorkflowTemplateNodeTable>(preflowNode);
                // insert additional data not in payload
                node.Order = index;
                node.WorkflowSection = "preflowtasks";
                // add node to list
                nodes.Add(node);

                // build dependencies between this node and the nodes its dependent on
                foreach(var nodeDependency in preflowNode.DependencyNodeIds)
                {
                    // check node that this node is dependent upon actually exists
                    var otherNode = nodes.FirstOrDefault(n => n.Id == nodeDependency);
                    if (otherNode == null)
                    {
                        return new HTTPResponse<string, string>() { Success = false, HttpCode = 400, Error = "Node dependency with node that doesn't exist" };
                    }
                    var dependency = new NodeTaskDependencyTable()
                    {
                        Id = Guid.NewGuid().ToString(),
                        NodeId = node.Id,
                        DependencyNodeId = otherNode.Id,
                        WorkflowTemplateId = workflowTemplate.Id
                    };
                    dependencies.Add(dependency);
                }
            }

            // build models for mainflow nodes as well
            foreach(var mainflowNode in payload.MainflowNodes)
            {                
                var node = _mapper.Map<WorkflowTemplateNodeTable>(mainflowNode);
                // insert additional data not in payload
                node.Order = index;
                node.WorkflowSection = "mainflowtasks";
                // add node to list
                nodes.Add(node);

                // build dependencies between this node and the nodes its dependent on
                foreach (var nodeDependency in mainflowNode.DependencyNodeIds)
                {
                    // check node that this node is dependent upon actually exists
                    var otherNode = nodes.FirstOrDefault(n => n.Id == nodeDependency);
                    if (otherNode == null)
                    {
                        return new HTTPResponse<string, string>() { Success = false, HttpCode = 400, Error = "Node dependency with node that doesn't exist" };
                    }
                    var dependency = new NodeTaskDependencyTable()
                    {
                        Id = Guid.NewGuid().ToString(),
                        NodeId = node.Id,
                        DependencyNodeId = otherNode.Id,
                        WorkflowTemplateId = workflowTemplate.Id
                    };
                    dependencies.Add(dependency);
                }
            }

            // securely insert data
            var response = await _workflowTemplateRepository.InsertWorkflowTemplate(workflowTemplate, nodes, dependencies);
            if (response.Success == false)
            {
                return new HTTPResponse<string, string>() { Success = false, HttpCode = 500, Error = response.Error };
            }
            return new HTTPResponse<string, string>() { Success = true, HttpCode = 200, Data = "Successfully created workflow template" };
        }

        public async Task<HTTPResponse<List<WorkflowTemplateDTO>, string>> GetAllWorkflowTemplates()
        {
            var workflowTemplateIds = (await _workflowTemplateRepository.GetAll()).Select(t => t.Id);
            var workflowTemplateDTOS = new List<WorkflowTemplateDTO>();
            foreach(var workflowTemplateId in workflowTemplateIds)
            {
                var dto = (await GetWorkflowTemplate(workflowTemplateId)).Data;
                if (dto != null) workflowTemplateDTOS.Add(dto);
            }
            return new HTTPResponse<List<WorkflowTemplateDTO>, string>() { Success = true, HttpCode = 200, Data =  workflowTemplateDTOS };
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
            workflowTemplate.PreflowNodes = preflowTasks.ToList();
            workflowTemplate.MainflowNodes = mainflowTasks.ToList();
            return new HTTPResponse<WorkflowTemplateDTO, string>() { Success = true, HttpCode = 200, Data =  workflowTemplate };
        }

        public async Task<HTTPResponse<string, string>> UpdateWorkflowTemplate(CreateWorkflowTemplatePayload payload, string accountId)
        {


            return new HTTPResponse<string, string>() { Success = true, HttpCode = 200, Data = "Successfully updated workflow template" };
        }
    }
}
