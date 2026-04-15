using OdontoNova;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SistemaOdontonova.Interfaces
{
    /// <summary>
    /// Lógica de interacción para CalendarioCitaDiaria.xaml
    /// </summary>
    public partial class CalendarioCitaDiaria : Window
    {
        Button botonSeleccionado = null; //para las citas en turno
        DateTime inicioSemanActual;     
        DateTime vistaMesActual; 
        DateTime fechaSeleccionada;
        readonly CultureInfo cultInfoES = new CultureInfo("es-ES");
        bool seleccionDia = false;

        private Window formAnterior;

        public CalendarioCitaDiaria(Window anterior)
        {
            InitializeComponent();
           formAnterior = anterior;

            MostrarCitasPorDia(DateTime.Today, null);

            if(formAnterior.WindowState == WindowState.Maximized)
            {
                this.WindowState = WindowState.Maximized;
            }
            else
            {
                this.WindowState = WindowState.Normal;
            }
        }


        private void MostrarCitasPorDia(DateTime fecha, string turnoFiltro)
        {
            TarjetasPanel.Children.Clear();  // Limpiar las tarjetas previas

            CitasDAO citas = new CitasDAO();
            // Obtener las citas de la base de datos
            var citasDelDia = citas.ObtenerCitasPorDia(fecha, turnoFiltro);

            // Iterar sobre las citas y crear un UC_EventoCitaIndividual para cada una
            foreach (var cita in citasDelDia)
            {
                UC_EventoCitaIndividual tarjeta = new UC_EventoCitaIndividual();

                // Cargar los datos de la cita en el UserControl
                tarjeta.CargarCita(cita);

                // Agregar la tarjeta al panel
                TarjetasPanel.Children.Add(tarjeta);
            }
        }

        private void CalendarioCitaDiaria_Loaded(object sender, RoutedEventArgs e)
        {
            DateTime hoy = DateTime.Today;
            vistaMesActual = new DateTime(hoy.Year, hoy.Month, 1);
            inicioSemanActual = IniciarSemana(hoy, DayOfWeek.Monday); //iniciar en lunes
            fechaSeleccionada = hoy;
            Lb_DiaNumero.Text = hoy.Day.ToString();
            Lb_MesNombre.Text = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(cultInfoES.DateTimeFormat.GetMonthName(hoy.Month));
            Lb_DiaSemana.Text = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(cultInfoES.DateTimeFormat.GetDayName(hoy.DayOfWeek));
            seleccionDia = false;
            ActualizarLabelMes();
            ActualizarBotonesDias();
            ActualizarPanelEstadisticas(fechaSeleccionada);
        }

        #region Navegación Mes

        private void CambiarMes(int incremento)
        {
            vistaMesActual = vistaMesActual.AddMonths(incremento);
            // cuando cambie el mes la vista de semana cambia a la primera semana que contenga el dia 1 del mes
            inicioSemanActual = IniciarSemana(new DateTime(vistaMesActual.Year, vistaMesActual.Month, 1), DayOfWeek.Monday);
            fechaSeleccionada = inicioSemanActual;
            ActualizarLabelMes();
            ActualizarBotonesDias();
            ActualizarPanelEstadisticas(fechaSeleccionada);
        }

        private void Btn_MesAnterior_Click(object sender, RoutedEventArgs e)
        {
            CambiarMes(-1);
        }

        private void Btn_MesSiguiente_Click(object sender, RoutedEventArgs e)
        {
            CambiarMes(1);
        }
        #endregion


        #region Navegación Semana
        private void CambiarSemana(int dias)
        {
            inicioSemanActual = inicioSemanActual.AddDays(dias);
            vistaMesActual = new DateTime(inicioSemanActual.Year, inicioSemanActual.Month, 1);
            // si nueva semana tiene fecha de hoy se vuelve a seleccionar automaticamente
            if (DateTime.Today >= inicioSemanActual && DateTime.Today < inicioSemanActual.AddDays(7))
            {
                fechaSeleccionada = DateTime.Today;
                seleccionDia = false;
            }
            // si la fecha seleccionada actual no esta en la semana se mueve la selección al primer dia
            else if (fechaSeleccionada < inicioSemanActual || fechaSeleccionada >= inicioSemanActual.AddDays(7))
            {
                fechaSeleccionada = inicioSemanActual;
                seleccionDia = false;
            }
            ActualizarLabelMes();
            ActualizarBotonesDias();
            ActualizarPanelEstadisticas(fechaSeleccionada);
        }

        private void Btn_SemanaAnterior_Click(object sender, RoutedEventArgs e)
        {
            CambiarSemana(-7);
        }

        private void Btn_SemanaSiguiente_Click(object sender, RoutedEventArgs e)
        {
            CambiarSemana(7);
        }
        #endregion


        #region ActualizarCalendarioUI
        private void ActualizarLabelMes()
        {
            string nombreMes = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(cultInfoES.DateTimeFormat.GetMonthName(vistaMesActual.Month));
            Lb_MesAño.Text = nombreMes +" "+ vistaMesActual.Year;
        }

        private void ActualizarBotonesDias()
        {
            bool esSemanaActual = DateTime.Today >= inicioSemanActual && DateTime.Today < inicioSemanActual.AddDays(7);

            string[] diasCortos = { "LUN", "MAR", "MIE", "JUE", "VIE", "SAB", "DOM" };

            for (int i = 0; i < 7; i++)
            {
                DateTime dia = inicioSemanActual.AddDays(i);
                TextBlock diaNombre = (TextBlock)FindName("DiaNombre" + i);
                TextBlock diaNumero = (TextBlock)FindName("DiaNumero" + i);
                Border border = (Border)FindName("Dia" + i);

                if (diaNombre != null)
                {
                    //0 es lunes y domigo es 6
                    int indiceDia = ((int)dia.DayOfWeek + 6) % 7;
                    diaNombre.Text = diasCortos[indiceDia];
                }

                if (diaNumero != null)
                    diaNumero.Text = dia.Day.ToString();

                if (border != null)
                {
                    //Caso cuando el dia es seleccionado por el usuario
                    if (fechaSeleccionada.Date == dia.Date)
                    {
                        border.Background = (Brush)new BrushConverter().ConvertFromString("#1E3A5F");
                        if (diaNombre != null) diaNombre.Foreground = Brushes.White;
                        if (diaNumero != null) diaNumero.Foreground = Brushes.White;
                    }
                    //Caso cuando es el dia actual (tmb semana actual) y no hay otro dia seleccionado
                    else if (!seleccionDia && esSemanaActual && dia.Date == DateTime.Today)
                    {
                        border.Background = (Brush)new BrushConverter().ConvertFromString("#2B6CB0");
                        if (diaNombre != null) diaNombre.Foreground = Brushes.White;
                        if (diaNumero != null) diaNumero.Foreground = Brushes.White;
                    }
                    //Caso de otros dias (no sleccionados)
                    else
                    {
                        border.ClearValue(Border.BackgroundProperty);
                        if (diaNombre != null) diaNombre.Foreground = (Brush)new BrushConverter().ConvertFromString("#64748B");
                        if (diaNumero != null) diaNumero.Foreground = (Brush)new BrushConverter().ConvertFromString("#1E293B");
                    }
                }
            }
            //actualizar etiqueta de semana
            int semanaNumero = cultInfoES.Calendar.GetWeekOfYear(inicioSemanActual, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
            Lb_SemanaActual.Text = "Semana " +semanaNumero;

            DateTime inicio = inicioSemanActual;
            DateTime fin = inicioSemanActual.AddDays(6);
            if (inicio.Month == fin.Month)
            {
                string mesNombre = cultInfoES.DateTimeFormat.GetMonthName(inicio.Month);
                Lb_RangoSemana.Text = "Del " +inicio.Day+ " al " +fin.Day+ " de " +mesNombre;
            }
            else
            {
                string mesInicio = cultInfoES.DateTimeFormat.GetMonthName(inicio.Month);
                string mesFin = cultInfoES.DateTimeFormat.GetMonthName(fin.Month);
                Lb_RangoSemana.Text = "Del " +inicio.Day +" de " +mesInicio+ " al " +fin.Day+ " de " +mesFin;
            }
        }

        //panel derecho con los datos
        private void ActualizarPanelEstadisticas(DateTime fecha)
        {
            CitasDAO citasDao = new CitasDAO();

            // Total de citas del día (sin filtrar turno)
            int totalDia = citasDao.ObtenerCitasPorDia(fecha, botonSeleccionado?.Tag?.ToString()).Count();
            Lb_TotalCitas.Text = $"Total de citas hoy: {totalDia}";

            // Total de citas de la semana (del lunes al domingo de la semana actual)
            int totalSemana = 0;
            for (int i = 0; i < 7; i++)
            {
                DateTime diaSemana = inicioSemanActual.AddDays(i);
                totalSemana += citasDao.ObtenerCitasPorDia(diaSemana, null).Count();
            }
            Lb_TotalCitasSemana.Text = $"Citas esta semana: {totalSemana}";
        }

        #endregion

        private DateTime IniciarSemana(DateTime fecha, DayOfWeek inicioSem)
        {
            int diferencia = (7 + (fecha.DayOfWeek - inicioSem)) % 7;
            return fecha.AddDays(-diferencia).Date;
        }

        private void Dia_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is Border borde)
            {
                string nombre = borde.Name;
                if (nombre.StartsWith("Dia") && int.TryParse(nombre.Substring(3), out int id))
                {
                    fechaSeleccionada = inicioSemanActual.AddDays(id);
                    seleccionDia = true;
                    ActualizarBotonesDias();
                    MostrarCitasPorDia(fechaSeleccionada, botonSeleccionado?.Tag?.ToString());
                    ActualizarPanelEstadisticas(fechaSeleccionada);
                }
            }
        }

        //aqui para filtrar las citas del dia por turno
        #region NavegacionCitas
        private void Btn_TurnoMatutino_Click(object sender, RoutedEventArgs e)
        {
            Btn_TurnoMatutino.Tag = "Mañana";
            SeleccionarTurno(Btn_TurnoMatutino);
        }
        private void Btn_TurnoVespertino_Click(object sender, RoutedEventArgs e)
        {
            Btn_TurnoVespertino.Tag = "Tarde";
            SeleccionarTurno(Btn_TurnoVespertino);
        }

        private void SeleccionarTurno(Button boton)
        {
            // Desmarcar el botón anterior
            if (botonSeleccionado != null)
            {
                botonSeleccionado.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E2E8F0"));
                botonSeleccionado.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1E3A5F"));
            }

            // Marcar el nuevo botón
            boton.Background = new LinearGradientBrush((Color)Color.FromRgb(39, 99, 204), (Color)Color.FromRgb(12, 145, 166), 90);
            boton.Foreground = Brushes.White;

            botonSeleccionado = boton;

            // Mostrar las citas filtradas por turno
            MostrarCitasPorDia(fechaSeleccionada, botonSeleccionado.Tag.ToString());
        }
        #endregion

        #region NavegacionVentana
        private void Header_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        private void Btn_Minimizar_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void Btn_Maximizar_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
                this.WindowState = WindowState.Normal;
            else
                this.WindowState = WindowState.Maximized;
        }

        private void Btn_Cerrar_Click(object sender, RoutedEventArgs e)
        {
            formAnterior.Show();
            this.Close();
        }
        #endregion
    }
}
