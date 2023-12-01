﻿using Domain.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PresentationLayer.Models.ViewModels
{
    public class BookFlightViewModel
    {
        public List<Flight> Flights { get; set; }

        [Required]
        public int Row { get; set; }

        [Required]
        public int Column { get; set; }

        [Required]
        public Guid FlightIdFK { get; set; }

        [Required]
        public string Passport { get; set; }

        [Range(0, int.MaxValue)]
        public double PricePaid { get; set; }

        public bool Cancelled { get; set; }
    }
}
