using System;
using System.IO;

namespace DispersionMailerDesktop.Utilidades
{
    public static class Logger
    {
        public static void RegistrarErrorEnLog( string mensaje )
        {
            string rutaLog = @"C:\DispersionMailer\Datos\errores.log";

            try
            {
                string directorio = Path.GetDirectoryName( rutaLog );
                if(!Directory.Exists( directorio ))
                {
                    Directory.CreateDirectory( directorio );
                }

                using(StreamWriter writer = new StreamWriter( rutaLog, true ))
                {
                    writer.WriteLine( $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {mensaje}" );
                }
            } catch
            {
                // Silenciar errores del log para no detener la aplicación
            }
        }
    }
}