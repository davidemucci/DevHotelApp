using AutoMapper;
using DevHotelAPI.Dtos;
using DevHotelAPI.Entities;

namespace DevHotelAPI.Services.Mapper
{
    public class MainMapperProfile : Profile
    {
        public MainMapperProfile()
        {
            CreateMap<RoomTypeDto, RoomType>();
            CreateMap<RoomDto, Room>().ReverseMap();
            CreateMap<CustomerDto, Customer>().ReverseMap();
            CreateMap<ReservationDto, Reservation>().ReverseMap();
        }
    }
}
