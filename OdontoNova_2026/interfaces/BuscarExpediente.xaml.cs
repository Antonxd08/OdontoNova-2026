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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace OdontoNova
{
    /// <summary>
    /// Lógica de interacción para BuscarExpediente.xaml
    /// </summary>
    public partial class BuscarExpediente : Window
    {
        private Window formAnterior;

        public BuscarExpediente(Window formAnterior)
        {
            InitializeComponent();
            this.formAnterior = formAnterior;
            CargarFiltros();

            if (this.formAnterior.WindowState == WindowState.Maximized)
            {
                this.WindowState = WindowState.Maximized;
            }
            else
            {
                this.WindowState = WindowState.Normal;
            }
        }

        private void CargarFiltros()
        {
            cb_filtroBusqueda.Items.Add("Nombre");
            cb_filtroBusqueda.Items.Add("Apellido");
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
        private void txt_Busqueda_TextChanged(object sender, TextChangedEventArgs e)
        {
            txtPlaceholder.Visibility = string.IsNullOrWhiteSpace(txt_Busqueda.Text)
                ? Visibility.Visible
                : Visibility.Collapsed;

            string filtro = cb_filtroBusqueda.SelectedItem?.ToString();
            string texto = txt_Busqueda.Text.Trim();

            if (string.IsNullOrWhiteSpace(filtro) || texto.Length < 1)
            {
                OcultarSugerencias();
                lbx_sugerencias.Items.Clear();
                return;
            }

            // Simulación: obtener sugerencias del DAO
            Expedientes dao = new Expedientes();
            var sugerencias = dao.ObtenerSugerencias(filtro, texto);

            lbx_sugerencias.Items.Clear();

            if (sugerencias.Count == 0)
            {
                OcultarSugerencias();
            }
            else
            {
                foreach (var s in sugerencias)
                    lbx_sugerencias.Items.Add(s);

                MostrarSugerencias();
            }
        }

        private void MostrarSugerencias()
        {
            if (!popupSugerencias.IsOpen)
            {
                popupSugerencias.IsOpen = true;

                // Animación de aparición (fade + slide)
                DoubleAnimation fadeIn = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(250))
                {
                    EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
                };

                // Movimiento hacia abajo (slide)
                TranslateTransform tt = new TranslateTransform { Y = -10 };
                lbx_sugerencias.RenderTransform = tt;

                DoubleAnimation slideDown = new DoubleAnimation(-10, 0, TimeSpan.FromMilliseconds(250))
                {
                    EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
                };

                tt.BeginAnimation(TranslateTransform.YProperty, slideDown);
                lbx_sugerencias.BeginAnimation(OpacityProperty, fadeIn);


            }
        }

        private void OcultarSugerencias()
        {
            if (popupSugerencias.IsOpen)
            {
                // Animación de salida (fade + slide arriba)
                DoubleAnimation fadeOut = new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(200))
                {
                    EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseIn }
                };

                TranslateTransform tt = lbx_sugerencias.RenderTransform as TranslateTransform;
                if (tt == null)
                {
                    tt = new TranslateTransform();
                    lbx_sugerencias.RenderTransform = tt;
                }

                DoubleAnimation slideUp = new DoubleAnimation(0, -8, TimeSpan.FromMilliseconds(200))
                {
                    EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseIn }
                };

                fadeOut.Completed += (s, e) =>
                {
                    popupSugerencias.IsOpen = false;
                    lbx_sugerencias.BeginAnimation(OpacityProperty, null);
                    tt.BeginAnimation(TranslateTransform.YProperty, null);
                };

                lbx_sugerencias.BeginAnimation(OpacityProperty, fadeOut);
                tt.BeginAnimation(TranslateTransform.YProperty, slideUp);
            }
        }



        private void lbx_sugerencias_MouseClick(object sender, MouseButtonEventArgs e)
        {
            if (lbx_sugerencias.SelectedItem != null)
            {
                txt_Busqueda.Text = lbx_sugerencias.SelectedItem.ToString();
                OcultarSugerencias();
                txt_Busqueda.Focus();
                txt_Busqueda.SelectionStart = txt_Busqueda.Text.Length;
            }
        }

        private void txt_Busqueda_KeyDown(object sender, KeyEventArgs e)
        {
            // Si el popup está abierto y hay sugerencias
            if (popupSugerencias.IsOpen && lbx_sugerencias.Items.Count > 0)
            {
                // Flecha hacia abajo → mover selección al primer ítem
                if (e.Key == Key.Down)
                {
                    lbx_sugerencias.Focus();
                    lbx_sugerencias.SelectedIndex = 0;
                    e.Handled = true; // evitar que el TextBox procese la tecla
                }
            }

            // Enter → aceptar texto actual y cerrar popup
            if (e.Key == Key.Enter && popupSugerencias.IsOpen)
            {
                OcultarSugerencias();
                e.Handled = true;
            }
        }

        private void lbx_sugerencias_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && lbx_sugerencias.SelectedItem != null)
            {
                txt_Busqueda.Text = lbx_sugerencias.SelectedItem.ToString();
                OcultarSugerencias();
                txt_Busqueda.Focus();
                txt_Busqueda.SelectionStart = txt_Busqueda.Text.Length;
                e.Handled = true;
            }
            else if (e.Key == Key.Escape)
            {
                // ESC → cerrar popup sin seleccionar
                OcultarSugerencias();
                txt_Busqueda.Focus();
                e.Handled = true;
            }
        }

        private void popupSugerencias_Opened(object sender, EventArgs e)
        {
            if (popupSugerencias.Child is FrameworkElement content)
            {
                content.Opacity = 0;
                var sb = (Storyboard)popupSugerencias.Resources["popupOpenAnim"];
                content.BeginStoryboard(sb);
            }
        }

        private void btn_buscar_Click(object sender, RoutedEventArgs e)
        {
            string filtro = cb_filtroBusqueda.SelectedItem?.ToString();
            string texto = txt_Busqueda.Text.Trim();

            if (string.IsNullOrWhiteSpace(filtro) || string.IsNullOrWhiteSpace(texto))
            {
                MessageBox.Show("Seleccione un filtro y escriba un valor para buscar.", "Filtro incompleto", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string nombre = filtro == "Nombre" ? texto : null;
            string apellido = filtro == "Apellido" ? texto : null;

            Expedientes dao = new Expedientes();
            var resultados = dao.BuscarExpedientes(nombre, apellido);

            sp_contenedorBusqueda.Children.Clear();

            if (resultados.Count == 0)
            {
                MessageBox.Show("No se encontraron expedientes.", "Sin resultados", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            foreach (var resultado in resultados)
            {
                UC_Expediente control = new UC_Expediente();

                control.HorizontalAlignment = HorizontalAlignment.Stretch;
                control.Margin = new Thickness(0, 5, 0, 5);

                control.CargarDatos(resultado.expediente, resultado.paciente);

                control.EditarExpedienteClick += Control_EditarExpedienteClick;
                control.VerExpedienteClick += Control_VerExpedienteClick;
                control.VerHistorialClick += Control_VerHistorialClick;

                sp_contenedorBusqueda.Children.Add(control);
            }
        }

        private void Control_VerExpedienteClick(object sender, Paciente paciente)
        {
            UC_Expediente control = sender as UC_Expediente;
            if (control == null) return;

            ExpedientePaciente diseño = new ExpedientePaciente(this, paciente, control.expedienteGuardado);
            diseño.Show();
            this.Hide();
        }

        private void Control_VerHistorialClick(object sender, Paciente paciente)
        {
            Historial historial = new Historial(this, paciente);
            historial.Show();
            this.Hide();
        }

        private void Control_EditarExpedienteClick(object sender, Paciente paciente)
        {
            RegistrarPaciente editarPaciente = new RegistrarPaciente(this, paciente);
            editarPaciente.Show();
            this.Hide();
        }

        

    }
}
