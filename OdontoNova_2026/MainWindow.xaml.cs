using OdontoNova;
using OdontoNova.Clases;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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

namespace OdontoNova_2026
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Usuario usuarioActual;
        public MainWindow()
        {
            InitializeComponent();
            SqlConnection con = Conexion.cadena();
            usuarioActual = new Usuario();

            var usuarios = new List<ModeloCard>
            {
                new ModeloCard
                {
                    Titulo = "Recepcionista Turno Matutino",
                    Subtitulo = "Atención de 8:00 AM a 2:00 PM",
                    RutaIcono = "Assets/Images/Usuarios.png",
                    ColorIcono = new SolidColorBrush(Color.FromRgb(47, 128, 237))
                },
                new ModeloCard
                {
                    Titulo = "Recepcionista Turno Vespertino",
                    Subtitulo = "Atención de 3:00 PM a 7:00 PM",
                    RutaIcono = "Assets/Images/Usuarios.png",
                    ColorIcono = new SolidColorBrush(Color.FromRgb(96, 165, 250))
                },
                new ModeloCard
                {
                    Titulo = "Doctor Ruben Lara",
                    Subtitulo = "Consultas y revisiones",
                    RutaIcono = "Assets/Images/Usuarios.png",
                    ColorIcono = new SolidColorBrush(Color.FromRgb(18, 58, 138))
                }
            };

            UserCards.ItemsSource = usuarios;
        }

        private void Header_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
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

        private void BtnIngresar_Click(object sender, RoutedEventArgs e)
        {
            if (sender is System.Windows.Controls.Button boton && boton.DataContext is ModeloCard card)
            {

                // Asignar el rol según el título de la tarjeta
                if (card.Titulo.Contains("Recepcionista Turno Matutino"))
                    usuarioActual.Rol = "Recepcionista tm";
                else if (card.Titulo.Contains("Recepcionista Turno Vespertino"))
                    usuarioActual.Rol = "Recepcionista tv";
                else if (card.Titulo.Contains("Doctor Ruben Lara"))
                    usuarioActual.Rol = "Dueño";
                else
                    usuarioActual.Rol = "Invitado";

                // Abrir ventana de inicio de sesión con el rol definido
                var inicioSesion = new InicioSesion(usuarioActual);
                inicioSesion.Show();

                // Cerrar o esconder esta ventana
                this.Close();
            }
        }
    }
}
