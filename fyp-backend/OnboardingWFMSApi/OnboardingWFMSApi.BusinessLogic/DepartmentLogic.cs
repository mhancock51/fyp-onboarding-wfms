using OnboardingWFMSApi.DataAccess.Repositories;
using OnboardingWFMSApi.DataModels;
using OnboardingWFMSApi.DataModels.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnboardingWFMSApi.BusinessLogic
{
    public interface IDepartmentLogic
    {
        public Task<HTTPResponse<string, string>> CreateDepartment(string name);
        public Task<HTTPResponse<List<DepartmentTable>, string>> GetAllDepartments();
    }

    public class DepartmentLogic : IDepartmentLogic
    {
        readonly private IDepartmentRepository _departmentRepository;

        public DepartmentLogic(IDepartmentRepository departmentRepository)
        {
            _departmentRepository = departmentRepository;
        }

        public async Task<HTTPResponse<string, string>> CreateDepartment(string name)
        {
            await _departmentRepository.AddAsync(new DepartmentTable() { DisplayName = name });
            return new HTTPResponse<string, string> { Success = true, HttpCode = 200, Data = "Created department" };
        }

        public async Task<HTTPResponse<List<DepartmentTable>, string>> GetAllDepartments()
        {
            var departments = await _departmentRepository.GetAll();
            return new HTTPResponse<List<DepartmentTable>, string>() { Success = true, HttpCode = 200, Data = departments };
        }
    }
}
