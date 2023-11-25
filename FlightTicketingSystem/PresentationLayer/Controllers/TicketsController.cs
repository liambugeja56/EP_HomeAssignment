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
             var listOfFlights = _flightDbRepository.GetFlights().OrderBy(x => x.WholesalePrice);

            var availableFlights = listOfFlights
                                    .Where(f => f.DepartureDate > DateTime.Now)
                                    .ToList();

            double markUpPercentage = 1.2;

            var showFlights = from f in availableFlights
                               select new ListFlightsViewModel()
                               {
                                   Id = f.Id,
                                   DepartureDate = f.DepartureDate,
                                   ArrivalDate = f.ArrivalDate,
                                   CountryFrom = f.CountryFrom,
                                   CountryTo = f.CountryTo,
                                   RetailPrice = f.WholesalePrice * markUpPercentage
                               };

             return View(showFlights);
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
