using Microsoft.Extensions.Logging;
using OnboardingWFMSApi.DataAccess.Repositories.Task_Repositories;
using OnboardingWFMSApi.DataAccess.Repositories.Task_Repositories.Interfaces;
using OnboardingWFMSApi.DataModels;
using OnboardingWFMSApi.DataModels.Tables.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnboardingWFMSApi.BusinessLogic.Handlers.TaskInstanceHandlers
{
    public abstract class BaseTaskTemplateHandler<TTaskType> : BaseTaskHandler<TTaskType> where TTaskType : class, ITableEntity
    {
        protected readonly ITaskTypeInstanceRepository<TTaskType> _repository;
        private readonly ILogger<BaseTaskTemplateHandler<TTaskType>> _logger;

        public BaseTaskTemplateHandler(ITaskTypeInstanceRepository<TTaskType> repository, ILogger<BaseTaskTemplateHandler<TTaskType>> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<ServerResponse<object, string>> FetchTaskInstanceData(string taskInstanceId)
        {
            var taskInstanceMetaData = await _repository.GetByTaskInstanceId(taskInstanceId);
            if (taskInstanceMetaData == null)
            {
                _logger.LogDebug($"Failed to retrieve task instance data for task instance {taskInstanceId}");
            }
            return taskInstanceMetaData == null ? new ServerResponse<object, string>() { Success = false, Error = "Failed to retrieve task type instance data" }
                : new ServerResponse<object, string>() { Success = true, Data = taskInstanceMetaData };
        }

        public virtual async Task<ServerResponse<string, string>> UpdateTaskInstanceData(object taskInstanceData)
        {
            var updatedTaskInstanceData = CastObjectToType(taskInstanceData);

            var validationResponse = await ValidateTaskInstanceData(updatedTaskInstanceData);
            if (!validationResponse.Success)
            {
                return new ServerResponse<string, string>() { Success = false, Data = validationResponse.Data };
            }

            try
            {
                await _repository.UpdateAsync(updatedTaskInstanceData);
                return new ServerResponse<string, string>() { Success = true };
            }
            catch (Exception ex)
            {
                return new ServerResponse<string, string>() { Success = false, Data = ex.Message };
            }
        }

        public abstract Task<ServerResponse<string, string>> ValidateTaskInstanceData(object taskInstanceData);
        public abstract Task<ServerResponse<string, string>> CreateTaskInstanceData(object taskTemplateMetaData, string taskInstanceId);
        public abstract Task<ServerResponse<string, string>> IsTaskInstanceCompleteable(object taskInstanceMetaData);
    }
}
