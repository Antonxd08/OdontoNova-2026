using OdontoNovaWPF_FIS;
using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace OdontoNova
{
    /// <summary>
    /// Lógica de interacción para Historial.xaml
    /// </summary>
    public partial class Historial : Window
    {
        private Window formAnterior;
        private Paciente pacienteActual;

        public Historial(Window anterior, Paciente paciente)
        {
            InitializeComponent();
            formAnterior = anterior;
            pacienteActual = paciente;

           
            CargarHistorial();

            if (formAnterior.WindowState == WindowState.Maximized)
            {
                this.WindowState = WindowState.Maximized;
            }
            else
            {
                this.WindowState = WindowState.Normal;
            }
        }

        private void btn_regresar_Click(object sender, RoutedEventArgs e)
        {
            formAnterior.Show();
            this.Close();
        }

        private void Min_Click(object sender, RoutedEventArgs e)
        {

            this.WindowState = WindowState.Minimized;
        }
        private void Max_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
                this.WindowState = WindowState.Normal;
            else
                this.WindowState = WindowState.Maximized;
        }
        private void Header_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void CargarHistorial()
        {
            var historial = HistorialDental.ObtenerHistorialPorPaciente(pacienteActual.IdPaciente);

            sp_contenedorcitas.Children.Clear();

            foreach (var cita in historial)
            {
                
                UC_Historial control = new UC_Historial();

                control.HorizontalAlignment = HorizontalAlignment.Stretch;
                control.Margin = new Thickness(0, 5, 0, 5);

                control.CargarDatos(
                    cita.IdCita,
                    cita.FechaRegistro,
                    cita.EstadoCita,
                    cita.TipoTratamiento,
                    (double)cita.MontoPagado,
                    (double)cita.MontoExtraPagado
                    
                );

                sp_contenedorcitas.Children.Add(control);
            }
        }
    }
}
