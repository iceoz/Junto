using Junto.Application.Models;
using Junto.Application.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Junto.Api.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        private readonly IUserService _userAppService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        private int UserId => int.Parse(_httpContextAccessor.HttpContext.User.FindFirst(x => x.Type == ClaimTypes.NameIdentifier).Value);

        public UserController(IUserService userAppService, IHttpContextAccessor httpContextAccessor)
        {
            _userAppService = userAppService;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(401)]
        public async Task<ActionResult<IEnumerable<UserResponse>>> GetAll()
            => Ok(await _userAppService.GetAll());        

        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(401)]
        [Route("{id}")]
        public async Task<ActionResult<UserResponse>> GetOne([FromRoute] int id)
            => Ok(await _userAppService.GetOne(id));

        [AllowAnonymous]
        [ProducesResponseType(201)]
        [ProducesResponseType(404)]
        [HttpPost]
        public async Task<ActionResult<UserResponse>> Create(CreateUser user)
        {
            var userResponse = await _userAppService.Create(user);

            return CreatedAtAction(nameof(GetOne), new { id = userResponse.Id }, userResponse);
        }

        [HttpPut]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> Update(UpdateUser user)
        {
            await _userAppService.Update(user);
            return Ok();
        }

        [HttpPut]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [Route("change-password")]
        public async Task<ActionResult> UpdatePassword([FromBody]UpdateUserPassword password)
        { 
            await _userAppService.UpdatePassword(UserId, password);
            return Ok();
        }

        [HttpDelete]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [Route("{id}")]
        public async Task<ActionResult> Delete([FromRoute] int id)
        {
            await _userAppService.Delete(id);
            return Ok();
        }
    }
}
