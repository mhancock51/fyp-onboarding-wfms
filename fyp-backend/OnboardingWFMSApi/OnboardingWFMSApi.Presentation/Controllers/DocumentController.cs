using Microsoft.AspNetCore.Mvc;
using OnboardingWFMSApi.BusinessLogic;
using OnboardingWFMSApi.DataModels;
using OnboardingWFMSApi.DataModels.Payloads;
using System.Security.Claims;

namespace OnboardingWFMSApi.Presentation.Controllers
{
    [ApiController]
    [Route("api/document")]
    public class DocumentController : ControllerBase
    {
        private readonly IDocumentLogic _documentLogic;

        public DocumentController(IDocumentLogic documentLogic)
        {
            _documentLogic = documentLogic;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadDocument([FromBody] UploadDocumentPayload payload)
        {
            string accountId = UserIdentityUtils.GetAccountIdFromClaimIdentity(User.Identity as ClaimsIdentity);
            if (accountId == "")
            {
                var response = new HTTPResponse<string, string>() { Success = false, HttpCode = 401, Message = "Invalid credentials" };
                return StatusCode(response.HttpCode, response);
            }
            else
            {
                var response = await _documentLogic.UploadDocument(payload, accountId);
                return StatusCode(response.HttpCode, response);
            }
        }
    }
}
