using System;
using System.Collections.Generic;

namespace ServicioApi.Model.SisGes
{
    public partial class InterfazPago
    {
        public string NroRef { get; set; } = null!;
        public string? CodigoEmpresa { get; set; }
        public DateTime FechaAutorizacion { get; set; }
        public string CodigoAgencia { get; set; } = null!;
        public int? NumeroTerminal { get; set; }
        public string? CodigoUsuario { get; set; }
        public string? Numerorecibo { get; set; }
        public string? TipoArchivo { get; set; }
        public string? CodigoCliente { get; set; }
        public string? Numerocuenta { get; set; }
        public int? Monto { get; set; }
        public int? CodigoMedioPago { get; set; }
        public int? CodigoTransaccion { get; set; }
        public int? NroTrxPos { get; set; }
        public int? NroCaja { get; set; }
        public string? TipoDiferencia { get; set; }
        public string? Estado { get; set; }
        public string NumeroAutorizacion { get; set; } = null!;
        public string? Referencia { get; set; }

        public virtual TipoTransaccionCapa? CodigoTransaccionNavigation { get; set; }
    }
}
