using OdontoNova;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace SistemaOdontonova.Interfaces
{
    /// <summary>
    /// Lógica de interacción para CambiarContraseña.xaml
    /// </summary>
    public partial class CambiarContraseña : Window
    {
        int pasoActual = 1;
        DispatcherTimer timer;
        TimeSpan tiempoRestante;
        bool mostrarNuevaContra = false;
        bool mostrarConfirmarContra = false;

        private Usuario usuarioActual;

        public CambiarContraseña(Usuario usuarioActual)
        {
            InitializeComponent();
            Psw_Contraseñanueva.PasswordChanged += Psw_Contraseñanueva_PasswordChanged;
            this.usuarioActual = usuarioActual;
        }

        #region Paso 1 - Correo
        private void Btn_EnviarCodigo_Click(object sender, RoutedEventArgs e)
        {
            string correo = Txt_CorreoElectronico.Text.Trim();

            if (string.IsNullOrEmpty(correo))
            {
                MessageBox.Show("El correo electrónico es obligatorio.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!EsCorreoValido(correo))
            {
                MessageBox.Show("Formato de correo incorrecto.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            
            int idUsuario = TokenSeguridad_DAL.ObtenerIdUsuarioPorCorreo(correo);
            if (idUsuario == -1)
            {
                
                MessageBox.Show("El correo ingresado no coincide con ningún usuario registrado.",
                                "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }


            string token = TokenSeguridad_DAL.GenerarToken();
            TokenSeguridad_DAL.GuardarToken(correo, token);
            EnviarCorreoRecuperacion(correo, token);

            AnimarTransicionPasos(Paso1, Paso2);
            ActualizarBarraPasos(2);
            IniciarTimerToken();
            pasoActual = 2;
        }

        private bool EsCorreoValido(string correo)
        {
            try
            {
                var mail = new MailAddress(correo);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private void EnviarCorreoRecuperacion(string destinatario, string token)
        {
            try
            {
                string remitente = "odontonovaa@gmail.com"; 
                string contraseña = "rjsg ztyo bbls nmpp";

                string asunto = "Recuperación de contraseña";
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
        .logo {{
            width: 120px;
            margin-bottom: 15px;
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
        .token {{
            font-size: 28px;
            font-weight: bold;
            padding: 12px 18px;
            border-radius: 6px;
            background-color: #e8f1ff;
            border: 2px dashed #0057b8;
            color: #003f88;
            display: inline-block;
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
        <div class='title'>Código de recuperación</div>
        <p class='message'>Solicitaste recuperar tu contraseña. Usa el siguiente código:</p>
        <div class='token'>{token}</div>
        <p class='footer'>Este código expira en 5 minutos. Si no lo solicitaste, ignora este correo.</p>
    </div>
</body>
</html>";
                MailMessage mensaje = new MailMessage();
                mensaje.From = new MailAddress(remitente);
                mensaje.To.Add(destinatario);
                mensaje.Subject = asunto;
                mensaje.Body = cuerpo;
                mensaje.IsBodyHtml = true;

                SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
                smtp.Credentials = new NetworkCredential(remitente, contraseña);
                smtp.EnableSsl = true;

                smtp.Send(mensaje);

                MessageBox.Show("Código de recuperación enviado con éxito.", "Token Enviado", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al enviar correo: " + ex.Message, "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        #endregion

        #region Paso 2 - Token
        private void Btn_VerificarToken_Click(object sender, RoutedEventArgs e)
        {
            string correo = Txt_CorreoElectronico.Text.Trim();
            string token = Txt_tokenEnviado.Text.Trim();

            if (string.IsNullOrEmpty(token))
            {
                MessageBox.Show("Ingresa el token enviado a tu correo.", "Advertencia",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (TokenSeguridad_DAL.ValidarToken(correo, token))
            {
                TokenSeguridad_DAL.MarcarTokenUsado(correo, token);
                timer?.Stop();

                AnimarTransicionPasos(Paso2, Paso3);
                ActualizarBarraPasos(3);
                pasoActual = 3;
            }
            else
            {
                MessageBox.Show("El token ingresado no es válido o ha expirado.", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void IniciarTimerToken()
        {
            tiempoRestante = TimeSpan.FromMinutes(1);
            Lb_temporizador.Text = "Expira en " + tiempoRestante.ToString("mm\\:ss");
            Lb_temporizador.Foreground = Brushes.Red;

            timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            timer.Tick += (s, e) =>
            {
                if (tiempoRestante.TotalSeconds <= 0)
                {
                    timer.Stop();
                    Lb_temporizador.Text = "Token expirado";
                    Lb_temporizador.Foreground = Brushes.Gray;

                    TokenSeguridad_DAL.ExpirarToken(Txt_CorreoElectronico.Text.Trim());

                    //Mostrar botón para reenviar token
                    Btn_VerificarToken.Visibility = Visibility.Collapsed;
                    Btn_ReenviarToken.Visibility = Visibility.Visible;

                    return;
                }
                else
                {
                    tiempoRestante = tiempoRestante.Add(TimeSpan.FromSeconds(-1));
                    Lb_temporizador.Text = "Expira en " + tiempoRestante.ToString("mm\\:ss");
                }
            };
            timer.Start();
        }

        private void Btn_ReenviarToken_Click(object sender, RoutedEventArgs e)
        {
            string correo = Txt_CorreoElectronico.Text.Trim();

            // Validar correo antes de reenviar
            if (string.IsNullOrEmpty(correo))
            {
                MessageBox.Show("No se puede reenviar el token sin un correo.", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Generar y guardar un nuevo token
            string nuevoToken = TokenSeguridad_DAL.GenerarToken();
            TokenSeguridad_DAL.GuardarToken(correo, nuevoToken);

            // Enviar correo nuevamente
            EnviarCorreoRecuperacion(correo, nuevoToken);

            Btn_ReenviarToken.Visibility = Visibility.Collapsed;
            Btn_VerificarToken.Visibility = Visibility.Visible;

            // Reiniciar temporizador
            IniciarTimerToken();

            MessageBox.Show("Se envió un nuevo token a tu correo.", "Código reenviado",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        #endregion

        #region Paso 3 - Actualizar contraseña
        private void Btn_MostrarContraseña_Click(object sender, RoutedEventArgs e) 
        { 
            if (!mostrarNuevaContra) 
            { 
                Txt_ContraseñaNueva.Text = Psw_Contraseñanueva.Password; 
                Psw_Contraseñanueva.Visibility = Visibility.Collapsed; 
                Txt_ContraseñaNueva.Visibility = Visibility.Visible; 
                Btn_MostrarContraseña.Content = "🚫"; 
                mostrarNuevaContra = true; 
            }
            else 
            { 
                Psw_Contraseñanueva.Password = Txt_ContraseñaNueva.Text; 
                Txt_ContraseñaNueva.Visibility = Visibility.Collapsed; 
                Psw_Contraseñanueva.Visibility = Visibility.Visible; 
                Btn_MostrarContraseña.Content = "👁"; 
                mostrarNuevaContra = false; 
            } 
        } 
        private void Btn_MostrarContraseña2_Click(object sender, RoutedEventArgs e) 
        { 
            if (!mostrarConfirmarContra) 
            { 
                Txt_Confirmarcontraseña.Text = Psw_Confirmarcontraseña.Password; 
                Psw_Confirmarcontraseña.Visibility = Visibility.Collapsed; 
                Txt_Confirmarcontraseña.Visibility = Visibility.Visible; 
                Btn_MostrarContraseña2.Content = "🚫"; mostrarConfirmarContra = true; 
            } 
            else 
            { 
                Psw_Confirmarcontraseña.Password = Txt_Confirmarcontraseña.Text; 
                Txt_Confirmarcontraseña.Visibility = Visibility.Collapsed; 
                Psw_Confirmarcontraseña.Visibility = Visibility.Visible; 
                Btn_MostrarContraseña2.Content = "👁"; 
                mostrarConfirmarContra = false; 
            } 
        }

        private void Btn_ActualizarContra_Click(object sender, RoutedEventArgs e)
        {
            string nueva = Psw_Contraseñanueva.Password;
            string confirmar = Psw_Confirmarcontraseña.Password;
            string correo = Txt_CorreoElectronico.Text.Trim();

            if (nueva != confirmar)
            {
                MessageBox.Show("Las contraseñas no coinciden.", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            int idUsuario = TokenSeguridad_DAL.ObtenerIdUsuarioPorCorreo(correo);

            if (idUsuario == -1)
            {
                MessageBox.Show("No se encontró un usuario con ese correo.");
                return;
            }
            bool actualizado = AccesoDatos.ActualizarContraseña(idUsuario, nueva);
            if (actualizado)
            {
                MessageBox.Show("Tu contraseña ha sido actualizada con éxito.", "Listo",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                InicioSesion login = new InicioSesion(usuarioActual);
                login.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("No se pudo actualizar la contraseña.", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Psw_Contraseñanueva_PasswordChanged(object sender, RoutedEventArgs e)
        {
            string contraseña = Psw_Contraseñanueva.Password;
            (string texto, Brush color, double porcentaje) = CalcularFuerzaContraseña(contraseña);

            Lb_textoBarraContra.Text = texto;
            Lb_textoBarraContra.Foreground = color;
            PgBar_seguridad.Value = porcentaje;
            PgBar_seguridad.Foreground = color;
        }

        private (string texto, Brush color, double porcentaje) CalcularFuerzaContraseña(string contraseña)
        {
            if (string.IsNullOrEmpty(contraseña))
                return ("", Brushes.Gray, 0);

            int puntuacion = 0;
            if (contraseña.Length >= 8) puntuacion++;
            if (Regex.IsMatch(contraseña, @"[A-Z]")) puntuacion++;
            if (Regex.IsMatch(contraseña, @"[a-z]")) puntuacion++;
            if (Regex.IsMatch(contraseña, @"[0-9]")) puntuacion++;
            if (Regex.IsMatch(contraseña, @"[\W_]")) puntuacion++;

            if (puntuacion <= 2)
                return ("Seguridad: Débil", Brushes.Red, 35);
            else if (puntuacion == 3 || puntuacion == 4)
                return ("Seguridad: Media", Brushes.Orange, 70);
            else
                return ("Seguridad: Fuerte", Brushes.Green, 100);
        }
        #endregion

        #region Animaciones / Navegación
        private void AnimarTransicionPasos(UIElement from, UIElement to)
        {
            DoubleAnimation fadeOut = new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(300));
            fadeOut.Completed += (s, e) =>
            {
                from.Visibility = Visibility.Collapsed;
                to.Visibility = Visibility.Visible;

                DoubleAnimation fadeIn = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(300));
                to.BeginAnimation(UIElement.OpacityProperty, fadeIn);
            };
            from.BeginAnimation(UIElement.OpacityProperty, fadeOut);
        }

        private void ActualizarBarraPasos(int paso)
        {
            Brush activo = new SolidColorBrush(Color.FromRgb(11, 138, 162));
            Brush inactivo = new SolidColorBrush(Color.FromRgb(158, 158, 158));

            Punto1.Fill = (paso >= 1) ? activo : inactivo;
            Barra1.Fill = (paso >= 2) ? activo : inactivo;
            Punto2.Fill = (paso >= 2) ? activo : inactivo;
            Barra2.Fill = (paso >= 3) ? activo : inactivo;
            Punto3.Fill = (paso >= 3) ? activo : inactivo;
        }

        private void Btn_Cerrar_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("¿Deseas cerrar la ventana? Los datos no guardados se perderán.",
                "Confirmación", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                InicioSesion LOGIN = new InicioSesion(usuarioActual);
                LOGIN.Show();
                this.Close();
            }
        }

        private void CambiarContraseña_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed)
                DragMove();
        }
        #endregion
    }
}
