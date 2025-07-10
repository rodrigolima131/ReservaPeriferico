using System;
using System.Collections.Generic;

namespace ReservaPeriferico.Core.Entities
{
    public class Equipe
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string? Descricao { get; set; }
        public int UsuarioAdministradorId { get; set; }
        public DateTime DataCadastro { get; set; }
        public DateTime? DataAtualizacao { get; set; }
        public ICollection<UsuarioEquipe> Membros { get; set; } = new List<UsuarioEquipe>();
    }
} 