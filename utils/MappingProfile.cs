using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using ParkingManagementAPI.Models;
using ParkingManagementAPI.Models.DTO;

namespace ParkingManagementAPI.utils
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {

            CreateMap<SystemUser, SystemUserDTO>();
            CreateMap<SystemUserDTO, SystemUser>();
        }
    }
}