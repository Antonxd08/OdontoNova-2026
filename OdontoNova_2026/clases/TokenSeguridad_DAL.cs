using OdontoNova.Clases;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace OdontoNova
{
    static class TokenSeguridad_DAL
    {
        //36 CARACTERES
        //public static string GenerarToken()
        //{
        //    return Guid.NewGuid().ToString();
        //}

        public static string GenerarToken()
        {
            return GenerarCodigo(3) + "-" + GenerarCodigo(3);
        }

        private static string GenerarCodigo(int longitud)
        {
            const string caracteres = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            StringBuilder token = new StringBuilder(); //para almancear cada caracter
            byte[] bytes = new byte[longitud];

            using (RandomNumberGenerator random = RandomNumberGenerator.Create()) //crear el generador
            {
                random.GetBytes(bytes); //llenar el arreglo de bytes
            }

            for (int i = 0; i < longitud; i++)
            {
                //toma el numero del byte para convertirlo en un numero dentro de la posicion de caracteres permitidos
                int index = bytes[i] % caracteres.Length;
                token.Append(caracteres[index]); //añadirlo al string de token
            }

            return token.ToString();
        }

        public static int ObtenerIdUsuarioPorCorreo(string correo)
        {
            try
            {
                using (SqlConnection conn = Conexion.cadena())
                {
                    string query = "SELECT IdUsuario FROM Usuarios WHERE Correo = @Correo";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Correo", correo.Trim());

                        object result = cmd.ExecuteScalar();

                        if (result == null || result == DBNull.Value)
                        {
                            MessageBox.Show("El correo no existe en la base de datos.",
                                            "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return -1; // 🔹 Devuelve -1 para indicar que no se encontró el usuario
                        }

                        return Convert.ToInt32(result);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al obtener el ID del usuario: " + ex.Message,
                                "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return -1;
            }
        }


        public static void GuardarToken(string correo, string token)
        {
            using (SqlConnection conn = Conexion.cadena())
            {
                int idUsuario = ObtenerIdUsuarioPorCorreo(correo);

                //expirar todos los tokens previos o no usados
                string actualizar = @"UPDATE TokenSeguridad SET Usado = 1 WHERE IdUsuario = @IdUsuario AND Usado = 0";
                SqlCommand cmdAct = new SqlCommand(actualizar, conn);
                cmdAct.Parameters.AddWithValue("@IdUsuario", idUsuario);
                cmdAct.ExecuteNonQuery();

                string query = @"INSERT INTO TokenSeguridad (IdUsuario, Token, FechaCreacion, FechaExpiracion, Usado) VALUES (@IdUsuario, @Token, GETDATE(), DATEADD(MINUTE, 5, GETDATE()), 0)";
                SqlCommand cmdIn = new SqlCommand(query, conn);
                cmdIn.Parameters.AddWithValue("@IdUsuario", idUsuario);
                cmdIn.Parameters.AddWithValue("@Token", token);
                cmdIn.ExecuteNonQuery();
            }
        }

        public static bool ValidarToken(string correo, string token)
        {
            using (SqlConnection conn = Conexion.cadena())
            {
                int idUsuario = ObtenerIdUsuarioPorCorreo(correo);

                //los tokens usados deben estar marcados ocmo expirados
                string expirar = @"UPDATE TokenSeguridad SET Usado = 1 WHERE FechaExpiracion <= GETDATE() AND Usado = 0";
                SqlCommand cmdEx = new SqlCommand(expirar, conn);
                cmdEx.ExecuteNonQuery();

                string query = @"SELECT COUNT(*) FROM TokenSeguridad WHERE IdUsuario = @IdUsuario AND Token = @Token AND Usado = 0 AND FechaExpiracion > GETDATE()";
                SqlCommand cmdSe = new SqlCommand(query, conn);
                cmdSe.Parameters.AddWithValue("@IdUsuario", idUsuario);
                cmdSe.Parameters.AddWithValue("@Token", token);

                int conteo = (int)cmdSe.ExecuteScalar();
                return conteo > 0;
            }
        }

        //cuando ingresa el token correcto
        public static void MarcarTokenUsado(string correo, string token)
        {
            using (SqlConnection conn = Conexion.cadena())
            {
                int idUsuario = ObtenerIdUsuarioPorCorreo(correo);
                string query = @"UPDATE TokenSeguridad SET Usado = 1 WHERE Token = @Token AND IdUsuario = @IdUsuario";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@IdUsuario", idUsuario);
                cmd.Parameters.AddWithValue("@Token", token);
                cmd.ExecuteNonQuery();
            }
        }

        //expirarlo por tiempo
        public static void ExpirarToken(string correo)
        {
            using (SqlConnection conn = Conexion.cadena())
            {
                int idUsuario = ObtenerIdUsuarioPorCorreo(correo);
                string query = @"UPDATE TokenSeguridad SET Usado = 1 WHERE IdUsuario = @IdUsuario";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@IdUsuario", idUsuario);
                cmd.ExecuteNonQuery();
            }
        }
    }
}
