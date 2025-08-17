using System.ComponentModel.DataAnnotations;

namespace ReservaPeriferico.Application.DTOs;

public class PerifericoDto
{
    public int Id { get; set; }
    
    [Required(ErrorMessage = "Nome é obrigatório")]
    [StringLength(100, ErrorMessage = "Nome deve ter no máximo 100 caracteres")]
    public string Nome { get; set; } = string.Empty;
    
    [StringLength(500, ErrorMessage = "Descrição deve ter no máximo 500 caracteres")]
    public string? Descricao { get; set; }
    
    [Required(ErrorMessage = "Tipo é obrigatório")]
    [StringLength(50, ErrorMessage = "Tipo deve ter no máximo 50 caracteres")]
    public string Tipo { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Marca é obrigatória")]
    [StringLength(50, ErrorMessage = "Marca deve ter no máximo 50 caracteres")]
    public string Marca { get; set; } = string.Empty;
    
    [StringLength(50, ErrorMessage = "Modelo deve ter no máximo 50 caracteres")]
    public string? Modelo { get; set; }
    
    [Required(ErrorMessage = "Número de série é obrigatório")]
    [StringLength(20, ErrorMessage = "Número de série deve ter no máximo 20 caracteres")]
    public string NumeroSerie { get; set; } = string.Empty;
    
    public bool Ativo { get; set; } = true;
    
    public int EquipeId { get; set; } = 1; // Equipe padrão (hardcoded por enquanto)
    
    public DateTime DataCadastro { get; set; }
    
    public DateTime? DataAtualizacao { get; set; }
} 