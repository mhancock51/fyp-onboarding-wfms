using OnboardingWFMSApi.DataModels.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnboardingWFMSApi.DataModels.Models
{
    public class TaskTemplate : TaskTemplateTable
    {
        public object TaskTypeData { get; set; }
    }
}
