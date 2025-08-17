using Microsoft.EntityFrameworkCore;
using ReservaPeriferico.Core.Entities;
using ReservaPeriferico.Core.Interfaces;
using ReservaPeriferico.Infrastructure.Data;
using ReservaPeriferico.Core.Enums;

namespace ReservaPeriferico.Infrastructure.Repositories;

public class ReservaRepository : Repository<Reserva>, IReservaRepository
{
    public ReservaRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Reserva>> GetByUsuarioAsync(int usuarioId)
    {
        return await _dbSet
            .Include(r => r.Usuario)
            .Include(r => r.Periferico)
            .Include(r => r.Equipe)
            .Include(r => r.UsuarioAprovador)
            .Where(r => r.UsuarioId == usuarioId)
            .OrderByDescending(r => r.DataCadastro)
            .ToListAsync();
    }

    public async Task<IEnumerable<Reserva>> GetByPerifericoAsync(int perifericoId)
    {
        return await _dbSet
            .Include(r => r.Usuario)
            .Include(r => r.Periferico)
            .Include(r => r.Equipe)
            .Include(r => r.UsuarioAprovador)
            .Where(r => r.PerifericoId == perifericoId)
            .OrderByDescending(r => r.DataCadastro)
            .ToListAsync();
    }

    public async Task<IEnumerable<Reserva>> GetByPeriodoAsync(DateTime dataInicio, DateTime dataFim)
    {
        return await _dbSet
            .Include(r => r.Usuario)
            .Include(r => r.Periferico)
            .Include(r => r.Equipe)
            .Include(r => r.UsuarioAprovador)
            .Where(r => (r.DataInicio >= dataInicio && r.DataInicio <= dataFim) ||
                       (r.DataFim >= dataInicio && r.DataFim <= dataFim) ||
                       (r.DataInicio <= dataInicio && r.DataFim >= dataFim))
            .OrderByDescending(r => r.DataCadastro)
            .ToListAsync();
    }

    public async Task<IEnumerable<Reserva>> GetAtivasAsync()
    {
        var hoje = DateTime.Today;
        return await _dbSet
            .Include(r => r.Usuario)
            .Include(r => r.Periferico)
            .Include(r => r.Equipe)
            .Include(r => r.UsuarioAprovador)
            .Where(r => r.Status == StatusReserva.Aprovada && r.DataFim >= hoje && r.DataDevolucao == null)
            .OrderByDescending(r => r.DataCadastro)
            .ToListAsync();
    }

    public async Task<IEnumerable<Reserva>> GetByEquipeAsync(int equipeId)
    {
        return await _dbSet
            .Include(r => r.Usuario)
            .Include(r => r.Periferico)
            .Include(r => r.Equipe)
            .Include(r => r.UsuarioAprovador)
            .Where(r => r.EquipeId == equipeId)
            .OrderByDescending(r => r.DataCadastro)
            .ToListAsync();
    }

    public async Task<IEnumerable<Reserva>> GetPendentesAsync(int equipeId)
    {
        return await _dbSet
            .Include(r => r.Usuario)
            .Include(r => r.Periferico)
            .Include(r => r.Equipe)
            .Include(r => r.UsuarioAprovador)
            .Where(r => r.EquipeId == equipeId && r.Status == StatusReserva.Pendente)
            .OrderByDescending(r => r.DataCadastro)
            .ToListAsync();
    }

    public async Task<IEnumerable<Reserva>> GetAprovadasAsync(int equipeId)
    {
        return await _dbSet
            .Include(r => r.Usuario)
            .Include(r => r.Periferico)
            .Include(r => r.Equipe)
            .Include(r => r.UsuarioAprovador)
            .Where(r => r.EquipeId == equipeId && r.Status == StatusReserva.Aprovada)
            .OrderByDescending(r => r.DataCadastro)
            .ToListAsync();
    }

    public async Task<bool> PerifericoDisponivelAsync(int perifericoId, DateTime dataInicio, DateTime? dataFim, int? excludeId = null)
    {
        var dataFimCheck = dataFim ?? dataInicio.AddHours(1);
        
        var query = _dbSet.Where(r => r.PerifericoId == perifericoId &&
                                     r.Status == StatusReserva.Aprovada &&
                                     r.DataDevolucao == null &&
                                     ((r.DataInicio >= dataInicio && r.DataInicio <= dataFimCheck) ||
                                      (r.DataFim >= dataInicio && r.DataFim <= dataFimCheck) ||
                                      (r.DataInicio <= dataInicio && r.DataFim >= dataFimCheck)));
        
        if (excludeId.HasValue)
        {
            query = query.Where(r => r.Id != excludeId.Value);
        }
        
        return !await query.AnyAsync();
    }

    public async Task<bool> PerifericoDisponivelParaAprovacaoAsync(int perifericoId, DateTime dataInicio, DateTime dataFim, int? excludeId = null)
    {
        var query = _dbSet.Where(r => r.PerifericoId == perifericoId &&
                                     r.Status == StatusReserva.Aprovada &&
                                     r.DataDevolucao == null &&
                                     ((r.DataInicio >= dataInicio && r.DataInicio <= dataFim) ||
                                      (r.DataFim >= dataInicio && r.DataFim <= dataFim) ||
                                      (r.DataInicio <= dataInicio && r.DataFim >= dataFim)));
        
        if (excludeId.HasValue)
        {
            query = query.Where(r => r.Id != excludeId.Value);
        }
        
        return !await query.AnyAsync();
    }

    public override async Task<Reserva?> GetByIdAsync(int id)
    {
        return await _dbSet
            .Include(r => r.Usuario)
            .Include(r => r.Periferico)
            .Include(r => r.Equipe)
            .Include(r => r.UsuarioAprovador)
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public override async Task<IEnumerable<Reserva>> GetAllAsync()
    {
        return await _dbSet
            .Include(r => r.Usuario)
            .Include(r => r.Periferico)
            .Include(r => r.Equipe)
            .Include(r => r.UsuarioAprovador)
            .OrderByDescending(r => r.DataCadastro)
            .ToListAsync();
    }
} 