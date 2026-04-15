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
    /// Lógica de interacción para PagoCita.xaml
    /// </summary>
    public partial class PagoCita : Window
    {
        private int idCita;

        public PagoCita(int idCita)
        {
            InitializeComponent();
            this.idCita = idCita;
            lb_idCita.Text = idCita.ToString();
        }

        private void btn_ConfirmarPago_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Validar monto obligatorio
                if (!decimal.TryParse(txt_Monto.Text, out decimal monto))
                {
                    MessageBox.Show("El monto es obligatorio y debe ser numérico.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (monto < 0)
                {
                    MessageBox.Show("El monto no puede ser negativo.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Validar monto extra opcional
                decimal montoExtra = 0;
                if (!string.IsNullOrWhiteSpace(txt_MontoExtra.Text))
                {
                    if (!decimal.TryParse(txt_MontoExtra.Text, out montoExtra))
                    {
                        MessageBox.Show("El monto extra debe ser numérico si se proporciona.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    if (montoExtra < 0)
                    {
                        MessageBox.Show("El monto extra no puede ser negativo.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                }

                // Ejecutar procedimiento
                decimal totalPagado = Pago.RegistrarPagoCita(idCita, monto, montoExtra);

                MessageBox.Show($"✅ Pago registrado correctamente.\nTotal acumulado: ${totalPagado:N2}", "Confirmación", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("❌ Error al registrar el pago: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void chk_Extra_Changed(object sender, RoutedEventArgs e)
        {
            bool habilitado = chk_Extra.IsChecked == true;

            txt_MontoExtra.IsEnabled = habilitado;
            txt_MontoExtra.Opacity = habilitado ? 1 : 0.4;

            if (!habilitado)
                txt_MontoExtra.Text = "";
        }

        private void BtnCerrar_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("¿Deseas cancelar el pago de esta cita?", "Confirmar cancelación",
                                         MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
                this.Close();
        }

        // Arrastrar ventana
        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
                this.DragMove();
        }

    }
}
