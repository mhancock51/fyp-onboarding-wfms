using Microsoft.AspNetCore.Mvc;
using OnboardingWFMSApi.BusinessLogic;

namespace OnboardingWFMSApi.Presentation.Controllers
{
    [ApiController]
    [Route("api/department")]
    public class DepartmentController : ControllerBase
    {
        private readonly IDepartmentLogic _departmentLogic;

        public DepartmentController(IDepartmentLogic departmentLogic)
        {
            _departmentLogic = departmentLogic;
        }

        [HttpGet("create")]
        public async Task<IActionResult> CreateDepartment(string name)
        {
            var result = await _departmentLogic.CreateDepartment(name);
            return StatusCode(result.HttpCode, result);
        }
    }
}
