using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace OnboardingWFMSApi.DataModels.Payloads
{
    public class UpdateAccountPayload
    {
        [JsonPropertyName("displayName")]
        public string DisplayName { get; set; }
    }
}
