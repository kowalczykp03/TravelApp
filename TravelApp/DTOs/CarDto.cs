using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.DTOs
{
    public class CarDto
    {   

        public string Mark  { get; set; }
        public string Model { get; set; }
        
        public string RegistrationNumber { get; set; }
        
        public int ProductionYear { get; set; }
        
        public int NumberOfSeats { get; set; }
         public string Color { get; set; }
    }
}