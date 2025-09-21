using ReservaPeriferico.Core.Entities;
using ReservaPeriferico.Core.Enums;

namespace ReservaPeriferico.Core.Interfaces;

public interface IParametroRepository : IRepository<Parametro>
{
    Task<Parametro?> GetByChaveAsync(ParametroChave chave);
    Task<Parametro?> GetByChaveStringAsync(string chave);
    Task<Dictionary<string, string>> GetAllActiveParametersAsync();
    Task<bool> UpdateParameterAsync(ParametroChave chave, string valor, string? usuarioAtualizacao = null);
    Task<bool> UpdateParameterAsync(string chave, string valor, string? usuarioAtualizacao = null);
}




