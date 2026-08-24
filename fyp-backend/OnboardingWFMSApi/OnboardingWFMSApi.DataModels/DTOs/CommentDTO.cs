using OnboardingWFMSApi.DataModels.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace OnboardingWFMSApi.DataModels.DTOs
{
    public class CommentDTO : CommentTable
    {
        [JsonPropertyName("accountDirectory")]
        public AccountDirectoryDTO AccountDirectory { get; set; }
    }
}
