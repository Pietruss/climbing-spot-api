﻿using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace ClimbingAPI.Services.Interfaces
{
    public interface IImageService
    {
        Task UploadImage(int boulderId, IFormFile img);

        Task<string> Get(int imageId);
    }
}
