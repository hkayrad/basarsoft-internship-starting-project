using API.Models;
using API.Models.DTOs.Feature;
using API.Services.Feature;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FeaturesController(IFeatureServices featureServices)
    {
        private readonly IFeatureServices _featureServices = featureServices;

        [HttpPost]
        public async Task<Response<int>> AddFeature(AddFeatureDto feature)
        {
            return await _featureServices.AddFeature(feature);
        }

        [HttpPost("addMultiple")]
        public async Task<Response<int[]>> AddFeatures(AddFeatureDto[] features)
        {
            return await _featureServices.AddFeatures(features);
        }

        [HttpGet]
        public async Task<Response<Feature[]>> GetFeatures([FromQuery(Name = "pageNumber")] int? pageNumber, [FromQuery(Name = "pageSize")] int? pageSize)
        {
            return await _featureServices.GetFeatures(pageNumber, pageSize);
        }

        [HttpGet("{id}")]
        public async Task<Response<Feature>> GetFeature(int id)
        {
            return await _featureServices.GetFeatureById(id);
        }

        [HttpPut("{id}")]
        public async Task<Response<Feature>> UpdateFeature(int id, UpdateFeatureDto fature)
        {
            return await _featureServices.UpdateFeature(id, fature);
        }

        [HttpDelete("{id}")]
        public async Task<Response<bool>> DeleteFeature(int id)
        {
            return await _featureServices.DeleteFeature(id);
        }
    };
}

