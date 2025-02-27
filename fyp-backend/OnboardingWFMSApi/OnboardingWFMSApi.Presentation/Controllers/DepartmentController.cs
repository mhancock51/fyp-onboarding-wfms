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

        [HttpPost("create")]
        public async Task<IActionResult> CreateDepartment(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return BadRequest();
            }

            var result = await _departmentLogic.CreateDepartment(name);
            return StatusCode(result.HttpCode, result);
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllDepartments()
        {
            var result = await _departmentLogic.GetAllDepartments();
            return StatusCode(result.HttpCode, result);
        }
    }
}
