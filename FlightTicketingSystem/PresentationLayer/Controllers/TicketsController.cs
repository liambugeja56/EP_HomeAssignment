using DataAccess.Repositories;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;
using PresentationLayer.Models.ViewModels;

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
            var listOfFlights = _flightDbRepository.GetFlights().Where(f => f.DepartureDate > DateTime.Now)
                                    .ToList();

            double markUpPrice = 1.2;

            var showFlights = from f in listOfFlights
                              select new ListFlightsViewModel()
                              {
                                  Id = f.Id,
                                  DepartureDate = f.DepartureDate,
                                  ArrivalDate = f.ArrivalDate,
                                  CountryFrom = f.CountryFrom,
                                  CountryTo = f.CountryTo,
                                  WholesalePrice = f.WholesalePrice,
                                  RetailPrice = f.WholesalePrice * markUpPrice,
                                  isFullyBooked = IsFlightFullyBooked(f.Id)
                              };

            return View(showFlights);
        }

        public bool IsFlightFullyBooked(Guid flightId)
        {
            var flight = _flightDbRepository.GetFlight(flightId);

            int bookedSeats = _ticketDbRepository.GetTickets().Count(f => f.FlightIdFK == flightId);

            int flightSeatCapacity = flight.Rows * flight.Columns;

            return bookedSeats >= flightSeatCapacity;
        }
    }
}
