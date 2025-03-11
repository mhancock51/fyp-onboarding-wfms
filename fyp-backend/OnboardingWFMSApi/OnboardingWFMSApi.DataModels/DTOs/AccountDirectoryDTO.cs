using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnboardingWFMSApi.DataModels.DTOs
{
    public class AccountDirectoryDTO
    {
        public string DisplayName { get; set; }
        public string AccountId { get; set; }
        public string DepartmentId { get; set; }
        public string DepartmentName { get; set; }
    }
}
