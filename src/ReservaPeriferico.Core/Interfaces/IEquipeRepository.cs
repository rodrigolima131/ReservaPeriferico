using ReservaPeriferico.Core.Entities;

namespace ReservaPeriferico.Core.Interfaces
{
    public interface IEquipeRepository : IRepository<Equipe>
    {
        Task<Equipe?> GetByNomeAsync(string nome);
    }
} 