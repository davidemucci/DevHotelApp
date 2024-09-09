using DevHotelAPI.Entities;

namespace DevHotelAPI.Dtos
{
    public class CustomerDto
    {
        public Guid? Id { get; set; }
        public string? Name { get; set; }
        public string? Surname { get; set; }
        public string Email { get; set; }
        public string? Password { get; set; }
        public string? Address { get; set; }
        public List<Reservation>? Reservations { get; set; }
    }
}
