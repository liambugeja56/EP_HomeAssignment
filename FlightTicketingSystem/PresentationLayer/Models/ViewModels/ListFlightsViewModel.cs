﻿using System.ComponentModel.DataAnnotations;

namespace PresentationLayer.Models.ViewModels
{
    public class ListFlightsViewModel
    {
        public Guid Id { get; set; }

        [Required]
        public DateTime DepartureDate { get; set; }

        [Required]
        public DateTime ArrivalDate { get; set; }

        [Required]
        public string CountryFrom { get; set; }

        [Required]
        public string CountryTo { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public double WholesalePrice { get; set; }

        public double RetailPrice {  get; set; }

        public bool isFullyBooked {  get; set; }
    }
}
