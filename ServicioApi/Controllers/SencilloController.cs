using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ServicioApi.Model.Clases;
using ServicioApi.Model.Metodos;
using ServicioApi.Model.SisGes;
using ServicioApi.Modelview;
using Swashbuckle.AspNetCore.Annotations;

namespace ServicioApi.Controllers
{

    [ApiController]
    [Route("Sencillo")]
    public class SencilloController : ControllerBase
    {
        private Negocio.Negocio ng;
        private EnviodeCorreo enc;
        private readonly MySisgesDbcontext _db;
        public SencilloController(IConfiguration configuration, MySisgesDbcontext db)
        {

            //ng = new Negocio.Negocio();
            enc = new EnviodeCorreo();
            _db = db;
            ng = new Negocio.Negocio(_db);


        }

        [HttpGet]
        [Route("AlertaSencillo")]
        [Authorize]
        [SwaggerOperation(
    Summary = "Alerta de lo Estado de Sencillo",
    Description = "Envio de Alertas por Correo de los Estado de Sencillo")]
        public IActionResult AlertaSencillo()
        {
            ResponseModel response = new ResponseModel();
            List<ResponseModel> lstresponse = new List<ResponseModel>();

            var Json = JsonConvert.SerializeObject(response);

            response = ng.TiendaSinconciliarSencillo();
            lstresponse.Add(response);

            response = ng.SencilloTransito();
            lstresponse.Add(response);

            response = ng.SencilloAceptado();
            lstresponse.Add(response);

            //response = ng.SencilloDevolucion();
            //lstresponse.Add(response);


            Json = JsonConvert.SerializeObject(lstresponse);

            if (lstresponse.Where(x=>x.error == true).Any())
            {
                return UnprocessableEntity(Json);
            }
            else
            {
                return CreatedAtAction(null, Json);
            }

          

        }

        [HttpPost]
        [Route("AlertaCartaInstruccion")]
        public IActionResult AlertaCartaInstruccion([FromBody] CartaInstrucionView parametros )
        {
            ResponseModel response = new ResponseModel();


            response = enc.CargaCartaInstruccion(parametros.respuesta, parametros.inicio, parametros.fin);

            var Json = JsonConvert.SerializeObject(response);
            if (response.error)
            {
                return UnprocessableEntity(Json);
            }
            else
            {
                return CreatedAtAction(null, Json);
            }
         

           

        }
    }
}
