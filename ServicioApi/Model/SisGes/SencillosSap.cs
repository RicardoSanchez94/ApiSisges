using System;
using System.Collections.Generic;

namespace ServicioApi.Model.SisGes
{
    public partial class SencillosSap
    {
        public SencillosSap()
        {
            DetalleSencillos = new HashSet<DetalleSencillo>();
        }

        public Guid Id { get; set; }
        public string? Referencia { get; set; }
        public string? Asignacion { get; set; }
        public string? NDoc { get; set; }
        public string? Cla { get; set; }
        public int? Periodo { get; set; }
        public DateTime? FechaDoc { get; set; }
        public string? Io { get; set; }
        public string? LibMayor { get; set; }
        public int? ImporMl { get; set; }
        public string? Texto { get; set; }
        public string? RutProvedor { get; set; }
        public bool? ConciliacionTesoreria { get; set; }

        public virtual ICollection<DetalleSencillo> DetalleSencillos { get; set; }
    }
}
