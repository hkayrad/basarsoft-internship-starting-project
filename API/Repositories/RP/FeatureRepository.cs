using System;
using API.Models;
using API.Models.Contexts;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories.RP;

public class FeatureRepository(MapInfoContext dbContext) : IFeatureRepository
{
    private readonly MapInfoContext _dbContext = dbContext;

    public async Task<int> AddAsync(Feature feature)
    {
        _dbContext.Features.Add(feature);
        await _dbContext.SaveChangesAsync();
        return feature.Id;
    }

    public async Task<int[]> AddRangeAsync(Feature[] features)
    {
        using (var transaction = await _dbContext.Database.BeginTransactionAsync())
        {
            try
            {
                await _dbContext.Features.AddRangeAsync(features);
                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();
                return [.. features.Select(f => f.Id)];
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var feature = await _dbContext.Features.FindAsync(id);
        if (feature != null)
        {
            _dbContext.Features.Remove(feature);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        return false;
    }

    public async Task<List<Feature>> GetAllAsync()
    {
        return await _dbContext.Features.ToListAsync();
    }

    public async Task<List<Feature>> GetPagedAsync(int pageNumber, int pageSize)
    {
        return await _dbContext.Features
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<Feature?> GetByIdAsync(int id)
    {
        return await _dbContext.Features.FindAsync(id);
    }

    public async Task<Feature> UpdateAsync(Feature feature)
    {
        _dbContext.Features.Update(feature);
        await _dbContext.SaveChangesAsync();
        return feature;
    }
}
