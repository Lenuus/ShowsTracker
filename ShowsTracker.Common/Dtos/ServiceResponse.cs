using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowsTracker.Application
{
    public class ServiceResponse
    {

        public ServiceResponse(bool isSuccesfull, string errorMessage)
        {
            IsSuccesfull = isSuccesfull;
            ErrorMessage = errorMessage;
        }

        public string ErrorMessage { get; set; } = string.Empty;
        public bool IsSuccesfull { get; set; }
    }

    public class ServiceResponse<T>
    {
        public ServiceResponse(T data, bool isSuccesfull, string errorMessage)
        {
            Data = data;
            IsSuccesfull = isSuccesfull;
            ErrorMessage = errorMessage;

        }
        public T Data { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
        public bool IsSuccesfull { get; set; }

    }


}

