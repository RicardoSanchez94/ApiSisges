using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ServicioApi.Model.Clases;

namespace ServicioApi.Controllers
{
    [ApiController]
    [Route("SobranteFaltante")]
    public class SobranteFaltanteController : ControllerBase
    {
        private SobranteFaltante sf = new SobranteFaltante();

        [HttpGet]
        [Route("InsertarSobranteFaltante")]
        public async Task<IActionResult> InsertarSobranteFaltante(DateTime fechaInicio)
        {
            // DateTime fecha = DateTime.Parse(fechaInicio.ToString("dd-MM-yyyy",new CultureInfo("es-cl"))); 
            //Enivar Año-Mes-Dia
            ResponseModel response = new ResponseModel();

            if (fechaInicio >= DateTime.Now.AddDays(-2))
            {
                response.error = true;
                response.mensaje = "Error en los parametros de entrada ,La fecha no puede ser mayor al dia actual";
                return UnprocessableEntity(response);
            }
            response = sf.CargarSobranteFaltanteGlobal(fechaInicio);


            //response = await tbk.GetDataTbk(fechaInicio, fechaInicio);
            //response = tbk.InsertarDatos(null, FormatoInicio);

            var Json = JsonConvert.SerializeObject(response);
            //response.error = true;
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
