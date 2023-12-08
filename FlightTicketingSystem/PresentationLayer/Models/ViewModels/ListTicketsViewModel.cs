using Domain.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace PresentationLayer.Models.ViewModels
{
    public class ListTicketsViewModel
    {
        public int Id { get; set; }

        public int Row { get; set; }

        public int Column { get; set; }

        public Guid FlightIdFK { get; set; }

        [DisplayName("Flight To")]
        public string FlightTo { get; set; } 

        public string Passport { get; set; }

        [Range(0, int.MaxValue)]
        [DisplayName("Price Paid")]
        public double PricePaid { get; set; }

        public bool Cancelled { get; set; }

        [DisplayName("Passport Image")]
        public string? PassportImage { get; set; }
    }
}
