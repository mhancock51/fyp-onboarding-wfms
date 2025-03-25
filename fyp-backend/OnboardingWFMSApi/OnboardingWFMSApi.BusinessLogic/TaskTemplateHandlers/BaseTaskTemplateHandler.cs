using OnboardingWFMSApi.DataAccess.Repositories;
using OnboardingWFMSApi.DataAccess.Repositories.Task_Repositories;
using OnboardingWFMSApi.DataModels;
using OnboardingWFMSApi.DataModels.Tables.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnboardingWFMSApi.BusinessLogic.TaskTemplateHandlers
{
    public class BaseTaskTemplateHandler<TTaskType> : BaseTaskHandler<TTaskType> where TTaskType : class, ITableEntity
    {
        protected readonly ITaskTemplateRepository<TTaskType> _repository;

        public BaseTaskTemplateHandler(ITaskTemplateRepository<TTaskType> repository)
        {
            _repository = repository;
        }

        public async Task<ServerResponse<object, string>> GetTaskTypeData(string taskInstanceId)
        {
            // implement generic method to retrieve task type data, i.e. checklist data, project data, etc.
            var taskInstanceMetaData = await _repository.GetByTaskInstanceId(taskInstanceId);
            return taskInstanceMetaData == null ? new ServerResponse<object, string>() { Success = false, Error = "Failed to retrieve task instance metadata" }
                : new ServerResponse<object, string>() { Success = true, Data = taskInstanceMetaData };
        }
    }
}
