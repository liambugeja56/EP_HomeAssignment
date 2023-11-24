using DataAccess.Repositories;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace PresentationLayer.Controllers
{
    public class TicketsController : Controller
    {

        // Dependency Injection -------------------------------------------------------------------------------
        private FlightDbRepository _flightDbRepository;
        private TicketDbRepository _ticketDbRepository;

        public TicketsController(FlightDbRepository flightDbRepository, TicketDbRepository ticketDbRepository)
        {
            _flightDbRepository = flightDbRepository;
            _ticketDbRepository = ticketDbRepository;
        }
        // Dependency Injection -------------------------------------------------------------------------------

        public IActionResult Index()
        {
            var listOfFlights = _flightDbRepository.GetFlights().OrderBy(x => x.WholesalePrice);

            var availableFlights = listOfFlights
                .Where(flight => !IsFlightFullyBooked(flight.Id))
                .Where(flight => flight.DepartureDate > DateTime.Now).ToList();

            return View(availableFlights);
        }

        private bool IsFlightFullyBooked(Guid flightId)
        {
            var flight = _flightDbRepository.GetFlight(flightId);

            int bookedSeats = _ticketDbRepository.GetTickets().Count(x => x.FlightIdFK == flightId);

            int flightSeatCapacity = flight.Rows * flight.Columns;


            if (bookedSeats == flightSeatCapacity)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
