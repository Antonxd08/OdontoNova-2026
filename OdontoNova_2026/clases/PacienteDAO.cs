using OdontoNova.Clases;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace OdontoNova
{
    public class PacienteDAO
    {
        //procedimiento para registrar al paciente con sus datos 
        public bool RegistrarPaciente(Paciente paciente)
        {
            try
            {
                using (SqlConnection con = Conexion.cadena())
                {
                    if (con == null)
                    {
                        MessageBox.Show("No se pudo establecer conexión con la base de datos.",
                                        "Error de conexión", MessageBoxButton.OK, MessageBoxImage.Error);
                        return false;
                    }

                    using (SqlCommand cmd = new SqlCommand("spRegistrarPaciente", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@Nombre", paciente.Nombre);
                        cmd.Parameters.AddWithValue("@Apellido", paciente.Apellido);
                        cmd.Parameters.AddWithValue("@Telefono", paciente.Telefono);
                        cmd.Parameters.AddWithValue("@Genero", paciente.Genero);
                        cmd.Parameters.AddWithValue("@FechaNacimiento", paciente.FechaNacimiento);
                        cmd.Parameters.AddWithValue("@Direccion", paciente.Direccion);
                        cmd.Parameters.AddWithValue("@AntecedentesMedicos", paciente.AntecedentesMedicos);

                        // ✅ FotoPaciente no debe ser null (por tu constraint NOT NULL)
                        if (paciente.FotoPaciente == null || paciente.FotoPaciente.Length == 0)
                        {
                            MessageBox.Show("Debe seleccionar una foto antes de registrar el paciente.",
                                            "Error de validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return false;
                        }
                        cmd.Parameters.AddWithValue("@FotoPaciente", paciente.FotoPaciente);

                        // Ejecutar el SP y capturar errores
                        cmd.ExecuteNonQuery();
                        return true;
                    }
                }
            }
            catch (SqlException ex)
            {
                // Este catch atrapará cualquier RAISERROR del SP
                MessageBox.Show($"Error SQL: {ex.Message}", "Error al registrar paciente",
                                MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            catch (Exception ex)
            {
                // Otros errores generales
                MessageBox.Show($"Error general: {ex.Message}", "Error inesperado",
                                MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }


        

       

        public int ObtenerIdPorNombreCompleto(string nombre, string apellido)
        {
            using (SqlConnection con = Conexion.cadena())
            {
                string query = "SELECT TOP 1 IdPaciente FROM Pacientes WHERE Nombre = @nombre AND Apellido = @apellido";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@nombre", nombre);
                cmd.Parameters.AddWithValue("@apellido", apellido);

                object result = cmd.ExecuteScalar();
                return result != null ? Convert.ToInt32(result) : -1;
            }
        }

        public (string Nombre, string Apellido) ObtenerNombreYApellidoPorId(int idPaciente)
        {
            string nombre = string.Empty;
            string apellido = string.Empty;

            using (SqlConnection con = Conexion.cadena())
            {
                string query = "SELECT Nombre, Apellido FROM Pacientes WHERE IdPaciente = @IdPaciente";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@IdPaciente", idPaciente);

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        nombre = dr["Nombre"]?.ToString();
                        apellido = dr["Apellido"]?.ToString();
                    }
                }
            }

            return (nombre, apellido);
        }



        public bool ActualizarPaciente(Paciente paciente)
        {
            using (SqlConnection con = Conexion.cadena())
            {
                string query = @"UPDATE Pacientes SET 
                            Telefono = @Telefono,
                            Direccion = @Direccion,
                            AntecedentesMedicos = @AntecedentesMedicos,
                            FotoPaciente = @FotoPaciente
                         WHERE IdPaciente = @IdPaciente";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Telefono", paciente.Telefono);
                cmd.Parameters.AddWithValue("@Direccion", paciente.Direccion);
                cmd.Parameters.AddWithValue("@AntecedentesMedicos", paciente.AntecedentesMedicos ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@FotoPaciente", paciente.FotoPaciente ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@IdPaciente", paciente.IdPaciente);

                
                int filas = cmd.ExecuteNonQuery();
                return filas > 0;
            }
        }


    }
}
