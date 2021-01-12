using AutoMapper;
using EWallet.Api.DTOs;
using EWallet.Api.Model;
using EWallet.Api.Services.Repositories;
using EWallet.Api.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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

        [HttpPost]
        [Route("currency")]
        public async Task<IActionResult> ChangeWalletMainCurrency(string newCurrency)
        {
            var userId = _httpContext.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var currencyChangeResult = await _walletRepo.ChangeMainCurrency(newCurrency, userId);
            if(currencyChangeResult == 2)
            {
                return Ok(new ResponseModel(200, "Main Currency Successfully changed", newCurrency));
            }
            return BadRequest(new ResponseModel(400, "Failed to update", newCurrency));
        }

        [HttpPost]
        [Route("fund")]
        public async Task<IActionResult> FundWallet(FundRequestDTO model)
        {
            if (ModelState.IsValid)
            {

            }
            var errors = ModelState.Values.Select(model => model.Errors).ToList();
            return BadRequest(new ResponseModel(400, "There are some validation Errors", errors));
        }
    }
}
