using System;
using System.Collections.Generic;

namespace ServicioApi.Model.SisGes
{
    public partial class Interface
    {
        public Interface()
        {
            InterfazAutorizacions = new HashSet<InterfazAutorizacion>();
            InterfazPagos = new HashSet<InterfazPago>();
        }

        public Guid Id { get; set; }
        public int? Mes { get; set; }
        public int? Año { get; set; }
        public int? Idtipo { get; set; }

        public virtual TipoInterfaz? IdtipoNavigation { get; set; }
        public virtual ICollection<InterfazAutorizacion> InterfazAutorizacions { get; set; }
        public virtual ICollection<InterfazPago> InterfazPagos { get; set; }
    }
}
