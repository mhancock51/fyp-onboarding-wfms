using OnboardingWFMSApi.DataModels.Payloads;
using OnboardingWFMSApi.DataModels.Tables.Workflows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace OnboardingWFMSApi.DataModels.DTOs
{
    public class WorkflowInstanceDTO : WorkflowInstanceTable
    {
        [JsonPropertyName("workflowTemplate")]
        public WorkflowTemplateDTO WorkflowTemplate { get; set; }
        [JsonPropertyName("completedTasks")]
        public int CompletedTasks { get; set; }
        /// <summary>
        /// Either PREFLOW, MAINFLOW or COMPLETED 
        /// </summary>
        [JsonPropertyName("status")]
        public string Status { get; set; }
        [JsonPropertyName("onboardingEmployeeDetails")]
        public OnboardingEmployeeDetailsDTO OnboardingEmployeeDetails { get; set; }
    }
}
