using System;
using System.Collections.Generic;

namespace ServicioApi.Model.SisGes
{
    public partial class Venta
    {
        public int Trx { get; set; }
        public short? TrxImpreso { get; set; }
        public short? TipoTrx { get; set; }
        public int Local { get; set; }
        public string Cajero { get; set; } = null!;
        public int Caja { get; set; }
        public DateTime FechaVenta { get; set; }
        public DateTime? FechaVtaSistema { get; set; }
        public DateTime? FechaVentaLinea { get; set; }
        public short CajaFueraLinea { get; set; }
        public short ModoEntrenamiento { get; set; }
        public short CodigoAnula { get; set; }
        public string? Cliente { get; set; }
        public string? Vendedor { get; set; }
        public short VentaIniciada { get; set; }
        public decimal? TotalVenta { get; set; }
        public string Caja2 { get; set; } = null!;
        public string? FolioDocumento { get; set; }
        public string? TipoDocumento { get; set; }
        public int? TipoImpto { get; set; }
        public decimal? Impto { get; set; }
        public decimal? Neto { get; set; }
        public decimal Bruto { get; set; }
    }
}
