using ClimbingAPI.Authorization;
using ClimbingAPI.Authorization.AuthorizationEntity;
using ClimbingAPI.Entities;
using ClimbingAPI.Exceptions;
using ClimbingAPI.Services.Interfaces;
using ClimbingAPI.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;

namespace ClimbingAPI.Services
{
    public class ImageService: IImageService
    {
        private readonly ILogger<ImageService> _logger;
        private readonly ClimbingDbContext _dbContext;
        private readonly IUserContextService _userContext;
        private readonly IAuthorizationService _authorizationService;

        public ImageService(ILogger<ImageService> logger, ClimbingDbContext dbContext, IUserContextService userContext, IBoulderService boulderService, IAuthorizationService authorizationService)
        {
            _logger = logger;
            _dbContext = dbContext;
            _userContext = userContext;
            _authorizationService = authorizationService;
        }


        public async Task UploadImage(int boulderId, IFormFile img)
        {
            _logger.LogInformation($"INFO for: {Literals.Literals.UploadImage.GetDescription()} action from ImageService.");

            await Authorize(ResourceOperation.Create, Literals.Literals.UploadImage.GetDescription(), boulderId);

            MemoryStream ms = new MemoryStream();
            img.CopyTo(ms);

            await ValidateBoulder(boulderId);

            var image = CreateNewImage(img, boulderId, ms.ToArray());

            _dbContext.Images.Add(image);
            WhoColumns.CreationFiller(image, _userContext.GetUserId, DateTime.Now);
            _dbContext.SaveChanges();

        }

        private Image CreateNewImage(IFormFile img, int boulderId, byte[] imageData)
        {
           return new Image()
            {
                ImageTitle = img.FileName,
                BoulderId = boulderId,
                ImageData = imageData
            };
        }

        private async Task Authorize(ResourceOperation resourceOperation, string operation, int boulderId)
        {
            var climbingSpotId = await GetClimbingSpotId(boulderId);

            var imageAuthorization = new ImageAuthorization()
            {
                ClimbingSpotId = climbingSpotId,
            };

            var authorizationResult = _authorizationService.AuthorizeAsync(_userContext.User, imageAuthorization,
                new ResourceOperationRequirement(resourceOperation)).Result;
            if (!authorizationResult.Succeeded)
            {
                _logger.LogError($"ERROR for: {operation} action from ImageService. Authorization failed.");
                throw new UnAuthorizeException(Literals.Literals.AuthorizationFailed.GetDescription());
            }
        }

        private async Task<int> GetClimbingSpotId(int boulderId)
        {
            var boulder = await _dbContext
                .Boulder
                .FirstOrDefaultAsync(x => x.Id == boulderId);

            if (boulder is null)
            {
                _logger.LogError($"ERROR for: {Literals.Literals.UploadImage.GetDescription()} action from ImageService. Authorization failed.");
                throw new UnAuthorizeException(Literals.Literals.AuthorizationFailed.GetDescription());
            }
            return boulder.ClimbingSpotId;
        }

        private async Task ValidateBoulder(int boulderId)
        {
            var image = await _dbContext
                .Images
                .FirstOrDefaultAsync(x => x.BoulderId == boulderId);

            if(image != null)
            {
                _logger.LogError($"ERROR for: {Literals.Literals.UploadImage.GetDescription()} action from ImageService. Image is already assigned to boulder.");
                throw new BadRequestException(Literals.Literals.BoulderHasAssingedImage.GetDescription());
            }
        }

        public async Task<string> Get(int imageId)
        {
            _logger.LogInformation($"INFO for: {Literals.Literals.GetImageAction.GetDescription()} action from ImageService.");

            var image = await _dbContext.Images.FirstOrDefaultAsync(x => x.Id == imageId);
            if(image is null)
            {
                _logger.LogError($"ERROR for: {Literals.Literals.GetImageAction.GetDescription()} action from ImageService. Image with id {imageId} not found.");
                throw new NotFoundException(Literals.Literals.ImageNotFound.GetDescription());
            }

            string imageBase64Data = Convert.ToBase64String(image.ImageData);
            string imageDataURL = string.Format("data:image/jpg;base64,{0}", imageBase64Data);

            return imageDataURL;
        }
    }
}
