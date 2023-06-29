namespace ServicioApi.Model.Clases
{
    public class ResponseModel
    {
        public string mensaje { get; set; }
        //public string redirect { get; set; }
        public bool error { get; set; }
        public string status { get; set; }

        public string respuesta { get; set; }

        //public string pathQR { get; set; }
        //public string pathArchivoMenu { get; set; }

        public ResponseModel()
        {
            mensaje = "";
            //redirect = "";
            error = true;
            status = "";
            respuesta = "";
            //pathArchivoMenu = "";
        }
    }
}
