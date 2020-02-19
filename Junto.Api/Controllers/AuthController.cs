using Junto.Application.Models;
using Junto.Application.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Junto.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        private readonly IConfiguration _config;
        private readonly IUserService _userService;

        public AuthController(IUserService userService, IConfiguration config)
        {
            _config = config;
            _userService = userService;
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult> CreateToken([FromBody]UserAuthentication authRequest)
        {
            if (authRequest == null)
                return Unauthorized();

            var validUser = await _userService.ValidateAuth(authRequest);

            if (validUser != null)
                return Ok(new { Token = BuildToken(validUser) });

            return Unauthorized();
        }

        private string BuildToken(UserResponse validUser)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JwtToken:SecretKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(_config["JwtToken:Issuer"],
                _config["JwtToken:Issuer"],
                expires: DateTime.Now.AddMinutes(30),
                claims: new List<Claim>() { 
                    new Claim(ClaimTypes.Name, validUser.Name),
                    new Claim(ClaimTypes.NameIdentifier, validUser.Id.ToString())
                },
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
