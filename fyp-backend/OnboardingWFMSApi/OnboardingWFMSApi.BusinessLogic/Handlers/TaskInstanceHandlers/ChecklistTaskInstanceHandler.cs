using Microsoft.Extensions.Logging;
using OnboardingWFMSApi.DataAccess.Repositories.Task_Repositories;
using OnboardingWFMSApi.DataModels;
using OnboardingWFMSApi.DataModels.Models;
using OnboardingWFMSApi.DataModels.Tables.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.Marshalling;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace OnboardingWFMSApi.BusinessLogic.Handlers.TaskInstanceHandlers
{
    public interface IChecklistTaskInstanceHandler : ITaskInstanceHandler
    {
    }

    public class ChecklistTaskInstanceHandler : BaseTaskTemplateHandler<ChecklistTaskInstanceTable>, IChecklistTaskInstanceHandler
    {
        private readonly ILogger<ChecklistTaskInstanceHandler> _logger;

        private readonly IChecklistTaskTemplateRepository _checklistTaskTemplateRepository;

        public ChecklistTaskInstanceHandler(IChecklistTaskInstanceRepository repository,
            IChecklistTaskTemplateRepository checklistTaskTemplateRepository, ILogger<ChecklistTaskInstanceHandler> logger) : base(repository, logger)
        {
            _checklistTaskTemplateRepository = checklistTaskTemplateRepository;
            _logger = logger;
        }

        public string GetTaskTypeId()
        {
            return "checklist";
        }

        public override async Task<ServerResponse<string, string>> CreateTaskInstanceData(object taskTemplateMetaData, string taskInstanceId)
        {
            var checklistItems = (taskTemplateMetaData as ChecklistTaskTemplateTable).Items;
            await _repository.AddAsync(new ChecklistTaskInstanceTable() { ItemCompletionStatuses = new bool[checklistItems.Length], TaskInstanceId = taskInstanceId });
            return new ServerResponse<string, string>() { Success = true };
        }

        public override async Task<ServerResponse<string, string>> IsTaskInstanceCompleteable(object taskInstanceMetaData)
        {
            ChecklistTaskInstanceTable taskInstance = CastObjectToType(taskInstanceMetaData);
            if (taskInstance == null)
            {
                return new ServerResponse<string, string>() { Success = false, Error = "Failed to cast task instance data" };
            }

            // ensure all checklist items are complete            
            foreach (var itemStatus in taskInstance.ItemCompletionStatuses)
            {
                if (itemStatus == false)
                {
                    return new ServerResponse<string, string>() { Success = false, Error = "Incomplete checklist item" };
                }
            }
            return new ServerResponse<string, string>() { Success = true };
        }

        public override async Task<ServerResponse<string, string>> ValidateTaskInstanceData(object taskInstanceMetaData)
        {
            // cast object
            ChecklistTaskInstanceTable taskInstance = CastObjectToType(taskInstanceMetaData);
            if (taskInstance == null)
            {
                return new ServerResponse<string, string>() { Success = false, Error = "Failed to cast task instance data" };
            }

            ChecklistTaskTemplateTable taskTemplate = await _checklistTaskTemplateRepository.GetByTaskInstanceId(taskInstance.TaskInstanceId);
            if (taskTemplate == null)
            {
                return new ServerResponse<string, string>() { Success = false, Error = "Failed to cast task template data" };
            }

            // ensure the number of items in instance match number in template
            if (taskInstance.ItemCompletionStatuses.Length != taskTemplate.Items.Length)
            {
                return new ServerResponse<string, string>() { Success = false, Error = "Invalid checklist state" };
            }
            return new ServerResponse<string, string>() { Success = true };
        }
    }
}
