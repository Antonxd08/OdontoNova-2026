using OdontoNova.Interfaces;
using OdontoNova_2026;
using OdontoNovaWPF_FIS;
using SistemaOdontonova.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
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
using System.Windows.Threading;

namespace OdontoNova
{
    /// <summary>
    /// Lógica de interacción para MenuDoctor.xaml
    /// </summary>
    public partial class MenuDoctor : Window
    {
        private DispatcherTimer timer;
        private string rolSeleccionado;
        private Usuario usuarioActual;


        public MenuDoctor(Usuario usuario)
        {
            InitializeComponent();
            
            IniciarReloj();
            this.Loaded += MenuDoctor_Loaded;

            this.WindowState = WindowState.Maximized;

           

            usuarioActual = usuario;

        }

        private void CargarDatosUsuarioMenu()
        {
            // Mostrar el nombre
            txtNombreUsuarioMenu.Text = usuarioActual.NombreUsuario;

            // Mostrar la foto si tiene una
            if (usuarioActual.FotoUsuario != null)
            {
                using (MemoryStream ms = new MemoryStream(usuarioActual.FotoUsuario))
                {
                    BitmapImage imagen = new BitmapImage();
                    imagen.BeginInit();
                    imagen.CacheOption = BitmapCacheOption.OnLoad;
                    imagen.StreamSource = ms;
                    imagen.EndInit();
                    imgUsuarioMenu.Source = imagen;
                }
            }
            else
            {
                // Imagen por defecto
                imgUsuarioMenu.Source = new BitmapImage(
                    new Uri("pack://application:,,,/Assets/Images/user_456212.png"));
            }

            // Aplicar clip circular
            AplicarClipCircular(imgUsuarioMenu);
        }

        private void AplicarClipCircular(Image img)
        {
            EllipseGeometry clip = new EllipseGeometry();
            clip.Center = new Point(img.Width / 2, img.Height / 2);
            clip.RadiusX = img.Width / 2;
            clip.RadiusY = img.Height / 2;
            img.Clip = clip;
        }


        private void MenuDoctor_Loaded(object sender, RoutedEventArgs e)
        {
            CargarTodasLasCitasDelDia();
            CargarDatosUsuarioMenu();
        }

        #region Reloj
        private void IniciarReloj()
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += ActualizarFechaHora;
            timer.Start();
        }

        private void ActualizarFechaHora(object sender, EventArgs e)
        {
            DateTime ahora = DateTime.Now;
            txtb_fecha.Text = ahora.ToString("dddd dd 'de' MMMM 'del' yyyy");
            txtb_hora.Text = ahora.Hour.ToString("D2");
            txtb_minutos.Text = ahora.Minute.ToString("D2");
            txtb_segundos.Text = ahora.Second.ToString("D2");
        }
        #endregion

        #region Ventana (drag, minimizar, maximizar)
        private void Header_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
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

        private void btn_cerrarSesion_Click(object sender, RoutedEventArgs e)
        {
            MainWindow ventanaPrincipal = new MainWindow();
            ventanaPrincipal.Show();
            this.Close();
        }
        #endregion

        #region Citas
        private void CargarTodasLasCitasDelDia()
        {
            // Limpiar contenedor de citas
            var scroll = br_contenedorCitas.Child as ScrollViewer;
            var contenedor = scroll?.Content as StackPanel;
            if (contenedor == null) return;

            contenedor.Children.Clear();

            // Obtener citas
            CitasDAO citasDAO = new CitasDAO();
            List<Cita> citas = citasDAO.ObtenerCitasPorFechaYTurno(DateTime.Today, null);


            foreach (Cita cita in citas)
            {
                UC_Citas card = new UC_Citas(); 
                card.CargarDatos(cita.IdCita, cita.NombrePaciente, cita.TipoTratamiento, cita.HoraCita);
               
                contenedor.Children.Add(card);
            }

            if (citas.Count == 0)
            {
                System.Windows.Controls.TextBlock aviso = new System.Windows.Controls.TextBlock
                {
                    Text = "No hay citas registradas para hoy.",
                    Foreground = System.Windows.Media.Brushes.Gray,
                    FontSize = 14,
                    Margin = new Thickness(10)
                };
                contenedor.Children.Add(aviso);
                return;
            }

        }

        
        #endregion

        #region Botones de navegación
        private void btn_verCalendario_Click(object sender, RoutedEventArgs e)
        {
            CalendarioCitaDiaria verCalendario = new CalendarioCitaDiaria(this);
            verCalendario.Show();
            this.Hide();
        }

        private void btn_registrarCita_Click(object sender, RoutedEventArgs e)
        {
            Citas registrarCita = new Citas(this);
            registrarCita.Show();
            this.Hide();
        }

        private void btn_buscarExpediente_Click(object sender, RoutedEventArgs e)
        {
            BuscarExpediente expediente = new BuscarExpediente(this);
            expediente.Show();
            //this.Hide();
        }

        private void btn_generarReportes_Click(object sender, RoutedEventArgs e)
        {
            Reportes reportes = new Reportes(this, usuarioActual);
            reportes.Show();
            this.Hide();
        }

        private void btn_verPerfil_Click(object sender, RoutedEventArgs e)
        {
            PerfilUsuario perfil = new PerfilUsuario(this, usuarioActual);
            perfil.Show();
        }
        #endregion

        private void btn_AcercaDe_Click(object sender, RoutedEventArgs e)
        {
            AcercaDe verAcercade = new AcercaDe(this);
            verAcercade.Show();
            this.Hide();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            CitasDAO.ActualizarEstadoCitasPorTurno();
        }
    }
}
