using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using ClimbingAPI.Entities;
using ClimbingAPI.Entities.Boulder;
using ClimbingAPI.Exceptions;
using ClimbingAPI.Models.Boulder;
using ClimbingAPI.Services.Interfaces;
using ClimbingAPI.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ClimbingAPI.Services
{
    public class BoulderService: IBoulderService
    {
        private readonly ClimbingDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly ILogger<BoulderService> _logger;

        public BoulderService(ClimbingDbContext dbContext, IMapper mapper, ILogger<BoulderService> logger)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _logger = logger;
        }

        public int Create(CreateBoulderModelDto dto, int climbingSpotId)
        {
            _logger.LogInformation($"INFO for: CREATE action from BoulderService. Climbing Spot ID: \"{climbingSpotId}\".");

            GetClimbingSpotById(climbingSpotId);

            var boulderEntity = _mapper.Map<Boulder>(dto);
            boulderEntity.ClimbingSpotId = climbingSpotId;

            _dbContext.Boulder.Add(boulderEntity);
            
            WhoColumns.Fill(boulderEntity);
            _dbContext.SaveChanges();

            return boulderEntity.Id;
        }

        public BoulderDto Get(int boulderId, int climbingSpotId)
        {
            _logger.LogInformation($"INFO for: GET action from BoulderService. Boulder Id: \"{boulderId}\", Climbing Spot Id: {climbingSpotId}.");

            var climbingSpot = GetClimbingSpotById(climbingSpotId);

            var boulder = _dbContext.Boulder.FirstOrDefault(x => x.Id == boulderId);
            if (boulder is null || boulder.ClimbingSpotId != climbingSpotId)
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

        public void Delete(int boulderId, int climbingSpotId)
        {
            _logger.LogInformation($"INFO for: DELETE action from BoulderService. Boulder Id: {boulderId}.");

            GetClimbingSpotById(climbingSpotId);
            
            var boulder = _dbContext.Boulder.FirstOrDefault(x => x.Id == boulderId);
            if (boulder is null || boulder.ClimbingSpotId != climbingSpotId)
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

            var climbingSpot = GetClimbingSpotById(climbingSpotId);
            _dbContext.RemoveRange(climbingSpot.Boulder);

            _dbContext.SaveChanges();
        }

        private ClimbingSpot GetClimbingSpotById(int id)
        {
            var climbingSpot =_dbContext
                .ClimbingSpot
                .Include(x => x.Boulder)
                .FirstOrDefault(x => x.Id == id);
            if (climbingSpot is null)
            {
                _logger.LogError($"ERROR: action from Boulder Service GetClimbingSpotById(). Climbing Spot with ID: \"{id}\" not found.");
                throw new NotFoundException($"Climbing Spot with ID: \"{id}\" not found.");
            }

            return climbingSpot;
        }
    }
}
