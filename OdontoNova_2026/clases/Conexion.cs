using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace OdontoNova.Clases
{
    public class Conexion
    {
        private static readonly string connectionString =
            @"Data Source=tcp:sqlserver--servidor.database.windows.net;Initial Catalog=OdontoNova_Plus;User ID=Anton;Password=@Gabo1234";

        public static SqlConnection cadena()
        {
            SqlConnection con = new SqlConnection(connectionString);

            try
            {
                if (con.State == System.Data.ConnectionState.Closed)
                {
                    con.Open();
                }

                return con;
            }
            catch (SqlException ex)
            {
                // Error de SQL Server
                throw new Exception($"Error SQL al conectar: {ex.Message}", ex);
            }
            catch (InvalidOperationException ex)
            {
                // Error de estado de la conexin
                throw new Exception($"Error de operación: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                // Error general
                throw new Exception($"Error general de conexión: {ex.Message}", ex);
            }
        }

        public static bool ProbarConexion()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    return con.State == System.Data.ConnectionState.Open;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}
