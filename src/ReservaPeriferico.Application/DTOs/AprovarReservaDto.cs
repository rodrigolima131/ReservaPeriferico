using System.ComponentModel.DataAnnotations;
using ReservaPeriferico.Core.Enums;

namespace ReservaPeriferico.Application.DTOs;

public class AprovarReservaDto
{
    [Required]
    public int ReservaId { get; set; }
    
    [Required]
    public StatusReserva NovoStatus { get; set; }
    
    [StringLength(500, ErrorMessage = "Motivo deve ter no máximo 500 caracteres")]
    public string? Motivo { get; set; }
}
