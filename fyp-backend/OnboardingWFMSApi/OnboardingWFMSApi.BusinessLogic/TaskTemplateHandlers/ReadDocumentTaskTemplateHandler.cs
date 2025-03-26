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
