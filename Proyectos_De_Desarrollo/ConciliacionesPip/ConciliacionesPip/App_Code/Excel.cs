using OfficeOpenXml;
using System;
using System.Data;
using System.IO;
using System.Text;


public class Excel
{
    public Excel( )
    {
        // Establecer el contexto de la licencia
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
    }

    public DataSet ReadExcelFile( string filePath )
    {
        // Crear un DataSet para almacenar los datos
        DataSet dataSet = new DataSet( );

        // Asegúrate de que el archivo exista
        if(!File.Exists( filePath ))
        {
            throw new FileNotFoundException( "El archivo no existe.", filePath );
        }

        // Cargar el archivo Excel
        using(var package = new ExcelPackage( new FileInfo( filePath ) ))
        {
            foreach(var worksheet in package.Workbook.Worksheets)
            {
                DataTable dataTable = new DataTable( worksheet.Name );
                int rowCount = worksheet.Dimension.Rows;
                int colCount = worksheet.Dimension.Columns;

                // Agregar las columnas al DataTable
                for(int col = 1; col <= colCount; col++)
                {
                    dataTable.Columns.Add( worksheet.Cells[ 1, col ].Text ); // Usar la primera fila como encabezados
                }

                // Agregar las filas al DataTable
                for(int row = 2; row <= rowCount; row++) // Comenzar desde la fila 2 para evitar los encabezados
                {
                    var newRow = dataTable.NewRow( );
                    for(int col = 1; col <= colCount; col++)
                    {
                        newRow[ col - 1 ] = worksheet.Cells[ row, col ].Text; // Almacenar datos en la nueva fila
                    }
                    dataTable.Rows.Add( newRow );
                }

                // Agregar el DataTable al DataSet
                dataSet.Tables.Add( dataTable );
            }
        }

        return dataSet;
    }


    /// <summary>
    /// Lee los archivos de Excel de Aladdin
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
    /// <exception cref="FileNotFoundException"></exception>
    public DataTable ReadExcelFileAladdin( string filePath )
    {
        // Crear un DataSet para almacenar los datos
        DataTable dataTable = new DataTable( );

        // Asegúrate de que el archivo exista
        if(!File.Exists( filePath ))
        {
            throw new FileNotFoundException( "El archivo no existe.", filePath );
        }

        // Cargar el archivo Excel
        using(var package = new ExcelPackage( new FileInfo( filePath ) ))
        {
            var workSheet = package.Workbook.Worksheets[ 0 ];  //Abre la hoja uno

            int startRow = 13;
            int endColumn = workSheet.Dimension.End.Column;
            int endRow = workSheet.Dimension.End.Row;

            //Lee los nombres de las  columnas desde la fila 13
            for(int col = 1; col <= endColumn; col++)
            {
                var columnName = workSheet.Cells[ startRow, col ].Text.Trim( );
                if(string.IsNullOrEmpty( columnName ))
                {
                    columnName = $"Columna{col}";
                }
                dataTable.Columns.Add( columnName );
            }

            //Leeer los datos desde la fila 14 en adelante
            for(int row = startRow + 1; row <= endRow; row++)
            {
                var newRow = dataTable.NewRow( );
                for(int col = 1; col <= endColumn; col++)
                {
                    newRow[ col - 1 ] = workSheet.Cells[ row, col ].Text;
                }
                dataTable.Rows.Add( newRow );
            }
        }

        return dataTable;
    }


    /// <summary>
    /// Lee un archivo Excel y devuelve el contenido de una hoja específica como DataTable.
    /// </summary>
    /// <param name="filePath">Ruta del archivo .xlsx</param>
    /// <param name="sheetIndex">Índice de la hoja (0 = primera hoja)</param>
    /// <returns>DataTable con los datos de la hoja</returns>
    public DataTable ReadExcelFileBySheetIndex( string filePath, int sheetIndex )
    {
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        DataTable dataTable = new DataTable( );

        if(!File.Exists( filePath ))
            throw new FileNotFoundException( "Archivo no encontrado.", filePath );

        using(var package = new ExcelPackage( new FileInfo( filePath ) ))
        {
            if(package.Workbook.Worksheets.Count <= sheetIndex)
                throw new ArgumentException( "El archivo no contiene la hoja especificada." );

            var worksheet = package.Workbook.Worksheets[ sheetIndex ];
            bool esPrimeraFila = true;

            for(int row = 1; row <= worksheet.Dimension.End.Row; row++)
            {
                if(esPrimeraFila)
                {
                    for(int col = 1; col <= worksheet.Dimension.End.Column; col++)
                        dataTable.Columns.Add( worksheet.Cells[ 1, col ].Text );
                    esPrimeraFila = false;
                } else
                {
                    var nuevaFila = dataTable.NewRow( );
                    for(int col = 1; col <= worksheet.Dimension.End.Column; col++)
                        nuevaFila[ col - 1 ] = worksheet.Cells[ row, col ].Text;
                    dataTable.Rows.Add( nuevaFila );
                }
            }
        }

        return dataTable;
    }



    public DataTable ReadCsvFileVectorAnalitico( string rutaArchivo )
    {
        DataTable dataTable = new DataTable( );

        using(var reader = new StreamReader( rutaArchivo, Encoding.UTF8 )) // ⚠️ Fuerza UTF-8 aquí
        {
            bool esPrimeraLinea = true;
            string[ ] columnas;

            while(!reader.EndOfStream)
            {
                var linea = reader.ReadLine( );
                if(string.IsNullOrWhiteSpace( linea ))
                    continue;

                var valores = linea.Split( ',' );

                if(esPrimeraLinea)
                {
                    columnas = valores;
                    foreach(var nombreColumna in columnas)
                    {
                        // Elimina posibles espacios extra
                        dataTable.Columns.Add( nombreColumna.Trim( ) );
                    }
                    esPrimeraLinea = false;
                } else
                {
                    dataTable.Rows.Add( valores );
                }
            }
        }

        return dataTable;
    }


}
