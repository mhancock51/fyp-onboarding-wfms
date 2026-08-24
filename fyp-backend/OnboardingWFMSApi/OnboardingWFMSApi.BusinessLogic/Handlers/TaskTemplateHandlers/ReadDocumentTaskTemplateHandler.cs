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

namespace OnboardingWFMSApi.BusinessLogic.Handlers.TaskTemplateHandlers
{
    public interface IReadDocumentTaskTemplateHandler : ITaskTemplateHandler
    {
    }

    public class ReadDocumentTaskTemplateHandler : BaseTaskTemplateHandler<ReadDocumentTaskTemplateTable>, IReadDocumentTaskTemplateHandler
    {
        public ReadDocumentTaskTemplateHandler(IReadDocumentTaskTemplateRepository repository, ILogger<BaseTaskTemplateHandler<ReadDocumentTaskTemplateTable>> logger) : base(repository, logger)
        {

        }

        public string GetTaskTypeId()
        {
            return "read-document";
        }

        public override async Task<ServerResponse<string, string>> ValidateTaskTemplateData(object taskTypeData)
        {
            // cast object            
            ReadDocumentTaskTemplateTable readDocumentTaskData = CastObjectToType(taskTypeData);

            if (string.IsNullOrEmpty(readDocumentTaskData.DocumentName))
            {
                return new ServerResponse<string, string>() { Success = false, Error = "Document name must be provided" };
            }
            if (string.IsNullOrEmpty(readDocumentTaskData.DocumentUrl))
            {
                return new ServerResponse<string, string>() { Success = false, Error = "Document URL must be provided" };
            }
            if (string.IsNullOrEmpty(readDocumentTaskData.CheckBoxLabel))
            {
                return new ServerResponse<string, string>() { Success = false, Error = "Checkbox label must be provided" };
            }
            return new ServerResponse<string, string>() { Success = true, Data = "Task Type metadata validated successfully" };
        }

        public override async Task<ServerResponse<string, string>> UpdateTaskTemplateData(object updatedData, object existingData, bool hasActiveInstances)
        {
            var updatedTaskData = CastObjectToType(updatedData);
            var existingTaskData = CastObjectToType(existingData);

            if (updatedTaskData.CheckBoxLabel != existingTaskData.CheckBoxLabel)
            {
                return new ServerResponse<string, string>() { Success = false, Error = "Checkbox label can't be changed" };
            }
            if (updatedTaskData.DocumentName != updatedTaskData.DocumentName)
            {
                return new ServerResponse<string, string>() { Success = false, Error = "Document name can't be changed" };
            }

            // call and return result from base method to do base validation checks and then update record
            return await base.UpdateTaskTemplateData(updatedData, existingData, hasActiveInstances);
        }
    }
}
