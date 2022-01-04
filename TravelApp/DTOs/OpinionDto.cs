using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.DTOs
{
    public class OpinionDto
    {
        public int UserId { get; set; }
        public int OpinionValue { get; set; }
        public string OpinionDescription { get; set; }
 
    }
}