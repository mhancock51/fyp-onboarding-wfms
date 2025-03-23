using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace OnboardingWFMSApi.DataModels.DTOs
{
    public class AccountDirectoryDTO
    {
        [JsonPropertyName("displayName")]
        public string DisplayName { get; set; }
        [JsonPropertyName("id")]
        public string Id { get; set; }
        [JsonPropertyName("departmentId")]
        public string DepartmentId { get; set; }
        [JsonPropertyName("departmentName")]
        public string DepartmentName { get; set; }
        [JsonPropertyName("isAdmin")]
        public bool IsAdmin { get; set; }   
    }
}
