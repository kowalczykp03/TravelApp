using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TravelApp.DTOs;

namespace API.DTOs
{
    public class TripInfoDto
    {
       public int Id { get; set; }
       public MemberDto Creator { get; set; }
       public List<MemberDto> Passenger { get; set; } = new List<MemberDto>();
       public CarInfoDto Car { get; set; }
       public int NumberOfSeats { get; set; }
       
       public Decimal Price { get; set; }
       public DateTimeOffset StartTime { get; set; }
       
       public string StartFrom { get; set; }
       
       public string EndIn { get; set; }
    }
}