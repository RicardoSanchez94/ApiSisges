using System;
using System.Collections.Generic;

namespace ServicioApi.Model.SisGes
{
    public partial class InterfazAutorizacion
    {
        public string NroRef { get; set; } = null!;
        public string CodigoEmpresa { get; set; } = null!;
        public DateTime FechaAutorizacion { get; set; }
        public string CodigoAgencia { get; set; } = null!;
        public int? NumeroTerminal { get; set; }
        public string? NumeroBoleta { get; set; }
        public string? TipoTransaccion { get; set; }
        public string? CodigoUsuario { get; set; }
        public string NumeroAutorizacion { get; set; } = null!;
        public string? TipoArchivo { get; set; }
        public int? CodigoCliente { get; set; }
        public string? NumeroTarjeta { get; set; }
        public int? Monto { get; set; }
        public int? MontoPie { get; set; }
        public int? MesesDiferido { get; set; }
        public int? MesesGracia { get; set; }
        public DateTime? FechaAutoriza { get; set; }
        public int? NumeroCuotas { get; set; }
        public string? Extrafinanciamiento { get; set; }
        public int? CodigoPlanCredito { get; set; }
        public int? MontoAfecto { get; set; }
        public int? CodigoComercio { get; set; }
        public int? CodigoRubro { get; set; }
        public string? RutVendedor { get; set; }
        public int? CodigoPromocionVenta { get; set; }
        public int? MontoCuota { get; set; }
        public DateTime? FechaPrimerVencimiento { get; set; }
        public DateTime? FechaUltimoVencimiento { get; set; }
        public string? Confirmada { get; set; }
        public string? ModoProceso { get; set; }
        public int? NroTrxPos { get; set; }
        public int? NroCaja { get; set; }
        public string? TipoDiferencia { get; set; }
        public string? Estado { get; set; }
        public int? MontonoAfecto { get; set; }
    }
}
