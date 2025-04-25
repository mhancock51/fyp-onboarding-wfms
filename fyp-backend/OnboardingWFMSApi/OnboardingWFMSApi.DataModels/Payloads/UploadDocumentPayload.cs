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
        [JsonPropertyName("taskInstanceId")]
        public string TaskInstanceId {  get; set; }
        [JsonPropertyName("fileBase64")]
        public string FileBase64 { get; set; }
        [JsonPropertyName("fileName")]
        public string FileName { get; set; }
        [JsonPropertyName("documentName")]
        public string DocumentName { get; set; }
        [JsonPropertyName("accessAccountIds")]
        public string[] AccessAccountIds { get; set; }
    }
}
