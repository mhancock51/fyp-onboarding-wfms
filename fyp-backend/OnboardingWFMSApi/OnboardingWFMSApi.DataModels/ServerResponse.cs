using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnboardingWFMSApi.DataModels
{
    public class ServerResponse<T, TError>
    {

        public bool Success { get; set; }

        public T Data { get; set; }

        public TError Error { get; set; }

        public bool HasData => Data != null;

        public bool HasError => Error != null;

        public string Message { get; set; }
    }
}
