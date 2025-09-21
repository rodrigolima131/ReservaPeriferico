using Microsoft.Extensions.Logging;
using ReservaPeriferico.Application.Interfaces;
using ReservaPeriferico.Core.Enums;
using ReservaPeriferico.Core.Interfaces;

namespace ReservaPeriferico.Application.Services;

public class ParametroService : IParametroService
{
    private readonly IParametroRepository _parametroRepository;
    private readonly ILogger<ParametroService> _logger;
    private readonly Dictionary<string, string> _cache = new();
    private DateTime _lastCacheUpdate = DateTime.MinValue;
    private readonly TimeSpan _cacheExpiration = TimeSpan.FromMinutes(5);

    public ParametroService(IParametroRepository parametroRepository, ILogger<ParametroService> logger)
    {
        _parametroRepository = parametroRepository;
        _logger = logger;
    }

    public async Task<string?> GetParameterAsync(ParametroChave chave)
    {
        return await GetParameterAsync(chave.ToString());
    }

    public async Task<string?> GetParameterAsync(string chave)
    {
        try
        {
            await RefreshCacheIfNeeded();
            
            if (_cache.TryGetValue(chave, out var valor))
            {
                return valor;
            }

            // Se não estiver no cache, buscar no banco
            var parametro = await _parametroRepository.GetByChaveStringAsync(chave);
            if (parametro != null)
            {
                _cache[chave] = parametro.Valor;
                return parametro.Valor;
            }

            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar parâmetro {Chave}: {Message}", chave, ex.Message);
            return null;
        }
    }

    public async Task<T?> GetParameterAsync<T>(ParametroChave chave) where T : IConvertible
    {
        return await GetParameterAsync<T>(chave.ToString());
    }

    public async Task<T?> GetParameterAsync<T>(string chave) where T : IConvertible
    {
        try
        {
            var valor = await GetParameterAsync(chave);
            if (string.IsNullOrEmpty(valor))
                return default(T);

            return (T)Convert.ChangeType(valor, typeof(T));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao converter parâmetro {Chave} para tipo {Tipo}: {Message}", chave, typeof(T).Name, ex.Message);
            return default(T);
        }
    }

    public async Task<Dictionary<string, string>> GetAllParametersAsync()
    {
        try
        {
            await RefreshCacheIfNeeded();
            return new Dictionary<string, string>(_cache);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar todos os parâmetros: {Message}", ex.Message);
            return new Dictionary<string, string>();
        }
    }

    public async Task<bool> SetParameterAsync(ParametroChave chave, string valor, string? usuarioAtualizacao = null)
    {
        return await SetParameterAsync(chave.ToString(), valor, usuarioAtualizacao);
    }

    public async Task<bool> SetParameterAsync(string chave, string valor, string? usuarioAtualizacao = null)
    {
        try
        {
            var sucesso = await _parametroRepository.UpdateParameterAsync(chave, valor, usuarioAtualizacao);
            if (sucesso)
            {
                // Atualizar cache
                _cache[chave] = valor;
                _logger.LogInformation("Parâmetro {Chave} atualizado para: {Valor}", chave, valor);
            }
            return sucesso;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar parâmetro {Chave}: {Message}", chave, ex.Message);
            return false;
        }
    }

    public async Task<bool> SetParameterAsync<T>(ParametroChave chave, T valor, string? usuarioAtualizacao = null) where T : IConvertible
    {
        return await SetParameterAsync(chave.ToString(), valor.ToString()!, usuarioAtualizacao);
    }

    public async Task<bool> SetParameterAsync<T>(string chave, T valor, string? usuarioAtualizacao = null) where T : IConvertible
    {
        return await SetParameterAsync(chave, valor.ToString()!, usuarioAtualizacao);
    }

    private async Task RefreshCacheIfNeeded()
    {
        if (DateTime.Now - _lastCacheUpdate > _cacheExpiration)
        {
            try
            {
                var parametros = await _parametroRepository.GetAllActiveParametersAsync();
                _cache.Clear();
                foreach (var param in parametros)
                {
                    _cache[param.Key] = param.Value;
                }
                _lastCacheUpdate = DateTime.Now;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar cache de parâmetros: {Message}", ex.Message);
            }
        }
    }
}




