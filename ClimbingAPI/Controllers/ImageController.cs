using ClimbingAPI.Models.Image;
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
        [ResponseCache(Duration = 1200)]
        public async Task<ActionResult<ImageDto>> Get([FromRoute] int imageId)
        {
            var imageDto = await _service.Get(imageId);
            return Ok(imageDto);
        }

        [HttpDelete("/boulder/{boulderid}/image/{imageId}")]
        [Authorize]
        public async Task<ActionResult> Delete([FromRoute] int imageId, [FromRoute] int boulderId)
        {
            await _service.Delete(imageId, boulderId);
            return NoContent();
        }
    }
}
