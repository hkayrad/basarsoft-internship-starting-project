using System;
using System.Net;
using API.Helpers;
using API.Models;
using API.Models.DTOs.Location;
using Npgsql;

namespace API.Services.Location;

public class PostgresqlLocationServices : ILocationServices
{
    private readonly NpgsqlDataSource _dataSource = DbHelper.DataSource!;
    public async Task<Response<int>> AddLocation(AddLocationDto addLocationDto)
    {
        if (addLocationDto == null)
            return Response<int>.Fail("Location cannot be null", HttpStatusCode.BadRequest);

        await using var command = _dataSource.CreateCommand("INSERT INTO locations (name, wkt) VALUES (@name, @wkt) RETURNING id");
        command.Parameters.AddWithValue("name", addLocationDto.Name);
        command.Parameters.AddWithValue("wkt", addLocationDto.Wkt);

        using var reader = await command.ExecuteReaderAsync();

        if (!reader.HasRows)
            return Response<int>.Fail("Failed to add location.", HttpStatusCode.InternalServerError);

        await reader.ReadAsync();

        int newId = reader.GetInt32(0);

        return Response<int>.Success(newId, "Location added successfully.");
    }

    public async Task<Response<int[]>> AddLocations(AddLocationDto[] addLocationDtos)
    {
        int[] newLocationIds = [];

        if (addLocationDtos.Length == 0 || addLocationDtos == null)
            return Response<int[]>.Fail("No locations to add.", HttpStatusCode.NotFound);

        if (addLocationDtos.Length > 25)
            return Response<int[]>.Fail("Cannot add more than 25 locations at once.", HttpStatusCode.BadRequest);

        using var batch = _dataSource.CreateBatch();

        foreach (var locationDto in addLocationDtos)
        {
            var command = new NpgsqlBatchCommand
            {
                CommandText = "INSERT INTO locations (name, wkt) VALUES (@name, @wkt) RETURNING id"
            };
            command.Parameters.AddWithValue("name", locationDto.Name);
            command.Parameters.AddWithValue("wkt", locationDto.Wkt);
            batch.BatchCommands.Add(command);
        }

        await using var batchReader = await batch.ExecuteReaderAsync();

        if (!batchReader.HasRows)
            return Response<int[]>.Fail("Failed to add locations.", HttpStatusCode.InternalServerError);

        // WTF
        do
            while (await batchReader.ReadAsync())
                newLocationIds = [.. newLocationIds, batchReader.GetInt16(0)];
        while (await batchReader.NextResultAsync());
        // ----

        return Response<int[]>.Success(newLocationIds, "Locations added successfully.");
    }

    public async Task<Response<Models.Location[]>> GetLocations(int? pageNumber, int? pageSize)
    {
        var locations = new List<Models.Location>();

        if (pageNumber.HasValue || pageSize.HasValue)
        {
            // Both of them should be available together
            if (!pageNumber.HasValue || !pageSize.HasValue)
                return Response<Models.Location[]>.Fail("Page number and size must be provided together.", HttpStatusCode.BadRequest);

            if (pageNumber < 1 || pageSize < 1)
                return Response<Models.Location[]>.Fail("Page number and size must be greater than zero.", HttpStatusCode.BadRequest);

            await using var paginationCommand = _dataSource.CreateCommand("SELECT * FROM locations OFFSET @offset LIMIT @limit");
            paginationCommand.Parameters.AddWithValue("offset", pageSize.Value * (pageNumber.Value - 1));
            paginationCommand.Parameters.AddWithValue("limit", pageSize.Value);

            await using var paginationReader = await paginationCommand.ExecuteReaderAsync();

            if (!paginationReader.HasRows)
                return Response<Models.Location[]>.Fail("No locations found for the specified page.", HttpStatusCode.NotFound);

            while (await paginationReader.ReadAsync())
            {
                Models.Location location = new()
                {
                    Id = paginationReader.GetInt16(0),
                    Name = paginationReader.GetString(1),
                    Wkt = paginationReader.GetString(2)
                };
                locations.Add(location);
            }

            return Response<Models.Location[]>.Success([.. locations], "Locations retrieved successfully.");
        }

        await using var command = _dataSource.CreateCommand("SELECT * FROM locations");

        await using var reader = await command.ExecuteReaderAsync();

        if (!reader.HasRows)
            return Response<Models.Location[]>.Fail("No locations found.", HttpStatusCode.NotFound);

        // Why don't we use while inside a do-while loop?
        while (await reader.ReadAsync())
        {
            Models.Location location = new()
            {
                Id = reader.GetInt16(0),
                Name = reader.GetString(1),
                Wkt = reader.GetString(2)
            };
            locations.Add(location);
        }

        return Response<Models.Location[]>.Success([.. locations], "Locations retrieved successfully.");

    }

    public async Task<Response<Models.Location>> GetLocationById(int id)
    {
        if (id <= 0)
            return Response<Models.Location>.Fail("Location ID must be greater than zero.", HttpStatusCode.BadRequest);

        await using var command = _dataSource.CreateCommand("SELECT * FROM locations WHERE id = @id");
        command.Parameters.AddWithValue("id", id);

        await using var reader = await command.ExecuteReaderAsync();

        if (!reader.HasRows)
            return Response<Models.Location>.Fail("Location not found.", HttpStatusCode.NotFound);

        await reader.ReadAsync();

        var location = new Models.Location
        {
            Id = reader.GetInt16(0),
            Name = reader.GetString(1),
            Wkt = reader.GetString(2)
        };

        return Response<Models.Location>.Success(location, "Location retrieved successfully.");
    }

    public async Task<Response<Models.Location>> UpdateLocation(int id, UpdateLocationDto updateLocationDto)
    {
        if (id <= 0)
            return Response<Models.Location>.Fail("Location ID must be greater than zero.", HttpStatusCode.BadRequest);

        if (updateLocationDto == null)
            return Response<Models.Location>.Fail("Location cannot be null.", HttpStatusCode.BadRequest);

        await using var command = _dataSource.CreateCommand("UPDATE locations SET name = @name, wkt = @wkt WHERE id = @id RETURNING id, name, wkt");
        command.Parameters.AddWithValue("id", id);
        command.Parameters.AddWithValue("name", updateLocationDto.Name);
        command.Parameters.AddWithValue("wkt", updateLocationDto.Wkt);

        await using var reader = await command.ExecuteReaderAsync();

        if (!reader.HasRows)
            return Response<Models.Location>.Fail("Location not found.", HttpStatusCode.NotFound);

        await reader.ReadAsync();

        var location = new Models.Location
        {
            Id = reader.GetInt16(0),
            Name = reader.GetString(1),
            Wkt = reader.GetString(2)
        };

        return Response<Models.Location>.Success(location, "Location updated successfully.");
    }

    public async Task<Response<bool>> DeleteLocation(int id)
    {
        if (id <= 0)
            return Response<bool>.Fail("Location ID must be greater than zero.", HttpStatusCode.BadRequest);

        await using var command = _dataSource.CreateCommand("DELETE FROM locations WHERE id = @id");
        command.Parameters.AddWithValue("id", id);

        int affectedRows = await command.ExecuteNonQueryAsync();

        if (affectedRows == 0)
            return Response<bool>.Fail("Location not found.", HttpStatusCode.NotFound);

        return Response<bool>.Success(true, "Location deleted successfully.", HttpStatusCode.NoContent);
    }
}
