using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnboardingWFMSApi.DataModels.Payloads
{
    public class CreateTenantPayload
    {
        public string OwnerEmailAddress {get; set;}
        public string SubscriptionTeirId { get; set; }
    }
}