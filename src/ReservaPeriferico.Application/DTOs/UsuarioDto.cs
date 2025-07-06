using System.ComponentModel.DataAnnotations;

namespace ReservaPeriferico.Application.DTOs;

public class UsuarioDto
{
    public int Id { get; set; }
    
    [Required(ErrorMessage = "Nome é obrigatório")]
    [StringLength(100, ErrorMessage = "Nome deve ter no máximo 100 caracteres")]
    public string Nome { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Email é obrigatório")]
    [EmailAddress(ErrorMessage = "Email inválido")]
    [StringLength(100, ErrorMessage = "Email deve ter no máximo 100 caracteres")]
    public string Email { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Matrícula é obrigatória")]
    [StringLength(20, ErrorMessage = "Matrícula deve ter no máximo 20 caracteres")]
    public string Matricula { get; set; } = string.Empty;
    
    [StringLength(50, ErrorMessage = "Departamento deve ter no máximo 50 caracteres")]
    public string? Departamento { get; set; }
    
    [StringLength(50, ErrorMessage = "Cargo deve ter no máximo 50 caracteres")]
    public string? Cargo { get; set; }
    
    public bool Ativo { get; set; } = true;
    
    public DateTime DataCadastro { get; set; }
    
    public DateTime? DataAtualizacao { get; set; }
} 