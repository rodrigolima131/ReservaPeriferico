using ReservaPeriferico.Core.Enums;

namespace ReservaPeriferico.Application.DTOs;

public class HistoricoReservaDto
{
    public int Id { get; set; }
    
    // Informações do usuário que fez a solicitação
    public string UsuarioNome { get; set; } = string.Empty;
    public string UsuarioEmail { get; set; } = string.Empty;
    
    // Informações do periférico
    public string PerifericoNome { get; set; } = string.Empty;
    public string PerifericoTipo { get; set; } = string.Empty;
    public string PerifericoMarca { get; set; } = string.Empty;
    public string PerifericoModelo { get; set; } = string.Empty;
    
    // Informações da equipe
    public string EquipeNome { get; set; } = string.Empty;
    
    // Datas da reserva
    public DateTime DataInicio { get; set; }
    public DateTime? DataFim { get; set; }
    public DateTime DataCadastro { get; set; }
    public DateTime? DataAprovacao { get; set; }
    public DateTime? DataAtualizacao { get; set; }
    public DateTime? DataDevolucao { get; set; }
    
    // Status e informações da reserva
    public StatusReserva Status { get; set; }
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
    
    // Justificativa e observações
    public string? Observacoes { get; set; }
    public string? MotivoRejeicao { get; set; }
    
    // Ações tomadas
    public string? UsuarioAprovadorNome { get; set; }
    public string? UsuarioAprovadorEmail { get; set; }
    
    // Propriedades calculadas
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
}
