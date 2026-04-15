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
    /// Lógica de interacción para UC_Citas.xaml
    /// </summary>
    public partial class UC_Citas : UserControl
    {
        public event EventHandler<(int idCita, string monto)> PagoConfirmado;
        private int idCita;

        public UC_Citas()
        {
            InitializeComponent();

            // Tooltip
            btnConfirmarPago_pagar.ToolTip = "Confirmar asistencia y registrar pago";

            

            // Color inicial del botón
            btnConfirmarPago_pagar.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#276DCF"));
        }

        public void CargarDatos(int idCita, string nombre, string tratamiento, TimeSpan hora)
        {
            this.idCita = idCita;
            lb_nombrePaciente.Text = nombre;
            lb_tratamiento.Text = tratamiento;

            DateTime horaFormateada = DateTime.Today.Add(hora);
            lb_horaCita.Text = horaFormateada.ToShortTimeString();
        }

        

        private void Card_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            
        }

        private void btnConfirmarPago_pagar_Click(object sender, RoutedEventArgs e)
        {
            
            PagoCita pago = new PagoCita(idCita);
            pago.ShowDialog();
        }
    }
}
