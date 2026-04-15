using Microsoft.SqlServer.Server;
using OdontoNova_2026;
using SistemaOdontonova.Interfaces;
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
    /// Lógica de interacción para InicioSesion.xaml
    /// </summary>
    public partial class InicioSesion : Window
    {
        private string rolSeleccionado;
        private Usuario usuarioActual;
        bool mostrarNuevaContra = false;

        public InicioSesion(Usuario usuario)
        {
            InitializeComponent();

            rolSeleccionado = usuario.Rol;
            usuarioActual = usuario;

            this.WindowState = WindowState.Maximized;
        }

        private void Btn_MostrarContraseña_Click(object sender, RoutedEventArgs e)
        {
            if (!mostrarNuevaContra)
            {
                txtContrasenaVisible.Text = txtContrasena.Password;
                txtContrasenaVisible.Visibility = Visibility.Visible;
                txtContrasena.Visibility = Visibility.Collapsed;
                Btn_MostrarContraseña.Content = "🚫";
                mostrarNuevaContra = true;
            }
            else
            {
                txtContrasena.Password = txtContrasenaVisible.Text;
                txtContrasenaVisible.Visibility = Visibility.Collapsed;
                txtContrasena.Visibility = Visibility.Visible;
                Btn_MostrarContraseña.Content = "👁";
                mostrarNuevaContra = false;
            }
        }

        private void TextBlock_RecuperarContrasena(object sender, MouseButtonEventArgs e)
        {
            CambiarContraseña token = new CambiarContraseña(usuarioActual);
            token.Show();
            this.Hide(); 
        }

        private void btn_regresar_Click(object sender, RoutedEventArgs e)
        {
            MainWindow ventanaPrincipal = new MainWindow();
            ventanaPrincipal.Show();
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

        private void btn_acceder_Click(object sender, RoutedEventArgs e)
        {
            string nombre = txtUsuario.Text.Trim();

            
            string contra;
            if (txtContrasena.Visibility == Visibility.Visible)
                contra = txtContrasena.Password.Trim();
            else
                contra = txtContrasenaVisible.Text.Trim();

            if (string.IsNullOrEmpty(nombre) || string.IsNullOrEmpty(contra))
            {
                MessageBox.Show("Por favor complete todos los campos.", "Campos incompletos", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            AccesoDatos acceso = new AccesoDatos();
            Usuario usuario = acceso.ObtenerUsuarioPorNombre(nombre); // solo buscar por nombre

            if (usuario == null)
            {
                MessageBox.Show("Usuario incorrecto.", "Error de autenticación", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Verificar rol
            if (usuario.Rol != rolSeleccionado)
            {
                MessageBox.Show("El rol seleccionado no coincide con el rol del usuario.", "Error de inicio", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Verificar contraseña usando BCrypt
            if (!BCrypt.Net.BCrypt.Verify(contra, usuario.Contraseña))
            {
                MessageBox.Show("Contraseña incorrecta.", "Error de autenticación", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }


            // Redirigir según rol
            if (usuario.Rol == "Dueño")
            {
                MenuDoctor menuDoctor = new MenuDoctor(usuario);
                menuDoctor.Show();
            }
            else
            {
                MenuRecepcionista menuRecepcionista = new MenuRecepcionista(usuario);
                menuRecepcionista.Show();
            }

            this.Close();
        }

        private void TextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            CambiarContraseña token = new CambiarContraseña(usuarioActual);
            token.Show();
            this.Hide();
        }
    }
}
