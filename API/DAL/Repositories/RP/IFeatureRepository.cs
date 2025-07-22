using System;
using API.Models;

namespace API.Repositories.RP;

public interface IFeatureRepository
{
    Task<List<Feature>> GetAllAsync();

    Task<List<Feature>> GetPagedAsync(int pageNumber, int pageSize);
    Task<Feature?> GetByIdAsync(int id);
    Task<int> AddAsync(Feature feature);
    Task<int[]> AddRangeAsync(Feature[] features);
    Task<Feature> UpdateAsync(Feature feature);
    Task<bool> DeleteAsync(int id);

}
