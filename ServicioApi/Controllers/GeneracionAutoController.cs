using Microsoft.AspNetCore.DataProtection.XmlEncryption;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ServicioApi.Model.Clases;
using ServicioApi.Model.Metodos;
using ServicioApi.Model.SisGes;
using Swashbuckle.AspNetCore.Annotations;

namespace ServicioApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GeneracionAutoController : ControllerBase
    {
        private Funciones fn = new Funciones();
        private readonly MySisgesDbcontext _db;
        private Negocio.Negocio ng;
        //private Negocio.Negocio ng = new Negocio.Negocio();
        private EnviodeCorreo enc = new EnviodeCorreo();


        public GeneracionAutoController(IConfiguration configuration, MySisgesDbcontext db)
        {

            //_configuration = configuration;
            //db = new SisgesDBContext();
            _db = db;
            ng = new Negocio.Negocio(_db);
            fn = new Funciones();


        }


        [HttpGet]
        [Route("Listar")]
        public async Task<IActionResult> NuevaPersona(DateTime date)
        {
            ResponseModel response = new ResponseModel();
            response = await fn.Proc_SWITCH(date);
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

        [HttpGet]
        [Route("ConciliacionAutomatica")]
        [SwaggerOperation(
        Summary = "Genera la Conciliacion Automatica de CAAU ",
        Description = "Para Generar la Conciliacion Automatica de CAAU enviar fecha en el formato (yyyy-MM-dd) "
        )]
        public async Task<IActionResult> ConciliacionAutomatica(DateTime fecha)
        {
            ResponseModel response = new ResponseModel();
            response = await fn.spConciliacionAutomaticaCAAU(fecha);
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



        [HttpGet]
        [Route("EnvioCorreo")]
        public async Task<IActionResult> EnvioCorreo(string RespuestaCierre)
        {
            ResponseModel response = new ResponseModel();
            response = enc.EnvioCorreoCierreMes(RespuestaCierre);
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
