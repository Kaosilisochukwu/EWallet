using AutoMapper;
using EWallet.Api.Data;
using EWallet.Api.DTOs;
using EWallet.Api.Model;
using EWallet.Api.Services.Interfaces;
using EWallet.Api.Services.Repositories;
using EWallet.Api.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EWallet.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FundController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly FundRequestRepository _fundRequestRepo;
        private readonly WalletRepository _walletRepo;
        private readonly IHttpContextAccessor _httpContext;
        private readonly UserManager<User> _userManager;

        public FundController(IMapper mapper, FundRequestRepository fundRequestRepo, WalletRepository walletRepo, IHttpContextAccessor httpContext, UserManager<User> userManager)
        {
            _mapper = mapper;
            _fundRequestRepo = fundRequestRepo;
            _walletRepo = walletRepo;
            _httpContext = httpContext;
            _userManager = userManager;
        }

        [HttpPost]
        [Route("request", Name = "request")]
        public async Task<IActionResult> RequestFunding(FundRequestDTO model)
        {
            if (ModelState.IsValid)
            {
                var userId = _httpContext.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
                var user = await _userManager.FindByIdAsync(userId);
                var wallet = await _walletRepo.GetWalletById(model.WalletId);
                if(wallet != null)
                {
                    var walletCurrency = wallet.Currency;
                    var fundRequest = _mapper.Map<Fund>(model);
                    if(user.Role == "Noob" && model.FundingCurrency != wallet.Currency)
                    {
                        var convertedAmount = await Currency.ConvertCurrency(model.FundingCurrency, wallet.Currency, model.Amount);
                        fundRequest.Amount = convertedAmount;
                        fundRequest.FundingCurrency = walletCurrency;
                        var requestResult = await _fundRequestRepo.RequestFunding(fundRequest);
                        if (requestResult > 0)
                            return Created("request", new ResponseModel(200, "Request successful", fundRequest));
                        return BadRequest(new ResponseModel(400, "Failed", fundRequest));
                    }
                 }
                return NotFound(new ResponseModel(404, "Wallet was not found", model));
            }
            var errors = ModelState.Values.Select(model => model.Errors).ToList();
            return BadRequest(new ResponseModel(400, "There are some validation Errors", errors));
        }
    }
}
