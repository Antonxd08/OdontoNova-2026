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
    public class HistorialDental
    {
        public int IdHistorial { get; set; }
        public int IdPaciente { get; set; }
        public int IdCita { get; set; }
        public DateTime FechaRegistro { get; set; }
        public string EstadoCita { get; set; }
        public string TipoTratamiento { get; set; }
        public decimal MontoPagado { get; set; }

        public decimal MontoExtraPagado { get; set; }

        public static List<HistorialDental> ObtenerHistorialPorPaciente(int idPaciente)
        {
            List<HistorialDental> historial = new List<HistorialDental>();

            using (SqlConnection con = Conexion.cadena())
            {
                SqlCommand cmd = new SqlCommand("spObtenerHistorialPorPaciente", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdPaciente", idPaciente);

                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    historial.Add(new HistorialDental
                    {
                        IdHistorial = reader.GetInt32(0),
                        IdCita = reader.GetInt32(1),
                        FechaRegistro = reader.GetDateTime(2),
                        EstadoCita = reader.GetString(3),
                        TipoTratamiento = reader.GetString(4),
                        MontoPagado = reader.GetDecimal(5),
                        MontoExtraPagado = reader.GetDecimal(6)
                    });
                }
            }

            return historial;
        }

    }


}
