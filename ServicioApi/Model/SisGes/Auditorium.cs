using System;
using System.Collections.Generic;

namespace ServicioApi.Model.SisGes
{
    public partial class Auditorium
    {
        public Guid IdUsuario { get; set; }
        public DateTime Fecha { get; set; }
        public string? Descripcion { get; set; }

        public virtual Usuario IdUsuarioNavigation { get; set; } = null!;
    }
}
