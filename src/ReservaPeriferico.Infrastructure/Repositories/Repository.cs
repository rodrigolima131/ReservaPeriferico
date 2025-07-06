using Microsoft.EntityFrameworkCore;
using ReservaPeriferico.Core.Interfaces;
using ReservaPeriferico.Infrastructure.Data;

namespace ReservaPeriferico.Infrastructure.Repositories;

public class Repository<T> : IRepository<T> where T : class
{
    protected readonly ApplicationDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public Repository(ApplicationDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public virtual async Task<T?> GetByIdAsync(int id)
    {
        return await _dbSet.FindAsync(id);
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    public virtual async Task<T> AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public virtual async Task<T> UpdateAsync(T entity)
    {
        // Get the entity type to find existing tracked entities
        var entityType = typeof(T);
        
        // Detach any existing tracked entities of the same type
        var trackedEntries = _context.ChangeTracker.Entries()
            .Where(e => e.Entity.GetType() == entityType)
            .ToList();
            
        foreach (var entry in trackedEntries)
        {
            entry.State = EntityState.Detached;
        }
        
        // Update the entity
        _dbSet.Update(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public virtual async Task DeleteAsync(int id)
    {
        var entity = await GetByIdAsync(id);
        if (entity != null)
        {
            _dbSet.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }

    public virtual async Task<bool> ExistsAsync(int id)
    {
        return await _dbSet.FindAsync(id) != null;
    }
} 