using System.ComponentModel.DataAnnotations;

namespace ReservaPeriferico.Core.Entities;

public class Periferico
{
    public int Id { get; set; }
    
    [Required]
    [StringLength(100)]
    public string Nome { get; set; } = string.Empty;
    
    [StringLength(500)]
    public string? Descricao { get; set; }
    
    [Required]
    [StringLength(50)]
    public string Tipo { get; set; } = string.Empty;
    
    [Required]
    [StringLength(50)]
    public string Marca { get; set; } = string.Empty;
    
    [StringLength(50)]
    public string? Modelo { get; set; }
    
    [Required]
    [StringLength(20)]
    public string NumeroSerie { get; set; } = string.Empty;
    
    public bool Ativo { get; set; } = true;
    
    public DateTime DataCadastro { get; set; } = DateTime.UtcNow;
    
    public DateTime? DataAtualizacao { get; set; }
    
    // Relacionamentos
    public virtual ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();
} 