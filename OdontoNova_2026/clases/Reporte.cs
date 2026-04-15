using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OdontoNova.Clases
{
    public class Reporte
    {
        public DateTime FechaGeneracion { get; set; }
        public DateTime PeriodoInicio { get; set; }
        public DateTime PeriodoFin { get; set; }
        public int IdUsuario { get; set; }
        public int CitasAsistidas { get; set; }
        public int CitasNoAsistidas { get; set; }
        public decimal TotalPagos { get; set; }
        public string DiasMayorDemanda { get; set; }


        public Reporte GenerarReporte(DateTime inicio, DateTime fin, int idUsuario)
        {
            using (SqlConnection con = Conexion.cadena())
            {
                SqlCommand cmd = new SqlCommand("spGenerarReportePorRango", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@FechaInicio", inicio);
                cmd.Parameters.AddWithValue("@FechaFin", fin);
                cmd.Parameters.AddWithValue("@IdUsuario", idUsuario);

               
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    return new Reporte
                    {
                        FechaGeneracion = reader.GetDateTime(0),
                        PeriodoInicio = reader.GetDateTime(1),
                        PeriodoFin = reader.GetDateTime(2),
                        IdUsuario = reader.GetInt32(3),
                        CitasAsistidas = reader.GetInt32(4),
                        CitasNoAsistidas = reader.GetInt32(5),
                        TotalPagos = reader.GetDecimal(6),
                        DiasMayorDemanda = reader.GetString(7)
                    };
                }
            }

            return null;
        }

    }

}


