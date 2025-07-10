using System;

namespace ReservaPeriferico.Core.Entities
{
    public class UsuarioEquipe
    {
        public int UsuarioId { get; set; }
        public Usuario Usuario { get; set; } = null!;
        public int EquipeId { get; set; }
        public Equipe Equipe { get; set; } = null!;
        public bool IsAdministrador { get; set; }
        public DateTime DataEntrada { get; set; }
    }
} 