using ReservaPeriferico.Core.Entities;

namespace ReservaPeriferico.Core.Interfaces
{
    public interface IUsuarioEquipeRepository : IRepository<UsuarioEquipe>
    {
        Task<IEnumerable<UsuarioEquipe>> GetByEquipeIdAsync(int equipeId);
        Task<IEnumerable<UsuarioEquipe>> GetByUsuarioIdAsync(int usuarioId);
        Task AddMembroAsync(int equipeId, int usuarioId, bool isAdministrador = false);
        Task RemoveMembroAsync(int equipeId, int usuarioId);
        Task<bool> UsuarioIsMembroAsync(int equipeId, int usuarioId);
        Task<bool> UsuarioIsAdministradorAsync(int equipeId, int usuarioId);
    }
} 