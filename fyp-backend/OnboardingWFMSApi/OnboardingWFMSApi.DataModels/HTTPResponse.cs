using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnboardingWFMSApi.DataModels
{
    public class HTTPResponse<T, TError> : ServerResponse<T, TError>
    {
        public int HttpCode { get; set; }
    }
}
