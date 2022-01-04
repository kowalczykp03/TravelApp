using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;


namespace API.Entites
{
    public class AppUser
    {
        public int ID { get; set; }
        public int UserHash { get; set; }


        public string Email { get; set; }

        public string Name { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public DateTimeOffset Created { get; set; }
        public string Gender { get; set; }
        public string Description { get; set; }






    }
}