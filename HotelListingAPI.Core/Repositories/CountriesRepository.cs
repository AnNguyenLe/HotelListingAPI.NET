using AutoMapper;
using AutoMapper.QueryableExtensions;
using HotelListingAPI.Core.Contracts;
using HotelListingAPI.Core.Exceptions;
using HotelListingAPI.Core.Models.Country;
using HotelListingAPI.Data;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace HotelListingAPI.Repositories;

public class CountriesRepository(HotelListingDbContext context, IMapper mapper)
    : GenericRepository<Country>(context, mapper),
        ICountriesRepository
{
    public async Task<CountryDto> GetDetails(int id)
    {
        var country = await context
            .Countries.Include(q => q.Hotels)
            .ProjectTo<CountryDto>(mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(q => q.Id == id);

        if (country is null)
        {
            throw new NotFoundException(nameof(GetDetails), id);
        }

        return country;
    }
}
