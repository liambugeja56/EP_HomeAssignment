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
        public IActionResult Book(Guid flightId, int selectedRow, int selectedColumn)
        {
            var viewModel = new BookFlightViewModel
            {
                FlightIdFK = flightId,
                Row = selectedRow,
                Column = selectedColumn,
                Flights = _flightDbRepository.GetFlights().ToList(),
                Passport = "",
                PricePaid = 0,
                Cancelled = false,
                PassportImage = ""
            };

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

                if (flight.DepartureDate > DateTime.Now && !_ticketDbRepository.seatAvailable(model.FlightIdFK, model.Row, model.Column))
                {

                    if (model.Cancelled == false && !IsFlightFullyBooked(model.FlightIdFK))
                    {
                        _ticketDbRepository.Book(new Ticket()
                        {
                            Row = model.Row,
                            Column = model.Column,
                            FlightIdFK = model.FlightIdFK,
                            Passport = model.Passport,
                            PricePaid = flight.WholesalePrice * flight.CommissionRate,
                            Cancelled = false,
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
            catch (Exception ex)
            {

                TempData["error"] = "Ticket was not booked successfully: " + ex.Message;
                // Return to the "SeatingPlan" action if there's an exception
                return RedirectToAction("SeatingPlan", new { model.FlightIdFK });

                //TempData["error"] = "Ticket was not booked successfully";
                //model.Flights = _flightDbRepository.GetFlights().ToList();
                //return View(model);
            }
        }

        [HttpGet]
        public IActionResult SeatingPlan(Guid flightId)
        {
            var flightInfo = _flightDbRepository.GetFlight(flightId);

            SeatingPlanViewModel model = new SeatingPlanViewModel();

            //model.Flights = _flightDbRepository.GetFlights().ToList();
            model.FlightIdFK = flightId;
            model.MaxRows = flightInfo.Rows;
            model.MaxCols = flightInfo.Columns;
            model.Seats = new List<Seat>();

            // Populate seat availability information
            for (int row = 1; row <= model.MaxRows; row++)
            {
                for (int col = 1; col <= model.MaxCols; col++)
                {
                    var seatId = row + "," + col;

                    // Check if the seat is booked for the given flightId
                    bool isSeatBooked = _ticketDbRepository.seatAvailable(flightId, row, col);

                    Seat mySeat = new Seat
                    {
                        Id = seatId,
                        IsBooked = isSeatBooked // Set the IsBooked property based on seat availability
                    };

                    model.Seats.Add(mySeat); // Add the seat to the Seats list in the ViewModel
                }
            }

            return View(model);
        }

        [HttpPost]
        public IActionResult SeatingPlan(SeatingPlanViewModel m)
        {
            try
            {
                var flightInfo = _flightDbRepository.GetFlight(m.FlightIdFK);

                int row = Convert.ToInt16(m.SelectedSeat.Split(new char[] { ',' })[0]);
                int col = Convert.ToInt16(m.SelectedSeat.Split(new char[] { ',' })[1]);

                //return RedirectToAction("Book", new { flightId = m.FlightIdFK, selectedRow = row, selectedColumn = col });
                return View("Book");
            }
            catch (Exception)
            { 
                throw new Exception("This seat is already booked");
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
