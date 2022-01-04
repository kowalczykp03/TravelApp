using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entites;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TravelApp.DTOs;

namespace API.Data
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        public UserRepository(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<AppUser>> GetUsersAsync()
        {
            return await _context.Users
            .ToListAsync();
        }

        public async Task<IEnumerable<AppUser>> GetUsersByDataAsync(string data)
        {
            return await _context.Users
            .Where(u => u.Name.Contains(data) || data.Contains(u.Name) || data.Equals(u.UserHash.ToString())).ToListAsync();
        }
        public async Task<MemberDto> GetUserByHash(string hash)
        {
            var user = await _context.Users.Where(u => u.UserHash.ToString() == hash).FirstOrDefaultAsync();
            MemberDto memberDto = new MemberDto();
            if(user != null)
            {   List<Opinion> opinions = new List<Opinion>();
                var opinionsList = _context.Opinions.Where(o => o.UserId == user.ID).ToList();
                foreach(var o in opinionsList)
                {
                    opinions.Add(o);
                }
                memberDto = new MemberDto()
                {
                    Id = user.ID,
                    UserHash = user.UserHash,
                    Opinions = opinions,
                    Name = user.Name,
                    Gender = user.Gender,
                    Description = user.Description,
                    
                };
                
            }
            return memberDto;
        }
        public async Task<MemberDto> GetUserById(string hash)
        {
            var user = await _context.Users.Where(u => u.ID.ToString() == hash).FirstOrDefaultAsync();
            MemberDto memberDto = new MemberDto();
            if(user != null)
            {   List<Opinion> opinions = new List<Opinion>();
                var opinionsList = _context.Opinions.Where(o => o.UserId == user.ID).ToList();
                foreach(var o in opinionsList)
                {
                    opinions.Add(o);
                }
                memberDto = new MemberDto()
                {
                    Id = user.ID,
                    UserHash = user.UserHash,
                    Opinions = opinions,
                    Name = user.Name,
                    Gender = user.Gender,
                    Description = user.Description,
                    
                };
                
            }
            return memberDto;
        }
        public async Task<ActionResult<MemberDto>> EditUser(AppUser user, EditMemberDto memberDto)
        {

                user.Email = memberDto.Email;
                if(!String.IsNullOrEmpty(user.Name))
                {
                    user.Name = memberDto.Name;
                }
                if(!String.IsNullOrEmpty(user.Description))
                {
                    user.Description = memberDto.Description;
                }

                var userUpdated =  _context.Users.Update(user);
                var userToReturn = _mapper.Map<MemberDto>(user);
                return userToReturn;
        }

        public async Task<bool> SaveAllAsync()
        {
            return await _context.SaveChangesAsync() > 0; 
        }

        public void Update(AppUser user)
        {
            _context.Entry(user).State = EntityState.Modified;
        }

    }
}