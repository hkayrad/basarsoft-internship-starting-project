using System;
using System.Net;
using API.Helpers;
using API.Helpers.Resources;
using API.Models;
using API.Models.DTOs.Feature;
using Npgsql;

namespace API.Services.Feature;

public class PostgresqlAdonetFeatureServices : IFeatureServices
{
    private readonly string _featuresTableName = "features";
    private readonly NpgsqlDataSource _dataSource = DbHelper.DataSource!;
    public async Task<Response<int>> AddFeature(AddFeatureDto addFeatureDto)
    {
        if (addFeatureDto == null)
            return Response<int>.Fail(FeatureServicesResourceHelper.GetString("FeatureCannotBeNull"), HttpStatusCode.BadRequest);

        await using var command = _dataSource.CreateCommand($"INSERT INTO {_featuresTableName} (name, wkt) VALUES (@name, @wkt) RETURNING id");
        command.Parameters.AddWithValue("name", addFeatureDto.Name);
        command.Parameters.AddWithValue("wkt", addFeatureDto.Wkt);

        using var reader = await command.ExecuteReaderAsync();

        if (!reader.HasRows)
            return Response<int>.Fail(FeatureServicesResourceHelper.GetString("FailedToAddFeature"), HttpStatusCode.InternalServerError);

        await reader.ReadAsync();

        int newId = reader.GetInt32(reader.GetOrdinal("id"));

        return Response<int>.Success(newId, FeatureServicesResourceHelper.GetString("FeatureAddedSuccessfully"));
    }

    public async Task<Response<int[]>> AddFeatures(AddFeatureDto[] addFeatureDtos)
    {
        var newFeaturesIds = new List<int>();

        if (addFeatureDtos == null || addFeatureDtos.Length == 0)
            return Response<int[]>.Fail(FeatureServicesResourceHelper.GetString("NoFeaturesToAdd"), HttpStatusCode.NotFound);

        if (addFeatureDtos.Length > 25)
            return Response<int[]>.Fail(FeatureServicesResourceHelper.GetString("CannotAddMoreThan25Features"), HttpStatusCode.BadRequest);

        await using var conn = await _dataSource.OpenConnectionAsync();
        await using var transaction = await conn.BeginTransactionAsync();

        try
        {
            using var batch = conn.CreateBatch();
            batch.Transaction = transaction;

            foreach (var featureDto in addFeatureDtos)
            {
                var command = new NpgsqlBatchCommand
                {
                    CommandText = $"INSERT INTO {_featuresTableName} (name, wkt) VALUES (@name, @wkt) RETURNING id"
                };
                command.Parameters.AddWithValue("name", featureDto.Name);
                command.Parameters.AddWithValue("wkt", featureDto.Wkt);
                batch.BatchCommands.Add(command);
            }

            await using (var reader = await batch.ExecuteReaderAsync())
            {
                if (!reader.HasRows)
                {
                    await transaction.RollbackAsync();
                    return Response<int[]>.Fail(FeatureServicesResourceHelper.GetString("FailedToAddFeatures"), HttpStatusCode.InternalServerError);
                }

                do
                    while (await reader.ReadAsync())
                        newFeaturesIds = [.. newFeaturesIds, reader.GetInt32(reader.GetOrdinal("id"))];
                while (await reader.NextResultAsync());
            }

            await transaction.CommitAsync();
            return Response<int[]>.Success([.. newFeaturesIds], FeatureServicesResourceHelper.GetString("FeaturesAddedSuccessfully"));
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return Response<int[]>.Fail(FeatureServicesResourceHelper.GetString("FailedToAddFeatures", ex.Message), HttpStatusCode.InternalServerError);
        }
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

            await using var paginationCommand = _dataSource.CreateCommand($"SELECT * FROM {_featuresTableName} OFFSET @offset LIMIT @limit");
            paginationCommand.Parameters.AddWithValue("offset", pageSize.Value * (pageNumber.Value - 1));
            paginationCommand.Parameters.AddWithValue("limit", pageSize.Value);

            await using var paginationReader = await paginationCommand.ExecuteReaderAsync();

            if (!paginationReader.HasRows)
                return Response<Models.Feature[]>.Fail(FeatureServicesResourceHelper.GetString("NoFeaturesFoundForSpecifiedPage"), HttpStatusCode.NotFound);

            while (await paginationReader.ReadAsync())
            {
                Models.Feature feature = new()
                {
                    Id = paginationReader.GetInt32(paginationReader.GetOrdinal("id")),
                    Name = paginationReader.GetString(paginationReader.GetOrdinal("name")),
                    Wkt = paginationReader.GetString(paginationReader.GetOrdinal("wkt"))
                };
                features.Add(feature);
            }

            return Response<Models.Feature[]>.Success([.. features], FeatureServicesResourceHelper.GetString("FeaturesRetrievedSuccessfully"));
        }

        await using var command = _dataSource.CreateCommand($"SELECT * FROM {_featuresTableName}");

        await using var reader = await command.ExecuteReaderAsync();

        if (!reader.HasRows)
            return Response<Models.Feature[]>.Fail(FeatureServicesResourceHelper.GetString("NoFeaturesFound"), HttpStatusCode.NotFound);

        // Why don't we use while inside a do-while loop?
        while (await reader.ReadAsync())
        {
            Models.Feature feature = new()
            {
                Id = reader.GetInt32(reader.GetOrdinal("id")),
                Name = reader.GetString(reader.GetOrdinal("name")),
                Wkt = reader.GetString(reader.GetOrdinal("wkt"))
            };
            features.Add(feature);
        }

        return Response<Models.Feature[]>.Success([.. features], FeatureServicesResourceHelper.GetString("FeaturesRetrievedSuccessfully"));

    }

    public async Task<Response<Models.Feature>> GetFeatureById(int id)
    {
        if (id <= 0)
            return Response<Models.Feature>.Fail(FeatureServicesResourceHelper.GetString("FeatureIdMustBeGreaterThanZero"), HttpStatusCode.BadRequest);

        await using var command = _dataSource.CreateCommand($"SELECT * FROM {_featuresTableName} WHERE id = @id");
        command.Parameters.AddWithValue("id", id);

        await using var reader = await command.ExecuteReaderAsync();

        if (!reader.HasRows)
            return Response<Models.Feature>.Fail(FeatureServicesResourceHelper.GetString("FeatureNotFound"), HttpStatusCode.NotFound);

        await reader.ReadAsync();

        var feature = new Models.Feature
        {
            Id = reader.GetInt32(reader.GetOrdinal("id")),
            Name = reader.GetString(reader.GetOrdinal("name")),
            Wkt = reader.GetString(reader.GetOrdinal("wkt"))
        };

        return Response<Models.Feature>.Success(feature, FeatureServicesResourceHelper.GetString("FeatureRetrievedSuccessfully"));
    }

    public async Task<Response<Models.Feature>> UpdateFeature(int id, UpdateFeatureDto updateFeatureDto)
    {
        if (id <= 0)
            return Response<Models.Feature>.Fail(FeatureServicesResourceHelper.GetString("FeatureIdMustBeGreaterThanZero"), HttpStatusCode.BadRequest);

        if (updateFeatureDto == null)
            return Response<Models.Feature>.Fail(FeatureServicesResourceHelper.GetString("FeatureCannotBeNull"), HttpStatusCode.BadRequest);

        await using var command = _dataSource.CreateCommand($"UPDATE {_featuresTableName} SET name = @name, wkt = @wkt WHERE id = @id RETURNING id, name, wkt");
        command.Parameters.AddWithValue("id", id);
        command.Parameters.AddWithValue("name", updateFeatureDto.Name);
        command.Parameters.AddWithValue("wkt", updateFeatureDto.Wkt);

        await using var reader = await command.ExecuteReaderAsync();

        if (!reader.HasRows)
            return Response<Models.Feature>.Fail(FeatureServicesResourceHelper.GetString("FeatureNotFound"), HttpStatusCode.NotFound);

        await reader.ReadAsync();

        var feature = new Models.Feature
        {
            Id = reader.GetInt32(reader.GetOrdinal("id")),
            Name = reader.GetString(reader.GetOrdinal("name")),
            Wkt = reader.GetString(reader.GetOrdinal("wkt"))
        };

        return Response<Models.Feature>.Success(feature, FeatureServicesResourceHelper.GetString("FeatureUpdatedSuccessfully"));
    }

    public async Task<Response<bool>> DeleteFeature(int id)
    {
        if (id <= 0)
            return Response<bool>.Fail(FeatureServicesResourceHelper.GetString("FeatureIdMustBeGreaterThanZero"), HttpStatusCode.BadRequest);

        await using var command = _dataSource.CreateCommand($"DELETE FROM {_featuresTableName} WHERE id = @id");
        command.Parameters.AddWithValue("id", id);

        int affectedRows = await command.ExecuteNonQueryAsync();

        if (affectedRows == 0)
            return Response<bool>.Fail(FeatureServicesResourceHelper.GetString("FeatureNotFound"), HttpStatusCode.NotFound);

        return Response<bool>.Success(true, FeatureServicesResourceHelper.GetString("FeatureDeletedSuccessfully"), HttpStatusCode.NoContent);
    }
}
