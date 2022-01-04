using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TravelApp.DTOs
{
    public class TripsSearchDto
    {
        public string StartFrom { get; set; }
        
        public string EndIn { get; set; }
        
        public DateTimeOffset StartTime { get; set; }
        
        
    }
}