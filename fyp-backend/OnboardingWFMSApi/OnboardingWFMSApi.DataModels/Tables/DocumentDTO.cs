using OnboardingWFMSApi.DataModels.DTOs;
using OnboardingWFMSApi.DataModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace OnboardingWFMSApi.DataModels.Tables
{
    public class DocumentDTO : DocumentTable
    {
        [JsonPropertyName("creatorsAccount")]
        [JsonInclude]
        public AccountDirectoryDTO CreatorsAccount;
        [JsonPropertyName("taskInstance")]
        [JsonInclude]
        public TaskInstanceDTO TaskInstance;
    }
}
