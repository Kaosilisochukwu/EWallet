using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace EWallet.Api.Test.Controllers
{
    public class WalletController : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly HttpClient _client;
        public WalletController(WebApplicationFactory<Startup> factory)
        {
            _client = factory.CreateDefaultClient(new Uri("http://localhost/api/Wallet"));
        }

        [Fact]
        public async Task UserRegistration_Failed()
        {
            var response = await _client.GetAsync("");
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }
    }
}
