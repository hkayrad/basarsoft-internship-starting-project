using System;
using API.Models;
using API.Models.DTOs.Feature;

namespace API.Services.Feature;

public interface IFeatureServices
{
    Task<Response<int>> AddFeature(AddFeatureDto feature);

    Task<Response<int[]>> AddFeatures(AddFeatureDto[] features);

    Task<Response<Models.Feature[]>> GetFeatures(int? pageNumber, int? pageSize);

    Task<Response<Models.Feature>> GetFeatureById(int id);

    Task<Response<Models.Feature>> UpdateFeature(int id, UpdateFeatureDto feature);

    Task<Response<bool>> DeleteFeature(int id);
}
