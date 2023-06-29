using System;
using System.Collections.Generic;

namespace ServicioApi.Model.SisGes
{
    public partial class SencillosTienda
    {
        public SencillosTienda()
        {
            Remitos = new HashSet<Remito>();
        }

        public Guid Id { get; set; }
        public Guid? IdDetalleSencillo { get; set; }
        public int? CodigoEstadoSencillo { get; set; }
        public DateTime? FechaCreacion { get; set; }
        public int? IdTienda { get; set; }

        public virtual EstadoSencillo? CodigoEstadoSencilloNavigation { get; set; }
        public virtual DetalleSencillo? IdDetalleSencilloNavigation { get; set; }
        public virtual CentrosLocal? IdTiendaNavigation { get; set; }
        public virtual ICollection<Remito> Remitos { get; set; }
    }
}
