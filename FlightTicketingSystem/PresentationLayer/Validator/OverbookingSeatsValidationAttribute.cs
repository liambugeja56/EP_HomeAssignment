using PresentationLayer.Models.ViewModels;
using System.ComponentModel.DataAnnotations;
using DataAccess.Repositories;

namespace PresentationLayer.Validator
{
    public class OverbookingSeatsValidationAttribute : ValidationAttribute
    {
        public string GetErrorMessage() => $"Booking is exceeding the total number of available seats";

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var model = (BookFlightViewModel)validationContext.ObjectInstance;

            var flightId = model.FlightIdFK;
            var selectedRow = model.Row;
            var selectedColumn = model.Column;

            var _ticketRepository = (TicketDbRepository)(validationContext.GetService(typeof(TicketDbRepository)));

            if (_ticketRepository == null)
            {
                throw new InvalidOperationException("Ticket repository is not available.");
            }

            var totalBookedSeats = _ticketRepository.totalSeatBooking(flightId);
            var totalAvailableSeats = _ticketRepository.totalSeatAvailability(flightId, selectedRow, selectedColumn);

            if (totalBookedSeats >= totalAvailableSeats)
            {
                return new ValidationResult(GetErrorMessage());
            }

            return ValidationResult.Success;
        }

    }
}
