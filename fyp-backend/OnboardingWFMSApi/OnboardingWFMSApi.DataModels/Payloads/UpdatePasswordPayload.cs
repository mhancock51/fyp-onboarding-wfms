using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace OnboardingWFMSApi.DataModels.Payloads
{
    public class UpdatePasswordPayload
    {
        [JsonPropertyName("oldPassword")]
        public string OldPassword { get; set; }
        [JsonPropertyName("newPassword")]
        public string NewPassword { get; set; }
    }
}
