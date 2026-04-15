using OdontoNova.Clases;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OdontoNova
{
    public class Expedientes
    {
        public int IdExpediente { get; set; }
        public int IdPaciente { get; set; }
        public DateTime FechaApertura { get; set; }
        public decimal costoTratamiento { get; set; }
        public string EstadoPago { get; set; }

        //Lista que mostrara los resultados del procedimiento de bsuqueda de pacientes por filtro
        public List<(Paciente paciente, Expedientes expediente)> BuscarExpedientes(string nombre, string apellido)
        {
            List<(Paciente, Expedientes)> resultados = new List<(Paciente, Expedientes)>();

            using (SqlConnection con = Conexion.cadena())
            {
                SqlCommand cmd = new SqlCommand("spBuscarExpediente", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@nombre", (object)nombre ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@apellido", (object)apellido ?? DBNull.Value);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Paciente paciente = new Paciente
                        {
                            IdPaciente = reader.GetInt32(0),
                            Nombre = reader.GetString(1),
                            Apellido = reader.GetString(2),
                            Telefono = reader.GetString(3),
                            Genero = reader.GetString(4),
                            FechaNacimiento = reader.GetDateTime(5),
                            Direccion = reader.GetString(6),
                            AntecedentesMedicos = reader.GetString(7),
                            EstadoPaciente = reader.GetBoolean(8),
                            FotoPaciente = reader.IsDBNull(9) ? null : (byte[])reader["FotoPaciente"]

                        };

                        Expedientes expediente = new Expedientes
                        {
                            IdExpediente = reader.GetInt32(10),
                            IdPaciente = paciente.IdPaciente,
                            FechaApertura = reader.GetDateTime(11),
                            costoTratamiento = reader.GetDecimal(12),
                            EstadoPago = reader.GetString(13)
                        };

                        resultados.Add((paciente, expediente));
                    }
                }
            }

            return resultados;
        }

        //Lista para obtener sugerencias para la busqueda de pacientes
        public List<string> ObtenerSugerencias(string filtro, string texto)
        {
            List<string> resultados = new List<string>();

            using (SqlConnection con = Conexion.cadena())
            {
                string campo = filtro == "Nombre" ? "Nombre" : "Apellido";
                string query = $"SELECT DISTINCT {campo} FROM Pacientes WHERE {campo} LIKE @texto + '%' ORDER BY {campo}";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@texto", texto);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        resultados.Add(reader.GetString(0));
                    }
                }
            }

            return resultados;
        }
    }
}
