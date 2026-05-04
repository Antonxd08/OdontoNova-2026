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

namespace OdontoNova_2026.interfaces
{
    /// <summary>
    /// Lógica de interacción para MenuAdministrador.xaml
    /// </summary>
    public partial class MenuAdministrador : Window
    {
        public MenuAdministrador()
        {
            InitializeComponent();
        }

        private void btn_dashboard_Click(object sender, RoutedEventArgs e)
        {
            // Mostrar Dashboard, Ocultar Backups
            PanelDashboard.Visibility = Visibility.Visible;
            PanelBackups.Visibility = Visibility.Collapsed;
        }

        private void btn_backup_Click(object sender, RoutedEventArgs e)
        {
            // Mostrar Backups, Ocultar Dashboard
            PanelDashboard.Visibility = Visibility.Collapsed;
            PanelBackups.Visibility = Visibility.Visible;
        }

        private void btn_Examinar_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btn_FullBackup_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btn_DiffBackup_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btn_cerrarSesion_Click(object sender, RoutedEventArgs e)
        {
            MainWindow ventanaPrincipal = new MainWindow();
            ventanaPrincipal.Show();
            this.Close();
        }

        private void btn_Max_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
                this.WindowState = WindowState.Normal;
            else
                this.WindowState = WindowState.Maximized;
        }

        private void btn_Min_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void Header_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }
    }
}
