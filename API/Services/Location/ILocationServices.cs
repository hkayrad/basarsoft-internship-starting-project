using System;
using API.Models;
using API.Models.DTOs.Location;

namespace API.Services.Location;

public interface ILocationServices
{
    Task<Response<int>> AddLocation(AddLocationDto location);

    Task<Response<int[]>> AddLocations(AddLocationDto[] locations);

    Task<Response<Models.Location[]>> GetLocations(int? pageNumber, int? pageSize);

    Task<Response<Models.Location>> GetLocationById(int id);

    Task<Response<Models.Location>> UpdateLocation(int id, UpdateLocationDto location);

    Task<Response<bool>> DeleteLocation(int id);
}
