using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using OnboardingWFMSApi.DataModels.Models;
using OnboardingWFMSApi.DataModels.Tables.Interfaces;

namespace OnboardingWFMSApi.DataModels.Tables.Tasks
{
    [Table("projectasktemplate")]
    public class ProjectTaskTemplateTable : ITaskTypeTemplateTable
    {
        [Key]
        [JsonPropertyName("id")]
        public string Id { get; set; }
        [ForeignKey(nameof(TaskTemplateTable.Id))]
        [JsonPropertyName("taskTemplateId")]
        public string TaskTemplateId { get; set; }
        [JsonPropertyName("brief")]
        public string Brief { get; set; }
        [JsonPropertyName("deliverable")]
        public string Deliverable { get; set; }
        [JsonPropertyName("objectives")]
        [JsonInclude]
        public List<ProjectObjective> Objectives { get; set; }
        [JsonPropertyName("skills")]
        [JsonInclude]
        public List<string> Skills { get; set; }
        [JsonPropertyName("supportLinks")]
        [JsonInclude]
        public List<ProjectSupportLink> SupportLinks { get; set; }

        public string TenantId { get; set; }
    }    
}
