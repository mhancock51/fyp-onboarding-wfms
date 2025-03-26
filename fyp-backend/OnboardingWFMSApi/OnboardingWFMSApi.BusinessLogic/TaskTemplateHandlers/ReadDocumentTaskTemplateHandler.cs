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
    public interface IReadDocumentTaskTemplateHandler : ITaskTemplateHandler
    {
    }

    public class ReadDocumentTaskTemplateHandler : BaseTaskTemplateHandler<ReadDocumentTaskTemplateTable>, IReadDocumentTaskTemplateHandler
    {
        public ReadDocumentTaskTemplateHandler(IReadDocumentTaskTemplateRepository repository) : base(repository)
        {
            
        }

        public async Task<ServerResponse<string, string>> CreateTaskTypeMetaData(object taskTypeData, string taskTemplateId)
        {
            if (taskTypeData == null)
            {
                return new ServerResponse<string, string>() { Success = false, Error = "Task type data is null" };
            }
            // cast object
            ReadDocumentTaskTemplateTable readDocumentTaskData = CastObjectToType(taskTypeData);

            // validate 
            var validationResult = await ValidateTaskTypeData(taskTypeData);
            if (!validationResult.Success)
            {
                return new ServerResponse<string, string>() { Success = false, Error = validationResult.Error };
            }

            readDocumentTaskData.TaskTemplateId = taskTemplateId;

            try
            {
                await _repository.AddAsync(readDocumentTaskData);
                return new ServerResponse<string, string>() { Success = true };
            }
            catch (Exception ex)
            {
                return new ServerResponse<string, string>() { Success = false, Error = "Failed to insert checklist data" };
            }
        }

        public string GetTaskTypeId()
        {
            return "read-document";
        }

        public override async Task<ServerResponse<string, string>> ValidateTaskTypeData(object taskTypeData)
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
    }
}
