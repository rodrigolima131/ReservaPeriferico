using ReservaPeriferico.Application.DTOs;

namespace ReservaPeriferico.Application.Interfaces;

public interface IReservaService
{
    Task<ReservaDto?> GetByIdAsync(int id);
    Task<IEnumerable<ReservaDto>> GetAllAsync();
    Task<IEnumerable<ReservaDto>> GetByUsuarioAsync(int usuarioId);
    Task<IEnumerable<ReservaDto>> GetByPerifericoAsync(int perifericoId);
    Task<IEnumerable<ReservaDto>> GetByPeriodoAsync(DateTime dataInicio, DateTime dataFim);
    Task<IEnumerable<ReservaDto>> GetAtivasAsync();
    Task<IEnumerable<ReservaDto>> GetByEquipeAsync(int equipeId);
    Task<IEnumerable<ReservaDto>> GetPendentesAsync(int equipeId);
    Task<IEnumerable<ReservaDto>> GetAprovadasAsync(int equipeId);
    Task<ReservaDto> CreateAsync(ReservaDto reservaDto);
    Task<ReservaDto> SolicitarReservaAsync(int usuarioId, SolicitarReservaDto solicitarReservaDto);
    Task<ReservaDto> AprovarReservaAsync(int reservaId, int usuarioAprovadorId, AprovarReservaDto aprovarReservaDto);
    Task<ReservaDto> CancelarReservaAsync(int reservaId, int usuarioId);
    Task<ReservaDto> UpdateAsync(int id, ReservaDto reservaDto);
    Task DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
    Task<bool> PerifericoDisponivelAsync(int perifericoId, DateTime dataInicio, DateTime? dataFim, int? excludeId = null);
    Task<bool> UsuarioPodeReservarAsync(int usuarioId, int perifericoId);
    Task<bool> UsuarioPodeAprovarAsync(int usuarioId, int equipeId);
} 