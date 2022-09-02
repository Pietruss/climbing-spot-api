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
using System.Threading.Tasks;

namespace ClimbingAPI.Services
{
    public class BoulderService: IBoulderService
    {
        private readonly ClimbingDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly ILogger<BoulderService> _logger;
        private readonly IUserContextService _userContext;
        private readonly IAuthorizationService _authorizationService;
        private readonly IClimbingSpotService _climbingSpotService;

        public BoulderService(ClimbingDbContext dbContext, IMapper mapper, ILogger<BoulderService> logger, IUserContextService userContext, IAuthorizationService authorizationService, IClimbingSpotService climbingSpotService)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _logger = logger;
            _userContext = userContext;
            _authorizationService = authorizationService;
            _climbingSpotService = climbingSpotService;
        }

        public async Task<int> Create(CreateBoulderModelDto dto, int climbingSpotId)
        {
            _logger.LogInformation($"INFO for: CREATE action from BoulderService. Climbing Spot ID: \"{climbingSpotId}\".");

            await VerifyUserData(climbingSpotId, 0, ResourceOperation.Create, Literals.Literals.CreateBoulderAction.GetDescription());

            var boulderEntity = _mapper.Map<Boulder>(dto);
            boulderEntity.ClimbingSpotId = climbingSpotId;
            boulderEntity.CreatedById = _userContext.GetUserId;

            _dbContext.Boulder.Add(boulderEntity);
            
            WhoColumns.CreationFiller(boulderEntity, _userContext.GetUserId, DateTime.Now);
            _dbContext.SaveChanges();

            return boulderEntity.Id;
        }

        public async Task<BoulderDto> Get(int climbingSpotId, int boulderId)
        {
            _logger.LogInformation($"INFO for: GET action from BoulderService. Boulder Id: \"{boulderId}\", Climbing Spot Id: {climbingSpotId}.");

            await _climbingSpotService.ValidateClimbingSpotById(climbingSpotId);

            var boulder = await GetBoluderInClimbingSpot(climbingSpotId, boulderId);

            if (boulder is null)
            {
                _logger.LogError($"ERROR for: GET action from Boulder Service. Boulder ID: \"{boulderId}\" in Climbing Spot ID: {climbingSpotId} not found.");
                throw new NotFoundException($"Boulder with ID: {boulderId} not found for Climbing spot with ID: {climbingSpotId}.");
            }

            var boulderDto = _mapper.Map<BoulderDto>(boulder);

            return boulderDto;
        }

        public async Task<List<BoulderDto>> GetAll(int climbingSpotId)
        {
            _logger.LogInformation($"INFO for: GETALL action from BoulderService. Climbing Spot Id: {climbingSpotId}.");

            await _climbingSpotService.ValidateClimbingSpotById(climbingSpotId);

            var boulderLists = await GetAllBoludersInClimbingSpot(climbingSpotId);
            var boulderListDto = _mapper.Map<List<BoulderDto>>(boulderLists);

            return boulderListDto;
        }

        private async Task<List<Boulder>> GetAllBoludersInClimbingSpot(int climbingSpotId)
        {
            return await _dbContext
                .Boulder
                .Where(x => x.ClimbingSpotId == climbingSpotId)
                .Select(x => new Boulder() { Id = x.Id, Name = x.Name, Description = x.Description, ModificationDateTime = x.ModificationDateTime, CreatedById = x.CreatedById})
                .ToListAsync();
        }

        private async Task<Boulder> GetBoluderInClimbingSpot(int climbingSpotId, int boulderId)
        {
            var boulder = await _dbContext
                .Boulder
                .Where(x => x.ClimbingSpotId == climbingSpotId && x.Id == boulderId)
                .Select(x => new Boulder() { Id = x.Id, Name = x.Name, Description = x.Description, ModificationDateTime = x.ModificationDateTime, CreatedById = x.CreatedById })
                .FirstOrDefaultAsync();
            if(boulder is null)
            {
                _logger.LogError($"ERROR for: GET action from Boulder Service. Boulder with ID: \"{boulderId}\" not found.");
                throw new NotFoundException($"Boulder with ID: {boulderId} for Climbing Spot ID: {climbingSpotId} not found.");
            }

            return boulder;
        }

        public async Task Delete(int climbingSpotId, int boulderId)
        {
            _logger.LogInformation($"INFO for: DELETE action from BoulderService. Boulder Id: {boulderId}.");

            await VerifyUserData(climbingSpotId, boulderId, ResourceOperation.Delete, Literals.Literals.DeleteBoulderAction.GetDescription());

            await _climbingSpotService.ValidateClimbingSpotById(climbingSpotId);

            var boulder = await GetBoluderInClimbingSpot(climbingSpotId, boulderId);
            if (boulder is null)
            {
                _logger.LogError($"ERROR for: DELETE action from Boulder Service. Boulder with ID: \"{boulderId}\" not found.");
                throw new NotFoundException($"Boulder with ID: {boulderId} for Climbing Spot ID: {climbingSpotId} not found.");
            }

            _dbContext.Boulder.Remove(boulder);
            _dbContext.SaveChanges();
        }

        public async Task DeleteAll(int climbingSpotId)
        {
            _logger.LogInformation($"INFO for: DELETEALL action from BoulderService. Climbing Spot Id: {climbingSpotId}.");

            await VerifyUserData (climbingSpotId, 0, ResourceOperation.Delete, Literals.Literals.DeleteAllBouldersAction.GetDescription());

            var boulderLists = await GetAllBoludersInClimbingSpot(climbingSpotId);
            _dbContext.RemoveRange(boulderLists);

            _dbContext.SaveChanges();
        }

        public async Task Update(int climbingSpotId, int boulderId, UpdateBoulderDto dto)
        {
            _logger.LogInformation($"INFO for: UPDATE action from BoulderService. Boulder Id: {boulderId}.");

            await VerifyUserData(climbingSpotId, boulderId, ResourceOperation.Update, Literals.Literals.UpdateBoulderAction.GetDescription());

            var boulder = _dbContext.Boulder.FirstOrDefault(x => x.Id == boulderId);
            boulder = UpdateBoulderField(boulder, dto);

            WhoColumns.ModificationFiller(boulder, _userContext.GetUserId, DateTime.Now);
            _dbContext.Boulder.Update(boulder);
            _dbContext.SaveChanges();
        }

        private async Task VerifyUserData(int climbingSpotId, int boulderId, ResourceOperation resourceOperation, string operation)
        {
            var authorizationResult = await Authorize(climbingSpotId, boulderId, resourceOperation);
            if (!authorizationResult.Succeeded)
            {
                _logger.LogError($"ERROR for: {operation} action from ClimbingSpotService. Authorization failed.");
                throw new UnAuthorizeException(Literals.Literals.AuthorizationFailed.GetDescription());
            }
        }

        private async Task<AuthorizationResult> Authorize(int climbingSpotId, int boulderId, ResourceOperation resourceOperation)
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
