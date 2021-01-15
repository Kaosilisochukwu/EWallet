using AutoMapper;
using EWallet.Api.DTOs;
using EWallet.Api.Model;
using EWallet.Api.Services.Repositories;
using EWallet.Api.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EWallet.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class WalletController : ControllerBase
    {
        private readonly WalletRepository _walletRepo;
        private readonly IHttpContextAccessor _httpContext;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;

        public WalletController(WalletRepository walletRepo, IHttpContextAccessor httpContext, IMapper mapper, UserManager<User> userManager)
        {
            _walletRepo = walletRepo;
            _httpContext = httpContext;
            _mapper = mapper;
            _userManager = userManager;
        }

        [HttpPost]
        [Route("add", Name = "add")]
        public async Task<IActionResult> AddWallet(WalletToAdd model)
        {
            try
            {
                model.Currency = model.Currency.ToUpper();
                if (ModelState.IsValid)
                {
                    var userId = _httpContext.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
                    var wallet = _mapper.Map<Wallet>(model);
                    var user = await _userManager.FindByIdAsync(userId);
                    var failureResponseMessage = user.Role == "Noob" ? "Noob Users can only have one account" 
                                            : user.Role == "Admin" ? "Admins are not allowed to have an account"
                                            : "Wallet not added";
                    wallet.UserId = userId;
                    var addResult = await _walletRepo.AddWAllet(wallet);
                    if (addResult > 0)
                    {
                        return Created("add", new ResponseModel(201, "Wallet Successfully added", wallet));
                    }
                    return BadRequest(new ResponseModel(400, failureResponseMessage, wallet));
                }
                var errors = ModelState.Values.Select(model => model.Errors).ToList();
                return BadRequest(new ResponseModel(400, "There are some validation Errors", errors));
            }
            catch (System.Exception)
            {
                return BadRequest(new ResponseModel(400, "There are some validation Errors", model));
            }
        }

        [HttpPost]
        [Route("changeMainCurrency")]
        public async Task<IActionResult> ChangeWalletMainCurrency(CurrencyToChange model)
        {
            try
            {
                model.NewCurrency = model.NewCurrency.ToUpper();
                if (ModelState.IsValid)
                {
                    var userId = _httpContext.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
                    User user = await _userManager.FindByIdAsync(model.UserId);
                    User admin = await _userManager.FindByIdAsync(userId);
                    if (admin.Role != "Admin")
                        return Unauthorized(new ResponseModel(401, "Only Admins are allowed", model));
                    var currencyChangeResult = await _walletRepo.ChangeMainCurrency(model);
                    if(currencyChangeResult > 0)
                    {
                        return Ok(new ResponseModel(200, "Main Currency Successfully changed", model));
                    }
                    return Unauthorized(new ResponseModel(401, "Only Admins can Change main currency", model));
                }
                var errors = ModelState.Values.Select(model => model.Errors).ToList();
                return BadRequest(new ResponseModel(400, "There are some validation Errors", errors));
            }
            catch (System.Exception)
            {
                return BadRequest(new ResponseModel(400, "There are some validation Errors", model));
            }
        }

        [HttpPost]
        [Route("fund", Name = "request")]
        public async Task<IActionResult> FundWallet(FundRequestDTO model)
        {
            try
            {
                model.FundingCurrency = model.FundingCurrency.ToUpper();
                if (ModelState.IsValid)
                {
                    var userId = _httpContext.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
                    var user = await _userManager.FindByIdAsync(userId);
                    var wallet = await _walletRepo.GetWalletByCurrencyAndUserId(model.FundingCurrency, userId);
                    if(user.Role == "Admin")
                    {
                        var fundingResult = await _walletRepo.AdminFunding(model);
                        if (fundingResult > 0)
                            return Ok(new ResponseModel(200, "Admin funding successful", model));
                        return BadRequest(new ResponseModel(400, "Admins funding failed", model));
                    }
                    if (wallet != null && user.Role == "Noob")
                    {
                        var walletCurrency = wallet.Currency;
                        var fundRequest = _mapper.Map<Fund>(model);
                        var convertedAmount = await Currency.ConvertCurrency(model.FundingCurrency, wallet.Currency, model.Amount);
                        fundRequest.Amount = convertedAmount;
                        fundRequest.FundingCurrency = walletCurrency;
                        var requestResult = await _walletRepo.RequestFunding(fundRequest);
                        if (requestResult > 0)
                            return Created("request", new ResponseModel(200, "Request successful", model));
                        return BadRequest(new ResponseModel(400, "Funding Failed", model));
                    }
                    else if (user.Role == "Elite")
                    {
                        int fundRequestResult = 0;
                        if (wallet == null)
                            fundRequestResult = await _walletRepo.AddWAllet(new Wallet { Currency = model.FundingCurrency, Balance = model.Amount, UserId = userId });
                        else
                            fundRequestResult = await _walletRepo.FundWallet(wallet.Id, model.Amount);

                        if (fundRequestResult > 0)
                            return Ok(new ResponseModel(200, "Account successfully funded", model));
                        return BadRequest(new ResponseModel(400, "Funding Failed", model));
                    }
                    return NotFound(new ResponseModel(404, "Wallet was not found", model));
                }
                var errors = ModelState.Values.Select(model => model.Errors).ToList();
                return BadRequest(new ResponseModel(400, "There are some validation Errors", errors));
            }
            catch (System.Exception)
            {
                return BadRequest(new ResponseModel(400, "There are some validation Errors", model));
            }
        }

        [HttpPost]
        [Route("approve")]
        public async Task<IActionResult> ApproveFund(int requestId)
        {
            try
            {
                var userId = _httpContext.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null || user.Role != "Admin")
                    return Unauthorized(new ResponseModel(401, "Not permitted", requestId));
                var request = _walletRepo.GetRequestById(requestId);
                if(request != null)
                {
                    var approvalResult = await _walletRepo.ApproveFundRequest(requestId, userId);
                    if (approvalResult > 0)
                        return Ok(new ResponseModel(200, "Funding successfully approved", request));
                }
                return NotFound(new ResponseModel(404, "Request does not exist", requestId));
            }
            catch (System.Exception)
            {
                return BadRequest(new ResponseModel(400, "There are some validation Errors", requestId));
            }
        }

        [HttpPost]
        [Route("withdraw")]
        public async Task<IActionResult> WithdrawFund(WithdrawalDTO model)
        {
            try
            {
                model.Currency = model.Currency.ToUpper();
                if (ModelState.IsValid)
                {
                    var userId = _httpContext.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
                    var user = await _userManager.FindByIdAsync(userId);
                    var wallet = await _walletRepo.GetWalletByCurrencyAndUserId(model.Currency, userId);
                    if (user.Role == "Admin")
                        return Unauthorized(new ResponseModel(401, "Admin cannot withdraw Funds", model));
                    if (wallet != null && wallet.UserId == userId)
                    {
                        if (user.Role == "Noob" && wallet.Currency != model.Currency)
                            model.Amount = await Currency.ConvertCurrency(model.Currency, wallet.Currency, model.Amount);
                        var withdrawalResult = await _walletRepo.Withdrawfunds(new WithdrawFundsDTO { Amount = model.Amount, WalletId = wallet.Id }, userId);
                        if (withdrawalResult > 0)
                            return Ok(new ResponseModel(200, "Success", model));
                        return BadRequest(new ResponseModel(400, "Withdrawal Failed", model));
                    }
                    return NotFound(new ResponseModel(404, "Wallet does not exist", model));
                }
                var errors = ModelState.Values.Select(model => model.Errors).ToList();
                return BadRequest(new ResponseModel(400, "There are some validation Errors", errors));
            }
            catch (System.Exception)
            {
                return BadRequest(new ResponseModel(400, "There are some validation Errors", model));
            }
        }

        [HttpGet]
        [Route("{currency}")]
        public async Task<IActionResult> GetWalletByCurrency(string currency)
        {
            try
            {
                var adminId = _httpContext.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
                var admin = await _userManager.FindByIdAsync(adminId);
                if (admin.Role == "Admin")
                {
                    var wallets = await _walletRepo.GetWalletByCurrency(currency);
                    return Ok(new ResponseModel(200, "success", wallets));
                }
                return Unauthorized(new ResponseModel(401, "You are not authorized to access this route", currency));
            }
            catch (Exception)
            {
                return BadRequest(new ResponseModel(400, "Login Failed", null));
            }
        }

        [HttpGet]
        [Route("{userId}")]
        public async Task<IActionResult> GetWalletByUserId(string userId)
        {
            try
            {
                var adminId = _httpContext.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
                var admin = await _userManager.FindByIdAsync(adminId);
                if (admin.Role == "Admin")
                {
                    var wallets = await _walletRepo.GetWalletByUserId(userId);
                    return Ok(new ResponseModel(200, "success", wallets));
                }
                return Unauthorized(new ResponseModel(401, "You are not authorized to access this route", userId));
            }
            catch (Exception)
            {
                return BadRequest(new ResponseModel(400, "Login Failed", null));
            }
        }

        [HttpGet]
        [Route("")]
        public async Task<IActionResult> GetAllWallet()
        {
            try
            {
                var userId = _httpContext.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
                var user = await _userManager.FindByIdAsync(userId);
                if (user.Role == "Admin")
                {
                    var wallets = await _walletRepo.GetWallets();
                    return Ok(new ResponseModel(200, "success", wallets));
                }
                return Unauthorized(new ResponseModel(401, "You are not authorized to access this route", userId));
            }
            catch (Exception)
            {
                return BadRequest(new ResponseModel(400, "Login Failed", null));
            }
        }

        [HttpGet]
        [Route("{walletId}")]
        public async Task<IActionResult> GetAllWalletById(int walletId)
        {
            try
            {
                var userId = _httpContext.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
                var user = await _userManager.FindByIdAsync(userId);
                if (user.Role == "Admin")
                {
                    var wallets = await _walletRepo.GetWalletById(walletId);
                    return Ok(new ResponseModel(200, "success", wallets));
                }
                return Unauthorized(new ResponseModel(401, "You are not authorized to access this route", userId));
            }
            catch (Exception)
            {
                return BadRequest(new ResponseModel(400, "Login Failed", null));
            }
        }


        [HttpDelete]
        [Route("delete{walletId}")]
        public async Task<IActionResult> DeleteWallet(int walletId)
        {
            try
            {
                var userId = _httpContext.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
                var user = await _userManager.FindByIdAsync(userId);
                if (user.Role == "Admin")
                {
                    var wallets = await _walletRepo.DeleteWallet(walletId);
                    return Ok(new ResponseModel(200, "success", walletId));
                }
                return Unauthorized(new ResponseModel(401, "You are not authorized to access this route", userId));
            }
            catch (Exception)
            {
                return BadRequest(new ResponseModel(400, "Login Failed", null));
            }
        }
    }
}
