using OnboardingWFMSApi.BusinessLogic.TaskTemplateHandlers;
using OnboardingWFMSApi.DataModels.Models;
using OnboardingWFMSApi.DataModels.Tables.Interfaces;
using OnboardingWFMSApi.DataModels.Tables.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace OnboardingWFMSApi.BusinessLogic
{
    public class BaseTaskHandler<TTaskType> where TTaskType : class, ITableEntity
    {
        public TTaskType CastObjectToType(object taskTypeData) 
        {
            var checklistTaskData = taskTypeData as TTaskType;
            if (checklistTaskData == null)
            {
                JsonElement jsonElement = (JsonElement)taskTypeData;
                checklistTaskData = jsonElement.Deserialize<TTaskType>();
            }
            return checklistTaskData;
        }
    }
}
