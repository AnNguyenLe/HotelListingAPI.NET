using AutoMapper;
using HotelListingAPI.Core.Contracts;
using HotelListingAPI.Data;
using Microsoft.EntityFrameworkCore;

namespace HotelListingAPI.Repositories;

public class CountriesRepository(HotelListingDbContext context, IMapper mapper)
    : GenericRepository<Country>(context, mapper),
        ICountriesRepository
{
    private readonly HotelListingDbContext _context = context;

    public async Task<Country> GetDetails(int id)
    {
        return await _context.Countries.Include(q => q.Hotels).FirstOrDefaultAsync(q => q.Id == id);
    }
}
