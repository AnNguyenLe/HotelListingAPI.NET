using AutoMapper;
using HotelListingAPI.Core.Contracts;
using HotelListingAPI.Data;

namespace HotelListingAPI.Repositories;

public class HotelsRepository(HotelListingDbContext context, IMapper mapper)
    : GenericRepository<Hotel>(context, mapper),
        IHotelsRepository { }
