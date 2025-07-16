using System;
using API.Models.Location;

namespace API.Services;

public class LocationServices : ILocationServices
{
    private Location[] locations = [
        new Location { Id = 1, Name = "Location1", Wkt = "POINT(1 1)" },
        new Location { Id = 2, Name = "Location2", Wkt = "POINT(2 2)" },
        new Location { Id = 3, Name = "Location3", Wkt = "POINT(3 3)" }
    ];

    public async Task<Location[]> GetLocations()
    {
        return await Task.FromResult(locations.ToArray()); //Task.FromResult is used to simulate async behavior
    }

    public async Task<Location> GetLocationById(int id)
    {
        var location = locations.FirstOrDefault(x => x.Id == id);

        if (location == null)
            throw new Exception("Location not found");

        return await Task.FromResult(location);
    }

    public async Task DeleteLocation(int id)
    {
        var location = locations.FirstOrDefault(x => x.Id == id);
        if (location == null)
            throw new Exception("Location not found");

        locations = locations.Where(x => x.Id != id).ToArray();

        await Task.CompletedTask; // Simulate async operation
    }

    public async Task AddLocation(AddLocationDto addLocationDto)
    {
        if (addLocationDto == null)
            throw new Exception("Location cannot be null");

        int newId = locations.Max(x => x.Id) + 1;

        // Map DTO to Entity
        var newLocation = new Location
        {
            Id = newId,
            Name = addLocationDto.Name,
            Wkt = addLocationDto.Wkt
        };

        locations = locations.Append(newLocation).ToArray();

        await Task.CompletedTask; // Simulate async operation
    }

    public async Task AddLocations(AddLocationDto[] addLocationDtos)
    {
        if (addLocationDtos.Length == 0 || addLocationDtos == null)
            throw new Exception("No locations provided");

        Location[] newLocations = [];

        foreach (var newLocationDto in addLocationDtos)
        {
            if (newLocationDto == null)
                throw new Exception("Location cannot be null");

            int newId = locations.Max(x => x.Id) + 1;

            // Map DTO to Entity
            var newLocation = new Location
            {
                Id = newId,
                Name = newLocationDto.Name,
                Wkt = newLocationDto.Wkt
            };

            locations = locations.Append(newLocation).ToArray();
        }

        await Task.CompletedTask; // Simulate async operation
    }


    public async Task UpdateLocation(UpdateLocationDto locationDto)
    {
        if (locationDto.Id <= 0)
            throw new Exception("Invalid location ID");

        if (locationDto == null)
            throw new Exception("Location cannot be null");

        var existingLocation = locations.FirstOrDefault(x => x.Id == locationDto.Id);
        if (existingLocation == null)
            throw new Exception("Location not found");

        existingLocation.Name = locationDto.Name;
        existingLocation.Wkt = locationDto.Wkt;

        locations = locations.Select(x => x.Id == locationDto.Id ? existingLocation : x).ToArray();

        await Task.CompletedTask; // Simulate async operation
    }
}
