using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileServer
{
    public class InvalidResponse : IResponse
    {
        public string ErrorMessage { get; set; }

        public InvalidResponse(string errorMessage)
        {
            ErrorMessage = errorMessage;
        }
    }
}
