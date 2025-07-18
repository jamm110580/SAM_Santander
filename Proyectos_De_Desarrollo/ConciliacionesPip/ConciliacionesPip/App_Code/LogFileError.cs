using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;

/// <summary>
/// Descripción breve de LogFileError
/// </summary>
public class LogFileError
{
    public LogFileError( )
    {
        //
        // TODO: Agregar aquí la lógica del constructor
        //
    }

    public void WriteError( string message )
    {
        //Obtine la ruta del archivo donde se escribiran los errores.
        string logFilePath = ConfigurationManager.AppSettings[ "logFilePath" ];

        // Verificar si el archivo de log existe; si no, lo crea
        if(!File.Exists( logFilePath ))
        {
            using(File.Create( logFilePath ))
            { } // Crea el archivo y lo cierra
        }

        using(var logWriter = new StreamWriter( logFilePath, true ))
        {
            logWriter.WriteLine( $"{DateTime.Now}: {message}" );
        }
    }
}