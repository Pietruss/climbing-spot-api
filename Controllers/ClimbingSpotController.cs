﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using ClimbingAPI.Entities;
using ClimbingAPI.Models.ClimbingSpot;
using ClimbingAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ClimbingAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ClimbingSpotController: ControllerBase
    {
        private readonly ILogger<ClimbingSpotController> _logger;
        private readonly IClimbingSpotService _service;

        public ClimbingSpotController(ILogger<ClimbingSpotController> logger, IClimbingSpotService service)
        {
            _logger = logger;
            _service = service;
        }

        [HttpGet]
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
            return NotFound();
        }

        [HttpPut("{id}")]
        public ActionResult Update([FromBody] UpdateClimbingSpotDto dto, [FromRoute] int id)
        {   
            _service.Update(dto, id);
            return Ok();
        }
    }
}
