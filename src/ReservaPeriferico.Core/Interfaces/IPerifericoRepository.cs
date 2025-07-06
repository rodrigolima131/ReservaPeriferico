using ReservaPeriferico.Core.Entities;

namespace ReservaPeriferico.Core.Interfaces;

public interface IPerifericoRepository : IRepository<Periferico>
{
    Task<IEnumerable<Periferico>> GetByTipoAsync(string tipo);
    Task<IEnumerable<Periferico>> GetAtivosAsync();
    Task<Periferico?> GetByNumeroSerieAsync(string numeroSerie);
    Task<bool> NumeroSerieExistsAsync(string numeroSerie, int? excludeId = null);
} 