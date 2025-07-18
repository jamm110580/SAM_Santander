using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

public static class DatabaseHelper
{
    private static string connectionString = ConfigurationManager.ConnectionStrings[ "MyDatabaseConnection" ].ConnectionString;

    // Ejecuta un Store que regresa un solo valor (ej: COUNT, IDENTITY, etc)
    public static object EjecutarEscalar( string nombreSP, SqlParameter[ ] parametros )
    {
        using(SqlConnection conn = new SqlConnection( connectionString ))
        using(SqlCommand cmd = new SqlCommand( nombreSP, conn ))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            if(parametros != null)
                cmd.Parameters.AddRange( parametros );

            conn.Open( );
            return cmd.ExecuteScalar( );
        }
    }

    // Ejecuta un Store que no regresa resultado (INSERT, UPDATE, DELETE)
    public static void EjecutarNonQuery( string nombreSP, SqlParameter[ ] parametros )
    {
        using(SqlConnection conn = new SqlConnection( connectionString ))
        using(SqlCommand cmd = new SqlCommand( nombreSP, conn ))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            if(parametros != null)
                cmd.Parameters.AddRange( parametros );

            conn.Open( );
            cmd.ExecuteNonQuery( );
        }
    }

    // Ejecuta un Store que regresa un DataTable (SELECT de muchos registros)
    public static DataTable EjecutarDataTable( string nombreSP, SqlParameter[ ] parametros )
    {
        using(SqlConnection conn = new SqlConnection( connectionString ))
        using(SqlCommand cmd = new SqlCommand( nombreSP, conn ))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            if(parametros != null)
                cmd.Parameters.AddRange( parametros );

            using(SqlDataAdapter adapter = new SqlDataAdapter( cmd ))
            {
                DataTable dt = new DataTable( );
                adapter.Fill( dt );
                return dt;
            }
        }
    }

    // Ejecuta un Store y regresa un SqlDataReader (si quieres leer fila por fila)
    public static SqlDataReader EjecutarReader( string nombreSP, SqlParameter[ ] parametros, SqlConnection conn )
    {
        SqlCommand cmd = new SqlCommand( nombreSP, conn );
        cmd.CommandType = CommandType.StoredProcedure;
        if(parametros != null)
            cmd.Parameters.AddRange( parametros );

        return cmd.ExecuteReader( );
    }
}
