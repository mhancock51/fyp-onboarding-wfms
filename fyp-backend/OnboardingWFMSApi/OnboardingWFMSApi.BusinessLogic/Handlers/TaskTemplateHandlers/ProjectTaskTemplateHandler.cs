using Microsoft.Extensions.Logging;
using OnboardingWFMSApi.DataAccess.Repositories;
using OnboardingWFMSApi.DataAccess.Repositories.Task_Repositories;
using OnboardingWFMSApi.DataModels;
using OnboardingWFMSApi.DataModels.Tables.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace OnboardingWFMSApi.BusinessLogic.Handlers.TaskTemplateHandlers
{
    public interface IProjectTaskTemplateHandler : ITaskTemplateHandler
    {

    }

    public class ProjectTaskTemplateHandler : BaseTaskTemplateHandler<ProjectTaskTemplateTable>, IProjectTaskTemplateHandler
    {
        public ProjectTaskTemplateHandler(ITaskTemplateRepository<ProjectTaskTemplateTable> repository, ILogger<BaseTaskTemplateHandler<ProjectTaskTemplateTable>> logger) : base(repository, logger)
        {
        }

        public string GetTaskTypeId()
        {
            return "project-task";
        }

        public override async Task<ServerResponse<string, string>> ValidateTaskTemplateData(object taskTypeData)
        {
            // cast object
            ProjectTaskTemplateTable projectTaskData = CastObjectToType(taskTypeData);

            // validate values
            if (string.IsNullOrEmpty(projectTaskData.Brief))
            {
                return new ServerResponse<string, string>() { Success = false, Error = "Please provide a project breif" };
            }
            if (string.IsNullOrEmpty(projectTaskData.Deliverable))
            {
                return new ServerResponse<string, string>() { Success = false, Error = "Please provide a deliverable" };
            }
            if (projectTaskData.Objectives.Count == 0)
            {
                return new ServerResponse<string, string>() { Success = false, Error = "Please provide objectives" };
            }
            foreach (var objective in projectTaskData.Objectives)
            {
                if (string.IsNullOrEmpty(objective.Objective)) return new ServerResponse<string, string>() { Success = false, Error = "Objective is empty" };
            }

            return new ServerResponse<string, string>() { Success = true };
        }

        public override async Task<ServerResponse<string, string>> UpdateTaskTemplateData(object updatedData, object existingData, bool hasActiveInstances)
        {
            var updatedProjectData = CastObjectToType(updatedData);
            var existingProjectData = CastObjectToType(existingData);
            // don't allow deliverable to change if instances exist
            if (updatedProjectData.Deliverable != existingProjectData.Deliverable)
            {
                return new ServerResponse<string, string>() { Success = false, Error = "Deliverable can't change if instances exist" };
            }
            // don't allow change to objectives length if instances exist
            if (updatedProjectData.Objectives.Count != existingProjectData.Objectives.Count && hasActiveInstances)
            {
                return new ServerResponse<string, string>() { Success = false, Error = "Objectives can't be changed if instances exist" };
            }

            // run base method to do base validation checks and then update record
            return await base.UpdateTaskTemplateData(updatedData, existingData, hasActiveInstances);
        }
    }
}
