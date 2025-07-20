using System;
using System.Net;
using API.Helpers;
using API.Models;
using API.Models.DTOs.Feature;
using Npgsql;

namespace API.Services.Feature;

public class PostgresqlFeatureServices : IFeatureServices
{
    private readonly string _featuresTableName = "features";
    private readonly NpgsqlDataSource _dataSource = DbHelper.DataSource!;
    public async Task<Response<int>> AddFeature(AddFeatureDto addFeatureDtp)
    {
        if (addFeatureDtp == null)
            return Response<int>.Fail("Feature cannot be null", HttpStatusCode.BadRequest);

        await using var command = _dataSource.CreateCommand($"INSERT INTO {_featuresTableName} (name, wkt) VALUES (@name, @wkt) RETURNING id");
        command.Parameters.AddWithValue("name", addFeatureDtp.Name);
        command.Parameters.AddWithValue("wkt", addFeatureDtp.Wkt);

        using var reader = await command.ExecuteReaderAsync();

        if (!reader.HasRows)
            return Response<int>.Fail("Failed to add feature.", HttpStatusCode.InternalServerError);

        await reader.ReadAsync();

        int newId = reader.GetInt32(0);

        return Response<int>.Success(newId, "Feature added successfully.");
    }

    public async Task<Response<int[]>> AddFeatures(AddFeatureDto[] addFeatureDtos)
    {
        int[] newFeaturesIds = [];

        if (addFeatureDtos.Length == 0 || addFeatureDtos == null)
            return Response<int[]>.Fail("No features to add.", HttpStatusCode.NotFound);

        if (addFeatureDtos.Length > 25)
            return Response<int[]>.Fail("Cannot add more than 25 features at once.", HttpStatusCode.BadRequest);

        using var batch = _dataSource.CreateBatch();

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

        await using var batchReader = await batch.ExecuteReaderAsync();

        if (!batchReader.HasRows)
            return Response<int[]>.Fail("Failed to add features.", HttpStatusCode.InternalServerError);

        // WTF
        do
            while (await batchReader.ReadAsync())
                newFeaturesIds = [.. newFeaturesIds, batchReader.GetInt16(0)];
        while (await batchReader.NextResultAsync());
        // ----

        return Response<int[]>.Success(newFeaturesIds, "Features added successfully.");
    }

    public async Task<Response<Models.Feature[]>> GetFeatures(int? pageNumber, int? pageSize)
    {
        var features = new List<Models.Feature>();

        if (pageNumber.HasValue || pageSize.HasValue)
        {
            // Both of them should be available together
            if (!pageNumber.HasValue || !pageSize.HasValue)
                return Response<Models.Feature[]>.Fail("Page number and size must be provided together.", HttpStatusCode.BadRequest);

            if (pageNumber < 1 || pageSize < 1)
                return Response<Models.Feature[]>.Fail("Page number and size must be greater than zero.", HttpStatusCode.BadRequest);

            await using var paginationCommand = _dataSource.CreateCommand($"SELECT * FROM {_featuresTableName} OFFSET @offset LIMIT @limit");
            paginationCommand.Parameters.AddWithValue("offset", pageSize.Value * (pageNumber.Value - 1));
            paginationCommand.Parameters.AddWithValue("limit", pageSize.Value);

            await using var paginationReader = await paginationCommand.ExecuteReaderAsync();

            if (!paginationReader.HasRows)
                return Response<Models.Feature[]>.Fail("No features found for the specified page.", HttpStatusCode.NotFound);

            while (await paginationReader.ReadAsync())
            {
                Models.Feature feature = new()
                {
                    Id = paginationReader.GetInt16(0),
                    Name = paginationReader.GetString(1),
                    Wkt = paginationReader.GetString(2)
                };
                features.Add(feature);
            }

            return Response<Models.Feature[]>.Success([.. features], "Features retrieved successfully.");
        }

        await using var command = _dataSource.CreateCommand($"SELECT * FROM {_featuresTableName}");

        await using var reader = await command.ExecuteReaderAsync();

        if (!reader.HasRows)
            return Response<Models.Feature[]>.Fail("No features found.", HttpStatusCode.NotFound);

        // Why don't we use while inside a do-while loop?
        while (await reader.ReadAsync())
        {
            Models.Feature feature = new()
            {
                Id = reader.GetInt16(0),
                Name = reader.GetString(1),
                Wkt = reader.GetString(2)
            };
            features.Add(feature);
        }

        return Response<Models.Feature[]>.Success([.. features], "Features retrieved successfully.");

    }

    public async Task<Response<Models.Feature>> GetFeatureById(int id)
    {
        if (id <= 0)
            return Response<Models.Feature>.Fail("Feature ID must be greater than zero.", HttpStatusCode.BadRequest);

        await using var command = _dataSource.CreateCommand($"SELECT * FROM {_featuresTableName} WHERE id = @id");
        command.Parameters.AddWithValue("id", id);

        await using var reader = await command.ExecuteReaderAsync();

        if (!reader.HasRows)
            return Response<Models.Feature>.Fail("Feature not found.", HttpStatusCode.NotFound);

        await reader.ReadAsync();

        var feature = new Models.Feature
        {
            Id = reader.GetInt16(0),
            Name = reader.GetString(1),
            Wkt = reader.GetString(2)
        };

        return Response<Models.Feature>.Success(feature, "Feature retrieved successfully.");
    }

    public async Task<Response<Models.Feature>> UpdateFeature(int id, UpdateFeatureDto updateFeatureDto)
    {
        if (id <= 0)
            return Response<Models.Feature>.Fail("Feature ID must be greater than zero.", HttpStatusCode.BadRequest);

        if (updateFeatureDto == null)
            return Response<Models.Feature>.Fail("Feature cannot be null.", HttpStatusCode.BadRequest);

        await using var command = _dataSource.CreateCommand($"UPDATE {_featuresTableName} SET name = @name, wkt = @wkt WHERE id = @id RETURNING id, name, wkt");
        command.Parameters.AddWithValue("id", id);
        command.Parameters.AddWithValue("name", updateFeatureDto.Name);
        command.Parameters.AddWithValue("wkt", updateFeatureDto.Wkt);

        await using var reader = await command.ExecuteReaderAsync();

        if (!reader.HasRows)
            return Response<Models.Feature>.Fail("Feature not found.", HttpStatusCode.NotFound);

        await reader.ReadAsync();

        var feature = new Models.Feature
        {
            Id = reader.GetInt16(0),
            Name = reader.GetString(1),
            Wkt = reader.GetString(2)
        };

        return Response<Models.Feature>.Success(feature, "Feature updated successfully.");
    }

    public async Task<Response<bool>> DeleteFeature(int id)
    {
        if (id <= 0)
            return Response<bool>.Fail("Feature ID must be greater than zero.", HttpStatusCode.BadRequest);

        await using var command = _dataSource.CreateCommand($"DELETE FROM {_featuresTableName} WHERE id = @id");
        command.Parameters.AddWithValue("id", id);

        int affectedRows = await command.ExecuteNonQueryAsync();

        if (affectedRows == 0)
            return Response<bool>.Fail("Feature not found.", HttpStatusCode.NotFound);

        return Response<bool>.Success(true, "Feature deleted successfully.", HttpStatusCode.NoContent);
    }
}
