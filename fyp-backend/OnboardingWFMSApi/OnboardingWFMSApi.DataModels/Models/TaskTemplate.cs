using OnboardingWFMSApi.DataModels.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace OnboardingWFMSApi.DataModels.Models
{
    public class TaskTemplate : TaskTemplateTable
    {
        [JsonPropertyName("taskTypeData")]
        public object TaskTypeData { get; set; }
        [JsonPropertyName("taskType")]
        public TaskType TaskType { get; set; }
        [JsonPropertyName("activeInstances")]
        public int ActiveInstances { get; set; }
    }
}
