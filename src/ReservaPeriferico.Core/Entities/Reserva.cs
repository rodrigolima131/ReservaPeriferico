using System.ComponentModel.DataAnnotations;
using ReservaPeriferico.Core.Enums;

namespace ReservaPeriferico.Core.Entities;

public class Reserva
{
    public int Id { get; set; }
    
    public int UsuarioId { get; set; }
    
    public int PerifericoId { get; set; }
    
    public int EquipeId { get; set; }
    
    [Required]
    public DateTime DataInicio { get; set; }
    
    public DateTime? DataFim { get; set; }
    
    [StringLength(500)]
    public string? Observacoes { get; set; }
    
    [Required]
    public StatusReserva Status { get; set; } = StatusReserva.Pendente;
    
    public int? UsuarioAprovadorId { get; set; }
    
    public DateTime? DataAprovacao { get; set; }
    
    [StringLength(500)]
    public string? MotivoRejeicao { get; set; }
    
    public DateTime DataCadastro { get; set; } = DateTime.UtcNow;
    
    public DateTime? DataAtualizacao { get; set; }
    
    public DateTime? DataDevolucao { get; set; }
    
    // Relacionamentos
    public virtual Usuario Usuario { get; set; } = null!;
    public virtual Periferico Periferico { get; set; } = null!;
    public virtual Equipe Equipe { get; set; } = null!;
    public virtual Usuario? UsuarioAprovador { get; set; }
} 