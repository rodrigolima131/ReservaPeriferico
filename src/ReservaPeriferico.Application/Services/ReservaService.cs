using ReservaPeriferico.Application.DTOs;
using ReservaPeriferico.Application.Interfaces;
using ReservaPeriferico.Core.Entities;
using ReservaPeriferico.Core.Interfaces;
using ReservaPeriferico.Core.Exceptions;
using ReservaPeriferico.Core.Enums;

namespace ReservaPeriferico.Application.Services;

public class ReservaService : IReservaService
{
    private readonly IReservaRepository _reservaRepository;
    private readonly IPerifericoRepository _perifericoRepository;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IEquipeRepository _equipeRepository;
    private readonly IUsuarioEquipeRepository _usuarioEquipeRepository;

    public ReservaService(
        IReservaRepository reservaRepository,
        IPerifericoRepository perifericoRepository,
        IUsuarioRepository usuarioRepository,
        IEquipeRepository equipeRepository,
        IUsuarioEquipeRepository usuarioEquipeRepository)
    {
        _reservaRepository = reservaRepository;
        _perifericoRepository = perifericoRepository;
        _usuarioRepository = usuarioRepository;
        _equipeRepository = equipeRepository;
        _usuarioEquipeRepository = usuarioEquipeRepository;
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

    public async Task<IEnumerable<ReservaDto>> GetByEquipeAsync(int equipeId)
    {
        var reservas = await _reservaRepository.GetByEquipeAsync(equipeId);
        return reservas.Select(MapToDto);
    }

    public async Task<IEnumerable<ReservaDto>> GetPendentesAsync(int equipeId)
    {
        var reservas = await _reservaRepository.GetPendentesAsync(equipeId);
        return reservas.Select(MapToDto);
    }

    public async Task<IEnumerable<ReservaDto>> GetAprovadasAsync(int equipeId)
    {
        var reservas = await _reservaRepository.GetAprovadasAsync(equipeId);
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

    public async Task<ReservaDto> SolicitarReservaAsync(int usuarioId, SolicitarReservaDto solicitarReservaDto)
    {
        // Validar se o usuário pode reservar o periférico
        if (!await UsuarioPodeReservarAsync(usuarioId, solicitarReservaDto.PerifericoId))
            throw new ReservaException("Usuário não tem permissão para reservar este periférico");

        // Obter o periférico para pegar a equipe
        var periferico = await _perifericoRepository.GetByIdAsync(solicitarReservaDto.PerifericoId);
        if (periferico == null || !periferico.Ativo)
            throw new ReservaException("Periférico não encontrado ou inativo");

        // Validar se o periférico está disponível no período (apenas para reservas aprovadas)
        if (!await _reservaRepository.PerifericoDisponivelParaAprovacaoAsync(
            solicitarReservaDto.PerifericoId, solicitarReservaDto.DataInicio, 
            solicitarReservaDto.DataFim ?? solicitarReservaDto.DataInicio.AddHours(1)))
        {
            throw new PerifericoNaoDisponivelException("Periférico não está disponível no período solicitado");
        }

        var reserva = new Reserva
        {
            UsuarioId = usuarioId,
            PerifericoId = solicitarReservaDto.PerifericoId,
            EquipeId = periferico.EquipeId,
            DataInicio = solicitarReservaDto.DataInicio,
            DataFim = solicitarReservaDto.DataFim,
            Observacoes = solicitarReservaDto.Observacoes,
            Status = StatusReserva.Pendente,
            DataCadastro = DateTime.UtcNow
        };

        var createdReserva = await _reservaRepository.AddAsync(reserva);
        return MapToDto(createdReserva);
    }

    public async Task<ReservaDto> AprovarReservaAsync(int reservaId, int usuarioAprovadorId, AprovarReservaDto aprovarReservaDto)
    {
        var reserva = await _reservaRepository.GetByIdAsync(reservaId);
        if (reserva == null)
            throw new ReservaException("Reserva não encontrada");

        // Validar se o usuário pode aprovar reservas da equipe
        if (!await UsuarioPodeAprovarAsync(usuarioAprovadorId, reserva.EquipeId))
            throw new ReservaException("Usuário não tem permissão para aprovar reservas desta equipe");

        // Validar se a reserva está pendente
        if (reserva.Status != StatusReserva.Pendente)
            throw new ReservaException("Apenas reservas pendentes podem ser aprovadas/rejeitadas");

        // Se for aprovada, validar disponibilidade
        if (aprovarReservaDto.NovoStatus == StatusReserva.Aprovada)
        {
            if (!await _reservaRepository.PerifericoDisponivelAsync(
                reserva.PerifericoId, reserva.DataInicio, reserva.DataFim, reservaId))
            {
                throw new PerifericoNaoDisponivelException("Periférico não está disponível no período solicitado");
            }
        }

        // Atualizar status
        reserva.Status = aprovarReservaDto.NovoStatus;
        reserva.UsuarioAprovadorId = usuarioAprovadorId;
        reserva.DataAprovacao = DateTime.UtcNow.ToLocalTime();
        reserva.DataAtualizacao = DateTime.UtcNow.ToLocalTime();

        if (aprovarReservaDto.NovoStatus == StatusReserva.Rejeitada)
        {
            reserva.MotivoRejeicao = aprovarReservaDto.Motivo;
        }

        var updatedReserva = await _reservaRepository.UpdateAsync(reserva);
        return MapToDto(updatedReserva);
    }

    public async Task<ReservaDto> CancelarReservaAsync(int reservaId, int usuarioId)
    {
        var reserva = await _reservaRepository.GetByIdAsync(reservaId);
        if (reserva == null)
            throw new ReservaException("Reserva não encontrada");

        // Verificar se o usuário pode cancelar a reserva
        var usuarioEquipe = await _usuarioEquipeRepository.GetByUsuarioAndEquipeAsync(usuarioId, reserva.EquipeId);
        if (usuarioEquipe == null)
            throw new ReservaException("Usuário não pertence à equipe da reserva");

        // Se a reserva está aprovada, apenas administradores podem cancelar
        if (reserva.Status == StatusReserva.Aprovada && !usuarioEquipe.IsAdministrador)
            throw new ReservaException("Apenas administradores podem cancelar reservas aprovadas");

        // Se a reserva não está pendente ou aprovada, não pode ser cancelada
        if (reserva.Status != StatusReserva.Pendente && reserva.Status != StatusReserva.Aprovada)
            throw new ReservaException("Esta reserva não pode ser cancelada");

        reserva.Status = StatusReserva.Cancelada;
        reserva.DataAtualizacao = DateTime.UtcNow;

        var updatedReserva = await _reservaRepository.UpdateAsync(reserva);
        return MapToDto(updatedReserva);
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

    public async Task<bool> PerifericoDisponivelAsync(int perifericoId, DateTime dataInicio, DateTime? dataFim, int? excludeId = null)
    {
        return await _reservaRepository.PerifericoDisponivelAsync(perifericoId, dataInicio, dataFim, excludeId);
    }

    public async Task<bool> UsuarioPodeReservarAsync(int usuarioId, int perifericoId)
    {
        var periferico = await _perifericoRepository.GetByIdAsync(perifericoId);
        if (periferico == null) return false;

        var usuarioEquipe = await _usuarioEquipeRepository.GetByUsuarioAndEquipeAsync(usuarioId, periferico.EquipeId);
        return usuarioEquipe != null && usuarioEquipe.Usuario.Ativo;
    }

    public async Task<bool> UsuarioPodeAprovarAsync(int usuarioId, int equipeId)
    {
        var usuarioEquipe = await _usuarioEquipeRepository.GetByUsuarioAndEquipeAsync(usuarioId, equipeId);
        return usuarioEquipe != null && usuarioEquipe.IsAdministrador && usuarioEquipe.Usuario.Ativo;
    }

    private static ReservaDto MapToDto(Reserva reserva)
    {
        return new ReservaDto
        {
            Id = reserva.Id,
            UsuarioId = reserva.UsuarioId,
            PerifericoId = reserva.PerifericoId,
            EquipeId = reserva.EquipeId,
            DataInicio = reserva.DataInicio,
            DataFim = reserva.DataFim,
            Observacoes = reserva.Observacoes,
            Status = reserva.Status,
            UsuarioAprovadorId = reserva.UsuarioAprovadorId,
            DataAprovacao = reserva.DataAprovacao,
            MotivoRejeicao = reserva.MotivoRejeicao,
            DataCadastro = reserva.DataCadastro,
            DataAtualizacao = reserva.DataAtualizacao,
            DataDevolucao = reserva.DataDevolucao,
            UsuarioNome = reserva.Usuario?.Nome,
            PerifericoNome = reserva.Periferico?.Nome,
            EquipeNome = reserva.Equipe?.Nome,
            UsuarioAprovadorNome = reserva.UsuarioAprovador?.Nome
        };
    }

    private static Reserva MapToEntity(ReservaDto dto)
    {
        return new Reserva
        {
            Id = dto.Id,
            UsuarioId = dto.UsuarioId,
            PerifericoId = dto.PerifericoId,
            EquipeId = dto.EquipeId,
            DataInicio = dto.DataInicio,
            DataFim = dto.DataFim,
            Observacoes = dto.Observacoes,
            Status = dto.Status,
            UsuarioAprovadorId = dto.UsuarioAprovadorId,
            DataAprovacao = dto.DataAprovacao,
            MotivoRejeicao = dto.MotivoRejeicao,
            DataCadastro = dto.DataCadastro,
            DataAtualizacao = dto.DataAtualizacao,
            DataDevolucao = dto.DataDevolucao
        };
    }

    public async Task<IEnumerable<HistoricoReservaDto>> GetHistoricoAsync(FiltroHistoricoDto filtros)
    {
        var reservas = await _reservaRepository.GetAllAsync();
        
        // Filtrar por equipe se especificado
        if (filtros.EquipeId.HasValue)
        {
            reservas = reservas.Where(r => r.EquipeId == filtros.EquipeId.Value);
        }
        
        // Filtrar por usuário se especificado
        if (filtros.UsuarioId.HasValue)
        {
            reservas = reservas.Where(r => r.UsuarioId == filtros.UsuarioId.Value);
        }
        
        // Filtrar por periférico se especificado
        if (filtros.PerifericoId.HasValue)
        {
            reservas = reservas.Where(r => r.PerifericoId == filtros.PerifericoId.Value);
        }
        
        // Filtrar por status se especificado
        if (filtros.Status.HasValue)
        {
            reservas = reservas.Where(r => r.Status == filtros.Status.Value);
        }
        
        // Filtrar por período se especificado
        if (filtros.DataInicio.HasValue)
        {
            reservas = reservas.Where(r => r.DataCadastro >= filtros.DataInicio.Value);
        }
        
        if (filtros.DataFim.HasValue)
        {
            reservas = reservas.Where(r => r.DataCadastro <= filtros.DataFim.Value);
        }
        
        // Filtrar por termo de busca
        if (!string.IsNullOrWhiteSpace(filtros.TermoBusca))
        {
            var termo = filtros.TermoBusca.ToLower();
            reservas = reservas.Where(r => 
                (r.Usuario?.Nome?.ToLower().Contains(termo) == true) ||
                (r.Periferico?.Nome?.ToLower().Contains(termo) == true) ||
                (r.Observacoes?.ToLower().Contains(termo) == true));
        }
        
        // Filtrar reservas expiradas se solicitado
        if (!filtros.IncluirExpiradas)
        {
            reservas = reservas.Where(r => 
                r.Status != StatusReserva.Expirada && 
                (!r.DataFim.HasValue || r.DataFim.Value >= DateTime.UtcNow));
        }
        
        // Ordenar
        var query = filtros.Ordenacao.ToLower() switch
        {
            "datainicio" => filtros.OrdemDecrescente ? 
                reservas.OrderByDescending(r => r.DataInicio) : 
                reservas.OrderBy(r => r.DataInicio),
            "datafim" => filtros.OrdemDecrescente ? 
                reservas.OrderByDescending(r => r.DataFim) : 
                reservas.OrderBy(r => r.DataFim),
            "usuario" => filtros.OrdemDecrescente ? 
                reservas.OrderByDescending(r => r.Usuario?.Nome) : 
                reservas.OrderBy(r => r.Usuario?.Nome),
            "periferico" => filtros.OrdemDecrescente ? 
                reservas.OrderByDescending(r => r.Periferico?.Nome) : 
                reservas.OrderBy(r => r.Periferico?.Nome),
            "status" => filtros.OrdemDecrescente ? 
                reservas.OrderByDescending(r => r.Status) : 
                reservas.OrderBy(r => r.Status),
            _ => filtros.OrdemDecrescente ? 
                reservas.OrderByDescending(r => r.DataCadastro) : 
                reservas.OrderBy(r => r.DataCadastro)
        };
        
        return query.Select(MapToHistoricoDto);
    }

    private static HistoricoReservaDto MapToHistoricoDto(Reserva reserva)
    {
        return new HistoricoReservaDto
        {
            Id = reserva.Id,
            UsuarioNome = reserva.Usuario?.Nome ?? "N/A",
            UsuarioEmail = reserva.Usuario?.Email ?? "N/A",
            PerifericoNome = reserva.Periferico?.Nome ?? "N/A",
            PerifericoTipo = reserva.Periferico?.Tipo ?? "N/A",
            PerifericoMarca = reserva.Periferico?.Marca ?? "N/A",
            PerifericoModelo = reserva.Periferico?.Modelo ?? "N/A",
            EquipeNome = reserva.Equipe?.Nome ?? "N/A",
            DataInicio = reserva.DataInicio,
            DataFim = reserva.DataFim,
            DataCadastro = reserva.DataCadastro,
            DataAprovacao = reserva.DataAprovacao,
            DataAtualizacao = reserva.DataAtualizacao,
            DataDevolucao = reserva.DataDevolucao,
            Status = reserva.Status,
            Observacoes = reserva.Observacoes,
            MotivoRejeicao = reserva.MotivoRejeicao,
            UsuarioAprovadorNome = reserva.UsuarioAprovador?.Nome,
            UsuarioAprovadorEmail = reserva.UsuarioAprovador?.Email
        };
    }
} 