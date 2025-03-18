using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnboardingWFMSApi.BusinessLogic;
using OnboardingWFMSApi.DataModels;
using OnboardingWFMSApi.DataModels.Payloads;
using System.Security.Claims;

namespace OnboardingWFMSApi.Presentation.Controllers
{
    [ApiController]
    [Route("api/comment")]
    public class CommentController : ControllerBase
    {
        private readonly ICommentLogic _commentLogic;

        public CommentController(ICommentLogic commentLogic)
        {
            _commentLogic = commentLogic;
        }

        [Authorize]
        [HttpPost("/tasktemplate")]
        public async Task<IActionResult> CreateTaskTemplateComment([FromBody] CreateCommentPayload payload)
        {
            string accountId = UserIdentityUtils.GetAccountIdFromClaimIdentity(User.Identity as ClaimsIdentity);
            if (accountId == "")
            {
                var response = new HTTPResponse<string, string>() { Success = false, HttpCode = 401, Message = "Invalid credentials" };
                return StatusCode(response.HttpCode, response);
            }
            else
            {
                var response = await _commentLogic.CreateComment(payload, accountId);                
                return StatusCode(response.HttpCode, response);
            }
        }

        [Authorize]
        [HttpGet("/tasktemplate")]
        public async Task<IActionResult> GetTaskTemplateComments(string taskTemplateId)
        {
            var response = await _commentLogic.GetTaskTemplateComments(taskTemplateId);
            return StatusCode(response.HttpCode, response);
        }
    }
}
