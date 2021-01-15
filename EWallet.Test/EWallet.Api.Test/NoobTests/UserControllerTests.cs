using EWallet.Api.DTOs;
using EWallet.Api.Model;
using EWallet.Api.Utils;
using Microsoft.AspNetCore.Mvc.Testing;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace EWallet.Api.Test.Controllers
{
    public class UserControllerTests : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly HttpClient _client;
        //private readonly string _adminToken;
        public UserControllerTests(WebApplicationFactory<Startup> factory)
        {
            var mockist = new Mock<Startup>();
            _client = factory.CreateDefaultClient(new Uri("http://localhost/api/user")); 
         
        }
      


        [Fact]
        public async Task TestNoobRegistration()
        {
            //Arrange
            var validNoobData = new UserToRegisterDTO
            {
                FirstName = "Kaosi",
                LastName = "Nwizu",
                Email = "onyi@decoagon.com",
                Role = "Noob",
                Username = "brulhlverjks3jjrwo",
                Password = "Password",
                ConfirmPassowrd = "Password"
            };
            var invalidNoobData = new UserToRegisterDTO
            {
                FirstName = "Kaosi",
                LastName = "Nwizu",
                Email = "onyi@decagon.com",
                Role = "Noob",
                Username = "bruhs",
                Password = "Password",
                ConfirmPassowrd = "Pa$$word" //password mismatch
            };


            //Act
            var validRegistration = await _client.SendAsync(new HttpRequestMessage
            {
                RequestUri = new Uri("http://localhost/api/user/register"),
                Method = HttpMethod.Post,
                Content = new StringContent(JsonSerializer.Serialize(validNoobData), Encoding.UTF8, "application/json")
            });

            var invalidRegistration = await _client.SendAsync(new HttpRequestMessage
            {
                RequestUri = new Uri("http://localhost/api/user/register"),
                Method = HttpMethod.Post,
                Content = new StringContent(JsonSerializer.Serialize(invalidNoobData), Encoding.UTF8, "application/json")
            });
            
            var json1 = await validRegistration.Content.ReadAsStringAsync();
            var responseObj1 = JsonSerializer.Deserialize<TestResponseModel>(json1, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            var userToReturn = responseObj1.Data;

            var adminLoginModel = new UserToLoginDTO
            {
                Email = "neto@c.com",
                Password = "01234Admin"
            };

            var loginResult = await _client.SendAsync(new HttpRequestMessage
            {
                RequestUri = new Uri("http://localhost/api/user/login"),
                Content = new StringContent(JsonSerializer.Serialize(adminLoginModel), Encoding.UTF8, "application/json"),
                Method = HttpMethod.Post
            });
            var json = await loginResult.Content.ReadAsStringAsync();
            var responseObj = JsonSerializer.Deserialize<ResponseModel>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + responseObj.Data.ToString());
            var deletCurrentUser = await _client.SendAsync(new HttpRequestMessage
            {
                RequestUri = new Uri("http://localhost/api/user/delete"),
                Method = HttpMethod.Delete,
                Content = new StringContent(JsonSerializer.Serialize(new UserToDeleteDTO { UserId = userToReturn.Id }), Encoding.UTF8, "application/json"),
            });

            var json2 = await invalidRegistration.Content.ReadAsStringAsync();
            var responseObj2 = JsonSerializer.Deserialize<ResponseModel>(json2, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            //Assert

            Assert.Equal(HttpStatusCode.Created, validRegistration.StatusCode);
            Assert.Equal(HttpStatusCode.BadRequest, invalidRegistration.StatusCode);
            Assert.Equal(HttpStatusCode.OK, deletCurrentUser.StatusCode);
            Assert.Equal(responseObj1.Data.UserName, validNoobData.Username);
        }

        //[Fact]
        //public async Task NoobFundingTests()
        //{
        //    var validNoobData = new UserToRegisterDTO
        //    {
        //        FirstName = "Kaosi",
        //        LastName = "Nwizu",
        //        Email = "onyi@decoagon.com",
        //        Role = "Noob",
        //        Username = "userName",
        //        Password = "Password",
        //        ConfirmPassowrd = "Password"
        //    };

        //    var validRegistration = await _client.SendAsync(new HttpRequestMessage
        //    {
        //        RequestUri = new Uri("http://localhost/api/user/register"),
        //        Method = HttpMethod.Post,
        //        Content = new StringContent(JsonSerializer.Serialize(validNoobData), Encoding.UTF8, "application/json")
        //    });

        //    var json1 = await validRegistration.Content.ReadAsStringAsync();
        //    var responseObj1 = JsonSerializer.Deserialize<TestResponseModel>(json1, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        //    var userToReturn = responseObj1.Data;

        //    var userLoginModel = new UserToLoginDTO
        //    {
        //        Email = "onyi@decoagon.com",
        //        Password = "Password"
        //    };

        //    var loginResult = await _client.SendAsync(new HttpRequestMessage
        //    {
        //        RequestUri = new Uri("http://localhost/api/user/login"),
        //        Content = new StringContent(JsonSerializer.Serialize(userLoginModel), Encoding.UTF8, "application/json"),
        //        Method = HttpMethod.Post
        //    });
        //    var json = await loginResult.Content.ReadAsStringAsync();
        //    var responseObj = JsonSerializer.Deserialize<ResponseModel>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        //    _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + responseObj.Data.ToString());

        //    var requestFund = new FundRequestDTO
        //    {

        //        WalletId = 1,
        //        FundingCurrency = "USD",
        //        Amount = 500,
        //        RequestDate = DateTime.Now
        //    };

        //    var newWallet = new WalletToAdd
        //    {
        //        Currency = "USD",
        //        Amount = 900
        //    };

        //    var withdrawFund = new WithdrawalDTO
        //    {
        //        Amount = 500,
        //        Currency = "USD"
        //    };

        //    var CreateWalletResult = await _client.SendAsync(new HttpRequestMessage
        //    {
        //        RequestUri = new Uri("http://localhost/api/wallet/add"),
        //        Content = new StringContent(JsonSerializer.Serialize(newWallet), Encoding.UTF8, "application/json"),
        //        Method = HttpMethod.Post
        //    });


        //    var walletCreationJson = await loginResult.Content.ReadAsStringAsync();
        //    var creationReturn = JsonSerializer.Deserialize<Wallet>(walletCreationJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        //    var FundWalletResult = await _client.SendAsync(new HttpRequestMessage
        //    {
        //        RequestUri = new Uri("http://localhost/api/wallet/fund"),
        //        Content = new StringContent(JsonSerializer.Serialize(requestFund), Encoding.UTF8, "application/json"),
        //        Method = HttpMethod.Post
        //    });

        //    var WithdrawFundsResult = await _client.SendAsync(new HttpRequestMessage
        //    {
        //        RequestUri = new Uri("http://localhost/api/wallet/withdraw"),
        //        Content = new StringContent(JsonSerializer.Serialize(withdrawFund), Encoding.UTF8, "application/json"),
        //        Method = HttpMethod.Post
        //    });

        //    Assert.Equal(HttpStatusCode.Created, CreateWalletResult.StatusCode); 
        //    Assert.Equal(HttpStatusCode.BadRequest, FundWalletResult.StatusCode);
        //    Assert.Equal(HttpStatusCode.Created, WithdrawFundsResult.StatusCode);

        //    _client.DefaultRequestHeaders.Remove("Authorization");

        //    var adminLoginModel = new UserToLoginDTO
        //    {
        //        Email = "neto@c.com",
        //        Password = "01234Admin"
        //    };

        //    var adminLoginResult = await _client.SendAsync(new HttpRequestMessage
        //    {
        //        RequestUri = new Uri("http://localhost/api/wallet/delete?walletId=" + creationReturn.Id),
        //        Content = new StringContent(JsonSerializer.Serialize(adminLoginModel), Encoding.UTF8, "application/json"),
        //        Method = HttpMethod.Post
        //    });
        //    var adminJson = await adminLoginResult.Content.ReadAsStringAsync();
        //    var adminResponseObj = JsonSerializer.Deserialize<ResponseModel>(adminJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        //    _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + adminResponseObj.Data.ToString());
        //    var deletCurrentUser = await _client.SendAsync(new HttpRequestMessage
        //    {
        //        RequestUri = new Uri("http://localhost/api/user/delete"),
        //        Method = HttpMethod.Post,
        //        Content = new StringContent(JsonSerializer.Serialize(new UserToDeleteDTO { UserId = userToReturn.Id }), Encoding.UTF8, "application/json"),
        //    });
        //}
    }
}
