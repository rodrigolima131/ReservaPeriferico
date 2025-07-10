using System;
using System.Collections.Generic;

namespace ReservaPeriferico.Application.DTOs
{
    public class EquipeDto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string? Descricao { get; set; }
        public int UsuarioAdministradorId { get; set; }
        public List<int> MembrosIds { get; set; } = new();
        public DateTime DataCadastro { get; set; }
        public DateTime? DataAtualizacao { get; set; }
    }
} 