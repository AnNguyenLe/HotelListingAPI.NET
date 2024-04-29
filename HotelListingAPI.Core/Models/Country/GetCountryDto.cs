using HotelListingAPI.Core.Models.Hotel;

namespace HotelListingAPI.Core.Models.Country;

public class GetCountryDto : BaseCountryDto
{
    public string Id { get; set; }
}

public class CountryDto : BaseCountryDto
{
    public string Id { get; set; }
    public List<HotelDto> Hotels { get; set; }
}
