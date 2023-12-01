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
            var listOfFlights = _flightDbRepository.GetFlights().OrderBy(x => x.DepartureDate)
                                .Where(f => f.DepartureDate > DateTime.Now)
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

        [HttpGet]
        public IActionResult Book()
        {
            BookFlightViewModel viewModel = new BookFlightViewModel();

            viewModel.Flights = _flightDbRepository.GetFlights().ToList();

            return View(viewModel);

        }

        [HttpPost]
        public IActionResult Book(BookFlightViewModel model)
        {
            try
            {
                var flight = _flightDbRepository.GetFlight(model.FlightIdFK);

                if(flight.DepartureDate > DateTime.Now)
                {

                    if (!model.Cancelled)
                    {
                        _ticketDbRepository.Book(new Ticket()
                        {
                            Row = model.Row,
                            Column = model.Column,
                            FlightIdFK = model.FlightIdFK,
                            Passport = model.Passport,
                            PricePaid = flight.WholesalePrice * flight.CommissionRate,
                            Cancelled = model.Cancelled
                        });

                        TempData["message"] = "Ticket was booked successfully";

                        return RedirectToAction("Index");
                    }                
                    else
                    {
                        TempData["error"] = "Flight is either fully booked or the seat you have chosen is already taken";
                        model.Flights = _flightDbRepository.GetFlights().ToList();
                        return View(model);
                    }

                }
                else
                {
                    TempData["error"] = "Ticket was not booked succesfully";
                    model.Flights = _flightDbRepository.GetFlights().ToList();
                    return View(model);
                }                     
            }
            catch
            {
                TempData["error"] = "Ticket was not booked successfully";
                model.Flights = _flightDbRepository.GetFlights().ToList();
                return View(model);
            }
        }


        public IActionResult Details(Guid id)
        {
            var flight = _flightDbRepository.GetFlight(id);

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
                    isFullyBooked = IsFlightFullyBooked(flight.Id)
                };

                return View(myFlight);
            }
        }

        private bool IsFlightFullyBooked(Guid flightId)
        {
            var flight = _flightDbRepository.GetFlight(flightId);

            int bookedSeats = _ticketDbRepository.GetTickets().Count(f => f.FlightIdFK == flightId);

            int flightSeatCapacity = flight.Rows * flight.Columns;

            return bookedSeats >= flightSeatCapacity;
        }

    }
}
