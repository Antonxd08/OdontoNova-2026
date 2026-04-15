using OdontoNova.Interfaces;
using OdontoNova_2026;
using OdontoNovaWPF_FIS;
using SistemaOdontonova.Interfaces;
using System;
using System.Collections.Generic;
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
    /// Lógica de interacción para MenuRecepcionista.xaml
    /// </summary>
    public partial class MenuRecepcionista : Window
    {
        private DispatcherTimer timer;
       
        private string turno;

        private Usuario usuarioActual;

        public MenuRecepcionista(Usuario usuario)
        {
            InitializeComponent();
            usuarioActual = usuario;
            IniciarReloj();
            CargarCitasDelTurno();
            CargarDatosUsuarioMenu();

            this.WindowState = WindowState.Maximized;
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


        // ------------------- Reloj -------------------
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

        // ------------------- Manejo de Turno y Citas -------------------
        private void CargarCitasDelTurno()
        {
            // Determinar el turno según el rol
            switch (usuarioActual.Rol)
            {
                case "Recepcionista tm":
                    turno = "Mañana";
                    break;
                case "Recepcionista tv":
                    turno = "Tarde";
                    break;
                default:
                    turno = "Desconocido";
                    break;
            }

            // Obtener citas
            CitasDAO citasDAO = new CitasDAO();
            List<Cita> citas = citasDAO.ObtenerCitasPorFechaYTurno(DateTime.Today, turno);

            // Limpiar contenedor
            if (br_contenedorCitas.Child is ScrollViewer scroll &&
                scroll.Content is StackPanel contenedor)
            {
                contenedor.Children.Clear();

                if (citas.Count > 0)
                {
                    foreach (Cita cita in citas)
                    {
                        UC_Citas card = new UC_Citas();

                        card.HorizontalAlignment = HorizontalAlignment.Stretch;
                        card.Margin = new Thickness(0, 10, 0, 0);

                        card.CargarDatos(cita.IdCita, cita.NombrePaciente, cita.TipoTratamiento, cita.HoraCita);
                        
                        contenedor.Children.Add(card);
                    }
                }
                else
                {
                    contenedor.Children.Add(new TextBlock
                    {
                        Text = "No hay citas registradas para este turno.",
                        Foreground = Brushes.Gray,
                        FontStyle = FontStyles.Italic,
                        Margin = new Thickness(10),
                        FontSize = 14
                    });
                }
            }

     
        }

        // ------------------- Eventos de ventana -------------------
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
            this.WindowState = (this.WindowState == WindowState.Maximized)
                ? WindowState.Normal
                : WindowState.Maximized;
        }

        private void btn_cerrarSesion_Click(object sender, RoutedEventArgs e)
        {
            MainWindow ventanaPrincipal = new MainWindow();
            ventanaPrincipal.Show();
            this.Close();
        }

        #region Botones de navegación

        private void btn_registrarPaciente_Click(object sender, RoutedEventArgs e)
        {
            RegistrarPaciente registro = new RegistrarPaciente(this, null);
            registro.Show();
            this.Hide();
        }
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
           AcercaDe verAcercaDe = new AcercaDe(this);
           verAcercaDe.Show();
            this.Hide();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            CitasDAO.ActualizarEstadoCitasPorTurno();
        }
    }
}
