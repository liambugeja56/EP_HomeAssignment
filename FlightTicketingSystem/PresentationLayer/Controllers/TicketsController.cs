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
            var listOfFlights = _flightDbRepository.GetFlights();

            var availableFlights = listOfFlights
                                    .Where(f => f.DepartureDate > DateTime.Now)
                                    .ToList();

            double markUpPrice = 1.2;

            var showFlights = from f in availableFlights
                              select new ListFlightsViewModel()
                              {
                                  Id = f.Id,
                                  DepartureDate = f.DepartureDate,
                                  ArrivalDate = f.ArrivalDate,
                                  CountryFrom = f.CountryFrom,
                                  CountryTo = f.CountryTo,
                                  RetailPrice = f.WholesalePrice * markUpPrice,
                                  isFullyBooked = IsFlightFullyBooked(f.Id)
                              };

            return View(showFlights);
        }

        private bool IsFlightFullyBooked(Guid flightId)
        {
            var flight = _flightDbRepository.GetFlight(flightId);

            int bookedSeats = _ticketDbRepository.GetTickets().Count(f => f.FlightIdFK == flightId);

            int flightSeatCapacity = flight.Rows * flight.Columns;

            return bookedSeats >= flightSeatCapacity;
        }

        public IActionResult Details(Guid flightId)
        {
            var flight = _flightDbRepository.GetFlight(flightId);

            double markUpPrice = 1.2;

            if (flight == null)
            {
                return RedirectToAction("Index");
            }
            else
            {
                ListFlightsViewModel myFlight = new ListFlightsViewModel()
                {
                    Id = flight.Id,
                    DepartureDate = flight.DepartureDate,
                    ArrivalDate = flight.ArrivalDate,
                    CountryFrom = flight.CountryFrom,
                    CountryTo = flight.CountryTo,
                    WholesalePrice = flight.WholesalePrice,
                    RetailPrice = flight.WholesalePrice * markUpPrice,
                    isFullyBooked = flight.Equals(true)
                };

                return View(myFlight);
            }
        }
    }
}
