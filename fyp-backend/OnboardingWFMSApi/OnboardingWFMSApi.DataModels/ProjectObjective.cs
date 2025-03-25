using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace OnboardingWFMSApi.DataModels
{
    public class ProjectObjective : ITableEntity
    {
        [Key]
        public string Id { get; set; }
        [JsonPropertyName("objective")]
        public string Objective { get; set; }
        [JsonPropertyName("required")]
        public bool Required { get; set; }
    }
}
