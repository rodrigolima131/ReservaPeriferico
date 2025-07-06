using Microsoft.EntityFrameworkCore;
using ReservaPeriferico.Core.Entities;
using ReservaPeriferico.Core.Interfaces;
using ReservaPeriferico.Infrastructure.Data;

namespace ReservaPeriferico.Infrastructure.Repositories;

public class PerifericoRepository : Repository<Periferico>, IPerifericoRepository
{
    public PerifericoRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Periferico>> GetByTipoAsync(string tipo)
    {
        return await _dbSet
            .Where(p => p.Tipo == tipo && p.Ativo)
            .ToListAsync();
    }

    public async Task<IEnumerable<Periferico>> GetAtivosAsync()
    {
        return await _dbSet
            .Where(p => p.Ativo)
            .ToListAsync();
    }

    public async Task<Periferico?> GetByNumeroSerieAsync(string numeroSerie)
    {
        return await _dbSet
            .FirstOrDefaultAsync(p => p.NumeroSerie == numeroSerie);
    }

    public async Task<bool> NumeroSerieExistsAsync(string numeroSerie, int? excludeId = null)
    {
        var query = _dbSet.Where(p => p.NumeroSerie == numeroSerie);
        
        if (excludeId.HasValue)
        {
            query = query.Where(p => p.Id != excludeId.Value);
        }
        
        return await query.AnyAsync();
    }
} 