using Microsoft.Data.SqlClient;

namespace ServicioApi.Model.Clases
{
    public class SobranteFaltante
    {
        public ResponseModel CargarSobranteFaltanteGlobal(DateTime Fecha) 
        { 
            ResponseModel responseModel = new ResponseModel();

            try
            {
                //string conexion = Helper.Helper.ConexionGlobalModel();
                string conexion = "data source=172.18.14.29;initial catalog=GlobalSTORE;user id=sa;password=SoporteTI5951;MultipleActiveResultSets=True;App=EntityFramework;";

                var prueba = Fecha.ToString("dd/MM/yyyy").Replace("-","/");

                using (SqlConnection con = new SqlConnection(conexion))
                {
                    con.Open();

                    using (SqlCommand cmd = new SqlCommand("sp_FP_Sobrantes_Faltantes",con))
                    {
                        cmd.CommandTimeout = 500;
                        cmd.CommandText = $@"exec dbo.sp_FP_Sobrantes_Faltantes @FECHA='{prueba}'";
                        cmd.ExecuteNonQuery();

                    }
                    con.Close();

                }
            }
            catch (Exception)
            {

                throw;
            }

            return responseModel;
            
        }
    }
}
