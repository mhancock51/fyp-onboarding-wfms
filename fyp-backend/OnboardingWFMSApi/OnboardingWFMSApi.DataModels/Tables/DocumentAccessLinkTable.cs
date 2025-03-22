using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnboardingWFMSApi.DataModels.Tables
{
    [Table("documentaccesslink")]
    public class DocumentAccessLinkTable : ITableEntity
    {
        [Key]
        public string Id { get; set; }
        [ForeignKey(nameof(AccountTable.Id))]
        public string AccountId { get; set; }
        [ForeignKey(nameof(DocumentTable.Id))]
        public string DocumentId { get; set; }
    }
}
