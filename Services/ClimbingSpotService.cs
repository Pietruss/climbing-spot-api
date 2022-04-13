using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using ClimbingAPI.Authorization;
using ClimbingAPI.Entities;
using ClimbingAPI.Exceptions;
using ClimbingAPI.Models.ClimbingSpot;
using ClimbingAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

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

            var climbingSpot = _mapper.Map<ClimbingSpot>(dto);
            climbingSpot.CreatedById = _userContext.GetUserId;

            _dbContext.ClimbingSpot.Add(climbingSpot);
            _dbContext.SaveChanges();

            AssignClimbingSpotToUser(_userContext.GetUserId, climbingSpot.Id);

            _dbContext.SaveChanges();

            return climbingSpot.Id;
        }

        private void AssignClimbingSpotToUser(int? userId, int climbingSpotId)
        { 
            var climbingSpotUser = _dbContext.UserClimbingSpot.FirstOrDefault(x => x.UserId == userId && x.ClimbingSpotId == null);
            if (climbingSpotUser is null && userId != null)
            {
                    var userClimbingSpotEntity = new UserClimbingSpot()
                    {
                        ClimbingSpotId = climbingSpotId,
                        UserId = (int) userId,
                        RoleId = int.Parse(_userContext.User.FindFirst(c => c.Type ==  "RoleId").Value)
                    };
                    _dbContext.UserClimbingSpot.Add(userClimbingSpotEntity);
            }
            else
            {
                climbingSpotUser.ClimbingSpotId = climbingSpotId;
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
            _dbContext.SaveChanges();
        }
    }
}
