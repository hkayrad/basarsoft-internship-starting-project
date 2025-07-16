using API.Models.Location;
using API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LocationsController : ControllerBase
    {
        private readonly ILocationServices _locationServices;

        public LocationsController(ILocationServices locationServices)
        {
            _locationServices = locationServices;
        }

        [HttpGet]
        public async Task<ActionResult<Location[]>> GetLocations()
        {
            var result = await _locationServices.GetLocations();

            if (result == null || result.Length == 0)
                return NotFound("No locations found.");

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Location>> GetLocation(int id)
        {
            var result = await _locationServices.GetLocationById(id);

            if (result == null)
                return NotFound("Location not found.");

            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteLocation(int id)
        {
            await _locationServices.DeleteLocation(id);
            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult> AddLocation(AddLocationDto location)
        {
            await _locationServices.AddLocation(location);
            return NoContent();
        }

        [HttpPost("addMultiple")]
        public async Task<ActionResult> AddLocations(AddLocationDto[] locations)
        {
            await _locationServices.AddLocations(locations);
            return NoContent();
        }

        [HttpPut]
        public async Task<ActionResult> UpdateLocation(UpdateLocationDto location)
        {
            await _locationServices.UpdateLocation(location);
            return NoContent();
        }

    };
}

