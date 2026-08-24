using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using OnboardingWFMSApi.BusinessLogic.TenantLogic;
using OnboardingWFMSApi.DataAccess;
using OnboardingWFMSApi.DataAccess.Repositories.Tenant_Repositories;
using OnboardingWFMSApi.DataAccess.Repositories.Workflow_Repositories;

namespace OnboardingWFMSApi.BusinessLogic.Handlers.CustomAuthHandlers
{
    public class WorkflowInstancesRequirement : IAuthorizationRequirement
    {
        
    }

    public class WorkflowInstancesHandler : AuthorizationHandler<WorkflowInstancesRequirement>
    {
        private readonly ICurrentTenantService _currentTenant;
        private readonly ITenantLogic _tenantLogic;
        private readonly IWorkflowInstanceRepository _workflowInstances;
        private readonly ILogger<WorkflowInstancesHandler> _logger;

        public WorkflowInstancesHandler(ICurrentTenantService currentTenant, IWorkflowInstanceRepository workflowInstances, ILogger<WorkflowInstancesHandler> logger, ITenantLogic tenantLogic)
        {
            _currentTenant = currentTenant;
            _workflowInstances = workflowInstances;
            _logger = logger;
            _tenantLogic = tenantLogic;
        }

        protected async override Task HandleRequirementAsync(AuthorizationHandlerContext context, WorkflowInstancesRequirement requirement)
        {
            
        }
    }
}