namespace ServicioApi.Model.Clases
{
    public class FpRegistroTrxs
    {
        public DateTime FechaHora { get; set; }
        public int Local { get; set; }
        public int Caja { get; set; }
        public int NroTrx { get; set; }
        public int Secuencia { get; set; }
        public int RutCliente { get; set; }
        public long Pan { get; set; }
        public long? Monto { get; set; }
        public string CreditType { get; set; }
        public string IsoTipoCredito { get; set; }
        public string Bellbox1 { get; set; }
        public string Iso2 { get; set; }
        public string Iso3 { get; set; }
        public string Bellbox4 { get; set; }
        public string ReqType { get; set; }
        public string IsoReverse1 { get; set; }
        public string IsoReverse2 { get; set; }
        public string Estado { get; set; }
        public DateTime TimeStamp { get; set; }
        public int? Cuenta { get; set; }
        public int? NumeroCuotas { get; set; }
        public int? ValorCuota { get; set; }
        public int? MontoCredito { get; set; }
        public float? TasaInteres { get; set; }
        public string TerminalId { get; set; }
        public int? PiePagoMinimo { get; set; }
        public string AutorizacionFp { get; set; }
        public int? Impuesto { get; set; }
        public int? Comision { get; set; }
        public DateTime? FechaPrimerVenc { get; set; }
        public int? PagoMinEecc { get; set; }
        public int? MontoRedondeado { get; set; }
        public int? MontoRecaudado { get; set; }
        public int? AjusteLey { get; set; }
        public string CodAutorizacion { get; set; }
        public string PagoCodMedioPago { get; set; }
        public string RutCajero { get; set; }
        public int? PaymentId { get; set; }
    }

}
