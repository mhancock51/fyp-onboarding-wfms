using AutoMapper;
using OnboardingWFMSApi.DataAccess.Repositories;
using OnboardingWFMSApi.DataAccess.Repositories.Task_Repositories;
using OnboardingWFMSApi.DataModels;
using OnboardingWFMSApi.DataModels.Models;
using OnboardingWFMSApi.DataModels.Payloads;
using OnboardingWFMSApi.DataModels.Tables;
using OnboardingWFMSApi.DataModels.Tables.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnboardingWFMSApi.BusinessLogic
{
    public interface ITaskInstanceLogic
    {
        public Task<HTTPResponse<string, string>> CreateInstance(CreateTaskInstancePayload payload);
        public Task<HTTPResponse<List<TaskInstance>, string>> GetUsersAssignedTask(string accountId);
        public Task<HTTPResponse<TaskInstance, string>> GetTaskInstance(string tasksInstanceId);

        public Task<HTTPResponse<string, string>> UpdateChecklistInstanceState(UpdateInstanceStateChecklistPayload payload, string accountId);
        public Task<HTTPResponse<string, string>> CompleteTaskInstance(string taskInstanceId, string accountId);
    }
    public class TaskInstanceLogic : ITaskInstanceLogic
    {
        private string OPEN_TASK_STATUS = "open";
        private string COMPLETED_TASK_STATUS = "complete";

        private readonly IMapper _mapper;
        private readonly ITaskTemplateLogic _taskTemplateLogic;

        private readonly ITaskInstanceRepository _taskInstanceRepository;
        private readonly ITaskTemplateRepository _taskTemplateRepository;
        private readonly IAccountRepository _accountRepository;

        private readonly IChecklistTaskInstanceRepository _checklistTaskInstanceRepository;

        public TaskInstanceLogic(ITaskInstanceRepository taskInstanceRepository, IMapper mapper, ITaskTemplateLogic taskTemplateLogic, IAccountRepository accountRepository, ITaskTemplateRepository taskTemplateRepository,
            IChecklistTaskInstanceRepository checklistTaskInstanceRepository
        )
        {
            _taskInstanceRepository = taskInstanceRepository;
            _mapper = mapper;
            _taskTemplateLogic = taskTemplateLogic;
            _accountRepository = accountRepository;
            _checklistTaskInstanceRepository = checklistTaskInstanceRepository;
            _taskTemplateRepository = taskTemplateRepository;
        }

        public async Task<HTTPResponse<string, string>> CreateInstance(CreateTaskInstancePayload payload)
        {
            var instance = new TaskInstanceTable()
            {
                AssigneeAccountId = payload.AssigneeAccountId,
                AssignerAccountId = payload.AssignerAccountId,
                TaskTemplateId = payload.TaskTemplateId,
                CreationTimestamp = DateTime.Now,
                Status = OPEN_TASK_STATUS
            };
            var taskInstance = await _taskInstanceRepository.AddAsync(instance);

            // load template
            var response = await _taskTemplateLogic.GetTaskTemplateById(instance.TaskTemplateId); ;
            var taskTemplate = response.Data;
            // create instance of task type data
            // i.e. checklist task -> create row in ChecklistTaskInstance repo
            switch(taskTemplate.TaskTypeId)
            {
                case TaskTemplateLogic.CHECKLIST_TASK_TYPE_ID:
                    var checklistItems = (taskTemplate.TaskTypeData as ChecklistTaskTemplateTable).Items;
                    await _checklistTaskInstanceRepository.AddAsync(new ChecklistTaskInstanceTable() { ItemCompletionStatuses = new bool[checklistItems.Length], TaskInstanceId = taskInstance.Id });
                    break;
                case TaskTemplateLogic.READ_DOCUMENT_TASK_TYPE_ID:
                    // TODO
                    break;
                case TaskTemplateLogic.UPLOAD_DOCUMENT_TASK_TYPE_ID:
                    // TODO
                    break;
                default:
                    throw new Exception("Tasks template has an invalid task type");
            }            

            return new HTTPResponse<string, string>() { Success = true, HttpCode = 200, Data = "Successfully created task instance" };
        }

        public async Task<HTTPResponse<List<TaskInstance>, string>> GetUsersAssignedTask(string accountId)
        {
            var taskInstances = _mapper.Map<List<TaskInstance>>(await _taskInstanceRepository.GetUsersTaskInstances(accountId));               
            for (var i = 0; i < taskInstances.Count; i++)
            {
                // add template and task type instance data
                var response = await GetTaskInstance(taskInstances[i].Id);
                if (response.Success)
                {
                    taskInstances[i] = response.Data;
                }
                else
                {
                    throw new Exception("Task instance isn't associated with a valid task template or task type instance");
                }
            }
            return new HTTPResponse<List<TaskInstance>, string>() { Success = true, HttpCode = 200, Data = taskInstances };
        }

        public async Task<HTTPResponse<TaskInstance, string>> GetTaskInstance(string tasksInstanceId)
        {
            var taskInstance = _mapper.Map<TaskInstance>(await _taskInstanceRepository.GetById(tasksInstanceId));
            if (taskInstance == null)
            {
                return new HTTPResponse<TaskInstance, string>() { Success = false, Error = "No task instance found", HttpCode = 400 };
            }

            // get template data
            var response = await _taskTemplateLogic.GetTaskTemplateById(taskInstance.TaskTemplateId);
            if (response.Success)
            {
                taskInstance.template = response.Data;
            }
            else
            {
                throw new Exception("Task instance isn't associated with a valid task template");
            }
            // get task type data
            switch(taskInstance.template.TaskTypeId)
            {
                case TaskTemplateLogic.CHECKLIST_TASK_TYPE_ID:
                    taskInstance.InstanceData = await _checklistTaskInstanceRepository.GetByTaskInstanceId(taskInstance.Id);
                    break;
                case TaskTemplateLogic.UPLOAD_DOCUMENT_TASK_TYPE_ID:
                    taskInstance.InstanceData = null;
                    break;
                case TaskTemplateLogic.READ_DOCUMENT_TASK_TYPE_ID:
                    taskInstance.InstanceData = null;
                    break;
            }
            return new HTTPResponse<TaskInstance, string>() { Success = true, HttpCode = 200, Data = taskInstance }; 
        }

        public async Task<HTTPResponse<string, string>> UpdateChecklistInstanceState(UpdateInstanceStateChecklistPayload payload, string accountId)
        {
            // check task is a checklist task
            var response = await GetTaskInstance(payload.TaskInstanceId);
            if (!response.Success || response.Data == null)
            {
                return new HTTPResponse<string, string>() { Success = false, HttpCode = 400, Error = "Task instance doesn't exist" };
            }
            var taskInstance = response.Data;
            if (taskInstance.template.TaskTypeId != TaskTemplateLogic.CHECKLIST_TASK_TYPE_ID)
            {
                return new HTTPResponse<string, string>() { Success = false, HttpCode = 400, Error = "Task instance isn't a checklist task" };
            }
            // check user is assigned to task instance
            if (taskInstance.AssigneeAccountId != accountId)
            {
                return new HTTPResponse<string, string>() { Success = false, HttpCode = 400, Error = "User not authorised to update this resource" };
            }

            // check that checklist status length is correct
            var taskItems = (taskInstance.template.TaskTypeData as ChecklistTaskTemplateTable).Items;
            if (payload.ItemCompletionStatuses.Length != taskItems.Length)
            {
                return new HTTPResponse<string, string>() { Success = false, HttpCode = 400, Error = "Invalid checklist state" };
            }

            // update state
            var checklistInstance = await _checklistTaskInstanceRepository.GetByTaskInstanceId(taskInstance.Id);
            checklistInstance.ItemCompletionStatuses = payload.ItemCompletionStatuses;
            await _checklistTaskInstanceRepository.UpdateAsync(checklistInstance);

            return new HTTPResponse<string, string>() { Success = true, HttpCode = 200, Message = "Successfully updated state" };
        }

        public async Task<HTTPResponse<string, string>> CompleteTaskInstance(string taskInstanceId, string accountId)
        {
            // check task instance exists and isn't already complete
            var response = await GetTaskInstance(taskInstanceId);
            if (!response.Success || response.Data == null)
            {
                return new HTTPResponse<string, string>() { Success = false, HttpCode = 400, Error = "Task instance doesn't exist" };
            }
            var taskInstance = response.Data;
            if (taskInstance.Status == COMPLETED_TASK_STATUS)
            {
                return new HTTPResponse<string, string>() { Success = false, HttpCode = 400, Error = "Task is already completed" };
            }            
            // check account is assigned to task instance
            if (taskInstance.AssigneeAccountId != accountId)
            {
                return new HTTPResponse<string, string>() { Success = false, HttpCode = 400, Error = "User doesn't have access to update this resource" }; 
            }
            bool isComplete = false;
            // make sure task meets conditions to be complete
            switch(taskInstance.template.TaskTypeId)
            {
                case TaskTemplateLogic.CHECKLIST_TASK_TYPE_ID:
                    isComplete = IsChecklistTaskComplete(taskInstance);
                    break;
                case TaskTemplateLogic.READ_DOCUMENT_TASK_TYPE_ID:
                    // TODO implement completeness check
                    break;
                case TaskTemplateLogic.UPLOAD_DOCUMENT_TASK_TYPE_ID:
                    // TODO implement completeness check
                    break;
                default:
                    throw new Exception("Task template isn't associated with a valid task type id");
            }            

            if (!isComplete)
            {
                return new HTTPResponse<string, string>() { Success = false, HttpCode = 400, Error = "Task can't be completed yet" };
            }
            
            // mark task instance as complete
            taskInstance.Status = COMPLETED_TASK_STATUS;
            await _taskInstanceRepository.UpdateAsync(taskInstance);

            // TODO implement logic to notify correct users
            // TODO implement logic to call function in workflow to assign next task

            return new HTTPResponse<string, string>() { Success = true, HttpCode = 200, Data = "Successfully completed task" };
        }

        private bool IsChecklistTaskComplete(TaskInstance taskInstance)
        {
            if (taskInstance.template.TaskTypeId != TaskTemplateLogic.CHECKLIST_TASK_TYPE_ID)
            {
                throw new Exception("Task is not a checklist task");
            }

            // ensure all checklist items are complete
            var checklistInstanceData = (taskInstance.InstanceData as ChecklistTaskInstanceTable);
            foreach(var itemStatus in checklistInstanceData.ItemCompletionStatuses)
            {
                if (itemStatus == false)
                {
                    return false;
                }
            }
            return true;
        }
    }

}
