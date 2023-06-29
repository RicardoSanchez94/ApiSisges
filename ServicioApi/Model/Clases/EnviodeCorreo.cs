using System.Net.Mail;
using System.Net;
using System.Text;
using ServicioApi.Modelview;
using ServicioApi.Model.SisGes;
using System.Linq;

namespace ServicioApi.Model.Clases
{
    public class EnviodeCorreo
    {
        CredencialesCorreo Correo = Helper.Helper.GetCrendicalesCorreo();
        SisgesDBContext db = new SisgesDBContext();


        #region AlertaSencillos
        public ResponseModel EnviodeAlertaTesoreria(List<SencillosTesoreria> sencillosTesorerias)
        {
            var response = new ResponseModel();

         
            try
            {


                    var asunto = " Alerta Sencillos Tesoreria";
                String textoEmail = "";
                SmtpClient client = GetSmtpcliente();
                //SmtpClient client = new SmtpClient("smtp.office365.com", 587);


                //client.EnableSsl = Correo.EnableSsl;
                //client.Timeout = Correo.Timeout;
                //client.DeliveryMethod = Correo.DeliveryMethod;
                //client.UseDefaultCredentials = Correo.UseDefaultCredentials;

                //// nos autenticamos con nuestra cuenta de gmail

                ////client.Credentials = new NetworkCredential("bienestar@fashionspark.com", "rrhh.2021");
                //client.Credentials = new NetworkCredential(Correo.Email, Correo.Pass);//cambio contraseña 
                MailMessage mail = new MailMessage();
                mail.To.Add(new MailAddress("cuadratura.fp@fashionspark.com"));
                //mail.To.Add(new MailAddress("eugenio.barra@fashionspark.com"));
                mail.To.Add(new MailAddress("tesoreria@fashionspark.com"));
                //mail.To.Add(new MailAddress("ricardo.sanchez@fashionspark.com"));
                mail.To.Add(new MailAddress("marcelo.roa@fashionspark.com"));

                //mail.To.Add(new MailAddress("remigio.saez@fashionspark.com"));

                mail.From = new MailAddress(Correo.Email);
                //mail.From = new MailAddress("pedro.astete@fashionspark.com");
                mail.Subject = asunto;
                mail.IsBodyHtml = false;

                foreach (var Fch in sencillosTesorerias.GroupBy(x => new { x.FechaInicio, x.FechaFin }).Select(x => new { Inico = x.Key.FechaInicio, Fin = x.Key.FechaFin }).ToList())
                {
                    textoEmail += "Las Tiendas que no estan Conciliadas en el Periodo : " + Fch.Inico.ToString("dd-MM-yyyy") + " " + Fch.Fin.ToString("dd-MM-yyyy") + " Son : <br>";
                    textoEmail += "<table style='border-collapse: collapse; width: 100%; margin: auto;'>";
                    textoEmail += "<thead style='background-color: #ac0133;'><tr><th style='border: 1px solid #ffffff; text-align: center; padding: 8px; color: #ffffff;'>Tienda</th><th style='border: 1px solid #ffffff; text-align: center; padding: 8px; color: #ffffff;'>MontoSencillo</th><th style='border: 1px solid #ffffff; text-align: center; padding: 8px; color: #ffffff;'>Banco</th><th style='border: 1px solid #ffffff; text-align: center; padding: 8px; color: #ffffff;'>DiaLiberacion</th><th style='border: 1px solid #ffffff; text-align: center; padding: 8px; color: #ffffff;'>DiaEntrega</th></tr></thead>";

                    textoEmail += "<tbody>";
                    foreach (var item in sencillosTesorerias.Where(x => x.FechaInicio == Fch.Inico && x.FechaFin == Fch.Fin).OrderBy(x=>x.IdTienda))
                    {
                        textoEmail += "<tr>";
                        textoEmail += "<td style='border: 1px solid #ddd; padding: 8px; text-align: left;'>" + item.IdTienda + "</td>";
                        textoEmail += "<td style='border: 1px solid #ddd; padding: 8px; text-align: left;'>" + item.NuevoTotal + "</td>";
                        textoEmail += "<td style='border: 1px solid #ddd; padding: 8px; text-align: left;'>" + item.Banco + "</td>";
                        textoEmail += "<td style='border: 1px solid #ddd; padding: 8px; text-align: left;'>" + item.DiaLiberacion + "</td>";
                        textoEmail += "<td style='border: 1px solid #ddd; padding: 8px; text-align: left;'>" + item.DiaEntrega + "</td>";
                        textoEmail += "</tr>";
                    }
                    textoEmail += "</tbody></table>";

                    //foreach (var item in sencillosTesorerias.ToList())
                    //{
                    //    textoEmail += "<table><tr>";
                    //    textoEmail += "<td>Tienda" + item.IdTienda + "</td><br>";
                    //    textoEmail += "<td>MontoSencillo" + item.NuevoTotal + "</td><br>";
                    //    textoEmail += "<td>Banco" + item.Banco + "</td><br>";
                    //    textoEmail += "<td>DiaLiberacion" + item.DiaLiberacion + "</td><br>";
                    //    textoEmail += "<td>DiaEntrega" + item.DiaEntrega +  "</td></tr></table><br><br>";

                    //    //textoEmail += "Con Fecha Inicio de Carga : " + item.FechaInicio + "Y con Fecha Fin :" + item.FechaFin + "<br><br>";

                    //}
                }


                textoEmail += "Saludos Atentamente";



                AlternateView htmlView = AlternateView.CreateAlternateViewFromString(textoEmail, null, "text/html");

                mail.BodyEncoding = UTF8Encoding.UTF8;
                mail.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;
                //AlternateView htmlView = AlternateView.CreateAlternateViewFromString(textoEmail, Encoding.UTF8, "text/html");
                //mail.AlternateViews.Add(htmlView);

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
                using (var dbcontext = db.Database.BeginTransaction())
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


        public ResponseModel EnviodeAlertaTiendaSinAceptar(List<SencillosTienda> sencillosTesorerias, string Asunto)
        {
            var response = new ResponseModel();
            try
            {


                //var asunto = "Prueba Alerta Sencillos Tesoreria";
                var asunto = Asunto;
                String textoEmail = "";
                SmtpClient client = GetSmtpcliente();

                MailMessage mail = new MailMessage();
                //foreach (var item in sencillosTesorerias)
                //{
                //    mail.To.Add(item.IdTiendaNavigation.Correo);
                //}
                //mail.To.Add(new MailAddress("eugenio.barra@fashionspark.com"));
                ////mail.To.Add(new MailAddress("tesoreria@fashionspark.com"));
                mail.To.Add(new MailAddress("cuadratura.fp@fashionspark.com"));

                //mail.To.Add(new MailAddress("ricardo.sanchez@fashionspark.com"));
                mail.To.Add(new MailAddress("marcelo.roa@fashionspark.com"));



                mail.From = new MailAddress(Correo.Email);
                //mail.From = new MailAddress("pedro.astete@fashionspark.com");
                mail.Subject = asunto;
                mail.IsBodyHtml = false;
                var Descripcion = sencillosTesorerias.Select(x => x.CodigoEstadoSencilloNavigation.Nombre).FirstOrDefault();


              

                foreach (var Fch in sencillosTesorerias.GroupBy(x => new { x.IdDetalleSencilloNavigation.IdSencilloNavigation.FechaInicio, x.IdDetalleSencilloNavigation.IdSencilloNavigation.FechaFin }).Select(x => new { Inico = x.Key.FechaInicio, Fin = x.Key.FechaFin }).ToList())
                {


                    textoEmail += "Tiendas que estan estado : " + Descripcion + " Que no Han Aceptado el Sencillo pasado los 3 dias de Aceptar en el Periodo  " + Fch.Inico.Value.ToString("dd-MM-yyyy") + " " + Fch.Fin.Value.ToString("dd-MM-yyyy") + " Son : <br>";
                    textoEmail += "<table style='border-collapse: collapse; width: 100%; margin: auto;'>";
                    textoEmail += "<thead style='background-color: #ac0133;'><tr><th style='border: 1px solid #ffffff; text-align: center; padding: 8px; color: #ffffff;'>Tienda</th><th style='border: 1px solid #ffffff; text-align: center; padding: 8px; color: #ffffff;'>MontoSencillo</th><th style='border: 1px solid #ffffff; text-align: center; padding: 8px; color: #ffffff;'>Banco</th><th style='border: 1px solid #ffffff; text-align: center; padding: 8px; color: #ffffff;'>DiaLiberacion</th><th style='border: 1px solid #ffffff; text-align: center; padding: 8px; color: #ffffff;'>DiaEntrega</th></tr></thead>";

                    textoEmail += "<tbody>";
                    foreach (var item in sencillosTesorerias)
                    {
                        textoEmail += "<tr>";
                        textoEmail += "<td style='border: 1px solid #ddd; padding: 8px; text-align: left;'>" + item.IdTienda.ToString() + "</td>";
                        textoEmail += "<td style='border: 1px solid #ddd; padding: 8px; text-align: left;'>" + item.IdDetalleSencilloNavigation.NuevoTotal.Value.ToString() + "</td>";
                        textoEmail += "<td style='border: 1px solid #ddd; padding: 8px; text-align: left;'>" + item.IdDetalleSencilloNavigation.Banco.ToString() + "</td>";
                        textoEmail += "<td style='border: 1px solid #ddd; padding: 8px; text-align: left;'>" + item.IdDetalleSencilloNavigation.DiaLiberacion.ToString() + "</td>";
                        textoEmail += "<td style='border: 1px solid #ddd; padding: 8px; text-align: left;'>" + item.IdDetalleSencilloNavigation.DiaEntrega.ToString() + "</td>";
                        textoEmail += "</tr>";
                    }
                    textoEmail += "</tbody></table>";


                }


                textoEmail += "Saludos Atentamente";



                AlternateView htmlView = AlternateView.CreateAlternateViewFromString(textoEmail, null, "text/html");

                mail.BodyEncoding = UTF8Encoding.UTF8;
                mail.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;


                mail.AlternateViews.Add(htmlView);
                mail.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;
                //mail.Attachments.Add(data);
                client.Send(mail);

                response.error = false;
                response.mensaje = "CORREO ENVIADO CORRECTAMENTE";

                // SE ACTUALIZA FECHA ENVIO QR
                using (var dbcontext = db.Database.BeginTransaction())
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
        public ResponseModel EnviodeAlertaSencillos(List<SencillosTienda> sencillosTesorerias, string Asunto )
        {
            var response = new ResponseModel();
            try
            {

            
                //var asunto = "Prueba Alerta Sencillos Tesoreria";
                var asunto = Asunto;
                String textoEmail = "";
                SmtpClient client = GetSmtpcliente();
             
                MailMessage mail = new MailMessage();


                ////mail.To.Add(new MailAddress("tesoreria@fashionspark.com"));
                //mail.To.Add(new MailAddress("eugenio.barra@fashionspark.com"));
                mail.To.Add(new MailAddress("cuadratura.fp@fashionspark.com"));
                //mail.To.Add(new MailAddress("ricardo.sanchez@fashionspark.com"));
                mail.To.Add(new MailAddress("marcelo.roa@fashionspark.com"));



                mail.From = new MailAddress(Correo.Email);
                //mail.From = new MailAddress("pedro.astete@fashionspark.com");
                mail.Subject = asunto;
                mail.IsBodyHtml = false;
                var Descripcion = sencillosTesorerias.Select(x =>x.CodigoEstadoSencilloNavigation.Nombre).FirstOrDefault();


               
                foreach (var Fch in sencillosTesorerias.GroupBy(x => new { x.IdDetalleSencilloNavigation.IdSencilloNavigation.FechaInicio, x.IdDetalleSencilloNavigation.IdSencilloNavigation.FechaFin }).Select(x => new { Inico = x.Key.FechaInicio, Fin = x.Key.FechaFin }).ToList())
                {


                    textoEmail += "Tiendas que estan estado : " + Descripcion + " " + Fch.Inico.Value.ToString("dd-MM-yyyy") + " " + Fch.Fin.Value.ToString("dd-MM-yyyy") + " Son : <br>";
                    textoEmail += "<table style='border-collapse: collapse; width: 100%; margin: auto;'>";
                    textoEmail += "<thead style='background-color: #ac0133;'><tr><th style='border: 1px solid #ffffff; text-align: center; padding: 8px; color: #ffffff;'>Tienda</th><th style='border: 1px solid #ffffff; text-align: center; padding: 8px; color: #ffffff;'>MontoSencillo</th><th style='border: 1px solid #ffffff; text-align: center; padding: 8px; color: #ffffff;'>Banco</th><th style='border: 1px solid #ffffff; text-align: center; padding: 8px; color: #ffffff;'>DiaLiberacion</th><th style='border: 1px solid #ffffff; text-align: center; padding: 8px; color: #ffffff;'>DiaEntrega</th></tr></thead>";

                    textoEmail += "<tbody>";
                    foreach (var item in sencillosTesorerias.Where(x=>x.IdDetalleSencilloNavigation.IdSencilloNavigation.FechaInicio == Fch.Inico && x.IdDetalleSencilloNavigation.IdSencilloNavigation.FechaFin == Fch.Fin))
                    {
                        textoEmail += "<tr>";
                        textoEmail += "<td style='border: 1px solid #ddd; padding: 8px; text-align: left;'>" + item.IdTienda.ToString() + "</td>";
                        textoEmail += "<td style='border: 1px solid #ddd; padding: 8px; text-align: left;'>" + item.IdDetalleSencilloNavigation.NuevoTotal.Value.ToString() + "</td>";
                        textoEmail += "<td style='border: 1px solid #ddd; padding: 8px; text-align: left;'>" + item.IdDetalleSencilloNavigation.Banco.ToString() + "</td>";
                        textoEmail += "<td style='border: 1px solid #ddd; padding: 8px; text-align: left;'>" + item.IdDetalleSencilloNavigation.DiaLiberacion.ToString() + "</td>";
                        textoEmail += "<td style='border: 1px solid #ddd; padding: 8px; text-align: left;'>" + item.IdDetalleSencilloNavigation.DiaEntrega.ToString() + "</td>";
                        textoEmail += "</tr>";
                    }
                    textoEmail += "</tbody></table>";

                    
                }


                textoEmail += "Saludos Atentamente";



                AlternateView htmlView = AlternateView.CreateAlternateViewFromString(textoEmail, null, "text/html");

                mail.BodyEncoding = UTF8Encoding.UTF8;
                mail.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;
             

                mail.AlternateViews.Add(htmlView);
                mail.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;
                //mail.Attachments.Add(data);
                client.Send(mail);

                response.error = false;
                response.mensaje = "CORREO ENVIADO CORRECTAMENTE";

                // SE ACTUALIZA FECHA ENVIO QR
                using (var dbcontext = db.Database.BeginTransaction())
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


        public ResponseModel AlertaSencillosDevolucionTienda(List<SencillosTienda> sencillosTesorerias, string Asunto)
        {
            var response = new ResponseModel();
            try
            {


                //var asunto = "Prueba Alerta Sencillos Tesoreria";
                var asunto = Asunto;
                //string textoEmail = "";
                SmtpClient client = GetSmtpcliente();

                List<int> idTiendas = new List<int> { 19, 20, 25 };
                //foreach (var Tienda in sencillosTesorerias.Where(x => idTiendas.Contains(x.IdTienda.Value)))
                foreach (var Tienda in sencillosTesorerias)
                {
                    MailMessage mail = new MailMessage();
                    string textoEmail = "";

                    //DateTime? FechaEntrega = ObtenerFechaPorDiaDeLaSemana(Tienda.IdDetalleSencilloNavigation.IdSencilloNavigation.FechaInicio.Value, Tienda.IdDetalleSencilloNavigation.IdSencilloNavigation.FechaFin.Value, Tienda.IdDetalleSencilloNavigation.DiaEntrega);
                    //if ((FechaEntrega.Value - DateTime.Now).Days <= 3)
                    //{
                    mail.To.Add(Tienda.IdTiendaNavigation.Correo);
                    //Comentar Cuando termine el proceso
                    mail.To.Add(new MailAddress("cuadratura.fp@fashionspark.com"));
                    //    mail.To.Add(new MailAddress("tesoreria@fashionspark.com"));
                    //}

                    //mail.To.Add(new MailAddress("ricardo.sanchez@fashionspark.com"));

                    mail.From = new MailAddress(Correo.Email);
                    //mail.From = new MailAddress("pedro.astete@fashionspark.com");
                    mail.Subject = asunto;
                    mail.IsBodyHtml = false;



                    textoEmail += "Estimada Tienda : <br><br>";
                    textoEmail += "<strong>" + Tienda.IdTiendaNavigation.CenNombre.ToString() + "</strong><br><br>";
                    textoEmail += "Tiene un Sencillo por Devolver Con Correlativo N° : " + Tienda.IdDetalleSencilloNavigation.IdSencilloNavigation.Correlativo + " <br>";
                    textoEmail += "Por favor, Devolver el Sencillo en el siguiente enlace: ";
                    textoEmail += "<a href=\"http://172.18.14.98/SistemaGestion/Tienda/Sencillo\" style='color: black;'><strong>Link</strong></a><br>";




                    textoEmail += "Saludos Atentamente";



                    AlternateView htmlView = AlternateView.CreateAlternateViewFromString(textoEmail, null, "text/html");

                    mail.BodyEncoding = UTF8Encoding.UTF8;
                    mail.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;


                    mail.AlternateViews.Add(htmlView);
                    mail.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;

                    client.Send(mail);



                }




                response.error = false;
                response.mensaje = "CORREO ENVIADO CORRECTAMENTE";




            }
            catch (Exception ex)
            {
                response.error = true;
                response.mensaje = "ERROR: " + ex.Message;
            }
            return response;



        }

        public ResponseModel EnviodeAlertaSencillosTiendas(List<SencillosTienda> sencillosTesorerias, string Asunto)
        {
            var response = new ResponseModel();
            try
            {


                //var asunto = "Prueba Alerta Sencillos Tesoreria";
            
                //string textoEmail = "";
                SmtpClient client = GetSmtpcliente();

                //List<int> idTiendas = new List<int> { 19, 20, 25 };
                //foreach (var Tienda in sencillosTesorerias.Where(x => idTiendas.Contains(x.IdTienda.Value)))
                    foreach (var Tienda in sencillosTesorerias)
                    {
                    int dias = Math.Abs((Tienda.IdDetalleSencilloNavigation.FechaEntrega.Value - DateTime.Now).Days);
                    var asunto = dias > 1 ? "Retraso de Alerta Sencillo No Aceptado" : Asunto ;
                    MailMessage mail = new MailMessage();
                    string textoEmail = "";
                    mail.To.Add(Tienda.IdTiendaNavigation.Correo);

                    //Comentar Cuando Termine el Proceso
                    mail.To.Add(new MailAddress("cuadratura.fp@fashionspark.com"));


                    //mail.To.Add(new MailAddress("ricardo.sanchez@fashionspark.com"));

                    mail.From = new MailAddress(Correo.Email);
                    //mail.From = new MailAddress("pedro.astete@fashionspark.com");
                    mail.Subject = asunto;
                    mail.IsBodyHtml = false;

                 

                    textoEmail += "Estimada Tienda : <br><br>";
                    textoEmail += "<strong>" + Tienda.IdTiendaNavigation.CenNombre.ToString() + "</strong><br><br>";
                    textoEmail += "Tiene un Sencillo por Confirmar que debería haber llegado el día " + Tienda.IdDetalleSencilloNavigation.FechaEntrega.Value.ToString("dd-MM-yyyy") + ".<br>";
                    if (dias != 0)
                    {
                        textoEmail += "Tiene : <span style=\"color: black;\">" + dias.ToString() + " Dias de Retraso" + "</span><br><br>";
                    }
                    textoEmail += "Por favor, confirme el Sencillo en el siguiente enlace: ";
                    textoEmail += "<a href=\"http://172.18.14.98/SistemaGestion/Tienda/Sencillo\" style='color: black;'><strong>Link</strong></a><br>";




                    textoEmail += "Saludos Atentamente";



                    AlternateView htmlView = AlternateView.CreateAlternateViewFromString(textoEmail, null, "text/html");

                    mail.BodyEncoding = UTF8Encoding.UTF8;
                    mail.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;


                    mail.AlternateViews.Add(htmlView);
                    mail.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;

                    client.Send(mail);



                }




                response.error = false;
                response.mensaje = "CORREO ENVIADO CORRECTAMENTE";

               


            }
            catch (Exception ex)
            {
                response.error = true;
                response.mensaje = "ERROR: " + ex.Message;
            }
            return response;



        }

        public ResponseModel CargaCartaInstruccion(string respuesta, DateTime inicio, DateTime fin)
        {
            var response = new ResponseModel();
            try
            {


                var asunto = "Se ha Cargado Satisfactoriamente la Carta de Instruccion";
                String textoEmail = "";
                SmtpClient client = GetSmtpcliente();


                //client.EnableSsl = true;
                //client.Timeout = 1000000000;
                //client.DeliveryMethod = SmtpDeliveryMethod.Network;
                //client.UseDefaultCredentials = false;

                // nos autenticamos con nuestra cuenta de gmail

                //client.Credentials = new NetworkCredential("bienestar@fashionspark.com", "rrhh.2021");
                client.Credentials = new NetworkCredential(Correo.Email, Correo.Pass);//cambio contraseña 
                MailMessage mail = new MailMessage();
                //mail.To.Add(new MailAddress("eugenio.barra@fashionspark.com"));
                mail.To.Add(new MailAddress("tesoreria@fashionspark.com"));
                //mail.To.Add(new MailAddress("ricardo.sanchez@fashionspark.com"));
                mail.To.Add(new MailAddress("marcelo.roa@fashionspark.com"));
                mail.To.Add(new MailAddress("cuadratura.fp@fashionspark.com"));
                //mail.To.Add(new MailAddress("remigio.saez@fashionspark.com"));

                mail.From = new MailAddress(Correo.Email);
                //mail.From = new MailAddress("pedro.astete@fashionspark.com");
                mail.Subject = asunto;
                mail.IsBodyHtml = false;


                respuesta = respuesta.Replace("%", " ");
                textoEmail = respuesta + " En el periodo " + inicio.ToString("dd-MM-yyyy") + " / " + fin.ToString("dd-MM-yyyy");
                textoEmail += "<br>Saludos Atentamente";

                AlternateView htmlView = AlternateView.CreateAlternateViewFromString(textoEmail, null, "text/html");

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
                using (var dbcontext = db.Database.BeginTransaction())
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

        #endregion

        public ResponseModel EnvioCorreoCierreMes(string respuesta)
        {
            var response = new ResponseModel();
            try
            {


                var asunto = "Alerta de Cierre de Mes";
                String textoEmail = "";
                SmtpClient client = GetSmtpcliente();


                //client.EnableSsl = true;
                //client.Timeout = 1000000000;
                //client.DeliveryMethod = SmtpDeliveryMethod.Network;
                //client.UseDefaultCredentials = false;

                // nos autenticamos con nuestra cuenta de gmail

                //client.Credentials = new NetworkCredential("bienestar@fashionspark.com", "rrhh.2021");
                client.Credentials = new NetworkCredential(Correo.Email, Correo.Pass);//cambio contraseña 
                MailMessage mail = new MailMessage();
                //mail.To.Add(new MailAddress("eugenio.barra@fashionspark.com"));

                //mail.To.Add(new MailAddress("ricardo.sanchez@fashionspark.com"));
                mail.To.Add(new MailAddress("cuadratura.fp@fashionspark.com"));
                //mail.To.Add(new MailAddress("remigio.saez@fashionspark.com"));
                mail.To.Add(new MailAddress("marcelo.roa@fashionspark.com"));

                mail.From = new MailAddress(Correo.Email);
                //mail.From = new MailAddress("pedro.astete@fashionspark.com");
                mail.Subject = asunto;
                mail.IsBodyHtml = false;



                textoEmail = respuesta;
                textoEmail += "<br>Saludos Atentamente";

                AlternateView htmlView = AlternateView.CreateAlternateViewFromString(textoEmail, null, "text/html");

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
                using (var dbcontext = db.Database.BeginTransaction())
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

        public SmtpClient GetSmtpcliente()
        {
            SmtpClient client = new SmtpClient("smtp.office365.com", 587);

            client.EnableSsl = Correo.EnableSsl;
            client.Timeout = Correo.Timeout;
            client.DeliveryMethod = Correo.DeliveryMethod;
            client.UseDefaultCredentials = Correo.UseDefaultCredentials;

            // nos autenticamos con nuestra cuenta de gmail

            //client.Credentials = new NetworkCredential("bienestar@fashionspark.com", "rrhh.2021");
            client.Credentials = new NetworkCredential(Correo.Email, Correo.Pass);//cambio contraseña  .EnableSsl;

            return client;
        }

        public void ErrorApiTransbank(ResponseModel Texto)
        {
            var response = new ResponseModel();
            try
            {


                var asunto = "Error WorkerApiTransbank";
                String textoEmail = "";
                SmtpClient client = new SmtpClient("smtp.office365.com", 587);


                client.EnableSsl = true;
                client.Timeout = 1000000000;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = false;

                // nos autenticamos con nuestra cuenta de gmail

                //client.Credentials = new NetworkCredential("bienestar@fashionspark.com", "rrhh.2021");
                client.Credentials = new NetworkCredential("sisges.cuadratura@fashionspark.com", "Cat03082");//cambio contraseña 
                MailMessage mail = new MailMessage();
                //mail.To.Add(new MailAddress("eugenio.barra@fashionspark.com"));
                mail.To.Add(new MailAddress("patricio.meneses@fashionspark.com"));
                //mail.To.Add(new MailAddress("ricardo.sanchez@fashionspark.com"));
                //mail.To.Add(new MailAddress("remigio.saez@fashionspark.com"));
                mail.To.Add(new MailAddress("marcelo.roa@fashionspark.com"));

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

                //textoEmail = " <strong>Alerta de Error en WokerdeApiTransbank</strong><br><br>";
                //textoEmail = Texto.Message ;
                textoEmail = "  <strong> Estimados el Worker a Presentado un error a las: " + DateTime.Now + " </strong><br>" +
                "<div style= text-align: justify;>ExcepcionMensaje :" + Texto.mensaje + "<br>" +
                "ExcepInnerException: " + "</div>";

                AlternateView htmlView = AlternateView.CreateAlternateViewFromString(textoEmail, null, "text/html");

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
                //using (var dbcontext = db.Database.BeginTransaction())
                //{
                //    //var dato = db.Comensals.Where(x => x.IdComensal == guid).FirstOrDefault();
                //    //dato.FechaEnvioQr = DateTime.Now;
                //    //db.SaveChanges();
                //    //dbcontext.Commit();
                //}


            }
            catch (Exception ex)
            {
                response.error = true;
                response.mensaje = "ERROR: " + ex.Message;
            }




        }


    }
}
