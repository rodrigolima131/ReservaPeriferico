using System.ComponentModel.DataAnnotations;

namespace ReservaPeriferico.Core.Entities;

public class Reserva
{
    public int Id { get; set; }
    
    public int UsuarioId { get; set; }
    
    public int PerifericoId { get; set; }
    
    [Required]
    public DateTime DataInicio { get; set; }
    
    [Required]
    public DateTime DataFim { get; set; }
    
    [StringLength(500)]
    public string? Observacoes { get; set; }
    
    public DateTime DataCadastro { get; set; } = DateTime.UtcNow;
    
    public DateTime? DataAtualizacao { get; set; }
    
    public DateTime? DataDevolucao { get; set; }
    
    // Relacionamentos
    public virtual Usuario Usuario { get; set; } = null!;
    public virtual Periferico Periferico { get; set; } = null!;
} 