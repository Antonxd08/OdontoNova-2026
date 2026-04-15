using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OdontoNova.Clases;

namespace OdontoNova
{
    public class CitasDAO
    {
        public int AgendarCita(Cita cita)
        {
            using (SqlConnection con = Conexion.cadena())
            {
                SqlCommand cmd = new SqlCommand("spAgendarCita", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@idPaciente", cita.IdPaciente);
                cmd.Parameters.AddWithValue("@fechaCita", cita.FechaCita.Date);
                cmd.Parameters.AddWithValue("@horaCita", cita.HoraCita);
                cmd.Parameters.AddWithValue("@turno", cita.Turno);
                cmd.Parameters.AddWithValue("@estado", cita.Estado);
                cmd.Parameters.AddWithValue("@tipoTratamiento", cita.TipoTratamiento);
                cmd.Parameters.AddWithValue("@observaciones", cita.Observaciones ?? "");
                cmd.Parameters.AddWithValue("@dentista", cita.Dentista ?? (object)DBNull.Value);

                SqlParameter resultado = new SqlParameter("@resultado", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                cmd.Parameters.Add(resultado);

                cmd.ExecuteNonQuery();
                return Convert.ToInt32(resultado.Value);
            }
        }

        public DateTime? ObtenerUltimaFechaCita(int idPaciente)
        {
            using (SqlConnection con = Conexion.cadena())
            {
                string query = "SELECT MAX(FechaCita) FROM Citas WHERE IdPaciente = @idPaciente";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@idPaciente", idPaciente);

                object result = cmd.ExecuteScalar();
                return result != DBNull.Value ? Convert.ToDateTime(result) : (DateTime?)null;
            }
        }

        public List<Cita> ObtenerCitasPorFechaYTurno(DateTime fecha, string turno)
        {
            List<Cita> lista = new List<Cita>();
            using (SqlConnection con = Conexion.cadena())
            {
                SqlCommand cmd = new SqlCommand("spObtenerCitasPorFechaYTurno", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@fecha", fecha);
                cmd.Parameters.AddWithValue("@turno", turno ?? (object)DBNull.Value);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Cita cita = new Cita
                        {
                            IdCita = reader.GetInt32(0),
                            IdPaciente = reader.GetInt32(1),
                            FechaCita = reader.GetDateTime(2),
                            HoraCita = reader.GetTimeSpan(3),
                            Turno = reader.GetString(4),
                            Dentista = reader.IsDBNull(5) ? null : reader.GetString(5),
                            Estado = reader.GetString(6),
                            TipoTratamiento = reader.GetString(7),
                            Observaciones = reader.IsDBNull(8) ? null : reader.GetString(8),
                            IdPago = reader.IsDBNull(9) ? (int?)null : reader.GetInt32(9),
                            NombrePaciente = reader.GetString(10)
                        };
                        lista.Add(cita);
                    }
                }
            }
            return lista;
        }

        public static void ActualizarEstadoCitasPorTurno()
        {
            using (SqlConnection con = Conexion.cadena())
            {
                SqlCommand cmd = new SqlCommand("spActualizarEstadoCitasPorTurno", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.ExecuteNonQuery();
            }
        }
        

        public Cita ObtenerCitaPorId(int idCita)
        {
            Cita cita = null;
            using (SqlConnection con = Conexion.cadena())
            {
                string query = @"SELECT IdCita, IdPaciente, FechaCita, HoraCita, Turno, Estado, TipoTratamiento, Observaciones, Dentista
                             FROM Citas
                             WHERE IdCita = @IdCita";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@IdCita", idCita);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        cita = new Cita
                        {
                            IdCita = (int)reader["IdCita"],
                            IdPaciente = (int)reader["IdPaciente"],
                            FechaCita = (DateTime)reader["FechaCita"],
                            HoraCita = (TimeSpan)reader["HoraCita"],
                            Turno = reader["Turno"].ToString(),
                            Estado = reader["Estado"].ToString(),
                            TipoTratamiento = reader["TipoTratamiento"].ToString(),
                            Observaciones = reader["Observaciones"].ToString(),
                            Dentista = reader["Dentista"].ToString()
                        };
                    }
                }
            }
            return cita;
        }

        // 2Actualizar cita (solo campos editables)
        public bool ActualizarCitaYHistorial(Cita cita)
        {
            using (SqlConnection con = Conexion.cadena())
            {
               

                SqlCommand cmd = new SqlCommand("spActualizarCitaYHistorial", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IdCita", cita.IdCita);
                cmd.Parameters.AddWithValue("@FechaCita", cita.FechaCita);
                cmd.Parameters.AddWithValue("@HoraCita", cita.HoraCita);
                cmd.Parameters.AddWithValue("@Turno", cita.Turno);
                cmd.Parameters.AddWithValue("@Dentista", cita.Dentista);
                cmd.Parameters.AddWithValue("@TipoTratamiento", cita.TipoTratamiento);

                int resultado = Convert.ToInt32(cmd.ExecuteScalar());

                return resultado == 1;
            }
        }


        public List<Cita> ObtenerCitasPorDia(DateTime fecha, string turno = null, string estado = null)
        {
            List<Cita> citas = new List<Cita>();

            // Nombre del procedimiento almacenado
            string procedimiento = "spObtenerCitasPorFechaYTurnoYEstado";

            using (SqlConnection conn = Conexion.cadena())
            {
                SqlCommand cmd = new SqlCommand(procedimiento, conn);
                cmd.CommandType = CommandType.StoredProcedure;

                // Parámetros
                cmd.Parameters.AddWithValue("@fecha", fecha.Date);

                // Si se proporciona un turno, lo agregamos
                if (!string.IsNullOrEmpty(turno))
                {
                    cmd.Parameters.AddWithValue("@turno", turno);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@turno", DBNull.Value);  // No filtrar por turno si no se especifica
                }

                // Si se proporciona un estado, lo agregamos
                if (!string.IsNullOrEmpty(estado))
                {
                    cmd.Parameters.AddWithValue("@estado", estado);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@estado", DBNull.Value);  // No filtrar por estado si no se especifica
                }

                // Ejecutamos el procedimiento almacenado
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    // Mapear los resultados a la clase Cita
                    Cita cita = new Cita
                    {
                        IdCita = reader.GetInt32(0),
                        IdPaciente = reader.GetInt32(1),
                        FechaCita = reader.GetDateTime(2),
                        HoraCita = reader.GetTimeSpan(3),
                        Turno = reader.GetString(4),
                        Dentista = reader.GetString(5),
                        Estado = reader.GetString(6),
                        TipoTratamiento = reader.GetString(7),
                        Observaciones = reader.IsDBNull(8) ? null : reader.GetString(8),
                        IdPago = reader.IsDBNull(9) ? (int?)null : reader.GetInt32(9),
                        NombrePaciente = reader.GetString(10)
                    };

                    citas.Add(cita);
                }
            }

            return citas;
        }


    }
}
