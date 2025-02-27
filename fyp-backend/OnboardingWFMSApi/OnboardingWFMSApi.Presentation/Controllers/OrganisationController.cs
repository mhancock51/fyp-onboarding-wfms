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

        [HttpPost("create")]
        public async Task<IActionResult> CreateOrganisation(string name)
        {
            var result = await _organisationLogic.CreateOrganisation(name);
            return StatusCode(result.HttpCode, result);
        }

        [HttpPost("assign")]
        public async Task<IActionResult> AssignAdmin(string organisationId, string adminAccountId)
        {
            var result = await _organisationLogic.AssignAdminToOrganisation(organisationId, adminAccountId);
            return StatusCode(result.HttpCode, result);
        }
    }
}
