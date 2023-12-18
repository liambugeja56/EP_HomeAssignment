namespace PresentationLayer.Models.ViewModels
{
    public class SeatingPlanViewModel
    {
        public int MaxRows { get; set; }
        public int MaxCols { get; set; }

        public List<Seat> Seats = new List<Seat>();
        public string SelectedSeat { get; set; }

        public Guid FlightIdFK { get; set; }
    }

    public class Seat
    {
        public string Id { get; set; }
        public bool IsBooked { get; set; }
    }
}
