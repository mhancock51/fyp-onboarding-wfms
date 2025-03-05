using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnboardingWFMSApi.DataModels.Payloads
{
    public class UpdateInstanceStateReadDocPayload
    {
        public string TaskInstanceId { get; set; }
        public bool CheckboxChecked { get; set; }
        public bool LinkClicked { get; set; }
    }
}
