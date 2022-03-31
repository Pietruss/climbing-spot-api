using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClimbingAPI.Entities.Boulder;
using ClimbingAPI.Models.Boulder;
using ClimbingAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ClimbingAPI.Controllers
{
    [ApiController]
    [Route(("/climbingSpot/{climbingSpotId}/boulder"))]
    public class BoulderController: ControllerBase
    {
        private readonly ILogger<BoulderController> _logger;
        private readonly IBoulderService _service;

        public BoulderController(ILogger<BoulderController> logger, IBoulderService service)
        {
            _logger = logger;
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
        public ActionResult<BoulderDto> Get([FromRoute]int boulderId, [FromRoute] int climbingSpotId)
        {
            var boulderEntity = _service.Get(boulderId, climbingSpotId);
            return Ok(boulderEntity);
        }

        [HttpGet]
        public ActionResult<List<BoulderDto>> GetAll(int climbingSpotId)
        {
            var climbingList = _service.GetAll(climbingSpotId);
            return Ok(climbingList);
        }
    }
}
