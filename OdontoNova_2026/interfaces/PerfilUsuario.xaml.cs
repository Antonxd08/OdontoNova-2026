using Microsoft.Win32;
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

namespace OdontoNova.Interfaces
{
    /// <summary>
    /// Lógica de interacción para PerfilUsuario.xaml
    /// </summary>
    public partial class PerfilUsuario : Window
    {
        private Window formAnterior;

        private Usuario usuarioActual;

        private byte[] nuevaFoto;

        private bool mostrarContra = false;
        private bool mostrarConfirmar = false;

        public PerfilUsuario(Window anterior, Usuario usuario)
        {
            InitializeComponent();
            formAnterior = anterior;
            usuarioActual = usuario;

            CargarDatosUsuario();

            Btn_MostrarContra.Visibility = Visibility.Collapsed;
            Btn_MostrarConfirmarContra.Visibility = Visibility.Collapsed;

            if (formAnterior.WindowState == WindowState.Maximized)
            {
                this.WindowState = WindowState.Maximized;
            }
            else
            {
                this.WindowState = WindowState.Normal;
            }
        }

        private void AplicarClipCircular(Image img)
        {
            EllipseGeometry clip = new EllipseGeometry();
            clip.Center = new Point(img.Width / 2, img.Height / 2);
            clip.RadiusX = img.Width / 2;
            clip.RadiusY = img.Height / 2;
            img.Clip = clip;
        }


        private void CargarDatosUsuario()
        {
            // Datos de solo lectura
            lb_Usuario.Text = usuarioActual.NombreUsuario;
            lb_Contraseña.Text = "••••••••";
            lb_Correo.Text = usuarioActual.Correo;
            lb_Rol.Text = usuarioActual.Rol;
            lb_Fecha.Text = DateTime.Now.ToString("dd/MM/yyyy");

            // Cargar datos a los textbox
            txt_Usuario.Text = usuarioActual.NombreUsuario;
            txt_Correo.Text = usuarioActual.Correo;

            // Mostrar foto si existe
            if (usuarioActual.FotoUsuario != null)
            {
                using (MemoryStream ms = new MemoryStream(usuarioActual.FotoUsuario))
                {
                    BitmapImage imagen = new BitmapImage();
                    imagen.BeginInit();
                    imagen.CacheOption = BitmapCacheOption.OnLoad;
                    imagen.StreamSource = ms;
                    imagen.EndInit();

                    img_FotoUsuarioLectura.Source = imagen;
                    img_FotoUsuarioEditable.Source = imagen;

                    AplicarClipCircular(img_FotoUsuarioLectura);
                    AplicarClipCircular(img_FotoUsuarioEditable);
                }
            }
            else
            {
                BitmapImage placeholder = new BitmapImage(new Uri("pack://application:,,,/Assets/Images/user_456212.png"));
                img_FotoUsuarioLectura.Source = placeholder;
                img_FotoUsuarioEditable.Source = placeholder;

                AplicarClipCircular(img_FotoUsuarioLectura);
                AplicarClipCircular(img_FotoUsuarioEditable);
            }

        }

        private void Btn_CambiarFoto_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Filter = "Archivos de Imagen|*.jpg;*.jpeg;*.png;*.bmp";
            if (openFile.ShowDialog() == true)
            {
                // Guardar la nueva foto en bytes
                nuevaFoto = File.ReadAllBytes(openFile.FileName);

                // Convertir a BitmapImage
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.StreamSource = new MemoryStream(nuevaFoto);
                bitmap.EndInit();

                // Asignar a ambos Image
                img_FotoUsuarioLectura.Source = bitmap;
                img_FotoUsuarioEditable.Source = bitmap;

                // Aplicar clip circular
                AplicarClipCircular(img_FotoUsuarioLectura);
                AplicarClipCircular(img_FotoUsuarioEditable);
            }
        }

        private void Btn_EliminarFoto_Click(object sender, RoutedEventArgs e)
        {
            nuevaFoto = null;
            img_FotoUsuarioEditable.Source = new BitmapImage(new Uri("pack://application:,,,/Assets/Images/user_456212.png"));
            img_FotoUsuarioLectura.Source = new BitmapImage(new Uri("pack://application:,,,/Assets/Images/user_456212.png"));
        }

        #region NavegabilidadVentana
        private void PerfilUsuario_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }
        private void Btn_Regresar_Click(object sender, RoutedEventArgs e)
        {
            //rergresar al form principal
            this.Close();
        }
        #endregion


        #region BotonesParaEditar(ActivarCampos)
        private void Btn_EditarUsuario_Click(object sender, RoutedEventArgs e)
        {
            txt_Usuario.IsEnabled = true;
            txt_Usuario.Focus();
            Btn_EditarUsuario.Visibility = Visibility.Collapsed;
            Btn_GuardarUsuario.Visibility = Visibility.Visible;
            Btn_CancelarUsuario.Visibility = Visibility.Visible;
        }
        private void Btn_EditarContra_Click(object sender, RoutedEventArgs e)
        {
            txt_Contraseña.IsEnabled = true;
            txt_Contraseña.Clear();
            Btn_EditarContra.Visibility = Visibility.Collapsed;
            Btn_GuardarContra.Visibility = Visibility.Visible;
            Btn_CancelarContra.Visibility = Visibility.Visible;
            PanelConfirmarContra.Visibility = Visibility.Visible;
            txt_ConfirmarContra.Clear();
            Btn_MostrarContra.Visibility = Visibility.Visible;
            Btn_MostrarConfirmarContra.Visibility = Visibility.Visible;
        }
        private void Btn_EditarCorreo_Click(object sender, RoutedEventArgs e)
        {
            txt_Correo.IsEnabled = true;
            txt_Correo.Focus();
            Btn_EditarCorreo.Visibility = Visibility.Collapsed;
            Btn_GuardarCorreo.Visibility = Visibility.Visible;
            Btn_CancelarCorreo.Visibility = Visibility.Visible;
        }
        #endregion

        #region BotonesParaCancelar

        private void Btn_CancelarUsuario_Click(object sender, RoutedEventArgs e)
        {
            txt_Usuario.IsEnabled = false;
            Btn_EditarUsuario.Visibility = Visibility.Visible;
            Btn_GuardarUsuario.Visibility = Visibility.Collapsed;
            Btn_CancelarUsuario.Visibility = Visibility.Collapsed;
            txt_Usuario.Text = usuarioActual.NombreUsuario;
        }
        private void Btn_CancelarContra_Click(object sender, RoutedEventArgs e)
        {
            txt_Contraseña.IsEnabled = false;
            Btn_EditarContra.Visibility = Visibility.Visible;
            Btn_GuardarContra.Visibility = Visibility.Collapsed;
            Btn_CancelarContra.Visibility = Visibility.Collapsed;
            PanelConfirmarContra.Visibility = Visibility.Collapsed;
            txt_Contraseña.Clear();
            txt_ConfirmarContra.Clear();
        }
        private void Btn_CancelarCorreo_Click(object sender, RoutedEventArgs e)
        {
            txt_Correo.IsEnabled = false;
            Btn_EditarCorreo.Visibility = Visibility.Visible;
            Btn_GuardarCorreo.Visibility = Visibility.Collapsed;
            Btn_CancelarCorreo.Visibility = Visibility.Collapsed;
            txt_Correo.Text = usuarioActual.Correo;
        }
        #endregion

        #region BotonesParaGuardarCambios
        private void Btn_GuardarUsuario_Click(object sender, RoutedEventArgs e)
        {
            string nuevoUsuario = txt_Usuario.Text.Trim();

            // Validaci de q no puede estar vacío
            if (string.IsNullOrWhiteSpace(nuevoUsuario))
            {
                MessageBox.Show("El nombre de usuario no puede estar vacío.",
                                "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Validacion que no permite numeros
            if (nuevoUsuario.Any(char.IsDigit))
            {
                MessageBox.Show("El nombre de usuario no puede contener números.",
                                "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Validacin para solo letras y espacios
            if (!System.Text.RegularExpressions.Regex.IsMatch(nuevoUsuario, @"^[A-Za-zÁÉÍÓÚáéíóúÑñ ]+$"))
            {
                MessageBox.Show("El nombre de usuario solo puede contener letras y espacios.",
                                "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Evitar espacios dobles
            if (nuevoUsuario.Contains("  "))
            {
                MessageBox.Show("El nombre de usuario no puede contener espacios duplicados.",
                                "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Longitud minima y msxima
            if (nuevoUsuario.Length < 3 || nuevoUsuario.Length > 30)
            {
                MessageBox.Show("El nombre de usuario debe tener entre 3 y 30 caracteres.",
                                "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Evitar guardar si no hubo cambios
            if (nuevoUsuario.Equals(usuarioActual.NombreUsuario, StringComparison.OrdinalIgnoreCase))
            {
                MessageBox.Show("No se detectaron cambios en el nombre de usuario.",
                                "Información", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            // Validacion para evitar patrones absurdos
            if (nuevoUsuario.Distinct().Count() == 1) 
            {
                MessageBox.Show("El nombre de usuario no puede ser un patrón repetido.",
                                "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            txt_Usuario.IsEnabled = false;
            Btn_EditarUsuario.Visibility = Visibility.Visible;
            Btn_GuardarUsuario.Visibility = Visibility.Collapsed;
            Btn_CancelarUsuario.Visibility = Visibility.Collapsed;

            MessageBox.Show("Nombre de usuario validado correctamente.",
                            "Correcto", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        private void Btn_GuardarContra_Click(object sender, RoutedEventArgs e)
        {
            // Para la contraseña principal
            string nuevaContra;
            if (txt_Contraseña.Visibility == Visibility.Visible)
                nuevaContra = txt_Contraseña.Password.Trim();
            else
                nuevaContra = txt_ContraseñaVisible.Text.Trim();

            // Para la confirmación de contraseña
            string confirmarContra;
            if (txt_ConfirmarContra.Visibility == Visibility.Visible)
                confirmarContra = txt_ConfirmarContra.Password.Trim();
            else
                confirmarContra = txt_ConfirmarContraVisible.Text.Trim();

            // Validacion de no vacia 
            if (string.IsNullOrWhiteSpace(nuevaContra))
            {
                MessageBox.Show("La contraseña no puede estar vacía.",
                                "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Longitud minima y maxima
            if (nuevaContra.Length < 8 || nuevaContra.Length > 32)
            {
                MessageBox.Show("La contraseña debe tener entre 8 y 32 caracteres.",
                                "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Validacion Debe contener mayuscula
            if (!nuevaContra.Any(char.IsUpper))
            {
                MessageBox.Show("La contraseña debe contener al menos una letra mayúscula.",
                                "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Validacion Debe contener minuscula
            if (!nuevaContra.Any(char.IsLower))
            {
                MessageBox.Show("La contraseña debe contener al menos una letra minúscula.",
                                "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Validacion Debe contener numero
            if (!nuevaContra.Any(char.IsDigit))
            {
                MessageBox.Show("La contraseña debe contener al menos un número.",
                                "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Validacion Debe contener simbolo
            if (!System.Text.RegularExpressions.Regex.IsMatch(nuevaContra, @"[!@#$%^&*(),.?""{}|<>_\-+=]"))
            {
                MessageBox.Show("La contraseña debe incluir al menos un símbolo (%, $, @, *, etc).",
                                "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            
            //contra confirmada
            if (nuevaContra != confirmarContra)
            {
                MessageBox.Show("Las contraseñas no coinciden.",
                                "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            txt_Contraseña.IsEnabled = false;
            Btn_EditarContra.Visibility = Visibility.Visible;
            Btn_GuardarContra.Visibility = Visibility.Collapsed;
            Btn_CancelarContra.Visibility = Visibility.Collapsed;
            PanelConfirmarContra.Visibility = Visibility.Collapsed;



            MessageBox.Show("La contraseña es válida y se guardará correctamente.",
                            "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        private void Btn_GuardarCorreo_Click(object sender, RoutedEventArgs e)
        {
            string nuevoCorreo = txt_Correo.Text.Trim();

            // 1. No vacío
            if (string.IsNullOrEmpty(nuevoCorreo))
            {
                MensajeErrorCorreo("El correo no puede estar vacío.");
                return;
            }

            // 2. No permite espacios
            if (nuevoCorreo.Contains(" "))
            {
                MensajeErrorCorreo("El correo no puede contener espacios.");
                return;
            }

            // 3. Validación de formato (regex)
            string patronCorreo = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            if (!Regex.IsMatch(nuevoCorreo, patronCorreo))
            {
                MensajeErrorCorreo("Formato incorrecto. Ejemplo: usuario@correo.com");
                return;
            }

            // 4. Longitud razonable
            if (nuevoCorreo.Length > 120)
            {
                MensajeErrorCorreo("El correo es demasiado largo (máx. 120 caracteres).");
                return;
            }

            // 5. Verificacion de dominios permitidos
            string dominio = nuevoCorreo.Split('@')[1].ToLower();

            string[] dominiosPermitidos =
            {
                "gmail.com",
                "outlook.com",
                "hotmail.com",
                "yahoo.com",
                "icloud.com",
                "aol.com",
                "protonmail.com",
                "live.com"
            };

            if (!dominiosPermitidos.Contains(dominio))
            {
                MensajeErrorCorreo("El dominio de correo no es común. Inserte un correo válido como:\n" +
                                   "gmail.com, outlook.com, yahoo.com");
                return;
            }

            txt_Correo.BorderBrush = Brushes.Gray;
            txt_Correo.IsEnabled = false;

            Btn_EditarCorreo.Visibility = Visibility.Visible;
            Btn_GuardarCorreo.Visibility = Visibility.Collapsed;
            Btn_CancelarCorreo.Visibility = Visibility.Collapsed;

            MessageBox.Show("Correo validado correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void MensajeErrorCorreo(string mensaje)
        {
            txt_Correo.BorderBrush = Brushes.Red;
            MessageBox.Show(mensaje, "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
        #endregion

        private void Btn_MostrarContra_Click(object sender, RoutedEventArgs e)
        {
            if (!mostrarContra)
            {
                // Mostrar contraseña
                txt_ContraseñaVisible.Text = txt_Contraseña.Password;
                txt_ContraseñaVisible.Visibility = Visibility.Visible;
                txt_Contraseña.Visibility = Visibility.Collapsed;
                Btn_MostrarContra.Content = "🚫";
                mostrarContra = true;
            }
            else
            {
                // Ocultar contraseña
                txt_Contraseña.Password = txt_ContraseñaVisible.Text;
                txt_Contraseña.Visibility = Visibility.Visible;
                txt_ContraseñaVisible.Visibility = Visibility.Collapsed;
                Btn_MostrarContra.Content = "👁";
                mostrarContra = false;
            }
        }

        // Mostrar/ocultar confirmación de contraseña
        private void Btn_MostrarConfirmarContra_Click(object sender, RoutedEventArgs e)
        {
            if (!mostrarConfirmar)
            {
                // Mostrar confirmación
                txt_ConfirmarContraVisible.Text = txt_ConfirmarContra.Password;
                txt_ConfirmarContraVisible.Visibility = Visibility.Visible;
                txt_ConfirmarContra.Visibility = Visibility.Collapsed;
                Btn_MostrarConfirmarContra.Content = "🚫";
                mostrarConfirmar = true;
            }
            else
            {
                // Ocultar confirmación
                txt_ConfirmarContra.Password = txt_ConfirmarContraVisible.Text;
                txt_ConfirmarContra.Visibility = Visibility.Visible;
                txt_ConfirmarContraVisible.Visibility = Visibility.Collapsed;
                Btn_MostrarConfirmarContra.Content = "👁";
                mostrarConfirmar = false;
            }
        }
        //activar panel de edicion
        private void Btn_EditarPerfilGeneral_Click(object sender, RoutedEventArgs e)
        {
            VistaSoloLectura.Visibility = Visibility.Collapsed;
            VistaEditable.Visibility = Visibility.Visible;
        }

        private void Btn_GuardarCambios_Click(object sender, RoutedEventArgs e)
        {
            bool exito = true;
            List<string> cambiosRealizados = new List<string>();

            // GUARDAR USUARIO
            if (txt_Usuario.Text != usuarioActual.NombreUsuario)
            {
                if (AccesoDatos.ActualizarUsuario(usuarioActual.IdUsuario, txt_Usuario.Text))
                {
                    usuarioActual.NombreUsuario = txt_Usuario.Text;
                    cambiosRealizados.Add("Nombre de usuario");
                }
                else exito = false;
            }

            // GUARDAR CORREO
            if (txt_Correo.Text != usuarioActual.Correo)
            {
                if (AccesoDatos.ActualizarCorreo(usuarioActual.IdUsuario, txt_Correo.Text))
                {
                    usuarioActual.Correo = txt_Correo.Text;
                    cambiosRealizados.Add("Correo electrónico");
                }
                else exito = false;
            }

            // GUARDAR CONTRASEÑA
            if (
                !string.IsNullOrWhiteSpace(txt_Contraseña.Password) &&
                txt_Contraseña.Password == txt_ConfirmarContra.Password)
            {
                if (AccesoDatos.ActualizarContraseña(usuarioActual.IdUsuario, txt_Contraseña.Password))
                {
                    cambiosRealizados.Add("Contraseña");
                }
                else exito = false;
            }

            // GUARDAR FOTO (solo si se actualizó)
            if (nuevaFoto != null)
            {
                if (AccesoDatos.ActualizarFotoUsuario(usuarioActual.IdUsuario, nuevaFoto))
                {
                    usuarioActual.FotoUsuario = nuevaFoto;
                    cambiosRealizados.Add("Foto de perfil");
                }
                else exito = false;
            }

            // RESULTADO FINAL ---------------------------------------

            if (!cambiosRealizados.Any())
            {
                MessageBox.Show("No se realizaron cambios.", "Información", MessageBoxButton.OK, MessageBoxImage.Information);
                VistaEditable.Visibility = Visibility.Collapsed;
                VistaSoloLectura.Visibility = Visibility.Visible;
                ReiniciarPanelEdicion();
                return;
            }

            if (exito)
            {
                string resumen = "Se actualizaron los siguientes datos:\n\n";
                resumen += string.Join("\n", cambiosRealizados.Select(c => $"✔ {c}"));

                MessageBox.Show(resumen, "Cambios Guardados", MessageBoxButton.OK, MessageBoxImage.Information);
                
            }
            else
            {
                MessageBox.Show("Hubo un error al actualizar uno o más datos.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            VistaEditable.Visibility = Visibility.Collapsed;
            VistaSoloLectura.Visibility = Visibility.Visible;
            ReiniciarPanelEdicion();
        }

        private void ReiniciarPanelEdicion()
        {
            txt_Usuario.IsEnabled = false;
            txt_Contraseña.IsEnabled = false;
            txt_Correo.IsEnabled = false;
            PanelConfirmarContra.Visibility = Visibility.Collapsed;
            txt_ConfirmarContra.Clear();

            Btn_EditarUsuario.Visibility = Visibility.Visible;
            Btn_GuardarUsuario.Visibility = Visibility.Collapsed;
            Btn_CancelarUsuario.Visibility = Visibility.Collapsed;

            Btn_EditarContra.Visibility = Visibility.Visible;
            Btn_GuardarContra.Visibility = Visibility.Collapsed;
            Btn_CancelarContra.Visibility = Visibility.Collapsed;

            Btn_EditarCorreo.Visibility = Visibility.Visible;
            Btn_GuardarCorreo.Visibility = Visibility.Collapsed;
            Btn_CancelarCorreo.Visibility = Visibility.Collapsed;

            
            txt_Usuario.Text = usuarioActual.NombreUsuario;
            txt_Contraseña.Password = "";  
            txt_Correo.Text = usuarioActual.Correo;
        }
    }
}
