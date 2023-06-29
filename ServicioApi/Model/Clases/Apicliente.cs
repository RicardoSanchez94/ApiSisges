using Newtonsoft.Json;
using OfficeOpenXml;
using ServicioApi.Model.SisGes;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading.Tasks;

namespace ServicioApi.Model.Clases
{
    public class Apicliente
    {

        #region Constantes
        public static string UrlBase = "https://api.transbank.cl/transbank/publico/";
        public static string TokenId = "x-client-id";
        public static string TokenHeaderPass = "8eefa93c78f05a8ea717b0b23151bd7e";
        private EnviodeCorreo Correo = new EnviodeCorreo();
        private List<TransaccionesDetalle> DatosTransaccion = new List<TransaccionesDetalle>();
        private List<ApiGuardar> lstApiGuardar = new List<ApiGuardar>();
  

        #endregion

        #region Metodos

        public async Task<ResponseModel> ObtenerTransacc(DateTime FechaInicio, DateTime FechaFin, int intentos)
        {
            ResponseModel responseModel = new ResponseModel();
           var Api = Helper.Helper.ApiTransbank();
            try
            {
                //var url = string.Format(UrlBase + "transacciones?fecha-desde={0}&fecha-hasta={1}&page-size=500", FechaInicio, FechaFin);
                var url = string.Format(Api.UrlBase + Api.EndpointTransacciones, FechaInicio.ToString("yyyy-MM-dd"), FechaFin.ToString("yyyy-MM-dd"));
                //HttpClient cliente = new HttpClient();
                //HttpClientHandler clientHandler = new HttpClientHandler();
                //clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
                using (HttpClient cliente = new HttpClient())
                {
                    HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Get, url);
                    var total = new Transacciones.Root();
                    requestMessage.Headers.Add("Accept", Api.Accept);
                    requestMessage.Headers.Add(Api.TokenId, Api.TokenHeaderPass);
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    var response = await cliente.SendAsync(requestMessage);

                    var estatus = response.StatusCode;
                    if (!response.IsSuccessStatusCode)
                    {
                        Debug.WriteLine(estatus);
                        //if (estatus == HttpStatusCode.InternalServerError)
                        //{
                        //    responseModel.error = true;
                        //    responseModel.status = estatus.ToString();
                        //    responseModel.mensaje = "A ocurrido un error";
                        //    return responseModel;

                        //}
                        //if (estatus == HttpStatusCode.TooManyRequests)
                        //{
                        //    responseModel.error = true;
                        //    responseModel.status = estatus.ToString();
                        //    responseModel.mensaje = "Excedio la Cantidad de Peticciones en una Hora";
                        //    return responseModel;
                        //}

                        //var envio = Correo.ErrorApiTransbank("Trajo null en la primera peticcion");
                        Console.WriteLine("primera peticion trajo null T_T");
                        Thread.Sleep(10000);
                        if (intentos > 5)
                        {
                            responseModel.error = true;
                            responseModel.status = estatus.ToString();
                            responseModel.mensaje = "A superado la cantidad de intentos";
                            Correo.ErrorApiTransbank(responseModel);
                            return responseModel;
                        }
                        responseModel = await ObtenerTransacc(FechaInicio, FechaFin, intentos++);

                    }
                    var contenido = await response.Content.ReadAsStringAsync();
                    //total = JsonConvert.DeserializeObject<Transacciones.Root>(contenido);
                    responseModel.respuesta = contenido;
                    responseModel.error = false;

                    return responseModel;
                }


            }
            catch (Exception)
            {

                responseModel.error = true;
                responseModel.mensaje = "Error al ejecutar la Api TBK";
                Correo.ErrorApiTransbank(responseModel);
                return responseModel;
            }

        }


        public async Task<ResponseModel> Paginas(DateTime FechaInicio, DateTime FechaFin, int num, int intentos)
        {
            var Api = Helper.Helper.ApiTransbank();
            //var url = string.Format("https://api.transbank.cl/transbank/publico/transacciones?fecha-desde={0}&fecha-hasta={1}&page-number={2}&page-size=500", FechaInicio, FechaFin, num);
            var url = string.Format(Api.EndpointFull, FechaInicio.ToString("yyyy-MM-dd"), FechaFin.ToString("yyyy-MM-dd"), num);
      

            ResponseModel responseModel = new ResponseModel();
            using (HttpClient cliente = new HttpClient())
            {
                HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Get, url);
                requestMessage.Headers.Add(Api.TokenId, Api.TokenHeaderPass);
                var response = await cliente.SendAsync(requestMessage);
                var contenido = await response.Content.ReadAsStringAsync();
                var estatus = response.StatusCode;

                var total = JsonConvert.DeserializeObject<Transacciones.Root>(contenido);
                if (total.data == null)
                {
                    Debug.WriteLine(estatus);
                    //var envio = Correo.ErrorApiTransbank("Trajo null en la pagina :"+ num);
                    Console.WriteLine("primera peticion trajo null T_T paggina = " + num);
                    Thread.Sleep(10000);
                    if (intentos > 5)
                    {
                        responseModel.error = true;
                        responseModel.status = estatus.ToString();
                        responseModel.mensaje = "A superado la cantidad de intentos";
                        Correo.ErrorApiTransbank(responseModel);
                        return responseModel;
                    }
                    responseModel = await Paginas(FechaInicio, FechaFin, num, intentos++);

                }
                responseModel.respuesta = contenido;
                responseModel.error = false;

                return responseModel;
            }

        }


        public async Task<ResponseModel> GetDataTbk(DateTime FechaInicio, DateTime FechaFin)
        {
            ResponseModel response = new ResponseModel();
            List<Task<ResponseModel>> tasks = new List<Task<ResponseModel>>();
            List<TransaccionesDetalle> DatosTransaccion = new List<TransaccionesDetalle>();
            try
            {
                var pruebaawait = await ObtenerTransacc(FechaInicio, FechaFin, 1);
                if (pruebaawait.error)
                {
                    return pruebaawait;
                }

                TransaccionesDetalle oTransacciones = new TransaccionesDetalle();
                oTransacciones.Root = JsonConvert.DeserializeObject<Transacciones.Root>(pruebaawait.respuesta);
                var linkslast = oTransacciones.Root.links.last;
                var inicio = linkslast.IndexOf("page-number=") + 12;
                var termino = linkslast.IndexOf("&page-size");
                var numeroPaginas = int.Parse(linkslast.Substring(inicio, termino - inicio));
                DatosTransaccion.Add(oTransacciones);

                for (int i = 2; i <= numeroPaginas; i++)
                {
                    await Task.Delay(TimeSpan.FromSeconds(5));
                    var task = Paginas(FechaInicio, FechaFin, i, 1);
                    tasks.Add(task);
                }

                var results = await Task.WhenAll(tasks);

                foreach (var result in results)
                {
                    if (result.error)
                    {
                        return result;
                    }

                    TransaccionesDetalle oTransacciones1 = new TransaccionesDetalle();
                    oTransacciones1.Root = JsonConvert.DeserializeObject<Transacciones.Root>(result.respuesta);
                    DatosTransaccion.Add(oTransacciones1);
                }

                response = InsertarDatos(DatosTransaccion, FechaInicio);
                if (response.error)
                {
                    return response;
                }

                response.error = false;
                response.mensaje = "Se Inserto Correctamente";
                response.status = "200";
                return response;

            }
            catch (Exception ex)
            {
                response.error = true;
                response.mensaje = "Error al ejecutar " + ex.Message;
                Correo.ErrorApiTransbank(response);
                return response;
            }

            //try
            //{
            //    var pruebaawait = await ObtenerTransacc(FechaInicio, FechaFin, 1);
            //    if (pruebaawait.error)
            //    {
            //        return pruebaawait;
            //    }

            //    TransaccionesDetalle oTransacciones = new TransaccionesDetalle();
            //    oTransacciones.Root = JsonConvert.DeserializeObject<Transacciones.Root>(pruebaawait.respuesta);
            //    var linkslast = oTransacciones.Root.links.last;
            //    //var linkslast = "";
            //    var inicio = linkslast.IndexOf("page-number=") + 12;
            //    var termino = linkslast.IndexOf("&page-size");
            //    var numeroPaginas = int.Parse(linkslast.Substring(inicio, termino - inicio));
            //    DatosTransaccion.Add(oTransacciones);

            //    for (int i = 2; i <= numeroPaginas; i = i + 5)
            //    {
            //        bool espera1 = false;
            //        bool espera2 = false;
            //        bool espera3 = false;
            //        bool espera4 = false;
            //        bool espera5 = false;
            //        TransaccionesDetalle oTransacciones1 = new TransaccionesDetalle();
            //        TransaccionesDetalle oTransacciones2 = new TransaccionesDetalle();
            //        TransaccionesDetalle oTransacciones3 = new TransaccionesDetalle();
            //        TransaccionesDetalle oTransacciones4 = new TransaccionesDetalle();
            //        TransaccionesDetalle oTransacciones5 = new TransaccionesDetalle();

            //        //Task<Transacciones.Root> Root1 = null;
            //        //Task<Transacciones.Root> Root2 = null;
            //        //Task<Transacciones.Root> Root3 = null;
            //        //Task<Transacciones.Root> Root4 = null;
            //        //Task<Transacciones.Root> Root5 = null;

            //        Task<ResponseModel> Root1 = null;
            //        Task<ResponseModel> Root2 = null;
            //        Task<ResponseModel> Root3 = null;
            //        Task<ResponseModel> Root4 = null;
            //        Task<ResponseModel> Root5 = null;

            //        if (i <= numeroPaginas)
            //        {
            //            //Thread.Sleep(5000);
            //            Root1 = Paginas(FechaInicio, FechaFin, (i), 1);
            //            espera1 = true;
            //        }
            //        if ((i + 1) <= numeroPaginas)
            //        {
            //            Thread.Sleep(5000);
            //            Root2 = Paginas(FechaInicio, FechaFin, (i + 1), 1);
            //            espera2 = true;
            //        }
            //        if ((i + 2) <= numeroPaginas)
            //        {
            //            Thread.Sleep(5000);
            //            Root3 = Paginas(FechaInicio, FechaFin, (i + 2), 1);
            //            espera3 = true;
            //        }
            //        if ((i + 3) <= numeroPaginas)
            //        {
            //            Thread.Sleep(5000);
            //            Root4 = Paginas(FechaInicio, FechaFin, (i + 3), 1);
            //            espera4 = true;
            //        }
            //        if ((i + 4) <= numeroPaginas)
            //        {
            //            Thread.Sleep(5000);
            //            Root5 = Paginas(FechaInicio, FechaFin, (i + 4), 1);
            //            espera5 = true;
            //        }

            //        if (espera1)
            //        {

            //            var LT = await Root1;
            //            if (LT.error)
            //            {
            //                return LT;
            //            }
            //            oTransacciones1.Root = JsonConvert.DeserializeObject<Transacciones.Root>(LT.respuesta);
            //            DatosTransaccion.Add(oTransacciones1);
            //            Debug.WriteLine("Termino la peticcion " + i);

            //            // _logger.LogInformation("Termino la peticcion " + i);

            //        }
            //        if (espera2)
            //        {

            //            var LT = await Root2;
            //            if (LT.error)
            //            {
            //                return LT;
            //            }
            //            oTransacciones2.Root = JsonConvert.DeserializeObject<Transacciones.Root>(LT.respuesta);
            //            DatosTransaccion.Add(oTransacciones2);


            //        }
            //        if (espera3)
            //        {
            //            var LT = await Root3;
            //            if (LT.error)
            //            {
            //                return LT;
            //            }
            //            oTransacciones3.Root = JsonConvert.DeserializeObject<Transacciones.Root>(LT.respuesta);
            //            DatosTransaccion.Add(oTransacciones3);


            //        }
            //        if (espera4)
            //        {

            //            var LT = await Root4;
            //            if (LT.error)
            //            {
            //                return LT;
            //            }
            //            oTransacciones4.Root = JsonConvert.DeserializeObject<Transacciones.Root>(LT.respuesta);
            //            DatosTransaccion.Add(oTransacciones4);

            //        }
            //        if (espera5)
            //        {

            //            var LT = await Root5;
            //            if (LT.error)
            //            {
            //                return LT;
            //            }
            //            oTransacciones5.Root = JsonConvert.DeserializeObject<Transacciones.Root>(LT.respuesta);
            //            DatosTransaccion.Add(oTransacciones5);

            //        }
            //    }
            //    Debug.WriteLine("Termino la peticcion ");
            //    response = InsertarDatos(DatosTransaccion, FechaInicio);
            //    if (response.error)
            //    {
            //        return response;
            //    }
            //    response.error = false;
            //    response.mensaje = "Se Inserto Correctamente";
            //    response.status = "200";
            //    return response;

            //}
            //catch (Exception ex)
            //{


            //    response.error = true;
            //    //response.status = ex.
            //    response.mensaje = "Error al ejecutar " + ex.Message;
            //    Correo.ErrorApiTransbank(response);
            //    return response;
            //}

        }

        public ResponseModel InsertarDatos(List<TransaccionesDetalle> DatosTransaccion , DateTime Fechasistema )
        {
            ResponseModel response = new ResponseModel();
            try
            {
                foreach (var item in DatosTransaccion.Where(x => x.Root.data != null))
                {
                    item.Root.data.ForEach((a) => lstApiGuardar.Add(
                            new ApiGuardar()
                            {
                                codigoAutorizacion = (a.codigoAutorizacion),
                                codigoLocal = (a.codigoLocal),
                                fechaAbono = (a.fechaAbono),
                                fechaTransaccion = (a.fechaTransaccion),
                                montoCuotas = (a.montoCuotas),
                                montoVenta = (a.montoVenta),
                                nombreLocal = (a.nombreLocal),
                                numeroTarjeta = (a.numeroTarjeta),
                                tipoProducto = (a.tipoProducto),
                                tipoTransaccion = (a.tipoTransaccion),
                                totalCuotas = (a.totalCuotas),
                                tipoCuota = (a.tipoCuota),
                                montoAfecto = (a.montoAfecto),
                                montoExentoTotal = (a.montoExentoTotal),
                                ordenPedido = (a.ordenPedido),
                            }));

                }
                var dt = new DataTable();
                dt.Columns.Add("codigoAutorizacion", typeof(string));
                dt.Columns.Add("codigoLocal", typeof(int));
                dt.Columns.Add("fechaAbono", typeof(DateTime));
                dt.Columns.Add("fechaTransaccion", typeof(DateTime));
                dt.Columns.Add("montoCuotas", typeof(decimal));
                dt.Columns.Add("montoVenta", typeof(decimal));
                dt.Columns.Add("nombreLocal", typeof(string));
                dt.Columns.Add("numeroTarjeta", typeof(string));
                dt.Columns.Add("tipoProducto", typeof(string));
                dt.Columns.Add("tipoTransaccion", typeof(string));
                dt.Columns.Add("totalCuotas", typeof(int));
                dt.Columns.Add("tipoCuota", typeof(string));
                dt.Columns.Add("montoAfecto", typeof(decimal));
                dt.Columns.Add("montoExentoTotal", typeof(string));
                dt.Columns.Add("ordenPedido", typeof(string));

                //SisgesDBContext db = new SisgesDBContext();
                var lst = lstApiGuardar.Where(x => !x.tipoTransaccion.Equals("RAA")).
                  GroupBy(x => new
                  {
                      codigoAutorizacion = x.codigoAutorizacion,
                      codigoLocal = x.codigoLocal,
                      fechaAbono = x.fechaAbono,
                      fechaTransaccion = x.fechaTransaccion,
                      montoCuotas = x.montoCuotas,
                      montoVenta = x.montoVenta,
                      nombreLocal = x.nombreLocal,
                      numeroTarjeta = x.numeroTarjeta,
                      tipoProducto = x.tipoProducto,
                      tipoTransaccion = x.tipoTransaccion,
                      montoAfecto = x.montoAfecto,
                      ordenPedido = x.ordenPedido
                  }).Select(x => new ApiGuardar
                  {
                      codigoAutorizacion = x.Key.codigoAutorizacion,
                      codigoLocal = x.Key.codigoLocal,
                      fechaAbono = x.Key.fechaAbono,
                      fechaTransaccion = x.Key.fechaTransaccion,
                      montoCuotas = x.Key.montoCuotas,
                      montoVenta = x.Key.montoVenta,
                      nombreLocal = x.Key.nombreLocal,
                      numeroTarjeta = x.Key.numeroTarjeta,
                      tipoProducto = x.Key.tipoProducto,
                      tipoTransaccion = x.Key.tipoTransaccion,
                      montoAfecto = x.Key.montoAfecto,
                      ordenPedido = x.Key.ordenPedido
                  }).ToList();
                ////_logger.LogInformation("Cantidad Ventas Rescata por la Api : " + lstApiGuardar.Count);
                ////_logger.LogInformation("Cantidad Ventas Rescata por la Api : " + lst.Count);
                Debug.WriteLine("Cantidad Ventas Rescata por la Api : " + lst.Count);
                string ruta = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"File");
                if (!System.IO.Directory.Exists(ruta))
                {
                    System.IO.Directory.CreateDirectory(ruta);
                }
                using (ExcelPackage excelPackage = new Execelprueba().ConsumoAPI(lst.ToList()))
                {
                    if (File.Exists(ruta + "\\apitbk" + ".xlsx"))
                    {
                        File.Delete(ruta + "\\apitbk" + ".xlsx");
                    }
                    excelPackage.SaveAs(ruta + "\\apitbk" + ".xlsx");

                }

                foreach (var a in lst.ToList() /*lstApiGuardar.Where(x => !x.tipoTransaccion.Equals("RAA")).Distinct()*/)
                {
                    dt.Rows.Add(a.codigoAutorizacion, a.codigoLocal, (a.fechaAbono.Value.Year == 0001 ? null : a.fechaAbono), a.fechaTransaccion, a.montoCuotas,
                    a.montoVenta, a.nombreLocal.Replace("\r\n", "").Replace("\n", "").Replace("\r", ""), a.numeroTarjeta, a.tipoProducto, a.tipoTransaccion,
                    a.totalCuotas, a.tipoCuota, a.montoAfecto, a.montoExentoTotal, a.ordenPedido);
                }
                //_logger.LogInformation("Se inicio proceso de Almacenado");
                //_logger.LogInformation(dt.Rows.Count.ToString());
                var con = Helper.Helper.ConexionStringModel();
                using (SqlConnection sql = new SqlConnection(con))
                {
                    sql.Open();
                    //_logger.LogInformation("Apertura de Conexion BD SisGes");
                    Debug.WriteLine("Apertura de Conexion BD SisGes");
                    using (SqlCommand cmd = new SqlCommand("spInsertTransbankApi", sql))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.CommandTimeout = 10000000;
                        cmd.Parameters.AddWithValue("@TAU", dt);
                        //cmd.Parameters.AddWithValue("@TAU", null);
                        cmd.Parameters.AddWithValue("@Fecha", Fechasistema);

                        //parametros.SqlDbType = SqlDbType.Structured;
                        var reader = cmd.ExecuteNonQuery();
                    }
                    sql.Close();
                }
                response.error = false;
                response.mensaje = "Se Inserto Correctamente";
                //Correo.ErrorApiTransbank(response);
                return response;
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "ERROR: ExecuteAsync");

                response.error = true;
                response.mensaje = "Error al Insertar a la base de datos" + ex.Message;
                Correo.ErrorApiTransbank(response);
                return response;

            }

        }

        #endregion

    }
}
