using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace OnboardingWFMSApi.DataModels.DTOs
{
    public class AuthenticatedAccountDTO
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("displayName")]
        public string DisplayName { get; set; }

        [JsonPropertyName("emailAddress")]
        public string EmailAddress { get; set; }

        [JsonPropertyName("isSupervisor")]
        public bool IsSupervisor { get; set; }

        [JsonPropertyName("departmentId")]
        public string DepartmentId { get; set; }

        [JsonPropertyName("departmentName")]
        public string DepartmentName { get; set; }

        [JsonPropertyName("organisationId")]
        public string OrganisationId { get; set; }

        [JsonPropertyName("accountStatus")]
        public string AccountStatus { get; set; }

        [JsonPropertyName("jwtToken")]
        public string JwtToken { get; set; }
    }

}
