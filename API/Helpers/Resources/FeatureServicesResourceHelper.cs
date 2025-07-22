using System;
using System.Resources;

namespace API.Helpers.Resources;

public static class FeatureServicesResourceHelper
{
    private static readonly ResourceManager _resourceManager =
        new ResourceManager("API.Resources.Services.Feature.FeatureServices", typeof(FeatureServicesResourceHelper).Assembly);

    public static string GetString(string key)
    {
        return _resourceManager.GetString(key) ?? key;
    }

    public static string GetString(string key, params object[] args)
    {
        var format = _resourceManager.GetString(key) ?? key;
        return string.Format(format, args);
    }
}
