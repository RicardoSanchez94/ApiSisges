using System;
using System.Collections.Generic;

namespace ServicioApi.Model.SisGes
{
    public partial class Remito
    {
        public string Codigo { get; set; } = null!;
        public int IdTienda { get; set; }
        public Guid? IdSencillosTienda { get; set; }
        public DateTime? Fecha { get; set; }
        public int? CodigoTipoRemito { get; set; }
        public string? NumeroDepostio { get; set; }

        public virtual SencillosTienda? IdSencillosTiendaNavigation { get; set; }
        public virtual CentrosLocal IdTiendaNavigation { get; set; } = null!;
    }
}
