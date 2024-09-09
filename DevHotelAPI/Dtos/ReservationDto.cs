using DevHotelAPI.Entities;

namespace DevHotelAPI.Dtos
{
    public class ReservationDto
    {
        public Guid? Id { get; set; }
        public Customer? Customer { get; set; }
        public Guid? CustomerId { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public int RoomNumber { get; set; }
    }
}
