using System;
using System.Net;
using API.Helpers.Resources;
using API.Models;
using API.Models.Contexts;
using API.Models.DTOs.Feature;
using Microsoft.EntityFrameworkCore;

namespace API.Services.Feature;

public class PostgresqlEFFeatureService(MapInfoContext dbContext) : IFeatureService
{
    private readonly MapInfoContext _dbContext = dbContext;

    public async Task<Response<int>> AddFeature(AddFeatureDto addFeatureDto)
    {
        if (addFeatureDto == null)
            return Response<int>.Fail(FeatureServicesResourceHelper.GetString("FeatureCannotBeNull"), HttpStatusCode.BadRequest);

        var feature = new Models.Feature
        {
            Name = addFeatureDto.Name,
            Wkt = addFeatureDto.Wkt
        };

        _dbContext.Features.Add(feature);
        await _dbContext.SaveChangesAsync();

        return Response<int>.Success(feature.Id, FeatureServicesResourceHelper.GetString("FeatureAddedSuccessfully"));
    }

    public async Task<Response<int[]>> AddFeatures(AddFeatureDto[] addFeatureDtos)
    {
        if (addFeatureDtos == null || addFeatureDtos.Length == 0)
            return Response<int[]>.Fail(FeatureServicesResourceHelper.GetString("FeaturesCannotBeNullOrEmpty"), HttpStatusCode.BadRequest);

        if (addFeatureDtos.Length > 25)
            return Response<int[]>.Fail(FeatureServicesResourceHelper.GetString("BatchSizeLimitExceeded"), HttpStatusCode.BadRequest);

        using var transaction = await _dbContext.Database.BeginTransactionAsync();

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

            _dbContext.Features.AddRange(features);
            await _dbContext.SaveChangesAsync();

            await transaction.CommitAsync();

            return Response<int[]>.Success([.. features.Select(x => x.Id)], FeatureServicesResourceHelper.GetString("FeaturesAddedSuccessfully"));
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            return Response<int[]>.Fail(FeatureServicesResourceHelper.GetString("FailedToAddFeatures"), HttpStatusCode.InternalServerError);
        }
    }

    public async Task<Response<Models.Feature>> GetFeatureById(int id)
    {
        if (id <= 0)
            return Response<Models.Feature>.Fail(FeatureServicesResourceHelper.GetString("FeatureIdMustBeGreaterThanZero"), HttpStatusCode.BadRequest);

        var feature = await _dbContext.Features.FindAsync(id);

        if (feature == null)
            return Response<Models.Feature>.Fail(FeatureServicesResourceHelper.GetString("FeatureNotFound"), HttpStatusCode.NotFound);

        return Response<Models.Feature>.Success(feature, FeatureServicesResourceHelper.GetString("FeatureRetrievedSuccessfully"));
    }

    public async Task<Response<Models.Feature[]>> GetFeatures(int? pageNumber, int? pageSize)
    {
        var features = new List<Models.Feature>();

        if (pageNumber.HasValue || pageSize.HasValue)
        {
            // Both of them should be available together
            if (!pageNumber.HasValue || !pageSize.HasValue)
                return Response<Models.Feature[]>.Fail(FeatureServicesResourceHelper.GetString("PageNumberAndSizeMustBeProvidedTogether"), HttpStatusCode.BadRequest);

            if (pageNumber < 1 || pageSize < 1)
                return Response<Models.Feature[]>.Fail(FeatureServicesResourceHelper.GetString("PageNumberAndSizeMustBeGreaterThanZero"), HttpStatusCode.BadRequest);

            features = await _dbContext.Features
                .Skip(pageSize.Value * (pageNumber.Value - 1))
                .Take(pageSize.Value)
                .ToListAsync();

            return Response<Models.Feature[]>.Success([.. features], FeatureServicesResourceHelper.GetString("FeaturesRetrievedSuccessfully"));
        }

        features = await _dbContext.Features.ToListAsync();

        return Response<Models.Feature[]>.Success([.. features], FeatureServicesResourceHelper.GetString("FeaturesRetrievedSuccessfully"));
    }

    public async Task<Response<Models.Feature>> UpdateFeature(int id, UpdateFeatureDto updateFeatureDto)
    {
        if (id <= 0)
            return Response<Models.Feature>.Fail(FeatureServicesResourceHelper.GetString("FeatureIdMustBeGreaterThanZero"), HttpStatusCode.BadRequest);

        if (updateFeatureDto == null)
            return Response<Models.Feature>.Fail(FeatureServicesResourceHelper.GetString("FeatureCannotBeNull"), HttpStatusCode.BadRequest);

        var existingFeature = await _dbContext.Features.FindAsync(id);
        if (existingFeature == null)
            return Response<Models.Feature>.Fail(FeatureServicesResourceHelper.GetString("FeatureNotFound"), HttpStatusCode.NotFound);

        existingFeature.Name = updateFeatureDto.Name;
        existingFeature.Wkt = updateFeatureDto.Wkt;

        _dbContext.Features.Update(existingFeature);
        await _dbContext.SaveChangesAsync();

        return Response<Models.Feature>.Success(existingFeature, FeatureServicesResourceHelper.GetString("FeatureUpdatedSuccessfully"));
    }

    public async Task<Response<bool>> DeleteFeature(int id)
    {
        if (id <= 0)
            return Response<bool>.Fail(FeatureServicesResourceHelper.GetString("FeatureIdMustBeGreaterThanZero"), HttpStatusCode.BadRequest);

        var feature = await _dbContext.Features.FindAsync(id);
        if (feature == null)
            return Response<bool>.Fail(FeatureServicesResourceHelper.GetString("FeatureNotFound"), HttpStatusCode.NotFound);

        _dbContext.Features.Remove(feature);
        await _dbContext.SaveChangesAsync();

        return Response<bool>.Success(true, FeatureServicesResourceHelper.GetString("FeatureDeletedSuccessfully"));
    }
}
