namespace OnboardingWFMSApi.DataModels.DTOs
{
    /// <summary>
    /// Public pricing tier info returned by the landing-page pricing endpoint.
    /// Price data is fetched live from Stripe; feature lists are derived from the tier.
    /// </summary>
    public class PricingTierDTO
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        /// <summary>Price per unit in the smallest currency unit (e.g. cents).</summary>
        public long? UnitAmount { get; set; }

        /// <summary>Three-letter ISO currency code, e.g. "usd".</summary>
        public string Currency { get; set; } = string.Empty;

        /// <summary>Billing interval: "month" or "year".</summary>
        public string Interval { get; set; } = string.Empty;

        /// <summary>Human-readable price string, e.g. "$49/month".</summary>
        public string PriceDisplay { get; set; } = string.Empty;

        /// <summary>Is this the "most popular" / highlighted tier?</summary>
        public bool Highlighted { get; set; }

        /// <summary>Call-to-action button text.</summary>
        public string CtaText { get; set; } = string.Empty;

        /// <summary>Bullet-point feature list shown on the landing page.</summary>
        public List<string> Features { get; set; } = new();
    }
}
