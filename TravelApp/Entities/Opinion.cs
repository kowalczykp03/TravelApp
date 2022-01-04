using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace API.Entites
{
    public class Opinion
    {
        public int Id { get; set; }
        
        [ForeignKey("Users"), Column(Order = 0)] 
        public int UserId { get; set; }
        [ForeignKey("Users"), Column(Order = 0)]
        public int SenderId { get; set; }
        public int OpinionValue { get; set; }
        
        public string OpinionDescription { get; set; }
        public DateTimeOffset Date { get; set; }   
        
    }
}