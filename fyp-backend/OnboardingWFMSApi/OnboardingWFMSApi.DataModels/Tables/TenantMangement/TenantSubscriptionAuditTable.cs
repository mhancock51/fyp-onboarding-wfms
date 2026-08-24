using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using OnboardingWFMSApi.DataModels.Tables.Interfaces;

namespace OnboardingWFMSApi.DataModels.Tables.TenantMangement
{
    [Table("TenantSubscriptionAuditLogs")]
    public class TenantSubscriptionAuditLogsTable : ITableEntity
    {
        [Key]
        public string Id {get; set;}

        [ForeignKey(nameof(TenantTable.Id))]
        public string TenantId {get; set;}
        public DateTime Timestamp {get; set;}
        public string EventDescription {get; set;}
    }
}