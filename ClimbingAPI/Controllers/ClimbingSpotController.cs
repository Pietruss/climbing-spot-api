using ClimbingAPI.Models.ClimbingSpot;
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

        [HttpGet("{id}")]
        [AllowAnonymous]
        public ActionResult<ClimbingSpotDto> Get([FromRoute]int id)
        {
            var climbingSpot =_service.Get(id);
            return Ok(climbingSpot);
        }

        [HttpPost]
        public ActionResult Create([FromBody]CreateClimbingSpotDto dto)
        {
            var id = _service.Create(dto);
            return Created($"/climbingSpot/{id}", null);
        }

        [HttpDelete("{id}")]
        public ActionResult Delete([FromRoute] int id)
        {
            _service.Delete(id);
            return NoContent();
        }

        [HttpPut("{id}")]
        public ActionResult Update([FromBody] UpdateClimbingSpotDto dto, [FromRoute] int id)
        {   
            _service.Update(dto, id);
            return Ok();
        }
    }
}
