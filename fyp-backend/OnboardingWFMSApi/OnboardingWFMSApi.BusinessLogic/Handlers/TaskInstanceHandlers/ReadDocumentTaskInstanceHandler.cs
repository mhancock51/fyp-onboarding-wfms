using Microsoft.Extensions.Logging;
using OnboardingWFMSApi.DataAccess.Repositories.Task_Repositories;
using OnboardingWFMSApi.DataModels;
using OnboardingWFMSApi.DataModels.Models;
using OnboardingWFMSApi.DataModels.Tables.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace OnboardingWFMSApi.BusinessLogic.Handlers.TaskInstanceHandlers
{
    public interface IReadDocumentTaskInstanceHandler : ITaskInstanceHandler
    {

    }

    public class ReadDocumentTaskInstanceHandler : BaseTaskTemplateHandler<ReadDocumentTaskInstanceTable>, IReadDocumentTaskInstanceHandler
    {
        public ReadDocumentTaskInstanceHandler(IReadDocumentTaskInstanceRepository repository,
            ILogger<ReadDocumentTaskInstanceHandler> logger) : base(repository, logger)
        {

        }

        public string GetTaskTypeId()
        {
            return "read-document";
        }

        public override async Task<ServerResponse<string, string>> CreateTaskInstanceData(object taskTemplateMetaData, string taskInstanceId)
        {
            await _repository.AddAsync(new ReadDocumentTaskInstanceTable() { TaskInstanceId = taskInstanceId });
            return new ServerResponse<string, string>() { Success = true };
        }

        public override async Task<ServerResponse<string, string>> IsTaskInstanceCompleteable(object taskInstanceMetaData)
        {
            var taskInstance = CastObjectToType(taskInstanceMetaData);
            if (taskInstance == null)
            {
                return new ServerResponse<string, string>() { Success = false, Error = "Failed to cast task instance data" };
            }

            // ensure document has been opened and checkbox has been checked
            if (!taskInstance.CheckboxChecked)
            {
                return new ServerResponse<string, string>() { Success = false, Error = "Checkbox must be checked" };
            }
            if (!taskInstance.LinkClicked)
            {
                return new ServerResponse<string, string>() { Success = false, Error = "Link must have been clicked" };
            }

            return new ServerResponse<string, string>() { Success = true };
        }

        public override async Task<ServerResponse<string, string>> ValidateTaskInstanceData(object taskInstanceMetaData)
        {
            // No validation to be done
            return new ServerResponse<string, string>() { Success = true };
        }
    }
}
