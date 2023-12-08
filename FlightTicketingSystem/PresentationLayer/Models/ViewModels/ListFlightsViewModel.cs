using Domain.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace PresentationLayer.Models.ViewModels
{
    public class ListFlightsViewModel
    {
        public Guid Id { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        [DisplayName("Departure Date")]
        public DateTime DepartureDate { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        [DisplayName("Arrival Date")]
        public DateTime ArrivalDate { get; set; }

        [Required]
        [DisplayName("Country From")]
        public string CountryFrom { get; set; }

        [Required]
        [DisplayName("Country To")]
        public string CountryTo { get; set; }

        [DisplayName("Retail Price")]
        public double RetailPrice {  get; set; }

        [DisplayName("Fully Booked")]
        public bool isFullyBooked {  get; set; }

        //[DisplayName("Passport Image")]
        //public string? PassportImage { get; set; }
    }
}
