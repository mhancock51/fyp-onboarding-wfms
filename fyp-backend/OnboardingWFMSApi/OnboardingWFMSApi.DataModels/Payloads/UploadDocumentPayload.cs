using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnboardingWFMSApi.DataModels.Payloads
{
    public class UploadDocumentPayload
    {
        public required string? TaskInstanceId {  get; set; }        
        public required IFormFile File { get; set; }
        public string DocumentName { get; set; }
        public string[] AccessAccountIds { get; set; }
    }
}
