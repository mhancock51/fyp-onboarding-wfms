using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnboardingWFMSApi.DataModels.Payloads
{
    public class UpdateInstanceStateFileUploadPayload
    {
        public string TaskInstanceId { get; set; }
        public required IFormFile File { get; set; }
    }
}
