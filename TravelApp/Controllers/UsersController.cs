using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entites;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TravelApp.DTOs;

namespace API.Controllers
{
    public class UsersController : BaseApiController
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly DataContext _context;

        private readonly ITokenService _tokenService;
        
        public UsersController(IUserRepository userRepository, IMapper mapper, DataContext context, ITokenService tokenService)
        {
            _mapper = mapper;
            _userRepository = userRepository;
            _tokenService = tokenService;

            _context = context;
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers()
        {
            var users = await _userRepository.GetUsersAsync();
            var userToReturn = _mapper.Map<IEnumerable<MemberDto>>(users);
            return Ok(userToReturn);
        }

        [HttpGet("{data}")]
        public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsersByData(string data)
        {
            var user = await _userRepository.GetUsersByDataAsync(data);
            var userToReturn = _mapper.Map<IEnumerable<MemberDto>>(user);
            return Ok(userToReturn);
        }
        [HttpGet("user/{hash}")]
        public async Task<ActionResult<MemberDto>> GetUser(string hash)
        {
            var error = new ErrorDto();
            var user = await _userRepository.GetUserByHash(hash);
            if(user != null)
            {
                return user;
            }
            error.Error = "Użytkownik nie istnieje";
            return BadRequest(error);
        }
        [HttpGet("user")]
        public async Task<ActionResult<MyAccountDto>> GetUserByToken()
        {
            var error = new ErrorDto();
            var userId = "";
            userId = getUserIdFromToken();
            var appUser = await _context.Users.FirstOrDefaultAsync(u => u.ID.ToString() == userId);
            var user = await _userRepository.GetUserById(userId);
            if(user != null && appUser != null)
            {
                var userToReturn = new MyAccountDto()
                {
                    member = user,
                    Email = appUser.Email
                };

                return userToReturn;
            }
            error.Error = "Użytkownik nie istnieje";
            return BadRequest(error);
        }
        [HttpPatch]
        public async Task<ActionResult<MemberDto>> EditUser(EditMemberDto memberDto)
        {
            var error = new ErrorDto();
            var userId = "";
            userId = getUserIdFromToken();
            var user = await _context.Users.FirstOrDefaultAsync(u => u.ID.ToString() == userId);
            if(user != null)
            {
                if(!String.IsNullOrEmpty(memberDto.Email))
                {
                    user.Email = memberDto.Email;
                }
                if(!String.IsNullOrEmpty(memberDto.Name))
                {
                    user.Name = memberDto.Name;
                }
                if(!String.IsNullOrEmpty(memberDto.Description))
                {
                    user.Description = memberDto.Description;
                }
                if(!String.IsNullOrEmpty(memberDto.Gender))
                {
                    user.Gender = memberDto.Gender;
                }

                var userUpdated = _context.Users.Update(user);
                await _context.SaveChangesAsync();
                var userToReturn = _mapper.Map<MemberDto>(user);
                return userToReturn;
            }
            error.Error = "Nieprawidłowy email";
            return Unauthorized(error);


        }
        private async Task<bool> UserExists(string email)
        {
            return await _context.Users.AnyAsync(x => x.Email == email.ToLower());
        }
        private string getUserIdFromToken()
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var userId = "";
            var jwt = tokenHandler.ReadJwtToken(accessToken);
            var claim = jwt.Claims.First(claims => claims.Type == "userId");
            if(claim != null)
            {
                userId = claim.Value;
            }
            return userId;
        }

    }
    
}