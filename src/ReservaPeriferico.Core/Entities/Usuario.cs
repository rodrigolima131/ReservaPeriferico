using System.ComponentModel.DataAnnotations;

namespace ReservaPeriferico.Core.Entities;

public class Usuario
{
    public int Id { get; set; }
    
    [Required]
    [StringLength(100)]
    public string Nome { get; set; } = string.Empty;
    
    [Required]
    [StringLength(100)]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    
    [Required]
    [StringLength(20)]
    public string Matricula { get; set; } = string.Empty;
    
    [StringLength(50)]
    public string? Departamento { get; set; }
    
    [StringLength(50)]
    public string? Cargo { get; set; }
    
    public bool Ativo { get; set; } = true;
    
    public DateTime DataCadastro { get; set; } = DateTime.UtcNow;
    
    public DateTime? DataAtualizacao { get; set; }
    
    // Relacionamentos
    public virtual ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();
} 