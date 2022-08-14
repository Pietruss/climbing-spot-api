using AutoMapper;
using ClimbingAPI.Authorization;
using ClimbingAPI.Entities;
using ClimbingAPI.Entities.Boulder;
using ClimbingAPI.Exceptions;
using ClimbingAPI.Models.Boulder;
using ClimbingAPI.Services.Interfaces;
using ClimbingAPI.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ClimbingAPI.Services
{
    public class BoulderService: IBoulderService
    {
        private readonly ClimbingDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly ILogger<BoulderService> _logger;
        private readonly IUserContextService _userContext;
        private readonly IAuthorizationService _authorizationService;

        public BoulderService(ClimbingDbContext dbContext, IMapper mapper, ILogger<BoulderService> logger, IUserContextService userContext, IAuthorizationService authorizationService)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _logger = logger;
            _userContext = userContext;
            _authorizationService = authorizationService;
        }

        public int Create(CreateBoulderModelDto dto, int climbingSpotId)
        {
            _logger.LogInformation($"INFO for: CREATE action from BoulderService. Climbing Spot ID: \"{climbingSpotId}\".");

            var authorizationResult = Authorize(climbingSpotId, 0, ResourceOperation.Create);
            if (!authorizationResult.Succeeded)
            {
                _logger.LogError($"ERROR for: DELETE action from ClimbingSpotService. Authorization failed.");
                throw new UnAuthorizeException($"Authorization failed.");
            }

            var boulderEntity = _mapper.Map<Boulder>(dto);
            boulderEntity.ClimbingSpotId = climbingSpotId;
            boulderEntity.CreatedById = _userContext.GetUserId;

            _dbContext.Boulder.Add(boulderEntity);
            
            WhoColumns.CreationFiller(boulderEntity, _userContext.GetUserId, DateTime.Now);
            _dbContext.SaveChanges();

            return boulderEntity.Id;
        }

        public BoulderDto Get(int climbingSpotId, int boulderId)
        {
            _logger.LogInformation($"INFO for: GET action from BoulderService. Boulder Id: \"{boulderId}\", Climbing Spot Id: {climbingSpotId}.");

            var climbingSpot = GetClimbingSpotById(climbingSpotId);

            var boulder = climbingSpot.Boulder.FirstOrDefault(x => x.Id == boulderId);

            if (boulder is null)
            {
                _logger.LogError($"ERROR for: GET action from Boulder Service. Boulder ID: \"{boulderId}\" in Climbing Spot ID: {climbingSpotId} not found.");
                throw new NotFoundException($"Boulder with ID: {boulderId} not found for Climbing spot with ID: {climbingSpotId}.");
            }

            var boulderDto = _mapper.Map<BoulderDto>(boulder);

            return boulderDto;
        }

        public List<BoulderDto> GetAll(int climbingSpotId)
        {
            _logger.LogInformation($"INFO for: GETALL action from BoulderService. Climbing Spot Id: {climbingSpotId}.");

            var climbingSpot = GetClimbingSpotById(climbingSpotId);
            var boulderListDto = _mapper.Map<List<BoulderDto>>(climbingSpot.Boulder);

            return boulderListDto;
        }

        public void Delete(int climbingSpotId, int boulderId)
        {
            _logger.LogInformation($"INFO for: DELETE action from BoulderService. Boulder Id: {boulderId}.");

            var authorizationResult = Authorize(climbingSpotId, boulderId, ResourceOperation.Delete);
            if (!authorizationResult.Succeeded)
            {
                _logger.LogError($"ERROR for: DELETE action from ClimbingSpotService. Authorization failed.");
                throw new UnAuthorizeException($"Authorization failed.");
            }

            var climbingSpot = GetClimbingSpotById(climbingSpotId);
            
            var boulder = climbingSpot.Boulder.FirstOrDefault(x => x.Id == boulderId);
            if (boulder is null)
            {
                _logger.LogError($"ERROR for: DELETE action from Boulder Service. Boulder with ID: \"{boulderId}\" not found.");
                throw new NotFoundException($"Boulder with ID: {boulderId} for Climbing Spot ID: {climbingSpotId} not found.");
            }

            _dbContext.Boulder.Remove(boulder);
            _dbContext.SaveChanges();
        }

        public void DeleteAll(int climbingSpotId)
        {
            _logger.LogInformation($"INFO for: DELETEALL action from BoulderService. Climbing Spot Id: {climbingSpotId}.");

            var authorizationResult = Authorize(climbingSpotId, 0, ResourceOperation.Delete);
            if (!authorizationResult.Succeeded)
            {
                _logger.LogError($"ERROR for: DELETE action from ClimbingSpotService. Authorization failed.");
                throw new UnAuthorizeException($"Authorization failed.");
            }

            var climbingSpot = GetClimbingSpotById(climbingSpotId);
            _dbContext.RemoveRange(climbingSpot.Boulder);

            _dbContext.SaveChanges();
        }

        private ClimbingSpot GetClimbingSpotById(int id)
        {
            var climbingSpot =_dbContext
                .ClimbingSpot
                .AsNoTracking()
                .Include(x => x.Boulder)
                .FirstOrDefault(x => x.Id == id);
            if (climbingSpot is null)
            {
                _logger.LogError($"ERROR: action from Boulder Service GetClimbingSpotById(). Climbing Spot with ID: \"{id}\" not found.");
                throw new NotFoundException($"Climbing Spot with ID: \"{id}\" not found.");
            }

            return climbingSpot;
        }

        public void Update(int climbingSpotId, int boulderId, UpdateBoulderDto dto)
        {
            _logger.LogInformation($"INFO for: UPDATE action from BoulderService. Boulder Id: {boulderId}.");

            var authorizationResult = Authorize(climbingSpotId, boulderId, ResourceOperation.Update);
            if (!authorizationResult.Succeeded)
            {
                _logger.LogError($"ERROR for: UPDATE action from ClimbingSpotService. Authorization failed.");
                throw new UnAuthorizeException($"Authorization failed.");
            }
            var boulder = _dbContext.Boulder.FirstOrDefault(x => x.Id == boulderId);
            boulder = UpdateBoulderField(boulder, dto);

            WhoColumns.ModificationFiller(boulder, _userContext.GetUserId, DateTime.Now);
            _dbContext.Boulder.Update(boulder);
            _dbContext.SaveChanges();
        }

        private AuthorizationResult Authorize(int climbingSpotId, int boulderId, ResourceOperation resourceOperation)
        {
            var authorizationBoulder = new BoulderAuthorization()
            {
                BoulderId = boulderId,
                ClimbingSpotId = climbingSpotId
            };

            return _authorizationService.AuthorizeAsync(_userContext.User, authorizationBoulder,
                new ResourceOperationRequirement(resourceOperation)).Result;
        }

        private Boulder UpdateBoulderField(Boulder boulder, UpdateBoulderDto dto)
        {
            boulder.Name = dto.Name;
            boulder.Level = dto.Level;
            boulder.Description = dto.Description;

            return boulder;
        }
    }
}
