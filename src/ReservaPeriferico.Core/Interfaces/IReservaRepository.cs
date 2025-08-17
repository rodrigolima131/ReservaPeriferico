using ReservaPeriferico.Core.Entities;

namespace ReservaPeriferico.Core.Interfaces;

public interface IReservaRepository : IRepository<Reserva>
{
    Task<IEnumerable<Reserva>> GetByUsuarioAsync(int usuarioId);
    Task<IEnumerable<Reserva>> GetByPerifericoAsync(int perifericoId);
    Task<IEnumerable<Reserva>> GetByPeriodoAsync(DateTime dataInicio, DateTime dataFim);
    Task<IEnumerable<Reserva>> GetAtivasAsync();
    Task<IEnumerable<Reserva>> GetByEquipeAsync(int equipeId);
    Task<IEnumerable<Reserva>> GetPendentesAsync(int equipeId);
    Task<IEnumerable<Reserva>> GetAprovadasAsync(int equipeId);
    Task<bool> PerifericoDisponivelAsync(int perifericoId, DateTime dataInicio, DateTime? dataFim, int? excludeId = null);
    Task<bool> PerifericoDisponivelParaAprovacaoAsync(int perifericoId, DateTime dataInicio, DateTime dataFim, int? excludeId = null);
} 