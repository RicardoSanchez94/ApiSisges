using System;
using System.Collections.Generic;

namespace ServicioApi.Model.SisGes
{
    public partial class CentrosLocal
    {
        public CentrosLocal()
        {
            DetalleSencillos = new HashSet<DetalleSencillo>();
            Remitos = new HashSet<Remito>();
            SencillosTienda = new HashSet<SencillosTienda>();
        }

        public long CenPos { get; set; }
        public int CenCodigo { get; set; }
        public string? CenNombre { get; set; }
        public string? CenCodigoBanco { get; set; }
        public string? CenCuentaBanco { get; set; }
        public string? CenServidor { get; set; }
        public string? CenEstado { get; set; }
        public int? IdZona { get; set; }
        public string? Correo { get; set; }

        public virtual ICollection<DetalleSencillo> DetalleSencillos { get; set; }
        public virtual ICollection<Remito> Remitos { get; set; }
        public virtual ICollection<SencillosTienda> SencillosTienda { get; set; }
    }
}
