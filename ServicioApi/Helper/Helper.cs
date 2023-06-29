using ServicioApi.Model.Clases;
using ServicioApi.Model.SisGes;
using System.Globalization;

namespace ServicioApi.Helper
{
    public class Helper
    {
        public static string ConexionStringModel()
        {

            var builder = new ConfigurationBuilder()
              .SetBasePath(AppContext.BaseDirectory)
              .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            IConfiguration configuration = builder.Build();
            var conexion = configuration.GetSection("ConnectionStrings:StringConexionSisges");
            return conexion.Value;
        }

        public static string ConexionGlobalModel()
        {

            var builder = new ConfigurationBuilder()
              .SetBasePath(AppContext.BaseDirectory)
              .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            IConfiguration configuration = builder.Build();
            var conexion = configuration.GetSection("ConnectionModelGlobal:StringConexionGlobal");
            return conexion.Value;
        }

        public static AmbienteSAP AmbienteSAP(string ambiente)
        {

            var builder = new ConfigurationBuilder()
              .SetBasePath(AppContext.BaseDirectory)
              .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            IConfiguration configuration = builder.Build();
            //var conexion = configuration.GetSection("AmbienteSAP:" + ambiente + ":Ambiente");
            var model = configuration.GetSection("AmbienteSAP:" + ambiente + "").Get<AmbienteSAP>();
            return model;
        }

        public static CredencialesCorreo GetCrendicalesCorreo()
        {
            var model = new CredencialesCorreo();
            var builder = new ConfigurationBuilder()
               //.SetBasePath(Directory.GetCurrentDirectory())
               .SetBasePath(AppContext.BaseDirectory)
              .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            IConfiguration configuration = builder.Build();
            model = configuration.GetSection("ConnetionCorreo").Get<CredencialesCorreo>();
            return model;
        }

        public static CredencialesTBK ApiTransbank()
        {
            var model = new CredencialesTBK();
            var builder = new ConfigurationBuilder()
               //.SetBasePath(Directory.GetCurrentDirectory())
               .SetBasePath(AppContext.BaseDirectory)
              .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            IConfiguration configuration = builder.Build();
            model = configuration.GetSection("ApiTransbank").Get<CredencialesTBK>();
            return model;
        }

        public static sFTPCredentials GetCrendicales()
        {
            var model = new sFTPCredentials();
            var builder = new ConfigurationBuilder()
               //.SetBasePath(Directory.GetCurrentDirectory())
               .SetBasePath(AppContext.BaseDirectory)
              .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            IConfiguration configuration = builder.Build();
            model = configuration.GetSection("CredencialesSFTP").Get<sFTPCredentials>();
            return model;
        }

        public static sFTPCredentials GetCrendicalesSAP()
        {
            var model = new sFTPCredentials();
            var builder = new ConfigurationBuilder()
               //.SetBasePath(Directory.GetCurrentDirectory())
               .SetBasePath(AppContext.BaseDirectory)
              .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            IConfiguration configuration = builder.Build();
            model = configuration.GetSection("CredencialesSAPSFTP").Get<sFTPCredentials>();
            return model;
        }

        public static DateTime GetDateTimeFormatWhitTime(string date)
        {
            string formatOutPut = "yyyy/MM/dd HH:mm:ss";
            DateTime dt1;
            DateTime.TryParseExact(date, "ddMMyyyy HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt1);

            var dateResult = Convert.ToDateTime(dt1.ToString(formatOutPut, CultureInfo.InvariantCulture));
            return dateResult;
        }

        public static DateTime GetDateTimeFormatDate(string date)
        {
            try
            {
                DateTime dateResult = new DateTime(int.Parse(date.Substring(4, 4)), int.Parse(date.Substring(2, 2)), int.Parse(date.Substring(0, 2)));
                //string formatOutPut = "yyyy/MM/dd";
                //DateTime dt1;

                //DateTime.TryParseExact(date, formatOutPut, CultureInfo.InvariantCulture, DateTimeStyles.None, out dt1);
                //var dateResult = Convert.ToDateTime(dt1.ToString(formatOutPut, CultureInfo.InvariantCulture));
                return dateResult;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public static int GetCodigoTransaccion(string Tipo)
        {
            int number = 0;
            var M = Tipo.ToUpper();
            try
            {
                switch (M)
                {
                    case "PAGO":
                        number = 62;
                        break;
                    case "PAGO PROMOCION COBRANZA":
                        number = 63;
                        break;
                    case "PAGO PROMOCION CASTIGO":
                        number = 64;
                        break;
                    case "PREPAGO DEUDA TOTAL":
                        number = 65;
                        break;
                    case "PAGO PIE RENEGOCIACION":
                        number = 66;
                        break;
                    case "PAGO PIE REFINANCIAMIENTO":
                        number = 67;
                        break;
                    default:
                        number = 0;
                        break;
                }
                return number;
            }
            catch (Exception)
            {

                throw;
            }

        }


    }
}
