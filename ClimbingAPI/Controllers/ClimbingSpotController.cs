using ClimbingAPI.Models.ClimbingSpot;
using ClimbingAPI.Models.UserClimbingSpot;
using ClimbingAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace ClimbingAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class ClimbingSpotController: ControllerBase
    {
        private readonly IClimbingSpotService _service;

        public ClimbingSpotController(IClimbingSpotService service)
        {
            _service = service;
        }

        [HttpGet()]
        [AllowAnonymous]
        public ActionResult<IEnumerable<ClimbingSpotDto>> GetAll()
        {
            var result = _service.GetAll();
            return Ok(result);
        }

        [HttpGet("{climbingSpotId}")]
        [AllowAnonymous]
        public ActionResult<ClimbingSpotDto> Get([FromRoute]int climbingSpotId)
        {
            var climbingSpot =_service.Get(climbingSpotId);
            return Ok(climbingSpot);
        }

        [HttpPost]
        public ActionResult Create([FromBody]CreateClimbingSpotDto dto)
        {
            var id = _service.Create(dto);
            return Created($"/climbingSpot/{id}", null);
        }

        [HttpDelete("{climbingSpotId}")]
        public ActionResult Delete([FromRoute] int climbingSpotId)
        {
            _service.Delete(climbingSpotId);
            return NoContent();
        }

        [HttpPut("{id}")]
        public ActionResult Update([FromBody] UpdateClimbingSpotDto dto, [FromRoute] int climbingSpotId)
        {   
            _service.Update(dto, climbingSpotId);
            return Ok();
        }

        [HttpPost("assign-climbing-spot")]
        [Authorize]
        public ActionResult AssignClimbingSpotToUser([FromBody] UpdateUserClimbingSpotDto dto)
        {
            _service.AssignClimbingSpotToUserWithRole(dto);
            return Ok();
        }
    }
}
