using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Lógica de interacción para RegistrarPaciente.xaml
    /// </summary>
    public partial class RegistrarPaciente : Window
    {
        private Window formularioAnterior;
        private int? idPaciente = null; // Para edición

        // Constructor para edición
        public RegistrarPaciente(Window formularioAnterior, Paciente paciente)
        {
            InitializeComponent();
            this.formularioAnterior = formularioAnterior;

            dp_FechaRegistro.SelectedDate = DateTime.Today;


            cb_generoPaciente.Items.Clear();
            cb_generoPaciente.Items.Add("Masculino");
            cb_generoPaciente.Items.Add("Femenino");

            txt_nombrePaciente.TextChanged += ValidarTexto;
            txt_apellidoPaciente.TextChanged += ValidarTexto;
            txt_numTelefono.TextChanged += ValidarTelefono;
            dp_fechaNaciemiento.SelectedDateChanged += ValidarFechaNacimiento;
            txt_direccion.TextChanged += ValidarDireccion;
            cb_generoPaciente.SelectionChanged += ValidarCombo;

            // Cargar datos del paciente
            CargarDatosParaEdicion(paciente);

            if (this.formularioAnterior.WindowState == WindowState.Maximized)
            {
                this.WindowState = WindowState.Maximized;
            }
            else
            {
                this.WindowState = WindowState.Normal;
            }
        }

        private void CargarDatosParaEdicion(Paciente paciente)
        {
            if (paciente == null) return;

            idPaciente = paciente.IdPaciente; // Guardamos ID para actualizar
            txt_nombrePaciente.Text = paciente.Nombre;
            txt_apellidoPaciente.Text = paciente.Apellido;
            cb_generoPaciente.SelectedItem = paciente.Genero;
            dp_fechaNaciemiento.SelectedDate = paciente.FechaNacimiento;

            // Campos editables
            txt_numTelefono.Text = paciente.Telefono;
            txt_direccion.Text = paciente.Direccion;
            txt_antecedentesMedicos.Text = paciente.AntecedentesMedicos;

            if (paciente.FotoPaciente != null)
            {
                BitmapImage bitmap = new BitmapImage();
                using (MemoryStream ms = new MemoryStream(paciente.FotoPaciente))
                {
                    bitmap.BeginInit();
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.StreamSource = ms;
                    bitmap.EndInit();
                }
                imgFotoPaciente.Source = bitmap;
                imgFotoPaciente.Visibility = Visibility.Visible;
                placeholderFoto.Visibility = Visibility.Collapsed;
            }

            // Bloquear campos que no se deben editar
            txt_nombrePaciente.IsEnabled = false;
            txt_apellidoPaciente.IsEnabled = false;
            dp_fechaNaciemiento.IsEnabled = false;
            cb_generoPaciente.IsEnabled = false;
        }



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

        private void btn_regresar_Click(object sender, RoutedEventArgs e)
        {
            formularioAnterior.Show();
            this.Close();
        }

        #region Manejo de foto
        private void FotoPaciente_Click(object sender, MouseButtonEventArgs e)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog
            {
                Title = "Seleccionar foto del paciente",
                Filter = "Archivos de imagen|*.jpg;*.jpeg;*.png;*.bmp"
            };

            if (dialog.ShowDialog() == true)
            {
                var bitmap = new BitmapImage(new Uri(dialog.FileName));
                imgFotoPaciente.Source = bitmap;

                // Mostrar imagen y ocultar texto de marcador
                imgFotoPaciente.Visibility = Visibility.Visible;
                placeholderFoto.Visibility = Visibility.Collapsed;
                LimpiarError(borderFotoPaciente);
            }
        }

        private byte[] ObtenerFotoComoBytes()
        {
            if (imgFotoPaciente.Source == null) return null;

            BitmapImage bitmap = imgFotoPaciente.Source as BitmapImage;
            byte[] data;
            JpegBitmapEncoder encoder = new JpegBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bitmap));

            using (MemoryStream ms = new MemoryStream())
            {
                encoder.Save(ms);
                data = ms.ToArray();
            }

            return data;
        }
        #endregion

        #region Validaciones Visuales

        private void MarcarError(FrameworkElement control, string mensaje)
        {
            // Cambiar borde si es un control que soporta BorderBrush
            switch (control)
            {
                case Border border:
                    border.BorderBrush = Brushes.Red;
                    border.BorderThickness = new Thickness(2);
                    break;
                case TextBox textBox:
                    textBox.BorderBrush = Brushes.Red;
                    textBox.BorderThickness = new Thickness(2);
                    break;
                case ComboBox comboBox:
                    comboBox.BorderBrush = Brushes.Red;
                    comboBox.BorderThickness = new Thickness(2);
                    break;
            }

            // Tooltip con mensaje de error
            control.ToolTip = mensaje;

            // Mostrar ⚠ al lado
            var labelName = control.Name + "_Error";
            var lbl = this.FindName(labelName) as Label;
            if (lbl != null) lbl.Content = "⚠";
        }

        private void LimpiarError(FrameworkElement control)
        {
            // Restaurar borde
            switch (control)
            {
                case Border border:
                    border.BorderBrush = Brushes.Gray;
                    border.BorderThickness = new Thickness(1);
                    break;
                case TextBox textBox:
                    textBox.BorderBrush = Brushes.Gray;
                    textBox.BorderThickness = new Thickness(1);
                    break;
                case ComboBox comboBox:
                    comboBox.BorderBrush = Brushes.Gray;
                    comboBox.BorderThickness = new Thickness(1);
                    break;
            }

            // Quitar tooltip
            control.ToolTip = null;

            // Quitar ⚠
            var labelName = control.Name + "_Error";
            var lbl = this.FindName(labelName) as Label;
            if (lbl != null) lbl.Content = "";
        }

        private void ValidarTexto(object sender, TextChangedEventArgs e)
        {
            var txt = sender as TextBox;
            string valor = txt.Text.Trim();

            if (string.IsNullOrWhiteSpace(valor))
            {
                MarcarError(txt, "Este campo es obligatorio.");
            }
            else if (!Regex.IsMatch(valor, @"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$"))
            {
                MarcarError(txt, "Solo se permiten letras y espacios.");
            }
            else if (valor.Length < 2 || valor.Length > 50)
            {
                MarcarError(txt, "Debe tener entre 2 y 50 caracteres.");
            }
            else
            {
                LimpiarError(txt);
            }
        }

        private void ValidarTelefono(object sender, TextChangedEventArgs e)
        {
            var txt = sender as TextBox;
            string valor = txt.Text.Trim();

            if (string.IsNullOrWhiteSpace(valor))
            {
                MarcarError(txt, "Teléfono obligatorio.");
            }
            else if (!Regex.IsMatch(valor, @"^\d{10}$"))
            {
                MarcarError(txt, "Debe ser un número de 10 dígitos.");
            }
            else
            {
                LimpiarError(txt);
            }
        }

        private void ValidarFechaNacimiento(object sender, SelectionChangedEventArgs e)
        {
            if (dp_fechaNaciemiento.SelectedDate == null)
            {
                MarcarError(borderFechaNacimiento, "La fecha de nacimiento es obligatoria.");
                return;
            }

            DateTime fecha = dp_fechaNaciemiento.SelectedDate.Value;
            DateTime hoy = DateTime.Today;

            if (fecha > hoy)
            {
                MarcarError(borderFechaNacimiento, "La fecha no puede ser futura.");
                return;
            }

            int edad = hoy.Year - fecha.Year;
            if (fecha > hoy.AddYears(-edad)) edad--;

            int edadMinima = 10;
            int edadMaxima = 120;

            if (edad < edadMinima)
            {
                MarcarError(borderFechaNacimiento, $"El paciente debe tener al menos {edadMinima} años.");
            }
            else if (edad > edadMaxima)
            {
                MarcarError(borderFechaNacimiento, $"El paciente no puede tener más de {edadMaxima} años.");
            }
            else
            {
                LimpiarError(borderFechaNacimiento);
            }
        }

        private void ValidarDireccion(object sender, TextChangedEventArgs e)
        {
            var txt = sender as TextBox;
            string valor = txt.Text.Trim();

            if (string.IsNullOrWhiteSpace(valor))
            {
                MarcarError(txt, "La dirección es obligatoria.");
            }
            else if (valor.Length < 5 || valor.Length > 100)
            {
                MarcarError(txt, "Debe tener entre 5 y 100 caracteres.");
            }
            else if (!Regex.IsMatch(valor, @"^[a-zA-Z0-9áéíóúÁÉÍÓÚñÑ\s\#\-\.,]+$"))
            {
                MarcarError(txt, "Dirección con caracteres inválidos.");
            }
            else
            {
                LimpiarError(txt);
            }
        }

        private void ValidarCombo(object sender, SelectionChangedEventArgs e)
        {
            string valor = cb_generoPaciente.SelectedItem?.ToString();

            if (string.IsNullOrWhiteSpace(valor))
            {
                MarcarError(borderGenero, "Seleccione un género.");
            }
            else if (valor != "Masculino" && valor != "Femenino")
            {
                MarcarError(borderGenero, "Seleccione un género válido.");
            }
            else
            {
                LimpiarError(borderGenero);
            }
        }

        private void ValidarFotoClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (imgFotoPaciente.Source == null)
                MarcarError(borderFotoPaciente, "Seleccione una foto del paciente.");
            else
                LimpiarError(borderFotoPaciente);
        }

        private bool ValidarTodo()
        {
            ValidarTexto(txt_nombrePaciente, null);
            ValidarTexto(txt_apellidoPaciente, null);
            ValidarTelefono(txt_numTelefono, null);
            ValidarFechaNacimiento(dp_fechaNaciemiento, null);
            ValidarDireccion(txt_direccion, null);
            ValidarCombo(cb_generoPaciente, null);

            if (imgFotoPaciente.Source == null)
                MarcarError(borderFotoPaciente, "Seleccione una foto del paciente.");
            else
                LimpiarError(borderFotoPaciente);

            // Revisar si algún control sigue con borde rojo
            bool valido = true;
            foreach (var ctrl in new FrameworkElement[]
                     { txt_nombrePaciente, txt_apellidoPaciente, txt_numTelefono, dp_fechaNaciemiento, txt_direccion, cb_generoPaciente, borderFotoPaciente })
            {
                if (ctrl is Control c && c.BorderBrush == Brushes.Red)
                {
                    valido = false;
                    break;
                }
                else if (ctrl is Border b && b.BorderBrush == Brushes.Red)
                {
                    valido = false;
                    break;
                }
            }
            return valido;
        }

        #endregion

        #region Registrar Paciente
        private void btn_registrarPaciente_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidarTodo())
            {
                MessageBox.Show("Corrija los errores antes de continuar.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            Paciente paciente = new Paciente
            {
                IdPaciente = idPaciente ?? 0,
                Nombre = txt_nombrePaciente.Text.Trim(),
                Apellido = txt_apellidoPaciente.Text.Trim(),
                Telefono = txt_numTelefono.Text.Trim(),
                Genero = cb_generoPaciente.SelectedItem.ToString(),
                FechaNacimiento = dp_fechaNaciemiento.SelectedDate.Value,
                Direccion = txt_direccion.Text.Trim(),
                AntecedentesMedicos = string.IsNullOrWhiteSpace(txt_antecedentesMedicos.Text)
                        ? "Sin antecedentes médicos"
                        : txt_antecedentesMedicos.Text.Trim(),

                FotoPaciente = ObtenerFotoComoBytes()
            };

            try
            {
                PacienteDAO dao = new PacienteDAO();
                bool exito = idPaciente.HasValue
                    ? dao.ActualizarPaciente(paciente) // Edición
                    : dao.RegistrarPaciente(paciente); // Nuevo registro

                if (exito)
                {
                    MessageBox.Show(idPaciente.HasValue
                                        ? "Paciente actualizado exitosamente."
                                        : "Paciente registrado exitosamente.",
                                    "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);

                    if (!idPaciente.HasValue)
                        LimpiarFormulario(); // Limpiar solo si es registro nuevo
                }
                else
                {
                    MessageBox.Show("No se pudo guardar el paciente.", "Error",
                                    MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                // Captura errores inesperados
                MessageBox.Show($"Ocurrió un error al guardar el paciente: {ex.Message}", "Error",
                                MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        #endregion

        private void LimpiarFormulario()
        {
            txt_nombrePaciente.Clear();
            txt_apellidoPaciente.Clear();
            txt_numTelefono.Clear();
            cb_generoPaciente.SelectedIndex = -1;
            dp_fechaNaciemiento.SelectedDate = null;
            txt_direccion.Clear();
            txt_antecedentesMedicos.Clear();
            imgFotoPaciente.Source = null;
            placeholderFoto.Visibility = Visibility.Visible;
        }
        
    }
}
