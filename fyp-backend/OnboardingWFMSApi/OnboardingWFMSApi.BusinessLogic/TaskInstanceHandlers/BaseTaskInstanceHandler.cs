using OnboardingWFMSApi.DataAccess.Repositories.Task_Repositories;
using OnboardingWFMSApi.DataAccess.Repositories.Task_Repositories.Interfaces;
using OnboardingWFMSApi.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnboardingWFMSApi.BusinessLogic.TaskInstanceHandlers
{
    public abstract class BaseTaskInstanceHandler<TTaskType> : BaseTaskHandler<TTaskType> where TTaskType : class, ITableEntity
    {
        protected readonly ITaskTypeInstanceRepository<TTaskType> _repository;

        public BaseTaskInstanceHandler(ITaskTypeInstanceRepository<TTaskType> repository)
        {
            _repository = repository;
        }

        public async Task<ServerResponse<object, string>> FetchTaskInstanceData(string taskInstanceId)
        {
            var taskInstanceMetaData = await _repository.GetByTaskInstanceId(taskInstanceId);
            return taskInstanceMetaData == null ? new ServerResponse<object, string>() { Success = false, Error = "Failed to retrieve task type instance data" }
                : new ServerResponse<object, string>() { Success = true, Data = taskInstanceMetaData };
        }

        public async Task<ServerResponse<string, string>> UpdateTaskInstance(object taskInstanceData)
        {
            var updatedTaskInstanceData = CastObjectToType(taskInstanceData);

            var validationResponse = await ValidateTaskInstance(updatedTaskInstanceData);
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

        public abstract Task<ServerResponse<string, string>> ValidateTaskInstance(object taskInstanceData);
    }
}
