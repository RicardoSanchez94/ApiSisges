using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ServicioApi.Model.Clases;
using ServicioApi.Model.Metodos;
using ServicioApi.Model.SisGes;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;
using System.Globalization;

namespace ServicioApi.Controllers
{
    [ApiController]
    [Route("Ventas")]
    //[Route("Ventas/[controller]")]


    public class IdocController : ControllerBase
    {


        //private Negocio.Negocio ng = new Negocio.Negocio();
        private Negocio.Negocio ng;
        private readonly Funciones _fn;
        private readonly MySisgesDbcontext _db;



        private readonly ILogger<IdocController> _logger;
        public IdocController(ILogger<IdocController> logger, MySisgesDbcontext db)
        {
            _fn = new Funciones();
            _logger = logger;
            _db = db;
            ng = new Negocio.Negocio(_db);

        }


        [HttpGet]
        [Route("Barrido")]
        public string Get(string Fecha1, string Fecha2)
        {
            //var prueba = ng.GetTiendas();
            //var prueba2 = ng.GetTrx(prueba);
            var prueba3 = ng.EjecutarBarrido(Fecha1, Fecha2);

            return ("ok");
        }

        [HttpGet]
        [Route("DepositoRendicion")]

        public async Task<dynamic> DepositoRendicion(DateTime Fecha)
        {
            var ejeccuion = _fn.Proc_GENERAR_DEPOSITOS(Fecha);
            var ejeccuion2 = await _fn.Proc_GENERAR_RENDICIONES(Fecha);
            return (new { estado = "exitoso" });

        }


        [HttpGet]
        [Route("GetIdocVentasRP")]
        [SwaggerOperation(
        Summary = "Generacion de Idoc de Ventas ",
        Description = "Para Generar los Idocs de Venta enviar fecha en el formato (yyyy-MM-dd) y para enviar todas las tienda mandar 0, ademas si desea enviar el ambiente seria 1 = PRODUCCION, 0 = QA "
        )]
        public IActionResult GetIdocVentasRP(DateTime Fecha, int local, string ambiente)
        {
            List<ResponseModel> lstResponse = new List<ResponseModel>();
            var Json = JsonConvert.SerializeObject(lstResponse);

            var sap = Helper.Helper.AmbienteSAP(ambiente);
            lstResponse = ng.EjecutarVentasRp(Fecha, local, sap);

            Json = JsonConvert.SerializeObject(lstResponse);

            if (lstResponse.Where(x => x.error == true).Any())
            {
                return UnprocessableEntity(Json);
            }
            else
            {
                return CreatedAtAction(null,Json);
            }
            
        }
        [HttpGet]
        [Route("GetIdocNCFC_RP")]
        [SwaggerOperation(
        Summary = "Generacion de Idoc de NCFC ",
        Description = "Para Generar los Idocs de NCFC enviar fecha en el formato (yyyy-MM-dd) y para enviar todas las tienda mandar 0, ademas si desea enviar el ambiente seria 1 = PRODUCCION, 0 = QA "
        )]
        public IActionResult GetIdocNCFC_RP(DateTime Fecha, int local, string ambiente)
        {
            List<ResponseModel> lstResponse = new List<ResponseModel>();
            var Json = JsonConvert.SerializeObject(lstResponse);

            var sap = Helper.Helper.AmbienteSAP(ambiente);
            lstResponse = ng.EjecutarNCFC_RP(Fecha, local, sap);

            Json = JsonConvert.SerializeObject(lstResponse);

            if (lstResponse.Where(x => x.error == true).Any())
            {
                return UnprocessableEntity(Json);
            }
            else
            {
                return CreatedAtAction(null, Json);
            }
        }

        [HttpGet]
        [Route("GetIdocClientes_RP")]
        [SwaggerOperation(
        Summary = "Generacion de Idoc de Clientes ",
        Description = "Para Generar los Idocs de Clientes enviar fecha en el formato (yyyy-MM-dd) y para enviar todas las tienda mandar 0, ademas si desea enviar el ambiente seria 1 = PRODUCCION, 0 = QA "
        )]
        public IActionResult GetIdocClientes_RP(DateTime Fecha, int local, string ambiente)
        {
            List<ResponseModel> lstresponse = new List<ResponseModel>();
            var Json = JsonConvert.SerializeObject(lstresponse);

            var sap = Helper.Helper.AmbienteSAP(ambiente);
            lstresponse = ng.EjecutarClientes_RP(Fecha, local, sap);

            Json = JsonConvert.SerializeObject(lstresponse);

            if (lstresponse.Where(x => x.error == true).Any())
            {
                return UnprocessableEntity(Json);
            }
            else
            {
                return CreatedAtAction(null, Json);
            }
        }

        [HttpGet]
        [Route("GetIdocFIVenta_RP")]
        public string GetIdocFIVenta_RP(DateTime Fecha, int local, string ambiente)
        {
            var prueba = ng.EjecutarFiVentasRp(Fecha, local, ambiente);

            //var asd = ng.GenerarIdocFI(lst);
            return prueba.mensaje;
        }
        [HttpGet]
        [Route("GetIdocFIPagos_RP")]
        [SwaggerOperation(
    Summary = "Obtiene los IDOCs de FI Pagos para RP",
    Description = "Retorna los IDOCs de FI Pagos para un determinado local y fecha."
)]
        [SwaggerRequestExample(typeof(IdocFIPagos_RP), typeof(IdocFIPagos_RPExample))]
        public string GetIdocFIPagos_RP(DateTime Fecha, int local, string ambiente)
        {
            var prueba = ng.EjecutarFiPagosRp(Fecha, local, ambiente);

            //var asd = ng.GenerarIdocFI(lst);
            return prueba.mensaje;
        }


        [HttpGet]
        [Route("GetVentas")]
        public string GetVentas(string Fecha, int local)
        {
            //var prueba = ng.GetTiendas();
            //var prueba2 = ng.GetTrx(prueba);
            //int Ano = int.Parse(Fecha.Substring(0, 4));
            //int Moth = int.Parse(Fecha.Substring(4, 2));
            //int day = int.Parse(Fecha.Substring(6, 2));
            string format = "yyyyMMdd";

            var FechaFormato = DateTime.ParseExact(Fecha, format, CultureInfo.InvariantCulture);
            //DateTime prueba = new DateTime(FechaFormato);

            var prueba3 = ng.VentasActual(FechaFormato, local);

            return (prueba3);
        }

        [HttpGet]
        [Route("GetIdocMPVentas_RP")]
        [SwaggerOperation(
        Summary = "Generacion de Idoc de MP Ventas ",
        Description = "Para Generar los Idocs de Clientes enviar fecha en el formato (yyyy-MM-dd) y para enviar todas las tienda mandar 0, ademas si desea enviar el ambiente seria 1 = PRODUCCION, 0 = QA "
        )]
        public IActionResult GetIdocMPVentas_RP(DateTime Fecha, int local, string ambiente)
        {
            List<ResponseModel> lstResponse = new List<ResponseModel>();
            var Json = JsonConvert.SerializeObject(lstResponse);

            var sap = Helper.Helper.AmbienteSAP(ambiente);
            lstResponse = ng.EjecutarMPVentas_RP(Fecha, local, sap);

            Json = JsonConvert.SerializeObject(lstResponse);

            if (lstResponse.Where(x => x.error == true).Any())
            {
                return UnprocessableEntity(Json);
            }
            else
            {
                return CreatedAtAction(null, Json);
            }
        }

        //[HttpGet]
        //[Route("GetIdocMPVentas_RP")]
        //[SwaggerOperation(
        //Summary = "Generacion de Idoc de MP Ventas ",
        //Description = "Para Generar los Idocs de Clientes enviar fecha en el formato (yyyy-MM-dd) y para enviar todas las tienda mandar 0, ademas si desea enviar el ambiente seria 1 = PRODUCCION, 0 = QA "
        //)]
        //public string GetIdocMPVentas_RP(DateTime Fecha, int local, string ambiente)
        //{

        //    var sap = Helper.Helper.AmbienteSAP(ambiente);
        //    var prueba3 = ng.EjecutarMPVentas_RP(Fecha, local, sap);

        //    return ("ok");
        //}

    }

}
