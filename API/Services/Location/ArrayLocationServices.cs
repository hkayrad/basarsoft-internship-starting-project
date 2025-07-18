using System.Net;
using API.Models;
using API.Models.DTOs.Location;

namespace API.Services.Location;

public class ArrayLocationServices : ILocationServices
{
    private Models.Location[] locations = [
        new Models.Location { Id = 1, Name = "Location1", Wkt = "POINT(1 1)" },
        new Models.Location { Id = 2, Name = "Location2", Wkt = "POINT(2 2)" },
        new Models.Location { Id = 3, Name = "Location3", Wkt = "POINT(3 3)" },
        new Models.Location { Id = 4, Name = "Location4", Wkt = "POINT(4 4)" },
        new Models.Location { Id = 5, Name = "Location5", Wkt = "POINT(5 5)" },
        new Models.Location { Id = 6, Name = "Location6", Wkt = "POINT(6 6)" },
        new Models.Location { Id = 7, Name = "Location7", Wkt = "POINT(7 7)" },
        new Models.Location { Id = 8, Name = "Location8", Wkt = "POINT(8 8)" },
        new Models.Location { Id = 9, Name = "Location9", Wkt = "POINT(9 9)" },
        new Models.Location { Id = 10, Name = "Location10", Wkt = "POINT(10 10)" },
        new Models.Location { Id = 11, Name = "Location11", Wkt = "POINT(11 11)" },
        new Models.Location { Id = 12, Name = "Location12", Wkt = "POINT(12 12)" },
        new Models.Location { Id = 13, Name = "Location13", Wkt = "POINT(13 13)" },
        new Models.Location { Id = 14, Name = "Location14", Wkt = "POINT(14 14)" },
        new Models.Location { Id = 15, Name = "Location15", Wkt = "POINT(15 15)" }
    ];

    public async Task<Response<int>> AddLocation(AddLocationDto addLocationDto)
    {
        if (addLocationDto == null)
            return Response<int>.Fail("Location cannot be null", HttpStatusCode.BadRequest);

        int newId = locations.Max(x => x.Id) + 1;

        // Map DTO to Entity
        var newLocation = new Models.Location
        {
            Id = newId,
            Name = addLocationDto.Name,
            Wkt = addLocationDto.Wkt
        };

        locations = locations.Append(newLocation).ToArray();

        await Task.Delay(100); // Simulate async operation

        return Response<int>.Success(newLocation.Id, "Location added successfully.");
    }

    public async Task<Response<int[]>> AddLocations(AddLocationDto[] addLocationDtos)
    {
        if (addLocationDtos.Length == 0 || addLocationDtos == null)
            return Response<int[]>.Fail("No locations to add.", HttpStatusCode.NotFound);

        if (addLocationDtos.Length > 25)
            return Response<int[]>.Fail("Cannot add more than 25 locations at once.", HttpStatusCode.BadRequest);

        int[] newLocationIds = [];

        foreach (var newLocationDto in addLocationDtos)
        {
            int newId = locations.Max(x => x.Id) + 1;

            // Map DTO to Entity
            var newLocation = new Models.Location
            {
                Id = newId,
                Name = newLocationDto.Name,
                Wkt = newLocationDto.Wkt
            };

            locations = locations.Append(newLocation).ToArray();
            newLocationIds = newLocationIds.Append(newId).ToArray();
        }

        await Task.Delay(100); // Simulate async operation

        return Response<int[]>.Success(newLocationIds, "Locations added successfully.");
    }

    public async Task<Response<Models.Location[]>> GetLocations(int? pageNumber, int? pageSize)
    {
        if (locations == null || locations.Length == 0)
            return Response<Models.Location[]>.Fail("No locations found.", HttpStatusCode.NotFound);

        if (pageNumber.HasValue && pageSize.HasValue)
        {
            if (pageNumber < 1 || pageSize < 1)
                return Response<Models.Location[]>.Fail("Page number and size must be greater than zero.", HttpStatusCode.BadRequest);

            Models.Location[] paginatedLocations = locations
                .Skip(pageSize.Value * (pageNumber.Value - 1))
                .Take(pageSize.Value)
                .ToArray();

            if (paginatedLocations.Length == 0)
                return Response<Models.Location[]>.Fail("No locations found for the specified page.", HttpStatusCode.NotFound);

            return Response<Models.Location[]>.Success(paginatedLocations, "Locations retrieved successfully.");
        }

        await Task.Delay(100); // Simulate async operation

        return Response<Models.Location[]>.Success(locations, "Locations retrieved successfully.");
    }

    public async Task<Response<Models.Location>> GetLocationById(int id)
    {
        if (id <= 0)
            return Response<Models.Location>.Fail("Location ID must be greater than zero.", HttpStatusCode.BadRequest);

        var location = locations.FirstOrDefault(x => x.Id == id);

        if (location == null)
            return Response<Models.Location>.Fail("Location not found.");

        await Task.Delay(100); // Simulate async operation

        return Response<Models.Location>.Success(location, "Location retrieved successfully.");
    }

    public async Task<Response<Models.Location>> UpdateLocation(int id, UpdateLocationDto updateLocationDto)
    {
        if (id <= 0)
            return Response<Models.Location>.Fail("Location ID must be greater than zero.", HttpStatusCode.BadRequest);

        if (updateLocationDto == null)
            return Response<Models.Location>.Fail("Location cannot be null.");

        var existingLocation = locations.FirstOrDefault(x => x.Id == id);
        if (existingLocation == null)
            return Response<Models.Location>.Fail("Location not found.", HttpStatusCode.NotFound);

        existingLocation.Name = updateLocationDto.Name;
        existingLocation.Wkt = updateLocationDto.Wkt;

        locations = locations.Select(x => x.Id == id ? existingLocation : x).ToArray();

        await Task.Delay(100); // Simulate async operation

        return Response<Models.Location>.Success(existingLocation, "Location updated successfully.");
    }

    public async Task<Response<bool>> DeleteLocation(int id)
    {
        if (id <= 0)
            return Response<bool>.Fail("Location ID must be greater than zero.", HttpStatusCode.BadRequest);

        var location = locations.FirstOrDefault(x => x.Id == id);

        if (location == null)
            return Response<bool>.Fail("Location not found.", HttpStatusCode.NotFound);

        locations = locations.Where(x => x.Id != id).ToArray();

        await Task.Delay(100); // Simulate async operation

        return Response<bool>.Success(true, "Location deleted successfully.", HttpStatusCode.NoContent);
    }
}
