using Microsoft.Win32;
using System;
using System.Collections.Generic;
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
    /// Lógica de interacción para Citas.xaml
    /// </summary>
    public partial class Citas : Window
    {
        private Window formAnterior;

        private int? idCitaEditar = null;

        public Citas(Window formAnterior, int? idCita = null)
        {
            InitializeComponent();
            this.formAnterior = formAnterior;
            this.idCitaEditar = idCita;

            if (this.formAnterior.WindowState == WindowState.Maximized)
            {
                this.WindowState = WindowState.Maximized;
            }
            else
            {
                this.WindowState = WindowState.Normal;
            }

            // Inicializar combos
            cb_tratamiento.ItemsSource = new List<string> { "Colocacion", "Ajuste", "Retiro de brackets" };
            LlenarHorasCita();

            if (idCitaEditar.HasValue)
            {
                CargarDatosCita(idCitaEditar.Value);
            }
        }

        private void CargarDatosCita(int idCita)
        {
            CitasDAO citaDAO = new CitasDAO();
            Cita cita = citaDAO.ObtenerCitaPorId(idCita);

            if (cita == null)
            {
                MessageBox.Show("No se encontró la cita seleccionada.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                this.Close();
                return;
            }

            PacienteDAO pacienteDAO = new PacienteDAO();
            var datos = pacienteDAO.ObtenerNombreYApellidoPorId(cita.IdPaciente);

            txt_nombrePaciente.Text = datos.Nombre;
            txt_apellidoPaciente.Text = datos.Apellido;
            cb_tratamiento.Text = cita.TipoTratamiento;

            // Bloquear campos que no se pueden editar
            txt_nombrePaciente.IsEnabled = false;
            txt_apellidoPaciente.IsEnabled = false;

            // Campos editables
            dp_fechaCita.SelectedDate = cita.FechaCita;
            cb_horaCita.Text = cita.HoraCita.ToString(@"hh\:mm");
            cb_dentistaCita.Text = cita.Dentista;

            // Cambiar texto del botón para guardar
            btn_guardarCita.Content = "Actualizar Cita";
        }



        #region Manejo ventana
        private void Header_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Middle)
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
            formAnterior.Show();
            this.Close();
        }
        #endregion

        #region Validaciones
        private void MarcarError(FrameworkElement control, string mensaje)
        {
            switch (control)
            {
                case TextBox tb:
                    tb.BorderBrush = Brushes.Red;
                    tb.BorderThickness = new Thickness(2);
                    tb.ToolTip = mensaje;
                    break;
                case ComboBox cb:
                    cb.BorderBrush = Brushes.Red;
                    cb.BorderThickness = new Thickness(2);
                    cb.ToolTip = mensaje;
                    break;
                case Border border:
                    border.BorderBrush = Brushes.Red;
                    border.BorderThickness = new Thickness(2);
                    break;
                case DatePicker dp:
                    dp.BorderBrush = Brushes.Red;
                    dp.BorderThickness = new Thickness(2);
                    dp.ToolTip = mensaje;
                    break;
            }
        }

        private void LimpiarError(FrameworkElement control)
        {
            switch (control)
            {
                case TextBox tb:
                    tb.BorderBrush = Brushes.Gray;
                    tb.BorderThickness = new Thickness(1);
                    tb.ToolTip = null;
                    break;
                case ComboBox cb:
                    cb.BorderBrush = Brushes.Gray;
                    cb.BorderThickness = new Thickness(1);
                    cb.ToolTip = null;
                    break;
                case Border border:
                    border.BorderBrush = Brushes.Gray;
                    border.BorderThickness = new Thickness(1);
                    break;
                case DatePicker dp:
                    dp.BorderBrush = Brushes.Gray;
                    dp.BorderThickness = new Thickness(1);
                    dp.ToolTip = null;
                    break;
            }
        }


        private void txt_nombrePaciente_LostFocus(object sender, RoutedEventArgs e)
        {
            string valor = txt_nombrePaciente.Text.Trim();
            if (string.IsNullOrWhiteSpace(valor))

                MarcarError(txt_nombrePaciente, "Favor de ingresar el nombre");
            else if (Regex.IsMatch(valor, @"\d"))
                MarcarError(txt_nombrePaciente, "El nombre no puede contener números");
            else
                LimpiarError(txt_nombrePaciente);
        }

        private void txt_apellidoPaciente_LostFocus(object sender, RoutedEventArgs e)
        {
            string valor = txt_apellidoPaciente.Text.Trim();
            if (string.IsNullOrWhiteSpace(valor))
                MarcarError(txt_apellidoPaciente, "Favor de ingresar el apellido");
            else if (Regex.IsMatch(valor, @"\d"))
                MarcarError(txt_apellidoPaciente, "El apellido no puede contener números");
            else
                LimpiarError(txt_apellidoPaciente);
        }

        private void cb_tratamiento_LostFocus(object sender, RoutedEventArgs e)
        {
            var opciones = new List<string> { "Colocacion", "Ajuste", "Retiro de brackets" };
            if (string.IsNullOrWhiteSpace(cb_tratamiento.Text))
                MarcarError(borderTratamiento, "Seleccione un tratamiento.");
            else if (!opciones.Contains(cb_tratamiento.Text))
                MarcarError(borderTratamiento, "Seleccione un tratamiento válido.");
            else
                LimpiarError(borderTratamiento);
        }

        private void cb_dentistaCita_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(cb_dentistaCita.Text))
                MarcarError(borderDentistaCita, "Seleccione un dentista");
            else
                LimpiarError(borderDentistaCita);
        }

        private void dp_fechaCita_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!dp_fechaCita.SelectedDate.HasValue)
                MarcarError(dp_fechaCita, "Seleccione una fecha");
            else if (dp_fechaCita.SelectedDate.Value < DateTime.Today)
                MarcarError(dp_fechaCita, "La fecha debe ser igual o posterior a hoy");
            else
                LimpiarError(dp_fechaCita);
        }

        private void cb_horaCita_LostFocus(object sender, RoutedEventArgs e)
        {
            if (TimeSpan.TryParse(cb_horaCita.Text, out TimeSpan hora))
            {
                if ((hora < new TimeSpan(8, 0, 0)) ||
                    (hora > new TimeSpan(14, 0, 0) && hora < new TimeSpan(15, 0, 0)) ||
                    hora > new TimeSpan(18, 30, 0))
                    MarcarError(borderHoraCita, "Horario de atención: 8:00–14:00 y 15:00–18:30");
                else
                    LimpiarError(borderHoraCita);
            }
            else
            {
                MarcarError(borderHoraCita, "Hora inválida");
            }
        }

        private void txt_nombrePaciente_TextChanged(object sender, TextChangedEventArgs e)
        {
            string texto = txt_nombrePaciente.Text.Trim();
            if (string.IsNullOrEmpty(texto))
            {
                popupSugerenciasNombre.IsOpen = false;
                txtPlaceholderNombre.Visibility = Visibility.Visible;
                return;
            }

            txtPlaceholderNombre.Visibility = Visibility.Collapsed;

            var dao = new Expedientes();
            var sugerencias = dao.ObtenerSugerencias("Nombre", texto);

            if (sugerencias.Count > 0)
            {
                lbxSugerenciasNombre.ItemsSource = sugerencias;
                lbxSugerenciasNombre.SelectedIndex = 0;
                popupSugerenciasNombre.IsOpen = true;
            }
            else
            {
                popupSugerenciasNombre.IsOpen = false;
            }
        }

        private void lbxSugerenciasNombre_MouseClick(object sender, MouseButtonEventArgs e)
        {
            if (lbxSugerenciasNombre.SelectedItem != null)
            {
                txt_nombrePaciente.Text = lbxSugerenciasNombre.SelectedItem.ToString();
                txt_nombrePaciente.CaretIndex = txt_nombrePaciente.Text.Length;
                popupSugerenciasNombre.IsOpen = false;
            }
        }

        private void lbxSugerenciasNombre_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && lbxSugerenciasNombre.SelectedItem != null)
            {
                txt_nombrePaciente.Text = lbxSugerenciasNombre.SelectedItem.ToString();
                txt_nombrePaciente.CaretIndex = txt_nombrePaciente.Text.Length;
                popupSugerenciasNombre.IsOpen = false;
            }
        }

        private void txt_nombrePaciente_KeyDown(object sender, KeyEventArgs e)
        {
            if (popupSugerenciasNombre.IsOpen)
            {
                if (e.Key == Key.Down)
                {
                    lbxSugerenciasNombre.Focus();
                    lbxSugerenciasNombre.SelectedIndex = 0;
                }
            }
        }

        private void txt_nombrePaciente_GotFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txt_nombrePaciente.Text))
                txtPlaceholderNombre.Visibility = Visibility.Visible;
        }

        private void txt_apellidoPaciente_TextChanged(object sender, TextChangedEventArgs e)
        {
            string texto = txt_apellidoPaciente.Text.Trim();
            if (string.IsNullOrEmpty(texto))
            {
                popupSugerenciasApellido.IsOpen = false;
                txtPlaceholderApellido.Visibility = Visibility.Visible;
                return;
            }

            txtPlaceholderApellido.Visibility = Visibility.Collapsed;

            var dao = new Expedientes();
            var sugerencias = dao.ObtenerSugerencias("Apellido", texto);

            if (sugerencias.Count > 0)
            {
                lbxSugerenciasApellido.ItemsSource = sugerencias;
                lbxSugerenciasApellido.SelectedIndex = 0;
                popupSugerenciasApellido.IsOpen = true;
            }
            else
            {
                popupSugerenciasApellido.IsOpen = false;
            }
        }

        private void lbxSugerenciasApellido_MouseClick(object sender, MouseButtonEventArgs e)
        {
            if (lbxSugerenciasApellido.SelectedItem != null)
            {
                txt_apellidoPaciente.Text = lbxSugerenciasApellido.SelectedItem.ToString();
                txt_apellidoPaciente.CaretIndex = txt_apellidoPaciente.Text.Length;
                popupSugerenciasApellido.IsOpen = false;
            }
        }

        private void lbxSugerenciasApellido_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && lbxSugerenciasApellido.SelectedItem != null)
            {
                txt_apellidoPaciente.Text = lbxSugerenciasApellido.SelectedItem.ToString();
                txt_apellidoPaciente.CaretIndex = txt_apellidoPaciente.Text.Length;
                popupSugerenciasApellido.IsOpen = false;
            }
        }

        private void txt_apellidoPaciente_KeyDown(object sender, KeyEventArgs e)
        {
            if (popupSugerenciasApellido.IsOpen)
            {
                if (e.Key == Key.Down)
                {
                    lbxSugerenciasApellido.Focus();
                    lbxSugerenciasApellido.SelectedIndex = 0;
                }
            }
        }

        private void txt_apellidoPaciente_GotFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txt_apellidoPaciente.Text))
                txtPlaceholderApellido.Visibility = Visibility.Visible;
        }

        #endregion

        #region Guardar cita
        private void btn_guardarCita_Click(object sender, RoutedEventArgs e)
        {
            // Ejecutar todas las validaciones existentes
            txt_nombrePaciente_LostFocus(null, null);
            txt_apellidoPaciente_LostFocus(null, null);
            cb_tratamiento_LostFocus(null, null);
            cb_dentistaCita_LostFocus(null, null);
            dp_fechaCita_LostFocus(null, null);
            cb_horaCita_LostFocus(null, null);

            bool hayErrores = new FrameworkElement[] { txt_nombrePaciente, txt_apellidoPaciente, cb_tratamiento, cb_dentistaCita, dp_fechaCita, cb_horaCita }
                .Any(c => c.ToolTip != null);

            if (hayErrores)
            {
                MessageBox.Show("Corrija los errores antes de continuar.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // datos editables
            TimeSpan horaSeleccionada = TimeSpan.Parse(cb_horaCita.Text);
            string turno = ObtenerTurnoDesdeHora(horaSeleccionada);

            if (turno == "Fuera de horario")
            {
                MessageBox.Show("La hora seleccionada está fuera del horario permitido (08:00–14:00 y 15:00–18:30).", "Horario inválido", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            CitasDAO citaDAO = new CitasDAO();

            // Verificar si estamos en modo edición
            if (idCitaEditar.HasValue)
            {
                // Obtener la cita actual
                Cita cita = citaDAO.ObtenerCitaPorId(idCitaEditar.Value);

                if (cita == null)
                {
                    MessageBox.Show("No se encontró la cita seleccionada.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Solo modificar los campos editables
                cita.FechaCita = dp_fechaCita.SelectedDate.Value;
                cita.HoraCita = horaSeleccionada;
                cita.Dentista = cb_dentistaCita.Text;
                cita.Turno = turno;
                cita.TipoTratamiento = cb_tratamiento.Text;

                // Guardar cambios
                bool actualizado = citaDAO.ActualizarCitaYHistorial(cita);

                if (actualizado)
                    MessageBox.Show("Cita actualizada correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                else
                    MessageBox.Show("Error al actualizar la cita.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                // nueva cita: buscar paciente por nombre
                PacienteDAO pacienteDAO = new PacienteDAO();
                int idPaciente = pacienteDAO.ObtenerIdPorNombreCompleto(txt_nombrePaciente.Text.Trim(), txt_apellidoPaciente.Text.Trim());

                if (idPaciente == -1)
                {
                    MessageBox.Show("El paciente no existe", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Validar que no se pueda agendar en hora pasada del mismo día
                if (dp_fechaCita.SelectedDate.HasValue && dp_fechaCita.SelectedDate.Value.Date == DateTime.Today)
                {
                    if (horaSeleccionada < DateTime.Now.TimeOfDay)
                    {
                        MessageBox.Show("No se puede agendar una cita en una hora que ya pasó hoy.", "Hora inválida", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                }


                // Revisar última cita
                DateTime? ultimaCita = citaDAO.ObtenerUltimaFechaCita(idPaciente);
                if (ultimaCita != null)
                {
                    DateTime siguienteValida = ultimaCita.Value.AddMonths(1);
                    MessageBox.Show(
                        $"Última cita registrada: {ultimaCita:dd/MM/yyyy}\nSiguiente fecha válida para agendar: {siguienteValida:dd/MM/yyyy}",
                        "Información de citas anteriores", MessageBoxButton.OK, MessageBoxImage.Information
                    );
                }
                else
                {
                    MessageBox.Show("Este paciente no tiene citas registradas aún.", "Primera cita", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                // Crear objeto nueva cita
                Cita cita = new Cita
                {
                    IdPaciente = idPaciente,
                    FechaCita = dp_fechaCita.SelectedDate.Value,
                    HoraCita = horaSeleccionada,
                    Turno = turno,
                    Estado = "Pendiente",
                    TipoTratamiento = cb_tratamiento.Text,
                    Observaciones = "",
                    Dentista = cb_dentistaCita.Text
                };

                // Agendar cita
                int resultado = citaDAO.AgendarCita(cita);
                MostrarResultado(resultado);
            
            }
        }

        private void MostrarResultado(int codigo)
        {
            switch (codigo)
            {
                case 0:
                    MessageBox.Show("Cita agendada exitosamente", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                    break;
                case 2:
                    MessageBox.Show("El paciente no existe", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    break;
                case 3:
                    MessageBox.Show("Ya existe una cita en esa fecha y hora", "Conflicto", MessageBoxButton.OK, MessageBoxImage.Warning);
                    break;
                case 4:
                    MessageBox.Show("El paciente ya tiene una cita reciente.\nDebe esperar al menos un mes desde la última cita.", "Fecha inválida", MessageBoxButton.OK, MessageBoxImage.Warning);
                    break;
                case 99:
                    MessageBox.Show("Error al agendar la cita", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    break;
                default:
                    MessageBox.Show("Resultado desconocido", "Aviso", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    break;
            }
        }

        private string ObtenerTurnoDesdeHora(TimeSpan hora)
        {
            if (hora >= new TimeSpan(8, 0, 0) && hora <= new TimeSpan(14, 0, 0))
                return "Mañana";
            else if (hora >= new TimeSpan(15, 0, 0) && hora <= new TimeSpan(18, 30, 0))
                return "Tarde";
            else
                return "Fuera de horario";
        }

        private void LlenarHorasCita()
        {
            List<string> horasDisponibles = new List<string>();

            // Horario de mañana
            TimeSpan inicioManana = new TimeSpan(8, 0, 0);
            TimeSpan finManana = new TimeSpan(14, 0, 0);

            // Horario de tarde
            TimeSpan inicioTarde = new TimeSpan(15, 0, 0);
            TimeSpan finTarde = new TimeSpan(18, 30, 0);

            TimeSpan intervalo = new TimeSpan(0, 10, 0); // 10 minutos

            // Llenar mañana
            for (TimeSpan hora = inicioManana; hora <= finManana; hora += intervalo)
            {
                horasDisponibles.Add(hora.ToString(@"hh\:mm"));
            }

            // Llenar tarde
            for (TimeSpan hora = inicioTarde; hora <= finTarde; hora += intervalo)
            {
                horasDisponibles.Add(hora.ToString(@"hh\:mm"));
            }

            cb_horaCita.ItemsSource = horasDisponibles;
        }

        #endregion

    }
}
