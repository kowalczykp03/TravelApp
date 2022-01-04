using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entites;
using Microsoft.AspNetCore.Mvc;
using TravelApp.DTOs;

namespace API.Interfaces
{
    public interface IUserRepository
    {
        void Update(AppUser user);
        Task<bool> SaveAllAsync();
        Task<IEnumerable<AppUser>> GetUsersAsync();
        Task<IEnumerable<AppUser>> GetUsersByDataAsync(string name);
        Task<MemberDto> GetUserByHash(string hash);
        Task<ActionResult<MemberDto>> EditUser(AppUser user, EditMemberDto memberDto);
        Task<MemberDto> GetUserById(string hash);
       
    }
}