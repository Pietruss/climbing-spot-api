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
        public ActionResult<BoulderDto> Get([FromRoute]int boulderId, [FromRoute] int climbingSpotId)
        {
            var boulderEntity = _service.Get(boulderId, climbingSpotId);
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
        public ActionResult Delete([FromRoute] int boulderId, [FromRoute] int climbingSpotId)
        {
            _service.Delete(boulderId, climbingSpotId);
            return NoContent();
        }

        [HttpDelete]
        public ActionResult DeleteAll([FromRoute] int climbingSpotId)
        {
            _service.DeleteAll(climbingSpotId);
            return NoContent();
        }
    }
}
