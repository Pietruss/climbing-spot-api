using ClimbingAPI.Models.Boulder;
using ClimbingAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;

namespace ClimbingAPI.Controllers
{
    [ApiController]
    [Route(("/climbingSpot/{climbingSpotId}/boulder"))]
    [Authorize]
    public class BoulderController: ControllerBase
    {
        private readonly IBoulderService _service;

        public BoulderController(IBoulderService service)
        {
            _service = service;
        }
        [HttpPost]
        public ActionResult CreateBoulder([FromBody]CreateBoulderModelDto dto, [FromRoute] int climbingSpotId)
        {
            var id = _service.Create(dto, climbingSpotId);
            return Created($"/climbingSpot/{climbingSpotId}/boulder/{id}", null);
        }

        [HttpGet]
        [Route("{boulderId}")]
        [AllowAnonymous]
        public ActionResult<BoulderDto> Get([FromRoute]int climbingSpotId, [FromRoute] int boulderId)
        {
            var boulderEntity = _service.Get(climbingSpotId, boulderId);
            return Ok(boulderEntity);
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult<List<BoulderDto>> GetAll(int climbingSpotId)
        {
            var climbingList = _service.GetAll(climbingSpotId);
            return Ok(climbingList);
        }

        [HttpDelete("{boulderId}")]
        public ActionResult Delete([FromRoute] int climbingSpotId, [FromRoute] int boulderId)
        {
            _service.Delete(climbingSpotId, boulderId);
            return NoContent();
        }

        [HttpPut("{boulderId}")]
        public ActionResult Update([FromRoute] int climbingSpotId, [FromRoute] int boulderId, [FromBody] UpdateBoulderDto dto)
        {
            _service.Update(climbingSpotId, boulderId, dto);
            return Ok();
        }

        [HttpDelete]
        public ActionResult DeleteAll([FromRoute] int climbingSpotId)
        {
            _service.DeleteAll(climbingSpotId);
            return NoContent();
        }
    }
}
