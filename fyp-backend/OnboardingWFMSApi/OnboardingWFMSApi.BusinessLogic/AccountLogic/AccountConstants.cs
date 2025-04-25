using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnboardingWFMSApi.BusinessLogic.AccountLogic
{
    public static class AccountConstants
    {
        public const string INVITED_STATUS = "invited";
        public const string REGISTERED_STATUS = "registered";
        public const string CLOSED_STATUS = "closed";

        public const string EMAIL_ADDRESS_ANONYMISED = "---";
        public const string DISPLAY_NAME_ANONYMISED = "[Deleted]";
    }
}
