using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Descripción breve de ArchivoTemporal
/// </summary>
[Serializable]
public class ArchivoTemporal
{

   

    public ArchivoTemporal( )
    {
        //
        // TODO: Agregar aquí la lógica del constructor
        //
    }

    
    public string Nombre { get; set; }
    public string Ruta { get; set; }
    public string Fuente { get; set; }
    public int Registros { get; set; }
    public int Progreso { get; set; } = 0; // NUEVO

}