using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace OnboardingWFMSApi.DataModels.DTOs
{
    public class OnboardingAnalyticsDTO
    {
        public OnboardingAnalyticsDTO(double averageTimeToOnboard, int employeesOnboarding, int employeesOnboarded)
        {
            AverageTimeToOnboard = averageTimeToOnboard;
            EmployeesOnboarding = employeesOnboarding;
            EmployeesOnboarded = employeesOnboarded;
        }

        [JsonPropertyName("averageTimeToOnboard")]
        public double AverageTimeToOnboard { get; set; }
        [JsonPropertyName("employeesOnboarding")]
        public int EmployeesOnboarding { get; set; }
        [JsonPropertyName("employeesOnboarded")]
        public int EmployeesOnboarded { get; set; }
    }
}
