using API.Models;
using API.Models.DTOs.Feature;
using API.Services.Feature;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class FeaturesController(IFeatureServices featureServices) : ControllerBase
    {
        private readonly IFeatureServices _featureServices = featureServices;

        [MapToApiVersion("1.0")]
        [HttpPost]
        public async Task<Response<int>> AddFeature(AddFeatureDto feature)
        {
            return await _featureServices.AddFeature(feature);
        }

        [MapToApiVersion("1.0")]
        [HttpPost("addMultiple")]
        public async Task<Response<int[]>> AddFeatures(AddFeatureDto[] features)
        {
            return await _featureServices.AddFeatures(features);
        }

        [MapToApiVersion("1.0")]
        [HttpGet]
        public async Task<Response<Feature[]>> GetFeatures([FromQuery(Name = "pageNumber")] int? pageNumber, [FromQuery(Name = "pageSize")] int? pageSize)
        {
            return await _featureServices.GetFeatures(pageNumber, pageSize);
        }

        [MapToApiVersion("1.0")]
        [HttpGet("{id}")]
        public async Task<Response<Feature>> GetFeature(int id)
        {
            return await _featureServices.GetFeatureById(id);
        }

        [MapToApiVersion("1.0")]
        [HttpPut("{id}")]
        public async Task<Response<Feature>> UpdateFeature(int id, UpdateFeatureDto fature)
        {
            return await _featureServices.UpdateFeature(id, fature);
        }

        [MapToApiVersion("1.0")]
        [HttpDelete("{id}")]
        public async Task<Response<bool>> DeleteFeature(int id)
        {
            return await _featureServices.DeleteFeature(id);
        }
    };
}

