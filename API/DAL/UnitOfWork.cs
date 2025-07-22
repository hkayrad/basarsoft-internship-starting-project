using System;
using API.Models;
using API.Models.Contexts;
using API.Repositories.GRP;
using Microsoft.EntityFrameworkCore.Storage;

namespace API.DAL;

public class UnitOfWork(MapInfoContext context) : IUnitOfWork
{
    private readonly MapInfoContext _context = context;
    private IGenericRepository<Feature>? _featureRepository;
    private IDbContextTransaction? _transaction;
    private bool _disposed = false;

    public IGenericRepository<Feature> FeatureRepository
    {
        get
        {
            return _featureRepository ??= new GenericRepository<Feature>(_context);
        }
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public async Task BeginTransactionAsync()
    {
        _transaction = await _context.Database.BeginTransactionAsync();
    }

    public async Task CommitTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.CommitAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task RollbackTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            _context.Dispose();
            _disposed = true;
        }
    }
}
