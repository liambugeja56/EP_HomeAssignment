using DataAccess.DataContext;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
    public class TicketDbRepository
    {

        private AirlineDbContext _airlineDbContext;
        public TicketDbRepository(AirlineDbContext airlineDbContext)
        {
            _airlineDbContext = airlineDbContext;
        }

        public IQueryable<Ticket> GetTickets(Guid flightId)
        {
            return _airlineDbContext.Tickets.Where(t => t.FlightIdFK == flightId);
        }

        public Ticket? GetTicket(int id) //getting a ticket based on the Id
        {
            return _airlineDbContext.Tickets.SingleOrDefault(x => x.Id == id);
        }

        private bool seatAvailable(Guid flightId, int row, string column)
        {
            return _airlineDbContext.Tickets.Any(t => 
            t.FlightIdFK == flightId && t.Row == row && t.Column == column && !t.Cancelled);
        }

        public void Book(Ticket ticket) //to be continued
        {
            bool isSeatAvailable = seatAvailable(ticket.FlightIdFK, ticket.Row, ticket.Column);

            if(isSeatAvailable) //if the seat is available
            {
                _airlineDbContext.Tickets.Add(ticket);
                _airlineDbContext.SaveChanges();
            }
            else throw new Exception("This seat is already booked, please select an available seat");
        }

        public void Cancel(int id) //to be continued
        {
            var ticket = GetTicket(id); //getting the ticket using Function that gets us the ticket depending on the id passed

            if(ticket != null) //if the ticket is not null
            {
                ticket.Cancelled = true; //cancel the ticket
                _airlineDbContext.SaveChanges(); //save the changes - this way the ticket is set to "Cancelled"
            }
            else throw new Exception("Ticket cannot be cancelled. Make sure that the ticket exists");
        }
    }
}
