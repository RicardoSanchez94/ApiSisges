using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using ServicioApi.Model.Clases;
using ServicioApi.Model.SisGes;
using ServicioApi.Modelview;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Mail;
using System.Net;
using System.Text;
using ServicioApi.Model.Metodos;
using Microsoft.AspNetCore.Hosting.Server;
using OfficeOpenXml;
using System.Data.Entity.Core.Common;
using System.Text.RegularExpressions;
using Renci.SshNet;
using System.Globalization;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using static Org.BouncyCastle.Math.EC.ECCurve;
//using System.Data.Entity;

namespace ServicioApi.Negocio
{
    public class Negocio
    {
        //private SisgesDBContext db = new SisgesDBContext();
        private readonly MySisgesDbcontext _db;
        
        //private CredencialesCorreo CredencialesCorreo = Helper.Helper.GetCrendicalesCorreo();
        private Funciones fn = new Funciones();
        private Funciones f = new Funciones();
        private EnviodeCorreo correo = new EnviodeCorreo();

        
        public Negocio(MySisgesDbcontext db)
        {
            _db = db;
        }

        #region SencilloTienda
        public ResponseModel TiendaSinconciliarSencillo()
        {
            ResponseModel response = new ResponseModel();
            try
            {
                List<SencillosTesoreria> lst = new List<SencillosTesoreria>();
                Sencillo sn = new Sencillo();
                sn = _db.Sencillos.Where(x => x.Fecha.Value.Year == DateTime.Now.Year).OrderByDescending(x => x.Correlativo).FirstOrDefault();

                lst = _db.fnSencillosTiendaNoConciliados.Where(x => x.FechaInicio == sn.FechaInicio && x.FechaFin == sn.FechaFin).ToList();


                if (lst is null)
                {
                    response.error = false;
                    response.respuesta = "El Metodo Tienda Sin conciliar trajo null no se envio alertas ";
                    return response;
                }
                if (lst.Count != 0)
                {
                    response = correo.EnviodeAlertaTesoreria(lst.OrderBy(x=>x.IdTienda).ToList());
                }

                response.error = false;
                response.respuesta = "Se enviaron las alertas correctamente SencilloDevolucion";
                return response;

            }
            catch (Exception ex)
            {

                response.error = true;
                response.respuesta = "Error SencilloTransito" + ex;
                return response;
            }

        }
        public static DateTime? ObtenerFechaPorDiaDeLaSemana(DateTime fechaInicio, DateTime fechaFin, string diaDeLaSemana)
        {
            if (fechaInicio > fechaFin)
            {
                Console.WriteLine("La fecha de inicio no puede ser mayor que la fecha final.");
                return null;
            }
            diaDeLaSemana = diaDeLaSemana.ToLower();
            Dictionary<string, string> diasDeLaSemana = new Dictionary<string, string>()
            {
                {"lunes", "monday"},
                {"martes", "tuesday"},
                {"miercoles", "wednesday"},
                {"jueves", "thursday"},
                {"viernes", "friday"},
                {"sabado", "saturday"},
                {"domingo", "sunday"}
            };
            DayOfWeek dia;
            if (diasDeLaSemana.TryGetValue(diaDeLaSemana, out string diaEnIngles))
            {

                try
                {
                    dia = (DayOfWeek)Enum.Parse(typeof(DayOfWeek), diaEnIngles, true);
                    for (DateTime fechaActual = fechaInicio; fechaActual <= fechaFin; fechaActual = fechaActual.AddDays(1))
                    {
                        if (fechaActual.DayOfWeek == dia)
                        {
                            return fechaActual;
                        }
                    }
                    /*Console.WriteLine("No se encontró el día de la semana especificado en el rango de fechas dado.");*/
                    return null;
                }
                catch (ArgumentException)
                {
                    //Console.WriteLine("El día de la semana proporcionado no es válido.");
                    return null;
                }


            }
            else
            {
                return null;
            }

        }
        public ResponseModel SencilloTransito()
        {
            ResponseModel response = new ResponseModel();

            try
            {
                List<SencillosTienda> lst = new List<SencillosTienda>();
                lst = _db.SencillosTiendas.Where(x => x.CodigoEstadoSencillo == 2).Include(x => x.IdDetalleSencilloNavigation).ThenInclude(x => x.IdSencilloNavigation).Include(x => x.CodigoEstadoSencilloNavigation).Include(x => x.IdTiendaNavigation).Include(x => x.IdTiendaNavigation).ToList().
                    Where(x => x.IdDetalleSencilloNavigation.FechaEntrega <= DateTime.Now.Date
                    //DateTime? fechaEspecifica = ObtenerFechaPorDiaDeLaSemana(x.IdDetalleSencilloNavigation.IdSencilloNavigation.FechaInicio.Value, x.IdDetalleSencilloNavigation.IdSencilloNavigation.FechaFin.Value, x.IdDetalleSencilloNavigation.DiaEntrega);
                    //return fechaEspecifica.HasValue && fechaEspecifica.Value.Date <= DateTime.Now.Date;
                    ).ToList();

                //if (lst.Any(x => x.IdDetalleSencilloNavigation.FechaEntrega.Value > DateTime.Now.Date.AddDays(1)))
                //{
                //    response = correo.EnviodeAlertaTiendaSinAceptar(lst, "Envio de Alertas Sencillos Sin Aceptar");
                //}

                var listafiltrada = lst.Where(x => x.IdDetalleSencilloNavigation.FechaEntrega.Value.Date.AddDays(3) <= DateTime.Now.Date).ToList();

                if (listafiltrada.Count != 0)
                {
                    response = correo.EnviodeAlertaTiendaSinAceptar(listafiltrada, "Alertas Sencillos No Aceptados");
                }

                if (lst is null)
                {
                    response.error = false;
                    response.respuesta = "Se enviaron las alertas correctamente SencilloDevolucion";
                    return response;
                }
                if (lst.Count != 0)
                {
                    response = correo.EnviodeAlertaSencillos(lst.OrderBy(x => x.IdTienda).ToList(), "Alertas Sencillo Transito");
                    response = correo.EnviodeAlertaSencillosTiendas(lst.OrderBy(x => x.IdTienda).ToList(), "Alertas Sencillo Transito");
                }


                response.error = false;
                response.respuesta = "Se enviaron las alertas correctamente SencilloTransito";
                return response;
            }
            catch (Exception ex)
            {

                response.error = true;
                response.respuesta = "Error SencilloTransito" + ex;
                return response;
            }


        }
        public ResponseModel SencilloAceptado()
        {
            ResponseModel response = new ResponseModel();



            try
            {
                List<SencillosTienda> lst = new List<SencillosTienda>();
                List<SencillosTienda> lst2 = new List<SencillosTienda>();
                //List<SencillosTienda> lst3 = new List<SencillosTienda>();
                lst = _db.SencillosTiendas.Where(x => x.CodigoEstadoSencillo == 3)
              .Include(x => x.IdDetalleSencilloNavigation)
              .ThenInclude(x => x.IdSencilloNavigation)
              .Include(x => x.CodigoEstadoSencilloNavigation)
              .Include(x => x.IdTiendaNavigation).ToList();

                //lst3 = db.SencillosTiendas.Where(x => x.CodigoEstadoSencillo == 3 || x.CodigoEstadoSencillo == 2)
                // .Include(x => x.IdDetalleSencilloNavigation)
                // .ThenInclude(x => x.IdSencilloNavigation)
                // .Include(x => x.CodigoEstadoSencilloNavigation)
                // .Include(x => x.IdTiendaNavigation).ToList();




                var tiendasAgrupadas = lst.GroupBy(t => t.IdTienda).Select(group => new { Id = group.Key, Nombre = group.First().IdTienda, Cantidad = group.Count() }).ToList();

               
                //var ultimo = lst.Max(x => x.IdDetalleSencilloNavigation.IdSencilloNavigation.Correlativo);
                foreach (var item in tiendasAgrupadas.Where(x => x.Cantidad > 1))
                {
                    var ultimocorrelativo = lst.Where(x => x.IdTienda == item.Id.Value).Max(x => x.IdDetalleSencilloNavigation.IdSencilloNavigation.Correlativo);
                    lst2.AddRange(lst.Where(x => x.IdTienda == item.Id.Value && x.IdDetalleSencilloNavigation.IdSencilloNavigation.Correlativo != ultimocorrelativo));

                }
                if (lst2 is null)
                {
                    response.error = false;
                    response.respuesta = "Se enviaron las alertas correctamente SencilloDevolucion";
                    return response;
                }
                if (lst2.Count != 0)
                {
                    //var ultimoCorrelativo = lst.Where(x => x.CodigoEstadoSencillo == 3)
                    //       .Max(x => x.IdDetalleSencilloNavigation.IdSencilloNavigation.Correlativo);
                   // var resultados = lst.Where(x =>x.IdDetalleSencilloNavigation.IdSencilloNavigation.Correlativo != ultimo)
                   //.ToList();

                    response = correo.EnviodeAlertaSencillos(lst2, "Alertas Sencillo Aceptado");
                    response = correo.AlertaSencillosDevolucionTienda(lst2, "Alerta Sencillos por Devolver");
                }


                response.error = false;
                response.respuesta = "Se enviaron las alertas correctamente SencilloAceptado";
                return response;


            }
            catch (Exception ex)
            {

                response.error = true;
                response.respuesta = "Error SencilloAceptado" + ex;
                return response;
            }

        }
        public ResponseModel SencilloDevolucion()
        {
            ResponseModel response = new ResponseModel();
            try
            {
                List<SencillosTienda> lst = new List<SencillosTienda>();
                lst = _db.SencillosTiendas.Where(x => x.CodigoEstadoSencillo == 4)
                    .Include(x => x.IdDetalleSencilloNavigation)
                    .Include(x => x.CodigoEstadoSencilloNavigation)
                    .Include(x => x.IdTiendaNavigation)
                    .Include(x => x.Remitos).ToList();
                if (lst is null)
                {
                    response.error = false;
                    response.respuesta = "Se enviaron las alertas correctamente SencilloDevolucion";
                    return response;
                }
                if (lst.Count != 0)
                {
                    response = correo.EnviodeAlertaSencillos(lst, "Alertas Sencillo Devolucion");
                }
                response.error = false;
                response.respuesta = "Se enviaron las alertas correctamente SencilloDevolucion";
                return response;
            }
            catch (Exception ex)
            {

                response.error = true;
                response.respuesta = "Error SencilloDevolucion" + ex;
                return response;
            }
        }

        #endregion

        #region Metodos
        public ResponseModel Auditoria(Auditorium auditoria)
        {
            ResponseModel response = new ResponseModel();
            try
            {
                _db.Add(auditoria);
                _db.SaveChanges();
                response.error = false;
                response.mensaje = "Se a guardado la auditoria correctamente";
                return response;
            }
            catch (Exception ex)
            {
                response.error = true;
                response.mensaje = "Ocurrio un error" + ex.Message;
                return response;

            }
        }
        public List<Trx> GetTrx(List<Tiendas> lst)
        {
            List<Trx> list2 = new List<Trx>();
            foreach (var item in lst.Where(x => x.Habilitado.Equals("Y")))
            {
                if (item.ServidorIP != null || item.ServidorIP != "")
                {
                    using (SqlConnection conn = new SqlConnection(item.Conexion))
                    {
                        conn.Open();
                        //_logger.LogInformation("Conexion con Servidores: " + item.ServidorIP);
                        using (SqlCommand cmd = new SqlCommand("prueba", conn))
                        {
                            cmd.CommandText = "Select * from fn_NCFC (2'20220927','SAPFSP')";
                            var reader = cmd.ExecuteReader();

                            //var r = Serializer(reader.Read());
                            var dt = new DataTable();
                            dt.Load(reader);
                            string Json = string.Empty;
                            Json = JsonConvert.SerializeObject(dt);

                            list2.AddRange(JsonConvert.DeserializeObject<List<Trx>>(Json));

                            //while (reader.Read())
                            //{
                            //    list2.Add(new Trx()
                            //    {
                            //        Id = Convert.ToInt32(reader["TRAN_ID"].ToString()),
                            //        Fecha = DateTime.Parse(reader["TRAN_STRT_TS"].ToString())

                            //    });

                            //}
                            conn.Close();
                        }
                    }
                }

            }
            return new List<Trx>();
        }

        public List<Tiendas> GetTiendas()
        {
            var con = Helper.Helper.ConexionStringModel();

            List<Tiendas> lst = new List<Tiendas>();
            using (SqlConnection conn = new SqlConnection(con))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("prueba", conn))
                {
                    cmd.CommandText = "Select * from CENTROS_LOCAL";
                    var reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        lst.Add(new Tiendas()
                        {
                            Local = reader["CEN_Codigo"].ToString(),
                            ServidorIP = reader["CEN_Servidor"].ToString(),
                            Conexion = String.Format("data source={0};initial catalog=GlobalStore;user id=sa;password=fashions.2013;MultipleActiveResultSets=True;App=EntityFramework;", reader["CEN_Servidor"].ToString()),
                            Habilitado = reader["CEN_estado"].ToString()
                        });

                    }
                    //_logger.LogInformation("Informacion de Servidores");


                    //string Prueba = string.Format("exec Proc_GENERAR_VENTAS_IDOC_X_FECHA_X_LOCAL @I_Tienda={0},@C_Fecha ='{1}'", local, Fecha);
                    //context.Database.ExecuteSqlCommand(Prueba);
                }
            }



            return lst;
        }

        public int EjecutarBarrido(string Fecha1, string Fecha2)
        {
            //var prueba = db.Proc_TRASPASA_GS_SISGES_BARRIDO(Fecha1, Fecha2);
            //return prueba;
            return 1;
        }


        public string VentasActual(DateTime Fecha, int local)
        {
            var list = new List<Venta>();
            list = _db.Ventas.Where(x => x.FechaVenta.Year == Fecha.Year && x.FechaVenta.Month
            == Fecha.Month && x.FechaVenta.Day == Fecha.Day && x.Local == local).ToList();

            string Json = string.Empty;
            Json = JsonConvert.SerializeObject(list);

            list.AddRange(JsonConvert.DeserializeObject<List<Venta>>(Json));

            return Json;
        }

        public string FIventas(string Fecha, int Tienda)
        {
            var funcion = "select * from fnIdoc_FI (@Fecha,@IdTienda)";
            List<Parametros> parametros = new List<Parametros>();
            parametros.Add(new Parametros() { Name = "@Fecha", Value = Fecha });
            parametros.Add(new Parametros() { Name = "@IdTienda", Value = Tienda.ToString() });

            List<FnIdoc_FIcs> lst = new List<FnIdoc_FIcs>();

            //var pr = db.Funcion(funcion, parametros);
            //lst = JsonConvert.DeserializeObject<List<FnIdoc_FIcs>>(pr);



            //var lst = db.Funcion(funcion, parametros);

            //return pr;
            return ("OK");
        }
        public ResponseModel EnvioCorreo()
        {
            var response = new ResponseModel();
            try
            {


                var asunto = "Prueba Envio Correo API";
                String textoEmail = "";
                //2525 //587
                SmtpClient client = new SmtpClient("smtp.office365.com", 587);


                client.EnableSsl = true;
                client.Timeout = 1000000000;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = false;

                // nos autenticamos con nuestra cuenta de gmail

                //client.Credentials = new NetworkCredential("bienestar@fashionspark.com", "rrhh.2021");
                client.Credentials = new NetworkCredential("sisges.cuadratura@fashionspark.com", "Cat03082");//cambio contraseña 
                MailMessage mail = new MailMessage();
                mail.To.Add(new MailAddress("eugenio.barra@fashionspark.com"));
                mail.To.Add(new MailAddress("patricio.meneses@fashionspark.com"));
                mail.To.Add(new MailAddress("ricardo.sanchez@fashionspark.com"));
                mail.To.Add(new MailAddress("remigio.saez@fashionspark.com"));

                mail.From = new MailAddress("sisges.cuadratura@fashionspark.com");
                //mail.From = new MailAddress("pedro.astete@fashionspark.com");
                mail.Subject = asunto;
                mail.IsBodyHtml = false;

                //textoEmail = "<br><b>Día y fecha de solicitud</b>"
                //   + "<br>" + DateTime.Now.ToString() + "."
                //   + "<br><br>Estimado " + comensal.Nombre.ToUpper() + " " + comensal.ApellidoPaterno.ToUpper() + " " + (comensal.ApellidoMaterno == null ? " " : comensal.ApellidoMaterno.ToUpper()) + "<br><br>"
                //   + "Adjunto a este correo se encuentra codigo QR con el cual podra recibir su almuerzo diario en el casino de casa matriz<br>"
                //   + "este codigo debera ser scaneado en el dispositivo disponible en la fila del casino <br>"
                //   + "este codigo es unico e instranferible, no debe compartir su codigo para no presentar problemas al momento de solicitar su almuerzo<br><br>"
                //   + "<br><br>";




                //textoEmail = "Estimado <strong>" + comensal.Nombre.ToUpper() + " " + comensal.ApellidoPaterno.ToUpper() + " " + (comensal.ApellidoMaterno == null ? " " : comensal.ApellidoMaterno.ToUpper()) + "</strong><br><br>" +
                //             "<div style= text-align: justify;>Adjunto a este correo se encuentra Código QR con el cual podrá recibir su almuerzo diario en el casino ubicado en Casa Matriz. <br>" +
                //             "Este código deberá se escaneado en el dispositivo disponible en la fila del casino, el cual es personal e intransferible y solo está habilitado para hacer uso del servicio de casino una vez al día.<br><br>" +
                //             "Para consultas o dudas escribir a bienestar@fashionspark.com </div>";

                textoEmail = " <strong>Prueba envio correo desde la api</strong><br><br>";

                AlternateView htmlView = AlternateView.CreateAlternateViewFromString(textoEmail);
                mail.BodyEncoding = UTF8Encoding.UTF8;
                mail.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;

                //htmlView.LinkedResources.Add(
                //    new LinkedResource(path + filename)
                //    {
                //        ContentId = "QrComensal",
                //        TransferEncoding = TransferEncoding.Base64,
                //        ContentType = new ContentType("image/jpg")
                //    });

                mail.AlternateViews.Add(htmlView);
                mail.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;
                //mail.Attachments.Add(data);

                client.Send(mail);

                response.error = false;
                response.mensaje = "CORREO ENVIADO CORRECTAMENTE";

                // SE ACTUALIZA FECHA ENVIO QR
                using (var dbcontext = _db.Database.BeginTransaction())
                {
                    //var dato = db.Comensals.Where(x => x.IdComensal == guid).FirstOrDefault();
                    //dato.FechaEnvioQr = DateTime.Now;
                    //db.SaveChanges();
                    //dbcontext.Commit();
                }


            }
            catch (Exception ex)
            {
                response.error = true;
                response.mensaje = "ERROR: " + ex.Message;
            }
            return response;



        }


        public ResponseModel GenerarExcel()
        {
            ResponseModel response = new ResponseModel();
            try
            {
                string ruta = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"File");
                if (!System.IO.Directory.Exists(ruta))
                {
                    System.IO.Directory.CreateDirectory(ruta);
                }

                //excel.SaveAs(ruta + excel.File.Name + "xlsx");
                using (ExcelPackage excelPackage = new LibroExcel().ReporteUsuario(_db.Personas.ToList()))
                {
                    if (File.Exists(ruta + "\\Personas" + ".xlsx"))
                    {
                        File.Delete(ruta + "\\Personas" + ".xlsx");
                    }
                    excelPackage.SaveAs(ruta + "\\Personas" + ".xlsx");
                    //MemoryStream memoryStream = new MemoryStream();
                    //excelPackage.SaveAs((Stream)memoryStream);
                    //string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    //DateTime dateTime = DateTime.Now;
                    //dateTime = dateTime.Date;
                    //string fileDownloadName = "Personas " + ".xlsx";
                    //memoryStream.Position = 0L;
                    //saveas((Stream)memoryStream, contentType, fileDownloadName);
                    return response;
                }
            }
            catch (Exception)
            {

                throw;
            }
            return response;
        }
        #endregion

        #region Fiventas y pagos
        public ResponseModel GenerarIdocFIVentas(List<FIMP> Fis, DateTime Fecha)
        {
            ResponseModel response = new ResponseModel();
            try
            {
                string ruta = $@"C:\FSP_CUADRATURA\IDOC\FI_VENTAS\{DateTime.Now.ToString("MMMM")}\{DateTime.Now.ToString("dd-MM-yyyy")}\";
                //string ruta = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"File");
                if (!System.IO.Directory.Exists(ruta))
                {
                    System.IO.Directory.CreateDirectory(ruta);
                }

                //excel.SaveAs(ruta + excel.File.Name + "xlsx");
                using (ExcelPackage excelPackage = new LibroExcel().FI(Fis.ToList()))
                {
                    if (File.Exists(ruta + $"\\FI_VENTAS{Fecha.ToString("dd-MM-yyyy")}" + ".xlsx"))
                    {
                        File.Delete(ruta + $"\\FI_VENTAS{Fecha.ToString("dd-MM-yyyy")}" + ".xlsx");
                    }
                    excelPackage.SaveAs(ruta + $"\\FI_VENTAS{Fecha.ToString("dd-MM-yyyy")}" + ".xlsx");
                    //MemoryStream memoryStream = new MemoryStream();
                    //excelPackage.SaveAs((Stream)memoryStream);
                    //string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    //DateTime dateTime = DateTime.Now;
                    //dateTime = dateTime.Date;
                    //string fileDownloadName = "Personas " + ".xlsx";
                    //memoryStream.Position = 0L;
                    //saveas((Stream)memoryStream, contentType, fileDownloadName);
                    response.error = false;
                    response.respuesta = "Se ha generado el idoc correctamente";
                    return response;
                }
            }
            catch (Exception ex)
            {
                response.error = false;
                response.respuesta = "Se ha producido un error " + ex.Message;
                return response;
                throw;
            }

        }
        public ResponseModel GenerarIdocFIPagos(List<FIMP> Fis, DateTime Fecha)
        {
            ResponseModel response = new ResponseModel();
            try
            {
                string ruta = $@"C:\FSP_CUADRATURA\IDOC\FI_PAGOS\{DateTime.Now.ToString("MMMM")}\{DateTime.Now.ToString("dd-MM-yyyy")}\";
                string ruta2 = $@"C:\FSP_CUADRATURA\IDOC\CARGADOS\FI_PAGOS\{DateTime.Now.ToString("MMMM")}\{DateTime.Now.ToString("dd-MM-yyyy")}\";
                //string ruta = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"File");
                if (!System.IO.Directory.Exists(ruta))
                {
                    System.IO.Directory.CreateDirectory(ruta);
                }

                //excel.SaveAs(ruta + excel.File.Name + "xlsx");
                using (ExcelPackage excelPackage = new LibroExcel().FI(Fis.ToList()))
                {
                    if (File.Exists(ruta + $"\\FI_PAGOS{Fecha.ToString("dd-MM-yyyy")}" + ".xlsx"))
                    {
                        File.Delete(ruta + $"\\FI_PAGOS{Fecha.ToString("dd-MM-yyyy")}" + ".xlsx");
                    }
                    excelPackage.SaveAs(ruta + $"\\FI_PAGOS{Fecha.ToString("dd-MM-yyyy")}" + ".xlsx");
                    //MemoryStream memoryStream = new MemoryStream();
                    //excelPackage.SaveAs((Stream)memoryStream);
                    //string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    //DateTime dateTime = DateTime.Now;
                    //dateTime = dateTime.Date;
                    //string fileDownloadName = "Personas " + ".xlsx";
                    //memoryStream.Position = 0L;
                    //saveas((Stream)memoryStream, contentType, fileDownloadName);
                 /*   SFTPSAp(Helper.Helper.GetCrendicalesSAP(), ruta);*///Se comenta el dia 05-06-2023
                    MoverIdoc(ruta, ruta2);
                    response.error = false;
                    response.respuesta = "Se ha generado el idoc correctamente";
                    return response;
                }
            }
            catch (Exception ex)
            {
                response.error = false;
                response.respuesta = "Se ha producido un error " + ex.Message;
                return response;
                throw;
            }

        }
        #endregion
       

        #region Generacion de idocs y Reprocesos
        public List<ResponseModel> EjecutarVentasRp(DateTime Fecha, int local, AmbienteSAP Sap)
        {
            List<ResponseModel> lstResponse = new List<ResponseModel>();
            ResponseModel response = new ResponseModel();

            try
            {
                List<IdocVentasRP> listaIdoc = new List<IdocVentasRP>();
                var l = DateTime.Now.ToString("MMMM");
                string RutaDestinoFinal = $@"IDOC\VENTA\{DateTime.Now.ToString("MMMM")}\{DateTime.Now.ToString("dd-MM-yyyy")}\{Fecha.ToString("yyyyMMdd")}\Hora{DateTime.Now.ToString("HHmm")}\";
                string Base = $@"IDOC\VENTA\\";
                string ruta = $@"{Sap.Ruta}" + Base;

                if (!System.IO.Directory.Exists(ruta))
                {
                    System.IO.Directory.CreateDirectory(ruta);
                }
                if (local == 0)
                {
                    //List<int> ints = new List<int>();
                    //ints.Add(2);
                    //ints.Add(3);
                    //ints.Add(6);
                    //foreach (var item in ints)
                    //{
                    //    listaIdoc.AddRange(fn.fn_IdocVentasRP(Fecha.ToString("yyyyMMdd"), item, ambiente).ToList());
                    //}
                    foreach (var item in _db.CentrosLocals.Where(x => x.CenEstado.Equals("Y")).ToList())
                    {
                        listaIdoc.AddRange(_db.fn_IdocVentasRP(item.CenCodigo, Fecha.ToString("yyyyMMdd"), Sap.Ambiente).ToList());
                    }

                    foreach (var item in listaIdoc.GroupBy(x => x.Archivo).Select(y => y.Key).ToList())
                    {
                        //var ejecutar = fn.fn_IdocVentasRP(Fecha.ToString("yyyyMMdd"), local, ambiente).ToList();
                        var gruoup = item;
                        StringBuilder sb = new StringBuilder();
                        string s = string.Empty;

                        string rutafinal = ruta + gruoup;
                        foreach (var item1 in listaIdoc.Where(x => x.Archivo.Equals(item)).OrderBy(x => x.Orden))
                        {
                            s = item1.Detalle.ToString();
                            sb.AppendLine(s);


                            //System.IO.File.WriteAllText(rutafinal,s);

                        }
                        //System.IO.File.WriteAllText(rutafinal, s);
                        System.IO.StreamWriter st = new System.IO.StreamWriter(rutafinal);
                        st.Write(sb);
                        st.Flush();
                        st.Close();
                        response.error = false;
                        response.mensaje = "Se creo el idoc correspondiente";
                        lstResponse.Add(response);
                    }
                    response = SFTPSAp(Sap,ruta);
                    lstResponse.Add(response);

                    string rutaCargados = $@"{Sap.RutaeDestino}" + RutaDestinoFinal;
                    response = MoverIdoc(ruta, rutaCargados);
                }
                else
                {
                    listaIdoc.AddRange(_db.fn_IdocVentasRP(local, Fecha.ToString("yyyyMMdd"), Sap.Ambiente).ToList());
                    //var ejecutar = fn.fn_IdocVentasRP(Fecha.ToString("yyyyMMdd"), local, ambiente).ToList();
                    var gruoup = listaIdoc.GroupBy(x => x.Archivo).Select(y => y.Key).SingleOrDefault();
                    if (gruoup == null)
                    {
                        response.error = true;
                        response.mensaje = "No se creo el idoc de Ventas para esta tienda";
                        lstResponse.Add(response);
                        return lstResponse;
                    }

                    StringBuilder sb = new StringBuilder();
                    string s = string.Empty;

                    string rutafinal = ruta + gruoup;
                    foreach (var item in listaIdoc.OrderBy(x => x.Orden))
                    {
                        s = item.Detalle.ToString();
                        sb.AppendLine(s);


                        //System.IO.File.WriteAllText(rutafinal,s);

                    }
                    //System.IO.File.WriteAllText(rutafinal, s);
                    System.IO.StreamWriter st = new System.IO.StreamWriter(rutafinal);
                    st.Write(sb);
                    st.Flush();
                    st.Close();
                    response.error = false;
                    response.mensaje = "Se creo el idoc de Ventas correctamente";
                    lstResponse.Add(response);

                    response = SFTPSAp(Sap, ruta);
                    lstResponse.Add(response);

                    string rutaCargados = $@"{Sap.RutaeDestino}" + RutaDestinoFinal;
                    response = MoverIdoc(ruta, rutaCargados);
                }
                ////var ejecutar = fn.fn_IdocVentasRP(Fecha.ToString("yyyyMMdd"), local, ambiente).ToList();
                //var gruoup = listaIdoc.GroupBy(x => x.Archivo).Select(y => y.Key).SingleOrDefault();
                //StringBuilder sb = new StringBuilder();
                //string s = string.Empty;

                //string rutafinal = ruta + gruoup;
                //foreach (var item in listaIdoc.OrderBy(x => x.Orden))
                //{
                //    s = item.Detalle.ToString();
                //    sb.AppendLine(s);


                //    //System.IO.File.WriteAllText(rutafinal,s);

                // }
                ////System.IO.File.WriteAllText(rutafinal, s);
                //System.IO.StreamWriter st = new System.IO.StreamWriter(rutafinal);
                //st.Write(sb);
                //st.Flush();
                //st.Close();
                //response.error = false;
                //response.mensaje = "Se creo el idoc correspondiente";



            }
            catch (Exception ex)
            {

                response.error = true;
                response.mensaje = "Error al Ejecutar el idoc" + ex.Message;
                response.status = "500";
                lstResponse.Add(response);

            }
            return lstResponse;
        }

        public List<ResponseModel> EjecutarNCFC_RP(DateTime Fecha, int local, AmbienteSAP Sap)
        {
            List<ResponseModel> lstResponse = new List<ResponseModel>();
            ResponseModel response = new ResponseModel();

            try
            {
                List<IdocNCFC_RP> listaIdoc = new List<IdocNCFC_RP>();
                var l = DateTime.Now.ToString("MMMM");
                string RutaDestinoFinal = $@"IDOC\NCFC\{DateTime.Now.ToString("MMMM")}\{DateTime.Now.ToString("dd-MM-yyyy")}\{Fecha.ToString("yyyyMMdd")}\Hora{DateTime.Now.ToString("HHmm")}\";
                string Base = $@"IDOC\NCFC\\";
                string ruta = $@"{Sap.Ruta}" + Base;

                if (!System.IO.Directory.Exists(ruta))
                {
                    System.IO.Directory.CreateDirectory(ruta);
                }
                if (local == 0)
                {
                    //List<int> ints = new List<int>();
                    //ints.Add(2);
                    //ints.Add(3);
                    //ints.Add(6);
                    //foreach (var item in ints)
                    //{
                    //    listaIdoc.AddRange(fn.fn_IdocVentasRP(Fecha.ToString("yyyyMMdd"), item, ambiente).ToList());
                    //}
                    foreach (var item in _db.CentrosLocals.Where(x => x.CenEstado.Equals("Y")).ToList())
                    {
                        listaIdoc.AddRange(_db.fn_IdocNCFC_RP(item.CenCodigo, Fecha.ToString("yyyyMMdd"), Sap.Ambiente).ToList());
                    }

                    foreach (var item in listaIdoc.GroupBy(x => x.Archivo).Select(y => y.Key).ToList())
                    {
                        //var ejecutar = fn.fn_IdocVentasRP(Fecha.ToString("yyyyMMdd"), local, ambiente).ToList();
                        var gruoup = item;
                        StringBuilder sb = new StringBuilder();
                        string s = string.Empty;

                        string rutafinal = ruta + gruoup;
                        foreach (var item1 in listaIdoc.Where(x => x.Archivo.Equals(item)).OrderBy(x => x.Orden))
                        {
                            s = item1.Idoc.ToString();
                            sb.AppendLine(s);


                            //System.IO.File.WriteAllText(rutafinal,s);

                        }
                        //System.IO.File.WriteAllText(rutafinal, s);
                        System.IO.StreamWriter st = new System.IO.StreamWriter(rutafinal);
                        st.Write(sb);
                        st.Flush();
                        st.Close();
                        response.error = false;
                        response.mensaje = "Se creo el idoc correspondiente";
                        lstResponse.Add(response);
                    }
                    response = SFTPSAp(Sap,ruta);
                    lstResponse.Add(response);

                    string rutaCargados = $@"{Sap.RutaeDestino}" + RutaDestinoFinal;
                    response = MoverIdoc(ruta, rutaCargados);

                }
                else
                {
                    listaIdoc.AddRange(_db.fn_IdocNCFC_RP(local, Fecha.ToString("yyyyMMdd"), Sap.Ambiente).ToList());
                    //var ejecutar = fn.fn_IdocVentasRP(Fecha.ToString("yyyyMMdd"), local, ambiente).ToList();
                    var gruoup = listaIdoc.GroupBy(x => x.Archivo).Select(y => y.Key).SingleOrDefault();
                    if (gruoup == null)
                    {
                        response.error = true;
                        response.mensaje = "No se creo el idoc de NCFC para esta tienda";
                        lstResponse.Add(response);
                        return lstResponse;
                    }

                    StringBuilder sb = new StringBuilder();
                    string s = string.Empty;

                    string rutafinal = ruta + gruoup;
                    foreach (var item in listaIdoc.OrderBy(x => x.Orden))
                    {
                        s = item.Idoc.ToString();
                        sb.AppendLine(s);


                        //System.IO.File.WriteAllText(rutafinal,s);

                    }
                    //System.IO.File.WriteAllText(rutafinal, s);

                    System.IO.StreamWriter st = new System.IO.StreamWriter(rutafinal);
                    st.Write(sb);
                    st.Flush();
                    st.Close();
                    response.error = false;
                    response.mensaje = "Se creo el idoc de NCFC correctamente";
                    lstResponse.Add(response);

                    response = SFTPSAp(Sap, ruta);
                    lstResponse.Add(response);

                    string rutaCargados = $@"{Sap.RutaeDestino}" + RutaDestinoFinal;
                    response = MoverIdoc(ruta, rutaCargados);

                }

            }
            catch (Exception ex)
            {

                response.error = true;
                response.mensaje = "Error al Ejecutar el idoc" + ex.Message;
                response.status = "500";
                lstResponse.Add(response);
            }
            return lstResponse;
        }

        public List<ResponseModel> EjecutarClientes_RP(DateTime Fecha, int local, AmbienteSAP Sap)
        {
            List<ResponseModel> lstResponse = new List<ResponseModel>();
            ResponseModel response = new ResponseModel();
            try
            {
                List<FormatoIDOC> listaIdoc = new List<FormatoIDOC>();
                var l = DateTime.Now.ToString("MMMM");
                string RutaDestinoFinal = $@"IDOC\CLIENTES\\{DateTime.Now.ToString("MMMM")}\{DateTime.Now.ToString("dd-MM-yyyy")}\{Fecha.ToString("yyyyMMdd")}\Hora{DateTime.Now.ToString("HHmm")}";
                string Base = $@"IDOC\CLIENTES\\";
                string ruta = $@"{Sap.Ruta}" + Base;
                //string r = $@"{Sap.Ruta}IDOC\CLIENTES\\{DateTime.Now.ToString("MMMM")}\{DateTime.Now.ToString("dd-MM-yyyy")}\{Fecha.ToString("yyyyMMdd")}\";
                
                if (!System.IO.Directory.Exists(ruta))
                {
                    System.IO.Directory.CreateDirectory(ruta);
                }
                if (local == 0)
                {
                    //List<int> ints = new List<int>();
                    //ints.Add(2);
                    //ints.Add(3);
                    //ints.Add(6);
                    //foreach (var item in ints)
                    //{
                    //    listaIdoc.AddRange(fn.fn_IdocVentasRP(Fecha.ToString("yyyyMMdd"), item, ambiente).ToList());
                    //}
                    foreach (var item in _db.CentrosLocals.Where(x => x.CenEstado.Equals("Y")).ToList())
                    {
                        //listaIdoc.AddRange(fn.fn_IdocClientes_RP(Fecha.ToString("yyyyMMdd"), item.CenCodigo, Sap.Ambiente).ToList());
                        listaIdoc.AddRange(_db.fn_IdocClientes_RP(item.CenCodigo, Fecha.ToString("yyyyMMdd"), Sap.Ambiente).ToList());
                    }

                    foreach (var item in listaIdoc.GroupBy(x => x.Archivo).Select(y => y.Key).ToList())
                    {
                        //var ejecutar = fn.fn_IdocVentasRP(Fecha.ToString("yyyyMMdd"), local, ambiente).ToList();
                        var gruoup = item;
                        StringBuilder sb = new StringBuilder();
                        string s = string.Empty;

                        string rutafinal = ruta + gruoup;
                        foreach (var item1 in listaIdoc.Where(x => x.Archivo.Equals(item)).OrderBy(x => x.Orden))
                        {
                            s = item1.Idoc.ToString();
                            sb.AppendLine(s);


                            //System.IO.File.WriteAllText(rutafinal,s);

                        }
                        //System.IO.File.WriteAllText(rutafinal, s);
                        System.IO.StreamWriter st = new System.IO.StreamWriter(rutafinal);
                        // Reemplazar CRLF por LF
                        string content = sb.ToString().Replace("\r\n", "\n");
                        st.Write(content);
                        st.Flush();
                        st.Close();
                        response.error = false;
                        response.mensaje = "Se creo el idoc correspondiente";
                        lstResponse.Add(response);
                    }
                    response = SFTPSAp(Sap,ruta);
                    lstResponse.Add(response);

                    string rutaCargados = $@"{Sap.RutaeDestino}" + RutaDestinoFinal;
                    response = MoverIdoc(ruta, rutaCargados);

                }
                else
                {

                    listaIdoc.AddRange(_db.fn_IdocClientes_RP(local, Fecha.ToString("yyyyMMdd"), Sap.Ambiente).ToList());
                    //listaIdoc.AddRange(fn.fn_IdocClientes_RP(Fecha.ToString("yyyyMMdd"), local,Sap.Ambiente).ToList());
                    //var ejecutar = fn.fn_IdocVentasRP(Fecha.ToString("yyyyMMdd"), local, ambiente).ToList();
                    var gruoup = listaIdoc.GroupBy(x => x.Archivo).Select(y => y.Key).SingleOrDefault();
                    if (gruoup == null)
                    {
                        response.error = true;
                        response.mensaje = "No se creo el idoc de Clientes para esta tienda";
                        lstResponse.Add(response);
                        return lstResponse;
                    }

                    StringBuilder sb = new StringBuilder();
                    string s = string.Empty;

                    string rutafinal = ruta + gruoup;
                    foreach (var item in listaIdoc.OrderBy(x => x.Orden))
                    {
                        s = item.Idoc.ToString();
                        sb.AppendLine(s);


                        //System.IO.File.WriteAllText(rutafinal,s);

                    }
                    //System.IO.File.WriteAllText(rutafinal, s);
                    System.IO.StreamWriter st = new System.IO.StreamWriter(rutafinal);
                    string content = sb.ToString().Replace("\r\n", "\n");
                    st.Write(content);
                    st.Flush();
                    st.Close();
                    response.error = false;
                    response.mensaje = "Se creo el idoc correspondiente";
                    lstResponse.Add(response);                     

                    //Subida del SFTP
                    response = SFTPSAp(Sap, ruta);
                    lstResponse.Add(response);

                    //Moviendo Archivos
                    string RutaCargados = $@"{Sap.RutaeDestino}" + RutaDestinoFinal;
                    response = MoverIdoc(ruta, RutaCargados);

                }
                return lstResponse;
            }
            catch (Exception ex)
            {

                response.error = true;
                response.mensaje = "Error al Ejecutar el idoc" + ex.Message;
                response.status = "500";
                lstResponse.Add(response);
            }
            return lstResponse;
        }

        public List<ResponseModel> EjecutarMPVentas_RP(DateTime Fecha, int local, AmbienteSAP Sap)
        {
            List<ResponseModel> lstResponse = new List<ResponseModel>();
            ResponseModel response = new ResponseModel();
            try
            {
                List<FormatoIDOC> listaIdoc = new List<FormatoIDOC>();
                var l = DateTime.Now.ToString("MMMM");
                string RutaDestinoFinal = $@"IDOC\MPVentas\{DateTime.Now.ToString("MMMM")}\{DateTime.Now.ToString("dd-MM-yyyy")}\{Fecha.ToString("yyyyMMdd")}\Hora{DateTime.Now.ToString("HHmm")}\";
                string Base = $@"IDOC\MPVentas\\";
                string ruta = $@"{Sap.Ruta}" + Base;

                if (!System.IO.Directory.Exists(ruta))
                {
                    System.IO.Directory.CreateDirectory(ruta);
                }
                if (local == 0)
                {
                    //List<int> ints = new List<int>();
                    //ints.Add(2);
                    //ints.Add(3);
                    //ints.Add(6);
                    //foreach (var item in ints)
                    //{
                    //    listaIdoc.AddRange(fn.fn_IdocVentasRP(Fecha.ToString("yyyyMMdd"), item, ambiente).ToList());
                    //}
                    foreach (var item in _db.CentrosLocals.Where(x => x.CenEstado.Equals("Y")).ToList())
                    {
                        listaIdoc.AddRange(_db.fn_IdocMPVentas_RP(item.CenCodigo, Fecha.ToString("yyyyMMdd"), Sap.Ambiente).ToList());
                    }

                    foreach (var item in listaIdoc.GroupBy(x => x.Archivo).Select(y => y.Key).ToList())
                    {
                        //var ejecutar = fn.fn_IdocVentasRP(Fecha.ToString("yyyyMMdd"), local, ambiente).ToList();
                        var gruoup = item;
                        StringBuilder sb = new StringBuilder();
                        string s = string.Empty;

                        string rutafinal = ruta + gruoup;
                        foreach (var item1 in listaIdoc.Where(x => x.Archivo.Equals(item)).OrderBy(x => x.Orden))
                        {
                            s = item1.Idoc.ToString();
                            sb.AppendLine(s);


                            //System.IO.File.WriteAllText(rutafinal,s);

                        }
                        //System.IO.File.WriteAllText(rutafinal, s);
                        System.IO.StreamWriter st = new System.IO.StreamWriter(rutafinal);
                        st.Write(sb);
                        st.Flush();
                        st.Close();
                        response.error = false;
                        response.mensaje = "Se creo el idoc correspondiente";
                        lstResponse.Add(response);
                    }
                    response = SFTPSAp(Sap, ruta);
                    lstResponse.Add(response);

                    string rutaCargados = $@"{Sap.RutaeDestino}" + RutaDestinoFinal;
                    response = MoverIdoc(ruta, rutaCargados);
                }
                else
                {
                    listaIdoc.AddRange(fn.fn_IdocMPVentas_RP(Fecha.ToString("yyyyMMdd"), local, Sap.Ambiente).ToList());
                    //var ejecutar = fn.fn_IdocVentasRP(Fecha.ToString("yyyyMMdd"), local, ambiente).ToList();
                    //var gruoup = listaIdoc.GroupBy(x => x.Archivo).Select(y => y.Key).SingleOrDefault();

                    var gruoup = listaIdoc.GroupBy(x => x.Archivo).Select(y => y.Key).SingleOrDefault();
                    if (gruoup == null)
                    {
                        response.error = true;
                        response.mensaje = "No se creo el idoc de MP Ventas para esta tienda";
                        lstResponse.Add(response);
                        return lstResponse;
                    }

                    StringBuilder sb = new StringBuilder();
                    string s = string.Empty;

                    string rutafinal = ruta + gruoup;
                    foreach (var item in listaIdoc.OrderBy(x => x.Orden))
                    {
                        s = item.Idoc.ToString();
                        sb.AppendLine(s);


                        //System.IO.File.WriteAllText(rutafinal,s);

                    }
                    //System.IO.File.WriteAllText(rutafinal, s);
                    System.IO.StreamWriter st = new System.IO.StreamWriter(rutafinal);
                    st.Write(sb);
                    st.Flush();
                    st.Close();
                    response.error = false;
                    response.mensaje = "Se creo el idoc correspondiente";

                    lstResponse.Add(response);

                    response = SFTPSAp(Sap, ruta);
                    lstResponse.Add(response);

                    string rutaCargados = $@"{Sap.RutaeDestino}" + RutaDestinoFinal;
                    response = MoverIdoc(ruta, rutaCargados);

                }

            }
            catch (Exception ex)
            {
                response.error = true;
                response.mensaje = "Error al Ejecutar el idoc" + ex.Message;
                response.status = "500";
                lstResponse.Add(response);
            }
            return lstResponse;
        }
        public ResponseModel EjecutarFiVentasRp(DateTime Fecha, int local, string ambiente)
        {
            ResponseModel response = new ResponseModel();
            try
            {
                List<IdocVentasRP> listaIdoc = new List<IdocVentasRP>();
                List<FIMP> lst = new List<FIMP>();

                if (local == 0)
                {
                    foreach (var item in _db.CentrosLocals.Where(x => x.CenEstado.Equals("Y")))
                    {
                        var respuesta = fn.IdocFiVentas(Fecha.ToString("yyyyMMdd"), item.CenCodigo);

                        List<IdocFi> idocFiList = _db.IdocFis.Where(x => x.Fechaint.Equals(Fecha.ToString("ddMMyyyy")) && x.Xblnr == item.CenCodigo).ToList();

                        List<FIMP> fimpList = idocFiList.ConvertAll(idocFi => new FIMP
                        {
                            MANDT = idocFi.Mandt,
                            INTERFAZ = idocFi.Interfaz,
                            FECHAINT = idocFi.Fechaint,
                            CORRELAT = idocFi.Correlat,
                            NITEM = idocFi.Nitem,
                            BUKRS = idocFi.Bukrs,
                            BLART = idocFi.Blart,
                            WAERS = idocFi.Waers,
                            BUDAT = idocFi.Budat,
                            BLDAT = idocFi.Bldat,
                            MONAT = idocFi.Monat,
                            XBLNR = idocFi.Xblnr,
                            BKTXT = idocFi.Bktxt,
                            BUPLA = idocFi.Bupla,
                            NEWBS = idocFi.Newbs,
                            NEWKO = idocFi.Newko,
                            NEWUM = idocFi.Newum,
                            NEWBK = idocFi.Newbk,
                            WRBTR = idocFi.Wrbtr,
                            FWBAS = idocFi.Fwbas,
                            MWSKZ = idocFi.Mwskz,
                            GSBER = idocFi.Gsber,
                            KOSTL_PRCTR = idocFi.KostlPrctr,
                            AUFNR = idocFi.Aufnr,
                            ZTERM = idocFi.Zterm,
                            ZUONR = idocFi.Zuonr,
                            SGTXT = idocFi.Sgtxt,
                            VBUND = idocFi.Vbund,
                            XREF1 = idocFi.Xref1,
                            XREF2 = idocFi.Xref2,
                            XREF3 = idocFi.Xref3,
                            VALUT = idocFi.Valut,
                            XMWST = idocFi.Xmwst,
                            ZLSPR = idocFi.Zlspr,
                            ZFBDT = idocFi.Zfbdt,
                            MANSP = idocFi.Mansp,
                            NEWBW = idocFi.Newbw,
                            MENGE = idocFi.Menge,
                            KURSF = idocFi.Kursf,
                            WWERT = idocFi.Wwert,
                            PRCTR = idocFi.Prctr,
                            SKFBT = idocFi.Skfbt,
                            NAME1 = idocFi.Name1,
                            ORT01 = idocFi.Ort01,
                            STCD1 = idocFi.Stcd1,
                            EBELN = idocFi.Ebeln,
                            WERKS = idocFi.Werks
                        });

                        lst.AddRange(fimpList);
                    }

                }
                else
                {

                    var respuesta = fn.IdocFiVentas(Fecha.ToString("yyyyMMdd"), local);

                    List<IdocFi> idocFiList = _db.IdocFis.Where(x => x.Fechaint.Equals(Fecha.ToString("ddMMyyyy")) && x.Xblnr == local).ToList();

                    List<FIMP> fimpList = idocFiList.ConvertAll(idocFi => new FIMP
                    {
                        MANDT = idocFi.Mandt,
                        INTERFAZ = idocFi.Interfaz,
                        FECHAINT = idocFi.Fechaint,
                        CORRELAT = idocFi.Correlat,
                        NITEM = idocFi.Nitem,
                        BUKRS = idocFi.Bukrs,
                        BLART = idocFi.Blart,
                        WAERS = idocFi.Waers,
                        BUDAT = idocFi.Budat,
                        BLDAT = idocFi.Bldat,
                        MONAT = idocFi.Monat,
                        XBLNR = idocFi.Xblnr,
                        BKTXT = idocFi.Bktxt,
                        BUPLA = idocFi.Bupla,
                        NEWBS = idocFi.Newbs,
                        NEWKO = idocFi.Newko,
                        NEWUM = idocFi.Newum,
                        NEWBK = idocFi.Newbk,
                        WRBTR = idocFi.Wrbtr,
                        FWBAS = idocFi.Fwbas,
                        MWSKZ = idocFi.Mwskz,
                        GSBER = idocFi.Gsber,
                        KOSTL_PRCTR = idocFi.KostlPrctr,
                        AUFNR = idocFi.Aufnr,
                        ZTERM = idocFi.Zterm,
                        ZUONR = idocFi.Zuonr,
                        SGTXT = idocFi.Sgtxt,
                        VBUND = idocFi.Vbund,
                        XREF1 = idocFi.Xref1,
                        XREF2 = idocFi.Xref2,
                        XREF3 = idocFi.Xref3,
                        VALUT = idocFi.Valut,
                        XMWST = idocFi.Xmwst,
                        ZLSPR = idocFi.Zlspr,
                        ZFBDT = idocFi.Zfbdt,
                        MANSP = idocFi.Mansp,
                        NEWBW = idocFi.Newbw,
                        MENGE = idocFi.Menge,
                        KURSF = idocFi.Kursf,
                        WWERT = idocFi.Wwert,
                        PRCTR = idocFi.Prctr,
                        SKFBT = idocFi.Skfbt,
                        NAME1 = idocFi.Name1,
                        ORT01 = idocFi.Ort01,
                        STCD1 = idocFi.Stcd1,
                        EBELN = idocFi.Ebeln,
                        WERKS = idocFi.Werks
                    });

                    lst.AddRange(fimpList);

                }

                return response = GenerarIdocFIVentas(lst, Fecha);



            }
            catch (Exception ex)
            {

                response.error = true;
                response.mensaje = "Error al Ejecutar el idoc" + ex.Message;
                response.status = "500";


            }
            return response;
        }
        public ResponseModel EjecutarFiPagosRp(DateTime Fecha, int local, string ambiente)
        {
            ResponseModel response = new ResponseModel();
            try
            {
                List<IdocVentasRP> listaIdoc = new List<IdocVentasRP>();
                List<FIMP> lst = new List<FIMP>();

                if (local == 0)
                {
                    foreach (var item in _db.CentrosLocals.Where(x => x.CenEstado.Equals("Y")))
                    {
                        var respuesta = fn.IdocFiPagos(Fecha.ToString("yyyyMMdd"), item.CenCodigo);

                        List<IdocPagosFi> idocFiList = _db.IdocPagosFis.Where(x => x.Fechaint.Equals(Fecha.ToString("ddMMyyyy")) && x.Xblnr == item.CenCodigo).ToList();

                        List<FIMP> fimpList = idocFiList.ConvertAll(idocFi => new FIMP
                        {
                            MANDT = idocFi.Mandt,
                            INTERFAZ = idocFi.Interfaz,
                            FECHAINT = idocFi.Fechaint,
                            CORRELAT = idocFi.Correlat,
                            NITEM = idocFi.Nitem,
                            BUKRS = idocFi.Bukrs,
                            BLART = idocFi.Blart,
                            WAERS = idocFi.Waers,
                            BUDAT = idocFi.Budat,
                            BLDAT = idocFi.Bldat,
                            MONAT = idocFi.Monat,
                            XBLNR = idocFi.Xblnr,
                            BKTXT = idocFi.Bktxt,
                            BUPLA = idocFi.Bupla,
                            NEWBS = idocFi.Newbs,
                            NEWKO = idocFi.Newko,
                            NEWUM = idocFi.Newum,
                            NEWBK = idocFi.Newbk,
                            WRBTR = idocFi.Wrbtr,
                            FWBAS = idocFi.Fwbas,
                            MWSKZ = idocFi.Mwskz,
                            GSBER = idocFi.Gsber,
                            KOSTL_PRCTR = idocFi.KostlPrctr,
                            AUFNR = idocFi.Aufnr,
                            ZTERM = idocFi.Zterm,
                            ZUONR = idocFi.Zuonr,
                            SGTXT = idocFi.Sgtxt,
                            VBUND = idocFi.Vbund,
                            XREF1 = idocFi.Xref1,
                            XREF2 = idocFi.Xref2,
                            XREF3 = idocFi.Xref3,
                            VALUT = idocFi.Valut,
                            XMWST = idocFi.Xmwst,
                            ZLSPR = idocFi.Zlspr,
                            ZFBDT = idocFi.Zfbdt,
                            MANSP = idocFi.Mansp,
                            NEWBW = idocFi.Newbw,
                            MENGE = idocFi.Menge,
                            KURSF = idocFi.Kursf,
                            WWERT = idocFi.Wwert,
                            PRCTR = idocFi.Prctr,
                            SKFBT = idocFi.Skfbt,
                            NAME1 = idocFi.Name1,
                            ORT01 = idocFi.Ort01,
                            STCD1 = idocFi.Stcd1,
                            EBELN = idocFi.Ebeln,
                            WERKS = idocFi.Werks
                        });

                        lst.AddRange(fimpList);
                    }

                }
                else
                {

                    var respuesta = fn.IdocFiPagos(Fecha.ToString("yyyyMMdd"), local);

                    List<IdocPagosFi> idocFiList = _db.IdocPagosFis.Where(x => x.Fechaint.Equals(Fecha.ToString("ddMMyyyy")) && x.Xblnr == local).ToList();

                    List<FIMP> fimpList = idocFiList.ConvertAll(idocFi => new FIMP
                    {
                        MANDT = idocFi.Mandt,
                        INTERFAZ = idocFi.Interfaz,
                        FECHAINT = idocFi.Fechaint,
                        CORRELAT = idocFi.Correlat,
                        NITEM = idocFi.Nitem,
                        BUKRS = idocFi.Bukrs,
                        BLART = idocFi.Blart,
                        WAERS = idocFi.Waers,
                        BUDAT = idocFi.Budat,
                        BLDAT = idocFi.Bldat,
                        MONAT = idocFi.Monat,
                        XBLNR = idocFi.Xblnr,
                        BKTXT = idocFi.Bktxt,
                        BUPLA = idocFi.Bupla,
                        NEWBS = idocFi.Newbs,
                        NEWKO = idocFi.Newko,
                        NEWUM = idocFi.Newum,
                        NEWBK = idocFi.Newbk,
                        WRBTR = idocFi.Wrbtr,
                        FWBAS = idocFi.Fwbas,
                        MWSKZ = idocFi.Mwskz,
                        GSBER = idocFi.Gsber,
                        KOSTL_PRCTR = idocFi.KostlPrctr,
                        AUFNR = idocFi.Aufnr,
                        ZTERM = idocFi.Zterm,
                        ZUONR = idocFi.Zuonr,
                        SGTXT = idocFi.Sgtxt,
                        VBUND = idocFi.Vbund,
                        XREF1 = idocFi.Xref1,
                        XREF2 = idocFi.Xref2,
                        XREF3 = idocFi.Xref3,
                        VALUT = idocFi.Valut,
                        XMWST = idocFi.Xmwst,
                        ZLSPR = idocFi.Zlspr,
                        ZFBDT = idocFi.Zfbdt,
                        MANSP = idocFi.Mansp,
                        NEWBW = idocFi.Newbw,
                        MENGE = idocFi.Menge,
                        KURSF = idocFi.Kursf,
                        WWERT = idocFi.Wwert,
                        PRCTR = idocFi.Prctr,
                        SKFBT = idocFi.Skfbt,
                        NAME1 = idocFi.Name1,
                        ORT01 = idocFi.Ort01,
                        STCD1 = idocFi.Stcd1,
                        EBELN = idocFi.Ebeln,
                        WERKS = idocFi.Werks
                    });

                    lst.AddRange(fimpList);

                }

                return response = GenerarIdocFIPagos(lst, Fecha);



            }
            catch (Exception ex)
            {

                response.error = true;
                response.mensaje = "Error al Ejecutar el idoc" + ex.Message;
                response.status = "500";


            }
            return response;
        }

        #endregion



        public ResponseModel SFTPSAp(AmbienteSAP config, string ruta)
        {
            IEnumerable<FileSystemInfo> infos = new DirectoryInfo(ruta).GetFiles();
            //config.Path = "C:/FSP_CUADRATURA/IDOC/FI";
            Console.WriteLine("Entro a subir a sftp " + infos.Count());

            ResponseModel response = new ResponseModel();


            #region FTP POS

            using (SftpClient Sftp = credencialesOutPutSFTP(config))
            {
                //_logger.LogInformation("SE INTENTA CONEXION FTP  A SERVIDOR: " + config.Host.ToString());
                Console.WriteLine("path: " + config.Path);
                try
                {
                    Sftp.Connect();
                    if (Sftp.IsConnected)
                    {
                        //_logger.LogInformation("CONEXION FTP ABIERTA CON SERVIDOR: " + config.Host.ToString());
                        Console.WriteLine("Abrio la conexion");
                        foreach (FileSystemInfo info in infos)
                        {
                            Console.WriteLine("For de subida " + info.FullName + config.Path);
                            using (Stream fileStream = new FileStream(info.FullName, FileMode.Open))
                            {
                                Console.WriteLine("Dentro del using");
                                Sftp.UploadFile(fileStream, config.Path + info.Name);
                                //_logger.LogInformation("Idoc " + info.Name + " ENVIADO COORECTAMENTE A SERVIDOR");
                            }
                        }
                        Sftp.Disconnect();
                        Sftp.Dispose();
                        response.error = false;
                        response.status = "200";
                        response.respuesta = "Se subieron los Archivos al SFTp";
                        

                        //MoverIdoc(ruta, config.RutaeDestino);
                        //_logger.LogInformation("CONEXION FTP CERRADA CON SERVIDOR: " + config.Host.ToString());
                    }
                    else
                    {
                        //_logger.LogInformation("NO SE PUEDO ABRIR LA CONEXION POR FTP CON EL SERVIDOR: " + config.Host.ToString() + ", SE TERMINA PROCESO");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Dentro del using" + ex.Message);
                    response.error = true;
                    response.status = "400";
                    response.respuesta = "Error al subir al sftp";
                    response.mensaje = ex.Message;
                    //_logger.LogInformation("ERROR EN LA CONEXION POR FTP SERVIDOR: " + config.Host.ToString() + ", TERMINA PROCESO, ERROR: " + ex.Message.ToString());
                }
            }

            return response;

            #endregion





        }

        //public SftpClient credencialesOutPutSFTP(sFTPCredentials sftCredential)
        //{
        //    string host = @sftCredential.Host;
        //    string username = sftCredential.Username;
        //    string password = @sftCredential.Password;
        //    //string path = @sftCredential.Path;
        //    SftpClient sftp = new SftpClient(host, username, password);
        //    return sftp;
        //}

        public SftpClient credencialesOutPutSFTP(AmbienteSAP sftCredential)
        {
            string host = @sftCredential.Host;
            string username = sftCredential.Username;
            string password = @sftCredential.Password;
            //string path = @sftCredential.Path;
            SftpClient sftp = new SftpClient(host, username, password);
            return sftp;
        }

        public ResponseModel MoverIdoc(string ruta, string ruta2)
        {
            DateTime fechaHoy = DateTime.Today;
            DateTimeFormatInfo formatoFecha = CultureInfo.CurrentCulture.DateTimeFormat;
            string nombreMes = formatoFecha.GetMonthName(fechaHoy.Month);
            ResponseModel response = new ResponseModel();

            var prueba = new DirectoryInfo(ruta);
            Console.WriteLine("carpeta padre" + prueba.Name);
            IEnumerable<FileSystemInfo> infos = new DirectoryInfo(ruta).GetFiles();
            string[] files = System.IO.Directory.GetFiles(ruta);
            //ruta2 += prueba.Name + "/" + nombreMes;
            //Console.WriteLine("OLI validando ruta " + infos.Count());
            //var ruta2 = @"C:\FSP_CUADRATURA\IDOC\REPROCESOS\CLIENTES";
            //foreach (var info in infos)
            //{
            //    var name = info.Name;
            //    //ruta2 += ruta2 + name;
            //    var ruta4 = ruta2 + name;
            //    var ruta3 = ruta + name;
            //    System.IO.File.Copy(info, ruta2, true);
            //}
            try
            {
                if (!System.IO.Directory.Exists(ruta2))
                {
                    System.IO.Directory.CreateDirectory(ruta2);
                }

                foreach (string s in files)
                {
                    // Use static Path methods to extract only the file name from the path.
                    //var fileName = System.IO.Path.GetFileName(s);
                    //var destFile = System.IO.Path.Combine(ruta2, fileName);
                    //if (System.IO.File.Exists(destFile))
                    //{

                    //}
                    //System.IO.File.Move(s, destFile);
                    var nombreActualArchivo = System.IO.Path.GetFileName(s);
                    var nuevoNombreArchivo = nombreActualArchivo;
                    var contador = 0;
                    while (System.IO.File.Exists(System.IO.Path.Combine(ruta2, nuevoNombreArchivo)))
                    {
                        contador++;
                        var nombreArchivoSinExtension = System.IO.Path.GetFileNameWithoutExtension(nombreActualArchivo);
                        var extensionArchivo = System.IO.Path.GetExtension(nombreActualArchivo);
                        nuevoNombreArchivo = $"{nombreArchivoSinExtension}({contador}){extensionArchivo}";
                    }
                    var destFile = System.IO.Path.Combine(ruta2, nuevoNombreArchivo);
                    System.IO.File.Move(s, destFile);
                    //System.IO.Directory.Delete()
                }

                response.error = false;
                response.respuesta = "Se movieron los Archivos Correctamente";
               
                
            }
            catch (Exception ex)
            {

                response.error = true;
                response.mensaje = ex.Message;
                response.respuesta = "Error al Mover los Archvios";
            }

            return response;
    
        }
      
    }
}




