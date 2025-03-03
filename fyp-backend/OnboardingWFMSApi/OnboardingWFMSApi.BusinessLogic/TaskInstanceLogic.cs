using AutoMapper;
using OnboardingWFMSApi.DataAccess.Repositories;
using OnboardingWFMSApi.DataAccess.Repositories.Task_Repositories;
using OnboardingWFMSApi.DataModels;
using OnboardingWFMSApi.DataModels.Models;
using OnboardingWFMSApi.DataModels.Payloads;
using OnboardingWFMSApi.DataModels.Tables;
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
    }
    public class TaskInstanceLogic : ITaskInstanceLogic
    {
        private string OPEN_TASK_STATUS = "open";

        private readonly IMapper _mapper;
        private readonly ITaskTemplateLogic _taskTemplateLogic;

        private readonly ITaskInstanceRepository _taskInstanceRepository;
        private readonly IAccountRepository _accountRepository;

        public TaskInstanceLogic(ITaskInstanceRepository taskInstanceRepository, IMapper mapper, ITaskTemplateLogic taskTemplateLogic, IAccountRepository accountRepository)
        {
            _taskInstanceRepository = taskInstanceRepository;
            _mapper = mapper;
            _taskTemplateLogic = taskTemplateLogic;
            _accountRepository = accountRepository;
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
            await _taskInstanceRepository.AddAsync(instance);
            return new HTTPResponse<string, string>() { Success = true, HttpCode = 200, Data = "Successfully created task instance" };
        }

        public async Task<HTTPResponse<List<TaskInstance>, string>> GetUsersAssignedTask(string accountId)
        {
            var taskInstances = _mapper.Map<List<TaskInstance>>(await _taskInstanceRepository.GetUsersTaskInstances(accountId));            
            for (var i = 0; i < taskInstances.Count; i++)
            {
                var response = await _taskTemplateLogic.GetTaskTemplateById(taskInstances[i].TaskTemplateId);
                if (response.Success)
                {
                    taskInstances[i].template = response.Data;
                }
                else
                {
                    throw new Exception("Task instance isn't associated with a valid task template");
                }
            }
            return new HTTPResponse<List<TaskInstance>, string>() { Success = true, HttpCode = 200, Data = taskInstances };
        }
    }

}
