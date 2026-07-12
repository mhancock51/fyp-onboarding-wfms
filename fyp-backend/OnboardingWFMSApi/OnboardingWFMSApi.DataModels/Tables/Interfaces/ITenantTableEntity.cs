using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace OnboardingWFMSApi.DataModels.Tables.Interfaces
{
    public interface ITenantTableEntity : ITableEntity
    {
        [ForeignKey(nameof(TenantTable.Id))]
        public string TenantId { get; set; }
    }
}