namespace DevHotelAPI.Entities
{
    public class Room
    {
        public string? Description { get; set; }
        public int Number { get; set; }
        public int RoomTypeId { get; set; }
        public RoomType? Type { get; set; }
    }
}
