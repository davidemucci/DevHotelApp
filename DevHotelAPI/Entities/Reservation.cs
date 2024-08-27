namespace DevHotelAPI.Entities
{
    public class Reservation
    {
        public Guid Id { get; set; }
        public Client Client { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public int RoomNumber { get; set; }
    }
}

