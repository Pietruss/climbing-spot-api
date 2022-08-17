using ClimbingAPI.Models.Image;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace ClimbingAPI.Services.Interfaces
{
    public interface IImageService
    {
        Task UploadImage(int boulderId, IFormFile img);
        Task<ImageDto> Get(int imageId);
        Task Delete(int imageId, int boulderId);
    }
}
