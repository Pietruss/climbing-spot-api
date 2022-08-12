using AutoMapper;
using ClimbingAPI.Authorization;
using ClimbingAPI.Entities;
using ClimbingAPI.Exceptions;
using ClimbingAPI.Models.ClimbingSpot;
using ClimbingAPI.Models.UserClimbingSpot;
using ClimbingAPI.Services.Interfaces;
using ClimbingAPI.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace ClimbingAPI.Services
{
    public class ClimbingSpotService: IClimbingSpotService
    {
        private readonly ClimbingDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly ILogger<ClimbingSpotService> _logger;
        private readonly IAuthorizationService _authorizationService;
        private readonly IUserContextService _userContext;

        public ClimbingSpotService(ClimbingDbContext dbContext, IMapper mapper, ILogger<ClimbingSpotService>  logger, IAuthorizationService authorizationService, IUserContextService userContext)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _logger = logger;
            _authorizationService = authorizationService;
            _userContext = userContext;
        }
        public IEnumerable<ClimbingSpotDto> GetAll()
        {
            _logger.LogInformation("INFO for: GETALL action from ClimbingSpotService.");

            var climbingSpots = _dbContext
                .ClimbingSpot
                .Include(x => x.Address)
                .Include(x => x.Boulder)
                .ToList();

            var climbingSpotsDto = _mapper.Map<List<ClimbingSpotDto>>(climbingSpots);

            return climbingSpotsDto;
        }

        public ClimbingSpotDto Get(int id)
        {
            _logger.LogInformation($"INFO for: GET action from ClimbingSpotService. ID: \"{id}\".");

            var climbingSpot = _dbContext
                .ClimbingSpot
                .Include(x => x.Address)
                .Include(x => x.Boulder)
                .FirstOrDefault(x => x.Id == id);

            if(climbingSpot is null)
                throw new NotFoundException($"Restaurant with ID: {id} not found.");

            var climbingSpotDto = _mapper.Map<ClimbingSpotDto>(climbingSpot);

            return climbingSpotDto;
        }

        public int Create(CreateClimbingSpotDto dto)
        {
            _logger.LogInformation("INFO for: CREATE action from ClimbingSpotService.");

            VerifyUserAssignment();

            var climbingSpot = _mapper.Map<ClimbingSpot>(dto);
            climbingSpot.CreatedById = _userContext.GetUserId;

            _dbContext.ClimbingSpot.Add(climbingSpot);

            WhoColumns.CreationFiller(climbingSpot, _userContext.GetUserId);
            WhoColumns.CreationFiller(climbingSpot.Address, _userContext.GetUserId);

            _dbContext.SaveChanges();

            AssignClimbingSpotToUser(_userContext.GetUserId, climbingSpot.Id);

            _dbContext.SaveChanges();

            return climbingSpot.Id;
        }

        private void VerifyUserAssignment()
        {
            var userClimbingSpot = _dbContext.UserClimbingSpotLinks.FirstOrDefault(x =>
                x.UserId == _userContext.GetUserId && (x.RoleId == 1 || x.RoleId == 2));
            if(userClimbingSpot is null)
                throw new BadRequestException("You do not have enough rights to create ClimbingSpot.");
        }

        private void AssignClimbingSpotToUser(int? userId, int climbingSpotId)
        { 
            var climbingSpotUser = _dbContext.UserClimbingSpotLinks.FirstOrDefault(x => x.UserId == userId && x.ClimbingSpotId == null);
            if (climbingSpotUser is null && userId != null)
            {
                    var userClimbingSpotEntity = new UserClimbingSpotLinks()
                    {
                        ClimbingSpotId = climbingSpotId,
                        UserId = (int) userId,
                        RoleId = 2
                    };
                    WhoColumns.CreationFiller(userClimbingSpotEntity, _userContext.GetUserId);
                    _dbContext.UserClimbingSpotLinks.Add(userClimbingSpotEntity);
            }
            else
            {
                if (climbingSpotUser != null)
                {
                    climbingSpotUser.ClimbingSpotId = climbingSpotId;
                    WhoColumns.CreationFiller(climbingSpotUser, _userContext.GetUserId);
                }
            }
        }

        public void Delete(int id)
        {
            _logger.LogInformation($"INFO for: DELETE action from ClimbingSpotService. ID: \"{id}\".");

            var climbingSpot = _dbContext.ClimbingSpot.FirstOrDefault(x => x.Id == id);

            if (climbingSpot is null)
            {
                _logger.LogError($"ERROR for: DELETE action from ClimbingSpotService. ID: \"{id}\" not found.");
                throw new NotFoundException($"Restaurant with ID: {id} not found.");
            }

            var authorizationResult = _authorizationService.AuthorizeAsync(_userContext.User, climbingSpot,
                new ResourceOperationRequirement(ResourceOperation.Delete)).Result;
            if (!authorizationResult.Succeeded)
            {
                _logger.LogError($"ERROR for: DELETE action from ClimbingSpotService. Authorization failed.");
                throw new ForbidException($"Authorization failed.");
            }

            var userClimbingSpot = _dbContext.UserClimbingSpotLinks.Where(x => x.ClimbingSpotId == id);
            foreach (var item in userClimbingSpot)
            {
                _dbContext.UserClimbingSpotLinks.Remove(item);
            }

            _dbContext.ClimbingSpot.Remove(climbingSpot);
            _dbContext.SaveChanges();
        }

        public void Update(UpdateClimbingSpotDto dto, int id)
        {
            _logger.LogError($"INFO for: UPDATE action from ClimbingSpotService. ID: \"{id}\".");

            var climbingSpot = _dbContext.ClimbingSpot.FirstOrDefault(x => x.Id == id);

            if (climbingSpot is null)
            {
                _logger.LogError($"ERROR for: DELETE action from ClimbingSpotService. ID: \"{id}\" not found.");
                throw new NotFoundException($"Restaurant with ID: {id} not found.");
            }

            var authorizationResult = _authorizationService.AuthorizeAsync(_userContext.User, climbingSpot,
                new ResourceOperationRequirement(ResourceOperation.Update)).Result;
            if (!authorizationResult.Succeeded)
            {
                _logger.LogError($"ERROR for: DELETE action from ClimbingSpotService. Authorization failed.");
                throw new ForbidException($"Authorization failed.");
            }

            climbingSpot.Description = dto.Description;
            climbingSpot.Name = dto.Name;
            climbingSpot.ContactEmail = dto.ContactEmail;
            climbingSpot.ContactNumber = dto.ContactNumber;

            _dbContext.Update(climbingSpot);

            WhoColumns.ModificationFiller(climbingSpot, _userContext.GetUserId);

            _dbContext.SaveChanges();
        }

        public void AssignClimbingSpotToUserWithRole(UpdateUserClimbingSpotDto dto)
        {
            _logger.LogInformation("INFO for: AssignClimbingSpotToUserWithRole action from AccountService.");

            Validate(dto.UserId, dto.ClimbingSpotId, dto.RoleId, _userContext.User);

            var userClimbingSpotEntity = GetUserClimbingSpot(dto.UserId, dto.ClimbingSpotId, dto.RoleId);

            if (userClimbingSpotEntity is null)
            {
                var userClimbingSpot = new UserClimbingSpotLinks()
                {
                    ClimbingSpotId = dto.ClimbingSpotId,
                    UserId = dto.UserId,
                    RoleId = dto.RoleId
                };

                WhoColumns.CreationFiller(userClimbingSpot, _userContext.GetUserId);

                _dbContext.UserClimbingSpotLinks.Add(userClimbingSpot);
            }
            else
            {
                userClimbingSpotEntity.ClimbingSpotId = dto.ClimbingSpotId;
                userClimbingSpotEntity.UserId = dto.UserId;
                userClimbingSpotEntity.RoleId = dto.RoleId;
                WhoColumns.ModificationFiller(userClimbingSpotEntity, _userContext.GetUserId);
            }
            _dbContext.SaveChanges();
        }

        private UserClimbingSpotLinks GetUserClimbingSpot(int userId, int climbingSpotId, int roleId)
        {
            return _dbContext.UserClimbingSpotLinks.FirstOrDefault(x => (x.UserId == userId && x.ClimbingSpotId == null) || (x.UserId == userId && x.ClimbingSpotId == climbingSpotId && x.RoleId != roleId));
        }

        private void Validate(int userId, int climbingSpotId, int roleId, ClaimsPrincipal userPrincipal)
        {
            var user = _dbContext.User.FirstOrDefault(x => x.Id == userId);
            if (user is null)
                throw new NotFoundException($"User with ID: {userId} not exists.");

            var climbingSpot = _dbContext.ClimbingSpot.FirstOrDefault(x => x.Id == climbingSpotId);
            if (climbingSpot is null)
                throw new NotFoundException($"Climbing spot with ID: {climbingSpotId} not exists.");

            //checking if user is assigned to climbing spot. If not means that is not a manager or admin in that climbingSpot
            var userClaimId = userPrincipal.FindFirst(x => x.Type == ClaimTypes.NameIdentifier).Value;
            var userAssignedToClimbingSpot = _dbContext.UserClimbingSpotLinks.FirstOrDefault(x =>
                x.UserId == int.Parse(userClaimId) && x.ClimbingSpotId == climbingSpotId && (x.RoleId == 1 || x.RoleId == 2));
            if (userAssignedToClimbingSpot is null)
                throw new ForbidException(
                    $"User with ID: {userClaimId} is not assigned to climbing spot: {climbingSpotId}. You do not have enough rights.");

            var userClimbingSpotEntity =
                _dbContext.UserClimbingSpotLinks.FirstOrDefault(x => x.UserId == userId && x.ClimbingSpotId == climbingSpotId && x.RoleId == roleId);
            if (userClimbingSpotEntity is not null)
                throw new ForbidException(
                    $"User with ID: {userId} already assigned to climbing spot with ID: {climbingSpotId}.");


        }
    }
}
