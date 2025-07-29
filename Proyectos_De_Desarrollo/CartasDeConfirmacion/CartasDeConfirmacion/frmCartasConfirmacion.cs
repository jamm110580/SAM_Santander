using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using org.apache.pdfbox.pdmodel;
using org.apache.pdfbox.pdmodel.encryption;
using org.apache.pdfbox.util;
using System;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;



namespace SAM_APP
{
    public partial class frmCartasConfirmacion : Form
    {

        public frmCartasConfirmacion()
        {
            InitializeComponent();
        }

        private void btnAbrirArchivo_Click(object sender, EventArgs e)
        {


            if (txtFile.Text == "" && txtDir.Text == "")
            {
                MessageBox.Show("Favor de seleccionar el archivo o ruta donde se encuentran los archivos a transformar");
                return;
            }

            if (cmbLayout.Text == "")
            {
                MessageBox.Show("Favor de seleccionar el formato de archivo correspondiente");
                return;
            }


            switch (cmbLayout.Text.ToString())
            {
                case "BBVA":

                    ExtraccionBBVA();
                    break;

                case "Nafin":

                    ExtraccionNafin();
                    break;

                case "Scotiabank":

                    ExtraccionScotiabank();
                    break;

                case "Bancomext":

                    ExtraccionBancomext();
                    break;

                case "Banobras":

                    ExtraccionBanobras();
                    break;

                case "Banorte":

                    ExtraccionBanorte();
                    break;

                case "HSBC":

                    ExtraccionHSBC();
                    break;

                case "Banamex":

                    ExtraccionBanamex();
                    break;

                case "Finamex Capitales":

                ExtraccionFinamex( );

                break;


                default:
                    MessageBox.Show("El formato seleccionado aun no se encuentra habilitado");
                    break;
            }
        }


        private void ExtraccionBanamex( )
        {
            string Contraparte = "Citibanamex";
            string Contrato = "", Fondo = "", TipoValor = "", Emisora = "", Serie = "";
            string Tasa = "", Titulos = "", Precio = "", Plazo = "", Monto = "", FechaOperacion = "";

            try
            {
                string strpsw = txtPasw.Text;
                string file = txtFile.Text;
                string contenido = "";

                var passwordBytes = Encoding.UTF8.GetBytes( strpsw );
                var readerProps = new ReaderProperties( ).SetPassword( passwordBytes );

                using(var reader = new PdfReader( file, readerProps ))
                using(var pdfDoc = new PdfDocument( reader ))
                {
                    var strategy = new SimpleTextExtractionStrategy( );

                    for(int i = 1; i <= pdfDoc.GetNumberOfPages( ); i++)
                    {
                        contenido += PdfTextExtractor.GetTextFromPage( pdfDoc.GetPage( i ), strategy );
                    }
                }

                string[ ] lines = contenido.Split( new[ ] { "\r\n", "\n" }, StringSplitOptions.None );
                DataTable dt = new DataTable( );

                dt.Columns.Add( "Contrato", typeof( string ) );
                dt.Columns.Add( "Contraparte", typeof( string ) );
                dt.Columns.Add( "Fondo_de_inversion", typeof( string ) );
                dt.Columns.Add( "Tipo_valor", typeof( string ) );
                dt.Columns.Add( "Emisora", typeof( string ) );
                dt.Columns.Add( "Serie", typeof( string ) );
                dt.Columns.Add( "Tasa", typeof( string ) );
                dt.Columns.Add( "Titulos", typeof( string ) );
                dt.Columns.Add( "Precio", typeof( string ) );
                dt.Columns.Add( "Plazo", typeof( string ) );
                dt.Columns.Add( "Monto_neto", typeof( string ) );
                dt.Columns.Add( "Fecha_operacion", typeof( string ) );

                for(int idx = 0; idx < lines.Length; idx++)
                {
                    string line = lines[ idx ];

                    if(line.Contains( "Fecha de Concertación:" ))
                    {
                        string fechaTexto = line.Replace( "Fecha de Concertación:", "" ).Trim( );
                        if(DateTime.TryParse( fechaTexto, out DateTime fecha ))
                        {
                            FechaOperacion = fecha.ToString( "dd/MM/yy" );
                        }
                    }

                    if(line.Contains( "DVP" ) && line.Contains( "COMPRA" ) && line.Contains( "/" ))
                    {
                        string[ ] partes = line.Split( new[ ] { ' ' }, StringSplitOptions.RemoveEmptyEntries );

                        // Buscar valor tipo M/BONOS/260305
                        string valorCompleto = partes.FirstOrDefault( p => p.Contains( "/" ) );
                        if(!string.IsNullOrEmpty( valorCompleto ))
                        {
                            string[ ] segmentos = valorCompleto.Split( '/' );
                            if(segmentos.Length == 3)
                            {
                                TipoValor = segmentos[ 0 ].Split( ' ' ).Last( ).Trim( );
                                Emisora = segmentos[ 1 ].Trim( );
                                Serie = segmentos[ 2 ].Trim( );
                            }
                        }

                        // Extracción por índice (según visualización previa)
                        Contrato = idxVal( partes, 0 );
                        Titulos = idxVal( partes, 14 );
                        Tasa = idxVal( partes, 15 );
                        Plazo = idxVal( partes, 16 );
                        Precio = idxVal( partes, 17 );
                        Monto = idxVal( partes, 18 ).Replace( ",", "" );

                        // Buscar el fondo hacia atrás
                        for(int i = idx - 1; i >= 0; i--)
                        {
                            if((lines[ i ].Contains( "FONDO" ) || lines[ i ].Contains( "Fondo" )) && lines[ i ].Contains( "74" ))
                            {
                                string[ ] fondoParts = lines[ i ].Split( ' ' );
                                if(fondoParts.Length > 1)
                                {   
                                    Fondo = string.Join( " ", fondoParts.Skip( 1 ) ).Trim( );
                                }
                                break;
                            }
                        }

                        dt.Rows.Add( Contrato, Contraparte, Fondo, TipoValor, Emisora, Serie, Tasa, Titulos, Precio, Plazo, Monto, FechaOperacion );
                    }
                }

                dgvPrevio.DataSource = dt;
            } catch(Exception ex)
            {
                MessageBox.Show( "Error al procesar PDF: " + ex.Message );
            }

            // Función auxiliar para obtener valor por índice
            string idxVal( string[ ] arr, int idx )
            {
                return (idx < arr.Length) ? arr[ idx ].Trim( ) : "";
            }
        }





        private static PDDocument Decrypt(PDDocument doc, string password)
        {
            //password = "Confir-ma16";
            StandardDecryptionMaterial standardDecryptionMaterial = new StandardDecryptionMaterial(password);

            try
            {
                if (doc.isEncrypted())
                {
                    doc.openProtection(standardDecryptionMaterial);
                }

                if(doc.getEncryptionDictionary( ) == null)
                {
                    MessageBox.Show( "El PDF fue desencriptado parcialmente, pero no contiene un diccionario de cifrado válido." );
                }

                doc.decrypt(password);
                return doc;

            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Concat("No es posible accesar el archivo, verifique la contraseña o la estructura, detalle: ", ex.Message.ToString()));
                return null;
            }
        }


        private void btnFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog fldArchivo = new OpenFileDialog();

            fldArchivo.InitialDirectory = "C:\\";
            fldArchivo.Filter = "Archivos PDF(*.pdf)|*.pdf";

            // codigo para abrir el cuadro de dialogo
            if (fldArchivo.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    string str_RutaArchivo = fldArchivo.FileName;
                    txtFile.Text = str_RutaArchivo;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }


        private void ExtraccionHSBC( )
        {
            string Contraparte = "";
            string Fondo = "";
            string TipoValor = "";
            string Emisora = "";
            string Serie = "";
            string Tasa = "";
            string Titulos = "";
            string Precio = "";
            string Plazo = "";
            string Monto = "";
            string FechaOperacion = "";

            try
            {
                string strpsw = txtPasw.Text;
                var dt = new DataTable( );

                dt.Columns.Add( "Contraparte", typeof( string ) );
                dt.Columns.Add( "Fondo", typeof( string ) );
                dt.Columns.Add( "Tipo_valor", typeof( string ) );
                dt.Columns.Add( "Emisora", typeof( string ) );
                dt.Columns.Add( "Serie", typeof( string ) );
                dt.Columns.Add( "Tasa", typeof( string ) );
                dt.Columns.Add( "Titulos", typeof( string ) );
                dt.Columns.Add( "Precio", typeof( string ) );
                dt.Columns.Add( "Plazo", typeof( string ) );
                dt.Columns.Add( "Monto_neto", typeof( string ) );
                dt.Columns.Add( "Fecha_operacion", typeof( string ) );

                if(!string.IsNullOrWhiteSpace( txtDir.Text ))
                {
                    string[ ] files = Directory.GetFiles( txtDir.Text );

                    foreach(string file in files)
                    {
                        string fileExt = Path.GetExtension( file );
                        if(!fileExt.Equals( ".pdf", StringComparison.OrdinalIgnoreCase ))
                            continue;

                        string[ ] Separatorsstring3 = new string[ ] { "_" };
                        string[ ] Separatorsstring2 = new string[ ] { " " };

                        string strFileContent = "";
                        byte[ ] passwordBytes = Encoding.UTF8.GetBytes( strpsw );
                        var readerProps = new ReaderProperties( ).SetPassword( passwordBytes );

                        using(var pdfReader = new PdfReader( file, readerProps ))
                        using(var pdfDoc = new PdfDocument( pdfReader ))
                        {
                            var strategy = new SimpleTextExtractionStrategy( );
                            for(int page = 1; page <= pdfDoc.GetNumberOfPages( ); page++)
                            {
                                strFileContent += PdfTextExtractor.GetTextFromPage( pdfDoc.GetPage( page ), strategy );
                            }
                        }

                        string[ ] lines = strFileContent.Split( new[ ] { '\n' }, StringSplitOptions.RemoveEmptyEntries );

                        for(int i = 0; i < lines.Length; i++)
                        {
                            string line = lines[ i ].Trim( );

                            if(line.StartsWith( "REPORTADO" ) && !line.Contains( "REPORTADOR" ))
                                Contraparte = line.Replace( "REPORTADO", "" ).Replace( ":", "" ).Trim( );

                            if(line.StartsWith( "TASA % SOBRETASA" ))
                                Tasa = line.Replace( "TASA % SOBRETASA", "" ).Replace( ":", "" ).Trim( );

                            if(line.StartsWith( "TITULOS" ))
                                Titulos = line.Replace( "TITULOS", "" ).Replace( ":", "" ).Replace( ",", "" ).Trim( );

                            if(line.StartsWith( "PRECIO" ))
                                Precio = line.Replace( "PRECIO", "" ).Replace( ":", "" ).Replace( ",", "" ).Trim( );

                            if(line.StartsWith( "PLAZO" ))
                                Plazo = line.Replace( "PLAZO", "" ).Replace( ":", "" ).Trim( );

                            if(line.StartsWith( "MONTO A LIQUIDAR" ))
                                Monto = line.Replace( "MONTO A LIQUIDAR", "" ).Replace( ":", "" ).Replace( ",", "" ).Replace( "MXN", "" ).Trim( );

                            if(line.StartsWith( "FECHA DE CONCERTACION" ))
                            {
                                string fechaRaw = line.Replace( "FECHA DE CONCERTACION", "" ).Replace( ":", "" ).Trim( );
                                if(DateTime.TryParseExact( fechaRaw, "ddMMMyyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime fechaParsed ))
                                    FechaOperacion = fechaParsed.ToString( "dd/MM/yy" );
                                else
                                    FechaOperacion = fechaRaw; // Por si no entra
                            }

                            if(line.StartsWith( "EMISION" ))
                            {
                                string[ ] deta = line.Replace( "EMISION", "" ).Replace( ":", "" ).Trim( ).Split( Separatorsstring2, StringSplitOptions.RemoveEmptyEntries );
                                if(deta.Length >= 2)
                                {
                                    Emisora = deta[ 0 ];
                                    string tvSerie = deta[ 1 ];
                                    if(tvSerie.Length >= 6)
                                    {
                                        TipoValor = tvSerie.Substring( 0, tvSerie.Length - 6 );
                                        Serie = tvSerie.Substring( tvSerie.Length - 6 );
                                    }
                                }
                            }
                        }

                        // Obtener el fondo desde el nombre del archivo
                        string[ ] strFondo = Path.GetFileName( file ).Split( Separatorsstring3, StringSplitOptions.None );
                        if(strFondo.Length > 1)
                            Fondo = strFondo[ 1 ];

                        dt.Rows.Add(
                            Contraparte.Trim( ),
                            Fondo.Trim( ),
                            TipoValor.Trim( ),
                            Emisora.Trim( ),
                            Serie.Trim( ),
                            Tasa.Trim( ),
                            Titulos.Trim( ),
                            Precio.Trim( ),
                            Plazo.Trim( ),
                            Monto.Trim( ),
                            FechaOperacion.Trim( )
                        );
                    }
                }

                dgvPrevio.DataSource = dt;
            } catch(Exception error)
            {
                MessageBox.Show( "El archivo PDF leído no contenía el formato esperado -- " + error.Message );
            }
        }



        private void ExtraccionNafin()
        {

            string Contraparte = "";
            string Contrato = "";
            string FechaOperacion = "";
            string Plazo = "";
            string Tasa = "";
            string Monto = "";
            string Titulos = "";
            string Tipovalor = "";
            string Emisora = "";
            string Serie = "";
            string Precio = "";

            //try
            //{

                //Creo tabla para almacenar resultados
                var dt = new DataTable();

                ////Columnas
                dt.Columns.Add("Contraparte", typeof(string));

                dt.Columns.Add("Numero_Contrato", typeof(string));

                dt.Columns.Add("Tipo_valor", typeof(string));
                dt.Columns.Add("Emisora", typeof(string));

                dt.Columns.Add("Serie", typeof(string));
                dt.Columns.Add("Tasa", typeof(string));

                dt.Columns.Add("Titulos", typeof(string));
                dt.Columns.Add("Precio", typeof(string));
                dt.Columns.Add("Plazo", typeof(string));

                dt.Columns.Add("Monto_neto", typeof(string));
                dt.Columns.Add("Fecha_operacion", typeof(string));



                int i = 0;

                if (!string.IsNullOrWhiteSpace(txtDir.Text.ToString()))
                {

                    string[] files = Directory.GetFiles(txtDir.Text.ToString());



                    foreach (string file in files)
                    {

                        string fileExt = System.IO.Path.GetExtension(file);

                        if (fileExt == ".pdf" || fileExt == ".PDF")
                        {
                            i = 0;
                            string strFileContent = "";
                            string[] Separatorsstring = new string[] { "\r\n" };
                            string[] Separatorsstring2 = new string[] { "  " };

                            PDDocument doc = PDDocument.load(file);
                            PDFTextStripper pdfStripper = new PDFTextStripper();

                            strFileContent = pdfStripper.getText(doc);

                            string[] lines = strFileContent.Split(Separatorsstring, StringSplitOptions.None);

                            foreach (string s in lines)
                            {
                                if (i == 7)
                                {
                                    Contraparte = s.ToString().Replace("REPORTADO                     :", "").Trim();
                                }

                                if (i == 3)
                                {
                                    Contrato = s.ToString().Substring(81, 10).Trim();
                                }

                                if (i == 8)
                                {
                                    DateTime Fecha = DateTime.Parse(s.ToString().Substring(41, 10));
                                    FechaOperacion = Fecha.ToString("dd/MM/yy");
                                }

                                if (i == 12)
                                {
                                    Plazo = s.Replace("PLAZO                         :  ", "").Trim();
                                }

                                if (i == 13)
                                {
                                    Tasa = s.Replace("TASA PREMIO                   :  ", "").Replace("%", "").Trim();
                                }

                                //if (i == 6)
                                //{
                                //    FondoInversion = s.Replace("REPORTADOR                    :        ", "").Trim();
                                //}

                                if (i > 20 && s.Substring(2, 7).ToString().Trim() == "")
                                {
                                    break;
                                }

                                if (i > 19 && s.Substring(2, 7).ToString().Trim() != "")
                                {
                                    string[] movs = s.Split(Separatorsstring2, StringSplitOptions.None);

                                    Monto = Convert.ToDecimal(s.Substring(79, 16).ToString().Trim()).ToString("##########.##");
                                    Tipovalor = s.Substring(12, 2).ToString().Trim();
                                    Titulos = s.Substring(53, 16).ToString().Trim().Replace(",", "");
                                    Emisora = s.Substring(2, 7).ToString().Trim();
                                    Serie = s.Substring(14, 6).ToString().Trim();
                                    Precio = Convert.ToDecimal(s.Substring(40, 17).ToString().Trim().Replace(",", "")).ToString("##########.########");

                                    dt.Rows.Add(Contraparte.ToString(), Contrato.ToString().Trim(),
                                          Tipovalor.ToString(), Emisora.ToString(), Serie.ToString(),
                                          Tasa.ToString(), Titulos.ToString(), Precio.ToString(), Plazo.ToString(),
                                          Monto.ToString().Replace(",", ""), FechaOperacion.ToString());
                                    
                                }
                                i++;
                            }
                        }
                    }
                }

                dgvPrevio.DataSource = dt;
            //}
            //catch (Exception error)
            //{
            //    MessageBox.Show(string.Concat("El archvivo PDF leído no contenia el formato esperado -- ", error.Message.ToString()));
            //}
        }

        private void ExtraccionScotiabank( )
        {
            string Contraparte = "";
            string Contrato = "";
            string FechaOperacion = "";
            string Plazo = "";
            string Tasa = "";
            string Monto = "";
            string Titulos = "";
            string Tipovalor = "";
            string Emisora = "";
            string Serie = "";
            string Precio = "";

            try
            {
                var dt = new DataTable( );
                dt.Columns.Add( "Contraparte", typeof( string ) );
                dt.Columns.Add( "Numero_Contrato", typeof( string ) );
                dt.Columns.Add( "Tipo_valor", typeof( string ) );
                dt.Columns.Add( "Emisora", typeof( string ) );
                dt.Columns.Add( "Serie", typeof( string ) );
                dt.Columns.Add( "Tasa", typeof( string ) );
                dt.Columns.Add( "Titulos", typeof( string ) );
                dt.Columns.Add( "Precio", typeof( string ) );
                dt.Columns.Add( "Plazo", typeof( string ) );
                dt.Columns.Add( "Monto_neto", typeof( string ) );
                dt.Columns.Add( "Fecha_operacion", typeof( string ) );

                string strpsw = txtPasw.Text.ToString( );
                string file = txtFile.Text.ToString( );
                string contenido = "";

                var passwordBytes = Encoding.UTF8.GetBytes( strpsw );
                var readerProps = new ReaderProperties( ).SetPassword( passwordBytes );

                using(var pdfReader = new PdfReader( file, readerProps ))
                using(var pdfDoc = new PdfDocument( pdfReader ))
                {
                    var strategy = new SimpleTextExtractionStrategy( );
                    for(int page = 1; page <= pdfDoc.GetNumberOfPages( ); page++)
                    {
                        contenido += PdfTextExtractor.GetTextFromPage( pdfDoc.GetPage( page ), strategy );
                    }
                }

                // Normalización
                contenido = contenido.Replace( "\r\n", " " ).Replace( "\n", " " ).Replace( "\r", " " );

                // Contraparte
                var matchContraparte = Regex.Match( contenido, @"ID Contraparte/ID Cliente:\s*([A-Z0-9\-]+)", RegexOptions.IgnoreCase );
                if(matchContraparte.Success)
                    Contraparte = matchContraparte.Groups[ 1 ].Value.Trim( );

                // Contrato
                var matchContrato = Regex.Match( contenido, @"Reportador/Comprador:\s*FONDO\s+(.*?)\s+(SA DE|S.A. DE)", RegexOptions.IgnoreCase );
                if(matchContrato.Success)
                    Contrato = matchContrato.Groups[ 1 ].Value.Trim( );

                // Fecha de concertación
                var matchFecha = Regex.Match( contenido, @"Fecha de Concertación:\s*(\w+\s+\d{1,2},?\s+\d{4})", RegexOptions.IgnoreCase );
                if(matchFecha.Success)
                {
                    DateTime fecha = DateTime.ParseExact( matchFecha.Groups[ 1 ].Value.Replace( ",", "" ), "MMMM d yyyy", new CultureInfo( "es-MX" ) );
                    FechaOperacion = fecha.ToString( "dd/MM/yy" );
                }

                // Tasa de rendimiento
                var matchTasa = Regex.Match( contenido, @"Tasa de Rendimiento Bruto:\s*([\d.]+)%", RegexOptions.IgnoreCase );
                if(matchTasa.Success)
                    Tasa = matchTasa.Groups[ 1 ].Value.Trim( );

                // Detalles de asignación
                MatchCollection matchDetalles = Regex.Matches( contenido,
                    @"(?<clave>\S+)\s+(?<isin>\S+)\s+(?<valor_nominal>\d+\.\d+)\s+(?<titulos>[\d,]+)\s+(?<importe>[\d,]+\.\d+)\s+(?<plazo>\d+)\s+(?<precio_unitario>\d+\.\d+)",
                    RegexOptions.Multiline );

                foreach(Match m in matchDetalles)
                {
                    string[ ] partes = m.Groups[ "clave" ].Value.Trim( ).Split( '_' );
                    Tipovalor = partes.Length > 0 ? partes[ 0 ] : "";
                    Emisora = partes.Length > 1 ? partes[ 1 ] : "";
                    Serie = partes.Length > 2 ? partes[ 2 ] : "";
                    Titulos = m.Groups[ "titulos" ].Value.Replace( ",", "" ).Trim( );
                    Monto = m.Groups[ "importe" ].Value.Replace( ",", "" ).Trim( );
                    Plazo = m.Groups[ "plazo" ].Value.Trim( );
                    Precio = m.Groups[ "precio_unitario" ].Value.Trim( );

                    dt.Rows.Add(
                        Contraparte,
                        Contrato,
                        Tipovalor,
                        Emisora,
                        Serie,
                        Tasa,
                        Titulos,
                        Precio,
                        Plazo,
                        Monto,
                        FechaOperacion
                    );
                }

                dgvPrevio.DataSource = dt;
            } catch(Exception ex)
            {
                MessageBox.Show( "El archivo PDF leído no contenía el formato esperado -- " + ex.Message );
            }
        }


        private void ExtraccionBBVA()
        {
            string Contraparte = "";
            string Contrato = "";
            string FechaOperacion = "";
            string Plazo = "";
            string Tasa = "";
            string Monto = "";
            string FondoInversion = "";
            string ISIN = "";
            string Precio = "";
            string Titulos = "";

            string[] field;

            int j = 0;
            //       try
            //{
            //Creo tabla para almacenar resultados
            var dt = new DataTable();


            ////Columnas
            dt.Columns.Add("Contraparte", typeof(string));
            dt.Columns.Add("Numero_Contrato", typeof(string));
            dt.Columns.Add("Fondo_de_inversion", typeof(string));
            dt.Columns.Add("ISIN", typeof(string));
            dt.Columns.Add("Tasa", typeof(string));
            dt.Columns.Add("Titulos", typeof(string));
            dt.Columns.Add("Precio", typeof(string));
            dt.Columns.Add("Monto_neto", typeof(string));
            dt.Columns.Add("Plazo", typeof(string));
            dt.Columns.Add("Fecha_operacion", typeof(string));


            string strFileContent = "";
            string[] Separatorsstring = new string[] { "\r\n" };

            string[] Separatorsstring2 = new string[] { " " };

            if (!string.IsNullOrWhiteSpace(txtDir.Text.ToString()))
            {
                string[] files = Directory.GetFiles(txtDir.Text.ToString());

                foreach (string file in files)
                {

                    string fileExt = System.IO.Path.GetExtension(file);

                    if (fileExt == ".pdf" || fileExt == ".PDF")
                    {


                        PDDocument doc = PDDocument.load(file.ToString());

                        PDFTextStripper pdfStripper = new PDFTextStripper();

                        strFileContent = pdfStripper.getText(doc);

                        {
                            string[] lines = strFileContent.Split(Separatorsstring, StringSplitOptions.None);

                            /*Bancomer*/
                            Contraparte = lines[3].ToString().Substring(0, 4);
                            FondoInversion = lines[4].ToString().Replace("PARA: ","").Replace(",","");

                            int l = 0;
                            foreach (string line in lines)
                            {
                                if (lines[l].ToString().Contains("919-"))
                                {

                                    Contrato = lines[l].ToString();
                                }

                                if (lines[l].ToString().Contains("Tasa (%): "))
                                {
                                    Tasa = lines[l].ToString().Replace("Tasa (%): ", "");
                                }

                                if (lines[l].ToString().Contains("Plazo de la operación "))
                                {
                                    Plazo = lines[l].ToString().Replace("Plazo de la operación ", "");
                                }

                                if (lines[l].ToString().Contains("Fecha de operación: "))
                                {
                                    DateTime Fecha = DateTime.Parse(lines[l].ToString().Replace("Fecha de operación: ", ""));
                                    FechaOperacion = Fecha.ToString("dd/MM/yy");
                                }

                                if (lines[l].ToString().Contains("Reportador"))
                                {
                                    for (int i = 0; i < 11; i++)
                                    {

                                        field = lines[l + 9].ToString().Split(Separatorsstring2, StringSplitOptions.None);
                                        int lenfield = field[3].Length - 12;

                                        if (field[2].ToString().Contains("Referencia"))
                                        {
                                            break;
                                        }

                                        if (field.Length == 5)
                                        {
                                            ISIN = field[3].ToString().Substring(lenfield, 12);
                                            Titulos = field[4].ToString().Replace(",", "");

                                           
                                            Monto = Convert.ToDecimal(field[2].ToString().Replace("MXN", "").Replace(",", "")).ToString("##########.##");
                                            Precio = Convert.ToDecimal(field[1].ToString().Replace(",", "")).ToString("##########.########");
                                        }


                                        dt.Rows.Add(Contraparte.ToString(), Contrato.ToString(), FondoInversion.ToString(),
                                        ISIN.ToString(), Tasa.ToString(), Titulos.ToString(),
                                        Precio.ToString(), Monto.ToString(), Plazo.ToString(),
                                        FechaOperacion.ToString());

                                        l++;


                                    }

                                    break;
                                }

                                l++;
                            }                            

                        }

                    }
                }

            }

            dgvPrevio.DataSource = dt;
            //    }
            //catch (Exception error)
            //{
            //    MessageBox.Show(string.Concat("El archvivo PDF leído no contenia el formato esperado -- ", error.Message.ToString()));
            //}

        }

        private void ExtraccionBancomext()
        {
            string Contraparte = "";
            string Contrato = "";
            string Nombre = "";
            string Serie = "";
            string Tasa = "";
            string Titulos = "";
            string Precio = "";
            string Plazo = "";
            string Monto = "";
            string FechaOperacion = "";

            //try
            //{
                //Creo tabla para almacenar resultados
                var dt = new DataTable();

                //Columnas
                dt.Columns.Add("Contraparte", typeof(string));

                dt.Columns.Add("Numero_Contrato", typeof(string));

                dt.Columns.Add("Nombre", typeof(string));

                dt.Columns.Add("Serie", typeof(string));
                dt.Columns.Add("Tasa", typeof(string));

                dt.Columns.Add("Titulos", typeof(string));
                dt.Columns.Add("Precio", typeof(string));
                dt.Columns.Add("Plazo", typeof(string));

                dt.Columns.Add("Monto_neto", typeof(string));
                dt.Columns.Add("Fecha_operacion", typeof(string));


                int i = 0;

                if (!string.IsNullOrWhiteSpace(txtDir.Text.ToString()))
                {

                    string[] files = Directory.GetFiles(txtDir.Text.ToString());
                    foreach (string file in files)
                    {

                        string fileExt = System.IO.Path.GetExtension(file);

                        if (fileExt == ".pdf" || fileExt == ".PDF")
                        {
                            i = 0;
                            string strFileContent = "";
                            string[] Separatorsstring = new string[] { "\r\n" };
                            string[] Separatorsstring2 = new string[] { " " };

                            PDDocument doc = PDDocument.load(file);
                            PDFTextStripper pdfStripper = new PDFTextStripper();

                            strFileContent = pdfStripper.getText(doc);

                            string[] lines = strFileContent.Split(Separatorsstring, StringSplitOptions.None);

                            

                            int l = 0;
                            foreach (string line in lines)
                            {
                                if (lines[l].ToString().Contains("Contrato"))
                                {
                                    Contrato = lines[l].ToString().Replace("Contrato", "").Trim();

                                Contraparte = "BANCO NACIONAL DE COMERCIO EXTERIOR";// lines[l-11].ToString().Replace(",", "");
                                    //Contrato = lines[14].ToString().Replace("Contrato", "").Trim();
                                    Nombre = lines[l+3].ToString().Replace("Nombre", "").Replace(",", "");

                                    if (lines[l + 20].ToString().Contains("Tasa"))
                                    {
                                        Tasa = lines[l + 22].ToString().Replace(",", "");
                                        Plazo = lines[l + 21].ToString();
                                    }
                                    else 
                                    {
                                        Tasa = lines[l + 21].ToString().Replace(",", "");
                                        Plazo = lines[l + 20].ToString();
                                    }

                                    Monto = lines[l-8].ToString().Replace(",", "");

                                    DateTime Fecha = DateTime.Parse(lines[l+8].ToString());
                                    FechaOperacion = Fecha.ToString("dd/MM/yy");

                                    break;

                                }
                                l++;

                            }

                            for (int w = 0; w < 100;w++)
                            {
                                string[] movs = lines[w].Split(Separatorsstring2, StringSplitOptions.None);
                                if (movs[0].ToString() == "RFC")
                                {
                                    break;
                                }

                                if (movs[0].ToString() == "ATENTAMENTE")
                                {
                                    break;
                                }

                                Serie = movs[0].ToString();
                                Titulos = movs[1].ToString().Replace(",", "");
                                Precio = movs[2].ToString().Replace(",", "");
                                Monto = movs[3].ToString().Replace(",", "");

                                dt.Rows.Add(Contraparte.ToString(), Contrato.ToString().Trim(),
                                        Nombre.ToString(), Serie.ToString(),
                                        Tasa.ToString(), Titulos.ToString(), Precio.ToString(), Plazo.ToString(),
                                        Monto.ToString().Replace(",", ""), FechaOperacion.ToString());

                            }

                    }
                        i++;
                    }
                    dgvPrevio.DataSource = dt;
                }
            //}
            //catch (Exception error)
            //{
            //    MessageBox.Show(string.Concat("El archvivo PDF leído no contenia el formato esperado -- ", error.Message.ToString()));
            //}

        }

        private void ExtraccionBanobras()
        {

            string Contraparte = "";
            string Contrato = "";
            string Nombre = "";
            string TipoValor = "";
            string Emisora = "";
            string Serie = "";
            string Cuenta_Destino = "";
            string Tasa = "";
            string Titulos = "";
            string Precio = "";
            string Plazo = "";
            string Monto = "";
            string FechaOperacion = "";

            //try
            //{
            //Creo tabla para almacenar resultados
            var dt = new DataTable();

            //Columnas
            dt.Columns.Add("Contraparte", typeof(string));
            dt.Columns.Add("Nombre", typeof(string));
            dt.Columns.Add("Tipo_Valor", typeof(string));
            dt.Columns.Add("Emisora", typeof(string));

            dt.Columns.Add("Serie", typeof(string));
            dt.Columns.Add("Cuenta_Destino", typeof(string));
            dt.Columns.Add("Tasa", typeof(string));

            dt.Columns.Add("Titulos", typeof(string));
            dt.Columns.Add("Precio", typeof(string));
            dt.Columns.Add("Plazo", typeof(string));

            dt.Columns.Add("Monto_neto", typeof(string));
            dt.Columns.Add("Fecha_operacion", typeof(string));


            int i = 0;

            if (!string.IsNullOrWhiteSpace(txtDir.Text.ToString()))
            {

                string[] files = Directory.GetFiles(txtDir.Text.ToString());
                foreach (string file in files)
                {

                    string fileExt = System.IO.Path.GetExtension(file);

                    if (fileExt == ".pdf" || fileExt == ".PDF")
                    {
                        i = 0;
                        string strFileContent = "";
                        string[] Separatorsstring = new string[] { "\r\n" };
                        string[] Separatorsstring2 = new string[] { " " };

                        PDDocument doc = PDDocument.load(file);
                        PDFTextStripper pdfStripper = new PDFTextStripper();

                        strFileContent = pdfStripper.getText(doc);

                        string[] lines = strFileContent.Split(Separatorsstring, StringSplitOptions.None);

                        int l = 0;
                        foreach (string line in lines)
                        {
                            if (lines[l].ToString().Contains("Contrato"))
                            {
                                Contrato = lines[l].ToString().Replace("Contrato", "").Replace("I", "").Substring(7, 3).Trim();
                                Nombre = lines[l + 3].ToString().Replace("Nombre", "").Replace(",", "");


                                // Monto = lines[l - 8].ToString().Replace(",", "");
                                try
                                {
                                    if (lines[7].ToString() == "RFC")
                                    {
                                        DateTime Fecha = DateTime.Parse(lines[l + 8].ToString());
                                        FechaOperacion = Fecha.ToString("dd/MM/yy");
                                        Cuenta_Destino = lines[29].ToString().Replace(",", "");
                                        Contraparte = lines[32].ToString().Replace(",", "");
                                    }
                                    else
                                    {
                                        DateTime Fecha = DateTime.Parse(lines[l + 8].ToString());
                                        FechaOperacion = Fecha.ToString("dd/MM/yy");
                                        Cuenta_Destino = lines[30].ToString().Replace(",", "");
                                        Contraparte = lines[33].ToString().Replace(",", "");
                                    }

                                }
                                catch
                                {
                                    DateTime Fecha = DateTime.Parse(lines[l + 7].ToString());
                                    FechaOperacion = Fecha.ToString("dd/MM/yy");
                                    Cuenta_Destino = lines[27].ToString().Replace(",", "");
                                    Contraparte = lines[30].ToString().Replace(",", "");

                                }
                                break;

                            }
                            l++;

                        }

                        l = 0;
                        foreach (string line in lines)
                        {
                            if (lines[l].ToString().Contains("Tasa Neta"))
                            {
                                Tasa = lines[l + 4].ToString().Replace(",", "");
                                Plazo = lines[l + 1].ToString();
                            }
                            l++;
                        }

                        for (int w = 0; w < 100; w++)
                        {
                            string[] movs = lines[w].Split(Separatorsstring2, StringSplitOptions.None);
                            if (movs[0].ToString() == "ATENTAMENTE")
                            {
                                break;
                            }

                            TipoValor = movs[4].ToString(); //movs[0].ToString().Substring(0, (movs[0].ToString().Length - 12));
                            Emisora = movs[5].ToString();// movs[0].ToString().Substring(movs[0].ToString().Length - 12, 7);
                            Serie = movs[0].ToString().Substring(movs[0].ToString().Length - 6, 6);

                            //Serie = movs[0].ToString();

                            Titulos = movs[1].ToString().Replace(",", "");
                            Precio = movs[2].ToString().Replace(",", "");
                            Monto = movs[3].ToString().Replace(",", "").Replace("DV", "").Replace("RP", "");

                            dt.Rows.Add(Contraparte.ToString(),
                                    Nombre.ToString(), TipoValor.ToString(), Emisora.ToString(), Serie.ToString(),
                                    Cuenta_Destino.ToString(), Tasa.ToString(),
                                    Titulos.ToString(), Precio.ToString(), Plazo.ToString(),
                                    Monto.ToString().Replace(",", ""), FechaOperacion.ToString());

                        }
                    }
                    i++;
                }
                dgvPrevio.DataSource = dt;
            }
    
                   //}
                //}
                //catch (Exception error)
                //{
                //    MessageBox.Show(string.Concat("El archvivo PDF leído no contenia el formato esperado -- ", error.Message.ToString()));
               // }
           }

        /// <summary>
        /// Extrae información de cartas PDF de Finamex Capitales
        /// </summary>
        /// <param name="rutaArchivo">Ruta del archivo PDF</param>
        private void ExtraccionFinamex( )
        {
            try
            {
                PDDocument doc = PDDocument.load( txtFile.Text );
                PDFTextStripper stripper = new PDFTextStripper( );

                // Validamos si permite extraer texto
                if(!doc.getCurrentAccessPermission( ).canExtractContent( ))
                {
                    MessageBox.Show( "Este PDF tiene protegida la extracción de contenido (no se puede copiar texto)." );
                    doc.close( );
                    return;
                }

                int numPages = doc.getNumberOfPages( );

                string tipoOperacion = "DESCONOCIDO";

                DataTable dt = new DataTable( );
                dt.Columns.Add( "TipoOperacion", typeof( string ) );
                dt.Columns.Add( "EMISORA", typeof( string ) );
                dt.Columns.Add( "CANTIDAD", typeof( string ) );
                dt.Columns.Add( "PRECIO", typeof( string ) );
                dt.Columns.Add( "IMPORTEBRUTO", typeof( string ) );
                dt.Columns.Add( "%COM.", typeof( string ) );
                dt.Columns.Add( "COMISION", typeof( string ) );
                dt.Columns.Add( "IVA", typeof( string ) );
                dt.Columns.Add( "IMP_NETO", typeof( string ) );

                for(int i = 1; i <= numPages; i++)
                {
                    stripper.setStartPage( i );
                    stripper.setEndPage( i );

                    string pageText = stripper.getText( doc );

                    // Determinar el tipo de operación
                    if(pageText.Contains( "VENTA" ))
                        tipoOperacion = "VENTA";
                    else if(pageText.Contains( "COMPRA" ))
                        tipoOperacion = "COMPRA";

                    string[ ] lineas = pageText.Split( '\n' );

                    foreach(string linea in lineas)
                    {
                        string lineaLimpia = linea.Trim( );

                        // Solo tomamos líneas que comienzan con un número (número de fila de operación)
                        if(Regex.IsMatch( lineaLimpia, @"^\d+\s+\w+" ))
                        {
                            string[ ] partes = Regex.Split( lineaLimpia, @"\s+" );

                            if(partes.Length >= 9)
                            {
                                string emisora = partes[ 1 ] + " " + partes[ 2 ];
                                string cantidad = partes[ 3 ];
                                string precio = partes[ 4 ];
                                string importeBruto = partes[ 5 ];
                                string porcCom = partes[ 6 ];
                                string comision = partes[ 7 ];
                                string iva = partes[ 8 ];
                                string imp_neto = partes[ 9 ];

                                dt.Rows.Add( tipoOperacion, emisora, cantidad, precio, importeBruto, porcCom, comision, iva, imp_neto );
                            }
                        }
                    }
                }

                dgvPrevio.DataSource = dt;
                doc.close( );
            } catch(Exception ex)
            {
                MessageBox.Show( "Error al procesar Finamex: " + ex.Message );
            }
        }



        private void ExtraccionBanorte()
        {
            string Contraparte = "";
            string Contrato = "";
            string Fondo = "";
            string TipoValor = "";
            string Emisora = "";
            string Serie = "";
            string Tasa = "";
            string Titulos = "";
            string Precio = "";
            string Plazo = "";
            string Monto = "";
            string FechaOperacion = "";

            try
            {
                //Creo tabla para almacenar resultados
                var dt = new DataTable();

                //Columnas
                dt.Columns.Add("Contraparte", typeof(string));
                dt.Columns.Add("Contrato", typeof(string));
                dt.Columns.Add("Fondo", typeof(string));
                dt.Columns.Add("TipoValor", typeof(string));
                dt.Columns.Add("Emisora", typeof(string));
                dt.Columns.Add("Serie", typeof(string));
                dt.Columns.Add("Tasa", typeof(string));
                dt.Columns.Add("Titulos", typeof(string));
                dt.Columns.Add("Precio", typeof(string));
                dt.Columns.Add("Plazo", typeof(string));
                dt.Columns.Add("Monto_neto", typeof(string));
                dt.Columns.Add("Fecha_operacion", typeof(string));


                int i = 0;

                if (!string.IsNullOrWhiteSpace(txtDir.Text.ToString()))
                {

                    string[] files = Directory.GetFiles(txtDir.Text.ToString());
                    foreach (string file in files)
                    {

                        string fileExt = System.IO.Path.GetExtension(file);

                        if (fileExt == ".pdf" || fileExt == ".PDF")
                        {
                            i = 0;
                            int lineaextra = 0;
                            string strFileContent = "";
                            string[] Separatorsstring = new string[] { "\r\n" };
                            string[] Separatorsstring2 = new string[] { " " };

                            PDDocument doc = PDDocument.load(file);
                            PDFTextStripper pdfStripper = new PDFTextStripper();

                            strFileContent = pdfStripper.getText(doc);

                            string[] lines = strFileContent.Split(Separatorsstring, StringSplitOptions.None);

                            Fondo = lines[8].ToString().Replace("Nombre del Cliente: ", "").Replace(",", "");
                            
                            if (lines[11].ToString().Contains("Sucursal"))
                            { 
                                lineaextra = 2;
                                Fondo = string.Concat(lines[8].ToString().Replace("Nombre del Cliente:", "").Replace(",", "").Trim()," ", lines[9].ToString(), " ", lines[10].ToString()) ;
                            }

                            if (lines[10].ToString().Contains("Sucursal"))
                            { 
                                lineaextra = 1;
                                Fondo = string.Concat(lines[8].ToString().Replace("Nombre del Cliente:", "").Replace(",", "").Trim(), " ", lines[9].ToString());
                            }


                            Contraparte = "BANORTE"; /*No viene como texto en el PDF por lo que se inserta*/
                            Contrato = lines[7].ToString().Replace("Numero de Contrato:", "").Trim();
                            
                            Plazo = lines[19 + lineaextra].ToString().Replace("Plazo:", "").Replace("DIAS", "").Replace("DIA", "").Replace("S", "").Trim();
                            
                            DateTime Fecha = DateTime.Parse(lines[16 + lineaextra].ToString().Replace("Fecha de Concertacion:", "").Trim());
                            FechaOperacion = Fecha.ToString("dd/MM/yy");

                            Tasa = lines[20 + lineaextra].ToString().Replace("Premio o Tasa Contratada: ", "").Replace("%", "");

                            foreach (string s in lines)
                            {
                                int longitud = s.ToString().Length;
                                if (longitud >= 6)
                                {
                                    if (s.ToString().Substring(0, 6) == "Emisor")
                                    {
                                    while (lines[i].ToString().Substring(0, 5) != "Monto")
                                    {
                                        i++;
                                        if (lines[i].ToString().Substring(0, 5) == "Monto")
                                        {
                                            break;
                                        }
                                        
                                        int l = -1;

                                        string[] movs = lines[i].ToString().Split(Separatorsstring2, StringSplitOptions.None);
                                        foreach (string line in movs)
                                        {
                                            l++;
                                        }

                                        if (l == 5)
                                        { 
                                            TipoValor = movs[0].ToString();
                                            Titulos = movs[5].ToString().Replace(",", "");
                                            Precio = movs[3].ToString();
                                            Monto = (Convert.ToDecimal(Titulos) * Convert.ToDecimal(Precio)).ToString("##########.##");
                                        }

                                        if (l == 4)
                                        {
                                            TipoValor = movs[0].ToString();
                                            Titulos = movs[4].ToString().Replace(",", "");
                                            Precio = movs[2].ToString();
                                            Monto = (Convert.ToDecimal(Titulos) * Convert.ToDecimal(Precio)).ToString("##########.##");
                                        }

                                        string[] movsdet = lines[i + 1].ToString().Split(Separatorsstring2, StringSplitOptions.None);

                                        int longcampo = 0;
                                        longcampo = movs[1].ToString().Length;

                                        if (longcampo > 7)
                                        {
                                            Emisora = movs[1].ToString().Substring(0, 7);
                                            Serie = movs[1].ToString().Substring(7, 6);
                                        }
                                        else
                                        {
                                            Emisora = movs[1].ToString();
                                            Serie = movs[2].ToString();
                                        }

                                            dt.Rows.Add(Contraparte.ToString(), Contrato.ToString().Replace(",","").Trim(), Fondo.ToString().Replace(",", "").Trim(),
                                                            TipoValor.ToString(), Emisora.ToString(), Serie.ToString(),
                                                            Tasa.ToString(), Titulos.ToString(), Precio.ToString(), Plazo.ToString(),
                                                            Monto.ToString().Replace(",", ""), FechaOperacion.ToString());

                                            //Valores a modificar en cada envío
                                            //string CLIENT_ID = string.Concat(TipoValor.ToString(), "_", Emisora.ToString(), "_", Serie.ToString());
                                            //string QUANTITY = Titulos.ToString();
                                            //string PRICE = Precio.ToString();

                                            //string TRADE_DATE = FechaOperacion.ToString();
                                            //string SETTLE_dATE = "";
                                            //string MATURITY = "";

                                            //dt.Rows.Add(ORD_NUM, TRANSACTION, EX_BROKER, PORTFOLIO, CLIENT_ID,
                                            //QUANTITY, PRICE, TRADE_DATE, SETTLE_dATE, MATURITY, COLLATERAL_TYPE,
                                            //SECURITY_TYPE, RATE, COUPON_TYPE, CURRENCY, ACCRUAL_BASIS, TRADER, CONFIRMED_BY,
                                            //CONFIRMED_WITH, FEE, INTEREST, AFORO, PACTADO, TIPO_COTIZACION, PLACEMENT_TYPE,
                                            //FI_PRIM_MKT);

                                            i++;

                                        }

                                    }
                                }
                                i++;
                            }
                        }

                        dgvPrevio.DataSource = dt;
                    }
                }
            }

            catch (Exception error)
            {
                MessageBox.Show(string.Concat("El archvivo PDF leído no contenia el formato esperado -- ", error.Message.ToString()));
            }
        }

        public void CreateCSVFile(ref DataTable dt, string strFilePath)
        {
            try
            {
                // Create the CSV file to which grid data will be exported.
                StreamWriter sw = new StreamWriter(strFilePath, false);
                // First we will write the headers.
                //DataTable dt = m_dsProducts.Tables[0];
                int iColCount = dt.Columns.Count;
                for (int i = 0; i < iColCount; i++)
                {
                    sw.Write(dt.Columns[i]);
                    if (i < iColCount - 1)
                    {
                        sw.Write(",");
                    }
                }
                sw.Write(sw.NewLine);

                // Now write all the rows.

                foreach (DataRow dr in dt.Rows)
                {
                    for (int i = 0; i < iColCount; i++)
                    {
                        if (!Convert.IsDBNull(dr[i]))
                        {
                            sw.Write(dr[i].ToString());
                        }
                        if (i < iColCount - 1)
                        {
                            sw.Write(",");
                        }
                    }

                    sw.Write(sw.NewLine);
                }
                sw.Close();
            }
            catch (Exception error)
            {
                MessageBox.Show(string.Concat("El resultado de la lectura del archivo no se pudo exportar -- ", error.Message.ToString()));
            }
        }

        private void btnExportar_Click(object sender, EventArgs e)
        {
            string localDate = DateTime.Now.ToString("_yyyyMMdd_HHmm");

            string ruta = "";

            if (txtFile.Text == "" && txtDir.Text == "")
            {
                MessageBox.Show("Favor de seleccionar el archivo a tranformar");
                return;
            }

            if (cmbLayout.Text == "")
            {
                MessageBox.Show("Favor de seleccionar el formato de archivo correspondiente");
                return;
            }

            if (dgvPrevio.Rows.Count == 0)
            {
                MessageBox.Show("Favor de abrir archivo seleccionado");
                return;
            }

            try
            {
                if (txtFile.Text != "")
                {
                    var fi1 = new FileInfo(@txtFile.Text.ToString());
                    ruta = string.Concat(fi1.DirectoryName, "\\", cmbLayout.Text.ToString(), localDate.ToString(), ".csv");
                }
                else
                {
                    //var fi1 = new FileInfo(@txtDir.Text.ToString());
                    ruta = string.Concat(@txtDir.Text.ToString(), "\\", cmbLayout.Text.ToString(), localDate.ToString(), ".csv");
                }

                DataTable dt = (DataTable)dgvPrevio.DataSource;

                CreateCSVFile(ref dt, @ruta);

                MessageBox.Show(string.Concat("Archivo Generado Correctamente ", ruta.ToString()));
            }
            catch (Exception error)
            {
                MessageBox.Show(string.Concat("Error al generar archivo CSV -- ", error.Message.ToString()));
            }
        }

        private void btnDir_Click(object sender, EventArgs e)
        {
            var fbd = new FolderBrowserDialog();

            DialogResult result = fbd.ShowDialog();

            if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
            {
                txtDir.Text = fbd.SelectedPath;
            }
        }

        private void cmbLayout_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbLayout.Text == "BBVA")
            {
                lbletiqueta.Visible = true;
                lbletiqueta.Text = "Seleccione la carpeta donde se encuentran los archivos:";
                btnDir.Visible = true;
                txtDir.Visible = true;
                txtFile.Visible = false;
                btnFile.Visible = false;

                txtPasw.Visible = false;
                lblPasw.Visible = false;
                txtPasw.Text = "";
            }

            if (cmbLayout.Text == "Nafin")
            {
                lbletiqueta.Visible = true;
                lbletiqueta.Text = "Seleccione la carpeta donde se encuentran los archivos:";
                btnDir.Visible = true;
                txtDir.Visible = true;
                txtFile.Visible = false;
                btnFile.Visible = false;

                txtPasw.Visible = false;
                lblPasw.Visible = false;
                txtPasw.Text = "";
            }

            if (cmbLayout.Text == "Scotiabank")
            {
                lbletiqueta.Visible = true;
                lbletiqueta.Text = "Seleccione el archvivo correspondiente:";
                btnDir.Visible = false;
                txtDir.Visible = false;
                txtFile.Visible = true;
                btnFile.Visible = true;

                txtPasw.Visible = true;
                lblPasw.Visible = true;
                txtPasw.Text = "SANTANDER";
            }

            if (cmbLayout.Text == "Bancomext")
            {
                lbletiqueta.Visible = true;
                lbletiqueta.Text = "Seleccione la carpeta donde se encuentran los archivos:";
                btnDir.Visible = true;
                txtDir.Visible = true;
                txtFile.Visible = false;
                btnFile.Visible = false;

                txtPasw.Visible = false;
                lblPasw.Visible = false;
                txtPasw.Text = "";
            }

            if (cmbLayout.Text == "Banobras")
            {
                lbletiqueta.Visible = true;
                lbletiqueta.Text = "Seleccione la carpeta donde se encuentran los archivos:";
                btnDir.Visible = true;
                txtDir.Visible = true;
                txtFile.Visible = false;
                btnFile.Visible = false;

                txtPasw.Visible = false;
                lblPasw.Visible = false;
                txtPasw.Text = "";
            }

            if (cmbLayout.Text == "Banorte")
            {
                lbletiqueta.Visible = true;
                lbletiqueta.Text = "Seleccione la carpeta donde se encuentran los archivos:";
                btnDir.Visible = true;
                txtDir.Visible = true;
                txtFile.Visible = false;
                btnFile.Visible = false;

                txtPasw.Visible = false;
                lblPasw.Visible = false;
                txtPasw.Text = "";
            }

            if (cmbLayout.Text == "HSBC")
            {
                lbletiqueta.Visible = true;
                lbletiqueta.Text = "Seleccione la carpeta donde se encuentran los archivos:";
                btnDir.Visible = true;
                txtDir.Visible = true;
                txtFile.Visible = false;
                btnFile.Visible = false;

                txtPasw.Visible = true;
                lblPasw.Visible = true;
                txtPasw.Text = "SAGE07eN71mo";
            }

            if (cmbLayout.Text == "Banamex")
            {
                lbletiqueta.Visible = true;
                lbletiqueta.Text = "Seleccione el archvivo correspondiente:";
                btnDir.Visible = false;
                txtDir.Visible = false;
                txtFile.Visible = true;
                btnFile.Visible = true;

                txtPasw.Visible = true;
                lblPasw.Visible = true;
                txtPasw.Text = "Confir-ma16";
            } 
            
            if(cmbLayout.Text == "Finamex Capitales")
            {
                lbletiqueta.Visible = true;
                lbletiqueta.Text = "Seleccione el archvivo correspondiente:";
                btnDir.Visible = false;
                txtDir.Visible = false;
                txtFile.Visible = true;
                btnFile.Visible = true;
            }

            txtFile.Text = "";
            txtDir.Text = "";
            dgvPrevio.DataSource = null;
        }

        private void frmPDFLector_Load(object sender, EventArgs e)
        {
            lbletiqueta.Visible = false;
            txtFile.Visible = false;
            txtDir.Visible = false;
            btnFile.Visible = false;
            btnDir.Visible = false;

            lblPasw.Visible = false;
            txtPasw.Visible = false;
        }

    }
}
