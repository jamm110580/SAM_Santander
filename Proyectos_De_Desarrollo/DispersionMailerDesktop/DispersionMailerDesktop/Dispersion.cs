using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Drawing.Drawing2D;
using DispersionMailerDesktop.Model;
using DispersionMailerDesktop.Helpers;
using System.Xml;
using OfficeOpenXml;
using System.Globalization;
using DispersionMailerDesktop.Utilidades;
using System.Net.Mail;
using System.Net.Security;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Diagnostics.Contracts;

namespace DispersionMailerDesktop
{
    public partial class FormDispersion : Form
    {
        public FormDispersion( )
        {
            InitializeComponent( );
        }

        //********************************************************************************************************* //
        //*************************************** METODOS PARA ABRIR EL FORMULARIO ******************************** //
        //********************************************************************************************************* //

        /// <summary>
        /// METODO PARA HACER TODAS LAS CARGAS NECESARIAS ANTES DE ABRIR EL FORMULARIO
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormDispersion_Load( object sender, EventArgs e )
        {
            AplicaFormatos( );

            //Carga de DataGridView 

            string rutaExcel = Path.Combine( LeerRutaDesdeConfigSistema( ), "ListaDeDistribucion.xlsx" );

            if(File.Exists( rutaExcel ))
            {
                var lista = ExcelHelper.CargarDestinatariosDesdeExcel( rutaExcel );
                CargarContactosEnGrid( lista );
            } else
            {
                MessageBox.Show( "La lista de distribución no fue encontrado. Si es la primera vez que ejecutas por favor cargala.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
            }
        }

        //********************************************************************************************************* //
        //*************************************** METODOS PARA LOS CONTROLES ************************************** //
        //********************************************************************************************************* //

        /// <summary>
        /// BOTON DE LA DISPERSIÓN DE CONTRATOS
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnEnviarCorreos_Click( object sender, EventArgs e )
        {
            string rutaBase = LeerRutaDesdeConfig( ).Trim( );
            string rutaBaseMandatos = LeerRutaDesdeConfigCarpetaContratosNT( ).Trim( );

            if(string.IsNullOrWhiteSpace( rutaBase ) || !Directory.Exists( rutaBase ))
            {
                MessageBox.Show( "Por favor selecciona una ruta válida para los archivos de dispersión.", "Ruta inválida", MessageBoxButtons.OK, MessageBoxIcon.Warning );
                return;
            }

            string rutaExcel = Path.Combine( LeerRutaDesdeConfigSistema( ), "ListaDeDistribucion.xlsx" );

            if(!File.Exists( rutaExcel ))
            {
                MessageBox.Show( "No se encontró el archivo de distribución de correos.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
                return;
            }

            // Cargar destinatarios desde el Excel
            List<DestinatarioInfo> listaDestinatarios = CargarDestinatariosDesdeExcel( rutaExcel );

            // Buscar archivos ZIP, RAR, etc. En las dos rutas, la tradicional y la de BOSH
            var archivos = BuscarArchivosComprimidos( rutaBase, dtpFEchaDeDispersion.Value );
            var archivosMandatos = BuscarArchivosComprimidosPorFechaTodasEmpresas( rutaBaseMandatos, dtpFEchaDeDispersion.Value );

            if(archivos.Count == 0)
            {
                MessageBox.Show( "No se encontraron contratos con la fecha actual en las carpetas para enviar.", "Contratos no encontrados", MessageBoxButtons.OK, MessageBoxIcon.Information );
                return;
            }

            // Listas para registrar los correos enviados y no enviados
            List<DestinatarioInfo> correosEnviados = new List<DestinatarioInfo>( );
            List<DestinatarioInfo> correosNoEnviados = new List<DestinatarioInfo>( );

            foreach(var archivo in archivos)
            {
                // Reemplazar en el HTML
                string rutaPlantilla = Path.Combine( Application.StartupPath, "Plantilla", "plantillaCorreo.html" );

                if(!File.Exists( rutaPlantilla ))
                {
                    MessageBox.Show( "No se encontró la plantilla de correo HTML.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
                    return;
                }

                string html = File.ReadAllText( rutaPlantilla );
                html = html.Replace( "{{nombreArchivo}}", Path.GetFileName( archivo ) );


                string contrato = ObtenerContratoDesdeRuta( archivo );
                var destinatario = BuscarDestinatarioPorContrato( contrato, listaDestinatarios );
                var (listaPara, listaCC) = ObtenerDestinatariosPorContrato( contrato, listaDestinatarios );
                string destinatarioPara = BuscarDestinatariosParaPorContrato( contrato, listaDestinatarios );
                string destinatarioCC = BuscarDestinatariosCCPorContrato( contrato, listaDestinatarios );
                bool correoEnviado = false;

                if(!string.IsNullOrWhiteSpace( destinatarioPara ))
                {
                    string asunto = $"RETIRO Operación contrato {Path.GetFileName( archivo )} .";
                    correoEnviado = EnviarCorreo( destinatarioPara, destinatarioCC, asunto, html, archivo );
                }

                if(correoEnviado)
                {
                    correosEnviados.AddRange( listaPara );
                    correosEnviados.AddRange( listaCC );
                } else
                {
                    correosEnviados.AddRange( listaPara );
                    correosEnviados.AddRange( listaCC );
                }
            }

            foreach(var archivoMandato in archivosMandatos)
            {
                // Reemplazar en el HTML
                string rutaPlantilla = Path.Combine( Application.StartupPath, "Plantilla", "plantillaCorreo.html" );

                if(!File.Exists( rutaPlantilla ))
                {
                    MessageBox.Show( "No se encontró la plantilla de correo HTML.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
                    return;
                }

                string html = File.ReadAllText( rutaPlantilla );
                html = html.Replace( "{{nombreArchivo}}", Path.GetFileName( archivoMandato ) );

                string contrato = ObtenerContratoDesdeRutaParaMandatos( archivoMandato );
                var (listaPara, listaCC) = ObtenerDestinatariosPorContrato( contrato, listaDestinatarios );
                var destinatario = BuscarDestinatarioPorContrato( contrato, listaDestinatarios );
                string destinatarioPara = BuscarDestinatariosParaPorContrato( contrato, listaDestinatarios );
                string destinatarioCC = BuscarDestinatariosCCPorContrato( contrato, listaDestinatarios );
                bool correoEnviado = false;

                if(!string.IsNullOrWhiteSpace( destinatarioPara ))
                {
                    string asunto = $"RETIRO Operación contrato {Path.GetFileName( archivoMandato )} .";
                    try
                    {
                        correoEnviado = EnviarCorreo( destinatarioPara, destinatarioCC, asunto, html, archivoMandato );
                    } catch(Exception ex)
                    {
                        Logger.RegistrarErrorEnLog( $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Error con {destinatarioPara}: {ex.Message}" );
                        continue; // sigue con el siguiente
                    }

                }

                if(correoEnviado)
                {
                    correosEnviados.AddRange( listaPara );
                    correosEnviados.AddRange( listaCC );
                } else
                {
                    correosEnviados.AddRange( listaPara );
                    correosEnviados.AddRange( listaCC );
                }
            }

            LogIrregualridades( );
            string nombreArchivo = Path.Combine( LeerRutaDesdeConfigSistema( ).Trim( ), "IrregularidadesEncontradas" + DateTime.Now.ToString( "yyyyMMMMdd" ) + ".xlsx" );
            GuardarCorreosEnExcel( nombreArchivo, "Correos Enviados", correosEnviados );
            GuardarCorreosEnExcel( nombreArchivo, "Correos No Enviados", correosNoEnviados );

            MessageBox.Show( "Los correos se han enviado exitosamente. Por favor descargue el registro de actividades.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information );

            DescargarArchivoConDialogo( Path.Combine( LeerRutaDesdeConfigSistema( ).Trim( ), "IrregularidadesEncontradas" + DateTime.Now.ToString( "yyyyMMMMdd" ) + ".xlsx" ) );
        }

        /// <summary>
        /// CIERRA EL FORMULARIO
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCerrar_Click( object sender, EventArgs e )
        {
            this.Close( );
        }

        /// <summary>
        /// MOSTRAR LOS CONTACTOS DEL ARCHIVO DE EXCEL
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnListaContactos_Click( object sender, EventArgs e )
        {
            //Carga de DataGridView 

            string rutaExcel = Path.Combine( LeerRutaDesdeConfigSistema( ), "ListaDeDistribucion.xlsx" );

            if(File.Exists( rutaExcel ))
            {
                var lista = ExcelHelper.CargarDestinatariosDesdeExcel( rutaExcel );
                CargarContactosEnGrid( lista );
            } else
            {
                MessageBox.Show( "La lista de distribución no fue encontrado. Si es la primera vez que ejecutas por favor cargala.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
            }
        }

        /// <summary>
        /// MÉTODO PARA LEER LA RUTA DESDE DEL XML PARA LOS CONTRATOS DE DISPERSIÓN NO TRADICIONALES
        /// </summary>
        /// <returns></returns>
        private string LeerRutaDesdeConfigCarpetaContratosNT( )
        {
            string archivoConfig = Path.Combine( Environment.GetFolderPath( Environment.SpecialFolder.MyDocuments ), "DispersionMailer", "Datos", "configRuta.xml" );

            if(!File.Exists( archivoConfig ))
                return string.Empty;

            XmlDocument doc = new XmlDocument( );
            doc.Load( archivoConfig );

            XmlNode nodoRuta = doc.SelectSingleNode( "/RutaConfiguracion/RutaCarpetaContratosNT" );
            return nodoRuta?.InnerText ?? string.Empty;
        }

        //********************************************************************************************************* //
        //******************************** METODOS PARA DAR FORMATO AL FORMULARIO ******************************** //
        //********************************************************************************************************* //

        /// <summary>
        /// Devuelve una tupla con dos listas: destinatarios "Para" y destinatarios "CC" para el contrato especificado.
        /// </summary>
        /// <param name="contrato">Número de contrato.</param>
        /// <param name="destinatarios">Lista completa de destinatarios.</param>
        /// <returns>Tuple con listas de destinatarios "Para" y "CC".</returns>
        public static (List<DestinatarioInfo> Para, List<DestinatarioInfo> CC) ObtenerDestinatariosPorContrato( string contrato, List<DestinatarioInfo> destinatarios )
        {
            if(string.IsNullOrWhiteSpace( contrato ) || destinatarios == null)
                return (new List<DestinatarioInfo>( ), new List<DestinatarioInfo>( ));

            var listaPara = destinatarios
                .Where( d => d.Contrato == contrato && d.TipoDeEnvio.Equals( "Para", StringComparison.OrdinalIgnoreCase ) )
                .ToList( );

            var listaCC = destinatarios
                .Where( d => d.Contrato == contrato && d.TipoDeEnvio.Equals( "CC", StringComparison.OrdinalIgnoreCase ) )
                .ToList( );

            return (listaPara, listaCC);
        }

        /// <summary>
        /// APLICA FORMATO A TODOS LOS CONTROLES DEL FORMULARIO
        /// </summary>
        private void AplicaFormatos( )
        {
            //Formato de Botones
            AplicarFormatoBotonContactoSinContrato( );
            AplicarFormatoBotonlistaContactos( );
            AplicarFormatoBotonContratoSinContacto( );
            //Formato de paneles
            RedondearPanel( pnlDispersion );
            RedondearPanel( pnlActualizaContratos );
            RedondearPanel( panel1 );
            RedondearPanel( panel2 );
            RedondearPanel( pnlActualizaContratos );
            //Formato de calendario
            FormatearDateTimePicker( );
            //Formatea el Boton Cerrar
            FormatoBotonCerrar( );
            DarFormatoBotonDispersion( );
        }

        /// <summary>
        /// METODO PARA DAR FORMATO AL BOTON Y PONER LA IMAGEN DE FONDO PAAR EL BOTON DE IMAGEN
        /// </summary>
        private void AplicarFormatoBotonContactoSinContrato( )
        {
            // Redimensionar la imagen a 64x64
            Image original = Properties.Resources.Lista;
            Image resized = new Bitmap( original, new Size( 64, 64 ) );

            // Asignar imagen al botón
            btnContactosSinContrato.BackgroundImage = resized;
            btnContactosSinContrato.BackgroundImageLayout = ImageLayout.Center;

            // Estilo 3D y profesional
            btnContactosSinContrato.FlatStyle = FlatStyle.Flat;
            btnContactosSinContrato.FlatAppearance.BorderSize = 2;
            btnContactosSinContrato.FlatAppearance.BorderColor = Color.FromArgb( 20, 90, 160 ); // Azul profundo
            btnContactosSinContrato.BackColor = Color.FromArgb( 45, 125, 200 ); // Azul profesional
            btnContactosSinContrato.ForeColor = Color.White;
            btnContactosSinContrato.Font = new Font( "Segoe UI", 10, FontStyle.Bold );

            // Texto opcional
            btnContactosSinContrato.Text = ""; // Si quieres texto: "Contactos"

            // Bordes redondeados
            btnContactosSinContrato.Region = Region.FromHrgn(
                CreateRoundRectRgn( 0, 0, btnContactosSinContrato.Width, btnContactosSinContrato.Height, 20, 20 ) );

            // Sombra o efecto 3D visual (simulado con padding y border)
            btnContactosSinContrato.Padding = new Padding( 5 );
        }

        /// <summary>
        /// METODO PARA DAR FORMATO AL BOTON Y PONER LA IMAGEN DE FONDO PAAR EL BOTON DE LISTA DE CONTACTOS
        /// </summary>
        private void AplicarFormatoBotonlistaContactos( )
        {
            // Redimensionar la imagen a 64x64
            Image original = Properties.Resources.Excel;
            Image resized = new Bitmap( original, new Size( 64, 64 ) );

            // Asignar imagen al botón
            btnListaContactos.BackgroundImage = resized;
            btnListaContactos.BackgroundImageLayout = ImageLayout.Center;

            // Estilo 3D y profesional
            btnListaContactos.FlatStyle = FlatStyle.Flat;
            btnListaContactos.FlatAppearance.BorderSize = 2;
            btnListaContactos.FlatAppearance.BorderColor = Color.FromArgb( 20, 90, 160 ); // Azul profundo
            btnListaContactos.BackColor = Color.FromArgb( 45, 125, 200 ); // Azul profesional
            btnListaContactos.ForeColor = Color.White;
            btnListaContactos.Font = new Font( "Segoe UI", 10, FontStyle.Bold );

            // Texto opcional
            btnListaContactos.Text = ""; // Si quieres texto: "Contactos"

            // Bordes redondeados
            btnListaContactos.Region = Region.FromHrgn(
                CreateRoundRectRgn( 0, 0, btnListaContactos.Width, btnListaContactos.Height, 20, 20 ) );

            // Sombra o efecto 3D visual (simulado con padding y border)
            btnListaContactos.Padding = new Padding( 5 );
        }

        /// <summary>
        /// METODO PARA DAR FORMATO AL BOTON Y PONER LA IMAGEN DE FONDO PARA EL BOTON DE CONTRATOS SIN CONTACTO
        /// </summary>
        private void AplicarFormatoBotonContratoSinContacto( )
        {
            // Redimensionar la imagen a 64x64
            Image original = Properties.Resources.Contrato;
            Image resized = new Bitmap( original, new Size( 64, 64 ) );

            // Asignar imagen al botón
            btnContratosSinContacto.BackgroundImage = resized;
            btnContratosSinContacto.BackgroundImageLayout = ImageLayout.Center;

            // Estilo 3D y profesional
            btnContratosSinContacto.FlatStyle = FlatStyle.Flat;
            btnContratosSinContacto.FlatAppearance.BorderSize = 2;
            btnContratosSinContacto.FlatAppearance.BorderColor = Color.FromArgb( 20, 90, 160 ); // Azul profundo
            btnContratosSinContacto.BackColor = Color.FromArgb( 45, 125, 200 ); // Azul profesional
            btnContratosSinContacto.ForeColor = Color.White;
            btnContratosSinContacto.Font = new Font( "Segoe UI", 10, FontStyle.Bold );

            // Texto opcional
            btnContratosSinContacto.Text = ""; // Si quieres texto: "Contactos"

            // Bordes redondeados
            btnContratosSinContacto.Region = Region.FromHrgn(
                CreateRoundRectRgn( 0, 0, btnContactosSinContrato.Width, btnContactosSinContrato.Height, 20, 20 ) );

            // Sombra o efecto 3D visual (simulado con padding y border)
            btnContratosSinContacto.Padding = new Padding( 5 );
        }

        /// <summary>
        /// MÉTODO PARA APLICAR ESQUINAS REDONDEADAS A CUALQUIER OBJETO
        /// </summary>
        /// <param name="panel"></param>
        private void RedondearPanel( Panel panel )
        {
            GraphicsPath path = new GraphicsPath( );
            int radio = 20;
            path.AddArc( 0, 0, radio, radio, 180, 90 );
            path.AddArc( panel.Width - radio, 0, radio, radio, 270, 90 );
            path.AddArc( panel.Width - radio, panel.Height - radio, radio, radio, 0, 90 );
            path.AddArc( 0, panel.Height - radio, radio, radio, 90, 90 );
            path.CloseAllFigures( );
            panel.Region = new Region( path );
        }

        /// <summary>
        /// LE DA FORMATO AL DATIME PICKER DE LA FECHA DE DISPERSIÓN
        /// </summary>
        private void FormatearDateTimePicker( )
        {
            dtpFEchaDeDispersion.Format = DateTimePickerFormat.Custom;
            dtpFEchaDeDispersion.CustomFormat = "dd/MM/yyyy";
            dtpFEchaDeDispersion.Font = new Font( "Segoe UI", 10, FontStyle.Bold );
            dtpFEchaDeDispersion.CalendarMonthBackground = Color.White;
            dtpFEchaDeDispersion.CalendarForeColor = Color.Black;
            dtpFEchaDeDispersion.CalendarTitleBackColor = Color.SteelBlue;
            dtpFEchaDeDispersion.CalendarTitleForeColor = Color.White;
            dtpFEchaDeDispersion.CalendarTrailingForeColor = Color.Gray;
        }

        /// <summary>
        /// FORMATEA EL GRIDVIEW PARA QUE TENGA CABECERA Y ABARQUE TODO EL ESPACIO
        /// </summary>
        private void FormatearDataFridView( )
        {
            // Estilizado
            dgvContactos.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvContactos.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            dgvContactos.EnableHeadersVisualStyles = false;
            dgvContactos.ColumnHeadersDefaultCellStyle.BackColor = Color.SteelBlue;
            dgvContactos.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvContactos.ColumnHeadersDefaultCellStyle.Font = new Font( "Segoe UI", 10, FontStyle.Bold );
            dgvContactos.GridColor = Color.LightGray;
            dgvContactos.BorderStyle = BorderStyle.None;
            dgvContactos.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvContactos.RowHeadersVisible = false;
            dgvContactos.AlternatingRowsDefaultCellStyle.BackColor = Color.LightGray;
            dgvContactos.DefaultCellStyle.Font = new Font( "Segoe UI", 10 );
            dgvContactos.DefaultCellStyle.SelectionBackColor = Color.LightBlue;
        }

        /// <summary>
        /// DA FORMATO AL BOTON CERRAR
        /// </summary>
        private void FormatoBotonCerrar( )
        {
            btnCerrar.Text = "Cerrar";
            btnCerrar.Font = new Font( "Segoe UI Semibold", 12F, FontStyle.Bold );
            btnCerrar.BackColor = Color.FromArgb( 220, 53, 69 ); // Rojo elegante
            btnCerrar.ForeColor = Color.White;
            btnCerrar.FlatStyle = FlatStyle.Flat;
            btnCerrar.FlatAppearance.BorderSize = 0;
            btnCerrar.Size = new Size( 100, 40 );
            btnCerrar.Cursor = Cursors.Hand;
            btnCerrar.TabStop = false;
            btnCerrar.Region = System.Drawing.Region.FromHrgn(
                CreateRoundRectRgn( 0, 0, btnCerrar.Width, btnCerrar.Height, 25, 20 ) ); // Bordes redondeados
        }

        /// <summary>
        /// CAMBIA EL TAMAÑO DE UNA IMAGEN
        /// </summary>
        private Image ResizeImage( Image img, Size size )
        {
            Bitmap bmp = new Bitmap( size.Width, size.Height );
            using(Graphics g = Graphics.FromImage( bmp ))
            {
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.DrawImage( img, 0, 0, size.Width, size.Height );
            }
            return bmp;
        }

        /// <summary>
        /// DAR FORMATO EL BOTON ENVIAR DISPERSIÓN
        /// </summary>
        private void DarFormatoBotonDispersion( )
        {
            btnEnviarCorreos.Size = new Size( 456, 135 );
            btnEnviarCorreos.FlatStyle = FlatStyle.Flat;
            btnEnviarCorreos.FlatAppearance.BorderSize = 0;
            btnEnviarCorreos.FlatAppearance.MouseOverBackColor = Color.FromArgb( 30, 136, 229 ); // Hover azul más claro
            btnEnviarCorreos.FlatAppearance.MouseDownBackColor = Color.FromArgb( 25, 118, 210 ); // Click
            btnEnviarCorreos.BackColor = Color.FromArgb( 0, 123, 255 ); // Azul profesional
            btnEnviarCorreos.ForeColor = Color.White;

            // Fuente bonita y bien visible
            btnEnviarCorreos.Font = new Font( "Segoe UI", 11F, FontStyle.Bold );

            // Imagen y texto bien organizados
            btnEnviarCorreos.Text = "Enviar Dispersión";
            btnEnviarCorreos.Image = Properties.Resources.Mail; // Asegúrate de tener esta imagen
            btnEnviarCorreos.Image = ResizeImage( Properties.Resources.Mail, new Size( 70, 70 ) );
            btnEnviarCorreos.ImageAlign = ContentAlignment.TopCenter;
            btnEnviarCorreos.TextAlign = ContentAlignment.BottomCenter;
            btnEnviarCorreos.Padding = new Padding( 0, 10, 0, 10 ); // Espaciado para que no se amontone

            // Curvatura de esquinas (bordes redondeados)
            btnEnviarCorreos.Region = Region.FromHrgn( CreateRoundRectRgn( 0, 0, btnEnviarCorreos.Width, btnEnviarCorreos.Height, 20, 20 ) );

            // Sombra 3D simulada (opcional)
            btnEnviarCorreos.FlatAppearance.BorderColor = Color.FromArgb( 0, 102, 204 );

        }




        // ✅ Aquí dentro de la clase, pero fuera de otros métodos
        [System.Runtime.InteropServices.DllImport( "Gdi32.dll", EntryPoint = "CreateRoundRectRgn" )]
        private static extern IntPtr CreateRoundRectRgn(
            int nLeftRect, int nTopRect, int nRightRect, int nBottomRect,
            int nWidthEllipse, int nHeightEllipse
        );


        //********************************************************************************************************* //
        //********************************************** CARGA DE DATOS ******************************************* //
        //********************************************************************************************************* //

        /// <summary>
        /// CARGA LOS DATOS DEL GRID VIEW, INCLUYENDO LA UBICACIÓN DEL ARCHIVO (O MENSAJE DE NO ENCONTRADO)
        /// </summary>
        /// <param name="listaDestinatarios"></param>
        private void CargarContactosEnGrid( List<DestinatarioInfo> listaDestinatarios )
        {
            // Crear la tabla
            DataTable dt = new DataTable( );
            dt.Columns.Add( "Contrato", typeof( string ) );
            dt.Columns.Add( "TipoEnvio", typeof( string ) );
            dt.Columns.Add( "Correo", typeof( string ) );
            dt.Columns.Add( "UbicacionArchivo", typeof( string ) ); // Nueva columna

            // Leer las rutas configuradas
            string rutaBase = LeerRutaDesdeConfig( ).Trim( );
            string rutaBaseMandatos = LeerRutaDesdeConfigCarpetaContratosNT( ).Trim( );
            DateTime fechaDispersion = dtpFEchaDeDispersion.Value;

            // Buscar archivos en ambas rutas
            var archivos = BuscarArchivosComprimidos( rutaBase, fechaDispersion );
            var archivosMandatos = BuscarArchivosComprimidosPorFechaTodasEmpresas( rutaBaseMandatos, fechaDispersion );

            // Combinar las listas
            var todosLosArchivos = new List<string>( );
            todosLosArchivos.AddRange( archivos );
            todosLosArchivos.AddRange( archivosMandatos );

            foreach(var d in listaDestinatarios)
            {
                // Buscar el archivo para este contrato
                string archivoEncontrado = todosLosArchivos
                    .FirstOrDefault( a =>
                        (ObtenerContratoDesdeRuta( a ) == d.Contrato) ||
                        (ObtenerContratoDesdeRutaParaMandatos( a ) == d.Contrato)
                    );

                string ubicacion = archivoEncontrado ?? "Archivo no encontrado";

                dt.Rows.Add( d.Contrato, d.TipoDeEnvio, d.Correo, ubicacion );
            }

            dgvContactos.DataSource = dt;

            FormatearDataFridView( ); // Mantiene el formato bonito
        }


        /// <summary>
        ///MÉTODO PARA LEER LA RUTA DESDE EL XML 
        /// </summary>
        /// <returns></returns>
        private string LeerRutaDesdeConfigSistema( )
        {
            string archivoConfig = Path.Combine( Environment.GetFolderPath( Environment.SpecialFolder.MyDocuments ), "DispersionMailer", "Datos", "configRuta.xml" );

            if(!File.Exists( archivoConfig ))
                return string.Empty;

            XmlDocument doc = new XmlDocument( );
            doc.Load( archivoConfig );

            XmlNode nodoRuta = doc.SelectSingleNode( "/RutaConfiguracion/RutaSistema" );
            return nodoRuta?.InnerText ?? string.Empty;
        }

        /// <summary>
        /// CARGA LOS DESTINATARIOS DESDE EL EXCEL A UNA LISTA
        /// </summary>
        /// <param name="rutaArchivoExcel"></param>
        /// <returns></returns>
        public static List<DestinatarioInfo> CargarDestinatariosDesdeExcel( string rutaArchivoExcel )
        {
            var listaDestinatarios = new List<DestinatarioInfo>( );

            if(!File.Exists( rutaArchivoExcel ))
                return listaDestinatarios;

            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;

            using(var package = new ExcelPackage( new FileInfo( rutaArchivoExcel ) ))
            {
                var worksheet = package.Workbook.Worksheets.FirstOrDefault( );
                if(worksheet == null)
                    return listaDestinatarios;

                int rowCount = worksheet.Dimension.Rows;

                for(int row = 2; row <= rowCount; row++)
                {
                    string contrato = worksheet.Cells[ row, 1 ].Text.Trim( );
                    string tipoEnvio = worksheet.Cells[ row, 2 ].Text.Trim( );
                    string correo = worksheet.Cells[ row, 3 ].Text.Trim( );

                    if(!string.IsNullOrEmpty( contrato ) && !string.IsNullOrEmpty( correo ))
                    {
                        listaDestinatarios.Add( new DestinatarioInfo
                        {
                            Contrato = contrato,
                            TipoDeEnvio = tipoEnvio,
                            Correo = correo
                        } );
                    }
                }
            }

            return listaDestinatarios;
        }

        /// <summary>
        /// OBTIENE EL NÚMERO DE CONTRATO A PARTIR DE LA RUTA. Este número de contrato
        /// se utiliza para buscar el destinatario en la lista del Excel.
        /// </summary>
        /// <param name="rutaArchivo">Ruta completa del archivo ZIP.</param>
        /// <returns>Nombre del contrato extraído desde la estructura de carpetas.</returns>
        public static string ObtenerContratoDesdeRuta( string rutaArchivo )
        {
            // Obtener el directorio donde está el archivo
            DirectoryInfo directorio = new DirectoryInfo( Path.GetDirectoryName( rutaArchivo ) );

            // Buscar hacia atrás hasta encontrar un número de 4 dígitos que sea el año (ej. 2025)
            // y tomar el directorio anterior como el contrato
            while(directorio != null)
            {
                if(int.TryParse( directorio.Name, out int posibleAno ) && posibleAno >= 2000 && posibleAno <= 2100)
                {
                    return directorio.Parent?.Name; // El contrato está justo antes del año
                }
                directorio = directorio.Parent;
            }

            return null; // No encontrado
        }

        /// <summary>
        /// OBTIENE EL NÚMERO DE CONTRATO A PARTIR DE LA RUTA. ESTE NÚMERO DE CONTRATO
        /// SE UTILIZA PARA BUSCAR EL DESTINATARIO EN LA LISTA DEL EXCEL EN LOS CONTRATOS DE MANDATOS
        /// </summary>
        /// <param name="rutaArchivo">Ruta completa del archivo ZIP.</param>
        /// <returns>Nombre del contrato extraído desde la estructura de carpetas.</returns>
        public static string ObtenerContratoDesdeRutaParaMandatos( string rutaArchivo )
        {
            try
            {
                // Obtener el directorio que contiene el archivo
                DirectoryInfo directorioArchivo = new DirectoryInfo( Path.GetDirectoryName( rutaArchivo ) );

                // El contrato es el nombre del directorio que contiene directamente el archivo
                return directorioArchivo?.Name;
            } catch
            {
                return null;
            }
        }

        /// <summary>
        /// BUSCA LOS ARCIVOS DE LOS CONTRATOS EN LA RUTA ESTABLECIDA
        /// </summary>
        /// <param name="rutaBase"></param>
        /// <returns></returns>
        public static List<string> BuscarArchivosComprimidos( string rutaBase, DateTime Fecha )
        {
            List<string> archivosEncontrados = new List<string>( );
            string[ ] extensiones = { ".zip", ".rar", ".7z" };

            string añoActual = Fecha.Year.ToString( );
            string mesActual = Fecha.ToString( "MMMM", new CultureInfo( "es-MX" ) ).ToLower( ); // mayo
            string mesAño = $"{mesActual}{Fecha:yy}"; // mayo25
            string mesAbreviadoSinPunto = mesActual.Substring( 0, 3 ); // may
            string diaMesAño = $"{Fecha:dd}{mesAbreviadoSinPunto}{Fecha:yy}"; // 08jun25

            foreach(var contratoPath in Directory.GetDirectories( rutaBase ))
            {
                string rutaAño = Path.Combine( contratoPath, añoActual );
                if(!Directory.Exists( rutaAño ))
                    continue;

                string rutaMes = Path.Combine( rutaAño, mesAño );
                if(!Directory.Exists( rutaMes ))
                    continue;

                string rutaDia = Path.Combine( rutaMes, diaMesAño );
                if(!Directory.Exists( rutaDia ))
                    continue;

                // Solo archivos directamente en la carpeta 'diaMesAño', sin buscar en subdirectorios
                foreach(var archivo in Directory.GetFiles( rutaDia, "*.*", SearchOption.TopDirectoryOnly ))
                {
                    if(extensiones.Any( ext => archivo.EndsWith( ext, StringComparison.OrdinalIgnoreCase ) ))
                        archivosEncontrados.Add( archivo );
                }
            }

            return archivosEncontrados;
        }

        /// <summary>
        /// Busca los archivos comprimidos (.zip, .rar, .7z) dentro de todas las carpetas de empresa en la ruta base,
        /// siguiendo la estructura: [Empresa]\[Año]\[MesAño]\[DíaMesAño]
        /// </summary>
        /// <param name="rutaBase">Ruta raíz que contiene todas las carpetas de empresas</param>
        /// <param name="fecha">Fecha para construir la ruta esperada</param>
        /// <returns>Lista de archivos comprimidos encontrados</returns>
        public static List<string> BuscarArchivosComprimidosPorFechaTodasEmpresas( string rutaBase, DateTime fecha )
        {
            List<string> archivosEncontrados = new List<string>( );
            string[ ] extensiones = { ".zip", ".rar", ".7z" };

            string año = fecha.Year.ToString( );
            string mesNombre = fecha.ToString( "MMMM", new CultureInfo( "es-MX" ) ).ToLower( ); // junio
            string mesAño = $"{mesNombre}{fecha:yy}"; // junio25
            string mesAbreviado = mesNombre.Substring( 0, 3 ); // jun
            string diaMesAño = $"{fecha:dd}{mesAbreviado}{fecha:yy}"; // 16jun25

            // Verifica que la ruta base existe
            if(!Directory.Exists( rutaBase ))
                return archivosEncontrados;

            // Recorre cada carpeta de empresa (BOSCH, BOSCH1, BOSCH2, etc.)
            foreach(var carpetaEmpresa in Directory.GetDirectories( rutaBase ))
            {
                string rutaAño = Path.Combine( carpetaEmpresa, año );
                if(!Directory.Exists( rutaAño ))
                    continue;

                string rutaMes = Path.Combine( rutaAño, mesAño );
                if(!Directory.Exists( rutaMes ))
                    continue;

                string rutaDia = Path.Combine( rutaMes, diaMesAño );
                if(!Directory.Exists( rutaDia ))
                    continue;

                // Cada subcarpeta representa un contrato
                foreach(var carpetaContrato in Directory.GetDirectories( rutaDia ))
                {
                    foreach(var archivo in Directory.GetFiles( carpetaContrato, "*.*", SearchOption.TopDirectoryOnly ))
                    {
                        if(extensiones.Any( ext => archivo.EndsWith( ext, StringComparison.OrdinalIgnoreCase ) ))
                        {
                            archivosEncontrados.Add( archivo );
                        }
                    }
                }
            }

            return archivosEncontrados;
        }

        /// <summary>
        /// BUSCA EL DESTINATARIO CORRESPONDIENTE AL NÚMERO DE CONTRATO EN LA LISTA DE DESTINATARIOS.
        /// </summary>
        /// <param name="contrato">Número de contrato a buscar.</param>
        /// <param name="destinatarios">Lista de destinatarios cargados desde Excel.</param>
        /// <returns>El objeto DestinatarioInfo si se encuentra, o null si no existe coincidencia.</returns>
        public static DestinatarioInfo BuscarDestinatarioPorContrato( string contrato, List<DestinatarioInfo> destinatarios )
        {
            if(string.IsNullOrWhiteSpace( contrato ) || destinatarios == null)
                return null;

            return destinatarios.FirstOrDefault( d => d.Contrato == contrato );
        }

        /// <summary>
        /// BUSCA EL DESTINATARIO PARA POR NUMERO DE CONTRATO
        /// </summary>
        /// <param name="contrato"></param>
        /// <param name="lista"></param>
        /// <returns></returns>
        public static string BuscarDestinatariosParaPorContrato( string contrato, List<DestinatarioInfo> destinatarios )
        {

            var correosPara = destinatarios
                .Where( d => d.Contrato == contrato && d.TipoDeEnvio.Equals( "Para", StringComparison.OrdinalIgnoreCase ) )
                .Select( d => d.Correo.Trim( ) )
                .Distinct( );

            return string.Join( ";", correosPara );

        }

        /// <summary>
        /// BUSCA EL DESTINATARIO CC POR NUMERO DE CONTRATO
        /// </summary>
        /// <param name="contrato"></param>
        /// <param name="lista"></param>
        /// <returns></returns>
        public static string BuscarDestinatariosCCPorContrato( string contrato, List<DestinatarioInfo> destinatarios )
        {

            var correosCC = destinatarios
                .Where( d => d.Contrato == contrato && d.TipoDeEnvio.Equals( "CC", StringComparison.OrdinalIgnoreCase ) )
                .Select( d => d.Correo.Trim( ) )
                .Distinct( );


            return string.Join( ";", correosCC );

        }

        /// <summary>
        /// CREA EL ARCHIVO QUE TIENE LO QUE SUCEDE AL ENVIAR LOS CORREOS
        /// </summary>
        private void LogIrregualridades( )
        {
            // 1. Cargar lista desde el Excel
            List<DestinatarioInfo> listaDestinatarios = CargarDestinatariosDesdeExcel(
                Path.Combine( LeerRutaDesdeConfigSistema( ), "ListaDeDistribucion.xlsx" )
            );

            // 2. Buscar archivos de la estructura estándar
            List<string> archivosEncontrados = BuscarArchivosComprimidos(
                LeerRutaDesdeConfig( ).Trim( ), dtpFEchaDeDispersion.Value
            );

            // 3. Buscar archivos de la estructura para mandatos
            List<string> archivosEncontradosMandatos = BuscarArchivosComprimidosPorFechaTodasEmpresas(
                LeerRutaDesdeConfigCarpetaContratosNT( ).Trim( ), dtpFEchaDeDispersion.Value
            );

            // 4. Obtener contratos desde la estructura estándar
            HashSet<string> contratosEnCarpeta = new HashSet<string>(
                archivosEncontrados.Select( path => ObtenerContratoDesdeRuta( path ) )
                    .Where( c => !string.IsNullOrWhiteSpace( c ) )
            );

            // 5. Obtener contratos desde la estructura de mandatos
            HashSet<string> contratosEnCarpetaMandatos = new HashSet<string>(
                archivosEncontradosMandatos.Select( path => ObtenerContratoDesdeRutaParaMandatos( path ) )
                    .Where( c => !string.IsNullOrWhiteSpace( c ) )
            );

            // 6. Unir ambos conjuntos
            contratosEnCarpeta.UnionWith( contratosEnCarpetaMandatos );

            // 7. Obtener contratos desde el Excel
            HashSet<string> contratosEnExcel = new HashSet<string>(
                listaDestinatarios.Select( d => d.Contrato.Trim( ) )
            );

            // 8. Contratos que están en carpeta pero NO en Excel
            var contratosSinDestinatarios = contratosEnCarpeta.Except( contratosEnExcel ).ToList( );

            // 9. Contratos que están en Excel pero NO en carpeta
            var destinatariosSinContratos = contratosEnExcel.Except( contratosEnCarpeta ).ToList( );

            // 10. Guardar ambos reportes en Excel
            string nombreArchivo = Path.Combine(
                LeerRutaDesdeConfigSistema( ).Trim( ),
                "IrregularidadesEncontradas" + DateTime.Now.ToString( "yyyyMMMMdd" ) + ".xlsx"
            );

            GuardarListaEnExcel( nombreArchivo, "ContratosSinDestinatario", contratosSinDestinatarios );
            GuardarListaEnExcel( nombreArchivo, "DestinatariosSinContrato", destinatariosSinContratos );
        }


        /// <summary>
        /// ENVIA EL CORREO A LOS DESTINATARIOS
        /// </summary>        
        public static bool EnviarCorreo( string destinatarioMail, string destinatarioCC, string asunto, string cuerpoHtml, string archivoAdjunto )
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            using(MailMessage mensaje = new MailMessage( ))
            {
                mensaje.From = new MailAddress( "tesoreriasammx@santanderam.com", "Tesoreria.SAMMX", System.Text.Encoding.UTF8 );

                //Agregar lista de destinatarios
                string[ ] correos = destinatarioMail.Split( ';' );
                foreach(string correo in correos)
                {
                    mensaje.To.Add( correo.Trim( ) );
                }


                // Agregar destinatarios en copia
                if(!string.IsNullOrWhiteSpace( destinatarioCC ))
                {
                    string[ ] correosCC = destinatarioCC.Split( ';' );
                    foreach(string correoCC in correosCC)
                    {
                        if(!string.IsNullOrWhiteSpace( correoCC ))
                        {
                            mensaje.CC.Add( correoCC.Trim( ) );
                        }
                    }
                }


                mensaje.Subject = asunto;
                mensaje.Body = cuerpoHtml;
                mensaje.IsBodyHtml = true;
                mensaje.Priority = MailPriority.Normal;
                mensaje.BodyEncoding = Encoding.UTF8;
                mensaje.SubjectEncoding = Encoding.UTF8;

                if(!string.IsNullOrEmpty( archivoAdjunto ))
                {
                    mensaje.Attachments.Add( new Attachment( archivoAdjunto ) );
                }

                using(SmtpClient smtp = new SmtpClient( ))
                {
                    smtp.UseDefaultCredentials = false;
                    smtp.Host = "180.176.74.200"; //Host del servidor de correo
                    smtp.Port = 25; //Puerto de salida
                    smtp.Credentials = new System.Net.NetworkCredential( "tesoreriasammx@santanderam.com", "" );
                    ServicePointManager.ServerCertificateValidationCallback = delegate ( object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors )
                    { return true; };
                    smtp.EnableSsl = false;
                    smtp.Timeout = 30000;

                    try
                    {
                        smtp.Send( mensaje );
                        return true;
                    } catch(SmtpException ex)
                    {
                        string error = $"Error al enviar el correo a {destinatarioMail}: {ex.Message}";
                        Logger.RegistrarErrorEnLog( "[" + DateTime.Now.ToString( "yyyy-MM-dd HH:mm:ss" ) + "] " + error );
                        return false;
                    }
                }
            }
        }

        /// <summary>
        /// GUARDA CORREOS EN EXCEL
        /// </summary>
        /// <param name="ruta"></param>
        /// <param name="nombreHoja"></param>
        /// <param name="listaDestinatarios"></param>
        public static void GuardarCorreosEnExcel( string ruta, string nombreHoja, List<DestinatarioInfo> listaDestinatarios )
        {
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;

            // Filtrar la lista de destinatarios para eliminar los registros nulos o vacíos
            var listaFiltrada = listaDestinatarios.Where( dest => dest != null &&
                                                               !string.IsNullOrEmpty( dest.Contrato ) &&
                                                               !string.IsNullOrEmpty( dest.Correo ) &&
                                                               !string.IsNullOrEmpty( dest.TipoDeEnvio ) ).ToList( );

            FileInfo archivo = new FileInfo( ruta );

            // Si el archivo ya existe, lo abrimos, si no, lo creamos
            using(var package = archivo.Exists ? new ExcelPackage( archivo ) : new ExcelPackage( ))
            {
                // Intentamos obtener la hoja que ya existe
                var ws = package.Workbook.Worksheets.FirstOrDefault( s => s.Name == nombreHoja );

                // Si no existe, la creamos
                if(ws == null)
                {
                    ws = package.Workbook.Worksheets.Add( nombreHoja );
                    ws.Cells[ 1, 1 ].Value = "Contrato";        // Encabezado
                    ws.Cells[ 1, 2 ].Value = "Correo";          // Encabezado
                    ws.Cells[ 1, 3 ].Value = "Tipo de Envío";   // Encabezado
                }

                // Buscamos la fila disponible para agregar los nuevos datos
                int fila = ws.Dimension?.Rows + 1 ?? 2;

                // Agregar la información de cada destinatario a las celdas
                for(int i = 0; i < listaFiltrada.Count; i++)
                {
                    var destinatario = listaFiltrada[ i ];
                    ws.Cells[ fila + i, 1 ].Value = destinatario.Contrato;      // Contrato
                    ws.Cells[ fila + i, 2 ].Value = destinatario.Correo;        // Correo
                    ws.Cells[ fila + i, 3 ].Value = destinatario.TipoDeEnvio;     // Tipo de Envío
                }

                // Guardamos el archivo Excel sin sobrescribir
                package.SaveAs( archivo );
            }
        }

        /// <summary>
        /// DESCARGAR ARCHIVOS CON DIALOGOS
        /// </summary>
        /// <param name="rutaOrigen"></param>
        public static void DescargarArchivoConDialogo( string rutaOrigen )
        {
            // Verificar si el archivo existe en la ruta de origen
            if(File.Exists( rutaOrigen ))
            {
                // Crear el dialogo para que el usuario elija la carpeta de destino
                using(FolderBrowserDialog folderDialog = new FolderBrowserDialog( )) // Aquí le puedes poner "folderDialog"
                {
                    // Establecer una descripción personalizada
                    folderDialog.Description = "Selecciona la carpeta para guardar el archivo";

                    // Permitir que el usuario cree nuevas carpetas
                    folderDialog.ShowNewFolderButton = true;

                    // Establecer la carpeta inicial, si lo deseas, por ejemplo, la carpeta de documentos
                    folderDialog.SelectedPath = @"C:\"; // O puedes usar Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)

                    // Si el usuario selecciona una carpeta, continuamos
                    if(folderDialog.ShowDialog( ) == DialogResult.OK)
                    {
                        // Obtener la ruta de la carpeta seleccionada
                        string rutaDestino = Path.Combine( folderDialog.SelectedPath, Path.GetFileName( rutaOrigen ) );

                        // Copiar el archivo de la ruta origen a la ruta destino
                        try
                        {
                            File.Copy( rutaOrigen, rutaDestino, true ); // 'true' permite sobrescribir si ya existe
                            MessageBox.Show( "Archivo descargado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information );
                        } catch(Exception ex)
                        {
                            MessageBox.Show( $"Error al intentar descargar el archivo: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
                        }
                    } else
                    {
                        MessageBox.Show( "No se seleccionó ninguna carpeta. El archivo no se descargará.", "Cancelado", MessageBoxButtons.OK, MessageBoxIcon.Information );
                    }
                }
            } else
            {
                MessageBox.Show( "El archivo no existe en la ruta especificada.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
            }
        }

        /// <summary>
        /// GUARDAR LISTA EN EXCEL
        /// </summary>
        /// <param name="ruta"></param>
        /// <param name="nombreHoja"></param>
        /// <param name="lista"></param>
        public static void GuardarListaEnExcel( string ruta, string nombreHoja, List<string> lista )
        {
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;

            FileInfo archivo = new FileInfo( ruta );

            // Si el archivo ya existe, lo abrimos, si no, lo creamos
            using(var package = archivo.Exists ? new ExcelPackage( archivo ) : new ExcelPackage( ))
            {
                // Intentamos obtener la hoja que ya existe, si no la encontramos, la creamos
                var ws = package.Workbook.Worksheets.FirstOrDefault( s => s.Name == nombreHoja );

                if(ws == null)
                {
                    // Si no existe, la creamos
                    ws = package.Workbook.Worksheets.Add( nombreHoja );
                    ws.Cells[ 1, 1 ].Value = "Contratos";  // Encabezado de la columna
                }

                // Buscamos la fila disponible para agregar los nuevos datos
                int fila = ws.Dimension?.Rows + 1 ?? 2;

                // Agregar los contratos a la hoja
                for(int i = 0; i < lista.Count; i++)
                {
                    ws.Cells[ fila + i, 1 ].Value = lista[ i ];
                }

                // Guardamos los cambios en el archivo sin borrarlo
                package.SaveAs( archivo );
            }
        }

        /// <summary>
        /// MÉTODO PARA LEER LA RUTA DESDE EL XML PARA LOS CONTRATOS DE DISPERSIÓN
        /// </summary>
        /// <returns></returns>
        private string LeerRutaDesdeConfig( )
        {
            string archivoConfig = Path.Combine( Environment.GetFolderPath( Environment.SpecialFolder.MyDocuments ), "DispersionMailer", "Datos", "configRuta.xml" );

            if(!File.Exists( archivoConfig ))
                return string.Empty;

            XmlDocument doc = new XmlDocument( );
            doc.Load( archivoConfig );

            XmlNode nodoRuta = doc.SelectSingleNode( "/RutaConfiguracion/RutaCarpeta" );
            return nodoRuta?.InnerText ?? string.Empty;
        }

        /// <summary>
        /// OBTIENE LAS DIFERENCIAS ENTRE EL LISTADO DE CONTACTOS Y LOS CONTRATOS DENTRO DE LAS CARPETAS
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnContactosSinContrato_Click( object sender, EventArgs e )
        {
            // Cargar contactos desde el Excel
            List<DestinatarioInfo> listaDestinatarios = CargarDestinatariosDesdeExcel(
                Path.Combine( LeerRutaDesdeConfigSistema( ), "ListaDeDistribucion.xlsx" ) );

            // Obtener los contratos presentes en los archivos de la carpeta
            List<string> archivosEncontrados = BuscarArchivosComprimidos( LeerRutaDesdeConfig( ).Trim( ), dtpFEchaDeDispersion.Value );
            // Obtener los contratos presentes en los archivos de la carpeta mandatos
            List<string> archivosEncontradosMandatos = BuscarArchivosComprimidosPorFechaTodasEmpresas( LeerRutaDesdeConfigCarpetaContratosNT( ).Trim( ), dtpFEchaDeDispersion.Value );


            HashSet<string> contratosEnCarpeta = new HashSet<string>(
                archivosEncontrados.Select( path => ObtenerContratoDesdeRuta( path ) )
                .Where( c => !string.IsNullOrWhiteSpace( c ) ) );

            HashSet<string> contratosEnCarpetaMandatos = new HashSet<string>(
                archivosEncontradosMandatos.Select( path => ObtenerContratoDesdeRutaParaMandatos( path ) )
                .Where( c => !string.IsNullOrWhiteSpace( c ) ) );

            //Junto todo en una lista, los contratos Clientes en Directo y Mandatos
            contratosEnCarpeta.UnionWith( contratosEnCarpetaMandatos );

            // Filtrar los destinatarios cuyo contrato NO está en la carpeta
            var destinatariosSinArchivo = listaDestinatarios
                .Where( d => !contratosEnCarpeta.Contains( d.Contrato.Trim( ) ) )
                .ToList( );

            // Mostrar en el DataGridView
            dgvContactos.DataSource = destinatariosSinArchivo;

            //// También exportar el archivo Excel
            //string nombreArchivo = Path.Combine(
            //    LeerRutaDesdeConfigSistema( ).Trim( ),
            //    "IrregularidadesEncontradas" + DateTime.Now.ToString( "yyyyMMMMdd" ) + ".xlsx" );

            //GuardarListaEnExcel( nombreArchivo, "DestinatariosSinContrato", destinatariosSinArchivo );
        }

        /// <summary>
        /// OBTIENE LOS CONTRATOS QUE SE ENCUENTRAN EN LAS CARPETAS Y NO TIENEN CONTACTOS EN EL EXCEL
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnContratosSinContacto_Click( object sender, EventArgs e )
        {
            // 1. Cargar todos los destinatarios del Excel
            List<DestinatarioInfo> listaDestinatarios = CargarDestinatariosDesdeExcel(
                Path.Combine( LeerRutaDesdeConfigSistema( ), "ListaDeDistribucion.xlsx" ) );

            // 2. Obtener todos los archivos de la carpeta y extraer contratos de Clientes en Directo y Mandatos
            List<string> archivosEncontrados = BuscarArchivosComprimidos( LeerRutaDesdeConfig( ).Trim( ), dtpFEchaDeDispersion.Value );
            List<string> archivosEncontradosMandatos = BuscarArchivosComprimidosPorFechaTodasEmpresas( LeerRutaDesdeConfigCarpetaContratosNT( ).Trim( ), dtpFEchaDeDispersion.Value );

            HashSet<string> contratosEnCarpeta = new HashSet<string>(
                archivosEncontrados.Select( path => ObtenerContratoDesdeRuta( path ) )
                .Where( c => !string.IsNullOrWhiteSpace( c ) ) );

            HashSet<string> contratosEnCarpetaMandatos = new HashSet<string>(
                archivosEncontradosMandatos.Select( path => ObtenerContratoDesdeRutaParaMandatos( path ) )
                .Where( c => !string.IsNullOrWhiteSpace( c ) ) );

            //Se unen todos los contratos encontrados en carpetas en una lista de contratos
            contratosEnCarpeta.UnionWith( contratosEnCarpetaMandatos );

            // 3. Obtener los contratos que sí están en Excel
            HashSet<string> contratosEnExcel = new HashSet<string>(
                listaDestinatarios.Select( d => d.Contrato.Trim( ) ) );

            // 4. Contratos que están en carpeta pero no en Excel
            var contratosSinDestinatario = contratosEnCarpeta.Except( contratosEnExcel ).ToList( );

            // 5. Construir lista de DestinatarioInfo con campos vacíos
            List<DestinatarioInfo> listaIncompleta = contratosSinDestinatario
                .Select( c => new DestinatarioInfo
                {
                    Contrato = c,
                    Correo = "",         // Campos vacíos como indicas
                    TipoDeEnvio = ""
                } )
                .ToList( );

            // 6. Mostrar en el DataGridView
            dgvContactos.DataSource = listaIncompleta;

            //// 7. Exportar a Excel
            //string nombreArchivo = Path.Combine(
            //    LeerRutaDesdeConfigSistema( ).Trim( ),
            //    "ContratosSinDestinatario_" + DateTime.Now.ToString( "yyyyMMdd_HHmmss" ) + ".xlsx" );

            //GuardarListaEnExcel( nombreArchivo, "ContratosSinDestinatario", listaIncompleta );
        }

        /// <summary>
        /// VUELVE A CARGAR EL GRIDVIEW AL MOMENTO QUE SE SELECCIONA OTRA FECHA
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dtpFEchaDeDispersion_ValueChanged( object sender, EventArgs e )
        {
            // Volver a cargar lista de destinatarios
            string rutaExcel = Path.Combine( LeerRutaDesdeConfigSistema( ), "ListaDeDistribucion.xlsx" );

            if(File.Exists( rutaExcel ))
            {
                var lista = CargarDestinatariosDesdeExcel( rutaExcel );
                CargarContactosEnGrid( lista );
            } else
            {
                MessageBox.Show( "La lista de distribución no fue encontrada. Si es la primera vez que ejecutas, por favor cárgala.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
            }
        }


    }
}
