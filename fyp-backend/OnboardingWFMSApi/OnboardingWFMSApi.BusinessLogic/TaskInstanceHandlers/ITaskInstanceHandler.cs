using OnboardingWFMSApi.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnboardingWFMSApi.BusinessLogic.TaskInstanceHandlers
{
    public interface ITaskInstanceHandler
    {
        public string GetTaskTypeId();
        public Task<ServerResponse<string, string>> CreateTaskInstanceData(object taskTemplateMetaData, string taskInstanceId);
        public Task<ServerResponse<string, string>> UpdateTaskInstanceData(object updatedTaskInstanceMetaData);
        public Task<ServerResponse<string, string>> ValidateTaskInstanceData(object taskInstanceMetaData);
        public Task<ServerResponse<string, string>> IsTaskInstanceCompleteable(object taskInstanceMetaData);        
        public Task<ServerResponse<object, string>> FetchTaskInstanceData(string taskInstanceId);
    }
}
