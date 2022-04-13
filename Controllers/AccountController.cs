using ClimbingAPI.Entities;
using ClimbingAPI.Models.ClimbingSpot;
using ClimbingAPI.Models.User;
using ClimbingAPI.Models.UserClimbingSpot;
using ClimbingAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

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
        public ActionResult Login([FromBody]LoginUserDto dto)
        {
            var token = _service.GenerateJwt(dto);
            return Ok(token);
        }

        [HttpPost("assign-climbing-spot")]
        [Authorize(Roles = "Admin,Manager")]
        public ActionResult AssignClimbingSpotToUser([FromBody] UpdateUserClimbingSpotDto dto)
        {
            _service.AssignClimbingSpotToUserWithRole(dto, User);
            return Ok();
        }

    }
}
