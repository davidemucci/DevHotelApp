using DevHotelAPI.Entities;

namespace DevHotelAPI.Dtos
{
    public class ReservationDto
    {
        public Guid? Id { get; set; }
        public Client? Client { get; set; }
        public Guid? ClientId { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public int RoomNumber { get; set; }
    }
}
