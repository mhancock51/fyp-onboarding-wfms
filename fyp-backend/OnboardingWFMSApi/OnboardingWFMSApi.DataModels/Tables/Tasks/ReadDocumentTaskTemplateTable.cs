using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnboardingWFMSApi.DataModels.Tables.Tasks
{
    [Table("readdocumenttasktemplate")]
    public class ReadDocumentTaskTemplateTable : ITableEntity
    {
        [Key]
        public string Id { get; set; }
        [ForeignKey(nameof(TaskTemplateTable.Id))]
        public string TaskTemplateId { get; set; }
        public string DocumentName { get; set; }
        public string DocumentUrl { get; set; }
        public string CheckBoxLabel { get; set; }
    }
}
