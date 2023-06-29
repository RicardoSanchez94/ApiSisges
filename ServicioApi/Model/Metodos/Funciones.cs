using Mysqlx.Cursor;
using Newtonsoft.Json;
using ServicioApi.Model.Clases;
using ServicioApi.Modelview;
using System.Data;
using System.Data.SqlClient;

namespace ServicioApi.Model.Metodos
{
    public class Funciones
    {
        #region Funciones
        public string Funcion(string funcion, List<Parametros> Parametros)
        {
            string Json = string.Empty;
            DataTable dt = new DataTable();
            string con = Helper.Helper.ConexionStringModel();
            //string format = "Select * from" + funcion  ;

            using (SqlConnection conn = new SqlConnection(con))
            {
                conn.Open();
                //_logger.LogInformation("Conexion con Servidores: " + item.ServidorIP);
                using (SqlCommand cmd = new SqlCommand(funcion, conn))
                {
                    // cmd.CommandText = "Select * from fn_NCFC (2'20220927','SAPFSP')";
                    foreach (var item in Parametros)
                    {
                        cmd.Parameters.AddWithValue(item.Name, item.Value);
                    }
                    var reader = cmd.ExecuteReader();

                    //var r = Serializer(reader.Read());
                    dt = new DataTable();
                    dt.Load(reader);
                    Json = JsonConvert.SerializeObject(dt);

                    //list2.AddRange(JsonConvert.DeserializeObject<List<Trx>>(Json));

                    //while (reader.Read())
                    //{
                    //    list2.Add(new Trx()
                    //    {
                    //        Id = Convert.ToInt32(reader["TRAN_ID"].ToString()),
                    //        Fecha = DateTime.Parse(reader["TRAN_STRT_TS"].ToString())

                    //    });

                    //}
                    conn.Close();
                }
            }
            return Json;
        }
        public string IdocFi(string fecha, int tienda)
        {
            string Json = string.Empty;
            DataTable dt = new DataTable();
            string con = Helper.Helper.ConexionStringModel();
            string format = "SELECT * FROM fnIdoc_FI (@Fecha, @IdTienda)";

            using (SqlConnection conn = new SqlConnection(con))
            {
                conn.Open();
                //_logger.LogInformation("Conexion con Servidores: " + item.ServidorIP);
                using (SqlCommand cmd = new SqlCommand(format, conn))
                {
                    // cmd.CommandText = "Select * from fn_NCFC (2'20220927','SAPFSP')";

                    cmd.Parameters.AddWithValue("@Fecha", fecha);
                    cmd.Parameters.AddWithValue("@IdTienda", tienda);

                    var reader = cmd.ExecuteReader();

                    //var r = Serializer(reader.Read());
                    dt = new DataTable();
                    dt.Load(reader);
                    Json = JsonConvert.SerializeObject(dt);

                    //list2.AddRange(JsonConvert.DeserializeObject<List<Trx>>(Json));

                    //while (reader.Read())
                    //{
                    //    list2.Add(new Trx()
                    //    {
                    //        Id = Convert.ToInt32(reader["TRAN_ID"].ToString()),
                    //        Fecha = DateTime.Parse(reader["TRAN_STRT_TS"].ToString())

                    //    });

                    //}
                    conn.Close();
                }
            }
            return Json;
        }
        //public List<IdocVentasRP> fn_IdocVentasRP(string fecha, int tienda,string ambiente)
        //{
        //    string Json = string.Empty;
        //    DataTable dt = new DataTable();
        //    string con = Helper.Helper.ConexionStringModel();
        //    List<IdocVentasRP> lst = new List<IdocVentasRP>();
        //    string format = "SELECT * FROM fn_IdocVentasRP (@I_Tienda,@C_Fecha,@Ambiente)";

        //    using (SqlConnection conn = new SqlConnection(con))
        //    {
        //        conn.Open();
        //        //_logger.LogInformation("Conexion con Servidores: " + item.ServidorIP);
        //        using (SqlCommand cmd = new SqlCommand(format, conn))
        //        {
        //            // cmd.CommandText = "Select * from fn_NCFC (2'20220927','SAPFSP')";

        //            cmd.Parameters.AddWithValue("@C_Fecha", fecha);
        //            cmd.Parameters.AddWithValue("@I_Tienda", tienda);
        //            cmd.Parameters.AddWithValue("@Ambiente", ambiente);

        //            var reader = cmd.ExecuteReader();

        //            //var r = Serializer(reader.Read());
        //            dt = new DataTable();
        //            dt.Load(reader);
        //            Json = JsonConvert.SerializeObject(dt);

        //            lst.AddRange(JsonConvert.DeserializeObject<List<IdocVentasRP>>(Json));

        //            //while (reader.Read())
        //            //{
        //            //    list2.Add(new Trx()
        //            //    {
        //            //        Id = Convert.ToInt32(reader["TRAN_ID"].ToString()),
        //            //        Fecha = DateTime.Parse(reader["TRAN_STRT_TS"].ToString())

        //            //    });

        //            //}
        //            conn.Close();
        //        }
        //    }
        //    return lst;
        //}

        //public List<IdocNCFC_RP> fn_IdocNCFC_RP(string fecha, int tienda, string ambiente)
        //{
        //    string Json = string.Empty;
        //    DataTable dt = new DataTable();
        //    string con = Helper.Helper.ConexionStringModel();
        //    List<IdocNCFC_RP> lst = new List<IdocNCFC_RP>();
        //    string format = "SELECT * FROM fn_IdocNCFC_RP (@I_Tienda,@C_Fecha,@Ambiente)";

        //    using (SqlConnection conn = new SqlConnection(con))
        //    {
        //        conn.Open();
        //        //_logger.LogInformation("Conexion con Servidores: " + item.ServidorIP);
        //        using (SqlCommand cmd = new SqlCommand(format, conn))
        //        {
        //            // cmd.CommandText = "Select * from fn_NCFC (2'20220927','SAPFSP')";

        //            cmd.Parameters.AddWithValue("@C_Fecha", fecha);
        //            cmd.Parameters.AddWithValue("@I_Tienda", tienda);
        //            cmd.Parameters.AddWithValue("@Ambiente", ambiente);

        //            var reader = cmd.ExecuteReader();

        //            //var r = Serializer(reader.Read());
        //            dt = new DataTable();
        //            dt.Load(reader);
        //            Json = JsonConvert.SerializeObject(dt);

        //            lst.AddRange(JsonConvert.DeserializeObject<List<IdocNCFC_RP>>(Json));

        //            //while (reader.Read())
        //            //{
        //            //    list2.Add(new Trx()
        //            //    {
        //            //        Id = Convert.ToInt32(reader["TRAN_ID"].ToString()),
        //            //        Fecha = DateTime.Parse(reader["TRAN_STRT_TS"].ToString())

        //            //    });

        //            //}
        //            conn.Close();
        //        }
        //    }
        //    return lst;
        //}
        //public List<FormatoIDOC> fn_IdocClientes_RP(string fecha, int tienda, string ambiente)
        //{
        //    string Json = string.Empty;
        //    DataTable dt = new DataTable();
        //    string con = Helper.Helper.ConexionStringModel();
        //    List<FormatoIDOC> lst = new List<FormatoIDOC>();
        //    string format = "SELECT * FROM fn_IdocClientes_RP (@local,@Fecha,@Ambiente)";

        //    using (SqlConnection conn = new SqlConnection(con))
        //    {
        //        conn.Open();
        //        //_logger.LogInformation("Conexion con Servidores: " + item.ServidorIP);
        //        using (SqlCommand cmd = new SqlCommand(format, conn))
        //        {
        //            // cmd.CommandText = "Select * from fn_NCFC (2'20220927','SAPFSP')";
        //            cmd.CommandTimeout = 10000;
        //            cmd.Parameters.AddWithValue("@Fecha", fecha);
        //            cmd.Parameters.AddWithValue("@local", tienda);
        //            cmd.Parameters.AddWithValue("@Ambiente", ambiente);

        //            var reader = cmd.ExecuteReader();

        //            //var r = Serializer(reader.Read());
        //            dt = new DataTable();
        //            dt.Load(reader);
        //            Json = JsonConvert.SerializeObject(dt);

        //            lst.AddRange(JsonConvert.DeserializeObject<List<FormatoIDOC>>(Json));

        //            //while (reader.Read())
        //            //{
        //            //    list2.Add(new Trx()
        //            //    {
        //            //        Id = Convert.ToInt32(reader["TRAN_ID"].ToString()),
        //            //        Fecha = DateTime.Parse(reader["TRAN_STRT_TS"].ToString())

        //            //    });

        //            //}
        //            conn.Close();
        //        }
        //    }
        //    return lst;
        //}

        public List<FormatoIDOC> fn_IdocMPVentas_RP(string fecha, int tienda, string ambiente)
        {
            string Json = string.Empty;
            DataTable dt = new DataTable();
            string con = Helper.Helper.ConexionStringModel();
            List<FormatoIDOC> lst = new List<FormatoIDOC>();
            string format = "SELECT * FROM fn_IdocMPVentas (@IdTienda,@C_Fecha,@Ambiente)";

            using (SqlConnection conn = new SqlConnection(con))
            {
                conn.Open();
                //_logger.LogInformation("Conexion con Servidores: " + item.ServidorIP);
                using (SqlCommand cmd = new SqlCommand(format, conn))
                {
                    // cmd.CommandText = "Select * from fn_NCFC (2'20220927','SAPFSP')";
                    cmd.CommandTimeout = 10000;
                    cmd.Parameters.AddWithValue("@C_Fecha", fecha);
                    cmd.Parameters.AddWithValue("@IdTienda", tienda);
                    cmd.Parameters.AddWithValue("@Ambiente", ambiente);

                    var reader = cmd.ExecuteReader();

                    //var r = Serializer(reader.Read());
                    dt = new DataTable();
                    dt.Load(reader);
                    Json = JsonConvert.SerializeObject(dt);

                    lst.AddRange(JsonConvert.DeserializeObject<List<FormatoIDOC>>(Json));

                   
                    conn.Close();
                }
            }
            return lst;
        }

        public List<SencillosTesoreria> fnSencillosTiendaNoConciliados()
        {
            string Json = string.Empty;
            DataTable dt = new DataTable();
            string con = Helper.Helper.ConexionStringModel();
            List<SencillosTesoreria> lst = new List<SencillosTesoreria>();
            string format = "SELECT * FROM fnSencillosTiendaNoConciliados ()";

            using (SqlConnection conn = new SqlConnection(con))
            {
                conn.Open();
                //_logger.LogInformation("Conexion con Servidores: " + item.ServidorIP);
                using (SqlCommand cmd = new SqlCommand(format, conn))
                {
                    //cmd.Parameters.AddWithValue("@fechaInicio", Inicio);
                    //cmd.Parameters.AddWithValue("@fechaFin", Fin);
                  

                    var reader = cmd.ExecuteReader();

                    //var r = Serializer(reader.Read());
                    dt = new DataTable();
                    dt.Load(reader);
                    Json = JsonConvert.SerializeObject(dt);

                    lst.AddRange(JsonConvert.DeserializeObject<List<SencillosTesoreria>>(Json));

                    
                    conn.Close();
                }
            }
            return lst;
        }
        #endregion

        #region StoreProcedure

        public async Task<ResponseModel> spConciliacionAutomaticaCAAU (DateTime fecha)
        {
            ResponseModel response = new ResponseModel();
            try
            {
                string con = Helper.Helper.ConexionStringModel();
                using (SqlConnection conn = new SqlConnection(con))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("spConciliacionAutomaticaCAAU",conn))
                    {
                        cmd.CommandTimeout = 0;
                        cmd.CommandText = $@"exec dbo.spConciliacionAutomaticaCAAU @fecha='{fecha}'";
                        await cmd.ExecuteReaderAsync();
                    }
                }
                response.error = false;
                response.respuesta = "Se Ejecuto Correctamente la Conciliacion Automatica de CAAU";
            }
            catch (Exception ex )
            {

               response.error = true;
               response.respuesta = "Error al Ejecutar La Conciliacion Automatica de CAAU " + ex.Message;
            }

            return response;
        }


        public async Task<ResponseModel> Proc_GENERAR_DEPOSITOS(DateTime c_Fecha)
        {
            ResponseModel response = new ResponseModel();
            try
            {
                
                string con = Helper.Helper.ConexionStringModel();
                using (SqlConnection conn = new SqlConnection(con))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("Proc_GENERAR_DEPOSITOS", conn))
                    {

                        cmd.CommandTimeout = 500;
                        cmd.CommandText = $@"exec dbo.Proc_GENERAR_DEPOSITOS @P_Fecha='{c_Fecha}'";
                        await cmd.ExecuteReaderAsync();

                        //parametros.SqlDbType = SqlDbType.Structured;

                    }
                    conn.Close();

                }

                response.error = false;
                response.mensaje = "Se ejecuto Correctamente ";
                return response;
            }
            catch (Exception ex)
            {

                response.error = true;
                response.mensaje = "Error en la Ejecucion" + ex.InnerException;
                return response;
            }
         


        }

        public async Task<ResponseModel> Proc_SWITCH(DateTime c_Fecha)
        {
            ResponseModel response = new ResponseModel();
            try
            {

                string con = Helper.Helper.ConexionStringModel();
                using (SqlConnection conn = new SqlConnection(con))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("Proc_SWITCH", conn))
                    {

                        cmd.CommandTimeout = 1000;
                        cmd.CommandText = $@"exec dbo.Proc_SWITCH @C_Fecha='{c_Fecha}'";
                        await cmd.ExecuteReaderAsync();

                        //parametros.SqlDbType = SqlDbType.Structured;

                    }
                    conn.Close();

                }

                response.error = false;
                response.mensaje = "Se ejecuto Correctamente ";
                return response;
            }
            catch (Exception ex)
            {

                response.error = true;
                response.mensaje = "Error en la Ejecucion" + ex.InnerException;
                return response;
            }



        }

        public async Task<ResponseModel> Proc_GENERAR_RENDICIONES(DateTime c_Fecha)
        {
            ResponseModel response = new ResponseModel();
            try
            {

                string con = Helper.Helper.ConexionStringModel();
                using (SqlConnection conn = new SqlConnection(con))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("Proc_GENERAR_RENDICIONES", conn))
                    {

                        cmd.CommandTimeout = 500;
                        cmd.CommandText = $@"exec dbo.Proc_GENERAR_RENDICIONES @P_Fecha='{c_Fecha}'";
                       await cmd.ExecuteReaderAsync();

                        //parametros.SqlDbType = SqlDbType.Structured;

                    }
                    conn.Close();

                }

                response.error = false;
                response.mensaje = "Se ejecuto Correctamente ";
                return response;
            }
            catch (Exception ex)
            {

                response.error = true;
                response.mensaje = "Error en la Ejecucion" + ex.InnerException;
                return response;
            }



        }

        public ResponseModel IdocFiVentas(string fecha, int tienda)
        {
            ResponseModel response = new ResponseModel();
            string con = Helper.Helper.ConexionStringModel();
            string format = "exec Proc_GenIdocFI (@Fecha, @IdTienda)";
            try
            {
                using (SqlConnection conn = new SqlConnection(con))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(format, conn))
                    {
                        cmd.CommandTimeout = 500;
                        cmd.CommandText = $@"exec dbo.Proc_GenIdocFI @Fecha='{fecha}', @IdTienda={tienda}";
                        cmd.ExecuteReader();

                        conn.Close();
                        response.error = false;
                        response.mensaje = "se ejecuto correctamente el sp";
                    }
                }
                return response;
            }
            catch (Exception ex)
            {
                response.error = true;
                response.respuesta = "Se ha producido un error " + ex.Message;
                return response;
            }
            
        }
        public ResponseModel IdocFiPagos(string fecha, int tienda)
        {
            ResponseModel response = new ResponseModel();
            string con = Helper.Helper.ConexionStringModel();
            string format = "exec Proc_GenIdocPagosFI (@Fecha, @IdTienda)";
            try
            {
                using (SqlConnection conn = new SqlConnection(con))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(format, conn))
                    {
                        cmd.CommandTimeout = 500;
                        cmd.CommandText = $@"exec dbo.Proc_GenIdocPagosFI @Fecha='{fecha}', @IdTienda={tienda}";
                        cmd.ExecuteReader();

                        conn.Close();
                        response.error = false;
                        response.mensaje = "se ejecuto correctamente el sp";
                    }
                }
                return response;
            }
            catch (Exception ex)
            {
                response.error = true;
                response.respuesta = "Se ha producido un error " + ex.Message;
                return response;
            }

        }

        #endregion

    }
}
