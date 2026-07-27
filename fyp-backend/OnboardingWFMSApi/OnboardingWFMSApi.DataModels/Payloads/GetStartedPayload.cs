using System.Text.Json.Serialization;

namespace OnboardingWFMSApi.DataModels.Payloads
{
    public class GetStartedPayload
    {
        [JsonPropertyName("displayName")]
        public string DisplayName { get; set; } = string.Empty;

        [JsonPropertyName("email")]
        public string Email { get; set; } = string.Empty;

        [JsonPropertyName("password")]
        public string Password { get; set; } = string.Empty;

        [JsonPropertyName("organisationName")]
        public string OrganisationName { get; set; } = string.Empty;

        [JsonPropertyName("subscriptionTierId")]
        public string SubscriptionTierId { get; set; } = string.Empty;
    }
}
