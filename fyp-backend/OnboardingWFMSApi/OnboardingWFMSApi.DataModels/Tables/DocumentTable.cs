using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnboardingWFMSApi.DataModels.Tables
{
    [Table("document")]
    public class DocumentTable : ITableEntity
    {
        [Key]
        public string Id { get; set; }
        [ForeignKey(nameof(TaskInstanceTable.Id))]
        public string TaskInstanceId {  get; set; }
        [ForeignKey(nameof(AccountTable.Id))]
        public string CreatorId { get; set; }
        // TODO add foreign key constraint
        public string WorkflowInstanceId { get; set; }
        public byte[] DocumentData { get; set; }
        public string FileExtension { get; set; }
        public DateTime UploadTimestamp { get; set; }
        public string FileName { get; set; }
    }
}
