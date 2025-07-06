using ReservaPeriferico.Application.DTOs;
using ReservaPeriferico.Application.Interfaces;
using ReservaPeriferico.Core.Entities;
using ReservaPeriferico.Core.Interfaces;
using ReservaPeriferico.Core.Exceptions;

namespace ReservaPeriferico.Application.Services;

public class ReservaService : IReservaService
{
    private readonly IReservaRepository _reservaRepository;
    private readonly IPerifericoRepository _perifericoRepository;
    private readonly IUsuarioRepository _usuarioRepository;

    public ReservaService(
        IReservaRepository reservaRepository,
        IPerifericoRepository perifericoRepository,
        IUsuarioRepository usuarioRepository)
    {
        _reservaRepository = reservaRepository;
        _perifericoRepository = perifericoRepository;
        _usuarioRepository = usuarioRepository;
    }

    public async Task<ReservaDto?> GetByIdAsync(int id)
    {
        var reserva = await _reservaRepository.GetByIdAsync(id);
        return reserva != null ? MapToDto(reserva) : null;
    }

    public async Task<IEnumerable<ReservaDto>> GetAllAsync()
    {
        var reservas = await _reservaRepository.GetAllAsync();
        return reservas.Select(MapToDto);
    }

    public async Task<IEnumerable<ReservaDto>> GetByUsuarioAsync(int usuarioId)
    {
        var reservas = await _reservaRepository.GetByUsuarioAsync(usuarioId);
        return reservas.Select(MapToDto);
    }

    public async Task<IEnumerable<ReservaDto>> GetByPerifericoAsync(int perifericoId)
    {
        var reservas = await _reservaRepository.GetByPerifericoAsync(perifericoId);
        return reservas.Select(MapToDto);
    }

    public async Task<IEnumerable<ReservaDto>> GetByPeriodoAsync(DateTime dataInicio, DateTime dataFim)
    {
        var reservas = await _reservaRepository.GetByPeriodoAsync(dataInicio, dataFim);
        return reservas.Select(MapToDto);
    }

    public async Task<IEnumerable<ReservaDto>> GetAtivasAsync()
    {
        var reservas = await _reservaRepository.GetAtivasAsync();
        return reservas.Select(MapToDto);
    }

    public async Task<ReservaDto> CreateAsync(ReservaDto reservaDto)
    {
        // Validar se o periférico existe e está ativo
        var periferico = await _perifericoRepository.GetByIdAsync(reservaDto.PerifericoId);
        if (periferico == null || !periferico.Ativo)
            throw new ReservaException("Periférico não encontrado ou inativo");

        // Validar se o usuário existe e está ativo
        var usuario = await _usuarioRepository.GetByIdAsync(reservaDto.UsuarioId);
        if (usuario == null || !usuario.Ativo)
            throw new ReservaException("Usuário não encontrado ou inativo");

        // Validar se o periférico está disponível no período
        if (!await _reservaRepository.PerifericoDisponivelAsync(
            reservaDto.PerifericoId, reservaDto.DataInicio, reservaDto.DataFim))
        {
            throw new PerifericoNaoDisponivelException("Periférico não está disponível no período solicitado");
        }

        var reserva = MapToEntity(reservaDto);
        var createdReserva = await _reservaRepository.AddAsync(reserva);
        return MapToDto(createdReserva);
    }

    public async Task<ReservaDto> UpdateAsync(int id, ReservaDto reservaDto)
    {
        var existingReserva = await _reservaRepository.GetByIdAsync(id);
        if (existingReserva == null)
            throw new ArgumentException("Reserva não encontrada");

        // Validar se o periférico está disponível no período (excluindo a própria reserva)
        if (!await _reservaRepository.PerifericoDisponivelAsync(
            reservaDto.PerifericoId, reservaDto.DataInicio, reservaDto.DataFim, id))
        {
            throw new PerifericoNaoDisponivelException("Periférico não está disponível no período solicitado");
        }

        var reserva = MapToEntity(reservaDto);
        reserva.Id = id;
        reserva.DataAtualizacao = DateTime.UtcNow;
        
        var updatedReserva = await _reservaRepository.UpdateAsync(reserva);
        return MapToDto(updatedReserva);
    }

    public async Task DeleteAsync(int id)
    {
        await _reservaRepository.DeleteAsync(id);
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _reservaRepository.ExistsAsync(id);
    }

    public async Task<bool> PerifericoDisponivelAsync(int perifericoId, DateTime dataInicio, DateTime dataFim, int? excludeId = null)
    {
        return await _reservaRepository.PerifericoDisponivelAsync(perifericoId, dataInicio, dataFim, excludeId);
    }

    private static ReservaDto MapToDto(Reserva reserva)
    {
        return new ReservaDto
        {
            Id = reserva.Id,
            UsuarioId = reserva.UsuarioId,
            PerifericoId = reserva.PerifericoId,
            DataInicio = reserva.DataInicio,
            DataFim = reserva.DataFim,
            Observacoes = reserva.Observacoes,
            DataCadastro = reserva.DataCadastro,
            DataAtualizacao = reserva.DataAtualizacao,
            DataDevolucao = reserva.DataDevolucao,
            UsuarioNome = reserva.Usuario?.Nome,
            PerifericoNome = reserva.Periferico?.Nome
        };
    }

    private static Reserva MapToEntity(ReservaDto dto)
    {
        return new Reserva
        {
            Id = dto.Id,
            UsuarioId = dto.UsuarioId,
            PerifericoId = dto.PerifericoId,
            DataInicio = dto.DataInicio,
            DataFim = dto.DataFim,
            Observacoes = dto.Observacoes,
            DataCadastro = dto.DataCadastro,
            DataAtualizacao = dto.DataAtualizacao,
            DataDevolucao = dto.DataDevolucao
        };
    }
} 