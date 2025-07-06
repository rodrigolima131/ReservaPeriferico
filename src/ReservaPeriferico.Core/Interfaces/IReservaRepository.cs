using ReservaPeriferico.Core.Entities;

namespace ReservaPeriferico.Core.Interfaces;

public interface IReservaRepository : IRepository<Reserva>
{
    Task<IEnumerable<Reserva>> GetByUsuarioAsync(int usuarioId);
    Task<IEnumerable<Reserva>> GetByPerifericoAsync(int perifericoId);
    Task<IEnumerable<Reserva>> GetByPeriodoAsync(DateTime dataInicio, DateTime dataFim);
    Task<IEnumerable<Reserva>> GetAtivasAsync();
    Task<bool> PerifericoDisponivelAsync(int perifericoId, DateTime dataInicio, DateTime dataFim, int? excludeId = null);
} 