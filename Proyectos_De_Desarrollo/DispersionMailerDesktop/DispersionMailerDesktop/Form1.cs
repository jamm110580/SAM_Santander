using System.Drawing.Drawing2D;
using DispersionMailerDesktop.Model;
using System.Collections.Generic;
using System.Data;
using DispersionMailerDesktop.Helpers;
using System.Windows.Forms;
using System.Xml;
using OfficeOpenXml;
using System.Globalization;
using System.Net.Mail;
using System.Net.Security;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using DispersionMailerDesktop.Utilidades;


namespace DispersionMailerDesktop
{
    public partial class FormConfiguracion : Form
    {
        public FormConfiguracion( )
        {
            InitializeComponent( );
        }

        private void Form1_Load( object sender, EventArgs e )
        {
            //Aplica el estilo de los Paneles para que se vean curvados en las esquinas.
            AplicaEstilos( );

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

            //Lee la ruta donde estan las carpeta de los contratos y las pone en el TextBox
            tbRutaArchvosDispersion.Text = LeerRutaDesdeConfig( );

            //Lee la ruta donde estan las carpeta de los contratos no tradicionales y las pone en el TextBox
            tbRutaArchvosDispersionNT.Text = LeerRutaDesdeConfigCarpetaContratosNT( );

            //Lee la ruta donde estan la carpeta de sistema y las pone en el TextBox
            txtRutaCarpetaSistema.Text = LeerRutaDesdeConfigSistema( );
        }


        //********************************************************************************************************* //
        //**************************************** METODOS DE CONTROLES ******************************************* //
        //********************************************************************************************************* //

        /// <summary>
        /// ACTUALIZA EL EXCEL DE LOS CONTACTOS DENTRO DEL SISTEMA
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnActualizarContratos_Click( object sender, EventArgs e )
        {
            // Validar si se seleccionó un archivo con el OpenFileDialog
            if(!string.IsNullOrWhiteSpace( tbRutaArchivoContacots.Text ) && File.Exists( tbRutaArchivoContacots.Text ))
            {
                // Ruta de la carpeta de destino donde se guarda el archivo
                string rutaCarpetaArchivos = LeerRutaDesdeConfigSistema( );

                // Asegurarse de que la carpeta existe
                if(!Directory.Exists( rutaCarpetaArchivos ))
                {
                    Directory.CreateDirectory( rutaCarpetaArchivos );
                }

                // Borrar solo archivos de Excel en la carpeta
                string[ ] extensionesExcel = { ".xlsx", ".xls" };
                foreach(string file in Directory.GetFiles( rutaCarpetaArchivos ))
                {
                    if(extensionesExcel.Contains( Path.GetExtension( file ), StringComparer.OrdinalIgnoreCase ))
                    {
                        File.Delete( file );
                    }
                }

                // Copiar el nuevo archivo
                string nuevoArchivoOrigen = tbRutaArchivoContacots.Text;
                string nuevoNombreArchivo = "ListaDeDistribucion.xlsx"; // Siempre se renombra igual
                string rutaDestinoCompleta = Path.Combine( rutaCarpetaArchivos, nuevoNombreArchivo );
                File.Copy( nuevoArchivoOrigen, rutaDestinoCompleta );

                // Recargar datos al DataGridView
                var lista = ExcelHelper.CargarDestinatariosDesdeExcel( rutaDestinoCompleta );
                CargarContactosEnGrid( lista );

                // Confirmación (puedes mostrar en un Label o MessageBox)
                MessageBox.Show( "Lista de distribución cargada correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information );
            } else
            {
                MessageBox.Show( "Por favor selecciona un archivo válido.", "Archivo no válido", MessageBoxButtons.OK, MessageBoxIcon.Warning );
            }
        }

        /// <summary>
        /// SELECCIONA EL ARCHIVO DE EXCEL DE CONTACTOS QUE VA A SER SUBIDO AL SISTEMA
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSeleccionarArchivosContactos_Click( object sender, EventArgs e )
        {
            OFDArchivoExcel.Filter = "Archivos de Excel (*.xlsx)|*.xlsx";
            OFDArchivoExcel.Title = "Selecciona el nuevo archivo de contactos";

            if(OFDArchivoExcel.ShowDialog( ) == DialogResult.OK)
            {
                tbRutaArchivoContacots.Text = OFDArchivoExcel.FileName;
            }
        }

        /// <summary>
        /// SELECCIONA LA RUTA DE LA CARPETA QUE VA A FUNCIONAR PARA IR A BUSCAR LOS CONTRATOS QUE SE VAN A DISPERSAR
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSeleccionaCarpetaRuta_Click( object sender, EventArgs e )
        {
            if(fbdRutaCarpetaDispersion.ShowDialog( ) == DialogResult.OK)
            {
                tbRutaArchvosDispersion.Text = fbdRutaCarpetaDispersion.SelectedPath;
            }
        }

        /// <summary>
        /// GUARDA EN EL ARCHIVO XML LA RUTA DONDE SE ALOJARAN LOS CONTRATOS QUE SE DISPERSARAN
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnActualizarRuta_Click( object sender, EventArgs e )
        {
            string ruta = tbRutaArchvosDispersion.Text.Trim( );

            if(string.IsNullOrEmpty( ruta ))
            {
                MessageBox.Show( "Por favor, ingresa o selecciona una ruta válida.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning );
                return;
            }

            try
            {
                GuardarRutaEnConfig( ruta );
                MessageBox.Show( "Ruta guardada correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information );

                // Opcional: refrescar el TextBox con la ruta que se guardó
                tbRutaArchvosDispersion.Text = LeerRutaDesdeConfig( );
            } catch(Exception ex)
            {
                MessageBox.Show( "Error al guardar la ruta: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
            }
        }

        /// <summary>
        /// CON ESTE BOTON SE PUEDE ACTUALIZAR LA HUBICACION DE LA RUTA DEL ARCHIVO XML, DONDE SE GUARDAN TODAS LAS RUTAS DE SISTEMA
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRutaCarpetaSistema_Click( object sender, EventArgs e )
        {
            string ruta = txtRutaCarpetaSistema.Text.Trim( );

            if(string.IsNullOrEmpty( ruta ))
            {
                MessageBox.Show( "Por favor, ingresa o selecciona una ruta válida.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning );
                return;
            }

            try
            {
                GuardarRutaEnConfigSistema( ruta );
                MessageBox.Show( "Ruta guardada correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information );

                // Opcional: refrescar el TextBox con la ruta que se guardó
                tbRutaArchvosDispersion.Text = LeerRutaDesdeConfig( );
            } catch(Exception ex)
            {
                MessageBox.Show( "Error al guardar la ruta: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
            }
        }

        /// <summary>
        /// BOTON PARA SELECCIONAR LA RUTA DE DONDE SE BUSCARAN LAS CARPETAS DE LOS CONTRATOS 
        /// Y LA PINTA EN EL TEXTBOX. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSeleccionaCarpetaRutaSistema_Click( object sender, EventArgs e )
        {
            if(fbdCarpetaSistema.ShowDialog( ) == DialogResult.OK)
            {
                txtRutaCarpetaSistema.Text = fbdCarpetaSistema.SelectedPath;
            }
        }

        /// <summary>
        /// CIERRA LA VENTANA ACTUAL
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCerrar_Click( object sender, EventArgs e )
        {
            this.Close( );
        }


        /// <summary>
        /// GUARDA LA RUTA DE LA CARPETA DONDE ESTARAN LOS CONTRATOS NO TRADICIONALES
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnActualizarRutaContratos_Click( object sender, EventArgs e )
        {
            string ruta = tbRutaArchvosDispersionNT.Text.Trim( );

            if(string.IsNullOrEmpty( ruta ))
            {
                MessageBox.Show( "Por favor, ingresa o selecciona una ruta válida.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning );
                return;
            }

            try
            {
                GuardarRutaEnConfigCarpetaContratosNT( ruta );
                MessageBox.Show( "Ruta guardada correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information );

                // Opcional: refrescar el TextBox con la ruta que se guardó
                tbRutaArchvosDispersionNT.Text = LeerRutaDesdeConfigCarpetaContratosNT( );
            } catch(Exception ex)
            {
                MessageBox.Show( "Error al guardar la ruta: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
            }
        }


        /// <summary>
        /// SELECCIONA LA RUTA DE LOS CONTRATOS NO TRADICIONALES PARA PONERLOS EN EL TEXTBOX
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSeleccionaCarpetaRutaNT_Click( object sender, EventArgs e )
        {
            if(fbdCarpetaSistemaNT.ShowDialog( ) == DialogResult.OK)
            {
                tbRutaArchvosDispersionNT.Text = fbdCarpetaSistemaNT.SelectedPath;
            }
        }


        //********************************************************************************************************* //
        //******************************************** METODOS FUNCIONALES **************************************** //
        //********************************************************************************************************* //

        /// <summary>
        /// CARGA EL LISTADO DE EXCEL EN EL GRIDVIEW
        /// </summary>
        /// <param name="listaDestinatarios"></param>
        private void CargarContactosEnGrid( List<DestinatarioInfo> listaDestinatarios )
        {
            DataTable dt = new DataTable( );
            dt.Columns.Add( "Contrato", typeof( string ) );
            dt.Columns.Add( "TipoEnvio", typeof( string ) );
            dt.Columns.Add( "Correo", typeof( string ) );

            foreach(var d in listaDestinatarios)
            {
                dt.Rows.Add( d.Contrato, d.TipoDeEnvio, d.Correo );
            }

            dgvContactos.DataSource = dt; // Asegúrate de que este sea el nombre de tu DataGridView

            FormatearDataFridView( );  //Formatea el GridView para que tenga cabecera y abarque todo el espacio
        }

        /// <summary>
        /// METODO QUE GUARDA LA RUTA XML DE LA CARPETA DE LOS CONTRATOS
        /// </summary>
        /// <param name="ruta"></param>
        private void GuardarRutaEnConfig( string ruta )
        {
            string archivoConfig = Path.Combine( Environment.GetFolderPath( Environment.SpecialFolder.MyDocuments ), "DispersionMailer", "Datos", "configRuta.xml" );

            XmlDocument doc = new XmlDocument( );

            if(File.Exists( archivoConfig ))
            {
                doc.Load( archivoConfig );

                XmlNode nodoRuta = doc.SelectSingleNode( "/RutaConfiguracion/RutaCarpeta" );
                if(nodoRuta != null)
                {
                    nodoRuta.InnerText = ruta;
                } else
                {
                    XmlElement nuevoNodoRuta = doc.CreateElement( "RutaCarpeta" );
                    nuevoNodoRuta.InnerText = ruta;
                    doc.DocumentElement.AppendChild( nuevoNodoRuta );
                }
            } else
            {
                XmlElement root = doc.CreateElement( "RutaConfiguracion" );
                XmlElement nodoRuta = doc.CreateElement( "RutaCarpeta" );
                nodoRuta.InnerText = ruta;
                root.AppendChild( nodoRuta );
                doc.AppendChild( root );
            }

            doc.Save( archivoConfig );
        }

        /// <summary>
        /// METODO QUE GUARDA LA RUTA XML DE LA CARPETA DE LOS CONTRATOS
        /// </summary>
        /// <param name="ruta"></param>
        private void GuardarRutaEnConfigCarpetaContratosNT( string ruta )
        {
            string archivoConfig = Path.Combine( Environment.GetFolderPath( Environment.SpecialFolder.MyDocuments ), "DispersionMailer", "Datos", "configRuta.xml" );

            XmlDocument doc = new XmlDocument( );

            if(File.Exists( archivoConfig ))
            {
                doc.Load( archivoConfig );

                XmlNode nodoRuta = doc.SelectSingleNode( "/RutaConfiguracion/RutaCarpetaContratosNT" );
                if(nodoRuta != null)
                {
                    nodoRuta.InnerText = ruta;
                } else
                {
                    XmlElement nuevoNodoRuta = doc.CreateElement( "RutaCarpetaContratosNT" );
                    nuevoNodoRuta.InnerText = ruta;
                    doc.DocumentElement.AppendChild( nuevoNodoRuta );
                }
            } else
            {
                XmlElement root = doc.CreateElement( "RutaConfiguracion" );
                XmlElement nodoRuta = doc.CreateElement( "RutaCarpetaContratosNT" );
                nodoRuta.InnerText = ruta;
                root.AppendChild( nodoRuta );
                doc.AppendChild( root );
            }

            doc.Save( archivoConfig );
        }

        /// <summary>
        /// MÉTODO PARA LEER LA RUTA DESDE DEL XML PARA LOS CONTRATOS DE DISPERSIÓN
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

        /// <summary>
        ///MÉTODO PARA LEER LA RUTA DE LOS ARCHIVOS DEL SISTEMA EN EL XML 
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
        /// LEE EL ARCHIVO DE EXCEL Y CARGA LOS DESTINATARIOS A UNA LISTA
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
        /// METODO QUE GUARDA LA RUTA DEL ARCHIVO XML (ARCHIVO QUE CONTIENE LAS RUTAS DE TODOS LOS ARCHIVOS DEL SISTEMA)
        /// </summary>
        /// <param name="ruta"></param>
        private void GuardarRutaEnConfigSistema( string ruta )
        {
            string archivoConfig = Path.Combine( ruta, "configRuta.xml" );

            XmlDocument doc = new XmlDocument( );

            if(File.Exists( archivoConfig ))
            {
                doc.Load( archivoConfig );

                XmlNode nodoRuta = doc.SelectSingleNode( "/RutaConfiguracion/RutaSistema" );
                if(nodoRuta != null)
                {
                    nodoRuta.InnerText = ruta;
                } else
                {
                    XmlElement nuevoNodoRuta = doc.CreateElement( "RutaSistema" );
                    nuevoNodoRuta.InnerText = ruta;
                    doc.DocumentElement.AppendChild( nuevoNodoRuta );
                }
            } else
            {
                XmlElement root = doc.CreateElement( "RutaConfiguracion" );
                XmlElement nodoRuta = doc.CreateElement( "RutaSistema" );
                nodoRuta.InnerText = ruta;
                root.AppendChild( nodoRuta );
                doc.AppendChild( root );
            }

            doc.Save( archivoConfig );
        }

        //********************************************************************************************************* //
        //********************************************** APLICACION DE ESTILOS ******************************************* //
        //********************************************************************************************************* //

        /// <summary>
        /// APLICACION GENERAL DE ESTILOS
        /// </summary>
        private void AplicaEstilos( )
        {
            RedondearPanel( pnlActualizaContratos );
            RedondearPanel( pnlRutaDispersion );
            RedondearPanel( panel1 );
            RedondearPanel( pnlActualizaContratos );
            FormatoBotonCerrar( );
        }

        /// <summary>
        /// REDONDEA LAS ESQUINAS DE CUALQUIER PANEL
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
        /// APLICA ESTILO AL GRIDVIEW
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



        // ✅ Aquí dentro de la clase, pero fuera de otros métodos
        [System.Runtime.InteropServices.DllImport( "Gdi32.dll", EntryPoint = "CreateRoundRectRgn" )]
        private static extern IntPtr CreateRoundRectRgn(
            int nLeftRect, int nTopRect, int nRightRect, int nBottomRect,
            int nWidthEllipse, int nHeightEllipse
        );


        //********************************************************************************************************* //
        //********************************************************************************************************* //
        //********************************************************************************************************* //


    }
}
