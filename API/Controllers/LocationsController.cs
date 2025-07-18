using API.Models;
using API.Models.DTOs.Location;
using API.Services;
using API.Services.Location;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LocationsController(ILocationServices locationServices)
    {
        private readonly ILocationServices _locationServices = locationServices;

        [HttpPost]
        public async Task<Response<int>> AddLocation(AddLocationDto location)
        {
            return await _locationServices.AddLocation(location);
        }

        [HttpPost("addMultiple")]
        public async Task<Response<int[]>> AddLocations(AddLocationDto[] locations)
        {
            return await _locationServices.AddLocations(locations);
        }

        [HttpGet]
        public async Task<Response<Location[]>> GetLocations([FromQuery(Name = "pageNumber")] int? pageNumber, [FromQuery(Name = "pageSize")] int? pageSize)
        {
            return await _locationServices.GetLocations(pageNumber, pageSize);
        }

        [HttpGet("{id}")]
        public async Task<Response<Location>> GetLocation(int id)
        {
            return await _locationServices.GetLocationById(id);
        }

        [HttpPut("{id}")]
        public async Task<Response<Location>> UpdateLocation(int id, UpdateLocationDto location)
        {
            return await _locationServices.UpdateLocation(id, location);
        }

        [HttpDelete("{id}")]
        public async Task<Response<bool>> DeleteLocation(int id)
        {
            return await _locationServices.DeleteLocation(id);
        }
    };
}

