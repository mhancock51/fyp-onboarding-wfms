using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace OnboardingWFMSApi.DataModels.Payloads
{
    public class CreateCommentPayload
    {
        [JsonPropertyName("taskTemplateId")]
        public string TaskTemplateId { get; set; }
        [JsonPropertyName("text")]
        public string Text { get; set; }
    }
}
