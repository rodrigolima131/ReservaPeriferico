using ReservaPeriferico.Core.Enums;

namespace ReservaPeriferico.Application.Interfaces;

public interface IParametroService
{
    Task<string?> GetParameterAsync(ParametroChave chave);
    Task<string?> GetParameterAsync(string chave);
    Task<T?> GetParameterAsync<T>(ParametroChave chave) where T : IConvertible;
    Task<T?> GetParameterAsync<T>(string chave) where T : IConvertible;
    Task<Dictionary<string, string>> GetAllParametersAsync();
    Task<bool> SetParameterAsync(ParametroChave chave, string valor, string? usuarioAtualizacao = null);
    Task<bool> SetParameterAsync(string chave, string valor, string? usuarioAtualizacao = null);
    Task<bool> SetParameterAsync<T>(ParametroChave chave, T valor, string? usuarioAtualizacao = null) where T : IConvertible;
    Task<bool> SetParameterAsync<T>(string chave, T valor, string? usuarioAtualizacao = null) where T : IConvertible;
}




