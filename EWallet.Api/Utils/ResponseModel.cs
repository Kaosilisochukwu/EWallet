using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EWallet.Api.Utils
{
    public class ResponseModel
    {
        public ResponseModel(int statusCode, string message, object data)
        {
            StatusCode = statusCode;
            Message = message;
            Data = data;
        }

        public int StatusCode { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }
    }
}
