using HotelListingAPI.Data;

namespace HotelListingAPI.Core.Contracts;

public interface ICountriesRepository : IGenericRepository<Country>
{
    Task<Country> GetDetails(int id);
}
