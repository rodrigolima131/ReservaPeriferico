using System.ComponentModel.DataAnnotations;

namespace ReservaPeriferico.Application.DTOs;

public class SolicitarReservaDto
{
    [Required(ErrorMessage = "Periférico é obrigatório")]
    public int PerifericoId { get; set; }
    
    [Required(ErrorMessage = "Data de início é obrigatória")]
    public DateTime DataInicio { get; set; }
    
    public DateTime? DataFim { get; set; }
    
    [StringLength(500, ErrorMessage = "Observações deve ter no máximo 500 caracteres")]
    public string? Observacoes { get; set; }
}
