using OdontoNova;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SistemaOdontonova.Interfaces
{
    /// <summary>
    /// Lógica de interacción para UC_EventoCitaIndividual.xaml
    /// </summary>
    public partial class UC_EventoCitaIndividual : UserControl
    {
        public UC_EventoCitaIndividual()
        {
            InitializeComponent();
        }

        public void CargarCita(Cita cita)
        {
           

            TimeSpan hora = cita.HoraCita; 

            DateTime horaDateTime = DateTime.Today.Add(hora);

            // Formatear hora y AM/PM
            Txt_Hora.Text = horaDateTime.ToString("hh:mm"); // hora en formato 12h
            Txt_AMPM.Text = horaDateTime.ToString("tt");    // AM o PM


            Txt_NombrePaciente.Text = cita.NombrePaciente;
            // Asignar el tipo de tratamiento
            Txt_Tratamiento.Text = cita.TipoTratamiento;
            // Asignar el nombre del dentista
            Txt_Dentista.Text = cita.Dentista;
            // Asignar el estado de la cita
            Txt_Estado.Text = cita.Estado;

            // Cambiar el color de fondo del estado de la cita según el estado
            switch (cita.Estado)
            {
                case "Pendiente":
                    Border_Estado.Background = new SolidColorBrush(Colors.Orange); // Ejemplo color para pendiente
                    break;
                case "Asistida":
                    Border_Estado.Background = new SolidColorBrush(Colors.Green); // Ejemplo color para asistida
                    break;
                case "No Asistida":
                    Border_Estado.Background = new SolidColorBrush(Colors.Red); // Ejemplo color para no asistida
                    break;
                default:
                    Border_Estado.Background = new SolidColorBrush(Colors.Gray); // Default color
                    break;
            }
        }
    }
}
