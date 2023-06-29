using System;
using System.Collections.Generic;

namespace ServicioApi.Model.SisGes
{
    public partial class EstadoSencillo
    {
        public EstadoSencillo()
        {
            SencillosTienda = new HashSet<SencillosTienda>();
        }

        public int Codigo { get; set; }
        public string? Sigla { get; set; }
        public string? Nombre { get; set; }
        public string? Descripcion { get; set; }

        public virtual ICollection<SencillosTienda> SencillosTienda { get; set; }
    }
}
