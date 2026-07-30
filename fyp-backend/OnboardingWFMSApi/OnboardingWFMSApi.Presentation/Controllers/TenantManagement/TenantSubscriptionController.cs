using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OnboardingWFMSApi.BusinessLogic.TenantLogic;
using OnboardingWFMSApi.DataAccess;

namespace OnboardingWFMSApi.Presentation.Controllers.TenantManagement
{
    [ApiController]
    [Authorize]
    [Route("api/tenantsubscription")]
    public class TenantSubscriptionController : ControllerBase
    {
        private readonly ILogger<TenantSubscriptionController> _logger;
        private readonly ITenantLogic _tenantLogic;
        private readonly ICurrentTenantService _currentTenant;

        public TenantSubscriptionController(ILogger<TenantSubscriptionController> logger, ITenantLogic tenantLogic, ICurrentTenantService currentTenant)
        {
            _logger = logger;
            _tenantLogic = tenantLogic;
            _currentTenant = currentTenant;
        }

        [HttpGet]
        public async Task<IActionResult> GetTenantSubscription()
        {
            try
            {
                var response = await _tenantLogic.GetTenantSubscription(_currentTenant.TenantId);
                return StatusCode(response.HttpCode, response);
            }
            catch(Exception ex)
            {
                _logger.LogError($"Error occurred fetching tenant subscription: {ex}");
                return StatusCode(500, "Error occurred fetching tenant subscription.");
            }
        }

        [Authorize(Policy = "SupervisorRoleClaim")]
        [HttpPut("cancel")]
        public async Task<IActionResult> CancelTenantSubscription()
        {
            try
            {
                var response = await _tenantLogic.CancelTenantSubscription(_currentTenant.TenantId);
                return StatusCode(response.HttpCode, response);
            }
            catch(Exception ex)
            {
                _logger.LogError($"Error occurred cancelling tenant subscription: {ex}");
                return StatusCode(500, "Error occurred cancelling tenant subscription.");
            }
        }

    }
}