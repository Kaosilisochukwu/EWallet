using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EWallet.Api.DTOs
{
    public class TestResponseModel
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public TestUser Data { get; set; }
    }

    public class TestUser
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Role { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
    }
}
