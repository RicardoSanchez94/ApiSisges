using System;
using System.Collections.Generic;

namespace ServicioApi.Model.SisGes
{
    public partial class DetalleSencillo
    {
        public DetalleSencillo()
        {
            MontoSencillos = new HashSet<MontoSencillo>();
            SencillosTienda = new HashSet<SencillosTienda>();
        }

        public Guid IdSencillo { get; set; }
        public int? IdTienda { get; set; }
        public string? Banco { get; set; }
        public int? Total { get; set; }
        public string? DiaLiberacion { get; set; }
        public string? DiaEntrega { get; set; }
        public DateTime? FechaLiberacion { get; set; }
        public DateTime? FechaEntrega { get; set; }
        public Guid Id { get; set; }
        public int? NuevoTotal { get; set; }
        public bool? ConciliacionTesoreria { get; set; }
        public Guid? IdSencillosSap { get; set; }

        public virtual Sencillo IdSencilloNavigation { get; set; } = null!;
        public virtual SencillosSap? IdSencillosSapNavigation { get; set; }
        public virtual CentrosLocal? IdTiendaNavigation { get; set; }
        public virtual ICollection<MontoSencillo> MontoSencillos { get; set; }
        public virtual ICollection<SencillosTienda> SencillosTienda { get; set; }
    }
}
