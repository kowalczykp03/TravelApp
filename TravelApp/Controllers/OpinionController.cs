using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entites;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using TravelApp.DTOs;

namespace API.Controllers
{
    public class OpinionController : BaseApiController
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        public OpinionController(DataContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
        }

        [HttpPost("addOpinion")]
        public async Task<ActionResult<Opinion>> AddOpinion(OpinionDto opinionsDto)
        {
            var userId = "";
            var error = new ErrorDto();
            userId = getUserIdFromToken();

            DateTimeOffset date =DateTimeOffset.Now;
            if(!String.IsNullOrEmpty(userId))
            {
                var sender = _context.Users.FirstOrDefault(u => u.ID.ToString() == userId);
                if(sender == null)
                {
                    error.Error = "Nie utworzyłeś konta";
                    return Unauthorized(error);
                }
            }
            else
            {
                error.Error = "Nie jesteś zalogowany";
                return Unauthorized(error);
            }
            var user = _context.Users.SingleOrDefault(u => u.ID == opinionsDto.UserId);
            if(user != null)
            {
                var chechOpinion = _context.Opinions.Where(o => o.SenderId.ToString() == userId && opinionsDto.UserId == o.UserId);
                if(!chechOpinion.Any())
                {
                    var opinion = new Opinion
                    {
                        UserId = opinionsDto.UserId,
                        SenderId = Int32.Parse(userId),
                        OpinionValue = opinionsDto.OpinionValue,
                        OpinionDescription = opinionsDto.OpinionDescription,
                        Date = date
                    };
                    _context.Opinions.Add(opinion);
                    await _context.SaveChangesAsync();
                    return Ok(opinion);
                }
                error.Error = "Już dodałes opinię temu użytkownikowi";
                return BadRequest(error);

            }
            error.Error = "Użytkownik nie istnieje";
            return BadRequest(error);
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