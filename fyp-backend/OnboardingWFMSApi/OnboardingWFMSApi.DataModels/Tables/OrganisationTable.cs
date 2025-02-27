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
    public class OrganisationTable
    {
        [Key]
        public string OrganisationId { get; set; }
        public string Name { get; set; }        
    }
}
