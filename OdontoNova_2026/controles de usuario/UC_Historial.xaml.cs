using OdontoNova;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace OdontoNovaWPF_FIS
{
    /// <summary>
    /// Lógica de interacción para UC_Historial.xaml
    /// </summary>
    public partial class UC_Historial : UserControl
    {
        private int idCita;

        public UC_Historial()
        {
            InitializeComponent();
        }

        public void CargarDatos(int idCita, DateTime fechaHora, string estado, string tratamiento, double montoCita, double montoExtra)
        {
            this.idCita = idCita;
            lb_idCita.Text = idCita.ToString();
            lb_fechaYHora.Text = $"{fechaHora:d}\n{fechaHora:t}";
            lb_estadoCita.Text = estado;
            lb_tratamiento.Text = tratamiento;
            lb_montoCita.Text = montoCita.ToString("C2");
            lb_montoExtra.Text = montoExtra.ToString("C2");

            if (estado.Equals("Pendiente", StringComparison.OrdinalIgnoreCase))
            {
                Btn_EditarCita.Visibility = Visibility.Visible;
                Btn_EditarCita.IsEnabled = true;
            }
            else
            {
                Btn_EditarCita.Visibility = Visibility.Collapsed; // También puedes usar IsEnabled = false si quieres solo deshabilitarlo
            }
        }

        private void Btn_EditarCita_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show($"¿Desea modificar la cita {idCita}?",
                                     "Confirmar Modificación",
                                     MessageBoxButton.YesNo,
                                     MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                // 'Window.GetWindow(this)' obtiene la ventana contenedora del UserControl
                Window ventanaPadre = Window.GetWindow(this);

                // Abrir la ventana de Citas pasando la ventana padre y el id de la cita
                var ventanaCitas = new Citas(ventanaPadre, idCita);
                ventanaCitas.ShowDialog();
            }
        }
    }
}
