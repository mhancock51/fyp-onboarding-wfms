using Microsoft.Extensions.Logging;
using OnboardingWFMSApi.DataAccess.Repositories.Task_Repositories.Interfaces;
using OnboardingWFMSApi.DataModels;
using OnboardingWFMSApi.DataModels.Tables.Interfaces;

namespace OnboardingWFMSApi.BusinessLogic.Handlers.TaskInstanceHandlers
{
    public abstract class BaseTaskInstanceHandler<TTaskType> : BaseTaskHandler<TTaskType> where TTaskType : class, ITableEntity
    {
        protected readonly ITaskTypeInstanceRepository<TTaskType> _repository;
        private readonly ILogger<BaseTaskInstanceHandler<TTaskType>> _logger;

        public BaseTaskInstanceHandler(ITaskTypeInstanceRepository<TTaskType> repository, ILogger<BaseTaskInstanceHandler<TTaskType>> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<ServerResponse<object, string>> FetchTaskInstanceData(string taskInstanceId)
        {
            TTaskType taskInstanceData = await _repository.GetByTaskInstanceId(taskInstanceId);
            if (taskInstanceData == null)
            {
                _logger.LogDebug($"Failed to retrieve task instance data for task instance {taskInstanceId}");
            }
            return taskInstanceData == null ? new ServerResponse<object, string>() { Success = false, Error = "Failed to retrieve task type instance data" }
                : new ServerResponse<object, string>() { Success = true, Data = taskInstanceData };
        }

        public virtual async Task<ServerResponse<string, string>> UpdateTaskInstanceData(object taskInstanceData)
        {
            TTaskType updatedTaskInstanceData = CastObjectToType(taskInstanceData);

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
                _logger.LogError($"Error updating task instance data: {ex}");
                return new ServerResponse<string, string>() { Success = false};
            }
        }

        public abstract Task<ServerResponse<string, string>> ValidateTaskInstanceData(object taskInstanceData);
        public abstract Task<ServerResponse<string, string>> CreateTaskInstanceData(object taskTemplateData, string taskInstanceId);
        public abstract Task<ServerResponse<string, string>> IsTaskInstanceCompleteable(object taskInstanceData);
    }
}
