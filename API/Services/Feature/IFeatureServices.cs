using System;
using API.Models;
using API.Models.DTOs.Feature;

namespace API.Services.Feature;

public interface IFeatureService
{
    Task<Response<int>> AddFeature(AddFeatureDto addFeatureDto);

    Task<Response<int[]>> AddFeatures(AddFeatureDto[] addFeatureDtos);

    Task<Response<Models.Feature[]>> GetFeatures(int? pageNumber, int? pageSize);

    Task<Response<Models.Feature>> GetFeatureById(int id);

    Task<Response<Models.Feature>> UpdateFeature(int id, UpdateFeatureDto updateFeatureDto);

    Task<Response<bool>> DeleteFeature(int id);
}
