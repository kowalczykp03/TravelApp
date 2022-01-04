using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entites;
using AutoMapper;
using TravelApp.DTOs;

namespace API.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<AppUser, MemberDto>();
            CreateMap<MemberDto, AppUser>();
            CreateMap<AppUser, MyAccountDto>();
            CreateMap<MyAccountDto, AppUser>();
            CreateMap<MemberDto, Task<AppUser>>();
            CreateMap<Task<AppUser>, MemberDto>();
            CreateMap<Task<AppUser>, AppUser>();
            CreateMap<Trip, TripDto>();
        }
    }
}