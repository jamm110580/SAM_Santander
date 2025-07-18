using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web.UI.WebControls;
using OfficeOpenXml;
using System.Web.UI;

public partial class _Default : System.Web.UI.Page
{
    private const string KEY_ARCHIVOS = "ListaArchivos";
    
    protected void Page_Load( object sender, EventArgs e )
    {
        if(!IsPostBack)
        {
            ViewState[ KEY_ARCHIVOS ] = new List<ArchivoTemporal>( );

        }
    }

    /// <summary>
    /// Muestra los archivos cargados en el Repeater y habilita el botón de confirmación si hay archivos.
    /// </summary>
    /// <param name="archivos">Lista de archivos temporales a mostrar.</param>
    private void MostrarArchivos( List<ArchivoTemporal> archivos )
    {
        rptArchivos.DataSource = archivos;
        rptArchivos.DataBind( );
        btnConfirmarCarga.Visible = archivos.Any( );
        upArchivosPreview.Update( ); // Refresca la UI visualmente
    }

    private void AgregarArchivos( string tipo )
    {
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

        string basePath = ConfigurationManager.AppSettings[ "ExcelPath" + tipo ];

        if(!Directory.Exists( basePath ))
        {
            lblResultados.Text = "No se encontró la carpeta: " + basePath;
            pnlResultados.Visible = true;
            return;
        }

        var archivos = Directory.GetFiles( basePath, "*.*" )
            .Where( f => f.EndsWith( ".xlsx", StringComparison.OrdinalIgnoreCase ) ||
                        f.EndsWith( ".csv", StringComparison.OrdinalIgnoreCase ) )
            .ToArray( );

        var lista = new List<ArchivoTemporal>( );

        foreach(var archivo in archivos)
        {
            int registros = 0;

            if(archivo.EndsWith( ".xlsx", StringComparison.OrdinalIgnoreCase ))
            {
                using(var package = new ExcelPackage( new FileInfo( archivo ) ))
                {
                    var hoja = package.Workbook.Worksheets[ 0 ];
                    registros = hoja.Dimension?.End.Row - 1 ?? 0;
                }
            } else if(archivo.EndsWith( ".csv", StringComparison.OrdinalIgnoreCase ))
            {
                // Contar líneas y restar encabezado
                var lineas = File.ReadAllLines( archivo );
                registros = lineas.Length > 1 ? lineas.Length - 1 : 0;
            }

            lista.Add( new ArchivoTemporal
            {
                Nombre = Path.GetFileName( archivo ),
                Ruta = archivo,
                Fuente = tipo,
                Registros = registros,
                Progreso = 0
            } );
        }

        ViewState[ KEY_ARCHIVOS ] = lista;
        MostrarArchivos( lista );
    }


    protected void btnProcessPosicionAladdin_Click( object sender, EventArgs e )
    {
        AgregarArchivos( "Aladdin" );
    }

    protected void btnProcessPosicionAladdin_Reporto_Click( object sender, EventArgs e )
    {
        AgregarArchivos( "AladdinReporto" );
    }

    

    protected void btnProcessVectAnali_Click( object sender, EventArgs e )
    {
        AgregarArchivos( "VectAnali" );
    }

    /// <summary>
    /// Activa el Timer para comenzar el procesamiento progresivo de archivos.
    /// Ya no procesa directamente todos los archivos aquí.
    /// </summary>
    protected void btnConfirmarCarga_Click( object sender, EventArgs e )
    {
        var lista = ViewState[ KEY_ARCHIVOS ] as List<ArchivoTemporal>;
        if(lista == null || !lista.Any( ))
        {
            pnlResultados.Visible = true;
            lblResultados.Text = "No hay archivos para procesar.";
            return;
        }

        // Mostrar mensaje inicial
        lblArchivoEnProceso.Text = "Iniciando procesamiento de archivos...";
        pnlResultados.Visible = false;

        // Activar Timer para procesar los archivos uno por uno
        tmrProcesoArchivos.Enabled = true;
    }


    /// <summary>
    /// Procesa el siguiente archivo pendiente en la lista, uno por uno.
    /// Se actualiza el progreso y la UI después de cada archivo.
    /// Cuando todos los archivos han sido procesados, desactiva el Timer.
    /// </summary>
    private void ProcesarSiguienteArchivo( )
    {
        var lista = ViewState[ KEY_ARCHIVOS ] as List<ArchivoTemporal>;

        if(lista == null || lista.All( a => a.Progreso >= 100 ))
        {
            lblArchivoEnProceso.Text = "Todos los archivos fueron procesados.";
            pnlResultados.Visible = true;
            lblResultados.Text = "Carga finalizada correctamente.";
            MostrarArchivos( lista ?? new List<ArchivoTemporal>( ) );
            upArchivosPreview.Update( );

            // Desactivar Timer
            tmrProcesoArchivos.Enabled = false;
            return;
        }

        // Encuentra el siguiente archivo pendiente
        var archivo = lista.FirstOrDefault( a => a.Progreso < 100 );
        if(archivo == null)
        {
            lblArchivoEnProceso.Text = "Todos los archivos fueron procesados.";
            pnlResultados.Visible = true;
            lblResultados.Text = "Carga finalizada correctamente.";
            MostrarArchivos( lista );
            upArchivosPreview.Update( );
            tmrProcesoArchivos.Enabled = false;
            return;
        }

        lblArchivoEnProceso.Text = $"Procesando archivo: {archivo.Nombre}";

        try
        {
            switch(archivo.Fuente)
            {
                case "Aladdin":
                CargarPosicionAladdin( archivo );
                break;
                case "AladdinReporto":
                CargarPosicionAladdinReporto( archivo );
                break;
                case "ValBsmx":
                CargarValuacionesBSMX( archivo );
                break;
                case "ValS3":
                CargarValuacionesS3( archivo );
                break;
                case "VectAnali":
                CargarVectoresAnaliticos( archivo );
                break;
            }

            archivo.Progreso = 100;

            // Actualizar lista
            lista.RemoveAll( a => a.Nombre == archivo.Nombre );
            lista.Add( archivo );
            ViewState[ KEY_ARCHIVOS ] = lista;

            MostrarArchivos( lista );
            upArchivosPreview.Update( );
        } catch(Exception ex)
        {
            new LogFileError( ).WriteError( $"Error al procesar archivo {archivo.Nombre}: {ex.Message}" );
        }
    }

    protected void tmrProcesoArchivos_Tick( object sender, EventArgs e )
    {
        ProcesarSiguienteArchivo( ); // Procesa un archivo por tick
    }


    /// <summary>
    /// Carga los datos del archivo Aladdin a la base de datos.
    /// Cada fila se inserta individualmente. Los errores por fila se loguean.
    /// </summary>
    /// <param name="archivo">Objeto ArchivoTemporal con ruta y metadatos.</param>
    private void CargarPosicionAladdin( ArchivoTemporal archivo )
    {
        string connectionString = ConfigurationManager.ConnectionStrings[ "MyDatabaseConnection" ].ConnectionString;
        LogFileError logFile = new LogFileError( );
        int registrosTotalesInsertados = 0;
        int registrosTotalesActualizados = 0;

        try
        {
            Excel reader = new Excel( );
            DataTable dataTable = reader.ReadExcelFileAladdin( archivo.Ruta );
            int totalFilas = dataTable.Rows.Count;
            archivo.Registros = totalFilas;

            int currentRow = 0;
            foreach(DataRow row in dataTable.Rows)
            {
                currentRow++;
                try
                {
                    SqlParameter[ ] insertParams = new SqlParameter[ 13 ];
                    insertParams[ 0 ] = new SqlParameter( "@Buy_Sell", SqlDbType.VarChar, 50 ) { Value = row[ 0 ] ?? DBNull.Value };
                    insertParams[ 1 ] = new SqlParameter( "@Portfolio", SqlDbType.VarChar, 50 ) { Value = row[ 1 ] ?? DBNull.Value };
                    insertParams[ 2 ] = new SqlParameter( "@Tipo_Valor_Mexico", SqlDbType.VarChar, 50 ) { Value = row[ 2 ] ?? DBNull.Value };

                    decimal origFace;
                    insertParams[ 3 ] = new SqlParameter( "@Orig_Face", SqlDbType.VarChar, 50 )
                    {
                        Value = decimal.TryParse( row[ 3 ]?.ToString( ), out origFace ) ? (object) origFace : 0
                    };

                    insertParams[ 4 ] = new SqlParameter( "@ISIN", SqlDbType.VarChar, 50 ) { Value = row[ 4 ] ?? DBNull.Value };
                    insertParams[ 5 ] = new SqlParameter( "@CUSIP_Aladdin_ID", SqlDbType.VarChar, 50 ) { Value = row[ 5 ] ?? DBNull.Value };

                    DateTime posDate;
                    insertParams[ 6 ] = new SqlParameter( "@Pos_Date", SqlDbType.DateTime )
                    {
                        Value = DateTime.TryParse( row[ 6 ]?.ToString( ), out posDate ) ? (object) posDate : DBNull.Value
                    };

                    insertParams[ 7 ] = new SqlParameter( "@Settled", SqlDbType.VarChar, 50 ) { Value = row[ 7 ] ?? DBNull.Value };

                    decimal unsettled;
                    insertParams[ 8 ] = new SqlParameter( "@Unsettled", SqlDbType.VarChar, 50 )
                    {
                        Value = decimal.TryParse( row[ 8 ]?.ToString( ), out unsettled ) ? (object) unsettled : 0
                    };

                    insertParams[ 9 ] = new SqlParameter( "@Sec_Group", SqlDbType.VarChar, 50 ) { Value = row[ 9 ] ?? DBNull.Value };
                    insertParams[ 10 ] = new SqlParameter( "@Sec_Type", SqlDbType.VarChar, 50 ) { Value = row[ 10 ] ?? DBNull.Value };
                    insertParams[ 11 ] = new SqlParameter( "@Maturity", SqlDbType.VarChar, 50 ) { Value = row[ 11 ]?.ToString( ) ?? "" };
                    insertParams[ 12 ] = new SqlParameter( "@Activo", SqlDbType.VarChar, 50 ) { Value = 1 };

                    DatabaseHelper dbHelper = new DatabaseHelper( connectionString );
                    int rowsInserted = dbHelper.InsertData( "sp_InsertPosicionAladdin", insertParams );

                    if(rowsInserted > 0)
                        registrosTotalesInsertados++;
                    else
                    {
                        registrosTotalesActualizados++;
                        string rowValues = string.Join( ", ", row.ItemArray.Select( item => item?.ToString( ) ) );
                        logFile.WriteError( $"Se actualizó al insertar registro. Valores: {rowValues}" );
                    }
                } catch(Exception exRow)
                {
                    string rowValues = string.Join( ", ", row.ItemArray.Select( item => item?.ToString( ) ) );
                    logFile.WriteError( $"Error al insertar registro: {exRow.Message}. Valores: {rowValues}" );
                }

                // Actualizar progreso por porcentaje real
                archivo.Progreso = (int) ((currentRow / (float) totalFilas) * 100);
                lblArchivoEnProceso.Text = $"Procesando archivo: {archivo.Nombre} ({archivo.Progreso}%)";

                MostrarArchivos( ViewState[ KEY_ARCHIVOS ] as List<ArchivoTemporal> );
                upArchivosPreview.Update( );
            }

            ViewState[ "RegistrosInsertadosAladdin" ] = registrosTotalesInsertados;
            ViewState[ "RegistrosActualizadosAladdin" ] = registrosTotalesActualizados;
        } catch(Exception ex)
        {
            logFile.WriteError( "Error general al procesar archivo Aladdin: " + ex.Message );
        }
    }


    /// <summary>
    /// Carga los datos del archivo Aladdin Reporto a la base de datos.
    /// Inserta fila por fila, con manejo de errores y actualizaciones visuales.
    /// </summary>
    /// <param name="archivo">ArchivoTemporal que contiene ruta, nombre, fuente y progreso</param>
    private void CargarPosicionAladdinReporto( ArchivoTemporal archivo )
    {
        string connectionString = ConfigurationManager.ConnectionStrings[ "MyDatabaseConnection" ].ConnectionString;
        LogFileError logFile = new LogFileError( );
        int registrosTotalesInsertados = 0;
        int registrosTotalesActualizados = 0;

        try
        {
            Excel reader = new Excel( );
            DataTable dataTable = reader.ReadExcelFileAladdin( archivo.Ruta ); // Usa el método que ya tienes

            int totalFilas = dataTable.Rows.Count;
            archivo.Registros = totalFilas;

            int currentRow = 0;

            foreach(DataRow row in dataTable.Rows)
            {
                currentRow++;
                try
                {
                    SqlParameter[ ] insertParams = new SqlParameter[ 15 ];

                    insertParams[ 0 ] = new SqlParameter( "@Td_Num", SqlDbType.VarChar, 50 ) { Value = row[ 0 ] ?? DBNull.Value };
                    insertParams[ 1 ] = new SqlParameter( "@Trader", SqlDbType.VarChar, 50 ) { Value = row[ 1 ] ?? DBNull.Value };
                    insertParams[ 2 ] = new SqlParameter( "@Counterparty", SqlDbType.VarChar, 50 ) { Value = row[ 2 ] ?? DBNull.Value };
                    insertParams[ 3 ] = new SqlParameter( "@Portfolio", SqlDbType.VarChar, 50 ) { Value = row[ 3 ] ?? DBNull.Value };
                    insertParams[ 4 ] = new SqlParameter( "@Tipo_Valor_Mexico", SqlDbType.VarChar, 50 ) { Value = row[ 4 ] ?? DBNull.Value };

                    insertParams[ 5 ] = new SqlParameter( "@Orig_Face", SqlDbType.Decimal )
                    {
                        Value = decimal.TryParse( row[ 5 ]?.ToString( )?.Replace( ",", "" ), out decimal valor ) ? (object) valor : 0
                    };

                    insertParams[ 6 ] = new SqlParameter( "@Tran_Type", SqlDbType.VarChar ) { Value = row[ 6 ] ?? DBNull.Value };
                    insertParams[ 7 ] = new SqlParameter( "@CUSIP_Aladdin_ID", SqlDbType.VarChar, 50 ) { Value = row[ 7 ] ?? DBNull.Value };

                    insertParams[ 8 ] = new SqlParameter( "@Trade_Price", SqlDbType.Decimal )
                    {
                        Value = decimal.TryParse( row[ 8 ]?.ToString( )?.Replace( ",", "" ), out decimal precio ) ? (object) precio : 0
                    };

                    DateTime tradeDate;
                    insertParams[ 9 ] = new SqlParameter( "@Trade_Date", SqlDbType.DateTime )
                    {
                        Value = DateTime.TryParse( row[ 9 ]?.ToString( ), out tradeDate ) ? (object) tradeDate : DBNull.Value
                    };

                    DateTime posDate;
                    insertParams[ 10 ] = new SqlParameter( "@Pos_Date", SqlDbType.DateTime )
                    {
                        Value = DateTime.TryParse( row[ 10 ]?.ToString( ), out posDate ) ? (object) posDate : DBNull.Value
                    };

                    insertParams[ 11 ] = new SqlParameter( "@Effective_Rate", SqlDbType.VarChar, 50 ) { Value = row[ 11 ] ?? "" };
                    insertParams[ 12 ] = new SqlParameter( "@Net_Money", SqlDbType.VarChar, 50 ) { Value = row[ 12 ] ?? "" };
                    insertParams[ 13 ] = new SqlParameter( "@ISIN", SqlDbType.VarChar, 50 ) { Value = row[ 13 ] ?? "" };
                    insertParams[ 14 ] = new SqlParameter( "@Activo", SqlDbType.VarChar, 50 ) { Value = 1 };

                    DatabaseHelper dbHelper = new DatabaseHelper( connectionString );
                    int rowsInserted = dbHelper.InsertData( "sp_InsertPosicionAladdinReporto", insertParams );

                    if(rowsInserted > 0)
                    {
                        registrosTotalesInsertados++;
                    } else
                    {
                        registrosTotalesActualizados++;
                        string rowValues = string.Join( ", ", row.ItemArray.Select( item => item?.ToString( ) ) );
                        logFile.WriteError( $"Se actualizó al insertar registro. Valores: {rowValues}" );
                    }
                } catch(Exception exRow)
                {
                    string rowValues = string.Join( ", ", row.ItemArray.Select( item => item?.ToString( ) ) );
                    logFile.WriteError( $"Error al insertar registro: {exRow.Message}. Valores: {rowValues}" );
                }

                // Actualizar progreso por porcentaje real
                archivo.Progreso = (int) ((currentRow / (float) totalFilas) * 100);
                lblArchivoEnProceso.Text = $"Procesando archivo: {archivo.Nombre} ({archivo.Progreso}%)";

                MostrarArchivos( ViewState[ KEY_ARCHIVOS ] as List<ArchivoTemporal> );
                upArchivosPreview.Update( );
            }

            ViewState[ "RegistrosInsertadosAladdinReporto" ] = registrosTotalesInsertados;
            ViewState[ "RegistrosActualizadosAladdinReporto" ] = registrosTotalesActualizados;
        } catch(Exception ex)
        {
            logFile.WriteError( "Error general al procesar archivo Excel Aladdin Reporto: " + ex.Message );
        }
    }



    /// <summary>
    /// METODO QUE CARGA EL ARCHIVO DE VALUACIONES BSMX
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    /// <summary>
    /// Carga los datos del archivo de Valuaciones BSMX en la base de datos.
    /// </summary>
    /// <param name="archivo">ArchivoTemporal con ruta, nombre, fuente y progreso</param>
    private void CargarValuacionesBSMX( ArchivoTemporal archivo )
    {
        string connectionString = ConfigurationManager.ConnectionStrings[ "MyDatabaseConnection" ].ConnectionString;
        LogFileError LogfileError = new LogFileError( );
        int registrosTotalesInsertados = 0;
        int registrosTotalesActualizados = 0;

        try
        {
            Excel reader = new Excel( );
            DataTable dataTable = reader.ReadExcelFileAladdin( archivo.Ruta ); // Método existente

            int totalFilas = dataTable.Rows.Count;
            archivo.Registros = totalFilas;

            int currentRow = 0;

            foreach(DataRow row in dataTable.Rows)
            {
                currentRow++;
                try
                {
                    SqlParameter[ ] insertParams = new SqlParameter[ 15 ];
                    insertParams[ 0 ] = new SqlParameter( "@Column1", SqlDbType.SmallDateTime ) { Value = DateTime.TryParseExact( row[ "Source.Name" ]?.ToString( ), new[ ] { "MM/dd/yyyy", "M/d/yyyy" }, CultureInfo.InvariantCulture, DateTimeStyles.None, out var fecha ) ? fecha : throw new FormatException( "Fecha no válida" ) };
                    insertParams[ 1 ] = new SqlParameter( "@Column2", SqlDbType.VarChar ) { Value = row[ "Column2" ].ToString( ) };
                    insertParams[ 2 ] = new SqlParameter( "@ALADDIN", SqlDbType.VarChar ) { Value = row[ "ALADDIN" ].ToString( ) };
                    insertParams[ 3 ] = new SqlParameter( "@Column3", SqlDbType.VarChar ) { Value = row[ "Column3" ].ToString( ) };
                    insertParams[ 4 ] = new SqlParameter( "@Column4", SqlDbType.VarChar ) { Value = row[ "Column4" ].ToString( ) };
                    insertParams[ 5 ] = new SqlParameter( "@Column5", SqlDbType.VarChar ) { Value = row[ "Column5" ].ToString( ) };
                    insertParams[ 6 ] = new SqlParameter( "@Column6", SqlDbType.Decimal ) { Value = string.IsNullOrEmpty( row[ "Column6" ].ToString( ) ) ? 0 : Convert.ToDecimal( row[ "Column6" ], CultureInfo.InvariantCulture ) };
                    insertParams[ 7 ] = new SqlParameter( "@Column7", SqlDbType.VarChar ) { Value = row[ "Column7" ].ToString( ) };
                    insertParams[ 8 ] = new SqlParameter( "@Precio_Sucio", SqlDbType.Decimal ) { Value = string.IsNullOrEmpty( row[ "Column8" ].ToString( ) ) ? 0 : Convert.ToDecimal( row[ "Column8" ], CultureInfo.InvariantCulture ) };
                    insertParams[ 9 ] = new SqlParameter( "@Column9", SqlDbType.VarChar ) { Value = row[ "Column9" ].ToString( ) };
                    insertParams[ 10 ] = new SqlParameter( "@Column10", SqlDbType.Decimal ) { Value = string.IsNullOrEmpty( row[ "Column10" ].ToString( ) ) ? 0 : Convert.ToDecimal( row[ "Column10" ], CultureInfo.InvariantCulture ) };
                    insertParams[ 11 ] = new SqlParameter( "@Column11", SqlDbType.VarChar ) { Value = row[ "Column11" ].ToString( ) };
                    insertParams[ 12 ] = new SqlParameter( "@Column12", SqlDbType.VarChar ) { Value = row[ "Column12" ].ToString( ) };
                    insertParams[ 13 ] = new SqlParameter( "@Column13", SqlDbType.Decimal ) { Value = string.IsNullOrEmpty( row[ "Column13" ].ToString( ) ) ? 0 : Convert.ToDecimal( row[ "Column13" ], CultureInfo.InvariantCulture ) };
                    insertParams[ 14 ] = new SqlParameter( "@Activo", SqlDbType.Bit ) { Value = true };

                    DatabaseHelper dbHelper = new DatabaseHelper( connectionString );
                    int rowsInserted = dbHelper.InsertData( "Insert_OpeValuacionesBsmx", insertParams );

                    if(rowsInserted > 0)
                    {
                        registrosTotalesInsertados++;
                    } else
                    {
                        registrosTotalesActualizados++;
                        string rowValues = string.Join( ", ", row.ItemArray.Select( item => item?.ToString( ) ) );
                        LogfileError.WriteError( $"Se actualizó al insertar registro. Valores: {rowValues}" );
                    }
                } catch(Exception exRow)
                {
                    string rowValues = string.Join( ", ", row.ItemArray.Select( item => item?.ToString( ) ) );
                    LogfileError.WriteError( $"Error al insertar registro: {exRow.Message}. Valores: {rowValues}" );
                }

                // Progreso y UI
                archivo.Progreso = (int) ((currentRow / (float) totalFilas) * 100);
                lblArchivoEnProceso.Text = $"Procesando archivo: {archivo.Nombre} ({archivo.Progreso}%)";

                var lista = ViewState[ KEY_ARCHIVOS ] as List<ArchivoTemporal>;
                lista.RemoveAll( a => a.Nombre == archivo.Nombre );
                lista.Add( archivo );
                ViewState[ KEY_ARCHIVOS ] = lista;

                MostrarArchivos( lista );
                upArchivosPreview.Update( );
            }

            ViewState[ "RegistrosInsertadosValBSMX" ] = registrosTotalesInsertados;
            ViewState[ "RegistrosActualizadosValBSMX" ] = registrosTotalesActualizados;
        } catch(Exception ex)
        {
            LogfileError.WriteError( "Error general al procesar archivo Excel Valuaciones BSMX: " + ex.Message );
        }
    }




    /// <summary>
    /// Carga los datos del archivo de Valuaciones S3 en la base de datos.
    /// </summary>
    /// <param name="archivo">ArchivoTemporal con ruta, nombre, fuente y progreso</param>
    private void CargarValuacionesS3( ArchivoTemporal archivo )
    {
        string connectionString = ConfigurationManager.ConnectionStrings[ "MyDatabaseConnection" ].ConnectionString;
        LogFileError LogfileError = new LogFileError( );
        int registrosTotalesInsertados = 0;
        int registrosTotalesActualizados = 0;

        try
        {
            Excel reader = new Excel( );
            DataTable dataTable = reader.ReadExcelFileAladdin( archivo.Ruta ); // Usa el método común

            int totalFilas = dataTable.Rows.Count;
            archivo.Registros = totalFilas;

            int currentRow = 0;

            foreach(DataRow row in dataTable.Rows)
            {
                currentRow++;
                try
                {
                    SqlParameter[ ] insertParams = new SqlParameter[ 18 ];
                    // Agregar los parámetros uno por uno
                    insertParams[ 0 ] = new SqlParameter( "@Val_effdate", SqlDbType.SmallDateTime ) { Value = DateTime.TryParseExact( row[ "Val_effdate" ]?.ToString( ), new[ ] { "MM/dd/yyyy", "M/d/yyyy" }, CultureInfo.InvariantCulture, DateTimeStyles.None, out var fecha ) ? fecha : throw new FormatException( "Fecha no válida" ) };
                    insertParams[ 1 ] = new SqlParameter( "@Val_accountno", SqlDbType.VarChar ) { Value = row[ "Val_accountno" ].ToString( ) };
                    insertParams[ 2 ] = new SqlParameter( "@Aladdin", SqlDbType.VarChar ) { Value = row[ "Aladdin" ].ToString( ) };
                    insertParams[ 3 ] = new SqlParameter( "@Val_secunit", SqlDbType.VarChar ) { Value = row[ "Val_secunit" ].ToString( ) };
                    insertParams[ 4 ] = new SqlParameter( "@Val_sic", SqlDbType.VarChar ) { Value = row[ "Val_sic" ].ToString( ) };
                    insertParams[ 5 ] = new SqlParameter( "@Val_prodtype", SqlDbType.VarChar ) { Value = row[ "Val_prodtype" ].ToString( ) };
                    insertParams[ 6 ] = new SqlParameter( "@Val_emisora", SqlDbType.VarChar ) { Value = row[ "Val_emisora" ].ToString( ) };
                    insertParams[ 7 ] = new SqlParameter( "@Val_serie", SqlDbType.VarChar ) { Value = row[ "Val_serie" ].ToString( ) };
                    insertParams[ 8 ] = new SqlParameter( "@Val_cupon", SqlDbType.VarChar ) { Value = row[ "Val_cupon" ] };
                    insertParams[ 9 ] = new SqlParameter( "@Val_custodio", SqlDbType.VarChar ) { Value = row[ "Val_custodio" ].ToString( ) };
                    insertParams[ 10 ] = new SqlParameter( "@Val_posicion", SqlDbType.VarChar ) { Value = row[ "Val_posicion" ] };
                    insertParams[ 11 ] = new SqlParameter( "@val_port", SqlDbType.Decimal ) { Value = string.IsNullOrEmpty( row[ "val_port" ].ToString( ) ) ? 0 : Convert.ToDecimal( row[ "val_port" ], CultureInfo.InvariantCulture ) };
                    insertParams[ 12 ] = new SqlParameter( "@val_settccy", SqlDbType.VarChar ) { Value = row[ "val_settccy" ].ToString( ) };
                    insertParams[ 13 ] = new SqlParameter( "@val_repo", SqlDbType.VarChar ) { Value = row[ "val_repo" ] };
                    insertParams[ 14 ] = new SqlParameter( "@val_isin", SqlDbType.VarChar ) { Value = row[ "val_isin" ].ToString( ) };
                    insertParams[ 15 ] = new SqlParameter( "@val_pre_sucio", SqlDbType.VarChar ) { Value = row[ "val_pre_sucio" ] };
                    insertParams[ 16 ] = new SqlParameter( "@val_depo", SqlDbType.VarChar ) { Value = row[ "val_depo" ] };
                    insertParams[ 17 ] = new SqlParameter( "@Activo", SqlDbType.Bit ) { Value = true };

                    DatabaseHelper dbHelper = new DatabaseHelper( connectionString );
                    int rowsInserted = dbHelper.InsertData( "sp_InsertOpeValuacionesS3", insertParams );

                    if(rowsInserted > 0)
                    {
                        registrosTotalesInsertados++;
                    } else
                    {
                        registrosTotalesActualizados++;
                        string rowValues = string.Join( ", ", row.ItemArray.Select( item => item?.ToString( ) ) );
                        LogfileError.WriteError( $"Se actualizó al insertar registro. Valores: {rowValues}" );
                    }
                } catch(Exception exRow)
                {
                    string rowValues = string.Join( ", ", row.ItemArray.Select( item => item?.ToString( ) ) );
                    LogfileError.WriteError( $"Error al insertar registro: {exRow.Message}. Valores: {rowValues}" );
                }

                // Progreso y UI
                archivo.Progreso = (int) ((currentRow / (float) totalFilas) * 100);
                lblArchivoEnProceso.Text = $"Procesando archivo: {archivo.Nombre} ({archivo.Progreso}%)";

                var lista = ViewState[ KEY_ARCHIVOS ] as List<ArchivoTemporal>;
                lista.RemoveAll( a => a.Nombre == archivo.Nombre );
                lista.Add( archivo );
                ViewState[ KEY_ARCHIVOS ] = lista;

                MostrarArchivos( lista );
                upArchivosPreview.Update( );
            }

            ViewState[ "RegistrosInsertadosValS3" ] = registrosTotalesInsertados;
            ViewState[ "RegistrosActualizadosValS3" ] = registrosTotalesActualizados;
        } catch(Exception ex)
        {
            LogfileError.WriteError( "Error general al procesar archivo Excel Valuaciones S3: " + ex.Message );
        }
    }


    /// <summary>
    /// Carga los datos del archivo de Vector Analítico en la base de datos.
    /// </summary>
    /// <param name="archivo">ArchivoTemporal con ruta, nombre, fuente y progreso</param>
    private void CargarVectoresAnaliticos( ArchivoTemporal archivo )
    {
        string connectionString = ConfigurationManager.ConnectionStrings[ "MyDatabaseConnection" ].ConnectionString;
        LogFileError logFile = new LogFileError( );
        int registrosTotalesInsertados = 0;
        int registrosTotalesActualizados = 0;

        try
        {
            Excel reader = new Excel( );
            DataTable dataTable = reader.ReadCsvFileVectorAnalitico( archivo.Ruta );

            int totalFilas = dataTable.Rows.Count;
            archivo.Registros = totalFilas;

            int currentRow = 0;

            foreach(DataRow row in dataTable.Rows)
            {
                currentRow++;
                try
                {
                    SqlParameter[ ] insertParams = new SqlParameter[ 8 ];

                    insertParams[ 0 ] = new SqlParameter( "@Fecha", SqlDbType.SmallDateTime )
                    {
                        Value = DateTime.ParseExact( row[ "Fecha" ].ToString( ), "yyyyMMdd", CultureInfo.InvariantCulture )
                    };
                    insertParams[ 1 ] = new SqlParameter( "@Tipo_Valor", SqlDbType.VarChar, 50 ) { Value = row[ "TIPO VALOR" ].ToString( ) };
                    insertParams[ 2 ] = new SqlParameter( "@Emisora", SqlDbType.VarChar, 50 ) { Value = row[ "Emisora" ].ToString( ) };
                    insertParams[ 3 ] = new SqlParameter( "@Serie", SqlDbType.VarChar, 50 ) { Value = row[ "Serie" ].ToString( ) };
                    insertParams[ 4 ] = new SqlParameter( "@Precio_Sucio", SqlDbType.Decimal )
                    {
                        Value = string.IsNullOrEmpty( row[ "PRECIO SUCIO" ].ToString( ) ) ? 0 : Convert.ToDecimal( row[ "PRECIO SUCIO" ], CultureInfo.InvariantCulture )
                    };
                    insertParams[ 5 ] = new SqlParameter( "@Precio_Limpio", SqlDbType.Decimal )
                    {
                        Value = string.IsNullOrEmpty( row[ "PRECIO LIMPIO" ].ToString( ) ) ? 0 : Convert.ToDecimal( row[ "PRECIO LIMPIO" ], CultureInfo.InvariantCulture )
                    };
                    insertParams[ 6 ] = new SqlParameter( "@Isin", SqlDbType.VarChar, 50 ) { Value = row[ "Isin" ].ToString( ) };
                    insertParams[ 7 ] = new SqlParameter( "@Activo", SqlDbType.Bit ) { Value = true };

                    DatabaseHelper dbHelper = new DatabaseHelper( connectionString );
                    int rowsInserted = dbHelper.InsertData( "sp_Insert_VectorAnalitico", insertParams );

                    if(rowsInserted > 0)
                    {
                        registrosTotalesInsertados++;
                    } else
                    {
                        registrosTotalesActualizados++;
                        string rowValues = string.Join( ", ", row.ItemArray.Select( item => item?.ToString( ) ) );
                        logFile.WriteError( $"Se actualizó al insertar registro. Valores: {rowValues}" );
                    }
                } catch(Exception exRow)
                {
                    string rowValues = string.Join( ", ", row.ItemArray.Select( item => item?.ToString( ) ) );
                    logFile.WriteError( $"Error al insertar registro: {exRow.Message}. Valores: {rowValues}" );
                }

                // Actualizar progreso y refrescar UI
                archivo.Progreso = (int) ((currentRow / (float) totalFilas) * 100);
                lblArchivoEnProceso.Text = $"Procesando archivo: {archivo.Nombre} ({archivo.Progreso}%)";

                MostrarArchivos( ViewState[ KEY_ARCHIVOS ] as List<ArchivoTemporal> );
                upArchivosPreview.Update( );
            }

            ViewState[ "RegistrosInsertadosVectAnali" ] = registrosTotalesInsertados;
            ViewState[ "RegistrosActualizadosVectAnali" ] = registrosTotalesActualizados;
        } catch(Exception ex)
        {
            logFile.WriteError( "Error general al procesar archivo CSV Vector Analítico: " + ex.Message );
        }
    }



    /// <summary>
    /// Carga un archivo Excel que contiene en la hoja 2 las valuaciones BSMX
    /// y en la hoja 3 las valuaciones S3. Al final actualiza el campo ALADDIN
    /// automáticamente desde el catálogo de custodias.
    /// </summary>
    protected void btnCargarValuacionesUnificadas_Click( object sender, EventArgs e )
    {
        if(!fuArchivoValuaciones.HasFile)
        {
            lblArchivoEnProceso.Text = "Por favor selecciona un archivo Excel.";
            return;
        }

        try
        {
            string nombreArchivo = Path.GetFileName( fuArchivoValuaciones.FileName );
            string rutaArchivo = Server.MapPath( "~/App_Data/" + nombreArchivo );
            fuArchivoValuaciones.SaveAs( rutaArchivo );

            ArchivoTemporal archivo = new ArchivoTemporal
            {
                Nombre = nombreArchivo,
                Ruta = rutaArchivo,
                Fuente = "Valuaciones BSMX + S3",
                Progreso = 0
            };

            // Limpieza visual previa
            lblArchivoEnProceso.Text = "Iniciando carga...";
            upArchivosPreview.Update( );

            // Cargar BSMX (hoja 2)
            CargarValuacionesBSMXDesdeHoja( archivo );

            // Cargar S3 (hoja 3)
            CargarValuacionesS3DesdeHoja( archivo );

            // Paso final: Actualizar automáticamente campo ALADDIN desde el catálogo
            DatabaseHelper dbHelper = new DatabaseHelper( ConfigurationManager.ConnectionStrings[ "MyDatabaseConnection" ].ConnectionString );
            dbHelper.ExecuteStoredProcedure( "sp_Actualizar_Claves_Aladdin" );

            lblArchivoEnProceso.Text = "Carga finalizada correctamente. ALADDIN actualizado.";
            upArchivosPreview.Update( );
        } catch(Exception ex)
        {
            lblArchivoEnProceso.Text = "Error al procesar el archivo: " + ex.Message;
            LogFileError log = new LogFileError( );
            log.WriteError( "Error en btnCargarValuacionesUnificadas_Click: " + ex.ToString( ) );
        }
    }



    private void CargarValuacionesBSMXDesdeHoja( ArchivoTemporal archivo )
    {
        string connectionString = ConfigurationManager.ConnectionStrings[ "MyDatabaseConnection" ].ConnectionString;
        LogFileError logFile = new LogFileError( );
        int registrosInsertados = 0;
        int registrosActualizados = 0;

        try
        {
            Excel reader = new Excel( );
            DataTable dataTable = reader.ReadExcelFileBySheetIndex( archivo.Ruta, 1 ); // hoja 2 = índice 1
            archivo.Registros = dataTable.Rows.Count;

            int currentRow = 0;
            foreach(DataRow row in dataTable.Rows)
            {
                currentRow++;
                try 
                {
                    SqlParameter[ ] insertParams = new SqlParameter[ 15 ];
                    insertParams[ 0 ] = new SqlParameter( "@Column1", SqlDbType.SmallDateTime )
                    {
                        Value = DateTime.TryParseExact( row[ "Column1" ]?.ToString( ), new[ ] { "MM/dd/yyyy", "M/d/yyyy" }, CultureInfo.InvariantCulture, DateTimeStyles.None, out var fecha )
                                ? fecha
                                : throw new FormatException( "Fecha no válida" )
                    };
                    insertParams[ 1 ] = new SqlParameter( "@Column2", SqlDbType.VarChar ) { Value = row[ "Column2" ].ToString( ) };
                    insertParams[ 2 ] = new SqlParameter( "@ALADDIN", SqlDbType.VarChar ) { Value = "" }; // se actualizará después
                    insertParams[ 3 ] = new SqlParameter( "@Column3", SqlDbType.VarChar ) { Value = row[ "Column3" ].ToString( ) };
                    insertParams[ 4 ] = new SqlParameter( "@Column4", SqlDbType.VarChar ) { Value = row[ "Column4" ].ToString( ) };
                    insertParams[ 5 ] = new SqlParameter( "@Column5", SqlDbType.VarChar ) { Value = row[ "Column5" ].ToString( ) };
                    insertParams[ 6 ] = new SqlParameter( "@Column6", SqlDbType.Decimal ) { Value = string.IsNullOrEmpty( row[ "Column6" ].ToString( ) ) ? 0 : Convert.ToDecimal( row[ "Column6" ], CultureInfo.InvariantCulture ) };
                    insertParams[ 7 ] = new SqlParameter( "@Column7", SqlDbType.VarChar ) { Value = row[ "Column7" ].ToString( ) };
                    insertParams[ 8 ] = new SqlParameter( "@Precio_Sucio", SqlDbType.Decimal ) { Value = string.IsNullOrEmpty( row[ "Column8" ].ToString( ) ) ? 0 : Convert.ToDecimal( row[ "Column8" ], CultureInfo.InvariantCulture ) };
                    insertParams[ 9 ] = new SqlParameter( "@Column9", SqlDbType.VarChar ) { Value = row[ "Column9" ].ToString( ) };
                    insertParams[ 10 ] = new SqlParameter( "@Column10", SqlDbType.Decimal ) { Value = string.IsNullOrEmpty( row[ "Column10" ].ToString( ) ) ? 0 : Convert.ToDecimal( row[ "Column10" ], CultureInfo.InvariantCulture ) };
                    insertParams[ 11 ] = new SqlParameter( "@Column11", SqlDbType.VarChar ) { Value = row[ "Column11" ].ToString( ) };
                    insertParams[ 12 ] = new SqlParameter( "@Column12", SqlDbType.VarChar ) { Value = row[ "Column12" ].ToString( ) };
                    insertParams[ 13 ] = new SqlParameter( "@Column13", SqlDbType.Decimal ) { Value = string.IsNullOrEmpty( row[ "Column13" ].ToString( ) ) ? 0 : Convert.ToDecimal( row[ "Column13" ], CultureInfo.InvariantCulture ) };
                    insertParams[ 14 ] = new SqlParameter( "@Activo", SqlDbType.Bit ) { Value = true };

                    DatabaseHelper dbHelper = new DatabaseHelper( connectionString );
                    int rowsInserted = dbHelper.InsertData( "Insert_OpeValuacionesBsmx", insertParams );

                    if(rowsInserted > 0)
                        registrosInsertados++;
                    else
                    {
                        registrosActualizados++;
                        logFile.WriteError( "Registro BSMX actualizado: " + string.Join( ", ", row.ItemArray ) );
                    }
                } catch(Exception exRow)
                {
                    logFile.WriteError( "Error al insertar BSMX: " + exRow.Message + " Valores: " + string.Join( ", ", row.ItemArray ) );
                }

                archivo.Progreso = (int) ((currentRow / (float) dataTable.Rows.Count) * 100);
                lblArchivoEnProceso.Text = $"Procesando BSMX: {archivo.Nombre} ({archivo.Progreso}%)";
                MostrarArchivos( ViewState[ KEY_ARCHIVOS ] as List<ArchivoTemporal> );
                upArchivosPreview.Update( );
            }

            ViewState[ "RegistrosInsertadosValBSMX" ] = registrosInsertados;
            ViewState[ "RegistrosActualizadosValBSMX" ] = registrosActualizados;
        } catch(Exception ex)
        {
            logFile.WriteError( "Error general al procesar BSMX: " + ex.Message );
        }
    }


    private void CargarValuacionesS3DesdeHoja( ArchivoTemporal archivo )
    {
        string connectionString = ConfigurationManager.ConnectionStrings[ "MyDatabaseConnection" ].ConnectionString;
        LogFileError logFile = new LogFileError( );
        int registrosInsertados = 0;
        int registrosActualizados = 0;

        try
        {
            Excel reader = new Excel( );
            DataTable dataTable = reader.ReadExcelFileBySheetIndex( archivo.Ruta, 2 ); // hoja 3 = índice 2
            archivo.Registros = dataTable.Rows.Count;

            int currentRow = 0;
            foreach(DataRow row in dataTable.Rows)
            {
                currentRow++;
                try
                {
                    SqlParameter[ ] insertParams = new SqlParameter[ 18 ];
                    insertParams[ 0 ] = new SqlParameter( "@Val_effdate", SqlDbType.SmallDateTime )
                    {
                        Value = DateTime.TryParseExact( row[ "Val_effdate" ]?.ToString( ), new[ ] { "MM/dd/yyyy", "M/d/yyyy" }, CultureInfo.InvariantCulture, DateTimeStyles.None, out var fecha )
                                ? fecha
                                : throw new FormatException( "Fecha no válida" )
                    };
                    insertParams[ 1 ] = new SqlParameter( "@Val_accountno", SqlDbType.VarChar ) { Value = row[ "Val_accountno" ].ToString( ) };
                    insertParams[ 2 ] = new SqlParameter( "@Aladdin", SqlDbType.VarChar ) { Value = "" }; // se llenará después
                    insertParams[ 3 ] = new SqlParameter( "@Val_secunit", SqlDbType.VarChar ) { Value = row[ "Val_secunit" ].ToString( ) };
                    insertParams[ 4 ] = new SqlParameter( "@Val_sic", SqlDbType.VarChar ) { Value = row[ "Val_sic" ].ToString( ) };
                    insertParams[ 5 ] = new SqlParameter( "@Val_prodtype", SqlDbType.VarChar ) { Value = row[ "Val_prodtype" ].ToString( ) };
                    insertParams[ 6 ] = new SqlParameter( "@Val_emisora", SqlDbType.VarChar ) { Value = row[ "Val_emisora" ].ToString( ) };
                    insertParams[ 7 ] = new SqlParameter( "@Val_serie", SqlDbType.VarChar ) { Value = row[ "Val_serie" ].ToString( ) };
                    insertParams[ 8 ] = new SqlParameter( "@Val_cupon", SqlDbType.VarChar ) { Value = row[ "Val_cupon" ].ToString( ) };
                    insertParams[ 9 ] = new SqlParameter( "@Val_custodio", SqlDbType.VarChar ) { Value = row[ "Val_custodio" ].ToString( ) };
                    insertParams[ 10 ] = new SqlParameter( "@Val_posicion", SqlDbType.VarChar ) { Value = row[ "Val_posicion" ].ToString( ) };
                    insertParams[ 11 ] = new SqlParameter( "@val_port", SqlDbType.Decimal ) { Value = string.IsNullOrEmpty( row[ "val_port" ].ToString( ) ) ? 0 : Convert.ToDecimal( row[ "val_port" ], CultureInfo.InvariantCulture ) };
                    insertParams[ 12 ] = new SqlParameter( "@val_settccy", SqlDbType.VarChar ) { Value = row[ "val_settccy" ].ToString( ) };
                    insertParams[ 13 ] = new SqlParameter( "@val_repo", SqlDbType.VarChar ) { Value = row[ "val_repo" ].ToString( ) };
                    insertParams[ 14 ] = new SqlParameter( "@val_isin", SqlDbType.VarChar ) { Value = row[ "val_isin" ].ToString( ) };
                    insertParams[ 15 ] = new SqlParameter( "@val_pre_sucio", SqlDbType.VarChar ) { Value = row[ "val_pre_sucio" ].ToString( ) };
                    insertParams[ 16 ] = new SqlParameter( "@val_depo", SqlDbType.VarChar ) { Value = row[ "val_depo" ].ToString( ) };
                    insertParams[ 17 ] = new SqlParameter( "@Activo", SqlDbType.Bit ) { Value = true };

                    DatabaseHelper dbHelper = new DatabaseHelper( connectionString );
                    int rowsInserted = dbHelper.InsertData( "sp_InsertOpeValuacionesS3", insertParams );

                    if(rowsInserted > 0)
                        registrosInsertados++;
                    else
                    {
                        registrosActualizados++;
                        logFile.WriteError( "Registro S3 actualizado: " + string.Join( ", ", row.ItemArray ) );
                    }
                } catch(Exception exRow)
                {
                    logFile.WriteError( "Error al insertar S3: " + exRow.Message + " Valores: " + string.Join( ", ", row.ItemArray ) );
                }

                archivo.Progreso = (int) ((currentRow / (float) dataTable.Rows.Count) * 100);
                lblArchivoEnProceso.Text = $"Procesando S3: {archivo.Nombre} ({archivo.Progreso}%)";
                MostrarArchivos( ViewState[ KEY_ARCHIVOS ] as List<ArchivoTemporal> );
                upArchivosPreview.Update( );
            }

            ViewState[ "RegistrosInsertadosValS3" ] = registrosInsertados;
            ViewState[ "RegistrosActualizadosValS3" ] = registrosActualizados;
        } catch(Exception ex)
        {
            logFile.WriteError( "Error general al procesar S3: " + ex.Message );
        }
    }



}
