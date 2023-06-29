using System;
using System.Collections.Generic;

namespace ServicioApi.Model.SisGes
{
    public partial class MontoSencillo
    {
        public Guid Id { get; set; }
        public Guid IdDatalleSencillo { get; set; }
        public int? Monto1 { get; set; }
        public int? Monto2 { get; set; }
        public int? Monto3 { get; set; }
        public int? Monto4 { get; set; }
        public int? Monto5 { get; set; }
        public int? Monto6 { get; set; }
        public int? Monto7 { get; set; }

        public virtual DetalleSencillo IdDatalleSencilloNavigation { get; set; } = null!;
    }
}
