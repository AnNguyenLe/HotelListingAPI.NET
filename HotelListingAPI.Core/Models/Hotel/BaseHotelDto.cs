using System.ComponentModel.DataAnnotations;

namespace HotelListingAPI.Core.Models.Hotel;

public abstract class BaseHotelDto
{
    [Required]
    public required string Name { get; set; }

    [Required]
    public required string Address { get; set; }
    public double? Rating { get; set; }

    [Required]
    [Range(1, int.MaxValue)]
    public int CountryId { get; set; }
}
