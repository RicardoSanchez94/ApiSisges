using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Utilities;
using ServicioApi.Model.Clases;
using ServicioApi.Model.Metodos;
using ServicioApi.Model.SisGes;
using ServicioApi.Modelview;
using Swashbuckle.AspNetCore.Annotations;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.IdentityModel.Tokens.Jwt;
using System.Numerics;
using System.Runtime.InteropServices.ObjectiveC;
using System.Security.Claims;
using System.Text;

namespace ServicioApi.Controllers
{
    [ApiController]
    [Route("Pruebas")]
    public class PruebasController : ControllerBase
    {
        private Negocio.Negocio ng;
        private Funciones fn;
        //private SisgesDBContext db;
        private IConfiguration _configuration;
        private IHttpContextAccessor _httpContextAccessor;
        //private InsertCAAUCAPAcs ca = new InsertCAAUCAPAcs();
        private InsertCAAUCAPAcs ca;
        private readonly MySisgesDbcontext _db;


        public PruebasController(IConfiguration configuration,MySisgesDbcontext db)
        {

            _configuration = configuration;
            //db = new SisgesDBContext();
            _db = db;
            ng = new Negocio.Negocio(_db);
            fn = new Funciones();
            ca = new InsertCAAUCAPAcs(_db);


        }

        [HttpPost]
        [Route("LecturaArchivosSFT")]
        [Authorize]
        [SwaggerOperation(
    Summary = "Consumo de Archivos del SFTP",
    Description = "Lectura y consumo de las interfaces CAAU/CAPA.. Recibe parametro fecha : yyyy-MM-dd"
)]
        public async Task<IActionResult> LecturaArchivosSFT([FromBody] LecturaArchivoSFTPView lectura)
        {
            ResponseModel response = new ResponseModel();
            response = await ca.StarProceso(lectura.Fecha);
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
        public string EnvioCorreo()
        {
            var prueba = "";
            prueba = ng.EnvioCorreo().mensaje;
            return prueba;
        }


        [HttpGet]
        [Route("AlertaSencillo")]
        public string AlertaSencillo()
        {
            var prueba = "";
            prueba = ng.TiendaSinconciliarSencillo().mensaje;
            return prueba;
        }

        [HttpGet]
        [Route("GetPersona")]
        public string GetPersona()
        {
            var prueba = "";
            ng.GenerarExcel();
            return prueba;
        }

        //[HttpPost]
        //[Route("GetTiendaIdocFI")]
        //[Authorize]
        //public string GetIdocFI(DateTime Fecha, int Tienda)
        //{
        //    var prueba = "";
        //    List<FIMP> lst = new List<FIMP>();

        //    var identity = HttpContext.User.Identity as ClaimsIdentity;
        //    //string token = Request.Headers.Where(x => x.Key == "Authorization").FirstOrDefault().Value;
        //    var rToken = Jwt.ValidarToken(identity);


        //    //foreach (var item in db.CentrosLocals.Where(x=> x.CenEstado.Equals("Y")))
        //    //{
        //    //    lst.AddRange(JsonConvert.DeserializeObject<List<FIMP>>(fn.IdocFi(Fecha, item.CenCodigo)).ToList());
        //    //}

        //    lst = JsonConvert.DeserializeObject<List<FIMP>>(fn.IdocFi(Fecha, Tienda)).ToList();
        //    var asd = ng.GenerarIdocFIVentas(lst, Fecha);

        //    Auditorium auditorium = (new Auditorium
        //    {
        //        IdUsuario = rToken.result.Id,
        //        Fecha = DateTime.Now,
        //        Descripcion = $"Se genero idoc FI con la Tienda: {Tienda} Por el usuario: {rToken.result.Run}"
        //    });
        //    ng.Auditoria(auditorium);

        //    return prueba;
        //}
        //[HttpGet]
        //[Route("GetFullIdocFI")]
        //public string GetIdocFI(string Fecha)
        //{
        //    var prueba = "";
        //    List<FIMP> lst = new List<FIMP>();
        //    foreach (var item in db.CentrosLocals.Where(x => x.CenEstado.Equals("Y")))
        //    {
        //        lst.AddRange(JsonConvert.DeserializeObject<List<FIMP>>(fn.IdocFi(Fecha, item.CenCodigo)).ToList());
        //    }

        //    var asd = ng.GenerarIdocFIVentas(lst);
        //    return prueba;
        //}
        [HttpPost]
        [Route("Token")]
        [SwaggerOperation(
    Summary = "Genera Token",
    Description = "Para Genera el Token enviar Rut con guion y dijito verificador mas la contraseña con la que se logueo en el SistemaGestion "
)]
        public dynamic Token([FromBody] LoginView login)
        {
            //var data = JsonConvert.DeserializeObject<Login>(login.ToString());
            var prueba = "";
            //Usuario user = db.Personas.Where(x => x.Run.Replace(".", "") == login.Rut.Replace(".", "")).Select(x => x.Usuario).SingleOrDefault();
            Usuario user = _db.Personas.Where(x => x.Run.Replace(".", "") == login.rut.Replace(".", "")).Select(x => x.Usuario).SingleOrDefault();

            //var matched = BCrypt.Net.BCrypt.Verify(pass, usuario.Password);

            if (BCrypt.Net.BCrypt.Verify(login.pass, user.Password))
            {
                var jwt = _configuration.GetSection("Jwt").Get<Jwt>();

                var claims = new[]
                {
                    new Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub, jwt.Subject),
                    new Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                    new Claim("Id", user.Id.ToString())
                    //new Claim("Id", user.Id.ToString()),

                };
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key));
                var singIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

                var token = new JwtSecurityToken(
                    jwt.Issuer,
                    jwt.Audience,
                    claims,
                    expires: DateTime.Now.AddMinutes(0),
                    signingCredentials: singIn
                    );

                return new
                {
                    success = true,
                    message = "exito",
                    result = new JwtSecurityTokenHandler().WriteToken(token)
                };

            }

           

            return prueba;
        }
        [HttpGet]
        [Route("getMySQL")]
        public dynamic getMySQL(DateTime Fecha)
        {
            string servidor = "172.18.14.177"; //Nombre o ip del servidor de MySQL
            string bd = "maestro_cliente_ic"; //Nombre de la base de datos
            string usuario = "cuadratura"; //Usuario de acceso a MySQL
            string password = "tHD*6BeK$2"; //Contraseña de usuario de acceso a MySQL
            string datos = null; //Variable para almacenar el resultado

            //Crearemos la cadena de conexión concatenando las variables
            string cadenaConexion =  "Server=" + servidor + "; Database=" + bd + ";User Id=" + usuario + "; Password=" + password + ";";

            //Instancia para conexión a MySQL, recibe la cadena de conexión
            MySqlConnection conexionBD = new MySqlConnection(cadenaConexion);
            MySqlDataReader reader = null; //Variable para leer el resultado de la consulta

            //Agregamos try-catch para capturar posibles errores de conexión o sintaxis.
            try
            {
                string consulta = $"SELECT * FROM fp_registro_trxs WHERE Year(time_stamp) = {Fecha.Year} and month(time_stamp) = {Fecha.Month} and day(time_stamp) = {Fecha.Day}";
                //string consulta = "SELECT * FROM fp_registro_trxs WHERE   Year(time_stamp) = 2023 and month(time_stamp) = 02 and day(time_stamp) = 14"; //Consulta a MySQL (Muestra las bases de datos que tiene el servidor)
                MySqlCommand comando = new MySqlCommand(consulta); //Declaración SQL para ejecutar contra una base de datos MySQL
                comando.Connection = conexionBD; //Establece la MySqlConnection utilizada por esta instancia de MySqlCommand
                conexionBD.Open(); //Abre la conexión
                reader = comando.ExecuteReader(); //Ejecuta la consulta y crea un MySqlDataReader
                
                var dt = new DataTable();
                dt.Columns.Add("fecha_hora", typeof(DateTime));
                dt.Columns.Add("local", typeof(int));
                dt.Columns.Add("caja", typeof(int));
                dt.Columns.Add("nro_trx", typeof(int));
                dt.Columns.Add("secuencia", typeof(int));
                dt.Columns.Add("rut_cliente", typeof(int));
                dt.Columns.Add("pan", typeof(Int64));
                dt.Columns.Add("monto", typeof(Int64));
                dt.Columns.Add("credit_type", typeof(string));
                dt.Columns.Add("iso_tipo_credito", typeof(string));
                dt.Columns.Add("bellbox1", typeof(string));
                dt.Columns.Add("iso2", typeof(string));
                dt.Columns.Add("iso3", typeof(string));
                dt.Columns.Add("bellbox4", typeof(string));
                dt.Columns.Add("req_type", typeof(string));
                dt.Columns.Add("iso_reverse1", typeof(string));
                dt.Columns.Add("iso_reverse2", typeof(string));
                dt.Columns.Add("estado", typeof(string));
                dt.Columns.Add("time_stamp", typeof(DateTime));
                dt.Columns.Add("cuenta", typeof(int));
                dt.Columns.Add("numero_cuotas", typeof(int));
                dt.Columns.Add("valor_cuota", typeof(int));
                dt.Columns.Add("monto_credito", typeof(int));
                dt.Columns.Add("tasa_interes", typeof(decimal));
                dt.Columns.Add("terminal_id", typeof(string));
                dt.Columns.Add("pie_pago_minimo", typeof(int));
                dt.Columns.Add("autorizacion_fp", typeof(string));
                dt.Columns.Add("impuesto", typeof(int));
                dt.Columns.Add("comision", typeof(int));
                dt.Columns.Add("fecha_primer_venc", typeof(DateTime));
                dt.Columns.Add("pago_min_eecc", typeof(int));
                dt.Columns.Add("monto_redondeado", typeof(int));
                dt.Columns.Add("monto_recaudado", typeof(int));
                dt.Columns.Add("ajuste_ley", typeof(int));
                dt.Columns.Add("cod_autorizacion", typeof(string));
                dt.Columns.Add("pago_cod_medio_pago", typeof(string));
                dt.Columns.Add("rut_cajero", typeof(string));
                dt.Columns.Add("payment_id", typeof(int));

                dt.Load(reader);
                //var Json = JsonConvert.SerializeObject(dt);

                //List<FpRegistroTrxs> listaTransacciopnes= new List<FpRegistroTrxs>();
                //foreach (var item in dt.Rows)
                //{
                //    var oTransacciones = new FpRegistroTrxs();
                //    oTransacciones.FechaHora = 
                //}

                //var chanchito = JsonConvert.DeserializeObject<List<FpRegistroTrxs>>(Json);

                //var chanchitotriste = chanchito.ToList();

                //dt = JsonConvert.DeserializeObject<DataTable>(Json);

                conexionBD.Close();
                var con = Helper.Helper.ConexionStringModel();
                using (SqlConnection sql = new SqlConnection(con))
                {
                    sql.Open();
                    //_logger.LogInformation("Apertura de Conexion BD SisGes");
                    Debug.WriteLine("Apertura de Conexion BD SisGes");
                    using (SqlCommand cmd = new SqlCommand("spInserfP_registro_trxs", sql))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.CommandTimeout= 10000000;

                        cmd.Parameters.AddWithValue("@TAU", dt);
                        //cmd.Parameters.AddWithValue("@TAU", null);
                        cmd.Parameters.AddWithValue("@Fecha", Fecha);

                        //parametros.SqlDbType = SqlDbType.Structured;
                        var reader1 = cmd.ExecuteNonQuery();
                    }
                    sql.Close();
                }

                //while (reader.Read()) //Avanza MySqlDataReader al siguiente registro
                //{
                //    datos += reader.GetString(0) + "\n"; //Almacena cada registro con un salto de linea

                //}
                return new ResponseModel();
            }
            catch (MySqlException ex)
            {
                return new ResponseModel();
            }
            finally
            {
                
                conexionBD.Close(); //Cierra la conexión a MySQL
            }
        }
    }
}
