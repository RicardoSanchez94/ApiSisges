using Microsoft.AspNetCore.Mvc;
using Mysqlx.Cursor;
using Newtonsoft.Json;
using OfficeOpenXml.FormulaParsing.LexicalAnalysis;
using ServicioApi.Model.Clases;
using Swashbuckle.AspNetCore.Annotations;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Net;

namespace ServicioApi.Controllers
{
   // [SwaggerTag("ApiTransbank",)]
    [ApiController]
    [Route("ApiTransbank")]
    public class ApiTransbank : ControllerBase
    {
    
        private Negocio.Negocio ng;
        private Apicliente tbk = new Apicliente();

       
        [HttpGet]
        [Route("ConsultarTBK")]
        public async Task<IActionResult> ConsultaTBK(DateTime fechaInicio)
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
           


            response = await tbk.GetDataTbk(fechaInicio, fechaInicio);
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
