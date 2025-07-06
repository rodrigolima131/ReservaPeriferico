using ReservaPeriferico.Core.Entities;

namespace ReservaPeriferico.Core.Interfaces;

public interface IUsuarioRepository : IRepository<Usuario>
{
    Task<Usuario?> GetByEmailAsync(string email);
    Task<Usuario?> GetByMatriculaAsync(string matricula);
    Task<IEnumerable<Usuario>> GetAtivosAsync();
    Task<bool> EmailExistsAsync(string email, int? excludeId = null);
    Task<bool> MatriculaExistsAsync(string matricula, int? excludeId = null);
} 