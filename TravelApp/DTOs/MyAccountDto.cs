using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entites;

namespace TravelApp.DTOs
{
    public class MyAccountDto
    {
        public MemberDto member { get; set; }
        public string Email { get; set; }
        
        
    }
}