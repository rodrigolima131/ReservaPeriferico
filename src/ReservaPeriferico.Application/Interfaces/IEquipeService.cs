using ReservaPeriferico.Application.DTOs;

namespace ReservaPeriferico.Application.Interfaces
{
    public interface IEquipeService
    {
        Task<EquipeDto?> GetByIdAsync(int id);
        Task<IEnumerable<EquipeDto>> GetAllAsync();
        Task<EquipeDto?> GetByNomeAsync(string nome);
        Task<EquipeDto> CreateAsync(EquipeDto equipeDto);
        Task<EquipeDto> UpdateAsync(int id, EquipeDto equipeDto);
        Task DeleteAsync(int id);
        Task<IEnumerable<EquipeDto>> GetByUsuarioIdAsync(int usuarioId);
    }
} 