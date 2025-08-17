using ReservaPeriferico.Application.DTOs;
using ReservaPeriferico.Application.Interfaces;
using ReservaPeriferico.Core.Entities;
using ReservaPeriferico.Core.Interfaces;

namespace ReservaPeriferico.Application.Services;

public class PerifericoService : IPerifericoService
{
    private readonly IPerifericoRepository _perifericoRepository;

    public PerifericoService(IPerifericoRepository perifericoRepository)
    {
        _perifericoRepository = perifericoRepository;
    }

    public async Task<PerifericoDto?> GetByIdAsync(int id)
    {
        var periferico = await _perifericoRepository.GetByIdAsync(id);
        return periferico != null ? MapToDto(periferico) : null;
    }

    public async Task<IEnumerable<PerifericoDto>> GetAllAsync()
    {
        var perifericos = await _perifericoRepository.GetAllAsync();
        return perifericos.Select(MapToDto);
    }

    public async Task<IEnumerable<PerifericoDto>> GetAtivosAsync()
    {
        var perifericos = await _perifericoRepository.GetAtivosAsync();
        return perifericos.Select(MapToDto);
    }

    public async Task<IEnumerable<PerifericoDto>> GetByTipoAsync(string tipo)
    {
        var perifericos = await _perifericoRepository.GetByTipoAsync(tipo);
        return perifericos.Select(MapToDto);
    }

    public async Task<PerifericoDto> CreateAsync(PerifericoDto perifericoDto)
    {
        var periferico = MapToEntity(perifericoDto);
        var createdPeriferico = await _perifericoRepository.AddAsync(periferico);
        return MapToDto(createdPeriferico);
    }

    public async Task<PerifericoDto> UpdateAsync(int id, PerifericoDto perifericoDto)
    {
        var existingPeriferico = await _perifericoRepository.GetByIdAsync(id);
        if (existingPeriferico == null)
            throw new ArgumentException("Periférico não encontrado");

        // Atualizar apenas os campos que foram modificados
        existingPeriferico.Nome = perifericoDto.Nome;
        existingPeriferico.Descricao = perifericoDto.Descricao;
        existingPeriferico.Tipo = perifericoDto.Tipo;
        existingPeriferico.Marca = perifericoDto.Marca;
        existingPeriferico.Modelo = perifericoDto.Modelo;
        existingPeriferico.NumeroSerie = perifericoDto.NumeroSerie;
        existingPeriferico.Ativo = perifericoDto.Ativo;
        existingPeriferico.EquipeId = perifericoDto.EquipeId;
        existingPeriferico.DataAtualizacao = DateTime.UtcNow;
        
        var updatedPeriferico = await _perifericoRepository.UpdateAsync(existingPeriferico);
        return MapToDto(updatedPeriferico);
    }

    public async Task DeleteAsync(int id)
    {
        await _perifericoRepository.DeleteAsync(id);
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _perifericoRepository.ExistsAsync(id);
    }

    public async Task<bool> NumeroSerieExistsAsync(string numeroSerie, int? excludeId = null)
    {
        return await _perifericoRepository.NumeroSerieExistsAsync(numeroSerie, excludeId);
    }

    private static PerifericoDto MapToDto(Periferico periferico)
    {
        return new PerifericoDto
        {
            Id = periferico.Id,
            Nome = periferico.Nome,
            Descricao = periferico.Descricao,
            Tipo = periferico.Tipo,
            Marca = periferico.Marca,
            Modelo = periferico.Modelo,
            NumeroSerie = periferico.NumeroSerie,
            Ativo = periferico.Ativo,
            EquipeId = periferico.EquipeId,
            DataCadastro = periferico.DataCadastro,
            DataAtualizacao = periferico.DataAtualizacao
        };
    }

    private static Periferico MapToEntity(PerifericoDto dto)
    {
        return new Periferico
        {
            Id = dto.Id,
            Nome = dto.Nome,
            Descricao = dto.Descricao,
            Tipo = dto.Tipo,
            Marca = dto.Marca,
            Modelo = dto.Modelo,
            NumeroSerie = dto.NumeroSerie,
            Ativo = dto.Ativo,
            EquipeId = dto.EquipeId,
            DataCadastro = dto.Id > 0 ? dto.DataCadastro : DateTime.UtcNow,
            DataAtualizacao = dto.Id > 0 ? DateTime.UtcNow : dto.DataAtualizacao
        };
    }
} 