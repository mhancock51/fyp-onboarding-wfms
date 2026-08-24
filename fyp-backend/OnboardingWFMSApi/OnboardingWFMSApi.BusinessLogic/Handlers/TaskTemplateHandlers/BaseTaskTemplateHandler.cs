using Microsoft.Extensions.Logging;
using OnboardingWFMSApi.DataAccess.Repositories;
using OnboardingWFMSApi.DataAccess.Repositories.Task_Repositories;
using OnboardingWFMSApi.DataModels;
using OnboardingWFMSApi.DataModels.Tables.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnboardingWFMSApi.BusinessLogic.Handlers.TaskTemplateHandlers
{
    public abstract class BaseTaskTemplateHandler<TTaskType> : BaseTaskHandler<TTaskType> where TTaskType : class, ITaskTypeTemplateTable
    {
        protected readonly ITaskTemplateRepository<TTaskType> _repository;
        protected readonly ILogger<BaseTaskTemplateHandler<TTaskType>> _logger;

        public BaseTaskTemplateHandler(ITaskTemplateRepository<TTaskType> repository, ILogger<BaseTaskTemplateHandler<TTaskType>> logger)
        {
            _repository = repository;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<ServerResponse<object, string>> FetchTaskTemplateData(string taskTemplateId)
        {
            try
            {
                var taskInstanceMetaData = await _repository.GetByTaskTemplateId(taskTemplateId);
                return taskInstanceMetaData == null ? new ServerResponse<object, string>() { Success = false, Error = "Failed to retrieve task instance metadata" }
                    : new ServerResponse<object, string>() { Success = true, Data = taskInstanceMetaData };
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"Failed to retrieve task template data for task template {taskTemplateId}: {ex}");
                return new ServerResponse<object, string>() { Success = false, Error = "Failed to retrieve task template data" };
            }            
        }

        public virtual async Task<ServerResponse<string, string>> CreateTaskTemplateData(object taskTypeData, string taskTemplateId)
        {
            if (taskTypeData == null)
            {
                return new ServerResponse<string, string>() { Success = false, Error = "Task type data is null" };
            }
            // cast object
            TTaskType taskData = CastObjectToType(taskTypeData);

            // validate data
            var validationResult = await ValidateTaskTemplateData(taskTypeData);
            if (!validationResult.Success)
            {
                return new ServerResponse<string, string>() { Success = false, Error = validationResult.Error };
            }
            taskData.TaskTemplateId = taskTemplateId;
            try
            {
                // attempt to insert data into db
                await _repository.AddAsync(taskData);
                return new ServerResponse<string, string>() { Success = true };
            }
            catch (Exception ex)
            {
                return new ServerResponse<string, string>() { Success = false, Error = "Failed to insert task type data" };
            }
        }

        public abstract Task<ServerResponse<string, string>> ValidateTaskTemplateData(object taskTypeData);
        public virtual async Task<ServerResponse<string, string>> UpdateTaskTemplateData(object updatedData, object existingData, bool hasActiveInstances)
        {
            TTaskType updatedTaskData = CastObjectToType(updatedData);
            TTaskType existingTaskData = CastObjectToType(existingData);
            if (updatedTaskData.TaskTemplateId != existingTaskData.TaskTemplateId)
            {
                return new ServerResponse<string, string>() { Success = false, Error = "Task templates must match" };
            }
            var validationResponse = await ValidateTaskTemplateData(updatedTaskData);
            if (!validationResponse.Success)
            {
                return new ServerResponse<string, string>() { Success = false, Error = validationResponse.Error };
            }
            try
            {
                // update row
                await _repository.UpdateAsync(updatedTaskData);
                return new ServerResponse<string, string>() { Success = true };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to update task template data (Task Template Id {updatedTaskData.TaskTemplateId}): {ex}");
                return new ServerResponse<string, string>() { Success = false, Error = "Failed to update record" };
            }
        }
    }
}
