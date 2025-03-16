using OnboardingWFMSApi.DataModels.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnboardingWFMSApi.DataModels.Models
{
    public class TaskInstance : TaskInstanceTable
    {
        public TaskTemplate template { get; set; }
        /// <summary>
        /// Name of the workflow template that the task is associated to an instance of
        /// </summary>
        public string? WorkflowInstanceTemplateName { get; set; }
        public object InstanceData { get; set; }
    }
}
