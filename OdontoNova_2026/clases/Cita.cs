using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OdontoNova
{
    public class Cita
    {
        public int IdCita { get; set; }
        public int IdPaciente { get; set; }
        public DateTime FechaCita { get; set; }
        public TimeSpan HoraCita { get; set; }
        public string Turno { get; set; }
        public string Dentista { get; set; }
        public string Estado { get; set; }
        public string TipoTratamiento { get; set; }
        public string Observaciones { get; set; }
        public int? IdPago { get; set; }

        public string NombrePaciente { get; set; }

    }

}
