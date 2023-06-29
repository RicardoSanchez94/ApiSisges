using System;
using System.Collections.Generic;

namespace ServicioApi.Model.SisGes
{
    public partial class Usuario
    {
        public Usuario()
        {
            Auditoria = new HashSet<Auditorium>();
        }

        public Guid Id { get; set; }
        public string? Password { get; set; }
        public string? FechaCreacion { get; set; }
        public bool? Activo { get; set; }

        public virtual Persona IdNavigation { get; set; } = null!;
        public virtual ICollection<Auditorium> Auditoria { get; set; }
    }
}
