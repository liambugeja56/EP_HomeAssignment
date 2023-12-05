using Domain.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DataContext
{
    // abstraction of the database which models the database
    public class AirlineDbContext : IdentityDbContext<AuthenticatedUser>
    {
        public AirlineDbContext(DbContextOptions<AirlineDbContext> options)
            : base(options)
        {
        }

        public DbSet<Flight> ?Flights { get; set; }
        public DbSet<Ticket> ?Tickets { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Flight>()
                .Property(e => e.Id)
                .HasDefaultValueSql("NEWID()"); // This sets the default value to a new GUID

            // Other entity configurations...

            base.OnModelCreating(modelBuilder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLazyLoadingProxies();
        }
    }
}
