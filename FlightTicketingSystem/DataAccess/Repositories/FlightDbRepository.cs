using DataAccess.DataContext;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
    public class FlightDbRepository
    {
        private AirlineDbContext _airlineDbContext;
        public FlightDbRepository(AirlineDbContext airlineDbContext) {
            _airlineDbContext = airlineDbContext;
        }

        public IQueryable<Flight> GetFlights() {
            return _airlineDbContext.Flights;
        }

        public Flight? GetFlight(Guid id)
        {
            return _airlineDbContext.Flights.SingleOrDefault(x => x.Id == id);
        }

    }
}
