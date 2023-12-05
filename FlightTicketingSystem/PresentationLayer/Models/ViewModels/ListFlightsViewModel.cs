using Domain.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PresentationLayer.Models.ViewModels
{
    public class ListFlightsViewModel
    {
        public Guid Id { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime DepartureDate { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime ArrivalDate { get; set; }

        [Required]
        public string CountryFrom { get; set; }

        [Required]
        public string CountryTo { get; set; }

        public double RetailPrice {  get; set; }

        public bool isFullyBooked {  get; set; }

        public string? PassportImage { get; set; }
    }
}
