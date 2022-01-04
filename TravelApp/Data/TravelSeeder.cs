using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.Entites;
using Microsoft.EntityFrameworkCore;

namespace TravelApp.Data
{
    public class TravelSeeder
    {
        private readonly DataContext _dbContext;
        public TravelSeeder(DataContext dbContext)
        {
            _dbContext = dbContext;
            
        }
        public void Migrate()
        {
            if(_dbContext.Database.CanConnect())
            {
                var pendingMigrations = _dbContext.Database.GetPendingMigrations();
                if(pendingMigrations != null && pendingMigrations.Any())
                {
                    _dbContext.Database.Migrate();
                }

                if(!_dbContext.Users.Any())
                {
                    var users = getUsers();
                    _dbContext.Users.AddRange(users);
                    _dbContext.SaveChanges();
                }
            }
        }

        private IEnumerable<AppUser> getUsers()
        {
            var users = new List<AppUser>()
            {
                new AppUser()
                {
                    Email = "piotrkowalczyk@test.pl"
                },
                new AppUser()
                {
                    Email = "kacperk@test.pl"
                },
                new AppUser()
                {
                    Email = "sylwekk@test.pl"
                },
            };
            return users;
        }
    }
}