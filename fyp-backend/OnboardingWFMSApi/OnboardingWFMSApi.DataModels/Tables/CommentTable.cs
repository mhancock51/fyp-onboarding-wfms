using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnboardingWFMSApi.DataModels.Tables
{
    [Table("comment")]
    public class CommentTable : ITableEntity
    {
        [Key]   
        public string Id {  get; set; }
        [ForeignKey(nameof(AccountTable.Id))]
        public string CommenterId { get; set; }
        public string Text { get; set; }
        [ForeignKey(nameof(TaskTemplateTable.Id))]
        public string? TaskTemplateId { get; set; }
        public DateTime CreationTimestamp { get; set; } 
    }
}
