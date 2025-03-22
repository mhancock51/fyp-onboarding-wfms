using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnboardingWFMSApi.BusinessLogic;
using OnboardingWFMSApi.DataModels;
using OnboardingWFMSApi.DataModels.Payloads;
using System.Reflection.Metadata;
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
        
        [HttpGet]
        public async Task<IActionResult> GetDocument(string documentId)
        {
            string accountId = UserIdentityUtils.GetAccountIdFromClaimIdentity(User.Identity as ClaimsIdentity);
            if (accountId == "")
            {
                var response = new HTTPResponse<string, string>() { Success = false, HttpCode = 401, Message = "Invalid credentials" };
                return StatusCode(response.HttpCode, response);
            }
            else
            {
                var response = await _documentLogic.GetDocument(documentId, accountId);
                if (response.Success)
                {
                    string fileName = response.Data.FileName + response.Data.FileExtension;
                    return File(response.Data.DocumentData, $"application/{response.Data.FileExtension.Replace(".", "")}", fileName);
                }
                else
                {
                    return StatusCode(response.HttpCode, response);
                }
            }

        }

        [HttpGet("data")]
        public async Task<IActionResult> GetDocumentData(string documentId)
        {
            string accountId = UserIdentityUtils.GetAccountIdFromClaimIdentity(User.Identity as ClaimsIdentity);
            if (accountId == "")
            {
                var response = new HTTPResponse<string, string>() { Success = false, HttpCode = 401, Message = "Invalid credentials" };
                return StatusCode(response.HttpCode, response);
            }
            else
            {
                var response = await _documentLogic.GetDocument(documentId, accountId);
                return StatusCode(response.HttpCode, response);
            }
        }

        [Authorize]
        [HttpGet("workflow-instance/data")]
        public async Task<IActionResult> GetWorkflowsDocuments(string workflowInstanceId)
        {
            string accountId = UserIdentityUtils.GetAccountIdFromClaimIdentity(User.Identity as ClaimsIdentity);
            if (accountId == "")
            {
                var response = new HTTPResponse<string, string>() { Success = false, HttpCode = 401, Message = "Invalid credentials" };
                return StatusCode(response.HttpCode, response);
            }
            else {
                var response = await _documentLogic.GetDocumentsFromWorkflowInstance(workflowInstanceId, accountId);
                return StatusCode(response.HttpCode, response);
            }
        }
    }
}
