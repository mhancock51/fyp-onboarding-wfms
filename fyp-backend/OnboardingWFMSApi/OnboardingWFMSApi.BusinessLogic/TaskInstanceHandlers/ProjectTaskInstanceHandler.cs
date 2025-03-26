using OnboardingWFMSApi.DataAccess.Repositories.Task_Repositories;
using OnboardingWFMSApi.DataModels;
using OnboardingWFMSApi.DataModels.Tables.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace OnboardingWFMSApi.BusinessLogic.TaskInstanceHandlers
{
    public interface IProjectTaskInstanceHandler : ITaskInstanceHandler
    {

    }
    public class ProjectTaskInstanceHandler : BaseTaskInstanceHandler<ProjectTaskInstanceTable>, IProjectTaskInstanceHandler
    {
        private readonly IProjectTaskInstanceRepository _repository;
        private readonly IProjectTaskTemplateRepository _projectTaskTemplateRepository;
        private readonly ITaskInstanceRepository _taskInstanceRepository;

        public ProjectTaskInstanceHandler(IProjectTaskInstanceRepository repository, IProjectTaskTemplateRepository projectTaskTemplateRepository, ITaskInstanceRepository taskInstanceRepository) : base(repository)
        {            
            _projectTaskTemplateRepository = projectTaskTemplateRepository;
            _taskInstanceRepository = taskInstanceRepository;
        }

        public string GetTaskTypeId()
        {
            return "project-task";
        }

        public async Task<ServerResponse<string, string>> InsertTaskInstanceMetaData(object taskTemplateMetaData, string taskInstanceId)
        {
            var objectives = (taskTemplateMetaData as ProjectTaskTemplateTable).Objectives;
            await _repository.AddAsync(new ProjectTaskInstanceTable() { Id = "", ObjectiveStates = (new bool[objectives.Count]).ToList(), TaskInstanceId = taskInstanceId });            
            return new ServerResponse<string, string>() { Success = true };
        }

        public async Task<ServerResponse<string, string>> IsTaskInstanceCompleteable(object taskInstanceMetaData)
        {
            var projectInstance = taskInstanceMetaData as ProjectTaskInstanceTable;
            if (projectInstance == null)
            {
                return new ServerResponse<string, string>() { Success = false, Error = "Failed to cast task instance data" };
            }
            var taskInstance = await _taskInstanceRepository.GetById(projectInstance.TaskInstanceId);
            var taskTemplate = await _projectTaskTemplateRepository.GetByTaskTemplateId(taskInstance.TaskTemplateId);

            // ensure all checklist items are complete            
            for (int i = 0; i < projectInstance.ObjectiveStates.Count; i++)
            {
                if (projectInstance.ObjectiveStates[i] == false && taskTemplate.Objectives[i].Required)
                {
                    return new ServerResponse<string, string>() { Success = false, Error = "Incomplete required objective" };
                }
            }
            return new ServerResponse<string, string>() { Success = true };
        }

        public override async Task<ServerResponse<string, string>> ValidateTaskInstance(object taskInstanceMetaData)
        {
            // cast object            
            ProjectTaskInstanceTable taskInstance = CastObjectToType(taskInstanceMetaData);
            if (taskInstance == null)
            {
                return new ServerResponse<string, string>() { Success = false, Error = "Failed to cast task instance data" };
            }

            var projectTemplateData = await _projectTaskTemplateRepository.GetByTaskInstanceId(taskInstance.TaskInstanceId);
            if (projectTemplateData == null)
            {
                return new ServerResponse<string, string>() { Success = false, Error = "Failed to cast task template data" };
            }

            // ensure instance data has same number of objective state items as the template has objective items
            if (taskInstance.ObjectiveStates.Count != projectTemplateData.Objectives.Count)
            {
                return new ServerResponse<string, string>() { Success = false, Error = "Invalid objectives state" };
            }
            return new ServerResponse<string, string>() { Success = true };
        }
    }
}
