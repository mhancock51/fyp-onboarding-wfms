using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnboardingWFMSApi.BusinessLogic;
using OnboardingWFMSApi.DataModels;
using System.Security.Claims;

namespace OnboardingWFMSApi.Presentation.Controllers
{
    [ApiController]
    [Route("api/notifications")]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationLogic _notificationLogic;

        public NotificationController(INotificationLogic notificationLogic)
        {
            _notificationLogic = notificationLogic;
        }

        [Authorize]
        [HttpGet("all")]
        public async Task<IActionResult> GetAllNotifications()
        {
            string accountId = UserIdentityUtils.GetAccountIdFromClaimIdentity(User.Identity as ClaimsIdentity);
            if (accountId == "")
            {
                var response = new HTTPResponse<string, string>() { Success = false, HttpCode = 401, Message = "Invalid credentials" };
                return StatusCode(response.HttpCode, response);
            }
            else
            {
                var response = await _notificationLogic.GetAccountsNotification(accountId);
                return StatusCode(response.HttpCode, response);
            }
        }
    }
}
