using System;
using System.Net;
using API.DAL;
using API.Helpers.Resources;
using API.Models;
using API.Models.DTOs.Feature;
using Npgsql;

namespace API.Services.Feature;

public class PostgresqlGRPService(IUnitOfWork unitOfWork) : IFeatureService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Response<int>> AddFeature(AddFeatureDto addFeatureDto)
    {
        if (addFeatureDto == null)
            return Response<int>.Fail(FeatureServicesResourceHelper.GetString("FeatureDataCannotBeNull"), HttpStatusCode.BadRequest);

        try
        {
            var feature = new Models.Feature
            {
                Name = addFeatureDto.Name,
                Wkt = addFeatureDto.Wkt
            };

            await _unitOfWork.FeatureRepository.AddAsync(feature);
            await _unitOfWork.SaveChangesAsync();

            return Response<int>.Success(feature.Id, FeatureServicesResourceHelper.GetString("FeatureAddedSuccessfully"));
        }
        catch (NpgsqlException ex)
        {
            return Response<int>.Fail($"Database error: {ex.Message}", HttpStatusCode.InternalServerError);
        }
    }

    public async Task<Response<int[]>> AddFeatures(AddFeatureDto[] addFeatureDtos)
    {
        if (addFeatureDtos == null || addFeatureDtos.Length == 0)
            return Response<int[]>.Fail(FeatureServicesResourceHelper.GetString("FeatureDataCannotBeNullOrEmpty"), HttpStatusCode.BadRequest);

        try
        {
            await _unitOfWork.BeginTransactionAsync();

            List<Models.Feature> features = new();

            foreach (var addFeatureDto in addFeatureDtos)
            {
                var feature = new Models.Feature
                {
                    Name = addFeatureDto.Name,
                    Wkt = addFeatureDto.Wkt
                };
                features.Add(feature);
            }

            await _unitOfWork.FeatureRepository.AddRangeAsync([.. features]);
            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();

            return Response<int[]>.Success([.. features.Select(x => x.Id)], FeatureServicesResourceHelper.GetString("FeaturesAddedSuccessfully"));

        }
        catch (NpgsqlException ex)
        {
            await _unitOfWork.RollbackTransactionAsync();
            return Response<int[]>.Fail(FeatureServicesResourceHelper.GetString("DatabaseError", ex.Message), HttpStatusCode.InternalServerError);
        }
    }

    public async Task<Response<Models.Feature>> GetFeatureById(int id)
    {
        if (id <= 0)
            return Response<Models.Feature>.Fail(FeatureServicesResourceHelper.GetString("FeatureIdMustBeGreaterThanZero"), HttpStatusCode.BadRequest);

        try
        {
            var feature = await _unitOfWork.FeatureRepository.GetByIdAsync(id);
            if (feature == null)
                return Response<Models.Feature>.Fail(FeatureServicesResourceHelper.GetString("FeatureNotFound"), HttpStatusCode.NotFound);

            return Response<Models.Feature>.Success(feature, FeatureServicesResourceHelper.GetString("FeatureRetrievedSuccessfully"));
        }
        catch (NpgsqlException ex)
        {
            return Response<Models.Feature>.Fail(FeatureServicesResourceHelper.GetString("DatabaseError", ex.Message), HttpStatusCode.InternalServerError);
        }
    }

    public async Task<Response<Models.Feature[]>> GetFeatures(int? pageNumber, int? pageSize)
    {
        if (pageNumber.HasValue || pageSize.HasValue)
        {
            if (!pageNumber.HasValue || !pageSize.HasValue)
                return Response<Models.Feature[]>.Fail(FeatureServicesResourceHelper.GetString("PageNumberAndSizeMustBeProvidedTogether"), HttpStatusCode.BadRequest);

            if (pageNumber <= 0 || pageSize <= 0)
                return Response<Models.Feature[]>.Fail(FeatureServicesResourceHelper.GetString("PageNumberAndSizeMustBeGreaterThanZero"), HttpStatusCode.BadRequest);

            try
            {
                var features = await _unitOfWork.FeatureRepository.GetPagedAsync(pageNumber.Value, pageSize.Value);
                return Response<Models.Feature[]>.Success([.. features], FeatureServicesResourceHelper.GetString("FeaturesRetrievedSuccessfully"), HttpStatusCode.OK);
            }
            catch (NpgsqlException ex)
            {
                return Response<Models.Feature[]>.Fail(FeatureServicesResourceHelper.GetString("DatabaseError", ex.Message), HttpStatusCode.InternalServerError);
            }

        }

        try
        {
            var features = await _unitOfWork.FeatureRepository.GetAllAsync();
            return Response<Models.Feature[]>.Success([.. features], FeatureServicesResourceHelper.GetString("FeaturesRetrievedSuccessfully"));
        }
        catch (NpgsqlException ex)
        {
            return Response<Models.Feature[]>.Fail(FeatureServicesResourceHelper.GetString("DatabaseError", ex.Message), HttpStatusCode.InternalServerError);
        }
    }

    public async Task<Response<Models.Feature>> UpdateFeature(int id, UpdateFeatureDto updateFeatureDto)
    {
        if (id <= 0)
            return Response<Models.Feature>.Fail(FeatureServicesResourceHelper.GetString("FeatureIdMustBeGreaterThanZero"), HttpStatusCode.BadRequest);

        if (updateFeatureDto == null)
            return Response<Models.Feature>.Fail(FeatureServicesResourceHelper.GetString("FeatureDataCannotBeNull"), HttpStatusCode.BadRequest);

        try
        {
            var feature = await _unitOfWork.FeatureRepository.GetByIdAsync(id);
            if (feature == null)
                return Response<Models.Feature>.Fail(FeatureServicesResourceHelper.GetString("FeatureNotFound"), HttpStatusCode.NotFound);

            feature.Name = updateFeatureDto.Name;
            feature.Wkt = updateFeatureDto.Wkt;

            await _unitOfWork.FeatureRepository.UpdateAsync(feature);
            await _unitOfWork.SaveChangesAsync();

            return Response<Models.Feature>.Success(feature, FeatureServicesResourceHelper.GetString("FeatureUpdatedSuccessfully"), HttpStatusCode.OK);
        }
        catch (NpgsqlException ex)
        {
            return Response<Models.Feature>.Fail(FeatureServicesResourceHelper.GetString("DatabaseError", ex.Message), HttpStatusCode.InternalServerError);
        }
    }

    public async Task<Response<bool>> DeleteFeature(int id)
    {
        if (id <= 0)
            return Response<bool>.Fail(FeatureServicesResourceHelper.GetString("FeatureIdMustBeGreaterThanZero"), HttpStatusCode.BadRequest);

        try
        {
            var feature = await _unitOfWork.FeatureRepository.GetByIdAsync(id);
            if (feature == null)
                return Response<bool>.Fail(FeatureServicesResourceHelper.GetString("FeatureNotFound"), HttpStatusCode.NotFound);

            await _unitOfWork.FeatureRepository.DeleteAsync(id);
            await _unitOfWork.SaveChangesAsync();

            return Response<bool>.Success(true, FeatureServicesResourceHelper.GetString("FeatureDeletedSuccessfully"), HttpStatusCode.OK);
        }
        catch (NpgsqlException ex)
        {
            return Response<bool>.Fail(FeatureServicesResourceHelper.GetString("DatabaseError", ex.Message), HttpStatusCode.InternalServerError);
        }
    }
}
