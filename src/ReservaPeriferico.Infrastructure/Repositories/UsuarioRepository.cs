using Microsoft.EntityFrameworkCore;
using ReservaPeriferico.Core.Entities;
using ReservaPeriferico.Core.Interfaces;
using ReservaPeriferico.Infrastructure.Data;

namespace ReservaPeriferico.Infrastructure.Repositories;

public class UsuarioRepository : Repository<Usuario>, IUsuarioRepository
{
    public UsuarioRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Usuario?> GetByEmailAsync(string email)
    {
        return await _dbSet
            .FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<Usuario?> GetByMatriculaAsync(string matricula)
    {
        return await _dbSet
            .FirstOrDefaultAsync(u => u.Matricula == matricula);
    }

    public async Task<IEnumerable<Usuario>> GetAtivosAsync()
    {
        return await _dbSet
            .Where(u => u.Ativo)
            .ToListAsync();
    }

    public async Task<bool> EmailExistsAsync(string email, int? excludeId = null)
    {
        var query = _dbSet.Where(u => u.Email == email);
        
        if (excludeId.HasValue)
        {
            query = query.Where(u => u.Id != excludeId.Value);
        }
        
        return await query.AnyAsync();
    }

    public async Task<bool> MatriculaExistsAsync(string matricula, int? excludeId = null)
    {
        var query = _dbSet.Where(u => u.Matricula == matricula);
        
        if (excludeId.HasValue)
        {
            query = query.Where(u => u.Id != excludeId.Value);
        }
        
        return await query.AnyAsync();
    }
} 