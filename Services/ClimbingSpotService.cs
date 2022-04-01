using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using ClimbingAPI.Entities;
using ClimbingAPI.Exceptions;
using ClimbingAPI.Models.ClimbingSpot;
using ClimbingAPI.Services.Interfaces;
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

        public ClimbingSpotService(ClimbingDbContext dbContext, IMapper mapper, ILogger<ClimbingSpotService>  logger)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _logger = logger;
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

            var newModel = _mapper.Map<ClimbingSpot>(dto);

            _dbContext.ClimbingSpot.Add(newModel);

            _dbContext.SaveChanges();

            return newModel.Id;
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

            _dbContext.ClimbingSpot.Remove(climbingSpot);
            _dbContext.SaveChanges();
        }

        public void Update(UpdateClimbingSpotDto dto, int id)
        {
            _logger.LogError($"INFO for: UPDATE action from ClimbingSpotService. ID: \"{id}\".");

            var climbingSpotToUpdate = _dbContext.ClimbingSpot.FirstOrDefault(x => x.Id == id);

            if (climbingSpotToUpdate is null)
                throw new NotFoundException($"Restaurant with ID: {id} not found.");

            climbingSpotToUpdate.Description = dto.Description;
            climbingSpotToUpdate.Name = dto.Name;
            climbingSpotToUpdate.ContactEmail = dto.ContactEmail;
            climbingSpotToUpdate.ContactNumber = dto.ContactNumber;

            _dbContext.Update(climbingSpotToUpdate);
            _dbContext.SaveChanges();
        }
    }
}
