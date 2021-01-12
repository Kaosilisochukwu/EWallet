using AutoMapper;
using EWallet.Api.DTOs;
using EWallet.Api.Model;
using EWallet.Api.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EWallet.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _config;
        private readonly IMapper _mapper;

        public UserController( UserManager<User> userManager, SignInManager<User> signInManager, IHttpContextAccessor httpContextAccessor, IConfiguration config, IMapper mapper)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _httpContextAccessor = httpContextAccessor;
            _config = config;
            _mapper = mapper;
        }
        [HttpPost]
        [Route("register", Name = "register")]
        public async Task<IActionResult> RegisterUser([FromBody] UserToRegisterDTO model)
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

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] UserToLoginDTO model)
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
    }
}
