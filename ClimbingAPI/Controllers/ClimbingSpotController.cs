﻿using ClimbingAPI.Models.ClimbingSpot;
using ClimbingAPI.Models.UserClimbingSpot;
using ClimbingAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

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
        [ResponseCache(Duration = 1200)]
        public async Task<ActionResult<IEnumerable<ClimbingSpotDto>>> GetAll()
        {
            var result = await _service.GetAll();
            return Ok(result);
        }

        [HttpGet("{climbingSpotId}")]
        [AllowAnonymous]
        [ResponseCache(Duration = 1200)]
        public async Task<ActionResult<ClimbingSpotDto>> Get([FromRoute]int climbingSpotId)
        {
            var climbingSpot = await _service.Get(climbingSpotId);
            return Ok(climbingSpot);
        }

        [HttpPost]
        public async Task<ActionResult> Create([FromBody]CreateClimbingSpotDto dto)
        {
            var id = await _service.Create(dto);
            return Created($"/climbingSpot/{id}", null);
        }

        [HttpDelete("{climbingSpotId}")]
        public async Task<ActionResult> Delete([FromRoute] int climbingSpotId)
        {
            await _service.Delete(climbingSpotId);
            return NoContent();
        }

        [HttpPatch("{climbingSpotId}")]
        public ActionResult Update([FromBody] UpdateClimbingSpotDto dto, [FromRoute] int climbingSpotId)
        {   
            _service.Update(dto, climbingSpotId);
            return Ok();
        }

        [HttpPost("assign-climbing-spot")]
        public async Task<ActionResult> AssignClimbingSpotToUser([FromBody] UpdateUserClimbingSpotDto dto)
        {
            await _service.AssignClimbingSpotToUserWithRole(dto);
            return Ok();
        }
    }
}
