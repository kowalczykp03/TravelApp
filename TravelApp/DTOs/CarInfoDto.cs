using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;

namespace TravelApp.DTOs
{
    public class CarInfoDto
    {
        public int Id { get; set; }
        
        public MemberDto Owner {get; set;}
        public string Mark  { get; set; }
        public string Model { get; set; }
        
        public string RegistrationNumber { get; set; }
        public int ProductionYear { get; set; }
  
        public int NumberOfSeats { get; set; }
         public string Color { get; set; }
    }
}