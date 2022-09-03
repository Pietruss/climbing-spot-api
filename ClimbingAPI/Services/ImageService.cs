using ClimbingAPI.Authorization;
using ClimbingAPI.Authorization.AuthorizationEntity;
using ClimbingAPI.Entities;
using ClimbingAPI.Exceptions;
using ClimbingAPI.Models.Image;
using ClimbingAPI.Services.Interfaces;
using ClimbingAPI.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
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

            ValidateImage(img);

            MemoryStream ms = new MemoryStream();
            img.CopyTo(ms);

            await ValidateBoulder(boulderId);

            var image = CreateNewImage(img, boulderId, ms.ToArray());

            _dbContext.Images.Add(image);
            WhoColumns.CreationFiller(image, _userContext.GetUserId, DateTime.Now);
            _dbContext.SaveChanges();

        }

        private void ValidateImage(IFormFile img)
        {
            if(img is null)
            {
                _logger.LogError($"ERROR for: {Literals.Literals.UploadImage} action from ImageService. Image is broken or empty.");
                throw new BadRequestException(Literals.Literals.BrokenImage.GetDescription());
            }
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
            var ClimbingSpotId = await _dbContext
                .Boulder
                .Where(x => x.Id == boulderId)
                .Select(x => x.ClimbingSpotId)
                .FirstOrDefaultAsync();

            if (ClimbingSpotId == 0)
            {
                _logger.LogError($"ERROR for: {Literals.Literals.UploadImage.GetDescription()} action from ImageService. Authorization failed.");
                throw new UnAuthorizeException(Literals.Literals.AuthorizationFailed.GetDescription());
            }
            return ClimbingSpotId;
        }

        private async Task ValidateBoulder(int boulderId)
        {
            var image = await _dbContext
                .Images
                .Where(x => x.BoulderId == boulderId)
                .Select(x => x.Id)
                .FirstOrDefaultAsync();

            if(image != 0)
            {
                _logger.LogError($"ERROR for: {Literals.Literals.UploadImage.GetDescription()} action from ImageService. Image is already assigned to boulder.");
                throw new BadRequestException(Literals.Literals.BoulderHasAssingedImage.GetDescription());
            }
        }

        public async Task<ImageDto> Get(int imageId)
        {
            _logger.LogInformation($"INFO for: {Literals.Literals.GetImageAction.GetDescription()} action from ImageService.");

            var image = await GetImage(imageId); 
            
            string imageDataURL = string.Format("data:image/jpg;base64,{0}", Convert.ToBase64String(image.ImageData));

            return new ImageDto()
            {
                ImageDataUrl = imageDataURL,
                ImageName = image.ImageTitle
            };
        }

        public async Task Delete(int imageId, int boulderId)
        {
            _logger.LogInformation($"INFO for: {Literals.Literals.DeleteImageAction.GetDescription()} action from ImageService.");

            await Authorize(ResourceOperation.Delete, Literals.Literals.DeleteImageAction.GetDescription(), boulderId);

            var image = await GetImage(imageId);

            _dbContext.Images.Remove(image);
            _dbContext.SaveChanges();
        }

        private async Task<Image> GetImage(int imageId)
        {
            var image = await _dbContext.Images.Where(x => x.Id == imageId).Select(x => new Image () {Id = x.Id, ImageData = x.ImageData, ImageTitle = x.ImageTitle }).FirstOrDefaultAsync();

            if (image == null)
            {
                _logger.LogError($"ERROR for: {Literals.Literals.DeleteImageAction.GetDescription()} action from ImageService. Image not found.");
                throw new NotFoundException(Literals.Literals.DeleteImage.GetDescription());
            }

            return image;
        }
    }
}
