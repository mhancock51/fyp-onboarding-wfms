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

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetOrganisation()
        {
            var result = await _organisationLogic.GetOrganisation();
            return StatusCode(result.HttpCode, result);
        }

        [Authorize(Policy = "SupervisorRoleClaim")]
        [HttpPost("rename")]
        public async Task<IActionResult> RenameOrganisation(string newName)
        {
            var result = await _organisationLogic.RenameOrganisation(newName);
            return StatusCode(result.HttpCode, result);
        }

        [Authorize(Policy = "SupervisorRoleClaim")]
        [HttpPost("logo")]
        public async Task<IActionResult> UpdateOrganisationLogo([FromForm] IFormFile logoFile)
        {
            var result = await _organisationLogic.UpdateOrganisationLogo(logoFile);
            return StatusCode(result.HttpCode, result);
        }
    }
}
