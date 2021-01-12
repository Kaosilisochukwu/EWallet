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

        public int StatusCode { get; }
        public string Message { get; }
        public object Data { get; }
    }
}
