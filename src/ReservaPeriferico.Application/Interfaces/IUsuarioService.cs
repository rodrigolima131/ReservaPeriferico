using ReservaPeriferico.Application.DTOs;

namespace ReservaPeriferico.Application.Interfaces;

public interface IUsuarioService
{
    Task<UsuarioDto?> GetByIdAsync(int id);
    Task<IEnumerable<UsuarioDto>> GetAllAsync();
    Task<IEnumerable<UsuarioDto>> GetAtivosAsync();
    Task<UsuarioDto> CreateAsync(UsuarioDto usuarioDto);
    Task<UsuarioDto> UpdateAsync(int id, UsuarioDto usuarioDto);
    Task DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
    Task<bool> EmailExistsAsync(string email, int? excludeId = null);
    Task<bool> MatriculaExistsAsync(string matricula, int? excludeId = null);
} 