using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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

namespace OdontoNova_2026.interfaces
{
    /// <summary>
    /// Lógica de interacción para MenuAdministrador.xaml
    /// </summary>
    public partial class MenuAdministrador : Window
    {
        string ConexionBaseLocal = @"Data Source=LAPTOP-DD4EQNHS\MSSQLSERVER01;Initial Catalog=OdontoNova_2026_plus;Integrated Security=True;";

        // El SP de restauración vive en [master]: la conexión DEBE apuntar a master
        // para poder hacer SET SINGLE_USER sobre OdontoNova_2026_plus.
        private readonly string ConexionMaster =
            @"Data Source=LAPTOP-DD4EQNHS\MSSQLSERVER01;" +
            @"Initial Catalog=master;Integrated Security=True;";
        public MenuAdministrador()
        {
            InitializeComponent();
            CargarHistorial();
            this.WindowState = WindowState.Maximized;

        }

        #region Metodos para ejecutar SPs y cargar tablas
        // Modelo de datos para las tablas
        public class BackupLog
        {
            public string Fecha { get; set; }
            public string Hora { get; set; }
            public string Archivo { get; set; }
            public string Tamaño { get; set; }
        }

        private void CargarHistorial()
        {
            try
            {
                string ruta = txtRuta.Text;

                txtRutaRestore.Text = ruta;

                if (!Directory.Exists(ruta)) return;

                var archivos = new DirectoryInfo(ruta)
                    .GetFiles("*.bak")
                    .OrderByDescending(f => f.CreationTime)
                    .ToList();

                var listaFull = new List<BackupLog>();
                var listaDiff = new List<BackupLog>();

                foreach (var archivo in archivos)
                {
                    var log = new BackupLog
                    {
                        Fecha = archivo.CreationTime.ToString("dd/MM/yyyy"),
                        Hora = archivo.CreationTime.ToString("HH:mm"),
                        Archivo = archivo.Name,
                        Tamaño = (archivo.Length / 1024 / 1024) > 0
                                  ? $"{archivo.Length / 1024.0 / 1024.0:N2} MB"
                                  : $"{archivo.Length / 1024.0:N2} KB"
                    };

                    if (archivo.Name.Contains("Full"))
                        listaFull.Add(log);
                    else if (archivo.Name.Contains("Diff"))
                        listaDiff.Add(log);
                }

                // Historiales 
                dgHistorialFull.ItemsSource = listaFull;
                dgHistorialDiff.ItemsSource = listaDiff;

                // Tabla del panel de Restauracion
                dgRestoreFull.ItemsSource = new List<BackupLog>(listaFull);

                dgRestoreFull.SelectedItem = null;
                btn_RestoreDB.IsEnabled = false;
                borderSeleccionada.Visibility = Visibility.Collapsed;
                txtArchivoSeleccionado.Text = string.Empty;
            }
            catch (Exception)
            {
                
            }
        }

        private async Task EjecutarSPAsync(string spNombre, string tipo)
        {
            try
            {
                using (var conn = new SqlConnection(ConexionBaseLocal))
                {
                    var cmd = new SqlCommand(spNombre, conn)
                    {
                        CommandType = CommandType.StoredProcedure,
                        CommandTimeout = 300
                    };
                    cmd.Parameters.AddWithValue("@Ruta", txtRuta.Text);

                    await conn.OpenAsync();
                    await cmd.ExecuteNonQueryAsync();

                    MessageBox.Show(
                        $"¡Exito! El respaldo {tipo} ha sido generado correctamente.",
                        "Sistema", MessageBoxButton.OK, MessageBoxImage.Information);

                    CargarHistorial();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error al generar el respaldo: {ex.Message}",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion

        #region Botones para copias de seguridad
        private void btn_Examinar_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog
            {
                Title = "Seleccionar carpeta de destino",
                Filter = "Directorio|*.this.directory",
                FileName = "Seleccione esta carpeta",
                InitialDirectory = @"C:\"
            };

            if (dialog.ShowDialog() == true)
            {
                string carpeta = System.IO.Path.GetDirectoryName(dialog.FileName);
                if (!string.IsNullOrEmpty(carpeta))
                {
                    txtRuta.Text = carpeta.EndsWith("\\") ? carpeta : carpeta + "\\";
                    CargarHistorial(); // Actualizar tablas al cambiar carpeta
                }
            }
        }

        private async void btn_FullBackup_Click(object sender, RoutedEventArgs e)
        {
            btn_FullBackup.IsEnabled = false;
            await EjecutarSPAsync("spCopiaSegCompletaOdontoNova_2026", "Completo");
            btn_FullBackup.IsEnabled = true;
        }

        private async void btn_DiffBackup_Click(object sender, RoutedEventArgs e)
        {
            btn_DiffBackup.IsEnabled = false;
            await EjecutarSPAsync("spCopiaSegDiferencialOdontoNova_2026", "Diferencial");
            btn_DiffBackup.IsEnabled = true;
        }

        #endregion

        #region Panel de restauracion
        private void dgRestoreFull_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgRestoreFull.SelectedItem is BackupLog seleccionado)
            {
                btn_RestoreDB.IsEnabled = true;
                borderSeleccionada.Visibility = Visibility.Visible;
                txtArchivoSeleccionado.Text = seleccionado.Archivo;
            }
            else
            {
                btn_RestoreDB.IsEnabled = false;
                borderSeleccionada.Visibility = Visibility.Collapsed;
                txtArchivoSeleccionado.Text = string.Empty;
            }
        }

        /// Llama a spRestaurarOdontoNova_2026 en [master] con la ruta completa del archivo .bak seleccionado en la tabla.
        private async void btn_RestoreDB_Click(object sender, RoutedEventArgs e)
        {
            if (!(dgRestoreFull.SelectedItem is BackupLog seleccionado))
                return;

            string rutaCompleta = txtRuta.Text + seleccionado.Archivo;

            // Confirmacion
            var confirmacion = MessageBox.Show(
                $"Esta a punto de restaurar la base de datos con el archivo:\n\n" +
                $"  {seleccionado.Archivo}\n\n" +
                $"Esta acción:\n" +
                $"  • Reemplazara TODOS los datos actuales.\n" +
                $"  • Cerrara todas las conexiones activas.\n\n" +
                $"¿Desea continuar?",
                "Confirmar Restauración",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning,
                MessageBoxResult.No);   

            if (confirmacion != MessageBoxResult.Yes) return;

            // Segunda confirmacion
            var segundaConfirmacion = MessageBox.Show(
                "ULTIMA ADVERTENCIA: Esta operacion no se puede deshacer.\n\n¿Confirma la restauracion?",
                "⚠  Confirmar definitivamente",
                MessageBoxButton.YesNo,
                MessageBoxImage.Stop,
                MessageBoxResult.No);

            if (segundaConfirmacion != MessageBoxResult.Yes) return;

            // Ejecutar restauracion 
            btn_RestoreDB.IsEnabled = false;
            btn_restauracion.IsEnabled = false;

            try
            {
                using (var conn = new SqlConnection(ConexionMaster))
                {
                    var cmd = new SqlCommand("spRestaurarOdontoNova_2026", conn)
                    {
                        CommandType = CommandType.StoredProcedure,
                        CommandTimeout = 600   
                    };
                    cmd.Parameters.AddWithValue("@RutaArchivo", rutaCompleta);

                    await conn.OpenAsync();
                    await cmd.ExecuteNonQueryAsync();
                }

                MessageBox.Show(
                    $"✔  Base de datos restaurada con exito.\n\nArchivo utilizado:\n{seleccionado.Archivo}",
                    "Restauracion completada",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                dgRestoreFull.SelectedItem = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error durante la restauración:\n\n{ex.Message}",
                    "Error de Restauración",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
            finally
            {
                btn_RestoreDB.IsEnabled = false;  
                btn_restauracion.IsEnabled = true;
            }
        }

        #endregion

        #region Botones sidebar y controles de ventana
        private void btn_dashboard_Click(object sender, RoutedEventArgs e)
        {
            PanelDashboard.Visibility = Visibility.Visible;
            PanelBackups.Visibility = Visibility.Collapsed;
            PanelRestore.Visibility = Visibility.Collapsed;
        }

        private void btn_backup_Click(object sender, RoutedEventArgs e)
        {
            PanelDashboard.Visibility = Visibility.Collapsed;
            PanelBackups.Visibility = Visibility.Visible;
            PanelRestore.Visibility = Visibility.Collapsed;
            CargarHistorial();
        }

        private void btn_restauracion_Click(object sender, RoutedEventArgs e)
        {
            PanelDashboard.Visibility = Visibility.Collapsed;
            PanelBackups.Visibility = Visibility.Collapsed;
            PanelRestore.Visibility = Visibility.Visible;
            CargarHistorial(); 
        }

        private void btn_cerrarSesion_Click(object sender, RoutedEventArgs e)
        {
            new MainWindow().Show();
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

        #endregion

    }
}
