using API.Models.Location;
using API.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LocationsController(ILocationServices locationServices) : BaseLocationsController
    {
        private readonly ILocationServices _locationServices = locationServices;

        [HttpPost]
        public ActionResult<Response<int>> AddLocation(AddLocationDto location)
        {
            return HandleResponse(_locationServices.AddLocation(location));
        }

        [HttpPost("addMultiple")]
        public ActionResult<Response<int[]>> AddLocations(AddLocationDto[] locations)
        {
            return HandleResponse(_locationServices.AddLocations(locations));
        }

        [HttpGet]
        public ActionResult<Response<Location[]>> GetLocations()
        {
            return HandleResponse(_locationServices.GetLocations());
        }

        [HttpGet("{id}")]
        public ActionResult<Response<Location>> GetLocation(int id)
        {
            return HandleResponse(_locationServices.GetLocationById(id));
        }

        [HttpPut("{id}")]
        public ActionResult<Response<Location>> UpdateLocation(int id, UpdateLocationDto location)
        {
            return HandleResponse(_locationServices.UpdateLocation(id, location));
        }

        [HttpDelete("{id}")]
        public ActionResult<Response<bool>> DeleteLocation(int id)
        {
            return HandleResponse(_locationServices.DeleteLocation(id));
        }
    };
}

