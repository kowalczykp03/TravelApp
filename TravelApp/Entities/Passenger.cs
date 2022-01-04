using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace API.Entites
{
    public class Passenger
    {
        public Passenger(int userId, int tripId)
        {
            UserId = userId;
            TripId = tripId;
        } 
        public int Id { get; set; }
        
        [ForeignKey("Users"), Column(Order = 0)] 
        public int UserId { get; set; }
        [ForeignKey("Trips"), Column(Order = 0)]
        public int TripId { get; set; }
        
        

    }


}