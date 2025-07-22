using System;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories.GRP;

public class GenericRepository<T> : IGenericRepository<T> where T : class, IDisposable
{
    protected readonly DbContext _context;
    protected readonly DbSet<T> _dbSet;

    public GenericRepository(DbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _dbSet = _context.Set<T>();
    }

    public async Task<int> AddAsync(T entity)
    {
        _dbSet.Add(entity);
        await _context.SaveChangesAsync();
        return (int)(typeof(T).GetProperty("Id")?.GetValue(entity) ?? 0); // Assuming T has an Id property
    }

    public async Task<int[]> AddRangeAsync(T[] entities)
    {
        using (var transaction = await _context.Database.BeginTransactionAsync())
        {
            try
            {
                await _dbSet.AddRangeAsync(entities);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return [.. entities.Select(e => (int)(typeof(T).GetProperty("Id")?.GetValue(e) ?? 0))];
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
        var entity = await _dbSet.FindAsync(id);
        if (entity != null)
        {
            _dbSet.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        return false;
    }

    public async Task<List<T>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    public async Task<T?> GetByIdAsync(int id)
    {
        return await _dbSet.FindAsync(id);
    }

    public async Task<List<T>> GetPagedAsync(int pageNumber, int pageSize)
    {
        return await _dbSet
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<T> UpdateAsync(T entity)
    {
        _dbSet.Update(entity);
        await _context.SaveChangesAsync();
        return entity;
    }
}
