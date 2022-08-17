using ClimbingAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ClimbingAPI.Controllers
{
    [Route("/image/")]
    [ApiController]
    public class ImageController: ControllerBase
    {
        private IImageService _service;
        public ImageController(IImageService imageService)
        {
            _service = imageService;
        }

        [HttpPost("{boulderId}")]
        [Authorize]
        public async Task<ActionResult> UploadImage([FromRoute] int boulderId, [FromForm]IFormFile img)
        {
            await _service.UploadImage(boulderId, img);
            return Created("Image has been uploaded.", null);
        }

        [HttpGet("{imageId}")]
        public async Task<ActionResult<string>> Get([FromRoute] int imageId)
        {
            await _service.Get(imageId);
            return Ok();
        }
    }
}
