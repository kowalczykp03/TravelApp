using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Entites;

namespace API.DTOs
{
    public class MemberDto
    {
        public int Id { get; set; }
        
        public int UserHash { get; set; }
        public List<Opinion> Opinions {get; set;} = new List<Opinion>();
        
        public string Name { get; set; }

        public string Gender { get; set; }
        public string Description { get; set; }
        
        
    }
}