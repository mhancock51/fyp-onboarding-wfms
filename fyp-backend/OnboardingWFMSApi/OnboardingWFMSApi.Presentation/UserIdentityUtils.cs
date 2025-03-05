using OnboardingWFMSApi.DataModels;
using System.Security.Claims;

namespace OnboardingWFMSApi.Presentation
{
    public static class UserIdentityUtils
    {
        public static string GetAccountIdFromClaimIdentity(ClaimsIdentity identity)
        {
            var userIdClaim = identity.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return "";
            }
            else
            {
                return userIdClaim.Value;
            }
        }
    }
}
