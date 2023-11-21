using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class Flight
    {
        public Guid Id { get; set; }

        public int Rows { get; set; }

        public string Columns { get; set; }

        public DateTime DepartureDate { get; set; }

        public DateTime ArrivalDate { get; set; }

        public string CountryFrom { get; set; }

        public string CountryTo { get; set; }

        public double WholesalePrice { get; set; }

        public double CommissionRate { get; set; }
    }
}
