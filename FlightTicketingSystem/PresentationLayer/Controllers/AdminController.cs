using Microsoft.AspNetCore.Mvc;
using DataAccess.Repositories;
using Domain.Models;

namespace PresentationLayer.Controllers
{
    public class AdminController : Controller
    {
        private FlightDbRepository _flightDbRepository;
        private TicketDbRepository _ticketDbRepository;

        public AdminController(FlightDbRepository flightDbRepository, TicketDbRepository ticketDbRepository)
        {
            _flightDbRepository = flightDbRepository;
            _ticketDbRepository = ticketDbRepository;
        }

        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated == false)
            {
                // give an error to the user
                // block the user

                //error is a variable and the variable is assigned the message Access Denied
                TempData["error"] = "Access Denied";

                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        public IActionResult ListOfFlights() {
            
            var flights = _flightDbRepository.GetFlights().ToList();
            return View(flights);
        }

        public IActionResult TicketDetails(int ticketId) {
            
            var ticket = _ticketDbRepository.GetTicket(ticketId);
            return View(ticket);
        }

        public IActionResult ListOfTickets(Guid flightId) {
        
            var tickets = _ticketDbRepository.GetTickets().Where(t => t.FlightIdFK == flightId).ToList();
            return View(tickets);
        }
    }
}
