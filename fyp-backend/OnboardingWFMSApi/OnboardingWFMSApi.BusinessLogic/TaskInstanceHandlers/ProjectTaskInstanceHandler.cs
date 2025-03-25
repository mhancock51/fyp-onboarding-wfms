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
    public class ProjectTaskInstanceHandler : IProjectTaskInstanceHandler
    {
        private readonly IProjectTaskInstanceRepository _projectTaskInstanceRepository;
        private readonly IProjectTaskTemplateRepository _projectTaskTemplateRepository;
        private readonly ITaskInstanceRepository _taskInstanceRepository;

        public ProjectTaskInstanceHandler(IProjectTaskInstanceRepository projectTaskInstanceRepository, IProjectTaskTemplateRepository projectTaskTemplateRepository, ITaskInstanceRepository taskInstanceRepository)
        {
            _projectTaskInstanceRepository = projectTaskInstanceRepository;
            _projectTaskTemplateRepository = projectTaskTemplateRepository;
            _taskInstanceRepository = taskInstanceRepository;
        }

        public async Task<ServerResponse<object, string>> GetTaskInstanceMetaData(string taskInstanceId)
        {
            var projectInstance = await _projectTaskInstanceRepository.GetByTaskInstanceId(taskInstanceId);
            return projectInstance == null ? new ServerResponse<object, string>() { Success = false, Error = "Failed to retrieve task instance metadata" }
                : new ServerResponse<object, string>() { Success = true, Data = projectInstance };
        }

        public string GetTaskTypeId()
        {
            return "project-task";
        }

        public async Task<ServerResponse<string, string>> InsertTaskInstanceMetaData(object taskTemplateMetaData, string taskInstanceId)
        {
            var objectives = (taskTemplateMetaData as ProjectTaskTemplateTable).Objectives;
            await _projectTaskInstanceRepository.AddAsync(new ProjectTaskInstanceTable() { Id = "", ObjectiveStates = (new bool[objectives.Count]).ToList(), TaskInstanceId = taskInstanceId });            
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

        public async Task<ServerResponse<string, string>> UpdateTaskInstanceMetaData(object updatedTaskInstanceMetaData)
        {
            // cast object
            JsonElement jsonElement = (JsonElement)updatedTaskInstanceMetaData;
            ProjectTaskInstanceTable updatedTaskInstance = jsonElement.Deserialize<ProjectTaskInstanceTable>();
            
            // validate updated meta data before inserting
            var validationResponse = await ValidateTaskInstanceMetaData(updatedTaskInstanceMetaData);
            if (!validationResponse.Success)
            {
                return new ServerResponse<string, string>() { Success = false, Data = validationResponse.Data };
            }
            // update task instance                        
            await _projectTaskInstanceRepository.UpdateAsync(updatedTaskInstance);

            return new ServerResponse<string, string>() { Success = true };
        }

        public async Task<ServerResponse<string, string>> ValidateTaskInstanceMetaData(object taskInstanceMetaData)
        {
            // cast object
            JsonElement jsonElement = (JsonElement)taskInstanceMetaData;
            ProjectTaskInstanceTable projectInstance = jsonElement.Deserialize<ProjectTaskInstanceTable>();
            if (projectInstance == null)
            {
                return new ServerResponse<string, string>() { Success = false, Error = "Failed to cast task instance data" };
            }

            var projectTemplateData = await _projectTaskTemplateRepository.GetByTaskInstanceId(projectInstance.TaskInstanceId);
            if (projectTemplateData == null)
            {
                return new ServerResponse<string, string>() { Success = false, Error = "Failed to cast task template data" };
            }

            // ensure instance data has same number of objective state items as the template has objective items
            if (projectInstance.ObjectiveStates.Count != projectTemplateData.Objectives.Count)
            {
                return new ServerResponse<string, string>() { Success = false, Error = "Invalid objectives state" };
            }
            return new ServerResponse<string, string>() { Success = true };
        }
    }
}
