using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnboardingWFMSApi.BusinessLogic.WorkflowInstanceLogic
{
    public static class WorkflowInstanceConstants
    {
        public const string WORKFLOW_INSTANCE_PREFLOW_STATUS = "PREFLOW";
        public const string WORKFLOW_INSTANCE_MAINFLOW_STATUS = "MAINFLOW";
        public const string WORKFLOW_INSTANCE_COMPLETE_STATUS = "COMPLETE";

        public const string WORKFLOW_NODE_INSTANCE_UNASSIGNED_STATUS = "unassigned";
        public const string WORKFLOW_NODE_INSTANCE_OPEN_STATUS = "open";
        public const string WORKFLOW_NODE_INSTANCE_COMPLETE_STATUS = "complete";
    }
}
