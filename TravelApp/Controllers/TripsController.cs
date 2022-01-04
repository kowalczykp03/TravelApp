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
using Microsoft.EntityFrameworkCore;
using TravelApp.DTOs;
using TravelApp.Entities;

namespace API.Controllers
{
    public class TripsController : BaseApiController
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        public TripsController(DataContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
        }


        [HttpGet]
        public async Task<IEnumerable<TripInfoDto>> getTrips()
        {
           List<TripInfoDto> chosenTrips = new List<TripInfoDto>();
           DateTimeOffset date = DateTimeOffset.Now;

           var trips = await _context.Trips.Where(t =>  t.StartTime > date).ToListAsync();

           foreach(var t in trips)
           {    
               List<AppUser> users = new List<AppUser>();
               
               var passengers = await _context.Passenger.Where(s => s.TripId == t.Id).ToListAsync();
               foreach(var p in passengers)
               {
                    var user = await _context.Users.Where(u => p.UserId == u.ID).FirstOrDefaultAsync();
                    if(user != null)
                    {
                        users.Add(user);
                    }
               }
               var owner = await _context.Users.Where(o => o.ID == t.CreatorId).FirstOrDefaultAsync();
               var member = _mapper.Map<MemberDto>(owner);
               var passengerMembers = _mapper.Map<List<MemberDto>>(users);
               var car = await _context.Cars.SingleOrDefaultAsync(x => x.Id == t.CarId);
               if(car != null)
               {
                    var carDto = new CarInfoDto()
                    {
                            Id = car.Id,
                            Owner = member,
                            Mark = car.Mark,
                            Model = car.Model,
                            ProductionYear = car.ProductionYear,
                            RegistrationNumber = car.RegistrationNumber, 
                            NumberOfSeats = car.NumberOfSeats,
                            Color = car.Color
                    };

                    TripInfoDto trip = new TripInfoDto
                    {
                        Id = t.Id,
                        Creator = member,
                        Passenger = passengerMembers,
                        Price = t.Price,
                        StartTime = t.StartTime,
                        StartFrom = t.StartFrom,
                        NumberOfSeats = t.NumberOfSeats,
                        Car = carDto,
                        EndIn = t.EndIn
                    };

                    chosenTrips.Add(trip);
               }
           }
           return chosenTrips;
        }


        [HttpPost("addTrip")]
        public async Task<ActionResult<Trip>> addTrip(AddTripDto tripDto)
        {
            var userId = "";
            var error = new ErrorDto();
            userId = getUserIdFromToken();
            if(!String.IsNullOrEmpty(userId))
            {
                var user = await _context.Users.SingleOrDefaultAsync(x => x.ID.ToString() == userId);
                var car = await _context.Cars.SingleOrDefaultAsync(x => x.Id == tripDto.CarID);
                if(user != null)
                {
                    if( car != null)
                    {
                        var trip = new Trip
                        {
                            CreatorId = user.ID,
                            Price = tripDto.Price,
                            NumberOfSeats = tripDto.NumberOfSeats,
                            CarId = car.Id,
                            StartTime = tripDto.StartTime,
                            StartFrom = tripDto.StartFrom,
                            EndIn = tripDto.EndIn
                        };
                    
                        _context.Trips.Add(trip);
                        await _context.SaveChangesAsync();
                        return Ok(trip);
                    }
                    error.Error = "Nie wybrałeś samochodu";
                    return BadRequest(error);
                }

            }
            error.Error = "Użytkownik nie istieje";
            return BadRequest(error);
        }


        [HttpDelete("deleteTrip/{tripId}")]
        public async Task<ActionResult<Trip>> deleteTrip(int tripId)
        {
            var error = new ErrorDto();
            var trip = await _context.Trips.SingleOrDefaultAsync(x => x.Id == tripId);
            if(trip != null)
            {
                _context.Trips.Remove(trip);
                await _context.SaveChangesAsync();
                return Ok();
            }
            error.Error = "Przejazd nie istnieje"; 
            return BadRequest(error);

        }


       [HttpPost("addPassenger")]
       public async Task<ActionResult<Trip>> addPassenger(PassengerDto passengerDto)
       {
            var error = new ErrorDto();
            var userId = "";
            userId = getUserIdFromToken();
            var user = await _context.Users.SingleOrDefaultAsync(x => x.ID.ToString() == userId);
            var trip = await _context.Trips.SingleOrDefaultAsync(x => x.Id == passengerDto.TripId);
            var tripCreator = await _context.Trips.SingleOrDefaultAsync(x => x.Id == passengerDto.TripId && x.CreatorId.ToString() == userId);
            var passengers = await _context.Passenger.Where(p => p.TripId == trip.Id).ToListAsync();
            if(passengers.Count < trip.NumberOfSeats)
            { 
                if(user != null && trip != null && tripCreator == null)
                {
                    var passenger = new Passenger(user.ID, passengerDto.TripId);
                    _context.Passenger.Add(passenger);
                    await _context.SaveChangesAsync();
                    return trip; 
                }
                error.Error = "Nie udało się dołączyć do przejazdu";
                return BadRequest(error);
            }
            error.Error = "Brak wolnych miejsc";
           return BadRequest(error);
       }


       [HttpDelete("deletePassenger")]
        public async Task<ActionResult<Trip>> deletePassenger(PassengerDto passengerDto)
       {
           var user = await _context.Users.SingleOrDefaultAsync(x => x.ID == passengerDto.UserId);
           var trip = await _context.Trips.SingleOrDefaultAsync(x => x.Id == passengerDto.TripId);
           var error = new ErrorDto();
           if(user != null && trip != null)
           {
               var passenger = await _context.Passenger.SingleOrDefaultAsync(x => x.UserId == passengerDto.UserId && x.TripId == passengerDto.TripId);
               if(passenger != null)
               {
                    _context.Passenger.Remove(passenger);
                    await _context.SaveChangesAsync();
                    return Ok(); 
               }
           }
           error.Error = "Nie udało się usunąć pasażera";
           return BadRequest(error);
       }

       [HttpGet("{tripId}")]
       public async Task<ActionResult<TripInfoDto>> GetTripById(int tripId)
       {
           var trip = await _context.Trips.Where(t => t.Id == tripId).FirstOrDefaultAsync();
           var error = new ErrorDto();
           if(trip != null)
           {
               List<AppUser> users = new List<AppUser>();
               var passenger = _context.Passenger.Where(p => p.TripId == trip.Id).ToList();
               var owner = await _context.Users.Where(u => u.ID == trip.CreatorId).FirstOrDefaultAsync();
               foreach(var p in passenger)
               {
                    var user = await _context.Users.Where(u => p.UserId == u.ID).FirstOrDefaultAsync();
                    if(user != null)
                    {
                        users.Add(user);
                    }
               }
               var usersToReturn = _mapper.Map<List<MemberDto>>(users);
               var userToReturn = _mapper.Map<MemberDto>(owner);
               var car = await _context.Cars.SingleOrDefaultAsync(x => x.Id == trip.CarId);
               if(car != null)
               {
                var carDto = new CarInfoDto()
                {
                        Id = car.Id,
                        Owner = userToReturn,
                        Mark = car.Mark,
                        Model = car.Model,
                        ProductionYear = car.ProductionYear,
                        RegistrationNumber = car.RegistrationNumber, 
                        NumberOfSeats = car.NumberOfSeats,
                        Color = car.Color
                };
                var tripInfo = new TripInfoDto()
                {
                    Id = tripId,
                    Creator = userToReturn,
                    Passenger = usersToReturn,
                    Price = trip.Price,
                    StartTime = trip.StartTime,
                    StartFrom = trip.StartFrom,
                    NumberOfSeats = trip.NumberOfSeats,
                    Car = carDto,
                    EndIn = trip.EndIn

                };
                return tripInfo;
               }
           }   
           error.Error = "Przejazd nie istieje"; 
           return BadRequest(error);
       }
       
       [HttpPost("getTrips")]
       public async Task<ActionResult<List<TripInfoDto>>> getTripsByData(TripsSearchDto tripsSearchDto)
       {
           List<TripInfoDto> chosenTrips = new List<TripInfoDto>();
           DateTimeOffset date = DateTimeOffset.Now;
           if(tripsSearchDto.StartTime != null)
           {
               date = tripsSearchDto.StartTime;
           }
           var trips = await _context.Trips.Where(t => t.StartFrom.ToLower().Contains(tripsSearchDto.StartFrom.ToLower()) && t.EndIn.ToLower().Contains(tripsSearchDto.EndIn.ToLower()) && t.StartTime > tripsSearchDto.StartTime).ToListAsync();

           foreach(var t in trips)
           {    
               List<AppUser> users = new List<AppUser>();
               var passengers = await _context.Passenger.Where(s => s.TripId == t.Id).ToListAsync();
               foreach(var p in passengers)
               {
                    var user = await _context.Users.Where(u => p.UserId == u.ID).FirstOrDefaultAsync();
                    if(user != null)
                    {
                        users.Add(user);
                    }
               }
               var owner = await _context.Users.Where(o => o.ID == t.CreatorId).FirstOrDefaultAsync();
               var member = _mapper.Map<MemberDto>(owner);
               var passengerMembers = _mapper.Map<List<MemberDto>>(users);
               var car = await _context.Cars.SingleOrDefaultAsync(x => x.Id == t.CarId);
               if(car != null)
               {
                    var carDto = new CarInfoDto()
                    {
                            Id = car.Id,
                            Owner = member,
                            Mark = car.Mark,
                            Model = car.Model,
                            RegistrationNumber = car.RegistrationNumber, 
                            NumberOfSeats = car.NumberOfSeats,
                            Color = car.Color
                    };

                    TripInfoDto trip = new TripInfoDto
                    {
                        Id = t.Id,
                        Creator = member,
                        Passenger = passengerMembers,
                        Price = t.Price,
                        StartTime = t.StartTime,
                        StartFrom = t.StartFrom,
                        NumberOfSeats = t.NumberOfSeats,
                        Car = carDto,
                        EndIn = t.EndIn
                    };

                    chosenTrips.Add(trip);
               }
           }
           return chosenTrips;
       }
        [HttpGet("Messages/{id}")]
        public async Task<ActionResult<List<ChatDto>>> getChat(int id)
        {
            var error = new ErrorDto();
            var userId = "";
            userId = getUserIdFromToken();
            var passenger = await _context.Passenger.Where(p => p.TripId == id && p.UserId.ToString() == userId).FirstOrDefaultAsync();
            var driver = await _context.Trips.Where(o => o.CreatorId.ToString() == userId && o.Id == id).FirstOrDefaultAsync();
            if(passenger != null || driver != null)
            {
                var chat = new List<ChatDto>();
                var messages = await _context.Messages.Where(m => m.TripId == id).OrderByDescending(d => d.AddedDate).ToListAsync();

                foreach(var m in messages)
                {
                    var user = await _context.Users.Where(u => u.ID == m.OwnerId).FirstOrDefaultAsync();
                    var message = new ChatDto
                    {
                        Owner = user,
                        AddedDate = m.AddedDate,
                        TextMessage = m.TextMessage

                    };
                    chat.Add(message);
                }
                return Ok(chat);
            }
            error.Error = "Nie jesteś pasażerem przejazdu";
           return Unauthorized(error);
        }
        [HttpPost("Messages/AddMessage")]
        public async Task<ActionResult<Message>> addMessage(MessageDto messageDto)
        {
            var userId = "";
            userId = getUserIdFromToken();
            var error = new ErrorDto();
            var passenger = await _context.Passenger.Where(p => p.TripId == messageDto.TripId && p.UserId.ToString() == userId).FirstOrDefaultAsync();
            var driver = await _context.Trips.Where(o => o.CreatorId.ToString() == userId && o.Id == messageDto.TripId).FirstOrDefaultAsync();
            if(passenger != null || driver != null)
            {
                var message = new Message()
                {
                    OwnerId = Int32.Parse(userId),
                    TripId = messageDto.TripId,
                    TextMessage = messageDto.TextMessage,
                    AddedDate = DateTimeOffset.Now
                };
                _context.Messages.Add(message);
                await _context.SaveChangesAsync();
                return Ok();
            }
            error.Error = "Nie jesteś pasażerem przejazdu";
            return Unauthorized(error);
        }
        [HttpGet("myTrips")]
        public async Task<ActionResult<List<TripInfoDto>>> getMyTrips()
        {
            var userId = "";
            userId = getUserIdFromToken();
            var error = new ErrorDto();
            var chosenTrips = new List<TripInfoDto>();
            if(!String.IsNullOrEmpty(userId))
            {
                var trips = await _context.Trips.Where(t => t.CreatorId.ToString() == userId).ToListAsync();
                var tripsId = await _context.Passenger.Where(p => p.UserId.ToString() == userId).ToListAsync();
                foreach(var p in tripsId)
                {
                    var trip = await _context.Trips.FirstOrDefaultAsync(t => t.Id == p.TripId);
                    trips.Add(trip);
                }
                foreach(var t in trips)
                {
                    List<AppUser> users = new List<AppUser>();
                    var passengers = await _context.Passenger.Where(s => s.TripId == t.Id).ToListAsync();
                    foreach(var p in passengers)
                    {
                        var user = await _context.Users.Where(u => p.UserId == u.ID).FirstOrDefaultAsync();
                        if(user != null)
                        {
                            users.Add(user);
                        }
                    }
                    var owner = await _context.Users.Where(o => o.ID == t.CreatorId).FirstOrDefaultAsync();
                    var member = _mapper.Map<MemberDto>(owner);
                    var passengerMembers = _mapper.Map<List<MemberDto>>(users);
                    var car = await _context.Cars.SingleOrDefaultAsync(x => x.Id == t.CarId);
                    if(car != null)
                    {
                        var carDto = new CarInfoDto()
                        {
                            Id = car.Id,
                            Owner = member,
                            Mark = car.Mark,
                            Model = car.Model,
                            RegistrationNumber = car.RegistrationNumber, 
                            NumberOfSeats = car.NumberOfSeats,
                            Color = car.Color
                        };

                        TripInfoDto trip = new TripInfoDto
                        {
                            Id = t.Id,
                            Creator = member,
                            Passenger = passengerMembers,
                            Price = t.Price,
                            StartTime = t.StartTime,
                            StartFrom = t.StartFrom,
                            NumberOfSeats = t.NumberOfSeats,
                            Car = carDto,
                            EndIn = t.EndIn
                        };

                        chosenTrips.Add(trip);
                    }
                }
                return chosenTrips;
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