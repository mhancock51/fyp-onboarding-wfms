namespace OnboardingWFMSApi.DataModels.DTOs
{
    public class GetStartedResponseDTO
    {
        public string CheckoutUrl { get; set; } = string.Empty;
        public string JwtToken { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string TenantId { get; set; } = string.Empty;
        public string TierName { get; set; } = string.Empty;
    }
}
