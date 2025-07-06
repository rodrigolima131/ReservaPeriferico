using System.ComponentModel.DataAnnotations;

namespace ReservaPeriferico.Application.DTOs;

public class ReservaDto
{
    public int Id { get; set; }
    
    [Required(ErrorMessage = "Usuário é obrigatório")]
    public int UsuarioId { get; set; }
    
    [Required(ErrorMessage = "Periférico é obrigatório")]
    public int PerifericoId { get; set; }
    
    [Required(ErrorMessage = "Data de início é obrigatória")]
    public DateTime DataInicio { get; set; }
    
    [Required(ErrorMessage = "Data de fim é obrigatória")]
    public DateTime DataFim { get; set; }
    
    [StringLength(500, ErrorMessage = "Observações deve ter no máximo 500 caracteres")]
    public string? Observacoes { get; set; }
    
    public DateTime DataCadastro { get; set; }
    
    public DateTime? DataAtualizacao { get; set; }
    
    public DateTime? DataDevolucao { get; set; }
    
    // Propriedades de navegação para exibição
    public string? UsuarioNome { get; set; }
    public string? PerifericoNome { get; set; }
} 