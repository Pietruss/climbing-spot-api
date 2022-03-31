using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClimbingAPI.Entities;
using ClimbingAPI.Models.ClimbingSpot;
using ClimbingAPI.Services.Interfaces;
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
        public ActionResult<IEnumerable<ClimbingSpotDto>> GetAll()
        {
            var result = _service.GetAll();
            return Ok(result);
        }

        [HttpGet("{id}")]
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
        public ActionResult Update([FromBody] UpdateClimbingSpotDto dto, int id)
        {   
            _service.Update(dto, id);
            return Ok();
        }
    }
}
