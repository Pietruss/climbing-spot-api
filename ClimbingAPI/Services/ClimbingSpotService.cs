using AutoMapper;
using ClimbingAPI.Authorization;
using ClimbingAPI.Authorization.AuthorizationEntity;
using ClimbingAPI.Entities;
using ClimbingAPI.Exceptions;
using ClimbingAPI.Models.ClimbingSpot;
using ClimbingAPI.Models.Role;
using ClimbingAPI.Models.UserClimbingSpot;
using ClimbingAPI.Services.Interfaces;
using ClimbingAPI.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

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
                .AsNoTracking()
                .Include(x => x.Address)
                .Include(x => x.Boulder)
                .FirstOrDefault(x => x.Id == id);

            if(climbingSpot is null)
                throw new NotFoundException($"ClimbingSpot with ID: {id} not found.");

            var climbingSpotDto = _mapper.Map<ClimbingSpotDto>(climbingSpot);

            return climbingSpotDto;
        }

        public int Create(CreateClimbingSpotDto dto)
        {
            _logger.LogInformation("INFO for: CREATE action from ClimbingSpotService.");

            var authorizationResult = Authorize(ResourceOperation.Create, new ClimbingSpotAuthorization() { });
            if (!authorizationResult.Succeeded)
            {
                _logger.LogError($"ERROR for: CREATE action from ClimbingSpotService. Authorization failed.");
                throw new UnAuthorizeException(Literals.Literals.AuthorizationFailed.GetDescription());
            }

            var climbingSpot = _mapper.Map<ClimbingSpot>(dto);
            climbingSpot.CreatedById = _userContext.GetUserId;

            _dbContext.ClimbingSpot.Add(climbingSpot);

            WhoColumns.CreationFiller(climbingSpot, _userContext.GetUserId, DateTime.Now);
            WhoColumns.CreationFiller(climbingSpot.Address, _userContext.GetUserId, DateTime.Now);

            _dbContext.SaveChanges();

            AssignClimbingSpotToUser(_userContext.GetUserId, climbingSpot.Id);

            _dbContext.SaveChanges();

            return climbingSpot.Id;
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
                    WhoColumns.CreationFiller(userClimbingSpotEntity, _userContext.GetUserId, DateTime.Now);
                    _dbContext.UserClimbingSpotLinks.Add(userClimbingSpotEntity);
            }
            else
            {
                if (climbingSpotUser != null)
                {
                    climbingSpotUser.ClimbingSpotId = climbingSpotId;
                    WhoColumns.CreationFiller(climbingSpotUser, _userContext.GetUserId, DateTime.Now);
                }
            }
        }

        public void Delete(int climbingSpotId)
        {
            _logger.LogInformation($"INFO for: DELETE action from ClimbingSpotService. ID: \"{climbingSpotId}\".");

            var climbingSpot = _dbContext.ClimbingSpot.FirstOrDefault(x => x.Id == climbingSpotId);
            if (climbingSpot is null)
            {
                //I do not want to show user that mentioned record is not present in the database 
                _logger.LogError($"ERROR for: DELETE action from ClimbingSpotService. Authorization failed.");
                throw new UnAuthorizeException(Literals.Literals.AuthorizationFailed.GetDescription());
            }

            var authorizationResult = Authorize(ResourceOperation.Delete, new ClimbingSpotAuthorization() { CreatedById = climbingSpot.CreatedById });
            if (!authorizationResult.Succeeded)
            {
                _logger.LogError($"ERROR for: DELETE action from ClimbingSpotService. Authorization failed.");
                throw new UnAuthorizeException(Literals.Literals.AuthorizationFailed.GetDescription());
            }

            RemoveAllUserExceptFromOwner(climbingSpotId);

            _dbContext.ClimbingSpot.Remove(climbingSpot);
            _dbContext.SaveChanges();
        }

        private void RemoveAllUserExceptFromOwner(int climbingSpotId)
        {
            var userClimbingSpots = _dbContext.UserClimbingSpotLinks.Where(x => x.ClimbingSpotId == climbingSpotId);
            foreach (var user in userClimbingSpots)
            {
                if (user.UserId == _userContext.GetUserId)
                    user.ClimbingSpotId = null;
                else
                    _dbContext.UserClimbingSpotLinks.Remove(user);
            }
        }

        public void Update(UpdateClimbingSpotDto dto, int climbingSpotId)
        {
            _logger.LogError($"INFO for: UPDATE action from ClimbingSpotService. ID: \"{climbingSpotId}\".");

            
            var climbingSpot = _dbContext.ClimbingSpot.FirstOrDefault(x => x.Id == climbingSpotId);
            if (climbingSpot is null)
            {
                //I do not want to show user that mentioned record is not present in the database 
                _logger.LogError($"ERROR for: DELETE action from ClimbingSpotService. Authorization failed.");
                throw new UnAuthorizeException(Literals.Literals.AuthorizationFailed.GetDescription());
            }

            var authorizationResult = Authorize(ResourceOperation.Update, new ClimbingSpotAuthorization() { CreatedById = climbingSpot.CreatedById });
            if (!authorizationResult.Succeeded)
            {
                _logger.LogError($"ERROR for: DELETE action from ClimbingSpotService. Authorization failed.");
                throw new UnAuthorizeException(Literals.Literals.AuthorizationFailed.GetDescription());
            }

            climbingSpot.Description = dto.Description;
            climbingSpot.Name = dto.Name;
            climbingSpot.ContactEmail = dto.ContactEmail;
            climbingSpot.ContactNumber = dto.ContactNumber;

            _dbContext.Update(climbingSpot);

            WhoColumns.ModificationFiller(climbingSpot, _userContext.GetUserId, DateTime.Now);

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

                WhoColumns.CreationFiller(userClimbingSpot, _userContext.GetUserId, DateTime.Now);

                _dbContext.UserClimbingSpotLinks.Add(userClimbingSpot);
            }
            else
            {
                userClimbingSpotEntity.ClimbingSpotId = dto.ClimbingSpotId;
                userClimbingSpotEntity.UserId = dto.UserId;
                userClimbingSpotEntity.RoleId = dto.RoleId;
                WhoColumns.ModificationFiller(userClimbingSpotEntity, _userContext.GetUserId, DateTime.Now);
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
                throw new UnAuthorizeException(Literals.Literals.AuthorizationFailed.GetDescription());

            var climbingSpot = _dbContext.ClimbingSpot.FirstOrDefault(x => x.Id == climbingSpotId);
            if (climbingSpot is null)
                throw new UnAuthorizeException(Literals.Literals.AuthorizationFailed.GetDescription());

            //checking if user is assigned to climbing spot. If not means that is not a manager or admin in that climbingSpot
            var userClaimId = userPrincipal.FindFirst(x => x.Type == ClaimTypes.NameIdentifier).Value;
            var userAssignedToClimbingSpot = _dbContext.UserClimbingSpotLinks.FirstOrDefault(x =>
                x.UserId == int.Parse(userClaimId) && x.ClimbingSpotId == climbingSpotId && (x.RoleId == (int)Roles.Admin || x.RoleId == (int)Roles.Manager));
            if (userAssignedToClimbingSpot is null)
                throw new UnAuthorizeException(Literals.Literals.AuthorizationFailed.GetDescription());

            var userClimbingSpotEntity =
                _dbContext.UserClimbingSpotLinks.FirstOrDefault(x => x.UserId == userId && x.ClimbingSpotId == climbingSpotId && x.RoleId == roleId);
            if (userClimbingSpotEntity is not null)
                throw new BadRequestException(
                    $"User with ID: {userId} already assigned to climbing spot with ID: {climbingSpotId}.");


        }

        private AuthorizationResult Authorize(ResourceOperation resourceOperation, ClimbingSpotAuthorization climbingSpotAuthorization)
        {
            return _authorizationService.AuthorizeAsync(_userContext.User, climbingSpotAuthorization,
                new ResourceOperationRequirement(resourceOperation)).Result;
        }

        public async Task<List<ClimbingSpot>> GetClimbingSpotAssignedToUser(int userId)
        {
            return await _dbContext
                .ClimbingSpot
                .AsNoTracking()
                .Where(x => x.CreatedById == userId).ToListAsync();
        }
    }
}
