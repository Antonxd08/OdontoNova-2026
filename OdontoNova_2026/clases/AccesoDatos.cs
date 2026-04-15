using OdontoNova.Clases;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using BCrypt.Net;

namespace OdontoNova
{
    public class AccesoDatos
    {
        //Consulta para buscar usuario segun su nombre y contrasena
        public Usuario ObtenerUsuario(string nombreUsuario, string contraseña)
        {
            using (SqlConnection con = Conexion.cadena())
            {
                string query = "SELECT IdUsuario, NombreUsuario, Contraseña, Rol, Correo, FotoUsuario FROM Usuarios WHERE NombreUsuario = @nombre";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@nombre", nombreUsuario);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        string hash = reader.GetString(2); // Contraseña hasheada

                        // Verificar la contraseña ingresada con el hash
                        if (BCrypt.Net.BCrypt.Verify(contraseña, hash))
                        {
                            return new Usuario
                            {
                                IdUsuario = reader.GetInt32(0),
                                NombreUsuario = reader.GetString(1),
                                Contraseña = hash,
                                Rol = reader.GetString(3),
                                Correo = reader.GetString(4),
                                FotoUsuario = reader.IsDBNull(5) ? null : (byte[])reader["FotoUsuario"]
                            };
                        }
                        else
                        {
                            MessageBox.Show("Contraseña incorrecta.", "Error de autenticación", MessageBoxButton.OK, MessageBoxImage.Error);
                            return null;
                        }
                    }
                }
            }
            return null; // Usuario no encontrado
        }


        public Usuario ObtenerUsuarioPorNombre(string nombre)
        {

            using (SqlConnection conn = Conexion.cadena())
            {
                string query = @"SELECT IdUsuario, NombreUsuario, Contraseña, Rol, Correo, FotoUsuario FROM Usuarios WHERE NombreUsuario = @nombre";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Nombre", nombre);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new Usuario
                        {
                            IdUsuario = reader.GetInt32(0),
                            NombreUsuario = reader.GetString(1),
                            Contraseña = reader.GetString(2),
                            Rol = reader.GetString(3),
                            Correo = reader.GetString(4),
                            FotoUsuario = reader.IsDBNull(5) ? null : (byte[])reader["FotoUsuario"]
                        };
                    }
                }
            }
            return null;
        }

        public static bool ActualizarContraseña(int idUsuario, string nuevaContraseña)
        {
            try
            {
                using (SqlConnection conn = Conexion.cadena())
                {
                    string hash = BCrypt.Net.BCrypt.HashPassword(nuevaContraseña);

                    string query = @"UPDATE Usuarios 
                             SET Contraseña = @Hash 
                             WHERE IdUsuario = @IdUsuario";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Hash", hash);
                    cmd.Parameters.AddWithValue("@IdUsuario", idUsuario);

                    int filas = cmd.ExecuteNonQuery();
                    return filas > 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al actualizar contraseña: " + ex.Message);
                return false;
            }
        }

        public static bool ActualizarUsuario(int idUsuario, string nuevoUsuario)
        {
            try
            {
                using (SqlConnection conn = Conexion.cadena())
                {
                    string query = @"UPDATE Usuarios 
                             SET NombreUsuario = @NuevoUsuario 
                             WHERE IdUsuario = @IdUsuario";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@NuevoUsuario", nuevoUsuario);
                    cmd.Parameters.AddWithValue("@IdUsuario", idUsuario);

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al actualizar usuario: " + ex.Message);
                return false;
            }
        }

        public static bool ActualizarCorreo(int idUsuario, string nuevoCorreo)
        {
            try
            {
                using (SqlConnection conn = Conexion.cadena())
                {
                    string query = @"UPDATE Usuarios 
                             SET Correo = @NuevoCorreo 
                             WHERE IdUsuario = @IdUsuario";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@NuevoCorreo", nuevoCorreo);
                    cmd.Parameters.AddWithValue("@IdUsuario", idUsuario);

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al actualizar correo: " + ex.Message);
                return false;
            }
        }

        public static bool ActualizarFotoUsuario(int idUsuario, byte[] nuevaFoto)
        {
            try
            {
                using (SqlConnection conn = Conexion.cadena())
                {
                    string query = @"UPDATE Usuarios 
                             SET FotoUsuario = @Foto 
                             WHERE IdUsuario = @IdUsuario";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Foto", nuevaFoto ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@IdUsuario", idUsuario);

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al actualizar foto de usuario: " + ex.Message);
                return false;
            }
        }

        public static bool EliminarFotoUsuario(int idUsuario)
        {
            try
            {
                using (SqlConnection conn = Conexion.cadena())
                {
                    string query = @"UPDATE Usuarios 
                             SET FotoUsuario = NULL 
                             WHERE IdUsuario = @IdUsuario";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@IdUsuario", idUsuario);

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al eliminar foto de usuario: " + ex.Message);
                return false;
            }
        }


    }
}
