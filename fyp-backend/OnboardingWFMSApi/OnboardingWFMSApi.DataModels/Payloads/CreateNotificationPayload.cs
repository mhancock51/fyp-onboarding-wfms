using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace OnboardingWFMSApi.DataModels.Payloads
{
    public class CreateNotificationPayload
    {
        public CreateNotificationPayload(string recipientId, string description, string[] tags)
        {
            RecipientId = recipientId;
            Description = description;
            Tags = tags;
        }

        [JsonPropertyName("recipientId")]
        public string RecipientId { get; set; }
        [JsonPropertyName("description")]
        public string Description { get; set; }
        public string[] Tags { get; set; }
    }
}
