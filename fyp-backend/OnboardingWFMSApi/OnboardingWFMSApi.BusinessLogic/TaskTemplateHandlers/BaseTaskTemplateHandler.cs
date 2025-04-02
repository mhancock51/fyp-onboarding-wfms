using OnboardingWFMSApi.DataAccess.Repositories;
using OnboardingWFMSApi.DataAccess.Repositories.Task_Repositories;
using OnboardingWFMSApi.DataModels;
using OnboardingWFMSApi.DataModels.Tables.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnboardingWFMSApi.BusinessLogic.TaskTemplateHandlers
{
    public abstract class BaseTaskTemplateHandler<TTaskType> : BaseTaskHandler<TTaskType> where TTaskType : class, ITaskTypeTemplateTable
    {
        protected readonly ITaskTemplateRepository<TTaskType> _repository;

        public BaseTaskTemplateHandler(ITaskTemplateRepository<TTaskType> repository)
        {
            _repository = repository;
        }

        public async Task<ServerResponse<object, string>> FetchTaskTemplateData(string taskTemplateId)
        {
            // implement generic method to retrieve task type data, i.e. checklist data, project data, etc.
            var taskInstanceMetaData = await _repository.GetByTaskTemplateId(taskTemplateId);
            return taskInstanceMetaData == null ? new ServerResponse<object, string>() { Success = false, Error = "Failed to retrieve task instance metadata" }
                : new ServerResponse<object, string>() { Success = true, Data = taskInstanceMetaData };
        }

        public virtual async Task<ServerResponse<string, string>> CreateTaskTemplateData(object taskTypeData, string taskTemplateId)
        {
            if (taskTypeData == null)
            {
                return new ServerResponse<string, string>() { Success = false, Error = "Task type data is null" };
            }
            // cast object
            TTaskType taskData = CastObjectToType(taskTypeData);
            // validate
            var validationResult = await ValidateTaskTemplateData(taskTypeData);
            if (!validationResult.Success)
            {
                return new ServerResponse<string, string>() { Success = false, Error = validationResult.Error };
            }
            // attempt to insert data into db
            taskData.TaskTemplateId = taskTemplateId;
            try
            {
                await _repository.AddAsync(taskData);
                return new ServerResponse<string, string>() { Success = true };
            }
            catch (Exception ex)
            {
                return new ServerResponse<string, string>() { Success = false, Error = "Failed to insert task type data"};
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
        }
    }
}
