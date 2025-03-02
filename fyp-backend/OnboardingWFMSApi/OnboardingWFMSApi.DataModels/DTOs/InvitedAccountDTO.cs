using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnboardingWFMSApi.DataModels.DTOs
{
    public class InvitedAccountDTO
    {
        public string DisplayName { get; set; }
        public string EmailAddress { get; set; }
        public bool IsOnboarder { get; set; }
        public string DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public string OrganisationId { get; set; }
        public string OrganisationName { get; set; } 
    }
}
