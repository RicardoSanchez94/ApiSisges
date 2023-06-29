using System;
using System.Collections.Generic;

namespace ServicioApi.Model.SisGes
{
    public partial class Sencillo
    {
        public Sencillo()
        {
            DetalleSencillos = new HashSet<DetalleSencillo>();
        }

        public Guid Id { get; set; }
        public DateTime? Fecha { get; set; }
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public int? Correlativo { get; set; }

        public virtual ICollection<DetalleSencillo> DetalleSencillos { get; set; }
    }
}
