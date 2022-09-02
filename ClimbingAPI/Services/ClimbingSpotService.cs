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
        public async Task<IEnumerable<ClimbingSpotDto>> GetAll()
        {
            _logger.LogInformation("INFO for: GETALL action from ClimbingSpotService.");

            var climbingSpots = await _dbContext
                .ClimbingSpot
                .Include(x => x.Address)
                .Include(x => x.Boulder)
                .ToListAsync();

            var climbingSpotsDto = _mapper.Map<List<ClimbingSpotDto>>(climbingSpots);

            return climbingSpotsDto;
        }

        public ClimbingSpot GetClimbingSpotWithAddressAndBouldersById(int climbingSpotId)
        {
            var climbingSpot = _dbContext
                .ClimbingSpot
                .AsNoTracking()
                .Include(x => x.Address)
                .Include(x => x.Boulder)
                .FirstOrDefault(x => x.Id == climbingSpotId);

            ValidateClimbingSpotExistance(climbingSpot, climbingSpotId);

            return climbingSpot;
        }

        public async Task ValidateClimbingSpotById(int climbingSpotId)
        {
            var climbingSpot = await _dbContext
                .ClimbingSpot
                .AsNoTracking()
                .Where(x => x.Id == climbingSpotId)
                .Select(x => new ClimbingSpot() {Id = x.Id })
                .FirstOrDefaultAsync();

            ValidateClimbingSpotExistance(climbingSpot, climbingSpotId);
        }

        private void ValidateClimbingSpotExistance(ClimbingSpot climbingSpot, int climbingSpotId)
        {
            if (climbingSpot is null)
            {
                _logger.LogError($"ERROR: action from Climbing Service GetClimbingSpotById(). Climbing Spot with ID: \"{climbingSpotId}\" not found.");
                throw new NotFoundException($"Climbing Spot with ID: \"{climbingSpotId}\" not found.");
            }
        }

        public ClimbingSpotDto Get(int climbingSpotId)
        {
            _logger.LogInformation($"INFO for: GET action from ClimbingSpotService. ID: \"{climbingSpotId}\".");

            var climbingSpot = GetClimbingSpotWithAddressAndBouldersById(climbingSpotId);
            if (climbingSpot is null)
                throw new NotFoundException($"ClimbingSpot with ID: {climbingSpotId} not found.");

            var climbingSpotDto = _mapper.Map<ClimbingSpotDto>(climbingSpot);

            return climbingSpotDto;
        }

        public int Create(CreateClimbingSpotDto dto)
        {
            _logger.LogInformation("INFO for: CREATE action from ClimbingSpotService.");

            VerifyUserData(0, ResourceOperation.Create, Literals.Literals.CreateClimbingSpotAction.GetDescription(), out var climbingSpot);

            climbingSpot = _mapper.Map<ClimbingSpot>(dto);
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

            VerifyUserData(climbingSpotId, ResourceOperation.Delete, Literals.Literals.DeleteClimbingSpotAction.GetDescription(), out var climbingSpot);

            RemoveAllUserExceptFromOwner(climbingSpotId);

            _dbContext.ClimbingSpot.Remove(climbingSpot);
            _dbContext.SaveChanges();
        }

        private void VerifyUserData(int climbingSpotId, ResourceOperation resourceOperation, string operation, out ClimbingSpot climbingSpot)
        {
            climbingSpot = null;
            if(resourceOperation != ResourceOperation.Create)
            {
                climbingSpot = _dbContext.ClimbingSpot.FirstOrDefault(x => x.Id == climbingSpotId);
                if (climbingSpot is null)
                {
                    _logger.LogError($"ERROR for: {operation} action from ClimbingSpotService. Authorization failed.");
                    throw new UnAuthorizeException(Literals.Literals.AuthorizationFailed.GetDescription());
                }
            }

            var authorizationResult = Authorize(resourceOperation, new ClimbingSpotAuthorization() { CreatedById = climbingSpot != null ? climbingSpot.CreatedById : null });
            if (!authorizationResult.Succeeded)
            {
                _logger.LogError($"ERROR for: {operation} action from ClimbingSpotService. Authorization failed.");
                throw new UnAuthorizeException(Literals.Literals.AuthorizationFailed.GetDescription());
            }
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

            VerifyUserData(climbingSpotId, ResourceOperation.Update, Literals.Literals.UpdateClimbingSpotAction.GetDescription(), out var climbingSpot);

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

        public async Task<List<int>> GetClimbingSpotAssignedToUser(int userId)
        {
            return await _dbContext
                .ClimbingSpot
                .AsNoTracking()
                .Where(x => x.CreatedById == userId)
                .Select(x => x.Id)
                .ToListAsync();
        }
    }
}
