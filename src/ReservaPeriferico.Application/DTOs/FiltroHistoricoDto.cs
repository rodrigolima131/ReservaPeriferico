using ReservaPeriferico.Core.Enums;

namespace ReservaPeriferico.Application.DTOs;

public class FiltroHistoricoDto
{
    public int? EquipeId { get; set; }
    public int? UsuarioId { get; set; }
    public int? PerifericoId { get; set; }
    public StatusReserva? Status { get; set; }
    public DateTime? DataInicio { get; set; }
    public DateTime? DataFim { get; set; }
    public string? TermoBusca { get; set; }
    public bool IncluirExpiradas { get; set; } = true;
    public string Ordenacao { get; set; } = "DataCadastro";
    public bool OrdemDecrescente { get; set; } = true;
}
