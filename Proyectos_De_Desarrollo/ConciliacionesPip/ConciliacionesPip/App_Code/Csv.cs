using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.IO;
using System.Text;

/// <summary>
/// Descripción breve de Csv
/// </summary>
public class Csv
{
    public Csv( )
    {
        //
        // TODO: Agregar aquí la lógica del constructor
        //
    }


    public DataSet LoadCsvToDataSet( string filePath )
    {
        DataSet dataSet = new DataSet( );
        DataTable dataTable = new DataTable( );
        LogFileError logFileError = new LogFileError( );

        // Usa la codificación adecuada
        using(var reader = new StreamReader( filePath, Encoding.GetEncoding( "ISO-8859-1" ) ))
        {
            // Leer la primera línea para obtener los encabezados
            string headerLine = reader.ReadLine( );
            if(headerLine != null)
            {
                string[ ] headers = headerLine.Split( ',' );
                foreach(var header in headers)
                {
                    dataTable.Columns.Add( header );
                }

                // Leer el resto del archivo
                while(!reader.EndOfStream)
                {
                    string line = reader.ReadLine( );
                    string[ ] rows = line.Split( ',' );

                    try
                    {
                        if(rows.Length == dataTable.Columns.Count)
                        {
                            dataTable.Rows.Add( rows );
                        } else
                        {
                            logFileError.WriteError( $"Error en línea: {line} - Longitud de filas: {rows.Length}, esperada: {dataTable.Columns.Count}" );
                        }
                    } catch(Exception ex)
                    {
                        logFileError.WriteError( $"Error al procesar línea: {line} - Excepción: {ex.Message}" );
                    }
                }
            }
        }

        dataSet.Tables.Add( dataTable );
        return dataSet;
    }




}