using ReservaPeriferico.Application.DTOs;
using ReservaPeriferico.Application.Interfaces;
using ReservaPeriferico.Core.Entities;
using ReservaPeriferico.Core.Interfaces;

namespace ReservaPeriferico.Application.Services;

public class UsuarioService : IUsuarioService
{
    private readonly IUsuarioRepository _usuarioRepository;

    public UsuarioService(IUsuarioRepository usuarioRepository)
    {
        _usuarioRepository = usuarioRepository;
    }

    public async Task<UsuarioDto?> GetByIdAsync(int id)
    {
        var usuario = await _usuarioRepository.GetByIdAsync(id);
        return usuario != null ? MapToDto(usuario) : null;
    }

    public async Task<IEnumerable<UsuarioDto>> GetAllAsync()
    {
        var usuarios = await _usuarioRepository.GetAllAsync();
        return usuarios.Select(MapToDto);
    }

    public async Task<IEnumerable<UsuarioDto>> GetAtivosAsync()
    {
        var usuarios = await _usuarioRepository.GetAtivosAsync();
        return usuarios.Select(MapToDto);
    }

    public async Task<UsuarioDto> CreateAsync(UsuarioDto usuarioDto)
    {
        var usuario = MapToEntity(usuarioDto);
        var createdUsuario = await _usuarioRepository.AddAsync(usuario);
        return MapToDto(createdUsuario);
    }

    public async Task<UsuarioDto> UpdateAsync(int id, UsuarioDto usuarioDto)
    {
        var existingUsuario = await _usuarioRepository.GetByIdAsync(id);
        if (existingUsuario == null)
            throw new ArgumentException("Usuário não encontrado");

        var usuario = MapToEntity(usuarioDto);
        usuario.Id = id;
        usuario.DataAtualizacao = DateTime.UtcNow;
        
        var updatedUsuario = await _usuarioRepository.UpdateAsync(usuario);
        return MapToDto(updatedUsuario);
    }

    public async Task DeleteAsync(int id)
    {
        await _usuarioRepository.DeleteAsync(id);
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _usuarioRepository.ExistsAsync(id);
    }

    public async Task<bool> EmailExistsAsync(string email, int? excludeId = null)
    {
        return await _usuarioRepository.EmailExistsAsync(email, excludeId);
    }

    public async Task<bool> MatriculaExistsAsync(string matricula, int? excludeId = null)
    {
        return await _usuarioRepository.MatriculaExistsAsync(matricula, excludeId);
    }

    private static UsuarioDto MapToDto(Usuario usuario)
    {
        return new UsuarioDto
        {
            Id = usuario.Id,
            Nome = usuario.Nome,
            Email = usuario.Email,
            Matricula = usuario.Matricula,
            Departamento = usuario.Departamento,
            Cargo = usuario.Cargo,
            Ativo = usuario.Ativo,
            DataCadastro = usuario.DataCadastro,
            DataAtualizacao = usuario.DataAtualizacao
        };
    }

    private static Usuario MapToEntity(UsuarioDto dto)
    {
        return new Usuario
        {
            Id = dto.Id,
            Nome = dto.Nome,
            Email = dto.Email,
            Matricula = dto.Matricula,
            Departamento = dto.Departamento,
            Cargo = dto.Cargo,
            Ativo = dto.Ativo,
            DataCadastro = dto.DataCadastro,
            DataAtualizacao = dto.DataAtualizacao
        };
    }
} 