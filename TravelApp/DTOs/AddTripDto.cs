using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TravelApp.DTOs
{
    public class AddTripDto
    {  
       [Required]
       public Decimal Price { get; set; }
       [Required]
       public DateTimeOffset StartTime { get; set; }
       [Required]
       public int CarID {get; set; }
       [Required]
       public int NumberOfSeats {get; set;}
       [Required]
       public string StartFrom { get; set; }
       [Required]
       public string EndIn { get; set; }
       
       
       
    }
}