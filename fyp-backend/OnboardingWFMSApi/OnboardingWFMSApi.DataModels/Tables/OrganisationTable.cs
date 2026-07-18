using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnboardingWFMSApi.DataModels.Tables.Interfaces;

namespace OnboardingWFMSApi.DataModels.Tables
{
    [Table("organisation")]
    public class OrganisationTable : ITenantTableEntity
    {

        [Key]
        [Column("OrganisationId")]
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;

        [Column(TypeName = "mediumblob")]
        public byte[]? LogoImageData { get; set; }

        public string? LogoImageMimeType { get; set; }

        public string TenantId { get; set; } = string.Empty;
    }
}
