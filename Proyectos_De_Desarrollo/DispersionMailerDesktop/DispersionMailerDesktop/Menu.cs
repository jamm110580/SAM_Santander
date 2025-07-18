using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Xml;
using System.Drawing.Drawing2D;

namespace DispersionMailerDesktop
{
    public partial class Menu : Form
    {
        public Menu( )
        {
            InitializeComponent( );
        }

        private void dispersiónToolStripMenuItem_Click( object sender, EventArgs e )
        {
            AbrirFormularioEnPanel( new FormDispersion( ) );
        }

        private void configuraciónToolStripMenuItem_Click( object sender, EventArgs e )
        {
            AbrirFormularioEnPanel( new FormConfiguracion( ) );
        }

        private void capturaDeContratoToolStripMenuItem_Click( object sender, EventArgs e )
        {
            AbrirFormularioEnPanel( new CapturaContrato( ) );
        }

        private void configuraciónToolStripMenuItem1_Click( object sender, EventArgs e )
        {
            AbrirFormularioEnPanel( new ConfiguracionContratos( ) );
        }

        private void AbrirFormularioEnPanel( Form formulario )
        {
            // Limpia controles anteriores
            panelContenedor.Controls.Clear( );
            // Asegura que el panel se ajuste al formulario
            panelContenedor.Dock = DockStyle.Fill;

            // Configura el formulario para ser embebido
            formulario.TopLevel = false;
            formulario.FormBorderStyle = FormBorderStyle.None;
            formulario.Dock = DockStyle.None; // <- Importante para permitir el centrado manual

            // Agrega el formulario al panel
            panelContenedor.Controls.Add( formulario );
            panelContenedor.Tag = formulario;

            // Centrar manualmente dentro del panel
            formulario.Location = new Point(
                (panelContenedor.Width - formulario.Width) / 2,
                (panelContenedor.Height - formulario.Height) / 2
            );

            formulario.Show( );
        }

        private void Menu_Load( object sender, EventArgs e )
        {
            VerificarYCrearRutaSistema( );  //En caso de que sea la primera vez que se corre el programa se crea una carpeta para los archivos de sistema.


            PonerImagenFondoPanel( );

            // Asegura que el panel se ajuste al formulario
            panelContenedor.Dock = DockStyle.Fill;

        }

        /// <summary>
        /// PONE LA IMAGEN DEL FONDO A EL CONTENEDOR DE LOS FORMULARIOS
        /// </summary>
        private void PonerImagenFondoPanel( )
        {
            // Poner el Backgrou de mi panelContenedor
            using(MemoryStream ms = new MemoryStream( Properties.Resources.OficinaFondo ))
            {
                panelContenedor.BackgroundImage = Image.FromStream( ms );
            }
            panelContenedor.BackgroundImageLayout = ImageLayout.Stretch;
        }

        private void inicioToolStripMenuItem_Click( object sender, EventArgs e )
        {
            VolverAlInicio( );
        }

        private void VolverAlInicio( )
        {
            // Cierra el formulario embebido actual, si existe
            if(panelContenedor.Controls.Count > 0)
            {
                Form formularioActual = panelContenedor.Controls[ 0 ] as Form;
                if(formularioActual != null)
                {
                    formularioActual.Close( );
                    formularioActual.Dispose( );
                }

                // Limpia los controles del panel
                panelContenedor.Controls.Clear( );
            }

        }


        /// <summary>
        /// METODOD E LA CONFIGURACION INICIAL PARA CREAR LA CARPETA DE SISTEMA
        /// </summary>
        public void VerificarYCrearRutaSistema( )
        {
            // Obtener la ruta de "Documentos" del usuario
            string rutaBase = Path.Combine( Environment.GetFolderPath( Environment.SpecialFolder.MyDocuments ), "DispersionMailer", "Datos" );

            // Verificar si la ruta existe
            if(!Directory.Exists( rutaBase ))
            {
                // Si no existe, crearla
                Directory.CreateDirectory( rutaBase );
                GuardarRutaEnConfigSistema( rutaBase );
            }
        }


        /// <summary>
        /// METODO QUE GUARDA LA RUTA XML DE LA CARPETA DE LOS CONTRATOS
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

       
    }
}
