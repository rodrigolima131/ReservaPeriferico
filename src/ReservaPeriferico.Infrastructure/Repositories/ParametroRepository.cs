using Microsoft.EntityFrameworkCore;
using ReservaPeriferico.Core.Entities;
using ReservaPeriferico.Core.Enums;
using ReservaPeriferico.Core.Interfaces;
using ReservaPeriferico.Infrastructure.Data;

namespace ReservaPeriferico.Infrastructure.Repositories;

public class ParametroRepository : Repository<Parametro>, IParametroRepository
{
    public ParametroRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Parametro?> GetByChaveAsync(ParametroChave chave)
    {
        return await _context.Set<Parametro>()
            .FirstOrDefaultAsync(p => p.Chave == chave.ToString() && p.Ativo);
    }

    public async Task<Parametro?> GetByChaveStringAsync(string chave)
    {
        return await _context.Set<Parametro>()
            .FirstOrDefaultAsync(p => p.Chave == chave && p.Ativo);
    }

    public async Task<Dictionary<string, string>> GetAllActiveParametersAsync()
    {
        return await _context.Set<Parametro>()
            .Where(p => p.Ativo)
            .ToDictionaryAsync(p => p.Chave, p => p.Valor);
    }

    public async Task<bool> UpdateParameterAsync(ParametroChave chave, string valor, string? usuarioAtualizacao = null)
    {
        var parametro = await GetByChaveAsync(chave);
        if (parametro == null) return false;

        parametro.Valor = valor;
        parametro.DataAtualizacao = DateTime.Now;
        parametro.UsuarioAtualizacao = usuarioAtualizacao;

        _context.Set<Parametro>().Update(parametro);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdateParameterAsync(string chave, string valor, string? usuarioAtualizacao = null)
    {
        var parametro = await GetByChaveStringAsync(chave);
        if (parametro == null) return false;

        parametro.Valor = valor;
        parametro.DataAtualizacao = DateTime.Now;
        parametro.UsuarioAtualizacao = usuarioAtualizacao;

        _context.Set<Parametro>().Update(parametro);
        await _context.SaveChangesAsync();
        return true;
    }
}




