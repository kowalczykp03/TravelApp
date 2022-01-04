using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Entites;
using Microsoft.EntityFrameworkCore;
using TravelApp.Entities;

namespace API.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
            
        }
        public DbSet<AppUser> Users { get; set; }
        public DbSet<Trip> Trips {get; set; }
        public DbSet<Passenger> Passenger {get; set;}
        public DbSet<Car> Cars {get; set;}
        public DbSet<Opinion> Opinions {get; set;}
        public DbSet<Message> Messages {get; set;}
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
                 modelBuilder.Entity<AppUser>()
                .Property(r => r.Email)
                .IsRequired();   
        }
    }
}