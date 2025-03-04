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
    }
    public class TaskInstanceLogic : ITaskInstanceLogic
    {
        private string OPEN_TASK_STATUS = "open";

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
                    await _checklistTaskInstanceRepository.AddAsync(new ChecklistTaskInstanceTable() { ItemCompletionStatuses = new bool[checklistItems.Length], TaskInstanceId = taskInstance.TaskInstanceId });
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
                var response = await GetTaskInstance(taskInstances[i].TaskInstanceId);
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
                    taskInstance.InstanceData = await _checklistTaskInstanceRepository.GetByTaskInstanceId(taskInstance.TaskInstanceId);
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
    }

}
