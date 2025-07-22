using System;

namespace API.Repositories.GRP;

public interface IGenericRepository<T> where T : class
{
    Task<List<T>> GetAllAsync();
    Task<List<T>> GetPagedAsync(int pageNumber, int pageSize);
    Task<T?> GetByIdAsync(int id);
    Task<int> AddAsync(T entity);
    Task<int[]> AddRangeAsync(T[] entities);
    Task<T> UpdateAsync(T entity);
    Task<bool> DeleteAsync(int id);

}
