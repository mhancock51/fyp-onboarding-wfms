using OnboardingWFMSApi.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnboardingWFMSApi.BusinessLogic.TaskTemplateHandlers
{
    public interface ITaskTemplateHandler
    {
        public string GetTaskTypeId();
        public Task<ServerResponse<string, string>> CreateTaskTemplateData(object taskTypeData, string taskTemplateId);
        public Task<ServerResponse<object, string>> FetchTaskTemplateData(string taskTemplateId);
        /// <summary>
        /// Validate the data passed as metadata for the task template's speicific type, i.e. checklist data, document upload data, etc.
        /// </summary>
        /// <param name="taskTypeData"></param>
        /// <returns></returns>
        public Task<ServerResponse<string, string>> ValidateTaskTemplateData(object taskTypeData);
    }
}
