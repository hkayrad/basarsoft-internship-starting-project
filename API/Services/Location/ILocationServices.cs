using System;
using API.Models.Location;

namespace API.Services;

public interface ILocationServices
{
    Response<Location[]> GetLocations();

    Response<Location> GetLocationById(int id);

    Response<int> AddLocation(AddLocationDto location);

    Response<int[]> AddLocations(AddLocationDto[] locations);

    Response<bool> DeleteLocation(int id);

    Response<Location> UpdateLocation(int id, UpdateLocationDto location);
}
