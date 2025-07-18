using org.apache.pdfbox.pdmodel;
using org.apache.pdfbox.pdmodel.encryption;
using org.apache.pdfbox.util;
using System;
using System.Data;
using System.IO;
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

                default:
                    MessageBox.Show("El formato seleccionado aun no se encuentra habilitado");
                    break;
            }
        }

        private void ExtraccionBanamex()
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

            //string Tipovalor = "";

            //try
            //{
                //Leo el password del PDF
                string strpsw = txtPasw.Text.ToString();

                //Creo tabla para almacenar resultados
                var dt = new DataTable();

                //dt.Columns.Add("ORD_NUM", typeof(string));
                //dt.Columns.Add("TRANSACTION", typeof(string));
                //dt.Columns.Add("EX_BROKER", typeof(string));
                //dt.Columns.Add("PORTFOLIO", typeof(string));


                //dt.Columns.Add("CLIENT_ID", typeof(string));
                //dt.Columns.Add("QUANTITY", typeof(string));
                //dt.Columns.Add("PRICE", typeof(string));

                //dt.Columns.Add("TRADE_DATE", typeof(string));
                //dt.Columns.Add("SETTLE_DATE", typeof(string));
                //dt.Columns.Add("MATURITY", typeof(string));


                //dt.Columns.Add("COLLATERAL_TYPE", typeof(string));
                //dt.Columns.Add("SECURITY_TYPE", typeof(string));
                //dt.Columns.Add("RATE", typeof(string));
                //dt.Columns.Add("COUPON_TYPE", typeof(string));
                //dt.Columns.Add("CURRENCY", typeof(string));
                //dt.Columns.Add("ACCRUAL_BASIS", typeof(string));
                //dt.Columns.Add("TRADER", typeof(string));
                //dt.Columns.Add("CONFIRMED_BY", typeof(string));
                //dt.Columns.Add("CONFIRMED_WITH", typeof(string));
                //dt.Columns.Add("FEE", typeof(string));
                //dt.Columns.Add("INTEREST", typeof(string));
                //dt.Columns.Add("AFORO", typeof(string));
                //dt.Columns.Add("PACTADO", typeof(string));
                //dt.Columns.Add("TIPO_COTIZACION", typeof(string));
                //dt.Columns.Add("PLACEMENT_TYPE", typeof(string));
                //dt.Columns.Add("FI_PRIM_MKT", typeof(string));


                ////Columnas
                dt.Columns.Add("Contrato", typeof(string));
                dt.Columns.Add("Contraparte", typeof(string));
                dt.Columns.Add("Fondo_de_inversion", typeof(string));
                dt.Columns.Add("Tipo_valor", typeof(string));
                dt.Columns.Add("Emisora", typeof(string));
                dt.Columns.Add("Serie", typeof(string));
                dt.Columns.Add("Tasa", typeof(string));
                dt.Columns.Add("Titulos", typeof(string));
                dt.Columns.Add("Precio", typeof(string));
                dt.Columns.Add("Plazo", typeof(string));
                dt.Columns.Add("Monto_neto", typeof(string));
                dt.Columns.Add("Fecha_operacion", typeof(string));

                //string file = System.IO.Path.GetFileName(txtFile.Text.ToString());

                string strFileContent = "";
                string[] Separatorsstring = new string[] { "\r\n" };
                string[] Separatorsstring2 = new string[] { " " };
                string[] Separatorsstring3 = new string[] { "/" };


            //PDDocument doc = Decrypt(PDDocument.load(txtFile.Text),"");

            //doc = Decrypt(doc, "");

            //PDDocument doc = PDDocument.load(txtFile.Text);
            //PDDocument doc = PDDocument.load(file);

            PDDocument doc = PDDocument.load(txtFile.Text);
            StandardDecryptionMaterial dm = new StandardDecryptionMaterial(strpsw.ToString());
            doc = Decrypt(doc,"");
            //doc.openProtection(dm);

          

                PDFTextStripper pdfStripper = new PDFTextStripper();
                pdfStripper.setStartPage(1);
                pdfStripper.setEndPage(2);

                strFileContent = pdfStripper.getText(doc);

                string[] lines = strFileContent.Split(Separatorsstring, StringSplitOptions.None);
                int i = 0;
                int n = 0;
                string FondoLargo = "";

                foreach (string line in lines)
                {
                    FondoLargo = "";
                    Contraparte = "Citibanamex";
                    //FechaOperacion = lines[10].ToString().Replace("Fecha de Concertación:", "").Trim();

                    DateTime Fecha = DateTime.Parse(lines[10].ToString().Replace("Fecha de Concertación:", "").Trim());
                    FechaOperacion = Fecha.ToString("dd/MM/yy");


                    if (lines[i].ToString().Contains("DVP") && lines[i].ToString().Contains("COMPRA"))
                    {
                        string[] deta = lines[i].ToString().Split(Separatorsstring2, StringSplitOptions.None);

                        string[] deta2 = deta[3].ToString().Split(Separatorsstring3, StringSplitOptions.None);

                        TipoValor = deta2[0].ToString();
                        Emisora = deta2[1].ToString().Replace(",","");
                        Serie = deta2[2].ToString();
                        Tasa = deta[5].ToString();
                        Titulos = deta[4].ToString().Replace(",", "").Trim();
                        Precio = deta[7].ToString();
                        Plazo = deta[6].ToString();
                        Monto = deta[8].ToString().Replace(",", "").Trim();
                        n = 0;

                        while (n < 1)
                        {
                            n--;

                            FondoLargo = string.Concat(lines[i + n].ToString(), " ", FondoLargo);

                            if ((FondoLargo.ToString().Contains("Fondo") || FondoLargo.ToString().Contains("FONDO")) && FondoLargo.ToString().Contains("74"))
                            {
                                string[] deta3 = FondoLargo.ToString().Split(Separatorsstring2, StringSplitOptions.None);

                                Contrato = deta3[0].ToString().Replace("Contrato:", "").Trim();
                                Fondo = FondoLargo.ToString().Replace(Contrato,"").Trim().Replace(",","");
                                break;
                            }

                        }

                        dt.Rows.Add(Contrato.ToString(), Contraparte.ToString(), Fondo.ToString().Replace(",",""), TipoValor.ToString(),
                                   Emisora.ToString(), Serie.ToString(), Tasa.ToString(), Titulos.ToString(),
                                    Precio.ToString(), Plazo.ToString(), Monto.ToString(), FechaOperacion);


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

                    }
                    i++;

                }

                dgvPrevio.DataSource = dt;

            //}

            //catch (Exception error)
            //{
            //    MessageBox.Show(string.Concat("El archvivo PDF leído no contenia el formato esperado -- ", error.Message.ToString()));
            //}
        }

        private static PDDocument Decrypt(PDDocument doc, string password)
        {
            password = "Confir-ma16";
            StandardDecryptionMaterial standardDecryptionMaterial = new StandardDecryptionMaterial(password);

            try
            {
                if (doc.isEncrypted())
                {
                    doc.openProtection(standardDecryptionMaterial);
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

        private void ExtraccionHSBC()
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
                //Leo el password del PDF
                string strpsw = txtPasw.Text.ToString();

                //Creo tabla para almacenar resultados
                var dt = new DataTable();

                //Columnas
                dt.Columns.Add("Contraparte", typeof(string));
                dt.Columns.Add("Fondo", typeof(string));
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
                        i = 0;
                        string fileExt = System.IO.Path.GetExtension(file);

                        if (fileExt == ".pdf" || fileExt == ".PDF")
                        {

                            string strFileContent = "";
                            string[] Separatorsstring = new string[] { "\r\n" };
                            string[] Separatorsstring2 = new string[] { " " };
                            string[] Separatorsstring3 = new string[] { "_" };

                            PDDocument doc = Decrypt(PDDocument.load(file), strpsw);
                            if (doc == null)
                            {
                                return;
                            }

                            //PDDocument doc = PDDocument.load(file);
                            PDFTextStripper pdfStripper = new PDFTextStripper();

                            strFileContent = pdfStripper.getText(doc);

                            string[] lines = strFileContent.Split(Separatorsstring, StringSplitOptions.None);

                            foreach (string s in lines)
                            {
                                if (lines[i].Contains("REPORTADO") && !lines[i].Contains("REPORTADOR"))
                                {
                                    Contraparte = lines[i].ToString().Replace("REPORTADO", "").Replace(":", "").Replace(",","").Trim();
                                }

                                if (lines[i].Contains("TASA % SOBRETASA"))
                                {
                                    Tasa = lines[i].ToString().Replace("TASA % SOBRETASA", "").Replace(":", "").Trim();
                                }

                                if (lines[i].Contains("TITULOS"))
                                {
                                    Titulos = lines[i].ToString().Replace("TITULOS", "").Replace(":", "").Replace(",", "").Trim();
                                }

                                if (lines[i].Contains("PRECIO"))
                                {
                                    Precio = lines[i].ToString().Replace("PRECIO", "").Replace(":", "").Replace(",", "").Trim();
                                }

                                if (lines[i].Contains("PLAZO"))
                                {
                                    Plazo = lines[i].ToString().Replace("PLAZO", "").Replace(":", "").Trim();
                                }

                                if (lines[i].Contains("MONTO A LIQUIDAR"))
                                {
                                    Monto = lines[i].ToString().Replace("MONTO A LIQUIDAR", "").Replace(":", "").Replace(",", "").Replace("MXN", "").Trim();
                                }

                                if (lines[i].Contains("FECHA DE CONCERTACION"))
                                {
                                    DateTime Fecha = DateTime.Parse(lines[i].ToString().Replace("FECHA DE CONCERTACION", "").Replace(":", ""));
                                    FechaOperacion = Fecha.ToString("dd/MM/yy");
                                }

                                if (lines[i].Contains("EMISION"))
                                {
                                    string[] deta = lines[i].ToString().Replace("EMISION", "").Replace(":", "").Trim().Split(Separatorsstring2, StringSplitOptions.None);

                                    /*Tipo valor, emisora y serie vienen en el mismo campo*/
                                    TipoValor = deta[1].ToString();
                                    int u = TipoValor.Length;
                                    TipoValor = TipoValor.Substring(0, u - 6);
                                    Emisora = deta[0].ToString();

                                    int l = deta[1].ToString().Length - 6;
                                    Serie = deta[1].ToString().Substring(l, 6).Trim();
                                }

                                string[] strFondo = Path.GetFileName(file.ToString()).Split(Separatorsstring3, StringSplitOptions.None);
                                Fondo = strFondo[1].ToString();

                                i++;
                            }

                            dt.Rows.Add(Contraparte.ToString(), Fondo.ToString().Trim(), TipoValor.ToString().Trim(),
                                            Emisora.ToString(), Serie.ToString(),
                                            Tasa.ToString(), Titulos.ToString(), Precio.ToString(), Plazo.ToString(),
                                            Monto.ToString().Replace(",", ""), FechaOperacion.ToString());

                        }

                    }
                }

                dgvPrevio.DataSource = dt;
            }

            catch (Exception error)
            {
                MessageBox.Show(string.Concat("El archvivo PDF leído no contenia el formato esperado -- ", error.Message.ToString()));
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

        private void ExtraccionScotiabank()
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
            int j = 0;

            //try
            //{
                //Creo tabla para almacenar resultados
                var dt = new DataTable();

                ////Columnas
                dt.Columns.Add("Contraparte", typeof(string));

                dt.Columns.Add("Numero_Contrato", typeof(string));
                //dt.Columns.Add("Fondo_de_inversion", typeof(string));

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

                string strFileContent = "";
                string[] Separatorsstring = new string[] { "\r\n" };
                string[] Separatorsstring2 = new string[] { " " };

                PDDocument doc = PDDocument.load(txtFile.Text);
                PDFTextStripper pdfStripper = new PDFTextStripper();

                strFileContent = pdfStripper.getText(doc);

                string[] lines = strFileContent.Split(Separatorsstring, StringSplitOptions.None);

                Contraparte = lines[13].ToString();

                DateTime Fecha = DateTime.Parse(lines[15].ToString().Replace("ABR","04").Replace("ENE","01").Replace("FEB", "02").Replace("MAR", "03").Replace("MAY", "05").Replace("JUN", "06"));
                FechaOperacion = Fecha.ToString("dd/MM/yy");

                int BanBancodeMexico = 0;
                int BanBancodeMexicoEmisora = 0;

            foreach (string s in lines)
                {
                    if (i >= 40)
                    {
                        if (s.ToString().Substring(0, 7) != "TOTALES")
                        {
                            if (BanBancodeMexico == 1 && BanBancodeMexicoEmisora == 0)
                            {
                                string[] movs = s.Split(Separatorsstring2, StringSplitOptions.None);
                                
                                Emisora = movs[0].ToString();
                                BanBancodeMexicoEmisora = 1;
                                continue;

                            }

                            if (BanBancodeMexico == 1 && BanBancodeMexicoEmisora == 1)
                            {
                                string[] movs = s.Split(Separatorsstring2, StringSplitOptions.None);

                                Plazo = movs[2].ToString();
                                Tasa = movs[4].ToString();
                                Monto = movs[1].ToString().Replace(",", "");
                                Precio = movs[7].ToString();
                                Titulos = movs[5].ToString().Replace(",", "");
                                Serie = movs[0].ToString();

                                dt.Rows.Add(Contraparte.ToString(), Contrato.ToString().Trim(),
                                Tipovalor.ToString(), Emisora.ToString(), Serie.ToString(),
                                Tasa.ToString(), Titulos.ToString(), Precio.ToString(), Plazo.ToString(),
                                Monto.ToString().Replace(",", ""), FechaOperacion.ToString());

                                BanBancodeMexico = 0; 
                                BanBancodeMexicoEmisora = 0;
                                continue;
                            }

                            if (s.ToString().Substring(10, 5) == "FONDO")
                            {
                                Contrato = s.ToString().Replace("FONDO", "").Trim();
                                j = i + 3;
                            }

                            //Caso especial Banco de Mexico
                            if (i == j && s.Contains("BANCO DEMEXICO"))
                            {
                                string[] movs = s.Split(Separatorsstring2, StringSplitOptions.None);
                                Tipovalor = movs[0].ToString();
                                BanBancodeMexico = 1;
                                continue;
                            }


                            if (i == j && BanBancodeMexico == 0)
                            {
                                string[] movs = s.Split(Separatorsstring2, StringSplitOptions.None);

                                Plazo = movs[5].ToString();
                                Tasa = movs[6].ToString();
                                Monto = movs[4].ToString().Replace(",", "");
                                Tipovalor = movs[0].ToString();
                                Emisora = movs[2].ToString();
                                Serie = movs[3].ToString();
                                Precio = movs[10].ToString();
                                Titulos = movs[8].ToString().Replace(",", "");

                                dt.Rows.Add(Contraparte.ToString(), Contrato.ToString().Trim(),
                                         Tipovalor.ToString(), Emisora.ToString(), Serie.ToString(),
                                         Tasa.ToString(), Titulos.ToString(), Precio.ToString(), Plazo.ToString(),
                                         Monto.ToString().Replace(",", ""), FechaOperacion.ToString());

                            }

                        }
                        else
                        {
                            break;
                        }
                    }

                    i++;
                }

                dgvPrevio.DataSource = dt;
            //}
            //catch (Exception error)
            //{
            //    MessageBox.Show(string.Concat("El archvivo PDF leído no contenia el formato esperado -- ", error.Message.ToString()));
            //}
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

                txtPasw.Visible = false;
                lblPasw.Visible = false;
                txtPasw.Text = "";
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
