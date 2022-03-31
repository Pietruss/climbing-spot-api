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

            var id = _dbContext.ClimbingSpot.FirstOrDefault(x => x.Id == climbingSpotId);
            if (id is null)
                throw new NotFoundException($"Climbing spot with boulderId: {climbingSpotId} not found.");

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

            var climbingSpot = _dbContext.ClimbingSpot.FirstOrDefault(x => x.Id == climbingSpotId);
            if(climbingSpot is null)
                throw new NotFoundException($"Climbing spot with ID: {climbingSpotId} not found.");

            var boulder = _dbContext.Boulder.FirstOrDefault(x => x.Id == boulderId);
            if (boulder is null || boulder.ClimbingSpotId != climbingSpotId)
                throw new NotFoundException($"Boulder with ID: {boulderId} not found for Climbing spot with ID: {climbingSpotId}.");

            var boulderDto = _mapper.Map<BoulderDto>(boulder);

            return boulderDto;
        }

        public List<BoulderDto> GetAll(int climbingSpotId)
        {
            _logger.LogInformation($"INFO for: GETALL action from BoulderService. Climbing Spot Id: {climbingSpotId}.");

            var climbingSpot = _dbContext
                .ClimbingSpot
                .Include(x => x.Boulder)
                .FirstOrDefault(x => x.Id == climbingSpotId);
            if(climbingSpot is null)
                throw new NotFoundException($"Climbing spot with ID: {climbingSpotId} not found.");

            var boulderListDto = _mapper.Map<List<BoulderDto>>(climbingSpot.Boulder);

            return boulderListDto;
        }
    }
}
