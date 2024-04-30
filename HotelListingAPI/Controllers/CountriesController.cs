using AutoMapper;
using HotelListingAPI.Core.Contracts;
using HotelListingAPI.Core.Models;
using HotelListingAPI.Core.Models.Country;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.EntityFrameworkCore;

namespace HotelListingAPI.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    public class CountriesController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ICountriesRepository _countryRepository;
        private readonly ILogger<CountriesController> _logger;

        public CountriesController(
            IMapper mapper,
            ICountriesRepository countryRepository,
            ILogger<CountriesController> logger
        )
        {
            _mapper = mapper;
            _countryRepository = countryRepository;
            _logger = logger;
        }

        // GET: api/Countries/All
        [HttpGet("All")]
        [EnableQuery]
        public async Task<ActionResult<IEnumerable<GetCountryDto>>> GetCountries()
        {
            var countries = await _countryRepository.GetAllAsync<GetCountryDto>();
            return Ok(countries);
        }

        // GET: api/Countries?StartIndex=0&PageSize=25&PageNumber=1
        [HttpGet]
        public async Task<ActionResult<PagedResult<GetCountryDto>>> GetPagedCountries(
            [FromQuery] QueryParameters queryParameters
        )
        {
            var pagedCountriesResult = await _countryRepository.GetAllAsync<GetCountryDto>(
                queryParameters
            );
            return Ok(pagedCountriesResult);
        }

        // GET: api/Countries/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CountryDto>> GetCountry(int id)
        {
            var country = await _countryRepository.GetDetails(id);
            return Ok(country);
        }

        // PUT: api/Countries/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutCountry(int id, UpdateCountryDto updateCountryDto)
        {
            if (id != updateCountryDto.Id)
            {
                return BadRequest("Invalid record id");
            }

            try
            {
                await _countryRepository.UpdateAsync(id, updateCountryDto);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await CountryExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Countries
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<CountryDto>> PostCountry(CreateCountryDto createCountryDto)
        {
            var country = await _countryRepository.AddAsync<CreateCountryDto, GetCountryDto>(
                createCountryDto
            );

            return CreatedAtAction(nameof(GetCountry), new { id = country.Id }, country);
        }

        // DELETE: api/Countries/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> DeleteCountry(int id)
        {
            await _countryRepository.DeleteAsync(id);

            return NoContent();
        }

        private async Task<bool> CountryExists(int id)
        {
            return await _countryRepository.Exists(id);
        }
    }
}
