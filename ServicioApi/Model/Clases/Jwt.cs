using ServicioApi.Model.SisGes;
using System.Security.Claims;

namespace ServicioApi.Model.Clases
{
    public class Jwt
    {
        public string Key { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public string Subject { get; set; }


        public static dynamic ValidarToken(ClaimsIdentity identity)
        {
            SisgesDBContext db = new SisgesDBContext();
            try
            {
                if (identity.Claims.Count() == 0)
                {
                    return new
                    {

                        success = false,
                        message = "Verificar token",
                        result = ""
                    };
                }
                var idUsuario = identity.Claims.FirstOrDefault(x=>x.Type == "Id").Value;
                Persona persona = db.Personas.Where(x => x.Id == Guid.Parse(idUsuario)).SingleOrDefault();
                return new
                {

                    success = true,
                    message = "Exito",
                    result = persona
                };
            }
            catch (Exception ex)
            {

                return new
                {
                    
                    success= false, 
                    message= ex.ToString(),
                    result = ""
                };
            }
        }

    }
}
