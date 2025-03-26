using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace OnboardingWFMSApi.DataModels.Payloads
{
    public class UploadDocumentPayload
    {
        [FromForm(Name = "taskInstanceId")]
        public string? TaskInstanceId {  get; set; }
        [FromForm(Name = "file")]
        public IFormFile File { get; set; }
        [FromForm(Name = "documentName")]
        public string DocumentName { get; set; }
        [FromForm(Name = "accessAccountIds")]
        public List<string> AccessAccountIds { get; set; }
    }
}
