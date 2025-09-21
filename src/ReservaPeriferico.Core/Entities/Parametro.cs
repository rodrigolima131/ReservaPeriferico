using ReservaPeriferico.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace ReservaPeriferico.Core.Entities;

public class Parametro
{
    public int Id { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string Chave { get; set; } = string.Empty;
    
    [Required]
    public string Valor { get; set; } = string.Empty;
    
    [MaxLength(500)]
    public string? Descricao { get; set; }
    
    public bool Ativo { get; set; } = true;

    public ParametroTipo Tipo{ get; set; } 
    
    public DateTime DataCriacao { get; set; } = DateTime.Now;
    
    public DateTime? DataAtualizacao { get; set; }
    
    [MaxLength(100)]
    public string? UsuarioAtualizacao { get; set; }
}




