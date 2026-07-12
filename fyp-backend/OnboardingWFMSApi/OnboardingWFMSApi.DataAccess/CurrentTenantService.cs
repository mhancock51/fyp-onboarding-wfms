using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace OnboardingWFMSApi.DataAccess
{
    public interface ICurrentTenantService
    {
        string? TenantId { get; }
    }

    public class CurrentTenantService : ICurrentTenantService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentTenantService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string? TenantId =>
        _httpContextAccessor.HttpContext?.User?.FindFirst("TenantId")?.Value;
    }
}