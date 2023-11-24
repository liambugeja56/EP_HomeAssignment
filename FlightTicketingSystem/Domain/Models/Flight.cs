using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class Flight
    {
        [Key] //assigning the Primary Key for the Flight class 
        public Guid Id { get; set; }

        [Required]
        public int Rows { get; set; }
        
        [Required]
        public int Columns { get; set; }

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

        public double CommissionRate { get; set; }
    }
}
