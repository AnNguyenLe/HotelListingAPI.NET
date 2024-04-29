using System.ComponentModel.DataAnnotations.Schema;

namespace HotelListingAPI.Data;

public class Hotel
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Address { get; set; }
    public double Rating { get; set; }
    [ForeignKey(nameof(CountryId))]
    public int CountryId { get; set; }
    public Country Country { get; set; }
}
