using System;
using System.Net;
using API.Helpers.Resources;
using API.Models;
using API.Models.Contexts;
using API.Models.DTOs.Feature;
using Microsoft.EntityFrameworkCore;

namespace API.Services.Feature;

public class PostgresqlEFFeatureServices(FeatureContext context) : IFeatureServices
{
    private readonly FeatureContext _context = context;

    public async Task<Response<int>> AddFeature(AddFeatureDto addFeatureDto)
    {
        if (addFeatureDto == null)
            return Response<int>.Fail(FeatureServicesResourceHelper.GetString("FeatureCannotBeNull"), HttpStatusCode.BadRequest);

        var feature = new Models.Feature
        {
            Name = addFeatureDto.Name,
            Wkt = addFeatureDto.Wkt
        };

        _context.Features.Add(feature);
        await _context.SaveChangesAsync();

        return Response<int>.Success(feature.Id, FeatureServicesResourceHelper.GetString("FeatureAddedSuccessfully"));
    }

    public async Task<Response<int[]>> AddFeatures(AddFeatureDto[] addFeatureDtos)
    {
        if (addFeatureDtos == null || addFeatureDtos.Length == 0)
            return Response<int[]>.Fail(FeatureServicesResourceHelper.GetString("FeaturesCannotBeNullOrEmpty"), HttpStatusCode.BadRequest);

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

        _context.Features.AddRange(features);
        await _context.SaveChangesAsync();

        return Response<int[]>.Success([.. features.Select(f => f.Id)], FeatureServicesResourceHelper.GetString("FeaturesAddedSuccessfully"));
    }

    public async Task<Response<Models.Feature>> GetFeatureById(int id)
    {
        if (id <= 0)
            return Response<Models.Feature>.Fail(FeatureServicesResourceHelper.GetString("FeatureIdMustBeGreaterThanZero"), HttpStatusCode.BadRequest);

        var feature = await _context.Features.FindAsync(id);

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

            features = await _context.Features
                .Skip(pageSize.Value * (pageNumber.Value - 1))
                .Take(pageSize.Value)
                .ToListAsync();

            return Response<Models.Feature[]>.Success([.. features], FeatureServicesResourceHelper.GetString("FeaturesRetrievedSuccessfully"));
        }

        features = await _context.Features.ToListAsync();

        return Response<Models.Feature[]>.Success([.. features], FeatureServicesResourceHelper.GetString("FeaturesRetrievedSuccessfully"));
    }

    public async Task<Response<Models.Feature>> UpdateFeature(int id, UpdateFeatureDto updateFeatureDto)
    {
        if (id <= 0)
            return Response<Models.Feature>.Fail(FeatureServicesResourceHelper.GetString("FeatureIdMustBeGreaterThanZero"), HttpStatusCode.BadRequest);

        if (updateFeatureDto == null)
            return Response<Models.Feature>.Fail(FeatureServicesResourceHelper.GetString("FeatureCannotBeNull"), HttpStatusCode.BadRequest);

        var existingFeature = await _context.Features.FindAsync(id);
        if (existingFeature == null)
            return Response<Models.Feature>.Fail(FeatureServicesResourceHelper.GetString("FeatureNotFound"), HttpStatusCode.NotFound);

        existingFeature.Name = updateFeatureDto.Name;
        existingFeature.Wkt = updateFeatureDto.Wkt;

        _context.Features.Update(existingFeature);
        await _context.SaveChangesAsync();

        return Response<Models.Feature>.Success(existingFeature, FeatureServicesResourceHelper.GetString("FeatureUpdatedSuccessfully"));
    }

    public async Task<Response<bool>> DeleteFeature(int id)
    {
        if (id <= 0)
            return Response<bool>.Fail(FeatureServicesResourceHelper.GetString("FeatureIdMustBeGreaterThanZero"), HttpStatusCode.BadRequest);

        var feature = await _context.Features.FindAsync(id);
        if (feature == null)
            return Response<bool>.Fail(FeatureServicesResourceHelper.GetString("FeatureNotFound"), HttpStatusCode.NotFound);

        _context.Features.Remove(feature);
        await _context.SaveChangesAsync();

        return Response<bool>.Success(true, FeatureServicesResourceHelper.GetString("FeatureDeletedSuccessfully"));
    }
}
