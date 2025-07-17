using System.Net;
using API.Models.Location;

namespace API.Services;

public class StringLocationServices : ILocationServices
{
    private Location[] locations = [
        new Location { Id = 1, Name = "Location1", Wkt = "POINT(1 1)" },
        new Location { Id = 2, Name = "Location2", Wkt = "POINT(2 2)" },
        new Location { Id = 3, Name = "Location3", Wkt = "POINT(3 3)" }
    ];

    public Response<Location[]> GetLocations()
    {
        var locations = this.locations;

        if (locations == null)
            return Response<Location[]>.Fail("No locations found.", HttpStatusCode.NotFound);

        return Response<Location[]>.Success(locations, "Locations retrieved successfully.");
    }

    public Response<Location> GetLocationById(int id)
    {
        var location = locations.FirstOrDefault(x => x.Id == id);

        if (location == null)
            return Response<Location>.Fail("Location not found.");

        return Response<Location>.Success(location, "Location retrieved successfully.");
    }

    public Response<bool> DeleteLocation(int id)
    {
        var location = locations.FirstOrDefault(x => x.Id == id);

        if (location == null)
            return Response<bool>.Fail("Location not found.", HttpStatusCode.NotFound);

        locations = locations.Where(x => x.Id != id).ToArray();

        return Response<bool>.Success(true, "Location deleted successfully.", HttpStatusCode.NoContent);
    }

    public Response<int> AddLocation(AddLocationDto addLocationDto)
    {
        if (addLocationDto == null)
            return Response<int>.Fail("Location cannot be null", HttpStatusCode.BadRequest);

        int newId = locations.Max(x => x.Id) + 1;

        // Map DTO to Entity
        var newLocation = new Location
        {
            Id = newId,
            Name = addLocationDto.Name,
            Wkt = addLocationDto.Wkt
        };

        locations = locations.Append(newLocation).ToArray();

        return Response<int>.Success(newLocation.Id, "Location added successfully.");
    }

    public Response<int[]> AddLocations(AddLocationDto[] addLocationDtos)
    {
        if (addLocationDtos.Length == 0 || addLocationDtos == null)
            return Response<int[]>.Fail("No locations to add.", HttpStatusCode.NotFound);

        int[] newLocationIds = [];

        foreach (var newLocationDto in addLocationDtos)
        {
            if (newLocationDto == null)
                return Response<int[]>.Fail("Location cannot be null.");

            int newId = locations.Max(x => x.Id) + 1;

            // Map DTO to Entity
            var newLocation = new Location
            {
                Id = newId,
                Name = newLocationDto.Name,
                Wkt = newLocationDto.Wkt
            };

            locations = locations.Append(newLocation).ToArray();
            newLocationIds = newLocationIds.Append(newId).ToArray();
        }

        return Response<int[]>.Success(newLocationIds, "Locations added successfully.");
    }


    public Response<Location> UpdateLocation(int id, UpdateLocationDto updateLocationDto)
    {
        if (id <= 0 || id.GetType() != typeof(int))
            return Response<Location>.Fail("Invalid location ID.");

        if (updateLocationDto == null)
            return Response<Location>.Fail("Location cannot be null.");

        var existingLocation = locations.FirstOrDefault(x => x.Id == id);
        if (existingLocation == null)
            return Response<Location>.Fail("Location not found.", HttpStatusCode.NotFound);

        existingLocation.Name = updateLocationDto.Name;
        existingLocation.Wkt = updateLocationDto.Wkt;

        locations = locations.Select(x => x.Id == id ? existingLocation : x).ToArray();

        return Response<Location>.Success(existingLocation, "Location updated successfully.");
    }
}
