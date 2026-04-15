using Microsoft.Win32;
//using PdfSharp.Drawing;
//using PdfSharp.Pdf;
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

namespace OdontoNova
{
    /// <summary>
    /// Lógica de interacción para ExpedientePaciente.xaml
    /// </summary>
    public partial class ExpedientePaciente : Window
    {
        private Window formAnterior;
        private Paciente pacienteActual;
        private Expedientes expedienteActual;

        public ExpedientePaciente(Window anterior, Paciente paciente, Expedientes expediente)
        {
            InitializeComponent();
            formAnterior = anterior;
            pacienteActual = paciente;
            expedienteActual = expediente;

            CargarDatosExpediente();

            if (formAnterior.WindowState == WindowState.Maximized)
            {
                this.WindowState = WindowState.Maximized;
            }
            else
            {
                this.WindowState = WindowState.Normal;
            }
        }

        private void CargarDatosExpediente()
        {
            // Datos del expediente
            txtb_idExpediente.Text = expedienteActual.IdExpediente.ToString();
            txtb_fechaRegistro.Text = expedienteActual.FechaApertura.ToShortDateString();

            // Datos del paciente
            txtb_nombrePaciente.Text = pacienteActual.Nombre;
            txtb_apellidos.Text = pacienteActual.Apellido;
            txtb_numTelefono.Text = pacienteActual.Telefono;
            txtb_genero.Text = pacienteActual.Genero;
            txtb_fechaNacimiento.Text = pacienteActual.FechaNacimiento.ToShortDateString();
            txtb_direccion.Text = pacienteActual.Direccion;
            txtb_antecedentes.Text = pacienteActual.AntecedentesMedicos;
            txtEstadoPaciente.Text = pacienteActual.EstadoPaciente ? "Activo" : "Inactivo";

            txtb_edad.Text = CalcularEdad(pacienteActual.FechaNacimiento).ToString();

            // Cargar foto
            if (pacienteActual.FotoPaciente != null)
            {
                using (MemoryStream ms = new MemoryStream(pacienteActual.FotoPaciente))
                {
                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.StreamSource = ms;
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();
                    imgFotoPaciente.Source = bitmap;
                }
            }
            else
            {
                imgFotoPaciente.Source = new BitmapImage(new Uri("pack://application:,,,/Assets/Images/user_456212.png"));
            }
        }

        private int CalcularEdad(DateTime fechaNacimiento)
        {
            DateTime hoy = DateTime.Today;
            int edad = hoy.Year - fechaNacimiento.Year;

            // Ajustar si no ha cumplido años este año
            if (fechaNacimiento.Date > hoy.AddYears(-edad))
                edad--;

            return edad;
        }

        private void btn_regresar_Click(object sender, RoutedEventArgs e)
        {
            formAnterior.Show();
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

        private void ExportarVisualA_PDF()
        {
            try
            {
                SaveFileDialog saveDialog = new SaveFileDialog()
                {
                    Title = "Guardar expediente como PDF",
                    Filter = "Archivo PDF (*.pdf)|*.pdf",
                    FileName = $"Expediente_{pacienteActual.Nombre}_{pacienteActual.Apellido}.pdf"
                };

                if (saveDialog.ShowDialog() != true)
                    return;

                // Renderizar contenedor a imagen
                double dpi = 120;
                Rect bounds = VisualTreeHelper.GetDescendantBounds(ExpedienteContainer);

                RenderTargetBitmap rtb = new RenderTargetBitmap(
                    (int)(bounds.Width * dpi / 96),
                    (int)(bounds.Height * dpi / 96),
                    dpi,
                    dpi,
                    PixelFormats.Pbgra32);

                DrawingVisual dv = new DrawingVisual();
                using (DrawingContext dc = dv.RenderOpen())
                {
                    VisualBrush vb = new VisualBrush(ExpedienteContainer);
                    dc.DrawRectangle(vb, null, new Rect(new Point(), bounds.Size));
                }
                rtb.Render(dv);

                // Convertir a PNG
                PngBitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(rtb));

                byte[] imageBytes;
                using (MemoryStream ms = new MemoryStream())
                {
                    encoder.Save(ms);
                    imageBytes = ms.ToArray();
                }

                //// Crear PDF
                //PdfDocument document = new PdfDocument();
                //PdfPage page = document.AddPage();
                //XGraphics gfx = XGraphics.FromPdfPage(page);

                //XImage img;
                //using (MemoryStream msImg = new MemoryStream(imageBytes))
                //{
                //    img = XImage.FromStream(msImg);
                //}

                //// Ajustar a la página
                //double pageWidthPt = page.Width.Point;  
                //double scale = (pageWidthPt - 40) / img.PointWidth;
                //double imgWidth = img.PointWidth * scale;
                //double imgHeight = img.PointHeight * scale;

                //gfx.DrawImage(img, 20, 20, imgWidth, imgHeight);

                //// Guardar PDF
                //document.Save(saveDialog.FileName);

                //MessageBox.Show("Expediente exportado exitosamente.",
                //                "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al exportar el expediente:\n" + ex.Message,
                                "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void btn_exportarExpediente_Click(object sender, RoutedEventArgs e)
        {
            ExportarVisualA_PDF();
        }

    }
}
