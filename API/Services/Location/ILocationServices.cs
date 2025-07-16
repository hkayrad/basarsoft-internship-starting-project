using System;
using API.Models.Location;

namespace API.Services;

public interface ILocationServices
{
    Task<Location[]> GetLocations();

    Task<Location> GetLocationById(int id);

    Task DeleteLocation(int id);

    Task AddLocation(AddLocationDto location);

    Task UpdateLocation(UpdateLocationDto location);

    Task AddLocations(AddLocationDto[] locations);
}
