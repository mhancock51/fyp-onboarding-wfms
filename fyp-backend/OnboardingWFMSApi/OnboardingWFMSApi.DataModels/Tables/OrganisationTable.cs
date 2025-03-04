using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnboardingWFMSApi.DataModels.Tables
{
    [Table("organisation")]
    public class OrganisationTable : ITableEntity
    {
     
        [Key]
        [Column("OrganisationId")]
        public string Id { get; set; }
        public string Name { get; set; }        
    }
}
