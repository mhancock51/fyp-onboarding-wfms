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
