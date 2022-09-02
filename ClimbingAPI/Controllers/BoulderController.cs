using ClimbingAPI.Models.Boulder;
using ClimbingAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;

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
        public async Task<ActionResult> CreateBoulder([FromBody]CreateBoulderModelDto dto, [FromRoute] int climbingSpotId)
        {
            var id = await _service.Create(dto, climbingSpotId);
            return Created($"/climbingSpot/{climbingSpotId}/boulder/{id}", null);
        }

        [HttpGet]
        [Route("{boulderId}")]
        [AllowAnonymous]
        [ResponseCache(Duraktion = 1200)]
        public async Task<ActionResult<BoulderDto>> Get([FromRoute]int climbingSpotId, [FromRoute] int boulderId)
        {
            var boulderEntity = await _service.Get(climbingSpotId, boulderId);
            return Ok(boulderEntity);
        }

        [HttpGet]
        [AllowAnonymous]
        [ResponseCache(Duration = 1200)]
        public async Task<ActionResult<List<BoulderDto>>> GetAll(int climbingSpotId)
        {
            var climbingList = await _service.GetAll(climbingSpotId);
            return Ok(climbingList);
        }

        [HttpDelete("{boulderId}")]
        public async Task<ActionResult> Delete([FromRoute] int climbingSpotId, [FromRoute] int boulderId)
        {
            await _service.Delete(climbingSpotId, boulderId);
            return NoContent();
        }

        [HttpPatch("{boulderId}")]
        public async Task<ActionResult> Update([FromRoute] int climbingSpotId, [FromRoute] int boulderId, [FromBody] UpdateBoulderDto dto)
        {
            await _service.Update(climbingSpotId, boulderId, dto);
            return Ok();
        }

        [HttpDelete]
        public async Task<ActionResult> DeleteAll([FromRoute] int climbingSpotId)
        {
            await _service.DeleteAll(climbingSpotId);
            return NoContent();
        }
    }
}
