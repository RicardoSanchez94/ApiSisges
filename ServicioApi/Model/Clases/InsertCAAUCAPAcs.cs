using ICSharpCode.SharpZipLib.Lzw;
using Org.BouncyCastle.Utilities.Zlib;
using Renci.SshNet;
using ServicioApi.Model.SisGes;
using ServicioApi.Modelview;
using System.Text;
using System.Xml.Linq;

namespace ServicioApi.Model.Clases
{
    public class InsertCAAUCAPAcs

    {
        //private SisgesDBContext db = new SisgesDBContext();
        private readonly MySisgesDbcontext _db;

        public InsertCAAUCAPAcs(MySisgesDbcontext db)
        {
            _db = db;
        }
        public async Task<ResponseModel> StarProceso(DateTime Fecha)
        {
            ResponseModel response = new ResponseModel();
            //DateTime Fecha = DateTime.Today.AddDays(-2);
            //DateTime Fecha = new DateTime(2022, 05, 03);
            int Mes = Fecha.Month;
            int Yr = Fecha.Year;
            //  _logger.LogInformation("STAR" + Fecha2);

            try
            {
                //_logger.LogInformation("--Inicio de Carga CAAU y CAPA--");

                var config = Helper.Helper.GetCrendicales();
                //string Pathfile = @"/u/optimus/respaftp/certegy";
                string[] TipoInterfaz = new string[2];
                TipoInterfaz[0] = "CAAU";
                TipoInterfaz[1] = "CAPA";
                string nameFile = "";
                foreach (var item in TipoInterfaz.ToList())
                {
                    nameFile = item + Fecha.Day.ToString("00") + Fecha.Month.ToString("00") + "1D." + item + Fecha.Day.ToString("00") + Fecha.Month.ToString("00") + "1D.Z";
                    //string pathfilenombre = Pathfile + "/" + nameFile;
                    //_logger.LogInformation("ENTRANDO AL STFP PARA BUSCAR" + pathfilenombre);
                    SFTPSAp(config, nameFile, Fecha, item);
                    //lectura(Fecha, item);
                }
                //string nameFile = "CAAU" + Fecha2.Day.ToString("00") + Fecha2.Month.ToString("00") + "1D." + "CAAU" + Fecha2.Day.ToString("00") + Fecha2.Month.ToString("00") + "1D.Z";

                response.error = false;
                response.respuesta = "Se realizo la carga de las interfaces";
                
                return response;

            }
            catch (Exception ex)
            {

              response.error = true;
                response.respuesta = "Error al cargar las interfaces" + ex.Message; 
                return response;
            }

        }
        public ResponseModel lectura(DateTime Fecha, string Tipo)
        {
            ResponseModel response = new ResponseModel();
            var extension = ".Z";
            //var pathLocalFile = "/home/ftpuser/PasoAPI/R17_Api-Integracion-Tiendas/ServicioApi/CAAUCAPA/";
            var pathLocalFile = "C:/CaauCapa/";


            if (extension == ".Z")
            {
                InterfacesView newDocumentType = new InterfacesView();

                using (var inputStream = File.Open(pathLocalFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    using (var tazStream = new LzwInputStream(inputStream))
                    {
                        using (StreamReader lector = new StreamReader(tazStream, Encoding.UTF8))
                        {
                            //var value = lector.ReadLine();
                            var dataArray = GetLectorDoc(lector);
                            string muestra = dataArray[1].ToString();



                            if (muestra.Contains("|"))
                            {
                                newDocumentType = processFileWhithPipe(dataArray, Tipo, Fecha);
                            }


                            lector.Dispose();
                            lector.Close();
                        }
                        tazStream.Dispose();
                        tazStream.Close();
                    }
                    inputStream.Dispose();
                    inputStream.Close();
                }
                response = SaveInterfaz(newDocumentType);
            }
            return response;
        }

        #region ENVIO FTP PLANOS

        public ResponseModel SFTPSAp(sFTPCredentials config, string namefile, DateTime Fecha, string Tipo)
        {
            ResponseModel response = new ResponseModel();
            response.error = false;

            response.respuesta = "LA EXTENSIÓN DEL PLANO NO ES SOPORTADO POR LA APLICACIÓN, CONTACTE A ADMINISTRADOR DE SISTEMA.";


            //string prefijo = model.prefijoPlano;
            //string diaMes = model.fechaCargar.ToString("ddMM");
            //string document = prefijo + diaMes + "1D";
            //string extencionArchivo = "." + document;

            using (SftpClient sftp = credencialesOutPutSFTP(config))
            {
                string pathLocalFileAux = "";
                string pathLocalFile = "";
                try
                {
                    sftp.Connect();
                    if (sftp.IsConnected)
                    {
                        var file = sftp.ListDirectory(config.Path).Where(x => x.Name.Contains(namefile)).FirstOrDefault();
                        //string pathLocalFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), file.Name);                       

                        if (file == null)
                        {

                            response.error = true;
                             response.respuesta = "NO SE ENCONTRO PLANO CON LA FECHA INDICADA PARA PROCESAR" + namefile;
                            //_logger.LogInformation("NO SE ENCONTRO PLANO CON LA FECHA INDICADA PARA PROCESAR");
                            sftp.Disconnect();
                        }
                        else
                        {
                            var Rute = Helper.Helper.GetCrendicales();
                            pathLocalFile = Rute.RutaeDestino;
                            //pathLocalFile = Path.Combine(@"C:\CaauCapa\");
                            if (!Directory.Exists(pathLocalFile))
                            {
                                Directory.CreateDirectory(pathLocalFile);
                                //_logger.LogInformation("Se Crea el Directorio " + pathLocalFile);
                            }
                            pathLocalFile = pathLocalFile + file.Name;
                            // pathLocalFile = Path.Combine(@"~/CauCapa/" + file.Name);
                            pathLocalFileAux = pathLocalFile;

                            if (File.Exists(pathLocalFile))
                            {
                                File.Delete(pathLocalFile);
                            }
                            if (File.Exists(pathLocalFile))
                            {
                                response.error = true;
                                response.respuesta = "EL PLANO SELECCIONADO ESTA SIENDO PROCESADO POR OTRO USUARIO, INTENTE MAS TARDE.";
                                //_logger.LogInformation("EL PLANO SELECCIONADO ESTA SIENDO PROCESADO POR OTRO USUARIO, INTENTE MAS TARDE");
                                sftp.Disconnect();
                            }
                            else
                            {
                                using (Stream fileStream = File.OpenWrite(pathLocalFile))
                                {
                                    sftp.DownloadFile(file.FullName, fileStream);
                                    fileStream.Close();
                                    fileStream.Dispose();
                                }
                                // Otorga permisos de lectura
                                File.SetAttributes(pathLocalFile, FileAttributes.Normal);

                                sftp.Disconnect();
                                sftp.Dispose();
                                string currentFileName = file.FullName;
                                string newFile = Path.GetFileNameWithoutExtension(file.FullName);
                                string extension = Path.GetExtension(currentFileName);


                                if (extension == ".Z")
                                {
                                    InterfacesView newDocumentType = new InterfacesView();

                                    using (var inputStream = File.Open(pathLocalFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                                    {
                                        using (var tazStream = new LzwInputStream(inputStream))
                                        {
                                            using (StreamReader lector = new StreamReader(tazStream,                        Encoding.UTF8))
                                            {
                                                //var value = lector.ReadLine();
                                                var dataArray = GetLectorDoc(lector);
                                                string muestra = dataArray[1].ToString();



                                                if (muestra.Contains("|"))
                                                {
                                                    newDocumentType = processFileWhithPipe(dataArray,                               Tipo, Fecha);
                                                }


                                                lector.Dispose();
                                                lector.Close();
                                            }
                                            tazStream.Dispose();
                                            tazStream.Close();
                                        }
                                        inputStream.Dispose();
                                        inputStream.Close();
                                    }
                                    response = SaveInterfaz(newDocumentType);
                                }


                                //File.Delete(pathLocalFile);
                            }
                        }
                    }
                    else
                    {
                        response.error = false;
                        response.respuesta = "NO SE PUEDO CONECTAR AL SERVIDOR SFTP";
                    }
                    return response;
                }
                catch (Exception er)
                {
                    response.error = false;
                    response.respuesta = "CONTACTE AL ADMINISTRADOR DE SISTEMAS, ERROR: " + er.Message.ToString();
                    File.Delete(pathLocalFileAux);
                    return response;
                }
            }
        }

        #endregion

        #region  CLIENTES FTP Y SFTP


        public SftpClient credencialesOutPutSFTP(sFTPCredentials sftCredential)
        {
            string host = @sftCredential.Host;
            string username = sftCredential.Username;
            string password = @sftCredential.Password;
            //string path = @sftCredential.Path;
            SftpClient sftp = new SftpClient(host, username, password);
            return sftp;
        }

        #endregion

        public string[] GetLectorDoc(StreamReader stream)
        {
            string line;
            Int64 counter = 0;
            var list = new List<string>();
            //var docData = new string[]{}; //stream.ReadToEnd().Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

            while ((line = stream.ReadLine()) != null)
            {
                list.Add(line);
                counter++;
            }
            var lines = new string[list.Count()];
            for (int i = 0; i <= list.Count() - 1; i++)
            {
                lines[i] = list[i].ToString();
            }

            return lines;
        }
        public InterfacesView processFileWhithPipe(string[] archivo, string Tipo, DateTime Fecha)
        {
            var documentoT = new InterfacesView();


            //documentoT.OInterfaz.Idtipo = Tipo.IdTipo;
            //documentoT.OInterfaz.Id = Guid.NewGuid();
            //documentoT.OInterfaz.Mes = Fecha.Month;
            //documentoT.OInterfaz.Año = Fecha.Year;
            documentoT.Tipo = Tipo;
            InterfacesView newDocumentType = GetDocumentTypePipe(documentoT, archivo);
            Console.WriteLine(newDocumentType);
            return newDocumentType;
        }

        public InterfacesView GetDocumentTypePipe(InterfacesView documentType, string[] archivo)
        {
            try
            {

                #region INTERFAZ COAU_CAAU
                var nColumnas = 33;
                //if (documentType.OInterfaz.Idtipo == db.TipoInterfazs.Where(x => x.Nombre == "CAAU").Select(x => x.IdTipo).SingleOrDefault())
                if (documentType.Tipo == "CAAU")
                {

                    int contadorAnterior = 0;
                    int contadorNuevo = 0;
                    bool correcto = true;

                    for (int i = 0; i <= archivo.Count() - 1; i++)
                    {
                        var values = archivo[i].Split('|');
                        contadorNuevo = values.Count();

                        correcto = contadorNuevo < contadorAnterior ? false : true;

                        if (correcto)
                        {


                            contadorAnterior = contadorNuevo;


                            var prueba = values[25];

                            documentType.ListaInterfazAuto.Add(new InterfazAutorizacion()
                            {
                                //IdInterfaces = documentType.OInterfaz.Id,
                                //Id = Guid.NewGuid(),
                                CodigoEmpresa = values[0],
                                FechaAutorizacion = Helper.Helper.GetDateTimeFormatWhitTime(values[1]),
                                CodigoAgencia = values[2],
                                NumeroTerminal = values[3] != "" ? Int32.Parse(values[3]) : 0,
                                NumeroBoleta = values[4],
                                TipoTransaccion = values[5],
                                CodigoUsuario = values[6],
                                NumeroAutorizacion = values[7],
                                TipoArchivo = values[8],
                                CodigoCliente = values[9] != "" ? Int32.Parse(values[9]) : 0,
                                NumeroTarjeta = values[10].TrimStart('0'),
                                Monto = values[11] != "" ? Int32.Parse(values[11]) : 0,
                                MontoPie = values[12] != "" ? Int32.Parse(values[12]) : 0,
                                MesesDiferido = values[13] != "" ? Int32.Parse(values[13]) : 0,
                                MesesGracia = values[14] != "" ? Int32.Parse(values[14]) : 0,
                                FechaAutoriza = Helper.Helper.GetDateTimeFormatDate(values[15]),
                                NumeroCuotas = values[16] != "" ? Int32.Parse(values[16]) : 0,
                                Extrafinanciamiento = values[17],
                                CodigoPlanCredito = values[18] != "" ? Int32.Parse(values[18]) : 0,
                                MontoAfecto = values[19] != "" ? Int32.Parse(values[19]) : 0,
                                MontonoAfecto = values[20] != "" ? Int32.Parse(values[20]) : 0,
                                CodigoComercio = values[21] != "" ? Int32.Parse(values[21]) : 0,
                                CodigoRubro = values[22] != "" ? Int32.Parse(values[22]) : 0,
                                RutVendedor = values[23],
                                CodigoPromocionVenta = values[24] != "" ? Int32.Parse(values[24]) : 0,
                                MontoCuota = values[25] != "" ? Int32.Parse(values[25]) : 0,
                                FechaPrimerVencimiento = Helper.Helper.GetDateTimeFormatDate(values[26]),
                                FechaUltimoVencimiento = Helper.Helper.GetDateTimeFormatDate(values[27]),
                                Confirmada = values[28],
                                ModoProceso = values[29],
                                NroTrxPos = values[30] != "" ? int.Parse(values[30]) : 0,
                                NroRef = values[31],
                                NroCaja = values[32] != "" ? int.Parse(values[32]) : 0,
                            });
                        }
                    }

                }
                #endregion
                #region INTERFAZ COPA_CAPA
                else
                {
                    var TipotransaccionCAPA = _db.TipoTransaccionCapas.ToList();
                    nColumnas = 16;
                    for (int i = 0; i <= archivo.Count() - 1; i++)
                    {
                        var values = archivo[i].Split('|');
                        var prueba = Helper.Helper.GetCodigoTransaccion(values[12]);

                        documentType.ListaInterfazPago.Add(new InterfazPago()
                        {
                            //IdInterfaces = documentType.OInterfaz.Id,
                            //Id = Guid.NewGuid(),
                            CodigoEmpresa = values[0],
                            FechaAutorizacion = Helper.Helper.GetDateTimeFormatWhitTime(values[1]),
                            CodigoAgencia = values[2],
                            NumeroTerminal = values[3] != "" ? Int32.Parse(values[3]) : 0,
                            CodigoUsuario = values[4],
                            NumeroAutorizacion = (values[5]),
                            Numerorecibo = values[6],
                            TipoArchivo = values[7],
                            CodigoCliente = values[8],
                            Numerocuenta = values[9],
                            Monto = values[10] != "" ? Int32.Parse(values[10]) : 0,
                            CodigoMedioPago = values[11] != "" ? Int32.Parse(values[11]) : 0,
                            CodigoTransaccion = TipotransaccionCAPA.Where(x => x.Nombre.Equals(values[12])).Select(x=>x.Codigo).SingleOrDefault(),
                            NroTrxPos = values[13] != "" ? Int32.Parse(values[13]) : 0,
                            NroRef = values[14],
                            NroCaja = values[15] != "" ? Int32.Parse(values[15]) : 0,
                        });
                    }
                }
                #endregion


                return documentType;
                //Console.WriteLine("Llegando");
            }
            catch (Exception ex)
            {

                var prueba = ex;
                throw;

            }
        }

        public ResponseModel SaveInterfaz(InterfacesView documentType)
        {
            ResponseModel response = new ResponseModel();

            //var Autorizaciones = db.InterfazAutorizacions.
            //Select(x => new { x.NumeroAutorizacion, x.FechaAutoriza, x.CodigoAgencia, x.NroCaja }).ToList();

            //var ListaAutorizacione = doucmentType.ListaInterfazAuto.
            //Select(x => new { x.NumeroAutorizacion, x.FechaAutoriza, x.CodigoAgencia, x.NroCaja }).ToList();

            //var compara = ListaAutorizacione.Except(Autorizaciones).ToList();
            //var Autorizaciones = db.InterfazAutorizacions.;
            List<InterfazAutorizacion> compara = new List<InterfazAutorizacion>();
            List<InterfazPago> comparaPagos = new List<InterfazPago>();

            //foreach (var item in doucmentType.ListaInterfazAuto.ToList())
            //{
            //    var CAAU = db.InterfazAutorizacions.Where(x => x.NumeroAutorizacion == item.NumeroAutorizacion
            //    && x.FechaAutorizacion == item.FechaAutorizacion && x.CodigoAgencia == item.CodigoAgencia
            //    && x.NroCaja == item.NroCaja).Any();
            //    if (CAAU == false)
            //    {
            //        compara.Add(item);
            //    }

            //}
            //foreach (var item in doucmentType.ListaInterfazPago.ToList())
            //{
            //    var CAPA = db.InterfazPagos.Where(x => x.NumeroAutorizacion == item.NumeroAutorizacion
            //    && x.FechaAutorizacion == item.FechaAutorizacion && x.CodigoAgencia == item.CodigoAgencia
            //    && x.NroCaja == item.NroCaja).Any();
            //    if (CAPA == false)
            //    {
            //        comparaPagos.Add(item);
            //    }

            //}

            //compara = documentType.ListaInterfazAuto
            //            .Where(item => !db.InterfazAutorizacions
            //                .Any(x => x.NumeroAutorizacion == item.NumeroAutorizacion
            //                    && x.FechaAutorizacion == item.FechaAutorizacion
            //                    && x.CodigoAgencia == item.CodigoAgencia
            //                    && x.NroCaja == item.NroCaja))
            //            .ToList();
            //comparaPagos = documentType.ListaInterfazPago.ToList()
            //                .Where(item => !db.InterfazPagos
            //                    .Any(x => x.NumeroAutorizacion == item.NumeroAutorizacion
            //                        && x.FechaAutorizacion == item.FechaAutorizacion
            //                        && x.CodigoAgencia == item.CodigoAgencia
            //                        && x.NroCaja == item.NroCaja))
            //                .ToList();
            try
            {
                compara = (from item in documentType.ListaInterfazAuto
                           join x in _db.InterfazAutorizacions on new
                           {
                               item.NumeroAutorizacion,
                               item.FechaAutorizacion,
                               item.CodigoAgencia,
                               item.NroCaja
                           } equals new
                           {
                               x.NumeroAutorizacion,
                               x.FechaAutorizacion,
                               x.CodigoAgencia,
                               x.NroCaja
                           } into temp
                           where !temp.Any()
                           select item).ToList();

               
             comparaPagos = (from item in documentType.ListaInterfazPago
                                join x in _db.InterfazPagos on new
                                {
                                    item.NumeroAutorizacion,
                                    item.FechaAutorizacion,
                                    item.CodigoAgencia,
                                    item.NroCaja
                                } equals new
                                {
                                    x.NumeroAutorizacion,
                                    x.FechaAutorizacion,
                                    x.CodigoAgencia,
                                    x.NroCaja
                                } into temp
                                where !temp.Any()
                                select item).ToList();

              

          
                using (var context = _db.Database.BeginTransaction())
                {
                    //_db.Interfaces.Add(doucmentType.OInterfaz);
                    //if (doucmentType.OInterfaz.Idtipo == _db.TipoInterfazs.Where(x => x.Nombre == "CAAU").Select(x => x.IdTipo).SingleOrDefault())
                    if (documentType.Tipo == "CAAU")
                    {
                        _db.InterfazAutorizacions.AddRange(compara);
                    }
                    else
                    {
                        _db.InterfazPagos.AddRange(comparaPagos);
                    }
                    _db.SaveChanges();
                    context.Commit();
                }

                response.error = false;
                response.respuesta = "Se almaceno correctamente";
                return response;
            }
            catch (Exception ex)
            {

                var Error = ex;
                response.error = true;
                response.respuesta = "Error al almacenar" + ex.Message;
                return response;
            }


     
        }
    }
}
