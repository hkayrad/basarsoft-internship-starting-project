using System.Net;
using API.Models;
using API.Models.DTOs.Feature;

namespace API.Services.Feature;

public class ArrayFeatureService : IFeatureService
{
    private Models.Feature[] features = [
        new Models.Feature { Id = 1, Name = "Feature1", Wkt = "POINT(1 1)" },
        new Models.Feature { Id = 2, Name = "Feature2", Wkt = "POINT(2 2)" },
        new Models.Feature { Id = 3, Name = "Feature3", Wkt = "POINT(3 3)" },
        new Models.Feature { Id = 4, Name = "Feature4", Wkt = "POINT(4 4)" },
        new Models.Feature { Id = 5, Name = "Feature5", Wkt = "POINT(5 5)" },
        new Models.Feature { Id = 6, Name = "Feature6", Wkt = "POINT(6 6)" },
        new Models.Feature { Id = 7, Name = "Feature7", Wkt = "POINT(7 7)" },
        new Models.Feature { Id = 8, Name = "Feature8", Wkt = "POINT(8 8)" },
        new Models.Feature { Id = 9, Name = "Feature9", Wkt = "POINT(9 9)" },
        new Models.Feature { Id = 10, Name = "Feature10", Wkt = "POINT(10 10)" },
        new Models.Feature { Id = 11, Name = "Feature11", Wkt = "POINT(11 11)" },
        new Models.Feature { Id = 12, Name = "Feature12", Wkt = "POINT(12 12)" },
        new Models.Feature { Id = 13, Name = "Feature13", Wkt = "POINT(13 13)" },
        new Models.Feature { Id = 14, Name = "Feature14", Wkt = "POINT(14 14)" },
        new Models.Feature { Id = 15, Name = "Feature15", Wkt = "POINT(15 15)" }
    ];

    public async Task<Response<int>> AddFeature(AddFeatureDto addFeatureDto)
    {
        if (addFeatureDto == null)
            return Response<int>.Fail("Feature cannot be null", HttpStatusCode.BadRequest);

        int newId = features.Max(x => x.Id) + 1;

        // Map DTO to Entity
        var newFeature = new Models.Feature
        {
            Id = newId,
            Name = addFeatureDto.Name,
            Wkt = addFeatureDto.Wkt
        };

        features = features.Append(newFeature).ToArray();

        await Task.Delay(100); // Simulate async operation

        return Response<int>.Success(newFeature.Id, "Feature added successfully.");
    }

    public async Task<Response<int[]>> AddFeatures(AddFeatureDto[] addFeatureDtos)
    {
        if (addFeatureDtos.Length == 0 || addFeatureDtos == null)
            return Response<int[]>.Fail("No features to add.", HttpStatusCode.NotFound);

        if (addFeatureDtos.Length > 25)
            return Response<int[]>.Fail("Cannot add more than 25 features at once.", HttpStatusCode.BadRequest);

        int[] newFeatureIds = [];

        foreach (var newFeatureDto in addFeatureDtos)
        {
            int newId = features.Max(x => x.Id) + 1;

            // Map DTO to Entity
            var newFeature = new Models.Feature
            {
                Id = newId,
                Name = newFeatureDto.Name,
                Wkt = newFeatureDto.Wkt
            };

            features = features.Append(newFeature).ToArray();
            newFeatureIds = newFeatureIds.Append(newId).ToArray();
        }

        await Task.Delay(100); // Simulate async operation

        return Response<int[]>.Success(newFeatureIds, "Features added successfully.");
    }

    public async Task<Response<Models.Feature[]>> GetFeatures(int? pageNumber, int? pageSize)
    {
        if (features == null || features.Length == 0)
            return Response<Models.Feature[]>.Fail("No features found.", HttpStatusCode.NotFound);

        if (pageNumber.HasValue && pageSize.HasValue)
        {
            if (pageNumber < 1 || pageSize < 1)
                return Response<Models.Feature[]>.Fail("Page number and size must be greater than zero.", HttpStatusCode.BadRequest);

            Models.Feature[] paginatedFeatures = features
                .Skip(pageSize.Value * (pageNumber.Value - 1))
                .Take(pageSize.Value)
                .ToArray();

            if (paginatedFeatures.Length == 0)
                return Response<Models.Feature[]>.Fail("No features found for the specified page.", HttpStatusCode.NotFound);

            return Response<Models.Feature[]>.Success(paginatedFeatures, "Features retrieved successfully.");
        }

        await Task.Delay(100); // Simulate async operation

        return Response<Models.Feature[]>.Success(features, "Features retrieved successfully.");
    }

    public async Task<Response<Models.Feature>> GetFeatureById(int id)
    {
        if (id <= 0)
            return Response<Models.Feature>.Fail("Feature ID must be greater than zero.", HttpStatusCode.BadRequest);

        var feature = features.FirstOrDefault(x => x.Id == id);

        if (feature == null)
            return Response<Models.Feature>.Fail("Feature not found.");

        await Task.Delay(100); // Simulate async operation

        return Response<Models.Feature>.Success(feature, "Feature retrieved successfully.");
    }

    public async Task<Response<Models.Feature>> UpdateFeature(int id, UpdateFeatureDto updateFeatureDto)
    {
        if (id <= 0)
            return Response<Models.Feature>.Fail("Feature ID must be greater than zero.", HttpStatusCode.BadRequest);

        if (updateFeatureDto == null)
            return Response<Models.Feature>.Fail("Feature cannot be null.");

        var existingFeature = features.FirstOrDefault(x => x.Id == id);
        if (existingFeature == null)
            return Response<Models.Feature>.Fail("Feature not found.", HttpStatusCode.NotFound);

        existingFeature.Name = updateFeatureDto.Name;
        existingFeature.Wkt = updateFeatureDto.Wkt;

        features = features.Select(x => x.Id == id ? existingFeature : x).ToArray();

        await Task.Delay(100); // Simulate async operation

        return Response<Models.Feature>.Success(existingFeature, "Feature updated successfully.");
    }

    public async Task<Response<bool>> DeleteFeature(int id)
    {
        if (id <= 0)
            return Response<bool>.Fail("Feature ID must be greater than zero.", HttpStatusCode.BadRequest);

        var feature = features.FirstOrDefault(x => x.Id == id);

        if (feature == null)
            return Response<bool>.Fail("Feature not found.", HttpStatusCode.NotFound);

        features = features.Where(x => x.Id != id).ToArray();

        await Task.Delay(100); // Simulate async operation

        return Response<bool>.Success(true, "Feature deleted successfully.", HttpStatusCode.NoContent);
    }
}
