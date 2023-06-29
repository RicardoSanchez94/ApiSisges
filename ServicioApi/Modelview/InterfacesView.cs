using ServicioApi.Model.SisGes;

namespace ServicioApi.Modelview
{
    public class InterfacesView
    {
        public string Tipo { get; set; }

        public List<InterfazAutorizacion> ListaInterfazAuto { get; set; }

        public List<InterfazPago> ListaInterfazPago { get; set; }



        public InterfacesView()
        {

            ListaInterfazAuto = new List<InterfazAutorizacion>();
            ListaInterfazPago = new List<InterfazPago>();
            
        }
    }
}

