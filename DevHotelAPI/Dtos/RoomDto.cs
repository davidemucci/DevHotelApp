using DevHotelAPI.Entities;

namespace DevHotelAPI.Dtos
{
    public class RoomDto
    {
        public int? Number { get; set; }
        public string? Description { get; set; }
        public RoomType? Type { get; set; }
        public int? RoomTypeId { get; set; }
    }
}
