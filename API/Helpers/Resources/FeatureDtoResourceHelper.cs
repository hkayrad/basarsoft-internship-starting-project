using System;
using System.Resources;

namespace API.Helpers.Resources;

public static class FeatureDtoResourceHelper
{
    private static readonly ResourceManager _resourceManager = new ResourceManager("API.Resources.Models.DTOs.Feature.FeatureDto", typeof(FeatureDtoResourceHelper).Assembly);
    public static string GetString(string key)
    {
        return _resourceManager.GetString(key) ?? key;
    }
}
