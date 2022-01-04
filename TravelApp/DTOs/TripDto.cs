using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.DTOs
{
    public class TripDto
    {
       public int Id { get; set; }
       
       public int CreatorId { get; set; }
       
       public Decimal Price { get; set; }
       public DateTimeOffset StartTime { get; set; }
       
       public string StartFrom { get; set; }
       
       public string EndIn { get; set; }
    }
}