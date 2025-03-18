using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnboardingWFMSApi.DataModels.Payloads
{
    public class CreateCommentPayload
    {
        public string TaskTemplateId { get; set; }
        public string Text { get; set; }
    }
}
