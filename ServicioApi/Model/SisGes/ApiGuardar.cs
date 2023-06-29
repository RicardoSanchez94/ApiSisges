namespace ServicioApi.Model.SisGes
{
    public class ApiGuardar
    {
        public DateTime fechaTransaccion { get; set; }
        public string numeroTarjeta { get; set; }
        public string tipoProducto { get; set; }
        public string tipoCuota { get; set; }
        public int totalCuotas { get; set; }
        public decimal montoCuotas { get; set; }
        public string codigoAutorizacion { get; set; }
        public decimal montoAfecto { get; set; }
        public string montoExentoTotal { get; set; }
        public int codigoLocal { get; set; }
        public string nombreLocal { get; set; }
        public string tipoTransaccion { get; set; }
        public DateTime? fechaAbono { get; set; }
        public string ordenPedido { get; set; }
        public decimal montoVenta { get; set; }
    }
}
