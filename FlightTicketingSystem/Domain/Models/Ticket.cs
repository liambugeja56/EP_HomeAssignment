using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class Ticket
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)] //Ticket PK
        public int Id { get; set; }

        [Required]
        public int Row { get; set; }

        [Required]
        public string Column { get; set; }

        [Required]
        [ForeignKey("Flight")] //Foreign Key that links the property to the Flight class
        public Guid FlightIdFK { get; set; }
        public virtual Flight Flight { get; set; } //navigational property

        [Required]
        public string Passport { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public double PricePaid { get; set; }

        public bool Cancelled { get; set; }
    }
}
