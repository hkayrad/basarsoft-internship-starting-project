using System.Net;
using API.Models.Location;
using Microsoft.AspNetCore.Mvc;

namespace API.Services;

public class ArrayLocationServices : ILocationServices
{
    private Location[] locations = [
        new Location { Id = 1, Name = "Location1", Wkt = "POINT(1 1)" },
        new Location { Id = 2, Name = "Location2", Wkt = "POINT(2 2)" },
        new Location { Id = 3, Name = "Location3", Wkt = "POINT(3 3)" }
    ];

    public Response<Location[]> GetLocations(int? pageNumber)
    {
        const int pageSize = 10;

        if (locations == null || locations.Length == 0)
            return Response<Location[]>.Fail("No locations found.", HttpStatusCode.NotFound);

        if (pageNumber.HasValue)
        {
            Location[] paginatedLocations = locations
                .Skip(pageSize * (pageNumber.Value - 1))
                .Take(pageSize)
                .ToArray();

            if (paginatedLocations.Length == 0)
                return Response<Location[]>.Fail("No locations found for the specified page.", HttpStatusCode.NotFound);

            return Response<Location[]>.Success(paginatedLocations, "Locations retrieved successfully.");
        }

        return Response<Location[]>.Success(locations, "Locations retrieved successfully.");
    }

    public Response<Location> GetLocationById(int id)
    {
        if (id <= 0)
            return Response<Location>.Fail("Location ID must be greater than zero.", HttpStatusCode.BadRequest);

        var location = locations.FirstOrDefault(x => x.Id == id);

        if (location == null)
            return Response<Location>.Fail("Location not found.");

        return Response<Location>.Success(location, "Location retrieved successfully.");
    }

    public Response<bool> DeleteLocation(int id)
    {
        if (id <= 0)
            return Response<bool>.Fail("Location ID must be greater than zero.", HttpStatusCode.BadRequest);

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

        // if (addLocationDto.IsValid().Any(x => !x.IsValid))
        //     return Response<int>.Fail("Invalid location data.", HttpStatusCode.BadRequest);

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

        string?[] errorMessages = [];

        foreach (var newLocationDto in addLocationDtos)
        {
            // if (newLocationDto.IsValid().Any(x => !x.IsValid))
            // {
            //     foreach (var validationResult in newLocationDto.IsValid())
            //     {
            //         if (!validationResult.IsValid)
            //         {
            //             errorMessages = errorMessages.Append(validationResult.ErrorMessage).ToArray();
            //         }
            //     }
            //     continue;
            // }

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
        if (id <= 0)
            return Response<Location>.Fail("Location ID must be greater than zero.", HttpStatusCode.BadRequest);

        if (updateLocationDto == null)
            return Response<Location>.Fail("Location cannot be null.");

        // if (updateLocationDto.IsValid().Any(x => !x.IsValid))
        //     return Response<Location>.Fail("Invalid location data.", HttpStatusCode.BadRequest);

        var existingLocation = locations.FirstOrDefault(x => x.Id == id);
        if (existingLocation == null)
            return Response<Location>.Fail("Location not found.", HttpStatusCode.NotFound);

        existingLocation.Name = updateLocationDto.Name;
        existingLocation.Wkt = updateLocationDto.Wkt;

        locations = locations.Select(x => x.Id == id ? existingLocation : x).ToArray();

        return Response<Location>.Success(existingLocation, "Location updated successfully.");
    }
}
