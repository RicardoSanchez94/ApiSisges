using Microsoft.AspNetCore.DataProtection.XmlEncryption;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ServicioApi.Model.Clases;
using ServicioApi.Model.SisGes;
using System.Collections.Generic;
using System.Data;

namespace ServicioApi.Controllers
{
    [Route("Persona")]
    [ApiController]
    public class PersonaController : ControllerBase
    {

        private Negocio.Negocio ng;
        private readonly MySisgesDbcontext _db;

        public PersonaController(IConfiguration configuration, MySisgesDbcontext db)
        {

            //ng = new Negocio.Negocio();
            //enc = new EnviodeCorreo();
            _db = db;
            ng = new Negocio.Negocio(_db);


        }

        [HttpGet]
        [Route("Listar")]
        public string NuevaPersona ()
        {
            var prueba = "";
            return prueba;
        }

        [HttpGet]
        [Route("GetPersona")]
        public string GetPersona()
        {
            var prueba = "";
            return prueba;
        }

        [HttpGet]
        [Route("GetIdoc")]
        public string GetIdoc(string fecha, int local)
        {
            var prueba = ng.FIventas(fecha, local);

            //string Json = string.Empty;
            //Json = JsonConvert.SerializeObject(prueba);

            //prueba.AddRange(JsonConvert.DeserializeObject<List<FnIdoc_FIcs>>(Json));
            return prueba;
        }
    }
}
