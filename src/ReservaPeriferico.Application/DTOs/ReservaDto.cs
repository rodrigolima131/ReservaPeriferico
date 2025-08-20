using System.ComponentModel.DataAnnotations;
using ReservaPeriferico.Core.Enums;

namespace ReservaPeriferico.Application.DTOs;

public class ReservaDto
{
    public int Id { get; set; }
    
    [Required(ErrorMessage = "Usuário é obrigatório")]
    public int UsuarioId { get; set; }
    
    [Required(ErrorMessage = "Periférico é obrigatório")]
    public int PerifericoId { get; set; }
    
    [Required(ErrorMessage = "Equipe é obrigatória")]
    public int EquipeId { get; set; }
    
    [Required(ErrorMessage = "Data de início é obrigatória")]
    public DateTime DataInicio { get; set; }
    
    public DateTime? DataFim { get; set; }
    
    [StringLength(500, ErrorMessage = "Observações deve ter no máximo 500 caracteres")]
    public string? Observacoes { get; set; }
    
    [Required]
    public StatusReserva Status { get; set; } = StatusReserva.Pendente;
    
    public int? UsuarioAprovadorId { get; set; }
    
    public DateTime? DataAprovacao { get; set; }
    
    public string? MotivoRejeicao { get; set; }
    
    public DateTime DataCadastro { get; set; }
    
    public DateTime? DataAtualizacao { get; set; }
    
    public DateTime? DataDevolucao { get; set; }
    
    // Propriedades de navegação para exibição
    public string? UsuarioNome { get; set; }
    public string? UsuarioEmail { get; set; }
    public string? UsuarioMatricula { get; set; }
    public string? UsuarioDepartamento { get; set; }
    public string? UsuarioCargo { get; set; }
    
    public string? PerifericoNome { get; set; }
    public string? PerifericoDescricao { get; set; }
    public string? PerifericoTipo { get; set; }
    public string? PerifericoMarca { get; set; }
    public string? PerifericoModelo { get; set; }
    public string? PerifericoNumeroSerie { get; set; }
    
    public string? EquipeNome { get; set; }
    public string? EquipeDescricao { get; set; }
    public string? EquipeAdministradorNome { get; set; }
    
    public string? UsuarioAprovadorNome { get; set; }
    public string? UsuarioAprovadorEmail { get; set; }
    
    // Propriedades calculadas
    public string StatusDescricao => Status switch
    {
        StatusReserva.Pendente => "Pendente",
        StatusReserva.Aprovada => "Aprovada",
        StatusReserva.Rejeitada => "Rejeitada",
        StatusReserva.Cancelada => "Cancelada",
        StatusReserva.EmUso => "Em Uso",
        StatusReserva.Devolvida => "Devolvida",
        StatusReserva.Expirada => "Expirada",
        _ => "Desconhecido"
    };
    
    public bool EstaExpirada => Status != StatusReserva.Cancelada && 
                               Status != StatusReserva.Rejeitada && 
                               DataFim.HasValue && 
                               DataFim.Value < DateTime.UtcNow;
    
    public string DuracaoReserva
    {
        get
        {
            if (!DataFim.HasValue) return "N/A";
            
            var duracao = DataFim.Value - DataInicio;
            if (duracao.TotalDays >= 1)
                return $"{(int)duracao.TotalDays} dia(s)";
            else if (duracao.TotalHours >= 1)
                return $"{(int)duracao.TotalHours} hora(s)";
            else
                return $"{(int)duracao.TotalMinutes} minuto(s)";
        }
    }
    
    public string DuracaoFormatada
    {
        get
        {
            if (!DataFim.HasValue) return "Não definido";
            
            var duracao = DataFim.Value - DataInicio;
            if (duracao.Days > 0)
                return $"{duracao.Days}d {duracao.Hours}h {duracao.Minutes}min";
            else if (duracao.Hours > 0)
                return $"{duracao.Hours}h {duracao.Minutes}min";
            else
                return $"{duracao.Minutes}min";
        }
    }
} 