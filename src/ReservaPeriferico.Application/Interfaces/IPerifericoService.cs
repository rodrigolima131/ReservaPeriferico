using ReservaPeriferico.Application.DTOs;

namespace ReservaPeriferico.Application.Interfaces;

public interface IPerifericoService
{
    Task<PerifericoDto?> GetByIdAsync(int id);
    Task<IEnumerable<PerifericoDto>> GetAllAsync();
    Task<IEnumerable<PerifericoDto>> GetAtivosAsync();
    Task<IEnumerable<PerifericoDto>> GetByTipoAsync(string tipo);
    Task<PerifericoDto> CreateAsync(PerifericoDto perifericoDto);
    Task<PerifericoDto> UpdateAsync(int id, PerifericoDto perifericoDto);
    Task DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
    Task<bool> NumeroSerieExistsAsync(string numeroSerie, int? excludeId = null);
} 