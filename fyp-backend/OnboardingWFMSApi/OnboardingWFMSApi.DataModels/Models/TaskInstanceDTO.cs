using OnboardingWFMSApi.DataModels.DTOs;
using OnboardingWFMSApi.DataModels.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace OnboardingWFMSApi.DataModels.Models
{
    public class TaskInstanceDTO : TaskInstanceTable
    {
        [JsonPropertyName("template")]
        public TaskTemplate Template { get; set; }
        /// <summary>
        /// Name of the workflow template that the task is associated to an instance of
        /// </summary>
        [JsonPropertyName("workflowInstanceTemplateName")]
        public string? WorkflowInstanceTemplateName { get; set; }
        [JsonPropertyName("instanceData")]
        public object InstanceData { get; set; }
        [JsonPropertyName("workflowInstance")]
        [JsonInclude]
        public WorkflowInstanceDTO? WorkflowInstance { get; set; }
    }
}
