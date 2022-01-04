using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using API.Entites;

namespace TravelApp.Entities
{
    public class Message
    {
        public int Id { get; set; }
        [ForeignKey("Trips"), Column(Order = 0)]
        public int TripId { get; set; }
        [ForeignKey("Users"), Column(Order = 0)]
        public int OwnerId { get; set; }
        public string TextMessage { get; set; }
        public DateTimeOffset AddedDate { get; set; }
       
       
    }
}