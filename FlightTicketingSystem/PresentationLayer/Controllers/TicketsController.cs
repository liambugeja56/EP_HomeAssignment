using DataAccess.Repositories;
using Domain.Interfaces;
using Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PresentationLayer.Models.ViewModels;

namespace PresentationLayer.Controllers
{
    public class TicketsController : Controller
    {

        // Dependency Injection -------------------------------------------------------------------------------
        private FlightDbRepository _flightDbRepository;
        private ITicket _ticketDbRepository;
        private UserManager<AuthenticatedUser> _userManager;

        public TicketsController(FlightDbRepository flightDbRepository, ITicket ticketDbRepository, UserManager<AuthenticatedUser> userManager)
        {
            _flightDbRepository = flightDbRepository;
            _ticketDbRepository = ticketDbRepository;
            _userManager = userManager;
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
        public IActionResult Book(BookFlightViewModel model, [FromServices] IWebHostEnvironment host)
        {
            try
            {
                var flight = _flightDbRepository.GetFlight(model.FlightIdFK);

                string fileName = "";

                if(model.ImageFile != null)
                {
                    string uploadFolder = Path.Combine(host.WebRootPath, "uploads/passports");

                    if (!Directory.Exists(uploadFolder))
                    {
                        Directory.CreateDirectory(uploadFolder);
                    }

                    fileName = Guid.NewGuid().ToString() + "_" + model.ImageFile.FileName;

                    string filePath = Path.Combine(uploadFolder, fileName);

                    try
                    {
                        using(FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate))
                        {
                            model.ImageFile.CopyTo(fs);
                            fs.Flush();
                        }
                    }
                    catch (Exception)  
                    { 
                    
                    }

                } 

                if (flight.DepartureDate > DateTime.Now && !IsFlightFullyBooked(model.FlightIdFK))
                {

                    if (model.Cancelled == false)
                    {
                        _ticketDbRepository.Book(new Ticket()
                        {
                            Row = model.Row,
                            Column = model.Column,
                            FlightIdFK = model.FlightIdFK,
                            Passport = model.Passport,
                            PricePaid = flight.WholesalePrice * flight.CommissionRate,
                            Cancelled = model.Cancelled, 
                            PassportImage = fileName
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
                    TempData["error"] = "Ticket was not booked succesfully, please make sure the flight is not fully booked";
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
                    RetailPrice = flight.WholesalePrice * markUpPrice,
                    isFullyBooked = IsFlightFullyBooked(flight.Id)
                };

                return View(myFlight);
            }
        }

        public async Task<IActionResult> List()
        {
            if (User.Identity.IsAuthenticated)
            {
                string userPassport = await GetUserPassport();

                var userTickets = _ticketDbRepository.GetTickets().
                    Where(t => t.Passport == userPassport);

                var ticketViewModels = userTickets.Select(ticket => new ListTicketsViewModel
                {
                    Id = ticket.Id,
                    Row = ticket.Row,
                    Column = ticket.Column,
                    FlightIdFK = ticket.FlightIdFK,
                    Passport = ticket.Passport,
                    PricePaid = ticket.PricePaid,
                    Cancelled = ticket.Cancelled,
                    PassportImage = ticket.PassportImage,
                    FlightTo = _flightDbRepository.GetFlight(ticket.FlightIdFK).CountryTo
                });

                return View(ticketViewModels);
            }

            return View();
        }

        private async Task<string?> GetUserPassport()
        {
            var user = await _userManager.GetUserAsync(User);
            return user?.Passport;
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
