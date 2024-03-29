﻿using ClimbingAPI.Models.User;
using ClimbingAPI.Models.UserClimbingSpot;
using ClimbingAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ClimbingAPI.Controllers
{
    [Route("/account/")]
    [ApiController]
    public class AccountController: ControllerBase
    {
        private readonly IAccountService _service;

        public AccountController(IAccountService service)
        {
            _service = service;
        }
        [HttpPost]
        [Route("register")]
        public ActionResult RegisterUser([FromBody] CreateUserDto dto)
        {
            _service.Register(dto);
            return Created("User has been created.", null);
        }

        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody]LoginUserDto dto)
        {
            var token = await _service.Login(dto);
            return Ok(token);
        }

        [HttpPatch("update-user/{userId}")]
        [Authorize]
        public async Task<ActionResult> Update([FromBody] UpdateUserDto dto, [FromRoute] int userId)
        {
            await _service.Update(dto, userId);
            return Ok();
        }

        [HttpPatch("change-password/{userId}")]
        [Authorize]
        public async  Task<ActionResult> ChangePassword([FromBody] UpdateUserPasswordDto dto, [FromRoute] int userId)
        {
            await _service.ChangePassword(dto, userId);
            return Ok();
        }

        [HttpPost("delete-user/{userId}")]
        [Authorize]
        public async Task<ActionResult> DeleteUser([FromBody] DeleteUserDto dto, [FromRoute] int userId)
        {
            await _service.DeleteUser(dto, userId);
            return NoContent();
        }
    }
}
