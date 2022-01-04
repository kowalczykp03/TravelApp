using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace API.Entites
{
    public class Car
    {
        public int Id { get; set; }
        [ForeignKey("Users"), Column(Order = 0)] 
        public int OwnerId { get; set; }
        
        public string Mark  { get; set; }
        public string Model { get; set; }
        
        public string RegistrationNumber { get; set; }
        
        public int ProductionYear { get; set; }
        
        public int NumberOfSeats { get; set; }
        public string Color { get; set; }
        
        
        
        
        
    }
}