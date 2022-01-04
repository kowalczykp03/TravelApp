using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entites;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using API.Controllers;
using System.IdentityModel.Tokens.Jwt;
using TravelApp.DTOs;

namespace TravelApp.Controllers
{
    public class CarsController : BaseApiController
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        public CarsController(DataContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
        }


        [HttpPost("AddCar")]
        public async Task<ActionResult<Car>> AddCar(CarDto carDto)
        {
            var userId = "";
            userId = getUserIdFromToken();
            if(userId != null)
            {
                var car = new Car()
                {
                    OwnerId = Int32.Parse(userId),
                    Mark = carDto.Mark,
                    Model = carDto.Model,
                    ProductionYear = carDto.ProductionYear,
                    RegistrationNumber = carDto.RegistrationNumber,
                    NumberOfSeats = carDto.NumberOfSeats,
                    Color = carDto.Color
                };

                _context.Cars.AddRange(car);
                await _context.SaveChangesAsync();
                return Ok(car);
            }
            return BadRequest("User does not exist");
        } 

        [HttpPatch("EditCar")]
        public async Task<ActionResult<Car>> EditCar(EditCarDto carDto)
        {
            var userId = "";
            userId = getUserIdFromToken();
            if(userId != null)
            {
                var car = await _context.Cars.FirstOrDefaultAsync(c => c.Id == carDto.Id);
                if(car != null)
                {
                    if(!String.IsNullOrEmpty(carDto.Color))
                    {
                        car.Color = carDto.Color;
                    }
                    if(!String.IsNullOrEmpty(carDto.Mark))
                    {
                        car.Mark = carDto.Mark;
                    }
                     if(!String.IsNullOrEmpty(carDto.Model))
                    {
                        car.Model = carDto.Model;
                    }
                     if(!String.IsNullOrEmpty(carDto.RegistrationNumber))
                    {
                        car.RegistrationNumber = carDto.RegistrationNumber;
                    }
                    _context.Cars.Update(car);
                    await _context.SaveChangesAsync();
                    return Ok(car);
                }
            }
            return BadRequest("User does not exist");
        } 
        [HttpGet("{Id}")]
        public async Task<ActionResult<Car>> getCarById(int Id)
        {
            
            var car = await _context.Cars.FirstOrDefaultAsync(c => c.Id == Id);

            if(car != null)
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.ID == car.OwnerId);
                var memberDto = _mapper.Map<MemberDto>(user);
                var carInfo = new CarInfoDto()
                {
                    Id = car.Id,
                    Owner = memberDto,
                    Mark = car.Mark,
                    Model = car.Model,
                    ProductionYear = car.ProductionYear, 
                    RegistrationNumber = car.RegistrationNumber, 
                    NumberOfSeats = car.NumberOfSeats,
                    Color = car.Color
                };
                return car;
            }
            return BadRequest("Car does not exist");
        }

        [HttpGet]
        public async Task<ActionResult<List<Car>>> getCarByUser()
        {
            var userId = "";
            userId = getUserIdFromToken();
            var user = await _context.Users.FirstOrDefaultAsync(u => u.ID.ToString() == userId);
            var error = new ErrorDto();

            if(user != null)
            {
                var listOfCars = new List<CarInfoDto>();
                var memberDto = _mapper.Map<MemberDto>(user);
                var cars = await _context.Cars.Where(c => c.OwnerId == user.ID).ToListAsync();
                if(cars.Any())
                {
                    foreach(var car in cars)
                    {
                        var carInfo = new CarInfoDto()
                        {
                            Id = car.Id,
                            Owner = memberDto,
                            Mark = car.Mark,
                            Model = car.Model,
                            ProductionYear = car.ProductionYear,
                            RegistrationNumber = car.RegistrationNumber, 
                            NumberOfSeats = car.NumberOfSeats,
                            Color = car.Color
                        };
                        listOfCars.Add(carInfo);
                    }
                    return Ok(listOfCars);
                }
                error.Error = "Nie posiadasz samochodów";
                return Ok(error);
            }
            error.Error = "Użytkownik nie istnieje";
            return Unauthorized(error);
            
        }
            
        

        [HttpDelete("{Id}")]
        public async Task<ActionResult<Car>> deleteCarById(int Id)
        {
            var error = new ErrorDto();
            var userId = "";
            userId = getUserIdFromToken();
            var car = await _context.Cars.FirstOrDefaultAsync(c => c.Id == Id);
            if(car != null)
            {
                if(userId == car.OwnerId.ToString())
                {
                _context.Cars.Remove(car);
                await _context.SaveChangesAsync();
                return Ok();
                }
                error.Error = "Nie jesteś właścicielem pojazdu";
                return Unauthorized(error);
            }
            error.Error = "Pojazd nie istieje";
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