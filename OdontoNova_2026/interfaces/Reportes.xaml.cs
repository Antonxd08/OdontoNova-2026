


using Microsoft.Win32;
using OdontoNova.Clases;
//using PdfSharp.Drawing;
//using PdfSharp.Pdf;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
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
    /// Lógica de interacción para Reportes.xaml
    /// </summary>
    public partial class Reportes : Window
    {
        private Window formAnterior;
        private Usuario usuarioActual;
        public Reportes(Window formAnterior,  Usuario usuario)
        {
            InitializeComponent();
            this.formAnterior = formAnterior;
            this.WindowState = WindowState.Maximized;
            btn_exportarReporte.Visibility = Visibility.Collapsed;
            usuarioActual = usuario;
        }

        private void MostrarGraficaCitas(Reporte reporte)
        {
            // El panel ya está en XAML, solo llenamos datos
            panelReporteExport.Visibility = Visibility.Visible;

            // Textos
            txtPeriodoReporte.Text = $"Periodo: {reporte.PeriodoInicio:dd/MM/yyyy} - {reporte.PeriodoFin:dd/MM/yyyy}";
            txtFechaGeneracion.Text = reporte.FechaGeneracion.ToString("dd/MM/yyyy HH:mm");
            txtAsistidas.Text = reporte.CitasAsistidas.ToString();
            txtNoAsistidas.Text = reporte.CitasNoAsistidas.ToString();
            txtTotalPagos.Text = $"${reporte.TotalPagos:N2}";
            txtMayorDemanda.Text = reporte.DiasMayorDemanda;

            // Gráfica Pastel
            
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

        private void btn_generarReportes_Click(object sender, RoutedEventArgs e)
        {
            if (dpDesde.SelectedDate == null || dpHasta.SelectedDate == null)
            {
                MessageBox.Show("Seleccione ambas fechas.", "Advertencia", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            DateTime inicio = dpDesde.SelectedDate.Value;
            DateTime fin = dpHasta.SelectedDate.Value;
            int idUsuario = 1;

            try
            {
                Reporte reportClass = new Reporte();
                var reporte = reportClass.GenerarReporte(inicio, fin, idUsuario);

                if (reporte != null)
                {
                    MostrarGraficaCitas(reporte);
                    btn_exportarReporte.Visibility = Visibility.Visible;
                    borderEnviarCorreo.Visibility = Visibility.Visible;

                }
                else
                {
                    MessageBox.Show("No se generó ningún reporte.", "Información", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show($"Error al generar el reporte:\n{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btn_exportarReporte_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Asegurarse de que el panel sea visible
                panelReporteExport.Visibility = Visibility.Visible;

                // Ajustar tamaño del panel completo (incluso si hay scroll)
                panelReporteExport.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                panelReporteExport.Arrange(new Rect(panelReporteExport.DesiredSize));

                int width = (int)panelReporteExport.ActualWidth;
                int height = (int)panelReporteExport.ActualHeight;

                // Renderizamos el panel a bitmap
                RenderTargetBitmap renderBitmap = new RenderTargetBitmap(width, height, 96d, 96d, PixelFormats.Pbgra32);
                renderBitmap.Render(panelReporteExport);

                // Convertimos bitmap a PNG en memoria
                PngBitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(renderBitmap));

                byte[] imageBytes;
                using (MemoryStream ms = new MemoryStream())
                {
                    encoder.Save(ms);
                    imageBytes = ms.ToArray();
                }

                // Abrimos diálogo para guardar archivo
                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    Filter = "PDF Files (*.pdf)|*.pdf",
                    FileName = "Reporte.pdf"
                };

                //if (saveFileDialog.ShowDialog() == true)
                //{
                //    // Creamos el PDF
                //    PdfDocument document = new PdfDocument();
                //    PdfPage page = document.AddPage();

                //    // Ajustamos tamaño de página según bitmap
                //    page.Width = XUnit.FromPoint(width * 72.0 / 96.0);
                //    page.Height = XUnit.FromPoint(height * 72.0 / 96.0);

                //    XGraphics gfx = XGraphics.FromPdfPage(page);

                //    // Insertamos la imagen desde memoria
                //    using (MemoryStream msImg = new MemoryStream(imageBytes))
                //    {
                //        XImage img = XImage.FromStream(msImg);
                //        gfx.DrawImage(img, 0, 0, page.Width.Point, page.Height.Point);
                //    }

                //    // Guardamos el PDF en la ruta seleccionada
                //    document.Save(saveFileDialog.FileName);

                //    MessageBox.Show("PDF exportado correctamente!", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                //}
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al exportar PDF: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btn_EnviarCorreo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Asegurar que el panel sea visible y medido
                panelReporteExport.Visibility = Visibility.Visible;
                panelReporteExport.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                panelReporteExport.Arrange(new Rect(panelReporteExport.DesiredSize));

                int width = (int)panelReporteExport.ActualWidth;
                int height = (int)panelReporteExport.ActualHeight;

                // Renderizamos a bitmap
                RenderTargetBitmap renderBitmap = new RenderTargetBitmap(width, height, 96d, 96d, PixelFormats.Pbgra32);
                renderBitmap.Render(panelReporteExport);

                // Guardamos bitmap como PNG en memoria
                PngBitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(renderBitmap));

                byte[] imageBytes;
                using (MemoryStream ms = new MemoryStream())
                {
                    encoder.Save(ms);
                    imageBytes = ms.ToArray();
                }

                // Generamos PDF en memoria usando la imagen
                //byte[] pdfBytes;
                //using (MemoryStream msPdf = new MemoryStream())
                //{
                //    PdfDocument document = new PdfDocument();
                //    PdfPage page = document.AddPage();

                //    page.Width = XUnit.FromPoint(width * 72.0 / 96.0);
                //    page.Height = XUnit.FromPoint(height * 72.0 / 96.0);

                //    XGraphics gfx = XGraphics.FromPdfPage(page);

                //    using (MemoryStream msImg = new MemoryStream(imageBytes))
                //    {
                //        XImage img = XImage.FromStream(msImg);
                //        gfx.DrawImage(img, 0, 0, page.Width.Point, page.Height.Point);
                //    }

                //    document.Save(msPdf, false);
                //    pdfBytes = msPdf.ToArray();
                //}

                // Obtener correo del usuario
                string correo = usuarioActual.Correo;
                if (string.IsNullOrEmpty(correo))
                {
                    MessageBox.Show("No se encontró el correo del usuario.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Preparar y enviar correo con PDF adjunto
                string remitente = "odontonovaa@gmail.com";
                string contraseña = "rjsg ztyo bbls nmpp";

                string asunto = "Tu reporte estadístico";
                string cuerpo = $@"
<html>
<head>
    <style>
        body {{
            background-color: #f2f6fc;
            margin: 0;
            padding: 0;
            font-family: 'Segoe UI', Arial, sans-serif;
        }}
        .card {{
            max-width: 450px;
            background-color: #ffffff;
            margin: 30px auto;
            padding: 20px 25px;
            border-radius: 12px;
            box-shadow: 0 4px 15px rgba(0,0,0,0.12);
            text-align: center;
        }}
        .title {{
            font-size: 22px;
            font-weight: 700;
            color: #0057b8;
            margin-bottom: 15px;
        }}
        .message {{
            font-size: 15px;
            color: #444;
            margin-bottom: 15px;
        }}
        .footer {{
            margin-top: 20px;
            font-size: 12px;
            color: #666;
        }}
    </style>
</head>
<body>
    <div class='card'>
        <div class='title'>Reporte estadístico</div>
        <p class='message'>Adjunto encontrarás el reporte estadístico que generaste.</p>
        <p class='footer'>Gracias por usar nuestro servicio.</p>
    </div>
</body>
</html>";

                MailMessage mensaje = new MailMessage();
                mensaje.From = new MailAddress(remitente);
                mensaje.To.Add(correo);
                mensaje.Subject = asunto;
                mensaje.Body = cuerpo;
                mensaje.IsBodyHtml = true;

                //using (MemoryStream msPdfAdjunto = new MemoryStream(pdfBytes))
                //{
                //    Attachment adjunto = new Attachment(msPdfAdjunto, "Reporte.pdf", "application/pdf");
                //    mensaje.Attachments.Add(adjunto);

                //    SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
                //    smtp.Credentials = new NetworkCredential(remitente, contraseña);
                //    smtp.EnableSsl = true;

                //    smtp.Send(mensaje);
                //}

                MessageBox.Show("Reporte enviado correctamente al correo del usuario.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al enviar correo: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        



    }
}
