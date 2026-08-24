using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnboardingWFMSApi.DataModels.DTOs
{
    public class TaskAnalyticsDTO
    {
        /// <summary>
        /// Number of tasks completed past their due date
        /// </summary>
        public int TasksCompletedOverdue { get; set; }
        /// <summary>
        /// Number of incomplete tasks that are overdue
        /// </summary>
        public int IncompleteOverdueTasks { get; set; }
    }
}
