using OdontoNova;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;

namespace OdontoNovaWPF_FIS
{
    /// <summary>
    /// Lógica de interacción para UC_Expediente.xaml
    /// </summary>
    public partial class UC_Expediente : UserControl
    {
        public Expedientes expedienteGuardado;
        public Paciente pacienteGuardado;

        public event EventHandler<Paciente> EditarExpedienteClick;
        public event EventHandler<Paciente> VerExpedienteClick;
        public event EventHandler<Paciente> VerHistorialClick;


        public UC_Expediente()
        {
            InitializeComponent();
        }

        public void CargarDatos(Expedientes expediente, Paciente paciente)
        {
            expedienteGuardado = expediente;
            pacienteGuardado = paciente;

            // Mostrar datos
            lb_idExpediente.Text = expediente.IdExpediente.ToString();
            lb_NombrePaciente.Text = paciente.Nombre;
            lb_ApellidoPaciente.Text = paciente.Apellido;
            lb_TelefonoPaciente.Text = paciente.Telefono;
            lb_FechaRegistro.Text = expediente.FechaApertura.ToShortDateString();

            Btn_EditarExpediente.ToolTip = "Editar Expediente";
            Btn_VerExpediente.ToolTip = "Ver Expediente";
            Btn_VerHistorial.ToolTip = "Ver Historial del paciente";

            if (paciente.EstadoPaciente)
            {
                pbx_activo.Source = new BitmapImage(new Uri("pack://application:,,,/Assets/Images/banderaActivo.png"));
                pbx_activo.ToolTip = "Activo";
            }
            else
            {
                pbx_activo.Source = new BitmapImage(new Uri("pack://application:,,,/Assets/Images/banderaInactivo.png"));
                pbx_activo.ToolTip = "Inactivo";
            }

            // Cargar imagen
            if (paciente.FotoPaciente != null && paciente.FotoPaciente.Length > 0)
            {
                FotoPacienteEllipse.Fill = new ImageBrush(ByteArrayToImageSource(paciente.FotoPaciente));
            }
            else
            {
                // Imagen por defecto
                FotoPacienteEllipse.Fill = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/Assets/Images/user_456212.png")));
            }
        }

        private void Btn_EditarExpediente_Click(object sender, RoutedEventArgs e)
        {
            EditarExpedienteClick?.Invoke(this, pacienteGuardado);
        }

        private void Btn_VerExpediente_Click(object sender, RoutedEventArgs e)
        {
            VerExpedienteClick?.Invoke(this, pacienteGuardado);
        }

        private void Btn_VerHistorial_Click(object sender, RoutedEventArgs e)
        {
            VerHistorialClick?.Invoke(this, pacienteGuardado);
        }


        private ImageSource ByteArrayToImageSource(byte[] bytes)
        {
            using (var ms = new MemoryStream(bytes))
            {
                var image = new BitmapImage();
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.StreamSource = ms;
                image.EndInit();
                return image;
            }
        }
    }
}
