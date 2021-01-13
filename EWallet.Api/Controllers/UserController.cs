using AutoMapper;
using EWallet.Api.DTOs;
using EWallet.Api.Model;
using EWallet.Api.Services.Repositories;
using EWallet.Api.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EWallet.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserRepository _userRepo;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IHttpContextAccessor _httpContext;
        private readonly IConfiguration _config;
        private readonly IMapper _mapper;
        private readonly WalletRepository _walletRepo;

        public UserController(UserRepository userRepo, UserManager<User> userManager, SignInManager<User> signInManager, IHttpContextAccessor httpContextAccessor, IConfiguration config, IMapper mapper, WalletRepository walletRepo)
        {
            _userRepo = userRepo;
            _userManager = userManager;
            _signInManager = signInManager;
            _httpContext = httpContextAccessor;
            _config = config;
            _mapper = mapper;
            _walletRepo = walletRepo;
        }
        [HttpPost]
        [Route("register", Name = "register")]
        public async Task<IActionResult> RegisterUser([FromBody] UserToRegisterDTO model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var user = _mapper.Map<User>(model);
                    var registrationResult = await _userManager.CreateAsync(user, model.Password);
                    if (registrationResult.Succeeded)
                    {
                        var userToReturn = _mapper.Map<UserToReturn>(user);
                        return Created("register", new ResponseModel(201, "User Was successfully registered", userToReturn));
                    }
                    var error = registrationResult.Errors;
                    return BadRequest(new ResponseModel(400, "Registration Failed", error));
                }
                var errors = ModelState.Values.Select(model => model.Errors).ToList();
                return BadRequest(new ResponseModel(400, "There are some validation Errors", errors));
            }
            catch (Exception)
            {
                return BadRequest(new ResponseModel(400, "Failed", model));
            }
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] UserToLoginDTO model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var user = await _userManager.FindByEmailAsync(model.Email);
                    var signInResult = await _signInManager.PasswordSignInAsync(user, model.Password, true, false);
                    if (signInResult.Succeeded)
                    {
                        var token = TokenConfig.GenerateToken(user, _config);
                        return Ok(new ResponseModel(200, "User Was successfully Logged in", new { token }));
                    }
                    return BadRequest(new ResponseModel(400, "Login Failed", signInResult));
                }
                var errors = ModelState.Values.Select(model => model.Errors).ToList();
                return BadRequest(new ResponseModel(400, "There are some validation Errors", errors));
            }
            catch (Exception)
            {
                return BadRequest(new ResponseModel(400, "Failed", model));
            }
        }


        [HttpPost]
        [Route("role/toelite")]
        [Authorize]
        public async Task<IActionResult> NoobToElite(string userId)
        {
            try
            {
                var adminId = _httpContext.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
                var admin = await _userManager.FindByIdAsync(adminId);
                var user = await _userManager.FindByIdAsync(userId);
                if (admin != null && admin.Role == "Admin")
                {
                    var userToReturn = _mapper.Map<UserToReturn>(user);
                    var changeResult = await _walletRepo.PromoteNoob(userId);
                    if (changeResult > 0)
                    {
                        return Ok(new ResponseModel(200, "Success", userToReturn));
                    }
                    return BadRequest(new ResponseModel(400, "Login Failed", userToReturn));
                }
                return Unauthorized(new ResponseModel(401, "You are not authorized to access this route", adminId));
            }
            catch (Exception)
            {
                return BadRequest(new ResponseModel(400, "Failed", userId));
            }
        }


        [HttpPost]
        [Route("role/tonoob")]
        [Authorize]
        public async Task<IActionResult> ElitToNoob(string userId)
        {
            try
            {
                var adminId = _httpContext.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
                var admin = await _userManager.FindByIdAsync(adminId);
                var user = await _userManager.FindByIdAsync(userId);
                if (admin != null && admin.Role == "Admin")
                {
                    var userToReturn = _mapper.Map<UserToReturn>(user);
                    var changeResult = await _walletRepo.DemoteElite(userId);
                    if (changeResult > 0)
                    {
                        userToReturn.Role = "Noob";
                        return Ok(new ResponseModel(200, "Success", userToReturn));
                    }
                    return BadRequest(new ResponseModel(400, "Login Failed", userToReturn));
                }
                return Unauthorized(new ResponseModel(401, "You are not authorized to access this route", adminId));
            }
            catch (Exception)
            {
                return BadRequest(new ResponseModel(400, "Failed", userId));
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var userId = _httpContext.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
                var user = await _userManager.FindByIdAsync(userId);
                if (user.Role == "Admin")
                {
                    var users = await _userRepo.GetAllUsers();
                    return Ok(new ResponseModel(200, "success", users));
                }
                return Unauthorized(new ResponseModel(401, "You are not authorized to access this route", userId));
            }
            catch (Exception)
            {
                return BadRequest(new ResponseModel(400, "Login Failed", null));
            }
        }

        [HttpGet]
        [Route("{userId}")]
        public async Task<IActionResult> GetUsersById( string userId)
        {
            try
            {
                var adminId = _httpContext.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
                var admin = await _userManager.FindByIdAsync(adminId);
                if (admin.Role == "Admin")
                {
                    var user = await _userRepo.GetUserById(userId);
                    return Ok(new ResponseModel(200, "success", user));
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
