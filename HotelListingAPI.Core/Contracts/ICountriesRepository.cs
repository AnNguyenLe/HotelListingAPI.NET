using HotelListingAPI.Core.Models.Country;
using HotelListingAPI.Data;

namespace HotelListingAPI.Core.Contracts;

public interface ICountriesRepository : IGenericRepository<Country>
{
    Task<CountryDto> GetDetails(int id);
}
