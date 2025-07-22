using System;
using API.Models;
using API.Repositories.GRP;
using Npgsql;

namespace API.DAL;

public interface IUnitOfWork : IDisposable
{
    IGenericRepository<Feature> FeatureRepository { get; }
    Task<int> SaveChangesAsync();

    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}
