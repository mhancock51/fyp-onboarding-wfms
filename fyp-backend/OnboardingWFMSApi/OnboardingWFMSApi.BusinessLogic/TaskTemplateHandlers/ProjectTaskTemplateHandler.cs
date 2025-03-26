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

    public class ProjectTaskTemplateHandler : BaseTaskTemplateHandler<ProjectTaskTemplateTable>, IProjectTaskTemplateHandler
    {
        public ProjectTaskTemplateHandler(IProjectTaskTemplateRepository repository) : base(repository)
        {
            
        }

        public async Task<ServerResponse<string, string>> CreateTaskTypeMetaData(object taskTypeData, string taskTemplateId)
        {
            if (taskTypeData == null)
            {
                return new ServerResponse<string, string>() { Success = false, Error = "Task type data is null" };
            }
            // cast object            
            ProjectTaskTemplateTable projectTaskData = CastObjectToType(taskTypeData);
            
            if (projectTaskData == null)
            {
                return new ServerResponse<string, string>() { Success = false, Error = "Invalid task type data provided" };
            }
            var validationResult = await ValidateTaskTypeData(projectTaskData);
            if (!validationResult.Success)
            {
                return new ServerResponse<string, string>() { Success = false, Error = validationResult.Error };
            }
            projectTaskData.Id = "";
            projectTaskData.TaskTemplateId = taskTemplateId;
            // insert record
            try
            {
                await _repository.AddAsync(projectTaskData);
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

        public override async Task<ServerResponse<string, string>> ValidateTaskTypeData(object taskTypeData)
        {
            // cast object
            ProjectTaskTemplateTable projectTaskData = CastObjectToType(taskTypeData);

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
                if (string.IsNullOrEmpty(objective.Objective)) return new ServerResponse<string, string>() { Success = false, Error = "Objective is empty" };
            }

            return new ServerResponse<string, string>() { Success = true };
        }
    }
}
