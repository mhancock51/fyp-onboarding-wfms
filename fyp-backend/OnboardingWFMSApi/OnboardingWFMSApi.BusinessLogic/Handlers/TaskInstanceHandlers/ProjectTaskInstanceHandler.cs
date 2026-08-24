using Microsoft.Extensions.Logging;
using OnboardingWFMSApi.DataAccess.Repositories.Task_Repositories;
using OnboardingWFMSApi.DataModels;
using OnboardingWFMSApi.DataModels.Tables.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace OnboardingWFMSApi.BusinessLogic.Handlers.TaskInstanceHandlers
{
    public interface IProjectTaskInstanceHandler : ITaskInstanceHandler
    {

    }
    public class ProjectTaskInstanceHandler : BaseTaskInstanceHandler<ProjectTaskInstanceTable>, IProjectTaskInstanceHandler
    {
        private readonly IProjectTaskTemplateRepository _projectTaskTemplateRepository;
        private readonly ITaskInstanceRepository _taskInstanceRepository;

        public ProjectTaskInstanceHandler(IProjectTaskInstanceRepository repository, IProjectTaskTemplateRepository projectTaskTemplateRepository,
            ITaskInstanceRepository taskInstanceRepository, ILogger<ProjectTaskInstanceHandler> logger) : base(repository, logger)
        {
            _projectTaskTemplateRepository = projectTaskTemplateRepository;
            _taskInstanceRepository = taskInstanceRepository;
        }

        public string GetTaskTypeId()
        {
            return "project-task";
        }

        public override async Task<ServerResponse<string, string>> CreateTaskInstanceData(object taskTemplateData, string taskInstanceId)
        {
            if (taskTemplateData is not ProjectTaskTemplateTable projectTemplate)
            {
                return new ServerResponse<string, string> { Success = false, Error = "Invalid task template data" };
            }

            // create project task instance
            var objectiveStates = new bool[projectTemplate.Objectives.Count];
            var instance = new ProjectTaskInstanceTable
            {
                Id = string.Empty,
                ObjectiveStates = objectiveStates.ToList(),
                TaskInstanceId = taskInstanceId
            };

            try
            {
                // insert new instance
                await _repository.AddAsync(instance);
            }
            catch (Exception ex)
            {
                return new ServerResponse<string, string>() { Success = false, Error = $"Failed to insert project task instance data: {ex}" };
            }

            return new ServerResponse<string, string> { Success = true };
        }

        public override async Task<ServerResponse<string, string>> IsTaskInstanceCompleteable(object taskInstanceData)
        {
            if (taskInstanceData is not ProjectTaskInstanceTable projectInstance)
            {
                return new ServerResponse<string, string> { Success = false, Error = "Failed to cast task instance data" };
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

        public override async Task<ServerResponse<string, string>> ValidateTaskInstanceData(object taskInstanceData)
        {
            // cast object            
            ProjectTaskInstanceTable taskInstance = CastObjectToType(taskInstanceData);
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
