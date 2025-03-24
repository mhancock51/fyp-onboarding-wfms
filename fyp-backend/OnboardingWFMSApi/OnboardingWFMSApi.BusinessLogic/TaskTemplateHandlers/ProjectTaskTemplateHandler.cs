using OnboardingWFMSApi.DataAccess.Repositories.Task_Repositories;
using OnboardingWFMSApi.DataModels;
using OnboardingWFMSApi.DataModels.Tables.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace OnboardingWFMSApi.BusinessLogic.TaskTemplateHandlers
{
    public interface IProjectTaskTemplateHandler : ITaskTemplateHandler
    {

    }

    public class ProjectTaskTemplateHandler : IProjectTaskTemplateHandler
    {
        private readonly IProjectTaskTemplateRepository _projectTaskTemplateRepository;

        public ProjectTaskTemplateHandler(IProjectTaskTemplateRepository projectTaskTemplateRepository)
        {
            _projectTaskTemplateRepository = projectTaskTemplateRepository;
        }

        public async Task<ServerResponse<string, string>> CreateTaskTypeMetaData(object taskTypeData, string taskTemplateId)
        {
            if (taskTypeData == null)
            {
                return new ServerResponse<string, string>() { Success = false, Error = "Task type data is null" };
            }
            var projectTaskData = JsonSerializer.Deserialize<ProjectTaskTemplateTable>(taskTypeData.ToString());
            if (projectTaskData == null)
            {
                return new ServerResponse<string, string>() { Success = false, Error = "Invalid task type data provided" };
            }
            var validationResult = await ValidateTaskTypeMetaData(projectTaskData);
            if (!validationResult.Success)
            {
                return new ServerResponse<string, string>() { Success = false, Error = validationResult.Error };
            }
            projectTaskData.Id = "";
            projectTaskData.TaskTemplateId = taskTemplateId;
            // insert record
            try
            {
                await _projectTaskTemplateRepository.AddAsync(projectTaskData);
                return new ServerResponse<string, string>() { Success = true };
            }
            catch (Exception ex)
            {
                return new ServerResponse<string, string>() { Success = false, Error = "Failed to insert project task data" };
            }
        }

        public string GetTaskTypeId()
        {
            return "project-task";
        }

        public async Task<ServerResponse<object, string>> GetTaskTypeMetaData(string taskTemplateId)
        {
            var projectTaskData = await _projectTaskTemplateRepository.GetByTaskTemplateId(taskTemplateId);
            return projectTaskData == null ?
                new ServerResponse<object, string>() { Success = false, Error = "Failed to retrieve project task data" } :
                new ServerResponse<object, string>() { Success = true, Data = projectTaskData };
        }

        public async Task<ServerResponse<string, string>> ValidateTaskTypeMetaData(object taskTypeData)
        {
            if (taskTypeData.GetType() != typeof(ProjectTaskTemplateTable))
            {
                return new ServerResponse<string, string>() { Success = false, Error = "Invalid task type data provided" };
            }
            ProjectTaskTemplateTable projectTaskData = taskTypeData as ProjectTaskTemplateTable;

            // validate values
            if (string.IsNullOrEmpty(projectTaskData.Brief))
            {
                return new ServerResponse<string, string>() { Success = false, Error = "Please provide a project breif" };
            }
            if (projectTaskData.Objectives.Count == 0)
            {
                return new ServerResponse<string, string>() { Success = false, Error = "Please provide objectives" };
            }
            foreach(var objective in projectTaskData.Objectives)
            {
                if (string.IsNullOrEmpty(objective)) return new ServerResponse<string, string>() { Success = false, Error = "Objective is empty" };
            }

            return new ServerResponse<string, string>() { Success = true };
        }
    }
}
