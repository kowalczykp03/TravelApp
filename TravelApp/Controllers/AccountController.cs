using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entites;
using API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TravelApp.DTOs;

namespace API.Controllers
{
    [Authorize]
    public class AccountController : BaseApiController
    {
        private readonly DataContext _context;
        private readonly ITokenService _tokenService;
        public AccountController(DataContext context, ITokenService tokenService)
        {
            _tokenService = tokenService;
            _context = context;
        }
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto regiserDto)
        {
            var error = new LoginErrorDto();
            if (await UserExists(regiserDto.Email))
            { 
                error.Type = "email";
                error.Error = "Email jest zajęty";
                return BadRequest(error);
            }
            Random rnd = new Random();
            var hash = 0;
            bool hashExists = true;
            do
            {

                hash = rnd.Next(10000, 10000000);
                var userByHash = _context.Users.Where(u => u.UserHash == hash).ToList();
                if (userByHash == null)
                {
                    hashExists = false;
                }

            } while (!hashExists);
            using var hmac = new HMACSHA512();

            var user = new AppUser
            {
                Email = regiserDto.Email.ToLower(),
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(regiserDto.Password)),
                PasswordSalt = hmac.Key,
                UserHash = hash,
                Created = DateTimeOffset.Now

            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return new UserDto
            {
                Email = user.Email,
                Token = _tokenService.CreateToken(user),
                UserHash = hash

            };

        }
         [AllowAnonymous]
         [HttpPost("login")]
         public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
         {
             var user = await _context.Users.SingleOrDefaultAsync(x => x.Email == loginDto.Email.ToLower());
             var error = new LoginErrorDto();
             if (user == null) 
             {
                 error.Type = "Email";
                 error.Error = "Nieprawidłowy adres email";
                 return Unauthorized(error);
             }
             using var hmac = new HMACSHA512(user.PasswordSalt);
             var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));
             for (int i = 0; i < computedHash.Length; i++)
             {
                 if (computedHash[i] != user.PasswordHash[i]) 
                 { 
                     error.Type = "Password";
                     error.Error = "Nieprawidłowe hasło";
                     return Unauthorized(error);
                 }
             }
             return new UserDto
             {
                 Email = user.Email,
                 Token = _tokenService.CreateToken(user),
                 UserHash = user.UserHash
             };
         }
         [HttpDelete]
         public async Task<ActionResult> DeleteAccount()
         {
             var error = new ErrorDto();
            var currentUser = HttpContext.User;
            var userId = "";
            if(currentUser.HasClaim(c => c.Type == "userId"))
            {
                userId = currentUser.Claims.FirstOrDefault(c => c.Type == "userId").Value.ToString(); 
            }
             var user = await _context.Users.Where(u => u.ID.ToString() == userId).FirstOrDefaultAsync();
             if (user != null)
             {
                 _context.Users.Remove(user);
                 await _context.SaveChangesAsync();
                 return Ok();
             }
             error.Error = "Użytkownik nie istnieje";
             return BadRequest(error);
         }
        private async Task<bool> UserExists(string email)
        {
            return await _context.Users.AnyAsync(x => x.Email == email.ToLower());
        }
    }
}