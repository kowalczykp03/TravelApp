using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Entites;

namespace TravelApp.DTOs
{
    public class ChatDto
    {
        public AppUser Owner { get; set; }
        public DateTimeOffset AddedDate { get; set; }
        
        public string TextMessage { get; set; }
    }
}