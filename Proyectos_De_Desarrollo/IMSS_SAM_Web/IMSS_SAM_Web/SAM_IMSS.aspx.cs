using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Runtime.InteropServices;
using Excel = Microsoft.Office.Interop.Excel;
using OfficeOpenXml;
using System.Reflection;
using OfficeOpenXml.Style;
using System.Configuration;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel; // Para .xlsx
using NPOI.HSSF.UserModel; // Para .xls
using System.IO.Compression;
using MathNet.Numerics.Distributions;
using ICSharpCode.SharpZipLib.Zip;
using Microsoft.Ajax.Utilities;
using ExcelInterop = Microsoft.Office.Interop.Excel;
using NPOI.SS.Formula.Functions;


namespace IMSS_SAM_Web
{
    public partial class SAM_IMSS : System.Web.UI.Page
    {
        string pathlog;

        public static string GetFileName(string targetDirectory, string filename)
        {
            // Process the list of files found in the directory.
            string[] fileEntries = Directory.GetFiles(targetDirectory);
            foreach (string fileName in fileEntries)
            {
                if (fileName.Contains(filename.ToString()))
                {
                    return fileName;
                }
            }
            return "";
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                //******************************************************************************************************************************************************************************************
                //********************************************************************************* Carga de fechas *********************************************************************************
                ////****************************************************************************************************************************************************************************************
                Recursos.appfecha = DateTime.Now.ToString( "yyyyMMdd" );                
                Recursos.logfecha = DateTime.Now.ToString("yyyyMMdd_HHmmss");

                pathlog = string.Concat(HttpContext.Current.Server.MapPath("~\\App_Data"), "\\Log\\Log", Recursos.appfecha.ToString(), ".txt");
                using (StreamWriter swlog = File.AppendText(pathlog.ToString()))
                {
                    swlog.WriteLine(string.Concat(Recursos.logfecha.ToString(), "--", "Se abre ventana para generar layout del IMSS"));
                }

                //obtener el ultimo archivo
                string dir_archivo = Recursos.strRutaAladdin.ToString().Trim();
                var directory = new DirectoryInfo(dir_archivo);
                var ultimo_archivo_positions = (from f in directory.GetFiles()
                                                where f.Name.Contains("positions-downloadReport")
                                                orderby f.LastWriteTime descending
                                                select f).First();

                var ultimo_archivo_trades = (from f in directory.GetFiles()
                                             where f.Name.Contains("trades-downloadReport")
                                             orderby f.LastWriteTime descending
                                             select f).First();
                
                using (StreamWriter swlog = File.AppendText(pathlog.ToString()))
                {
                    swlog.WriteLine(string.Concat(Recursos.logfecha.ToString(), "--", "Se cargaron correctamente las rutas de los archivos de Aladdin"));
                }

            }
            catch
            {

            }


            try
            {
                string dir_archivo = Recursos.strRutaValmer.ToString().Trim();
                var directory = new DirectoryInfo(dir_archivo);

                var ultimo_archivo_valmer = (from f in directory.GetFiles()
                                             where f.Name.Contains(string.Concat("C:\\Users\\SAMMX\\Documents\\BAU\\IMSS\\IMSS Archivos de Carga\\VectorAnaliticoMD", ".xls"))
                                             orderby f.LastWriteTime descending
                                             select f).First();

                dir_archivo = Recursos.strRutaPip.ToString().Trim();
                directory = new DirectoryInfo(dir_archivo);


                var ultimo_archivo_pip = (from f in directory.GetFiles()
                                          where f.Name.Contains(string.Concat("C:\\Users\\SAMMX\\Documents\\BAU\\IMSS\\IMSS Archivos de Carga\\VectorAnalitico", Recursos.appfecha.ToString(), "MD.xls"))
                                          orderby f.LastWriteTime descending
                                          select f).First();

                using (StreamWriter swlog = File.AppendText(pathlog.ToString()))
                {
                    swlog.WriteLine(string.Concat(Recursos.logfecha.ToString(), "--", "Se cargaron rutas de los vectores correctamente"));
                }
            }
            catch
            {
                string fechaarchivos = string.Concat(Recursos.appfecha.ToString().Substring(0, 4), "_", Recursos.appfecha.ToString().Substring(4, 2), "_", Recursos.appfecha.ToString().Substring(6, 2));               
            }

        }

        private int ValidarVectorValmerCSV()
        {
            string connectionString = string.Concat("Server=", Recursos.strServer, "; Database=", Recursos.strBD, "; User Id=", Recursos.strUsr, "; Password=", Recursos.strPassword, "; Connection Timeout=60;");
            SqlConnection con = new SqlConnection(connectionString);

            //Valido que el vector de valmer se el del día
            string strqry = @"select count(*) from VectorAnaliticoValmerCSV where datediff(dd, convert(datetime,Fecha, 103), getdate()) > 1";
            con.Open();
            SqlCommand cmd = new SqlCommand(strqry, con);
            cmd.CommandType = CommandType.Text;
            int count = Convert.ToInt32(cmd.ExecuteScalar());

            if (count == 0)
                return 0;
            else
                return 1;
        }

        private int ValidarVectorValmer()
        {
            string connectionString = (ConfigurationManager.ConnectionStrings["SAM_IMSS_Connection"].ConnectionString);
            SqlConnection con = new SqlConnection(connectionString);

            //Valido que el vector de valmer se el del día
            string strqry = @"select count(*) from VectorAnaliticoValmer where datediff(dd, convert(datetime,fecha, 103), convert(datetime,getdate(),103)) > 1";
            con.Open();
            SqlCommand cmd = new SqlCommand(strqry, con);
            cmd.CommandType = CommandType.Text;
            int count = Convert.ToInt32(cmd.ExecuteScalar());

            if (count == 0)
                return 0;
            else
                return 1;
        }

        //**********************************************************************************************************************************************************************************
        //**********************************************************************************************************************************************************************************
        //**********************************************************************************************************************************************************************************
        //************************************************************************** BOTON DE PROCESAR *************************************************************************************
        //**********************************************************************************************************************************************************************************
        //**********************************************************************************************************************************************************************************
        //**********************************************************************************************************************************************************************************
        protected void btnProcesar_Click(object sender, EventArgs e)
        {            
            Recursos.appfecha = DateTime.Now.ToString( "yyyyMMdd" );
            Recursos.logfecha = DateTime.Now.ToString( "yyyyMMdd_HHmmss" );
            string script = string.Empty;

            using (StreamWriter swlog = File.AppendText(pathlog.ToString()))
            {
                swlog.WriteLine(string.Concat(Recursos.logfecha.ToString(), "--", "Inicia proceso para cargar layouts y generar archivos del IMSS"));
            }

            //**********************************************************************************************************************************************************************************
            //**********************************************************************************************************************************************************************************
            //******************************************************************** INICIA CARGA DE ARCHIVOS ************************************************************************************
            //**********************************************************************************************************************************************************************************
            //**********************************************************************************************************************************************************************************


            //*****************************************************************************************
            //ESTE METODO PERTENECE AL BOTON "VECTOR MD VALMER" ***************************************
            //*****************************************************************************************
            if (fuArchivoExcelMDValmer.HasFile && chMDValmer.Checked == true)
            {
                // Obtener el nombre del archivo
                string fileName = fuArchivoExcelMDValmer.FileName;
                // Ruta donde se guardará el archivo en el servidor
                string rutaDestino = Server.MapPath("~/App_Data/FileTemp/" + fileName);

                LimpiarTablaVector();
                if (chkXLS.Checked)
                {                    
                    SaveAsCsvVectorMD(rutaDestino.ToString().Replace(".xlsx", ".csv"));
                                       
                    BulkVectorValmer(rutaDestino.ToString().Replace(".xlsx", ".csv"));

                    int Valida = ValidarVectorValmer();

                    if (Valida == 1)
                    {
                        script = "alert('SAM - IMSS: El vector de valmer no es del día, continúa proceso');";
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", script, true);
                        using (StreamWriter swlog = File.AppendText(pathlog.ToString()))
                        {
                            swlog.WriteLine(string.Concat(Recursos.logfecha.ToString(), "--", "El vector de valmer no es del día, continua proceso"));
                        }
                    }
                }

                if (chkCSV.Checked)
                {
                    // Obtener la ruta donde se guardará el archivo en el servidor
                    string serverPath = Server.MapPath("~/App_Data/FileTemp/" + fuArchivoExcelMDValmer.FileName);

                    // Guardar el archivo en el servidor
                    fuArchivoExcelMDValmer.SaveAs(serverPath);

                    BulkVectorValmerCSV(rutaDestino.ToString());
                }
                using (StreamWriter swlog = File.AppendText(pathlog.ToString()))
                {
                    swlog.WriteLine(string.Concat(Recursos.logfecha.ToString(), "--", "El vector de valmer se cargo de manera correcta -- ", rutaDestino.ToString()));
                }
                script = "alert('SAM - IMSS: Carga de vector de Valmer exitosa.');";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", script, true);

            }

            //*****************************************************************************************
            //ESTE METODO PERTENECE AL BOTON "VECTOR ANALITICO PIP" ***********************************
            //*****************************************************************************************        
            if (FileUploadVecPIP.HasFiles && chVectorPip.Checked == true)
            {
                // Obtener el nombre del archivo
                string fileNameVecPiP = FileUploadVecPIP.FileName;
                // Ruta donde se guardará el archivo en el servidor
                string rutaDestinoVecPiP = Server.MapPath("~/App_Data/FileTemp/" + fileNameVecPiP);

                LimpiarVectorPiP();
                SaveAsCsvVectorPiP(rutaDestinoVecPiP.ToString().Replace(".xls", ".csv"));
                BulkVectorPiP(rutaDestinoVecPiP.ToString().Replace(".xls", ".csv"));

                script = "alert('SAM - IMSS: Carga de vector de PiP exitosa.');";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", script, true);

                using (StreamWriter swlog = File.AppendText(pathlog.ToString()))
                {
                    swlog.WriteLine(string.Concat(Recursos.logfecha.ToString(), "--", "El vector de PIP se cargo de manera correcta -- ", rutaDestinoVecPiP.ToString()));
                }
            }

            //*****************************************************************************************
            //ESTE METODO PERTENECE AL BOTON "ARCHIVO POSITIONS - LAYOUTS (ARCHIVOS ALADDIN)" *********
            //*****************************************************************************************
            if (FileUploadPositionAladdin.HasFiles && chArcPositions.Checked == true)
            {
                // Obtener el nombre del archivo
                string fileNamePosition = FileUploadPositionAladdin.FileName;
                // Ruta donde se guardará el archivo en el servidor
                string rutaDestinoPosition = Server.MapPath("~/App_Data/FileTemp/" + fileNamePosition);

                using (StreamWriter swlog = File.AppendText(pathlog.ToString()))
                {
                    swlog.WriteLine(string.Concat(Recursos.logfecha.ToString(), "--", "Inicia carga archivo Position -- ", rutaDestinoPosition.ToString()));
                }

                LimpiarTablasAladdinPosition();
                LeerArchivoPositions(rutaDestinoPosition.ToString());
            }

            //*****************************************************************************************
            //ESTE METODO PERTENECE AL BOTON "ARCHIVO TRADES - CUSTODIOS (ARCHIVOS ALADDIN)" *********
            //*****************************************************************************************
            if (FileUploadTrades.HasFiles && chArchTrades.Checked == true)
            {
                // Obtener el nombre del archivo
                string fileNameTrades = FileUploadTrades.FileName;
                // Ruta donde se guardará el archivo en el servidor
                string rutaDestinofileNameTrades = Server.MapPath("~/App_Data/FileTemp/" + fileNameTrades);


                using (StreamWriter swlog = File.AppendText(pathlog.ToString()))
                {
                    swlog.WriteLine(string.Concat(Recursos.logfecha.ToString(), "--", "Inicia carga archivo Trades Custodio -- ", rutaDestinofileNameTrades.ToString()));
                }

                //Valido que el archivo traiga todas los trades que liquidan en el dia en comparacion con lo que tenia programado un dia antes
                ValidarLiquidacionPositionsTrades(rutaDestinofileNameTrades.ToString());

                LimpiarTablasAladdinTrades();
                LeerArchivoTrades(rutaDestinofileNameTrades.ToString());
            }

            string tiempo = DateTime.Now.ToString("HHmmss");


            //*****************************************************************************************
            //VALIDA QUE TODOS LOS INSTRUMENTOS QUE SE ENCUENTRAN EN LAS OPERACIONES ESTEN EN LOS ARCHIVOS VectorAnaliticoValmer y VectorAnaliticoPiP *********
            //*****************************************************************************************

            if ((chkPosicion.Checked == true) || (chkTrades.Checked == true) || (chkValuada.Checked == true))
            {
                int ban = ValidoInstrumentosenVector();
                if (ban == 1)
                {
                    script = "alert('SAM - IMSS: Hay intrumentos en la posición que no se encuentran en el vector de Pip, continua generación de archivos.');";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", script, true);
                }

                if (ban == 2)
                {
                    script = "alert('Hay intrumentos en la posición que no se encuentran en el vector de Valmer, continua generación de archivos.');";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", script, true);
                }
            }

            //**********************************************************************************************************************************************************************************
            //**********************************************************************************************************************************************************************************
            //******************************************************************** INICIA GENERACIÓN DE ARCHIVOS ********************************************************************************
            //**********************************************************************************************************************************************************************************
            //**********************************************************************************************************************************************************************************

            //*****************************************************************************************
            // ****** ELIMINA TODOS LOS ARCHIVOS DE LA CARPETA GENERATE PARA GENERAR LOS NUEVOS  ******
            //*****************************************************************************************

            EliminarArchivosGenerados();

            //*****************************************************************************************
            //ESTE METODO PERTENECE AL BOTON "POSICION" - (GENERACIÓN DE ARCHIVO) *********************
            //*****************************************************************************************
            if (chkPosicion.Checked == true)
            {
                if (chkXLS.Checked)
                {
                    ExpArcPosLayout(string.Concat(HttpContext.Current.Server.MapPath("~/App_Data/Generate/"), "MSANT", Recursos.appfecha, "layout.csv").Trim());
                }

                if (chkCSV.Checked)
                {
                    ExpArcPosLayoutCSV(string.Concat(HttpContext.Current.Server.MapPath("~/App_Data/Generate/"), "MSANT", Recursos.appfecha, "layout.csv").Trim());
                }

                using (StreamWriter swlog = File.AppendText(pathlog.ToString()))
                {
                    swlog.WriteLine(string.Concat(Recursos.logfecha.ToString(), "--", "Layout generado correctamente -- ", string.Concat("MSANT", Recursos.appfecha, "layout.csv").Trim()));
                }
                script = "alert('SAM - IMSS: Layout exitoso.');";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", script, true);
            }

            //*****************************************************************************************
            //ESTE METODO PERTENECE AL BOTON "TRADES" - (GENERACIÓN DE ARCHIVO) ***********************
            //*****************************************************************************************
            if (chkTrades.Checked == true)
            {
                if (chkXLS.Checked)
                {
                    ExpArcTrades(string.Concat(HttpContext.Current.Server.MapPath("~/App_Data/Generate/"), "MSANT", Recursos.appfecha, "_FV.csv").Trim());
                }

                if (chkCSV.Checked)
                {
                    ExpArcTradesCSV(string.Concat(HttpContext.Current.Server.MapPath("~/App_Data/Generate/"), "MSANT", Recursos.appfecha, "_FV.csv").Trim());
                }

                using (StreamWriter swlog = File.AppendText(pathlog.ToString()))
                {
                    swlog.WriteLine(string.Concat(Recursos.logfecha.ToString(), "--", "Layout FV generado correctamente -- ", string.Concat(string.Concat("MSANT", Recursos.appfecha, "_FV.csv").Trim())));
                }
                script = "alert('SAM - IMSS: Trades exitoso.');";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", script, true);
            }

            //*****************************************************************************************
            //ESTE METODO PERTENECE AL BOTON "EXCEL POSICIÓN VALUADA" - (GENERACIÓN DE ARCHIVO) *******
            //*****************************************************************************************
            if (chkValuada.Checked == true)
            {
                if (chkXLS.Checked)
                {
                    ExpArcPosValuada(string.Concat(HttpContext.Current.Server.MapPath("~/App_Data/Generate/"), Recursos.appfecha, "excel_SAM.xlsx").Trim());
                }
                if (chkCSV.Checked)
                {
                    ExpArcPosValuadaCSV(string.Concat(HttpContext.Current.Server.MapPath("~/App_Data/Generate/"), Recursos.appfecha, "excel_SAM.xlsx").Trim());
                }


                using (StreamWriter swlog = File.AppendText(pathlog.ToString()))
                {
                    swlog.WriteLine(string.Concat(Recursos.logfecha.ToString(), "--", "Layout posición valuada excel generado correctamente -- ", string.Concat(Recursos.appfecha, "excel_SAM.xlsx").Trim()));
                }
                script = "alert('SAM - IMSS:  Valuada exitoso.');";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", script, true);
            }

            //*****************************************************************************************
            //ESTE METODO PERTENECE AL BOTON "CUSODIO BBVA" - (GENERACIÓN DE ARCHIVO) *****************
            //*****************************************************************************************
            if (chkBBVA.Checked == true)
            {
                ExpArcBBVA(string.Concat(HttpContext.Current.Server.MapPath("~/App_Data/Generate/"), "BBVA", Recursos.appfecha, "_", tiempo.ToString(), ".txt").Trim());

                using (StreamWriter swlog = File.AppendText(pathlog.ToString()))
                {
                    swlog.WriteLine(string.Concat(Recursos.logfecha.ToString(), "--", "Layout BBVA generado correctamente -- ", string.Concat("BBVA", Recursos.appfecha, "_", tiempo.ToString(), ".txt").Trim()));
                }
                script = "alert('SAM - IMSS: Custodio BBVA exitoso.');";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", script, true);
            }

            //*****************************************************************************************
            //ESTE METODO PERTENECE AL BOTON "CUSODIO S3" - (GENERACIÓN DE ARCHIVO) *******************
            //*****************************************************************************************
            if (chkS3.Checked == true)
            {
                ExpArcS3(string.Concat(HttpContext.Current.Server.MapPath("~/App_Data/Generate/"), "S3", Recursos.appfecha, "_", tiempo.ToString(), ".txt").Trim());

                using (StreamWriter swlog = File.AppendText(pathlog.ToString()))
                {
                    swlog.WriteLine(string.Concat(Recursos.logfecha.ToString(), "--", "Layout S3 generado correctamente -- ", string.Concat("S3", Recursos.appfecha, "_", tiempo.ToString(), ".txt").Trim()));
                }
                script = "alert('SAM - IMSS: Custodio S3 exitoso.');";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", script, true);
            }

            //*****************************************************************************************
            //ESTE METODO PERTENECE AL BOTON "CUSODIO S3 Complementario" - (GENERACIÓN DE ARCHIVO) *******************
            //*****************************************************************************************
            if (chkS3Comp.Checked == true)
            {
                ExpArcS3Comp(string.Concat(HttpContext.Current.Server.MapPath("~/App_Data/Generate/"), "S3_Comp", Recursos.appfecha, "_", tiempo.ToString(), ".xlsx").Trim());

                using (StreamWriter swlog = File.AppendText(pathlog.ToString()))
                {
                    swlog.WriteLine(string.Concat(Recursos.logfecha.ToString(), "--", "Layout S3 generado correctamente -- ", string.Concat("S3", Recursos.appfecha, "_", tiempo.ToString(), ".txt").Trim()));
                }
                script = "alert('SAM - IMSS: Custodio S3 exitoso.');";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", script, true);
            }

            //*****************************************************************************************
            // ******************* ESTE METODO GENERA EL ARCHIVO ZIP Y LO DESCARGA  *******************
            //*****************************************************************************************
                        
            DescargarArchivo();

            

        }


        /// <summary>
        /// Crea un archivo comprimido con todos los archivos creados
        /// </summary>
        /// <returns></returns>
        public string CrearZipDeCarpeta() {

            string carpetaOrigen = HttpContext.Current.Server.MapPath("~/App_Data/Generate");
            string rutaZip = HttpContext.Current.Server.MapPath("~/App_Data/Generate.zip");

            if (File.Exists(rutaZip))
            {
                File.Delete(rutaZip); //Elimino el ZIP si ya existe.
            }

            System.IO.Compression.ZipFile.CreateFromDirectory(carpetaOrigen, rutaZip);
            return rutaZip;
        }

        /// <summary>
        /// Una vez que se ha creado el archivo Zip lo descarga al la maquina del usuario.
        /// </summary>
        public void DescargarArchivo() { 
        
               string rutaZip = CrearZipDeCarpeta();

            if (System.IO.File.Exists(rutaZip)) {

                string nombreArchivo = "ArchivosGenerados.zip";

                Response.Clear();
                Response.ContentType = "application/zip";
                Response.AppendHeader("Content-Disposition", "attachment; filename" + nombreArchivo);
                Response.TransmitFile(rutaZip);
                Response.End();            
            }                    
        }

        /// <summary>
        /// Elimina todos los archivos de la carpeta Generate para empezara generar los nuevos.
        /// </summary>
        public void EliminarArchivosGenerados() {

            string carpeta = Server.MapPath("~/App_Data/Generate");

            if (Directory.Exists(carpeta)) {

                string[] archivos = Directory.GetFiles(carpeta);

                foreach (string archivo in archivos)
                {
                    try
                    {
                        File.Delete(archivo);
                    }
                    catch (Exception)
                    {
                        throw;
                    }

                }
            
            }

        }

        public void SaveAsCsvVectorMD(string destinationCsvFilePath)
        {
            try
            {
                // Obtener el archivo desde el control FileUpload
                byte[] fileBytes = fuArchivoExcelMDValmer.FileBytes;

                using (var stream = new MemoryStream(fileBytes))
                {
                    IWorkbook workbook;

                    // Detectar si el archivo es .xls o .xlsx
                    if (fuArchivoExcelMDValmer.FileName.EndsWith(".xls"))
                    {
                        workbook = new HSSFWorkbook(stream); // Formato antiguo
                    }
                    else
                    {
                        workbook = new XSSFWorkbook(stream); // Formato moderno
                    }

                    // Obtener la primera hoja
                    ISheet sheet = workbook.GetSheetAt(0);

                    // Crear el archivo CSV
                    using (var writer = new StreamWriter(destinationCsvFilePath))
                    {
                        int rows = sheet.PhysicalNumberOfRows;

                        // Iterar sobre cada fila
                        for (int rowIdx = 0; rowIdx < rows; rowIdx++)
                        {
                            IRow row = sheet.GetRow(rowIdx);
                            if (row == null)
                                continue; // Saltar filas vacías

                            var rowData = new List<string>();

                            // Iterar sobre cada celda
                            for (int colIdx = 0; colIdx < row.LastCellNum; colIdx++)
                            {
                                ICell cell = row.GetCell(colIdx, MissingCellPolicy.CREATE_NULL_AS_BLANK);
                                string cellValue = GetCellValue(cell); // Método auxiliar para obtener el valor como texto

                                // Reemplazar comas para evitar errores en el CSV
                                rowData.Add(cellValue.Replace(",", ""));
                            }

                            // Escribir la fila al CSV
                            writer.WriteLine(string.Join(",", rowData));
                        }
                    }
                }

                // Mensaje de éxito
                string script = "alert('Archivo convertido a CSV exitosamente.');";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", script, true);
            }
            catch (Exception ex)
            {
                // Manejo de excepciones
                string script = $"alert('Error: {ex.Message}');";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", script, true);
            }
        }

        // Método auxiliar para obtener el valor de la celda como texto
        private string GetCellValue(ICell cell)
        {
            if (cell == null)
                return "";

            switch (cell.CellType)
            {
                case CellType.String:
                    return cell.StringCellValue.Trim();
                case CellType.Numeric:
                    return cell.NumericCellValue.ToString();
                case CellType.Boolean:
                    return cell.BooleanCellValue ? "TRUE" : "FALSE";
                case CellType.Formula:
                    return cell.CellFormula;
                default:
                    return "";
            }
        }

        public void SaveAsCsvVectorPiP(string destinationCsvFilePath)
        {
            try
            {
                using (var streamVecPIP = new MemoryStream(FileUploadVecPIP.FileBytes))
                {
                    IWorkbook workbook;

                    // Determinar si el archivo es .xls o .xlsx
                    if (Path.GetExtension(FileUploadVecPIP.FileName).ToLower() == ".xls")
                    {
                        workbook = new HSSFWorkbook(streamVecPIP); // Formato antiguo
                    }
                    else
                    {
                        workbook = new XSSFWorkbook(streamVecPIP); // Formato moderno
                    }

                    // Obtener la primera hoja
                    ISheet sheet = workbook.GetSheetAt(0);

                    // Eliminar la primera fila (la cabecera está en la segunda fila)
                    sheet.ShiftRows(1, sheet.LastRowNum, -1); // Mueve todas las filas hacia arriba

                    // Convertir la hoja a un DataTable
                    DataTable dataTable = ConvertSheetToDataTable(sheet);

                    // Eliminar la columna "NOMBRE COMPLETO" (columna J, índice 9)
                    if (dataTable.Columns.Contains("NOMBRE COMPLETO"))
                    {
                        dataTable.Columns.Remove("NOMBRE COMPLETO");
                    }

                    // Crear el archivo CSV
                    using (var writer = new StreamWriter(destinationCsvFilePath))
                    {
                        // Escribir las filas del DataTable al CSV
                        foreach (DataRow row in dataTable.Rows)
                        {
                            var rowData = row.ItemArray.Select(cell => cell.ToString().Replace(",", "")).ToArray();
                            writer.WriteLine(string.Join(",", rowData));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Manejo de errores
                string script = $"alert('Error: {ex.Message}');";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", script, true);
            }
        }

        // Método para convertir la hoja de Excel a un DataTable
        private DataTable ConvertSheetToDataTable(ISheet sheet)
        {
            var dataTable = new DataTable();

            // Asumimos que la primera fila contiene los nombres de las columnas
            IRow headerRow = sheet.GetRow(0);
            if (headerRow != null)
            {
                foreach (ICell cell in headerRow.Cells)
                {
                    dataTable.Columns.Add(cell.ToString()); // Agregar las columnas al DataTable
                }
            }

            // Agregar las filas al DataTable
            for (int rowIdx = 1; rowIdx <= sheet.LastRowNum; rowIdx++) // Comienza desde la segunda fila
            {
                IRow row = sheet.GetRow(rowIdx);
                if (row != null)
                {
                    var rowData = new object[headerRow.Cells.Count];
                    for (int colIdx = 0; colIdx < rowData.Length; colIdx++)
                    {
                        ICell cell = row.GetCell(colIdx, MissingCellPolicy.CREATE_NULL_AS_BLANK);
                        //var value = GetCellValue(cell); //Esta comentado porque cambiaba los valores 
                        var value = cell.ToString();

                        //Si estamos en la columna "M" (Que es indice 12)
                        if (colIdx == 12)
                        {
                            if (value.ToString() == "-")
                            {
                                value = "0";
                            }
                            if (decimal.TryParse(value?.ToString(), out decimal number))
                            {
                                if (Convert.ToDouble(value) > 99999999999)
                                { 
                                    //Si es numero formatearlo a notacion cientifica
                                    value = number.ToString("0.#####E+0");
                                }
                            }
                        }

                        //Si estamos en la columna "N" (Esto se hace porque es fecha y si no cambia el valor a tipo numerico)
                        if (colIdx == 13 || colIdx == 28 || colIdx == 54 || colIdx == 55)
                        {
                            if (cell.CellType == CellType.Numeric && DateUtil.IsCellDateFormatted(cell))
                            {
                                DateTime fecha = Convert.ToDateTime(cell.DateCellValue);
                                value = fecha.ToString("MM/dd/yyyy").ToString();
                            }
                            else 
                            {
                                value = cell.ToString();
                            }
                        }

                        rowData[colIdx] = value;

                    }
                    dataTable.Rows.Add(rowData);
                }
            }

            return dataTable;
        }


        public void LimpiarTablaVector()
        {

            //Limpio tabla de paso y tabla del vector
            string strTruncate = @"Truncate table VectorAnaliticoValmerCSV";

            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["SAM_IMSS_Connection"].ConnectionString);

            con.Open();

            //Valido que la fecha del día sea igual a la del sistema
            SqlCommand cmd = new SqlCommand(strTruncate, con);
            cmd.CommandType = CommandType.Text;

            if (chkCSV.Checked)
            {
                cmd.ExecuteNonQuery();
            }

            con.Close();

            strTruncate = @"Truncate table VectorAnaliticoValmer";

            con = new SqlConnection(ConfigurationManager.ConnectionStrings["SAM_IMSS_Connection"].ConnectionString);

            con.Open();

            //Valido que la fecha del día sea igual a la del sistema
            cmd = new SqlCommand(strTruncate, con);
            cmd.CommandType = CommandType.Text;

            if (chkXLS.Checked)
            {
                cmd.ExecuteNonQuery();
            }

            con.Close();

        }

        public void LimpiarVectorPiP()
        {

            string strTruncate = @"Truncate table VectorAnaliticoPiP";

            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["SAM_IMSS_Connection"].ConnectionString);

            con.Open();

            //Valido que la fecha del día sea igual a la del sistema
            SqlCommand cmd = new SqlCommand(strTruncate, con);
            cmd.CommandType = CommandType.Text;
            cmd.ExecuteNonQuery();

            con.Close();
        }

        private int ValidoInstrumentosenVector()
        {

            int valor = 0;


            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["SAM_IMSS_Connection"].ConnectionString);

            //Valido que el vector de valmer se el del día
            string strqry = @"Select count(*) from IMSS_Positions where TipoValor not in (select[TIPO VALOR] + '_' + EMISORA + '_' + SERIE from VectorAnaliticoPiP)and TipoValor <> ''";
            con.Open();
            SqlCommand cmd = new SqlCommand(strqry, con);
            cmd.CommandType = CommandType.Text;
            int count = Convert.ToInt32(cmd.ExecuteScalar());

            if (count == 0)
                valor = 0;
            else
                valor = 1;

            if (chkCSV.Checked)
            {
                strqry = "Select count(*) from IMSS_Positions where TipoValor not in (select TV + '_' + EMISORA + '_' + SERIE from VectorAnaliticoValmerCSV) and TipoValor <> ''";
            }
            if (chkXLS.Checked)
            {
                strqry = "Select count(*) from IMSS_Positions where TipoValor not in (select [TIPO VALOR] + '_' + EMISORA + '_' + SERIE from VectorAnaliticoValmer) and TipoValor <> ''";
            }
            cmd = new SqlCommand(strqry, con);
            cmd.CommandType = CommandType.Text;
            count = Convert.ToInt32(cmd.ExecuteScalar());

            if (count == 0)
                valor = 0;
            else
                valor = 2;

            return valor;
        }

        private void BulkVectorValmerCSV(string rutaString)
        {
            // Usamos la ruta relativa dentro del directorio del proyecto, o dentro de un directorio temporal
            string projectPath = AppDomain.CurrentDomain.BaseDirectory; // Obtiene el directorio base del proyecto
            string outputDir = Path.Combine(projectPath, "FileTemp"); // Ruta dentro del directorio del proyecto
            string outputFilePath = Path.Combine(outputDir, "vector_precioscopia.csv"); // Ruta dinámica para el archivo de salida

            // Verificar si la carpeta ArchivosSalida existe, si no, crearla
            if (!Directory.Exists(outputDir))
            {
                Directory.CreateDirectory(outputDir);
            }

            // Crear el archivo temporal "copia.csv" en la ruta dinámica
            using (StreamWriter fileWrite = new StreamWriter(outputFilePath.Replace(".csv", "copia.csv")))
            {
                using (StreamReader fielRead = new StreamReader(rutaString))
                {
                    string linea;

                    // Leer las líneas del archivo original y escribir en el archivo temporal
                    while ((linea = fielRead.ReadLine()) != null)
                    {
                        string[] datos = linea.Split(new char[] { ',' });

                        if (datos[0] != "TipoMercado")
                        {
                            fileWrite.WriteLine(linea);
                        }
                    }
                }
            }

            // Eliminar el archivo original
            File.Delete(rutaString);

            // Renombrar el archivo temporal a la ruta original
            File.Move(outputFilePath.Replace(".csv", "copia.csv"), rutaString);

            int i = 0;
            string connectionString = ConfigurationManager.ConnectionStrings["SAM_IMSS_Connection"].ConnectionString;
            var dbConn = new SqlConnection(connectionString);
            var sr = new StreamReader(rutaString); // Leer el archivo original nuevamente

            string line = sr.ReadLine(); // Leer la primera línea (cabecera)
            string[] strArray = line.Split(',');
            var dt = new DataTable();

            // Crear columnas en el DataTable según la cabecera
            for (int index = 0; index < strArray.Length; index++)
                dt.Columns.Add(new DataColumn());

            // Leer las líneas restantes del archivo CSV
            do
            {
                DataRow row = dt.NewRow();
                string[] itemArray = line.Split(',');
                row.ItemArray = itemArray;
                dt.Rows.Add(row);
                i = i + 1;
                line = sr.ReadLine();
            } while (!string.IsNullOrEmpty(line));

            var bc = new SqlBulkCopy(dbConn, SqlBulkCopyOptions.TableLock, null)
            {
                DestinationTableName = "VectorAnaliticoValmerCSV",
                BatchSize = dt.Rows.Count
            };

            // Conectar a la base de datos y escribir los datos en la tabla
            dbConn.Open();
            bc.WriteToServer(dt);
            dbConn.Close();
            bc.Close();

            // Cerrar el StreamReader y eliminar el archivo original
            sr.Close();
            File.Delete(rutaString); // Eliminar el archivo original una vez cargado

        }



        private void BulkVectorValmer(string rutaString)
        {

            int i = 0;

            string connectionString = (ConfigurationManager.ConnectionStrings["SAM_IMSS_Connection"].ConnectionString);
            var dbConn = new SqlConnection(connectionString);
            var sr = new StreamReader(@rutaString);
            string line = sr.ReadLine();

            string[] strArray = line.Split(',');
            var dt = new DataTable();
            var dt2 = new DataTable();
            var dt3 = new DataTable();
            var dt4 = new DataTable();
            var dt5 = new DataTable();
            var dt6 = new DataTable();

            for (int index = 0; index < strArray.Length; index++)
            {
                dt.Columns.Add(new DataColumn());
                dt2.Columns.Add(new DataColumn());
                dt3.Columns.Add(new DataColumn());
                dt4.Columns.Add(new DataColumn());
                dt5.Columns.Add(new DataColumn());
                dt6.Columns.Add(new DataColumn());
            }

            do
            {



                string[] itemArray = line.Split(',');
                if (i < 5000)
                {
                    if (i > 0)
                    {
                        DataRow row = dt.NewRow();
                        row.ItemArray = itemArray;
                        dt.Rows.Add(row);
                    }
                }

                if (i >= 5000 && i < 10000)
                {
                    DataRow row2 = dt2.NewRow();
                    row2.ItemArray = itemArray;
                    dt2.Rows.Add(row2);
                }

                if (i >= 10000 && i < 15000)
                {
                    DataRow row3 = dt3.NewRow();
                    row3.ItemArray = itemArray;
                    dt3.Rows.Add(row3);
                }

                if (i >= 15000 && i < 20000)
                {
                    DataRow row4 = dt4.NewRow();
                    row4.ItemArray = itemArray;
                    dt4.Rows.Add(row4);
                }

                if (i >= 20000 && i < 25000)
                {
                    DataRow row5 = dt5.NewRow();
                    row5.ItemArray = itemArray;
                    dt5.Rows.Add(row5);
                }


                if (i >= 25000)
                {
                    DataRow row6 = dt6.NewRow();
                    row6.ItemArray = itemArray;
                    dt6.Rows.Add(row6);
                }


                i = i + 1;
                line = sr.ReadLine();
            } while (!string.IsNullOrEmpty(line));


            var bc = new SqlBulkCopy(dbConn, SqlBulkCopyOptions.TableLock, null)
            {
                DestinationTableName = "VectorAnaliticoValmer",
                BatchSize = dt.Rows.Count
            };
            dbConn.Open();
            bc.WriteToServer(dt);
            dbConn.Close();
            bc.Close();

            //Segunda li
            var bc2 = new SqlBulkCopy(dbConn, SqlBulkCopyOptions.TableLock, null)
            {
                DestinationTableName = "VectorAnaliticoValmer",
                BatchSize = dt2.Rows.Count
            };
            dbConn.Open();
            bc2.WriteToServer(dt2);
            dbConn.Close();
            bc2.Close();

            //tercer li
            var bc3 = new SqlBulkCopy(dbConn, SqlBulkCopyOptions.TableLock, null)
            {
                DestinationTableName = "VectorAnaliticoValmer",
                BatchSize = dt3.Rows.Count
            };
            dbConn.Open();
            bc3.WriteToServer(dt3);
            dbConn.Close();
            bc3.Close();

            //cuarto li
            var bc4 = new SqlBulkCopy(dbConn, SqlBulkCopyOptions.TableLock, null)
            {
                DestinationTableName = "VectorAnaliticoValmer",
                BatchSize = dt4.Rows.Count
            };
            dbConn.Open();
            bc4.WriteToServer(dt4);
            dbConn.Close();
            bc4.Close();

            //cuarto li
            var bc5 = new SqlBulkCopy(dbConn, SqlBulkCopyOptions.TableLock, null)
            {
                DestinationTableName = "VectorAnaliticoValmer",
                BatchSize = dt5.Rows.Count
            };
            dbConn.Open();
            bc5.WriteToServer(dt5);
            dbConn.Close();
            bc5.Close();

            var bc6 = new SqlBulkCopy(dbConn, SqlBulkCopyOptions.TableLock, null)
            {
                DestinationTableName = "VectorAnaliticoValmer",
                BatchSize = dt6.Rows.Count
            };
            dbConn.Open();
            bc6.WriteToServer(dt6);
            dbConn.Close();
            bc6.Close();

            sr.Close();
            File.Delete(@rutaString);

        }

        /// <summary>
        /// Hace el vaciado de Vector PIP a la base de datos.
        /// </summary>
        /// <param name="strfile"></param>
        public static void BulkVectorPiP(string strfile)
        {
            int i = 0;
            string connectionString = (ConfigurationManager.ConnectionStrings["SAM_IMSS_Connection"].ConnectionString);
            var dbConn = new SqlConnection(connectionString);
            var sr = new StreamReader(@strfile);
            string line = sr.ReadLine();

            string[] strArray = line.Split(',');
            var dt = new DataTable();
            var dt2 = new DataTable();
            var dt3 = new DataTable();
            var dt4 = new DataTable();
            var dt5 = new DataTable();
            var dt6 = new DataTable();

            for (int index = 0; index < strArray.Length; index++)
            {
                dt.Columns.Add(new DataColumn());
                dt2.Columns.Add(new DataColumn());
                dt3.Columns.Add(new DataColumn());
                dt4.Columns.Add(new DataColumn());
                dt5.Columns.Add(new DataColumn());
                dt6.Columns.Add(new DataColumn());
            }

            do
            {


                string[] itemArray = line.Split(',');
                if (i < 5000)
                {
                    DataRow row = dt.NewRow();
                    row.ItemArray = itemArray;
                    dt.Rows.Add(row);
                }

                if (i >= 5000 && i < 10000)
                {
                    DataRow row2 = dt2.NewRow();
                    row2.ItemArray = itemArray;
                    dt2.Rows.Add(row2);
                }

                if (i >= 10000 && i < 15000)
                {
                    DataRow row3 = dt3.NewRow();
                    row3.ItemArray = itemArray;
                    dt3.Rows.Add(row3);
                }

                if (i >= 15000 && i < 20000)
                {
                    DataRow row4 = dt4.NewRow();
                    row4.ItemArray = itemArray;
                    dt4.Rows.Add(row4);
                }


                if (i >= 20000 && i < 25000)
                {
                    DataRow row5 = dt5.NewRow();
                    row5.ItemArray = itemArray;
                    dt5.Rows.Add(row5);
                }

                if (i >= 25000)
                {
                    DataRow row6 = dt6.NewRow();
                    row6.ItemArray = itemArray;
                    dt6.Rows.Add(row6);
                }

                i = i + 1;
                line = sr.ReadLine();
            } while (!string.IsNullOrEmpty(line));



            var bc = new SqlBulkCopy(dbConn, SqlBulkCopyOptions.TableLock, null)
            {
                DestinationTableName = "VectorAnaliticoPiP",
                BatchSize = dt.Rows.Count
            };
            dbConn.Open();
            bc.WriteToServer(dt);
            dbConn.Close();
            bc.Close();

            //Segunda li
            var bc2 = new SqlBulkCopy(dbConn, SqlBulkCopyOptions.TableLock, null)
            {
                DestinationTableName = "VectorAnaliticoPiP",
                BatchSize = dt2.Rows.Count
            };
            dbConn.Open();
            bc2.WriteToServer(dt2);
            dbConn.Close();
            bc2.Close();

            //tercer li
            var bc3 = new SqlBulkCopy(dbConn, SqlBulkCopyOptions.TableLock, null)
            {
                DestinationTableName = "VectorAnaliticoPiP",
                BatchSize = dt3.Rows.Count
            };
            dbConn.Open();
            bc3.WriteToServer(dt3);
            dbConn.Close();
            bc3.Close();

            //cuarto li
            var bc4 = new SqlBulkCopy(dbConn, SqlBulkCopyOptions.TableLock, null)
            {
                DestinationTableName = "VectorAnaliticoPiP",
                BatchSize = dt4.Rows.Count
            };
            dbConn.Open();
            bc4.WriteToServer(dt4);
            dbConn.Close();
            bc4.Close();

            var bc5 = new SqlBulkCopy(dbConn, SqlBulkCopyOptions.TableLock, null)
            {
                DestinationTableName = "VectorAnaliticoPiP",
                BatchSize = dt5.Rows.Count
            };
            dbConn.Open();
            bc5.WriteToServer(dt5);
            dbConn.Close();
            bc5.Close();


            var bc6 = new SqlBulkCopy(dbConn, SqlBulkCopyOptions.TableLock, null)
            {
                DestinationTableName = "VectorAnaliticoPiP",
                BatchSize = dt6.Rows.Count
            };
            dbConn.Open();
            bc6.WriteToServer(dt6);
            dbConn.Close();
            bc6.Close();

            //Ya que cargue el archivo lo elimino
            sr.Close();
            File.Delete(@strfile);

        }

        public static void LimpiarTablasAladdinPosition()
        {

            //Limpio tabla de paso y tabla del vector
            string strTruncate = @"Truncate table IMSS_Positions";



            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["SAM_IMSS_Connection"].ConnectionString);

            con.Open();

            //Valido que la fecha del día sea igual a la del sistema
            SqlCommand cmd = new SqlCommand(strTruncate, con);
            cmd.CommandType = CommandType.Text;
            cmd.ExecuteNonQuery();

            con.Close();


            strTruncate = @"Truncate table IMSS_trades";
            con.Open();

            cmd = new SqlCommand(strTruncate, con);
            cmd.CommandType = CommandType.Text;
            cmd.ExecuteNonQuery();

            con.Close();
        }

        public static void LimpiarTablasAladdinTrades()
        {

            string strTruncate = @"Truncate table IMSS_CustodioTrades";


            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["SAM_IMSS_Connection"].ConnectionString);
            con.Open();

            SqlCommand cmd = new SqlCommand(strTruncate, con);
            cmd.CommandType = CommandType.Text;
            cmd.ExecuteNonQuery();

            con.Close();


        }


        public void LeerArchivoPositions(string ruta)
        {
            try
            {
                // Establecer el contexto de la licencia
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial; // Para uso no comercial
                                                                            // Cargar el archivo Excel utilizando EPPlus
                                                                            //FileInfo fileInfo = new FileInfo( ruta );
                                                                            //using(var package = new ExcelPackage( fileInfo ))

                using (var streamPositions = new MemoryStream(FileUploadPositionAladdin.FileBytes))
                using (var packagePositions = new ExcelPackage(streamPositions))
                {
                    // Conectar a la base de datos
                    SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["SAM_IMSS_Connection"].ConnectionString);
                    con.Open();

                    try
                    {
                        // Abro la hoja de posiciones (primer worksheet)
                        var hojaExcel = packagePositions.Workbook.Worksheets[0]; // Primer worksheet
                        int ultimaFila = hojaExcel.Dimension.End.Row; // Obtiene la última fila con datos
                        for (int fila = 13; fila <= ultimaFila; fila++)
                        {

                            //Declaracion de variables
                            string Settle_Date = string.Empty;
                            string Maturity = string.Empty;
                            string TradeDate = string.Empty;

                            // Leer los valores de las celdas
                            string Buy_Sell = hojaExcel.Cells[fila, 1].Text;

                            // Si la celda Buy_Sell está vacía, terminamos el ciclo
                            if (string.IsNullOrEmpty(Buy_Sell))
                                break;

                            string Portfolio = hojaExcel.Cells[fila, 2].Text;
                            string InvNum = hojaExcel.Cells[fila, 3].Text;
                            string TipoValor = hojaExcel.Cells[fila, 4].Text;
                            string Td_Num = hojaExcel.Cells[fila, 5].Text;


                            if (hojaExcel.Cells[fila, 5].Text != "")
                            {
                                TradeDate = hojaExcel.Cells[fila, 6].Text.Substring(3, 2) + "/" + hojaExcel.Cells[fila, 6].Text.Substring(0, 2) + "/" + hojaExcel.Cells[fila, 6].Text.Substring(6, 4);
                            }
                            else
                            {
                                TradeDate = hojaExcel.Cells[fila, 6].Text;
                            }

                            string CollateralQuantity = hojaExcel.Cells[fila, 7].Text.Replace(",", "");
                            string Orig_Face = hojaExcel.Cells[fila, 8].Text.Replace(",", "");
                            string PurchasePrice = hojaExcel.Cells[fila, 9].Text.Replace(",", "");
                            string Coupon = hojaExcel.Cells[fila, 10].Text;


                            if (hojaExcel.Cells[fila, 11].Text != "")
                            {
                                Settle_Date = hojaExcel.Cells[fila, 11].Text.Substring(3, 2) + "/" + hojaExcel.Cells[fila, 11].Text.Substring(0, 2) + "/" + hojaExcel.Cells[fila, 11].Text.Substring(6, 4);
                            }
                            else
                            {
                                Settle_Date = hojaExcel.Cells[fila, 11].Text;
                            }

                            if (hojaExcel.Cells[fila, 12].Text != "")
                            {
                                Maturity = hojaExcel.Cells[fila, 12].Text.Substring(3, 2) + "/" + hojaExcel.Cells[fila, 12].Text.Substring(0, 2) + "/" + hojaExcel.Cells[fila, 12].Text.Substring(6, 4);
                            }
                            else
                            {
                                Maturity = hojaExcel.Cells[fila, 12].Text;
                            }

                            string Currency = hojaExcel.Cells[fila, 13].Text;
                            string Collateral_Price = hojaExcel.Cells[fila, 14].Text;
                            string Collateral_ISIN = hojaExcel.Cells[fila, 15].Text;
                            string ISIN = hojaExcel.Cells[fila, 16].Text;
                            string Settled = hojaExcel.Cells[fila, 17].Text.Replace(",", "");
                            string CUSIP = hojaExcel.Cells[fila, 18].Text;

                            if (string.IsNullOrEmpty(Buy_Sell)) // Si Buy_Sell está vacío, terminamos el ciclo
                            {
                                break;
                            }

                            // Insertar en la base de datos
                            string strValues = string.Concat("'", Buy_Sell, "','", Portfolio, "','", InvNum, "','", TipoValor, "','", Td_Num, "','", TradeDate, "','",
                                CollateralQuantity, "','", Orig_Face, "','", PurchasePrice, "','", Coupon, "','",
                                Settle_Date, "','", Maturity, "','", Currency, "','", Collateral_Price, "','", Collateral_ISIN, "','", CUSIP, "','", ISIN, "','", Settled, "'");

                            string strInsert = @"INSERT INTO IMSS_Positions (Buy_Sell, Portfolio, InvNum, TipoValor, Td_Num, TradeDate, 
                    CollateralQuantity, Orig_Face, PurchasePrice, Coupon, 
                    Settle_Date, Maturity, Currency, Collateral_Price, Collateral_ISIN, CUSIP, ISIN, Settled) 
                    VALUES (" + strValues + ")";

                            SqlCommand cmd = new SqlCommand(strInsert, con);
                            cmd.CommandType = CommandType.Text;
                            cmd.ExecuteNonQuery();
                        }

                    }
                    catch (Exception e)
                    {
                        // Manejo de excepciones y registro de logs
                        using (StreamWriter swlog = File.AppendText(pathlog.ToString()))
                        {
                            swlog.WriteLine(string.Concat(DateTime.Now.ToString(), "--", "Error al cargar archivo Position -- ", e.Message));
                        }

                        string script = "alert('Error');";
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", script, true);
                        throw;
                    }

                    try
                    {

                        string TradeDateDos = string.Empty;
                        string SettleDateDos = string.Empty;

                        // Abro la hoja de operaciones (segundo worksheet)
                        var hojaExcelDos = packagePositions.Workbook.Worksheets[1]; // Segundo worksheet
                        int ultimaFilaDos = hojaExcelDos.Dimension.End.Row; // Obtiene la última fila con datos
                        for (int fila = 13; fila <= ultimaFilaDos; fila++)
                        {
                            // Leer los valores de las celdas
                            string Fund = hojaExcelDos.Cells[fila, 1].Text;

                            // Si la celda Buy_Sell está vacía, terminamos el ciclo
                            if (string.IsNullOrEmpty(Fund))
                                break;

                            string InvNum = hojaExcelDos.Cells[fila, 2].Text;
                            string TipoValor = hojaExcelDos.Cells[fila, 3].Text;
                            string Td_Num = hojaExcelDos.Cells[fila, 4].Text;
                            string CounterParty = hojaExcelDos.Cells[fila, 5].Text;
                            string Buy_Sell = hojaExcelDos.Cells[fila, 6].Text;
                            string TranType = hojaExcelDos.Cells[fila, 7].Text;
                            string TradeFace = hojaExcelDos.Cells[fila, 8].Text.Replace(",", "");
                            string OrigFace = hojaExcelDos.Cells[fila, 9].Text.Replace(",", "");
                            string TradePrice = hojaExcelDos.Cells[fila, 10].Text.Replace(",", "");

                            if (hojaExcelDos.Cells[fila, 11].Text != "")
                            {
                                TradeDateDos = hojaExcelDos.Cells[fila, 11].Text.Substring(3, 2) + "/" + hojaExcelDos.Cells[fila, 11].Text.Substring(0, 2) + "/" + hojaExcelDos.Cells[fila, 11].Text.Substring(6, 4);
                            }
                            else
                            {
                                TradeDateDos = hojaExcelDos.Cells[fila, 11].Text;
                            }

                            if (hojaExcelDos.Cells[fila, 12].Text != "")
                            {
                                SettleDateDos = hojaExcelDos.Cells[fila, 12].Text.Substring(3, 2) + "/" + hojaExcelDos.Cells[fila, 12].Text.Substring(0, 2) + "/" + hojaExcelDos.Cells[fila, 12].Text.Substring(6, 4);
                            }
                            else
                            {
                                SettleDateDos = hojaExcelDos.Cells[fila, 12].Text;
                            }

                            string Principal = hojaExcelDos.Cells[fila, 13].Text.Replace(",", "");
                            string NetMoney = hojaExcelDos.Cells[fila, 14].Text.Replace(",", "");
                            string Collateral_ISIN = hojaExcelDos.Cells[fila, 15].Text;

                            string Collateral_Quantity = hojaExcelDos.Cells[fila, 16].Text.Replace(",", "");
                            int indexOfDot = Collateral_Quantity.IndexOf(".");
                            Collateral_Quantity = indexOfDot >= 0 ? Collateral_Quantity.Substring(0, indexOfDot) : Collateral_Quantity;

                            string Cupon = hojaExcelDos.Cells[fila, 17].Text;
                            string Maturity = hojaExcelDos.Cells[fila, 18].Text;
                            string IssueDate = hojaExcelDos.Cells[fila, 19].Text;

                            if (string.IsNullOrEmpty(Td_Num) || Td_Num.Contains(")")) // Si Td_Num está vacío o contiene paréntesis, terminamos el ciclo
                            {
                                break;
                            }

                            // Insertar en la base de datos
                            string strValues = string.Concat("'", Fund, "','", InvNum, "','", TipoValor, "','", Td_Num, "','", CounterParty, "','", Buy_Sell, "','", TranType, "','",
                                TradeFace, "','", OrigFace, "','", TradePrice, "','", TradeDateDos, "','", SettleDateDos, "','", Principal, "','", NetMoney, "','", Collateral_ISIN, "','",
                                Collateral_Quantity, "','", Cupon, "','", Maturity, "','", IssueDate, "'");

                            string strInsert = @"INSERT INTO IMSS_Trades (Fund, InvNum, TipoValor, Td_Num, CounterParty, Buy_Sell, TranType, 
                    TradeFace, OrigFace, TradePrice, TradeDate, SettleDate, Principal, NetMoney, Collateral_ISIN, Collateral_Quantity, Cupon,
                    Maturity, IssueDate) 
                    VALUES (" + strValues + ")";

                            SqlCommand cmd = new SqlCommand(strInsert, con);
                            cmd.CommandType = CommandType.Text;
                            cmd.ExecuteNonQuery();
                        }


                    }
                    catch (Exception e)
                    {
                        // Manejo de excepciones y registro de logs
                        using (StreamWriter swlog = File.AppendText(pathlog.ToString()))
                        {
                            swlog.WriteLine(string.Concat(DateTime.Now.ToString(), "--", "Error al cargar archivo Position -- ", e.Message));
                        }

                        string script = "alert('Error');";
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", script, true);
                        throw;
                    }


                    con.Close(); // Cerrar la conexión

                    // Eliminar objetos Excel (esto no es necesario en EPPlus)
                }
            }
            catch (Exception e)
            {
                // Manejo de excepciones y registro de logs
                using (StreamWriter swlog = File.AppendText(pathlog.ToString()))
                {
                    swlog.WriteLine(string.Concat(DateTime.Now.ToString(), "--", "Error al cargar archivo Position -- ", e.Message));
                }

                string script = "alert('Error');";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", script, true);
            }
            finally
            {
                // Registro de éxito
                using (StreamWriter swlog = File.AppendText(pathlog.ToString()))
                {
                    swlog.WriteLine(string.Concat(DateTime.Now.ToString(), "--", "Carga archivo Position correcta -- ", "Archivo Positions"));
                }

                string script = "alert('SAM - IMSS: Carga de Positions correcta.');";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", script, true);
            }
        }

        public void LeerArchivoTrades(string ruta)
        {
            try
            {
                // Establecer el contexto de la licencia
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial; // Para uso no comercial

                // Carga del archivo Excel usando EPPlus
                //FileInfo archivoExcel = new FileInfo( ruta );
                //using(ExcelPackage paqueteExcel = new ExcelPackage( archivoExcel ))
                using (var streamPositions = new MemoryStream(FileUploadTrades.FileBytes))
                using (var paqueteExcel = new ExcelPackage(streamPositions))
                {
                    ExcelWorksheet hojaExcel = paqueteExcel.Workbook.Worksheets[0]; // Usamos el índice 1 para la primera hoja (recordar que es 0-indexed)

                    // Obtener el número de la última fila con datos
                    int ultimaFilaConDatos = hojaExcel.Dimension.End.Row;

                    // Conexión a la base de datos
                    using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["SAM_IMSS_Connection"].ConnectionString))
                    {
                        con.Open();

                        for (int fila = 13; fila <= ultimaFilaConDatos; fila++)
                        {
                            // Leer los datos de las celdas
                            string InvNum = hojaExcel.Cells[fila, 1].Text;
                            string Td_Num = hojaExcel.Cells[fila, 2].Text;
                            string Fund = hojaExcel.Cells[fila, 3].Text;
                            string Tran_Type = hojaExcel.Cells[fila, 4].Text;
                            string Trader = hojaExcel.Cells[fila, 5].Text;
                            string Tipo_Valor = hojaExcel.Cells[fila, 6].Text;
                            string Trade_Date = hojaExcel.Cells[fila, 7].Text.Substring(3, 2) + "/" + hojaExcel.Cells[fila, 7].Text.Substring(0, 2) + "/" + hojaExcel.Cells[fila, 7].Text.Substring(6, 4);
                            string Settle_Date = hojaExcel.Cells[fila, 8].Text.Substring(3, 2) + "/" + hojaExcel.Cells[fila, 8].Text.Substring(0, 2) + "/" + hojaExcel.Cells[fila, 8].Text.Substring(6, 4);
                            string Counterparty = hojaExcel.Cells[fila, 9].Text;
                            string Counterparty_Desk = hojaExcel.Cells[fila, 10].Text;
                            string Currency = hojaExcel.Cells[fila, 11].Text;
                            string Orig_Face = hojaExcel.Cells[fila, 12].Text.Replace(",", "").Split('.')[0];
                            string Trade_Price = hojaExcel.Cells[fila, 13].Text.Replace(",", "");
                            string Effective_Rate = hojaExcel.Cells[fila, 14].Text.Replace(",", "");
                            string Principal = hojaExcel.Cells[fila, 15].Text.Replace(",", "");
                            string Commission = hojaExcel.Cells[fila, 16].Text.Replace(",", "");
                            string Ex_Commission = hojaExcel.Cells[fila, 17].Text.Replace(",", "");
                            string Net_Money = hojaExcel.Cells[fila, 18].Text.Replace(",", "");
                            string CUSIP = hojaExcel.Cells[fila, 19].Text;
                            string ISIN = hojaExcel.Cells[fila, 20].Text;

                            if (InvNum.Contains("("))
                            {
                                // Finalizamos el ciclo si encontramos una fila vacía o marcador de fin
                                break;
                            }

                            // Validación de existencia de INVNUM
                            string strValues = $"'{InvNum}','{Td_Num}','{Fund}','{Tran_Type}','{Trader}','{Tipo_Valor}','{Trade_Date}','{Settle_Date}','{Counterparty}','{Counterparty_Desk}','{Currency}','{Orig_Face}','{Trade_Price}','{Effective_Rate}','{Principal}','{Commission}','{Ex_Commission}','{Net_Money}','{CUSIP}','{ISIN}'";

                            string strInsert = @"
                    IF NOT EXISTS (SELECT * FROM IMSS_CustodioTrades WHERE InvNum = '" + InvNum + @"')
                    BEGIN
                        Insert Into IMSS_CustodioTrades 
                        (InvNum, Td_Num, Fund, Tran_Type, Trader, Tipo_Valor, Trade_Date, Settle_Date, Counterparty, Counterparty_Desk, Currency, Orig_Face, 
                        Trade_Price, Effective_Rate, Principal, Commission, Ex_Commission, Net_Money, CUSIP, ISIN) 
                        values (" + strValues + @")
                    End";

                            // Ejecutar el comando SQL
                            using (SqlCommand cmd = new SqlCommand(strInsert, con))
                            {
                                cmd.CommandType = CommandType.Text;
                                cmd.ExecuteNonQuery();
                            }
                        }

                        con.Close();
                    }

                }
            }
            catch (Exception e)
            {
                string script = "alert('SAM - IMSS: Error.');";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", script, true);
            }
            finally
            {
                string script = "alert('SAM - IMSS: Carga de Trades correcta.');";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", script, true);
            }
        }

        public void ValidarLiquidacionPositionsTrades(string ruta)
        {
            try
            {
                // Establecer el contexto de la licencia
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial; // Para uso no comercial

                //// Verificar que la ruta y el archivo existen
                //bool RutaExiste = Directory.Exists( Path.GetDirectoryName( ruta ) );
                //bool ArchivoExiste = File.Exists( ruta );

                //if(!ArchivoExiste)
                //{
                //    throw new FileNotFoundException( "El archivo no fue encontrado." );
                //}

                // Leer el archivo Excel usando EPPlus
                //using(var package = new ExcelPackage( new FileInfo( ruta ) ))
                using (var streamPositions = new MemoryStream(FileUploadTrades.FileBytes))
                using (var package = new ExcelPackage(streamPositions))
                {
                    // Obtener la primera hoja del archivo Excel
                    var hojaExcel = package.Workbook.Worksheets[0]; // Asumimos que la primera hoja es la que necesitamos
                    int fila = 14; // Comenzamos en la fila 14 como en tu código original

                    // Conexión a la base de datos
                    using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["SAM_IMSS_Connection"].ConnectionString))
                    {
                        con.Open();

                        // Borrar la tabla temporal
                        string strTruncate = @"Truncate table IMSS_TMPTrades";
                        SqlCommand cmd = new SqlCommand(strTruncate, con);
                        cmd.CommandType = CommandType.Text;
                        cmd.ExecuteNonQuery();

                        // Insertar las filas desde el archivo Excel
                        while (hojaExcel.Cells[fila, 1].Value != null)
                        {
                            string Fund = hojaExcel.Cells[fila, 3].Text;
                            string InvNum = hojaExcel.Cells[fila, 1].Text;
                            string TipoValor = hojaExcel.Cells[fila, 6].Text;
                            string Td_Num = hojaExcel.Cells[fila, 2].Text.Replace(",", "");
                            string CounterParty = hojaExcel.Cells[fila, 9].Text;
                            string Buy_Sell = hojaExcel.Cells[fila, 11].Text;
                            string TranType = hojaExcel.Cells[fila, 4].Text;
                            string TradeFace = hojaExcel.Cells[fila, 13].Text.Replace(",", "");
                            string OrigFace = hojaExcel.Cells[fila, 12].Text.Replace(",", "");
                            string TradePrice = hojaExcel.Cells[fila, 13].Text.Replace(",", "");
                            string TradeDate = hojaExcel.Cells[fila, 7].Text;
                            string SettleDate = hojaExcel.Cells[fila, 8].Text;
                            string Principal = hojaExcel.Cells[fila, 15].Text.Replace(",", "");
                            string NetMoney = hojaExcel.Cells[fila, 18].Text.Replace(",", "");
                            string Collateral_ISIN = hojaExcel.Cells[fila, 20].Text;
                            string Collateral_Quantity = hojaExcel.Cells[fila, 1].Text.Replace(",", "");
                            string Cupon = hojaExcel.Cells[fila, 11].Text;
                            string Maturity = hojaExcel.Cells[fila, 11].Text;
                            string IssueDate = hojaExcel.Cells[fila, 11].Text;

                            // Comprobar si la transacción ya no tiene datos
                            if (Td_Num.Contains(")"))
                            {
                                break; // Salir del ciclo si no hay más datos
                            }

                            // Construir la consulta de inserción
                            string strValues = $"'{Fund}','{InvNum}','{TipoValor}','{Td_Num}','{CounterParty}','{Buy_Sell}','{TranType}','{TradeFace}','{OrigFace}','{TradePrice}','{TradeDate}','{SettleDate}','{Principal}','{NetMoney}','{Collateral_ISIN}','{Collateral_Quantity}','{Cupon}','{Maturity}','{IssueDate}'";
                            string strInsert = @"Insert Into IMSS_TMPTrades (Fund,InvNum,TipoValor,Td_Num,CounterParty,Buy_Sell,TranType,TradeFace,OrigFace,TradePrice,TradeDate,SettleDate,Principal,NetMoney,Collateral_ISIN,Collateral_Quantity,Cupon,Maturity, IssueDate) values (" + strValues + ")";

                            // Ejecutar la consulta de inserción
                            cmd = new SqlCommand(strInsert, con);
                            cmd.CommandType = CommandType.Text;
                            cmd.ExecuteNonQuery();

                            fila++;
                        }

                        // Validar las operaciones de liquidación
                        string qryvalida = @"Select count(*) from [IMSS_TMPTrades]
                                      where datediff(dd, getdate(), convert(datetime, FORMAT(CONVERT(DATE, SettleDate, 101), 'dd/MM/yyyy'), 103)) = 0
                                      and TipoValor not in (select TipoValor from [IMSS_Trades]
                                      where datediff(dd, getdate(), convert(datetime, substring(SettleDate, 1, 10), 103)) = 0)";
                        SqlCommand cmdvalida = new SqlCommand(qryvalida, con);
                        cmdvalida.CommandType = CommandType.Text;
                        SqlDataReader rdvalida = cmdvalida.ExecuteReader();

                        while (rdvalida.Read())
                        {
                            if (rdvalida[0].ToString().Trim() != "0")
                            {
                                // Log en caso de que no haya todas las operaciones de liquidación
                                using (StreamWriter swlog = File.AppendText(pathlog.ToString()))
                                {
                                    swlog.WriteLine($"{Recursos.logfecha.ToString()} -- El archivo del día no contiene todas las operaciones por liquidar en comparación al archivo del día habil anterior -- {ruta}");
                                }

                                // Alerta en el front-end
                                string script = "alert('SAM - IMSS: El archivo del día no contiene todas las operaciones por liquidar en comparación al archivo del día habil anterior.');";
                                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", script, true);
                                break;
                            }
                        }
                        rdvalida.Close();
                        con.Close();
                    }
                }

                // Mensaje de éxito
                using (StreamWriter swlog = File.AppendText(pathlog.ToString()))
                {
                    swlog.WriteLine($"{Recursos.logfecha.ToString()} -- Carga archivo Trades Custodio correcta");
                }

                string successScript = "alert('SAM - IMSS: Validación de Positions correcta.');";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", successScript, true);
            }
            catch (Exception e)
            {
                // Manejo de errores
                using (StreamWriter swlog = File.AppendText(pathlog.ToString()))
                {
                    swlog.WriteLine($"{Recursos.logfecha.ToString()} -- El archivo de Trades aún no trae la segunda pestaña -- {e.Message}");
                }

                string errorScript = "alert('Error en la validación de posiciones.');";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", errorScript, true);
            }
        }


        public void ExpArcTrades(string rutaArchivo)
        {
            string Comillas = "\"";

            string detalle = "";
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["SAM_IMSS_Connection"].ConnectionString);

            con.Open();

            using (StreamWriter sw = File.CreateText(rutaArchivo.ToString()))
            { }

            detalle = string.Concat("exec IMSS_ArcTrades '", Recursos.appfecha.ToString(), "'");
            SqlCommand cmd = new SqlCommand(detalle, con);
            cmd.CommandType = CommandType.Text;
            SqlDataReader rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                using (StreamWriter sw = File.AppendText(rutaArchivo.ToString()))
                {
                    string linea = string.Concat(rdr["Mandatario"].ToString(), ",",
                        rdr["FechaArchivo"].ToString(), ",",
                        rdr["FechaOperacion"].ToString(), ",",
                        rdr["Portafolio"].ToString(), ",",
                        rdr["ClaseActivo"].ToString(), ",",
                        rdr["TipoValor"].ToString(), ",",
                        rdr["Emisora"].ToString(), ",",
                        rdr["Serie"].ToString(), ",",
                        rdr["PrecioSucio"].ToString(), ",",
                        rdr["Titulos"].ToString(), ",",
                        rdr["FechaLiquidacion"].ToString(), ",",
                        rdr["Intermediario"].ToString(), ",",
                        rdr["MontoLiquidado"].ToString(), ",",
                        rdr["NumeroMandato"].ToString(), ",",
                        rdr["ClaveFechaLiquidacion"].ToString(), ",",
                        rdr["ClaveOperacion"].ToString(), ",",
                        rdr["PrecioPactado"].ToString());

                    sw.WriteLine(linea);
                }
            }
            rdr.Close();


            con.Close();

        }

        public void ExpArcTradesCSV(string rutaArchivo)
        {
            string Comillas = "\"";

            string detalle = "";
            string connectionString = ConfigurationManager.ConnectionStrings["SAM_IMSS_Connection"].ConnectionString;
            SqlConnection con = new SqlConnection(connectionString);
            con.Open();

            using (StreamWriter sw = File.CreateText(rutaArchivo.ToString()))
            { }

            detalle = string.Concat("exec IMSS_ArcTrades_csv '", Recursos.appfecha.ToString(), "'");
            SqlCommand cmd = new SqlCommand(detalle, con);
            cmd.CommandType = CommandType.Text;
            SqlDataReader rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                using (StreamWriter sw = File.AppendText(rutaArchivo.ToString()))
                {
                    string linea = string.Concat(rdr["Mandatario"].ToString(), ",",
                        rdr["FechaArchivo"].ToString(), ",",
                        rdr["FechaOperacion"].ToString(), ",",
                        rdr["Portafolio"].ToString(), ",",
                        rdr["ClaseActivo"].ToString(), ",",
                        rdr["TipoValor"].ToString(), ",",
                        rdr["Emisora"].ToString(), ",",
                        rdr["Serie"].ToString(), ",",
                        rdr["PrecioSucio"].ToString(), ",",
                        rdr["Titulos"].ToString(), ",",
                        rdr["FechaLiquidacion"].ToString(), ",",
                        rdr["Intermediario"].ToString(), ",",
                        rdr["MontoLiquidado"].ToString(), ",",
                        rdr["NumeroMandato"].ToString(), ",",
                        rdr["ClaveFechaLiquidacion"].ToString(), ",",
                        rdr["ClaveOperacion"].ToString(), ",",
                        rdr["PrecioPactado"].ToString());

                    sw.WriteLine(linea);
                }
            }
            rdr.Close();


            con.Close();

        }

        public void ExpArcPosLayout(string rutaArchivo)
        {
            string encabezado = "";
            string detalle = "";
            string connectionString = ConfigurationManager.ConnectionStrings["SAM_IMSS_Connection"].ConnectionString;
            SqlConnection con = new SqlConnection(connectionString);
            con.Open();

            detalle = string.Concat("exec IMSS_ArcPosLayout '", Recursos.appfecha.ToString(), "'");
            SqlCommand cmd = new SqlCommand(detalle, con);
            cmd.CommandType = CommandType.Text;
            cmd.ExecuteNonQuery();


            encabezado = "Select DISTINCT 'H'as [H], 'MSANT' as [Mandato], FechaPosicion as [Fecha], 'MSANT' as [Mandatario], count(*) as [Registros]  from  IMSS_RepPosLayoutHist where datediff(DD,FechaReporte, getdate()) = 0  GROUP BY FechaPosicion";
            cmd = new SqlCommand(encabezado, con);
            cmd.CommandType = CommandType.Text;
            SqlDataReader rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                using (StreamWriter sw = File.CreateText(rutaArchivo.ToString()))
                {
                    string linea = string.Concat(rdr["H"].ToString(), ",", rdr["Mandato"].ToString(), ",", rdr["Fecha"].ToString(), ",", rdr["Mandatario"].ToString(), ",", rdr["Registros"].ToString());
                    sw.WriteLine(linea);
                }
            }
            rdr.Close();

            detalle = string.Concat("exec IMSS_ArcPosLayout '", Recursos.appfecha.ToString(), "'");
            cmd = new SqlCommand(detalle, con);
            cmd.CommandType = CommandType.Text;
            rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                using (StreamWriter sw = File.AppendText(rutaArchivo.ToString()))
                {
                    string linea = string.Concat(rdr["ClaveOperacion"].ToString(), ",",
                        rdr["ClaveMandato"].ToString(), ",",
                        rdr["FechaPosicion"].ToString(), ",",
                        rdr["Portafolio"].ToString(), ",",
                        rdr["SubPortafolio"].ToString(), ",",
                        rdr["ClaseActivo"].ToString(), ",",
                        rdr["TipoValor"].ToString(), ",",
                        rdr["Emisora"].ToString(), ",",
                        rdr["Serie"].ToString(), ",",
                        rdr["SumaTitulosAcciones"].ToString(), ",",
                        rdr["DiasCupon"].ToString(), ",",
                        rdr["TasaCupon"].ToString(), ",",
                        rdr["DxVCupon"].ToString(), ",",
                        rdr["FechaInicialCupon"].ToString(), ",",
                        rdr["FechaFinalCupon"].ToString(), ",",
                        rdr["FechaEmisionInstrumento"].ToString(), ",",
                        rdr["FechaVencimientoOperacion"].ToString(), ",",
                        rdr["DiasPorVencerInstrumento"].ToString(), ",",
                        rdr["YTM"].ToString(), ",",
                        rdr["TasaPactada"].ToString(), ",",
                        rdr["Moneda"].ToString(), ",",
                        rdr["Subyacente"].ToString(), ",",
                        rdr["SumaMontoInvertido_1"].ToString(), ",",
                        rdr["TipoCambio"].ToString(), ",",
                        rdr["Sector"].ToString(), ",",
                        rdr["S&P"].ToString(), ",",
                        rdr["Fitch"].ToString(), ",",
                        rdr["Moody's"].ToString(), ",",
                        rdr["HRR"].ToString(), ",",
                        rdr["Intermediario"].ToString(), ",",
                        rdr["DescripcionIntermediario"].ToString(), ",",
                        rdr["ClasificadorIntermediario"].ToString(), ",",
                        rdr["TipoOperacion"].ToString(), ",",
                        rdr["Operacion"].ToString(), ",",
                        rdr["Emisor"].ToString(), ",",
                        rdr["OrigenEmisor"].ToString(), ",",
                        rdr["Sobretasa"].ToString(), ",",
                        rdr["VolatilidadImplicita"].ToString(), ",",
                        rdr["StatusIntrumento"].ToString(), ",",
                        rdr["IdentificadorIMSS"].ToString(), ",",
                        rdr["Mandatario"].ToString(), ",",
                        rdr["MontoEmitido"].ToString().Trim(), ",",
                        rdr["TitulosCirculacion"].ToString(), ",",
                        rdr["TitulosEmitidos"].ToString(), ",",
                        rdr["ValorNominal"].ToString(), ",",
                        rdr["SumaMontoInvertido_2"].ToString());

                    sw.WriteLine(linea);
                }
            }
            rdr.Close();


            con.Close();
        }

        public void ExpArcPosLayoutCSV(string rutaArchivo)
        {
            string encabezado = "";
            string detalle = "";
            string connectionString = (ConfigurationManager.ConnectionStrings["SAM_IMSS_Connection"].ConnectionString);
            SqlConnection con = new SqlConnection(connectionString);
            con.Open();

            detalle = string.Concat("exec IMSS_ArcPosLayout_csv '", Recursos.appfecha.ToString(), "'");
            SqlCommand cmd = new SqlCommand(detalle, con);
            cmd.CommandType = CommandType.Text;
            cmd.ExecuteNonQuery();


            encabezado = "Select DISTINCT 'H'as [H], 'MSANT' as [Mandato], FechaPosicion as [Fecha], 'MSANT' as [Mandatario], count(*) as [Registros]  from  IMSS_RepPosLayoutHist where datediff(DD,FechaReporte, getdate()) = 0  GROUP BY FechaPosicion";
            cmd = new SqlCommand(encabezado, con);
            cmd.CommandType = CommandType.Text;
            SqlDataReader rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                using (StreamWriter sw = File.CreateText(rutaArchivo.ToString()))
                {
                    string linea = string.Concat(rdr["H"].ToString(), ",", rdr["Mandato"].ToString(), ",", rdr["Fecha"].ToString(), ",", rdr["Mandatario"].ToString(), ",", rdr["Registros"].ToString());
                    sw.WriteLine(linea);
                }
            }
            rdr.Close();

            detalle = string.Concat("exec IMSS_ArcPosLayout_csv '", Recursos.appfecha.ToString(), "'");
            cmd = new SqlCommand(detalle, con);
            cmd.CommandType = CommandType.Text;
            rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                using (StreamWriter sw = File.AppendText(rutaArchivo.ToString()))
                {
                    string linea = string.Concat(rdr["ClaveOperacion"].ToString(), ",",
                        rdr["ClaveMandato"].ToString(), ",",
                        rdr["FechaPosicion"].ToString(), ",",
                        rdr["Portafolio"].ToString(), ",",
                        rdr["SubPortafolio"].ToString(), ",",
                        rdr["ClaseActivo"].ToString(), ",",
                        rdr["TipoValor"].ToString(), ",",
                        rdr["Emisora"].ToString(), ",",
                        rdr["Serie"].ToString(), ",",
                        rdr["SumaTitulosAcciones"].ToString(), ",",
                        rdr["DiasCupon"].ToString(), ",",
                        rdr["TasaCupon"].ToString(), ",",
                        rdr["DxVCupon"].ToString(), ",",
                        rdr["FechaInicialCupon"].ToString(), ",",
                        rdr["FechaFinalCupon"].ToString(), ",",
                        rdr["FechaEmisionInstrumento"].ToString(), ",",
                        rdr["FechaVencimientoOperacion"].ToString(), ",",
                        rdr["DiasPorVencerInstrumento"].ToString(), ",",
                        rdr["YTM"].ToString(), ",",
                        rdr["TasaPactada"].ToString(), ",",
                        rdr["Moneda"].ToString(), ",",
                        rdr["Subyacente"].ToString(), ",",
                        rdr["SumaMontoInvertido_1"].ToString(), ",",
                        rdr["TipoCambio"].ToString(), ",",
                        rdr["Sector"].ToString(), ",",
                        rdr["S&P"].ToString(), ",",
                        rdr["Fitch"].ToString(), ",",
                        rdr["Moody's"].ToString(), ",",
                        rdr["HRR"].ToString(), ",",
                        rdr["Intermediario"].ToString(), ",",
                        rdr["DescripcionIntermediario"].ToString(), ",",
                        rdr["ClasificadorIntermediario"].ToString(), ",",
                        rdr["TipoOperacion"].ToString(), ",",
                        rdr["Operacion"].ToString(), ",",
                        rdr["Emisor"].ToString(), ",",
                        rdr["OrigenEmisor"].ToString(), ",",
                        rdr["Sobretasa"].ToString(), ",",
                        rdr["VolatilidadImplicita"].ToString(), ",",
                        rdr["StatusIntrumento"].ToString(), ",",
                        rdr["IdentificadorIMSS"].ToString(), ",",
                        rdr["Mandatario"].ToString(), ",",
                        rdr["MontoEmitido"].ToString().Trim(), ",",
                        rdr["TitulosCirculacion"].ToString(), ",",
                        rdr["TitulosEmitidos"].ToString(), ",",
                        rdr["ValorNominal"].ToString(), ",",
                        rdr["SumaMontoInvertido_2"].ToString());

                    sw.WriteLine(linea);
                }
            }
            rdr.Close();


            con.Close();
        }

        public void ExpArcPosValuada(string rutaArchivo)
        {
            try
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial; // Necesario para EPPlus
                // Abre un archivo Excel con EPPlus (crear un nuevo archivo)
                FileInfo archivoExcel = new FileInfo(rutaArchivo);
                using (ExcelPackage paqueteExcel = new ExcelPackage(archivoExcel))
                {
                    // Verifica si ya existe una hoja con el nombre "PosValuada" y la elimina si es necesario
                    ExcelWorksheet hojaExcel = paqueteExcel.Workbook.Worksheets["PosValuada"];
                    if (hojaExcel != null)
                    {
                        paqueteExcel.Workbook.Worksheets.Delete("PosValuada");
                    }

                    // Crea una nueva hoja de cálculo
                    hojaExcel = paqueteExcel.Workbook.Worksheets.Add("PosValuada");

                    // Establecer encabezados
                    hojaExcel.Cells[1, 1].Value = "Tipo de Valor";
                    hojaExcel.Cells[1, 2].Value = "Emisora";
                    hojaExcel.Cells[1, 3].Value = "Serie";
                    hojaExcel.Cells[1, 4].Value = "Titulos";
                    hojaExcel.Cells[1, 5].Value = "Precio";
                    hojaExcel.Cells[1, 6].Value = "Monto Invertido";
                    hojaExcel.Cells[1, 7].Value = "Valor Mercado";

                    // Conexión a la base de datos
                    using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["SAM_IMSS_Connection"].ConnectionString))
                    {
                        // Realiza la consulta
                        string strqry = $"Exec IMSS_ArcPosicionValuada '{Recursos.appfecha.ToString()}'";
                        con.Open();
                        SqlCommand cmd = new SqlCommand(strqry, con);
                        cmd.CommandType = CommandType.Text;
                        SqlDataReader rdr = cmd.ExecuteReader();

                        int i = 2;  // Comienza en la segunda fila (debido a los encabezados)
                        while (rdr.Read())
                        {
                            // Asigna valores a cada celda
                            //hojaExcel.Cells[i, 1].Value = rdr["Tipo Valor"].ToString();
                            if (Decimal.TryParse(rdr["Tipo Valor"]?.ToString(), out Decimal tipoValor))
                            {
                                hojaExcel.Cells[i, 1].Value = tipoValor;
                            }
                            else
                            {
                                hojaExcel.Cells[i, 1].Value = rdr["Tipo Valor"].ToString();
                            }
                            
                            hojaExcel.Cells[i, 2].Value = rdr["Emisora"].ToString();

                            //hojaExcel.Cells[i, 3].Value = rdr["Serie"].ToString();
                            if (Decimal.TryParse(rdr["Serie"]?.ToString(), out Decimal serie))
                            {
                                hojaExcel.Cells[i, 3].Value = serie;
                            }
                            else
                            {
                                hojaExcel.Cells[i, 3].Value = rdr["Serie"].ToString();
                            }

                            //hojaExcel.Cells[i, 4].Value = rdr["Titulos"].ToString();
                            if (Decimal.TryParse(rdr["Titulos"]?.ToString(), out Decimal titulos))
                            {
                                hojaExcel.Cells[i, 4].Value = titulos;
                            }
                            else
                            {
                                hojaExcel.Cells[i, 4].Value = rdr["Titulos"].ToString();
                            }

                            //hojaExcel.Cells[i, 5].Value = rdr["Precio"].ToString();
                            if (Decimal.TryParse(rdr["Precio"]?.ToString(), out Decimal precio))
                            {
                                hojaExcel.Cells[i, 5].Value = precio;
                            }
                            else
                            {
                                hojaExcel.Cells[i, 5].Value = rdr["Precio"].ToString();
                            }

                            //hojaExcel.Cells[i, 6].Value = rdr["Monto Invertido"].ToString();
                            if (Decimal.TryParse(rdr["Monto Invertido"]?.ToString(), out Decimal montoinvertido))
                            {
                                hojaExcel.Cells[i, 6].Value = montoinvertido;
                            }
                            else
                            {
                                hojaExcel.Cells[i, 6].Value = rdr["Monto Invertido"].ToString();
                            }

                            //hojaExcel.Cells[i, 7].Value = rdr["Valor Mercado"].ToString();
                            if (Decimal.TryParse(rdr["Valor Mercado"]?.ToString(), out Decimal valormercado))
                            {
                                hojaExcel.Cells[i, 7].Value = valormercado;
                            }
                            else
                            {
                                hojaExcel.Cells[i, 7].Value = rdr["Valor Mercado"].ToString();
                            }
                            i++;
                        }
                        rdr.Close();
                    }

                    // Guardar el archivo Excel
                    paqueteExcel.Save();
                }
            }
            catch (Exception ex)
            {
                // Manejo de errores (se puede personalizar dependiendo de tus necesidades)
                string script = $"alert('Error: {ex.Message}');";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", script, true);
            }
            finally
            {
                // Alerta de éxito, si todo salió bien
                string script = "alert('Exportación de posiciones valuadas completa.');";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", script, true);
            }
        }

        public void ExpArcPosValuadaCSV(string rutaArchivo)
        {
            try
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial; // Necesario para EPPlus

                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["SAM_IMSS_Connection"].ConnectionString))
                {
                    string strqry = $"Exec IMSS_ArcPosicionValuada_csv '{Recursos.appfecha.ToString()}'";
                    con.Open();
                    SqlCommand cmd = new SqlCommand(strqry, con);
                    cmd.CommandType = CommandType.Text;
                    SqlDataReader rdr = cmd.ExecuteReader();

                    using (ExcelPackage excel = new ExcelPackage())
                    {
                        ExcelWorksheet worksheet = excel.Workbook.Worksheets.Add("Posición Valuada");

                        // Escribir encabezados
                        worksheet.Cells[1, 1].Value = "Tipo de Valor";
                        worksheet.Cells[1, 2].Value = "Emisora";
                        worksheet.Cells[1, 3].Value = "Serie";
                        worksheet.Cells[1, 4].Value = "Títulos";
                        worksheet.Cells[1, 5].Value = "Precio";
                        worksheet.Cells[1, 6].Value = "Monto Invertido";
                        worksheet.Cells[1, 7].Value = "Valor Mercado";

                        // Aplicar estilo a los encabezados
                        using (ExcelRange range = worksheet.Cells[1, 1, 1, 7])
                        {
                            range.Style.Font.Bold = true;
                            range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                            range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                        }

                        int row = 2;

                        // Escribir los datos en el archivo Excel
                        while (rdr.Read())
                        {
                            //worksheet.Cells[row, 1].Value = rdr["Tipo Valor"] == DBNull.Value ? string.Empty : rdr["Tipo Valor"].ToString();                            
                            if (Decimal.TryParse(rdr["Tipo Valor"]?.ToString(), out Decimal tipoValor))
                            {
                                worksheet.Cells[row, 1].Value = tipoValor;
                            }
                            else
                            {
                                worksheet.Cells[row, 1].Value = rdr["Tipo Valor"] == DBNull.Value ? string.Empty : rdr["Tipo Valor"].ToString();
                            }

                            worksheet.Cells[row, 2].Value = rdr["Emisora"] == DBNull.Value ? string.Empty : rdr["Emisora"].ToString();

                            //worksheet.Cells[row, 3].Value = rdr["Serie"] == DBNull.Value ? string.Empty : rdr["Serie"].ToString();
                            if (Decimal.TryParse(rdr["Serie"]?.ToString(), out Decimal serie))
                            {
                                worksheet.Cells[row, 3].Value = serie;
                            }
                            else
                            {
                                worksheet.Cells[row, 3].Value = rdr["Serie"] == DBNull.Value ? string.Empty : rdr["Serie"].ToString();
                            }

                            //worksheet.Cells[row, 4].Value = rdr["Titulos"] == DBNull.Value ? 0 : Convert.ToInt32(rdr["Titulos"]);
                            if (Decimal.TryParse(rdr["Titulos"]?.ToString(), out Decimal titulos))
                            {
                                worksheet.Cells[row, 4].Value = titulos;
                            }
                            else
                            {
                                worksheet.Cells[row, 4].Value = rdr["Titulos"] == DBNull.Value ? 0 : Convert.ToInt32(rdr["Titulos"]);
                            }

                            //worksheet.Cells[row, 5].Value = rdr["Precio"] == DBNull.Value ? 0.0 : Convert.ToDouble(rdr["Precio"]);
                            if (Decimal.TryParse(rdr["Precio"]?.ToString(), out Decimal precio))
                            {
                                worksheet.Cells[row, 5].Value = precio;
                            }
                            else
                            {
                                worksheet.Cells[row, 5].Value = rdr["Precio"] == DBNull.Value ? 0.0 : Convert.ToDouble(rdr["Precio"]);
                            }

                            //worksheet.Cells[row, 6].Value = rdr["Monto Invertido"] == DBNull.Value ? 0.0 : Convert.ToDouble(rdr["Monto Invertido"]);
                            if (Decimal.TryParse(rdr["Monto Invertido"]?.ToString(), out Decimal montoinvertido))
                            {
                                worksheet.Cells[row, 6].Value = montoinvertido;
                            }
                            else
                            {
                                worksheet.Cells[row, 6].Value = rdr["Monto Invertido"] == DBNull.Value ? 0.0 : Convert.ToDouble(rdr["Monto Invertido"]);
                            }

                            //worksheet.Cells[row, 7].Value = rdr["Valor Mercado"] == DBNull.Value ? 0.0 : Convert.ToDouble(rdr["Valor Mercado"]);
                            if (Decimal.TryParse(rdr["Valor Mercado"]?.ToString(), out Decimal valormercado))
                            {
                                worksheet.Cells[row, 7].Value = valormercado;
                            }
                            else
                            {
                                worksheet.Cells[row, 7].Value = rdr["Valor Mercado"] == DBNull.Value ? 0.0 : Convert.ToDouble(rdr["Valor Mercado"]);
                            }

                            row++;
                        }

                        rdr.Close();

                        // Ajustar el ancho de las columnas automáticamente
                        worksheet.Cells.AutoFitColumns();

                        // Guardar el archivo en la ruta especificada
                        FileInfo excelFile = new FileInfo(rutaArchivo);
                        excel.SaveAs(excelFile);
                    }
                }

                // Mensaje de éxito
                string script = "alert('Exportación a Excel completa.');";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", script, true);
            }
            catch (Exception ex)
            {
                // Manejo de errores
                string script = $"alert('Error: {ex.Message}');";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", script, true);
            }
        }

        public void ExpArcBBVA(string rutaArchivo)
        {
            string detalle = "";
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["SAM_IMSS_Connection"].ConnectionString);

            con.Open();

            detalle = string.Concat("exec IMSS_CustodioBBVA");
            SqlCommand cmd = new SqlCommand(detalle, con);
            cmd.CommandType = CommandType.Text;
            SqlDataReader rdr = cmd.ExecuteReader();

            while (rdr.Read())
            {

                using (StreamWriter sw = File.AppendText(rutaArchivo.ToString()))
                {
                    string linea = string.Concat(rdr["Texto"].ToString());

                    sw.WriteLine(linea);
                }
            }
            rdr.Close();

            con.Close();
        }

        public void ExpArcS3(string rutaArchivo)
        {

            string detalle = "";
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["SAM_IMSS_Connection"].ConnectionString);

            con.Open();

            detalle = string.Concat("exec IMSS_CustodioS3");
            SqlCommand cmd = new SqlCommand(detalle, con);
            cmd.CommandType = CommandType.Text;
            SqlDataReader rdr = cmd.ExecuteReader();

            while (rdr.Read())
            {

                using (StreamWriter sw = File.AppendText(rutaArchivo.ToString()))
                {
                    string linea = string.Concat(rdr["Texto"].ToString());

                    sw.WriteLine(linea);
                }
            }
            rdr.Close();


            con.Close();

        }

        public void ExpArcS3Comp(string rutaArchivo)
        {
            try
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial; // Necesario para EPPlus

                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["SAM_IMSS_Connection"].ConnectionString))
                {
                    string strqry = $"Exec IMSS_CustodioS3_Estrucutrado";
                    con.Open();
                    SqlCommand cmd = new SqlCommand(strqry, con);
                    cmd.CommandType = CommandType.Text;
                    SqlDataReader rdr = cmd.ExecuteReader();

                    using (ExcelPackage excel = new ExcelPackage())
                    {
                        ExcelWorksheet worksheet = excel.Workbook.Worksheets.Add("S3 Complementario");

                        //// Escribir encabezados
                        //worksheet.Cells[1, 1].Value = "Referencia";
                        //worksheet.Cells[1, 2].Value = "Tipo_OP";
                        //worksheet.Cells[1, 3].Value = "Titulo";
                        //worksheet.Cells[1, 4].Value = "ISIN";
                        //worksheet.Cells[1, 5].Value = "Emisora";
                        //worksheet.Cells[1, 6].Value = "Cuenta";
                        //worksheet.Cells[1, 7].Value = "Moneda";
                        //worksheet.Cells[1, 8].Value = "Activo";
                        //worksheet.Cells[1, 9].Value = "Cuenta";
                        //worksheet.Cells[1, 10].Value = "Fecha_Liquidacion";
                        //worksheet.Cells[1, 11].Value = "Monto";
                        //worksheet.Cells[1, 12].Value = "Contraparte";
                        //worksheet.Cells[1, 13].Value = "Plazo";
                        //worksheet.Cells[1, 14].Value = "Tasa";
                        //worksheet.Cells[1, 15].Value = "Space_One";
                        //worksheet.Cells[1, 16].Value = "Space_Two";
                        //worksheet.Cells[1, 17].Value = "Space_Three";

                        //// Aplicar estilo a los encabezados
                        //using (ExcelRange range = worksheet.Cells[1, 1, 1, 17])
                        //{
                        //    range.Style.Font.Bold = true;
                        //    range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                        //    range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                        //}

                        int row = 1;

                        // Escribir los datos en el archivo Excel
                        while (rdr.Read())
                        {
                            worksheet.Cells[row, 1].Value = rdr["Referencia"] == DBNull.Value ? string.Empty : rdr["Referencia"].ToString();
                            worksheet.Cells[row, 2].Value = rdr["Tipo_OP"] == DBNull.Value ? string.Empty : rdr["Tipo_OP"].ToString();
                            worksheet.Cells[row, 3].Value = rdr["Titulo"] == DBNull.Value ? (Decimal)0 : Convert.ToDecimal( rdr["Titulo"]);
                            worksheet.Cells[row, 4].Value = rdr["ISIN"] == DBNull.Value ? string.Empty : rdr["ISIN"].ToString();
                            worksheet.Cells[row, 5].Value = rdr["Emisora"] == DBNull.Value ? string.Empty : rdr["Emisora"].ToString();
                            worksheet.Cells[row, 6].Value = rdr["Cuenta"] == DBNull.Value ? (Decimal)0 : Convert.ToDecimal(rdr["Cuenta"]);
                            worksheet.Cells[row, 7].Value = rdr["Moneda"] == DBNull.Value ? string.Empty : rdr["Moneda"].ToString();
                            worksheet.Cells[row, 8].Value = rdr["Activo"] == DBNull.Value ? string.Empty : rdr["Activo"].ToString();
                            worksheet.Cells[row, 9].Value = rdr["Cuenta"] == DBNull.Value ? (Decimal)0 : Convert.ToDecimal(rdr["Cuenta"]);
                            
                            if (DateTime.TryParse(rdr["Fecha_Liquidacion"]?.ToString(), out DateTime fechaLiquidacion))
                            {

                                DateTime fecha = new DateTime(int.Parse(rdr["Fecha_Liquidacion"].ToString().Substring(6,4)), int.Parse(rdr["Fecha_Liquidacion"].ToString().Substring(3, 2)), int.Parse(rdr["Fecha_Liquidacion"].ToString().Substring(0,2)));
                                worksheet.Cells[row, 10].Value = fecha;
                                worksheet.Cells[row, 10].Style.Numberformat.Format = "dd-MM-yyyy";
                            }
                            else
                            {
                                worksheet.Cells[row, 10].Value = rdr["Fecha_Liquidacion"] == DBNull.Value ? string.Empty : rdr["Fecha_Liquidacion"].ToString();
                            }

                            worksheet.Cells[row, 11].Value = rdr["Monto"] == DBNull.Value ? (Decimal)0 : Convert.ToDecimal( rdr["Monto"]);
                            worksheet.Cells[row, 12].Value = rdr["Contraparte"] == DBNull.Value ? string.Empty : rdr["Contraparte"].ToString();  
                            
                            if (decimal.TryParse(rdr["Plazo"]?.ToString(), out decimal numberPlazo))
                            {
                                worksheet.Cells[row, 13].Value = rdr["Plazo"] == DBNull.Value ? (Decimal)0 : Convert.ToDecimal(rdr["Plazo"]);
                            }
                            else
                            {
                                worksheet.Cells[row, 13].Value = rdr["Plazo"] == DBNull.Value ? string.Empty : rdr["Plazo"].ToString();
                            }
                            if (decimal.TryParse(rdr["Tasa"]?.ToString(), out decimal numberTasa))
                            {
                                worksheet.Cells[row, 14].Value = rdr["Tasa"] == DBNull.Value ? (Decimal)0 : Convert.ToDecimal(rdr["Tasa"]);
                            }
                            else
                            {
                                worksheet.Cells[row, 14].Value = rdr["Tasa"] == DBNull.Value ? string.Empty : rdr["Tasa"].ToString();
                            }
                            
                            worksheet.Cells[row, 15].Value = rdr["Space_One"] == DBNull.Value ? string.Empty : (rdr["Space_One"]);
                            worksheet.Cells[row, 16].Value = rdr["Space_Two"] == DBNull.Value ? string.Empty : (rdr["Space_Two"]);
                            worksheet.Cells[row, 17].Value = rdr["Space_Three"] == DBNull.Value ? string.Empty : (rdr["Space_Three"]);

                            row++;
                        }

                        rdr.Close();

                        // Ajustar el ancho de las columnas automáticamente
                        worksheet.Cells.AutoFitColumns();

                        // Guardar el archivo en la ruta especificada
                        FileInfo excelFile = new FileInfo(rutaArchivo);
                        excel.SaveAs(excelFile);
                    }
                }

                // Mensaje de éxito
                string script = "alert('Exportación a Excel completa.');";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", script, true);
            }
            catch (Exception ex)
            {
                // Manejo de errores
                string script = $"alert('Error: {ex.Message}');";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", script, true);
            }
        }

        protected void btnSelTrades_Click(object sender, EventArgs e)
        {
            //OpenFileDialog fldArchivo = new OpenFileDialog( );

            //fldArchivo.InitialDirectory = Environment.GetFolderPath( Environment.SpecialFolder.Personal );
            //fldArchivo.Filter = "Archivos XLSX(*.xlsx)|*.xlsx";

            // codigo para abrir el cuadro de dialogo
            //if(fldArchivo.ShowDialog( ) == DialogResult.OK)
            //{
            //    try
            //    {
            //        string str_RutaArchivo = fldArchivo.FileName;
            //        txArchTradesCustodio.Text = str_RutaArchivo;
            //    } catch(Exception)
            //    {
            //        throw;
            //    }
            //}
        }


        protected void btnCargarArchivo_Click(object sender, EventArgs e)
        {
            if (fuArchivoExcelMDValmer.HasFile)
            {
                // Obtener el nombre del archivo
                string fileName = fuArchivoExcelMDValmer.FileName;

                // Ruta donde se guardará el archivo en el servidor
                string rutaDestino = Server.MapPath("~/ArchivosSalida/" + fileName);

                // Guarda el archivo en el servidor
                fuArchivoExcelMDValmer.SaveAs(rutaDestino);

                // Muestra la ruta completa en el TextBox
                //txtRutaArchivo.Text = rutaDestino;

                // Cargar el archivo Excel en un DataTable
                DataTable dt = CargarExcelEnDataTable(rutaDestino);

                // Si el DataTable contiene datos, los mostramos en el GridView
                if (dt != null && dt.Rows.Count > 0)
                {
                    //gvDatosExcel.DataSource = dt;
                    //gvDatosExcel.DataBind( );
                }
                else
                {
                    // Si no hay datos, mostramos un mensaje
                    Response.Write("El archivo no contiene datos válidos.");
                }
            }
            else
            {
                // Si no se selecciona ningún archivo, mostramos un mensaje
                Response.Write("Por favor, selecciona un archivo primero.");
            }
        }

        private DataTable CargarExcelEnDataTable(string rutaArchivo)
        {
            DataTable dt = new DataTable();

            // Abre el archivo Excel con EPPlus
            using (ExcelPackage package = new ExcelPackage(new FileInfo(rutaArchivo)))
            {
                // Obtiene la primera hoja (puedes ajustar esto si tienes varias hojas)
                ExcelWorksheet worksheet = package.Workbook.Worksheets[0];

                // Verifica si hay filas y columnas en el archivo Excel
                if (worksheet.Dimension == null)
                    return null;

                // Crea las columnas del DataTable basadas en los encabezados del archivo Excel
                foreach (var firstRowCell in worksheet.Cells[1, 1, 1, worksheet.Dimension.End.Column])
                {
                    dt.Columns.Add(firstRowCell.Text); // Añade las cabeceras al DataTable
                }

                // Agrega los datos de cada fila al DataTable
                for (int rowNum = 2; rowNum <= worksheet.Dimension.End.Row; rowNum++) // Comienza en la fila 2 para evitar los encabezados
                {
                    DataRow row = dt.NewRow();
                    for (int colNum = 1; colNum <= worksheet.Dimension.End.Column; colNum++)
                    {
                        row[colNum - 1] = worksheet.Cells[rowNum, colNum].Text; // Añade el contenido de la celda al DataRow
                    }
                    dt.Rows.Add(row);
                }
            }

            return dt;
        }

        protected void btSelArchivoPiP_Click(object sender, EventArgs e)
        {
            //OpenFileDialog fldArchivo = new OpenFileDialog( );

            //fldArchivo.InitialDirectory = Environment.GetFolderPath( Environment.SpecialFolder.Personal );
            //fldArchivo.Filter = "Archivos XLS(*.xls)|*.xls";

            //// codigo para abrir el cuadro de dialogo
            //if(fldArchivo.ShowDialog( ) == DialogResult.OK)
            //{
            //    try
            //    {
            //        string str_RutaArchivo = fldArchivo.FileName;
            //        txFilePiP.Text = str_RutaArchivo;
            //    } catch(Exception)
            //    {
            //        throw;
            //    }
            //}
        }

        protected void btRutaPosition_Click(object sender, EventArgs e)
        {
            ////OpenFileDialog fldArchivo = new OpenFileDialog( );

            //fldArchivo.InitialDirectory = Environment.GetFolderPath( Environment.SpecialFolder.Personal );
            //fldArchivo.Filter = "Archivos XLSX(*.xlsx)|*.xlsx";

            //// codigo para abrir el cuadro de dialogo
            //if(fldArchivo.ShowDialog( ) == DialogResult.OK)
            //{
            //    try
            //    {
            //        string str_RutaArchivo = fldArchivo.FileName;
            //        txArchPosition.Text = str_RutaArchivo;
            //    } catch(Exception)
            //    {
            //        throw;
            //    }
            //}
        }


        /// <summary>
        ///  Cuando se hace el cambio de Check para procesar tipos de archivo CSV y se limpia el FileUpLoad para el archivo Valmer (JAMM110580)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void chkCSV_CheckedChanged(object sender, EventArgs e)
        {
            //Deselcciona chkXLS, porque ambos Check no pueden estar seleccionados.
            if (chkCSV.Checked)
            {
                chkXLS.Checked = false;
            }

            //Resetea el valor de asp:FileUpload ID="fuArchivoExcelMDValmer"
            fuArchivoExcelMDValmer.Attributes.Clear();
        }

        /// <summary>
        ///  Cuando se hace el cambio de Check para procesar tipos de archivo XLSX y se limpia el FileUpLoad para el archivo Valmer (JAMM110580)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void chkXLS_CheckedChanged(object sender, EventArgs e)
        {
            //Deselcciona chkCSV, porque ambos Check no pueden estar seleccionados.
            if (chkXLS.Checked)
            {
                chkCSV.Checked = false;
            }

            //Resetea el valor de asp:FileUpload ID="fuArchivoExcelMDValmer"
            fuArchivoExcelMDValmer.Attributes.Clear();
        }

        protected void btnSelCarpetaSalida_Click(object sender, EventArgs e)
        {
            //var fbd = new FolderBrowserDialog( );

            //fbd.SelectedPath = Recursos.strRutaLayouts.ToString( ).Trim( );
            //DialogResult result = fbd.ShowDialog( );

            //if(result == DialogResult.OK && !string.IsNullOrWhiteSpace( fbd.SelectedPath ))
            //{
            //    txLayouts.Text = string.Concat( fbd.SelectedPath, @"\" );
            //}


            //Resetea el valor de asp:FileUpload ID="fuArchivoExcelMDValmer"
            fuArchivoExcelMDValmer.Attributes.Clear();
        }


    }
}