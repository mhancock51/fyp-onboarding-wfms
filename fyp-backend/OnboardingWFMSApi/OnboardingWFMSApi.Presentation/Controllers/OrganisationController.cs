using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnboardingWFMSApi.BusinessLogic;

namespace OnboardingWFMSApi.Presentation
{
    [ApiController]
    [Route("api/organisation")]
    public class OrganisationController : ControllerBase
    {

        private readonly ILogger<OrganisationController> _logger;
        private readonly IOrganisationLogic _organisationLogic;

        public OrganisationController(ILogger<OrganisationController> logger, IOrganisationLogic organisationLogic)
        {
            _logger = logger;
            _organisationLogic = organisationLogic;
        }


        [HttpPost("assign")]
        public async Task<IActionResult> AssignAdmin(string organisationId, string adminAccountId)
        {
            var result = await _organisationLogic.AssignAdminToOrganisation(organisationId, adminAccountId);
            return StatusCode(result.HttpCode, result);
        }

        [HttpGet]
        public async Task<IActionResult> GetOrganisation()
        {
            var result = await _organisationLogic.GetOrganisation();
            return StatusCode(result.HttpCode, result);
        }

        [Authorize]
        [HttpPost("rename")]
        public async Task<IActionResult> RenameOrganisation(string newName)
        {
            // TODO: make this supervisor only
            var result = await _organisationLogic.RenameOrganisation(newName);
            return StatusCode(result.HttpCode, result);
        }
    }
}
