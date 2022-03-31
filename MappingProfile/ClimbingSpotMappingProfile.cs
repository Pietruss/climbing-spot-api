using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using ClimbingAPI.Entities;
using ClimbingAPI.Entities.Address;
using ClimbingAPI.Entities.Boulder;
using ClimbingAPI.Models.Boulder;
using ClimbingAPI.Models.ClimbingSpot;

namespace ClimbingAPI.MappingProfile
{
    public class ClimbingSpotMappingProfile: Profile
    {
        public ClimbingSpotMappingProfile()
        {
            CreateMap<ClimbingSpot, ClimbingSpotDto>()
                .ForMember(x => x.City, c => c.MapFrom(s => s.Address.City))
                .ForMember(x => x.Street, c => c.MapFrom(s => s.Address.Street))
                .ForMember(x => x.PostalCode, c => c.MapFrom(s => s.Address.PostalCode))
                .ForMember(x => x.BoulderList, c => c.MapFrom(s => s.Boulder));

            CreateMap<CreateClimbingSpotDto, ClimbingSpot>()
                .ForMember(x => x.Address, c => c.MapFrom(dto => new Address()
                {
                    City = dto.City,
                    Street = dto.Street,
                    PostalCode = dto.PostalCode
                }));

            CreateMap<Boulder, BoulderDto>();

            CreateMap<CreateBoulderModelDto, Boulder>();
        }
    }
}
