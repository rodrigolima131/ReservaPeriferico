using ReservaPeriferico.Application.DTOs;
using ReservaPeriferico.Application.Interfaces;
using ReservaPeriferico.Core.Entities;
using ReservaPeriferico.Core.Interfaces;

namespace ReservaPeriferico.Application.Services;

public class EquipeService : IEquipeService
{
    private readonly IEquipeRepository _equipeRepository;
    private readonly IUsuarioEquipeRepository _usuarioEquipeRepository;

    public EquipeService(IEquipeRepository equipeRepository, IUsuarioEquipeRepository usuarioEquipeRepository)
    {
        _equipeRepository = equipeRepository;
        _usuarioEquipeRepository = usuarioEquipeRepository;
    }

    public async Task<EquipeDto?> GetByIdAsync(int id)
    {
        var equipe = await _equipeRepository.GetByIdAsync(id);
        return equipe != null ? MapToDto(equipe) : null;
    }

    public async Task<IEnumerable<EquipeDto>> GetAllAsync()
    {
        var equipes = await _equipeRepository.GetAllAsync();
        return equipes.Select(MapToDto);
    }

    public async Task<EquipeDto?> GetByNomeAsync(string nome)
    {
        var equipe = await _equipeRepository.GetByNomeAsync(nome);
        return equipe != null ? MapToDto(equipe) : null;
    }

    public async Task<EquipeDto> CreateAsync(EquipeDto equipeDto)
    {
        var equipe = MapToEntity(equipeDto);
        equipe.DataCadastro = DateTime.UtcNow;
        var createdEquipe = await _equipeRepository.AddAsync(equipe);

        // Adicionar o administrador como membro (sempre)
        await _usuarioEquipeRepository.AddMembroAsync(createdEquipe.Id, equipeDto.UsuarioAdministradorId, true);

        // Adicionar os outros membros (excluindo o administrador para evitar duplicação)
        var membrosParaAdicionar = equipeDto.MembrosIds
            .Where(id => id != equipeDto.UsuarioAdministradorId)
            .Distinct()
            .ToList();

        foreach (var membroId in membrosParaAdicionar)
        {
            await _usuarioEquipeRepository.AddMembroAsync(createdEquipe.Id, membroId, false);
        }

        return MapToDto(createdEquipe);
    }

    public async Task<EquipeDto> UpdateAsync(int id, EquipeDto equipeDto)
    {
        var existingEquipe = await _equipeRepository.GetByIdAsync(id);
        if (existingEquipe == null)
            throw new ArgumentException("Equipe não encontrada");

        var equipe = MapToEntity(equipeDto);
        equipe.Id = id;
        equipe.DataAtualizacao = DateTime.UtcNow;
        var updatedEquipe = await _equipeRepository.UpdateAsync(equipe);

        // Atualizar membros (remover todos e adicionar novamente)
        var membrosAtuais = await _usuarioEquipeRepository.GetByEquipeIdAsync(id);
        foreach (var membro in membrosAtuais)
        {
            await _usuarioEquipeRepository.RemoveMembroAsync(id, membro.UsuarioId);
        }

        // Adicionar o administrador como membro
        await _usuarioEquipeRepository.AddMembroAsync(id, equipeDto.UsuarioAdministradorId, true);

        // Adicionar os outros membros
        foreach (var membroId in equipeDto.MembrosIds.Where(m => m != equipeDto.UsuarioAdministradorId))
        {
            await _usuarioEquipeRepository.AddMembroAsync(id, membroId, false);
        }

        return MapToDto(updatedEquipe);
    }

    public async Task DeleteAsync(int id)
    {
        // Remover todos os membros primeiro
        var membros = await _usuarioEquipeRepository.GetByEquipeIdAsync(id);
        foreach (var membro in membros)
        {
            await _usuarioEquipeRepository.RemoveMembroAsync(id, membro.UsuarioId);
        }

        await _equipeRepository.DeleteAsync(id);
    }

    public async Task<IEnumerable<EquipeDto>> GetByUsuarioIdAsync(int usuarioId)
    {
        var equipes = await _equipeRepository.GetAllAsync();
        return equipes.Where(e => e.Membros.Any(m => m.UsuarioId == usuarioId)).Select(MapToDto);
    }

    private static EquipeDto MapToDto(Equipe equipe)
    {
        return new EquipeDto
        {
            Id = equipe.Id,
            Nome = equipe.Nome,
            Descricao = equipe.Descricao,
            UsuarioAdministradorId = equipe.UsuarioAdministradorId,
            MembrosIds = equipe.Membros.Select(m => m.UsuarioId).ToList(),
            DataCadastro = equipe.DataCadastro,
            DataAtualizacao = equipe.DataAtualizacao
        };
    }

    private static Equipe MapToEntity(EquipeDto dto)
    {
        return new Equipe
        {
            Id = dto.Id,
            Nome = dto.Nome,
            Descricao = dto.Descricao,
            UsuarioAdministradorId = dto.UsuarioAdministradorId,
            DataCadastro = dto.DataCadastro,
            DataAtualizacao = dto.DataAtualizacao
        };
    }
} 