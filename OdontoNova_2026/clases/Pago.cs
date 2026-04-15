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
    public class Pago
    {
        public int IdPago { get; set; }
        public DateTime FechaPago { get; set; }
        public decimal Monto { get; set; }

        public static decimal RegistrarPagoCita(int idCita, decimal monto, decimal montoExtra)
        {
            using (SqlConnection con = Conexion.cadena())
            {
                SqlCommand cmd = new SqlCommand("spRegistrarPagoCita", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IdCita", idCita);
                cmd.Parameters.AddWithValue("@Monto", monto);
                cmd.Parameters.AddWithValue("@MontoExtra", montoExtra);

                object resultado = cmd.ExecuteScalar();

                return resultado != null ? Convert.ToDecimal(resultado) : 0;
            }
        }


    }
}
