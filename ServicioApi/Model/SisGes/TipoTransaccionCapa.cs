using System;
using System.Collections.Generic;

namespace ServicioApi.Model.SisGes
{
    public partial class TipoTransaccionCapa
    {
        public TipoTransaccionCapa()
        {
            InterfazPagos = new HashSet<InterfazPago>();
        }

        public int Codigo { get; set; }
        public string? Nombre { get; set; }

        public virtual ICollection<InterfazPago> InterfazPagos { get; set; }
    }
}
