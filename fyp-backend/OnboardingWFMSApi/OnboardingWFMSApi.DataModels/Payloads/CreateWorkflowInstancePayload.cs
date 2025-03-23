using OnboardingWFMSApi.DataModels.Tables;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace OnboardingWFMSApi.DataModels.Payloads
{
    public class CreateWorkflowInstancePayload
    {
        [JsonPropertyName("workflowTeamplateId ")]
        public string WorkflowTeamplateId { get; set; }
        [JsonPropertyName("supervisorAccountId")]        
        public string SupervisorAccountId { get; set; }
        [JsonPropertyName("onboardingEmployeeDetails")]
        public CreateOnboardingEmployeeDetailsPayload? OnboardingEmployeeDetails { get; set; }
    }

    public class CreateOnboardingEmployeeDetailsPayload()
    {
        [JsonPropertyName("displayName")]
        public string DisplayName { get; set; }
        [JsonPropertyName("emailAddress")]
        public string EmailAddress { get; set; }
        [JsonPropertyName("departmentId")]
        public string DepartmentId { get; set; }
    }
}
