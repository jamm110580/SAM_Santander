using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics.Contracts;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DispersionMailerDesktop
{
    public partial class CapturaContrato : Form
    {
        string connectionString = ConfigurationManager.ConnectionStrings[ "MyDatabaseConnection" ].ConnectionString;
        string usuarioWindows = Environment.UserName;

        public CapturaContrato( )
        {
            InitializeComponent( );
        }

        private void CapturaContrato_Load( object sender, EventArgs e )
        {
            //Valida que el usuario que esta ingresando exista como usuario de captura de contratos
            if(!UsuarioRegistrado( usuarioWindows ))
            {
                MessageBox.Show( "Este usuario no está autorizado para capturar contratos.", "Acceso denegado", MessageBoxButtons.OK, MessageBoxIcon.Warning );
                this.Close( ); // Cierra el formulario
                return;
            } else
            {

                //Asigna formato a el Formulario
                FormatearFormulario( );

                CargarContratosEnGrid( dgvContratos );

                VerificarEstatus( );

                VerificarPermisoSubirArchivo( );
            }

        }

        //********************************************************************************************************* //
        //****************************************** ACCIONES DE CONTROLES **************************************** //
        //********************************************************************************************************* //

        /// <summary>
        /// HABILITA EL VALOR DEL MONTO PARA PODER MODIFICARLO
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtMontoDelContrato_Enter( object sender, EventArgs e )
        {
            string texto = txtMontoDelContrato.Text.Replace( "$", "" ).Trim( );
            txtMontoDelContrato.Text = texto;
        }

        /// <summary>
        /// CONVIERTE EL MONTO DEL CONTRATO EN LETRAS UNA VEZ QUE SE CAPTURA EL MONTO EN EL CONTROL
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtMontoDelContrato_Leave( object sender, EventArgs e )
        {            
            if(EsNumero( txtMontoDelContrato.Text ))
            {
                decimal monto = decimal.Parse( txtMontoDelContrato.Text );
                txtMontoEnLetra.Text = ConvertirNumeroALetras( monto );
            } else
            {
                txtMontoEnLetra.Text = "";
            }

            FormateaMonto( );

            VerificarEstatus( ); //Obtiene el estatus actual del contrato y formate las imagenes del flujo.
        }


        /// <summary>
        /// LE PONE EL SIGNO DE PESOS AL MONTO CAPTURADO Y LE PONE FORMATO DE MONEDA
        /// </summary>
        private void FormateaMonto( )
        {
            if(decimal.TryParse( txtMontoDelContrato.Text, out decimal monto ))
            {
                // Se asigna el formato moneda mexicana con símbolo $
                txtMontoDelContrato.Text = string.Format( System.Globalization.CultureInfo.GetCultureInfo( "es-MX" ), "{0:C2}", monto );
            } else
            {
                MessageBox.Show( "El monto debera de ser presentado en numero. ", "Corrija el monto.", MessageBoxButtons.OK, MessageBoxIcon.Information );
                return;
            }
        }

        /// <summary>
        /// VALIDA QUE EL CONTRATO CAPTURADO SEA NUEVO O NO ESTE REPETIDO EN CASO DEL USUARIO NIVEL UNO, EN CASO DE OTRO NIVEL QUE SEA UN CONTRATO EXISTENTE
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtNumContrato_Leave( object sender, EventArgs e )
        {
            string usuario = Environment.UserName;
            string contrato = txtNumContrato.Text.Trim( );

            if(!ValidarContratoPorNumero( contrato, usuario ))
            {
                MessageBox.Show( "El proceso ha sido cancelado por política de flujo de captura. ", "Captura detenida", MessageBoxButtons.OK, MessageBoxIcon.Information );
                return;
            }
        }


        /// <summary>
        /// GUARDA EL CONTRATO CAPTURADO
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnGuardar_Click( object sender, EventArgs e )
        {
            GuardarContrato( );
            CargarContratosEnGrid( dgvContratos );
            InhabilitaControles( );
        }



        /// <summary>
        /// CARGA LOS DATOS DEL ARCHIVO LAYOUT EN LOS CONTROLES DEL FORMULARIO
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSubirArchivo_Click( object sender, EventArgs e )
        {
            if(ofdContratoLayout.ShowDialog( ) == DialogResult.OK)
            {
                string ruta = ofdContratoLayout.FileName;
                string[ ] lineas = File.ReadAllLines( ruta );

                foreach(string linea in lineas)
                {
                    string[ ] campos = linea.Split( '\t' );

                    if(campos.Length == 6)
                    {
                        string contrato = campos[ 0 ];
                        string fechaAplicacionStr = campos[ 1 ];
                        string fechaSolicitudStr = campos[ 2 ];
                        string nombreCliente = campos[ 3 ];
                        string montoLetra = campos[ 4 ];
                        string montoNumeroStr = campos[ 5 ];

                        if(!DateTime.TryParse( fechaAplicacionStr, out DateTime fechaAplicacion ) ||
                            !DateTime.TryParse( fechaSolicitudStr, out DateTime fechaSolicitud ))
                        {
                            MessageBox.Show( "Una o ambas fechas no tienen un formato válido." );
                            continue;
                        }

                        if(!decimal.TryParse( montoNumeroStr, out decimal montoNumero ))
                        {
                            MessageBox.Show( "El monto del contrato no tiene un formato válido." );
                            continue;
                        }

                        // Cargar en controles
                        txtNumContrato.Text = contrato;
                        txtFechaAplicacion.Value = fechaAplicacion;
                        txtFechaSolicitud.Value = fechaSolicitud;
                        txtNombreCliente.Text = nombreCliente;
                        txtMontoDelContrato.Text = montoNumero.ToString( "N2" );
                        txtMontoEnLetra.Text = montoLetra;

                        InhabilitaControles( );

                        MessageBox.Show( "Datos cargados exitosamente en el formulario." );
                        break; // Solo procesamos la primera línea del archivo
                    } else
                    {
                        MessageBox.Show( "El archivo no tiene el formato esperado. Debe tener 6 campos separados por tabulaciones." );
                    }
                }
            }
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

        //********************************************************************************************************* //
        //************************************* ASIGNA FORMATO A LOS CONTROLES ************************************ //
        //********************************************************************************************************* //



        /// <summary>
        /// DA FORMATO A TODOS LOS CONTROLES DEL FORMULARIO
        /// </summary>
        private void FormatearFormulario( )
        {
            FormateaLosTextBox( );
            FormatoBotonCerrar( );
            muestraUsuarioFirmado( );
            FormatearBotonGuardar( );
            AsignarImagenFlecha( pbFlechaUno, Properties.Resources.FlechaDerecha );
            AsignarImagenFlecha( pbFlechaDos, Properties.Resources.FlechaDerecha );
        }


        /// <summary>
        /// FORMATEA EL BOTÓN btnGuardar CON TEXTO A LA DERECHA, IMAGEN A LA IZQUIERDA Y ESQUINAS CURVAS
        /// </summary>
        private void FormatearBotonGuardar( )
        {
            btnGuardar.Size = new Size( 456, 68 );
            btnGuardar.FlatStyle = FlatStyle.Flat;
            btnGuardar.FlatAppearance.BorderSize = 0;
            btnGuardar.FlatAppearance.MouseOverBackColor = Color.FromArgb( 30, 136, 229 );
            btnGuardar.FlatAppearance.MouseDownBackColor = Color.FromArgb( 25, 118, 210 );
            btnGuardar.BackColor = Color.FromArgb( 0, 123, 255 );
            btnGuardar.ForeColor = Color.White;
            btnGuardar.Font = new Font( "Segoe UI", 11F, FontStyle.Bold );

            btnGuardar.Text = "Guardar y avanzar >>";
            btnGuardar.Image = Properties.Resources.Guardar; // Tu imagen en recursos
            btnGuardar.Image = ResizeImage( Properties.Resources.Guardar, new Size( 40, 40 ) ); // Tamaño más compacto
            btnGuardar.ImageAlign = ContentAlignment.MiddleLeft;
            btnGuardar.TextAlign = ContentAlignment.MiddleRight;
            btnGuardar.TextImageRelation = TextImageRelation.ImageBeforeText;
            btnGuardar.Padding = new Padding( 10, 0, 10, 0 );

            // Aplicar curva de esquinas
            btnGuardar.Region = Region.FromHrgn( CreateRoundRectRgn( 0, 0, btnGuardar.Width, btnGuardar.Height, 20, 20 ) );
        }

        /// <summary>
        /// REDIMENSIONA LA IMAGEN
        /// </summary>
        /// <param name="image"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        private Image ResizeImage( Image image, Size size )
        {
            Bitmap bmp = new Bitmap( size.Width, size.Height );
            using(Graphics g = Graphics.FromImage( bmp ))
            {
                g.DrawImage( image, new Rectangle( Point.Empty, size ) );
            }
            return bmp;
        }

        /// <summary>
        /// OBTIENE EL MONTO NUMÉRICO DESDE UN TEXTO FORMATEADO (ej: "$ 34,000.00").
        /// SI NO ES UN NÚMERO VÁLIDO, DEVUELVE 0.
        /// </summary>
        /// <param name="texto">Texto ingresado con o sin formato monetario</param>
        /// <returns>Decimal con el valor numérico limpio</returns>
        private decimal ObtenerMontoDesdeTexto( string texto )
        {
            if(string.IsNullOrWhiteSpace( texto ))
                return 0;

            string limpio = texto.Replace( "$", "" ).Replace( ",", "" ).Trim( );

            return decimal.TryParse( limpio, out decimal resultado ) ? resultado : 0;
        }

        /// <summary>
        /// ASIGNA UNA IMAGEN A UN PICTUREBOX Y LA AJUSTA A SU TAMAÑO
        /// </summary>
        /// <param name="pb">El PictureBox de destino</param>
        /// <param name="imagen">La imagen de recursos</param>
        private void AsignarImagenFlecha( PictureBox pb, Image imagen )
        {
            pb.Image = imagen;
            pb.SizeMode = PictureBoxSizeMode.StretchImage;
            pb.Width = 100;
            pb.Height = 62;
        }

        /// <summary>
        /// MANDA A LLAMAR EL METODO QUE LE DA FORMATO A LOS TEXTBOX LOS HACE POR CADA TEXTBOX EXISTENTE
        /// </summary>
        private void FormateaLosTextBox( )
        {
            FormatearTextBox( txtNumContrato );
            FormatearTextBox( txtNombreCliente );
            FormatearTextBox( txtMontoDelContrato );
            FormatearTextBox( txtMontoEnLetra );

        }

        /// <summary>
        /// DA FORMATO A LOS TEXTBOX
        /// </summary>
        private void FormatearTextBox( TextBox tb )
        {
            tb.BackColor = Color.White;
            tb.BorderStyle = BorderStyle.FixedSingle;
            tb.Font = new Font( "Segoe UI", 10, FontStyle.Regular );
            tb.ForeColor = Color.Black;
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
        //***************************************** METODOS DE FUNCIONALIDAD ************************************** //
        //********************************************************************************************************* //

        /// <summary>
        /// VERIFICA SI EL USUARIO ACTUAL TIENE ASIGNADA LA ETAPA 3 DEL FLUJO
        /// </summary>
        private void VerificarPermisoSubirArchivo( )
        {
            try
            {
                using(SqlConnection conn = new SqlConnection( connectionString ))
                using(SqlCommand cmd = new SqlCommand( @"
            SELECT cf.NivelFlujo
            FROM Usuarios u
            INNER JOIN ConfiguracionFlujo cf ON u.IdUsuario = cf.IdUsuario
            WHERE u.UsuarioWindows = @UsuarioWindows AND cf.Activo = 1
        ", conn ))
                {
                    cmd.Parameters.AddWithValue( "@UsuarioWindows", usuarioWindows ); // ya deberías tener este valor global

                    conn.Open( );
                    object resultado = cmd.ExecuteScalar( );

                    if(resultado != null && Convert.ToInt32( resultado ) == 3)
                    {
                        btnSubirArchivo.Enabled = true;
                        InhabilitaControles( );
                    } else
                    {
                        btnSubirArchivo.Enabled = false;
                    }
                }
            } catch(Exception ex)
            {
                MessageBox.Show( "Error al verificar permisos: " + ex.Message );
                btnSubirArchivo.Enabled = false;
            }
        }


        /// <summary>
        /// DEESHABILITA CONTROLES UNA VEZ QUE SE HA CAPTURADO EL CONTRATO O NO TIENE PERMISOS PARA ESCRIBIR
        /// </summary>
        private void InhabilitaControles( ) 
        {
            txtNumContrato.Enabled = false;
            txtFechaAplicacion.Enabled = false;
            txtFechaSolicitud.Enabled = false;
            txtNombreCliente.Enabled = false;
            txtMontoDelContrato.Enabled= false;
            txtMontoEnLetra.Enabled = false;
        }


        /// <summary>
        /// CARGA TODOS LOS CONTRATOS Y SUS ETAPAS AL DATAGRIDVIEW CON CHECKBOXES DESHABILITADOS
        /// </summary>
        /// <param name="grid">El DataGridView donde se mostrarán los contratos</param>
        private void CargarContratosEnGrid( DataGridView grid )
        {
            try
            {
                DataTable dt = DatabaseHelper.EjecutarDataTable( "sp_ObtenerContratosConEstatus", null );
                grid.DataSource = dt;

                // Asignar encabezados
                grid.Columns[ "EtapaUno" ].HeaderText = "Etapa 1";
                grid.Columns[ "EtapaDos" ].HeaderText = "Etapa 2";
                grid.Columns[ "EtapaTres" ].HeaderText = "Etapa 3";

                // Alinear al centro y hacerlas solo lectura
                foreach(string col in new[ ] { "EtapaUno", "EtapaDos", "EtapaTres" })
                {
                    grid.Columns[ col ].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    grid.Columns[ col ].ReadOnly = true;                       // Solo lectura
                    grid.Columns[ col ].CellTemplate = new DataGridViewCheckBoxCell( ); // Tipo checkbox
                }

                // Opcional: deshabilitar edición completa del grid si lo deseas
                grid.ReadOnly = false; // El grid puede ser editable, pero las columnas clave no

            } catch(Exception ex)
            {
                MessageBox.Show( "Error al cargar los contratos: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
            }
        }


        /// <summary>
        /// CONVIERTE UN NÚMERO DECIMAL A LETRAS CON LA LEYENDA 'PESOS'
        /// </summary>
        /// <param name="numero">Número decimal a convertir</param>
        /// <returns>Cadena con el número en letras y la palabra Pesos</returns>
        private string ConvertirNumeroALetras( decimal numero )
        {
            string resultado = "";

            long valor = (long) numero;
            if(valor == 0)
                return "Cero Pesos";

            resultado = NumeroEnLetras( valor ).Trim( );
            return $"{resultado} Pesos";
        }

        /// <summary>
        /// CONVIERTE EL UN NUMERO EN LETRAS
        /// </summary>
        /// <param name="numero"></param>
        /// <returns></returns>
        private string NumeroEnLetras( long numero )
        {
            if(numero == 0)
                return "";

            string[ ] unidades = { "", "Uno", "Dos", "Tres", "Cuatro", "Cinco", "Seis", "Siete", "Ocho", "Nueve" };
            string[ ] decenas = { "", "Diez", "Veinte", "Treinta", "Cuarenta", "Cincuenta", "Sesenta", "Setenta", "Ochenta", "Noventa" };
            string[ ] especiales = { "Once", "Doce", "Trece", "Catorce", "Quince" };

            if(numero < 10)
                return unidades[ numero ];

            if(numero < 20)
            {
                if(numero == 10)
                    return "Diez";
                return especiales[ numero - 11 ];
            }

            if(numero < 100)
            {
                int d = (int) (numero / 10);
                int u = (int) (numero % 10);
                return decenas[ d ] + (u > 0 ? " y " + NumeroEnLetras( u ) : "");
            }

            if(numero < 1000)
            {
                int c = (int) (numero / 100);
                int resto = (int) (numero % 100);

                if(c == 1 && resto == 0)
                    return "Cien";
                string centena = c == 1 ? "Ciento" : c == 5 ? "Quinientos" : c == 7 ? "Setecientos" : c == 9 ? "Novecientos" : unidades[ c ] + "cientos";
                return centena + (resto > 0 ? " " + NumeroEnLetras( resto ) : "");
            }

            if(numero < 1000000)
            {
                int miles = (int) (numero / 1000);
                int resto = (int) (numero % 1000);

                string milesTexto = miles == 1 ? "Mil" : NumeroEnLetras( miles ) + " Mil";
                return milesTexto + (resto > 0 ? " " + NumeroEnLetras( resto ) : "");
            }

            if(numero < 1000000000000)
            {
                long millones = numero / 1000000;
                long resto = numero % 1000000;

                string millonesTexto = millones == 1 ? "Un Millón" : NumeroEnLetras( millones ) + " Millones";
                return millonesTexto + (resto > 0 ? " " + NumeroEnLetras( resto ) : "");
            }

            return numero.ToString( ); // fallback por seguridad
        }


        /// <summary>
        /// VALIDA LA CAPTURA DEL CONTRATO BASADO SOLO EN EL NÚMERO DE CONTRATO Y EL USUARIO ACTUAL.
        /// - Si no existe el contrato, solo NivelFlujo 1 puede registrar.
        /// - Si existe, valida que el usuario tenga el siguiente nivel en el flujo para continuar.
        /// - Evita duplicados en la misma etapa.
        /// </summary>
        /// <param name="numeroContrato">Número de contrato capturado.</param>
        /// <param name="usuarioWindows">Usuario Windows que está firmando.</param>
        /// <returns>True si el usuario puede capturar, False si no.</returns>
        private bool ValidarContratoPorNumero( string numeroContrato, string usuarioWindows )
        {
            try
            {
                SqlParameter[ ] parametros = new SqlParameter[ ]
                {
            new SqlParameter("@NumeroContrato", numeroContrato),
            new SqlParameter("@UsuarioWindows", usuarioWindows)
                };

                DataTable dt = DatabaseHelper.EjecutarDataTable( "sp_ValidarContratoPorNumero", parametros );

                if(dt.Rows.Count == 0)
                {
                    MessageBox.Show( "Error al validar el contrato.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
                    return false;
                }

                DataRow row = dt.Rows[ 0 ];
                bool existeContrato = Convert.ToInt32( row[ "ExisteContrato" ] ) == 1;
                int ultimaEtapa = Convert.ToInt32( row[ "UltimaEtapa" ] );
                int nivelUsuario = Convert.ToInt32( row[ "NivelUsuario" ] );

                if(!existeContrato)
                {
                    // No existe contrato, solo nivel 1 puede registrar
                    if(nivelUsuario != 1)
                    {
                        MessageBox.Show( "Usted no tiene autorización para registrar nuevos contratos.", "Acceso denegado", MessageBoxButtons.OK, MessageBoxIcon.Warning );
                        btnGuardar.Enabled = false;
                        return false;
                    }
                    return true;
                } else
                {
                    // Existe contrato
                    if(nivelUsuario == 1)
                    {
                        MessageBox.Show( "El contrato ya existe. Usted no puede capturarlo de nuevo.", "Captura duplicada", MessageBoxButtons.OK, MessageBoxIcon.Warning );
                        btnGuardar.Enabled = false;
                        return false;
                    }
                    // Usuario debe ser el siguiente nivel
                    if(nivelUsuario == ultimaEtapa + 1)
                    {
                        btnGuardar.Enabled = true;
                        VerificarEstatus( );
                        return true;                        
                    } else
                    {
                        MessageBox.Show( "Usted no tiene autorización para capturar este contrato en esta etapa.", "Acceso denegado", MessageBoxButtons.OK, MessageBoxIcon.Warning );
                        btnGuardar.Enabled = false;
                        return false;
                    }
                }
            } catch(Exception ex)
            {
                MessageBox.Show( "Error al validar el contrato: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
                return false;
            }
        }



        /// <summary>
        /// VALIDA QUE EL USUARIO QUE ESTA INGRESANDO EN EL FORMULARIO EXISTA COMO UN USUARIO DE REGISTRO DE CONTRATOS
        /// </summary>
        /// <param name="usuario"></param>
        /// <returns></returns>
        private bool UsuarioRegistrado( string usuario )
        {
            SqlParameter[ ] parametros = new SqlParameter[ ]
            {
        new SqlParameter("@UsuarioWindows", usuario)
            };

            try
            {
                object resultado = DatabaseHelper.EjecutarEscalar( "sp_VerificarUsuarioExiste", parametros );
                int existe = Convert.ToInt32( resultado );
                return existe > 0;
            } catch(Exception ex)
            {
                MessageBox.Show( "Error al verificar el usuario: " + ex.Message );
                return false;
            }
        }


        /// <summary>
        /// MUESTRA EL USUARIO QUE ESTA INGRESANDO AL FORMULARIO
        /// </summary>
        private void muestraUsuarioFirmado( )
        {
            lblUsuario.Text = Environment.UserName;
        }

        /// <summary>
        /// VALIDA SI EL TEXTO CONTIENE UN NÚMERO DÉCIMAL VÁLIDO, LIMPIANDO SÍMBOLOS COMO '$' Y ','
        /// </summary>
        /// <param name="texto">Texto ingresado posiblemente con formato moneda</param>
        /// <returns>True si el texto es un número válido, False si no</returns>
        private bool EsNumero( string texto )
        {
            if(string.IsNullOrWhiteSpace( texto ))
                return false;

            // Elimina $ y , para que TryParse funcione
            string limpio = texto.Replace( "$", "" ).Replace( ",", "" ).Trim( );

            return decimal.TryParse( limpio, out _ );
        }

        /// <summary>
        /// GUARDA EL CONTRATO CAPTURADO Y REGISTRA FLUJO SI ES NIVEL 3
        /// </summary>
        private void GuardarContrato( )
        {
            if(!EsNumero( txtMontoDelContrato.Text ))
            {
                MessageBox.Show( "El monto debe ser numérico.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
                return;
            }

            using(SqlConnection conn = new SqlConnection( connectionString ))
            {
                conn.Open( );

                SqlTransaction transaccion = conn.BeginTransaction( );

                try
                {
                    // 1. Insertar contrato
                    using(SqlCommand cmd = new SqlCommand( "sp_InsertarContrato", conn, transaccion ))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue( "@Contrato", txtNumContrato.Text );
                        cmd.Parameters.AddWithValue( "@FechaAplicacion", DateTime.Parse( txtFechaAplicacion.Text ) );
                        cmd.Parameters.AddWithValue( "@FechaSolicitud", DateTime.Parse( txtFechaSolicitud.Text ) );
                        cmd.Parameters.AddWithValue( "@NombreCliente", txtNombreCliente.Text );
                        cmd.Parameters.AddWithValue( "@MontoLetra", txtMontoEnLetra.Text );
                        cmd.Parameters.AddWithValue( "@MontoNumero", decimal.Parse( txtMontoDelContrato.Text.Replace( "$", "" ).Replace( ",", "" ).Trim( ) ) );
                        cmd.Parameters.AddWithValue( "@UsuarioWindows", usuarioWindows );

                        cmd.ExecuteNonQuery( );
                    }

                    // 2. Verificar nivel del usuario
                    int nivelFlujo = 0;
                    int idUsuario = 0;
                    int idContrato = 0;

                    using(SqlCommand cmdNivel = new SqlCommand( @"
                SELECT u.IdUsuario, ISNULL(cf.NivelFlujo, 0)
                FROM Usuarios u
                LEFT JOIN ConfiguracionFlujo cf ON u.IdUsuario = cf.IdUsuario AND cf.Activo = 1
                WHERE u.UsuarioWindows = @UsuarioWindows
            ", conn, transaccion ))
                    {
                        cmdNivel.Parameters.AddWithValue( "@UsuarioWindows", usuarioWindows );

                        using(SqlDataReader reader = cmdNivel.ExecuteReader( ))
                        {
                            if(reader.Read( ))
                            {
                                idUsuario = reader.GetInt32( 0 );
                                nivelFlujo = reader.GetInt32( 1 );
                            }
                        }
                    }

                    // 3. Si es nivel 3, registrar en EstatusFlujoContrato
                    if(nivelFlujo == 3)
                    {
                        // Obtener el último IdContrato insertado
                        using(SqlCommand cmdContrato = new SqlCommand( "SELECT TOP 1 IdContrato FROM Contratos WHERE Contrato = @Contrato ORDER BY IdContrato DESC", conn, transaccion ))
                        {
                            cmdContrato.Parameters.AddWithValue( "@Contrato", txtNumContrato.Text );
                            object result = cmdContrato.ExecuteScalar( );
                            if(result != null)
                                idContrato = Convert.ToInt32( result );
                        }

                        if(idContrato > 0)
                        {
                            using(SqlCommand cmdInsertFlujo = new SqlCommand( @"
                        INSERT INTO EstatusFlujoContrato (IdContrato, Etapa, IdUsuario, FechaAccion, ArchivoLayout)
                        VALUES (@IdContrato, 3, @IdUsuario, GETDATE(), NULL)
                    ", conn, transaccion ))
                            {
                                cmdInsertFlujo.Parameters.AddWithValue( "@IdContrato", idContrato );
                                cmdInsertFlujo.Parameters.AddWithValue( "@IdUsuario", idUsuario );
                                cmdInsertFlujo.ExecuteNonQuery( );
                            }
                        }
                    }

                    transaccion.Commit( );

                    MessageBox.Show( "Contrato guardado correctamente." );
                    VerificarEstatus( );
                } catch(Exception ex)
                {
                    transaccion.Rollback( );
                    MessageBox.Show( "Error al guardar el contrato: " + ex.Message );
                }
            }
        }


        /// <summary>
        /// ASIGNA EL ESTATUS DE LAS IMAGENES SEGUN EL PUNTO DEL FLUJO DONDE SE ENCUENTRE EL PROCESO
        /// </summary>
        private void VerificarEstatus( )
        {
            // Primero poner las imágenes rojas por default
            imgUno.Image = Properties.Resources.UnoRojo;
            imgDos.Image = Properties.Resources.DosRojo;
            imgTres.Image = Properties.Resources.TresRojo;

            string contrato = txtNumContrato.Text;
            if(!EsNumero( txtMontoDelContrato.Text ))
                return;

            decimal monto = decimal.Parse( txtMontoDelContrato.Text.Replace( "$", "" ).Replace( ",", "" ).Trim( ) );

            using(SqlConnection conn = new SqlConnection( connectionString ))
            using(SqlCommand cmd = new SqlCommand( "sp_ObtenerEstatusFlujoContrato", conn ))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue( "@Contrato", contrato );
                cmd.Parameters.AddWithValue( "@MontoNumero", monto );

                try
                {
                    conn.Open( );
                    SqlDataReader reader = cmd.ExecuteReader( );
                    int maxEtapa = 0;

                    while(reader.Read( ))
                    {
                        int etapa = Convert.ToInt32( reader[ "Etapa" ] );
                        if(etapa > maxEtapa)
                            maxEtapa = etapa;
                    }

                    if(maxEtapa >= 1)
                        imgUno.Image = Properties.Resources.UnoVerde;
                    if(maxEtapa >= 2)
                        imgDos.Image = Properties.Resources.DosVerde;
                    if(maxEtapa >= 3)
                        imgTres.Image = Properties.Resources.TresVerde;
                } catch(Exception ex)
                {
                    MessageBox.Show( "Error al verificar el estatus del contrato: " + ex.Message );
                }
            }
        }

       
    }
}
