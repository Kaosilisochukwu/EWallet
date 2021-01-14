using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace EWallet.Api.Test
{
    public class ShadowCopyTest : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly HttpClient _client;
        public ShadowCopyTest(WebApplicationFactory<Startup> factory)
        {
            _client = factory.CreateDefaultClient();
        }

        [Fact]
        public async Task HealthCare_ReturnsOk()
        {
            
            var response = await _client.GetAsync("/healthcare");
            response.EnsureSuccessStatusCode();
        }
    }
}
