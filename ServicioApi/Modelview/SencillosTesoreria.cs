namespace ServicioApi.Modelview
{
    public class SencillosTesoreria
    {
        public int IdTienda { get; set; }
        public int NuevoTotal { get; set; }
        public string Banco { get; set; }
        public string DiaLiberacion { get; set; }
        public string DiaEntrega { get; set; }

        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }

    }
}
