using System;
using System.Net;
using API.Helpers.Resources;
using API.Models;
using API.Models.DTOs.Feature;
using API.Repositories.RP;
using Npgsql;

namespace API.Services.Feature;

public class PostgresqlRepositoryService(IFeatureRepository featureRepository) : IFeatureService
{
    private readonly IFeatureRepository _featureRepository = featureRepository;

    public async Task<Response<int>> AddFeature(AddFeatureDto addFeatureDto)
    {
        if (addFeatureDto == null)
            return Response<int>.Fail(FeatureServicesResourceHelper.GetString("FeatureCannotBeNull"), HttpStatusCode.BadRequest);

        var feature = new Models.Feature
        {
            Name = addFeatureDto.Name,
            Wkt = addFeatureDto.Wkt
        };

        try
        {
            var addedFeatureId = await _featureRepository.AddAsync(feature);
            return Response<int>.Success(addedFeatureId, FeatureServicesResourceHelper.GetString("FeatureAddedSuccessfully"));
        }
        catch (NpgsqlException ex)
        {
            return Response<int>.Fail(FeatureServicesResourceHelper.GetString("ErrorAddingFeature", ex.Message), HttpStatusCode.InternalServerError);
        }
    }

    public async Task<Response<int[]>> AddFeatures(AddFeatureDto[] addFeatureDtos)
    {
        if (addFeatureDtos == null || addFeatureDtos.Length == 0)
            return Response<int[]>.Fail(FeatureServicesResourceHelper.GetString("FeaturesCannotBeNullOrEmpty"), HttpStatusCode.BadRequest);

        if (addFeatureDtos.Length > 25)
            return Response<int[]>.Fail(FeatureServicesResourceHelper.GetString("BatchSizeLimitExceeded"), HttpStatusCode.BadRequest);

        try
        {
            var features = new List<Models.Feature>();
            foreach (var dto in addFeatureDtos)
            {
                var feature = new Models.Feature
                {
                    Name = dto.Name,
                    Wkt = dto.Wkt
                };
                features.Add(feature);
            }

            var addedFeatureIds = await _featureRepository.AddRangeAsync([.. features]);

            return Response<int[]>.Success(addedFeatureIds, FeatureServicesResourceHelper.GetString("FeaturesAddedSuccessfully"));
        }
        catch (Exception)
        {
            return Response<int[]>.Fail(FeatureServicesResourceHelper.GetString("FailedToAddFeatures"), HttpStatusCode.InternalServerError);
        }
    }

    public async Task<Response<Models.Feature>> GetFeatureById(int id)
    {
        if (id <= 0)
            return Response<Models.Feature>.Fail(FeatureServicesResourceHelper.GetString("FeatureIdMustBeGreaterThanZero"), HttpStatusCode.BadRequest);

        try
        {
            var feature = await _featureRepository.GetByIdAsync(id);

            if (feature == null)
                return Response<Models.Feature>.Fail(FeatureServicesResourceHelper.GetString("FeatureNotFound"), HttpStatusCode.NotFound);

            return Response<Models.Feature>.Success(feature, FeatureServicesResourceHelper.GetString("FeatureRetrievedSuccessfully"));
        }
        catch (NpgsqlException ex)
        {
            return Response<Models.Feature>.Fail(FeatureServicesResourceHelper.GetString("ErrorRetrievingFeature", ex.Message), HttpStatusCode.InternalServerError);
        }
    }

    public async Task<Response<Models.Feature[]>> GetFeatures(int? pageNumber, int? pageSize)
    {
        List<Models.Feature>? features;

        try
        {
            if (pageNumber.HasValue || pageSize.HasValue)
            {
                // Both of them should be available together
                if (!pageNumber.HasValue || !pageSize.HasValue)
                    return Response<Models.Feature[]>.Fail(FeatureServicesResourceHelper.GetString("PageNumberAndSizeMustBeProvidedTogether"), HttpStatusCode.BadRequest);

                if (pageNumber < 1 || pageSize < 1)
                    return Response<Models.Feature[]>.Fail(FeatureServicesResourceHelper.GetString("PageNumberAndSizeMustBeGreaterThanZero"), HttpStatusCode.BadRequest);

                features = await _featureRepository.GetPagedAsync(pageNumber.Value, pageSize.Value);

                return Response<Models.Feature[]>.Success([.. features], FeatureServicesResourceHelper.GetString("FeaturesRetrievedSuccessfully"));
            }

            features = await _featureRepository.GetAllAsync();

            return Response<Models.Feature[]>.Success([.. features], FeatureServicesResourceHelper.GetString("FeaturesRetrievedSuccessfully"));
        }
        catch (NpgsqlException ex)
        {
            return Response<Models.Feature[]>.Fail(FeatureServicesResourceHelper.GetString("ErrorRetrievingFeatures", ex.Message), HttpStatusCode.InternalServerError);
        }
    }

    public async Task<Response<Models.Feature>> UpdateFeature(int id, UpdateFeatureDto updateFeatureDto)
    {
        if (id <= 0)
            return Response<Models.Feature>.Fail(FeatureServicesResourceHelper.GetString("FeatureIdMustBeGreaterThanZero"), HttpStatusCode.BadRequest);

        if (updateFeatureDto == null)
            return Response<Models.Feature>.Fail(FeatureServicesResourceHelper.GetString("FeatureCannotBeNull"), HttpStatusCode.BadRequest);

        try
        {
            var existingFeature = await _featureRepository.GetByIdAsync(id);
            if (existingFeature == null)
                return Response<Models.Feature>.Fail(FeatureServicesResourceHelper.GetString("FeatureNotFound"), HttpStatusCode.NotFound);

            existingFeature.Name = updateFeatureDto.Name;
            existingFeature.Wkt = updateFeatureDto.Wkt;

            await _featureRepository.UpdateAsync(existingFeature);

            return Response<Models.Feature>.Success(existingFeature, FeatureServicesResourceHelper.GetString("FeatureUpdatedSuccessfully"));
        }
        catch (NpgsqlException ex)
        {
            return Response<Models.Feature>.Fail(FeatureServicesResourceHelper.GetString("ErrorUpdatingFeature", ex.Message), HttpStatusCode.InternalServerError);
        }
    }

    public async Task<Response<bool>> DeleteFeature(int id)
    {
        if (id <= 0)
            return Response<bool>.Fail(FeatureServicesResourceHelper.GetString("FeatureIdMustBeGreaterThanZero"), HttpStatusCode.BadRequest);

        try
        {
            var feature = await _featureRepository.GetByIdAsync(id);
            if (feature == null)
                return Response<bool>.Fail(FeatureServicesResourceHelper.GetString("FeatureNotFound"), HttpStatusCode.NotFound);

            await _featureRepository.DeleteAsync(id);

            return Response<bool>.Success(true, FeatureServicesResourceHelper.GetString("FeatureDeletedSuccessfully"));
        }
        catch (NpgsqlException ex)
        {
            return Response<bool>.Fail(FeatureServicesResourceHelper.GetString("ErrorDeletingFeature", ex.Message), HttpStatusCode.InternalServerError);
        }
    }
}
