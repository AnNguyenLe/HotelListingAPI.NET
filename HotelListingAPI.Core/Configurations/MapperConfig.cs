using AutoMapper;
using HotelListingAPI.Core.Models.Country;
using HotelListingAPI.Core.Models.Hotel;
using HotelListingAPI.Core.Models.Users;
using HotelListingAPI.Data;

namespace HotelListingAPI.Core.Configurations;

public class MapperConfig : Profile
{
    public MapperConfig()
    {
        CreateMap<Country, CreateCountryDto>().ReverseMap();
        CreateMap<Country, GetCountryDto>().ReverseMap();
        CreateMap<Country, CountryDto>().ReverseMap();
        CreateMap<Country, UpdateCountryDto>().ReverseMap();

        CreateMap<Hotel, HotelDto>().ReverseMap();
        CreateMap<Hotel, CreateHotelDto>().ReverseMap();

        CreateMap<ApiUser, ApiUserDto>().ReverseMap();
    }
}
