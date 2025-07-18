using System;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Threading;

public class DatabaseHelper
{
    private string _connectionString;

    // Constructor para inicializar la cadena de conexión
    public DatabaseHelper( string connectionString )
    {
        _connectionString = connectionString;
    }

    // Método para consultar datos mediante un Stored Procedure
    public DataSet ExecuteQuery( string storedProcedureName, SqlParameter[ ] parameters = null )
    {
        DataSet dataSet = new DataSet( );

        using(SqlConnection connection = new SqlConnection( _connectionString ))
        {
            using(SqlCommand command = new SqlCommand( storedProcedureName, connection ))
            {
                command.CommandType = CommandType.StoredProcedure;

                if(parameters != null)
                {
                    command.Parameters.AddRange( parameters );
                }

                using(SqlDataAdapter adapter = new SqlDataAdapter( command ))
                {
                    try
                    {
                        connection.Open( );
                        adapter.Fill( dataSet );
                    } catch(Exception ex)
                    {
                        // Manejo de excepciones
                        throw new Exception( "Error al ejecutar la consulta: " + ex.Message );
                    }
                }
            }
        }

        return dataSet;
    }

    // Método para insertar datos mediante un Stored Procedure
    public int InsertData( string storedProcedureName, SqlParameter[ ] parameters )
    {
        using(SqlConnection connection = new SqlConnection( _connectionString ))
        {
            using(SqlCommand command = new SqlCommand( storedProcedureName, connection ))
            {
                command.CommandType = CommandType.StoredProcedure;

                if(parameters != null)
                {
                    command.Parameters.AddRange( parameters );
                }

                try
                {
                    connection.Open( );

                    // Ejecutar el procedimiento almacenado y recuperar el valor de "Resultado" (la última columna seleccionada)
                    int resultado = (int) command.ExecuteScalar( );  // Ejecuta el Stored Procedure y recupera el valor de "Resultado"
                    
                    connection.Close( );

                    return resultado;  // Retorna el valor 0 (UPDATE) o 1 (INSERT)

                } catch(Exception ex)
                {
                    connection.Close( );
                    // Manejo de excepciones
                    throw new Exception( "Error al insertar datos: " + ex.Message );
                }
            }
        }
    }

    /// <summary>
    /// Ejecuta un procedimiento almacenado sin parámetros.
    /// </summary>
    /// <param name="storedProcedureName">Nombre del procedimiento almacenado</param>
    public void ExecuteStoredProcedure( string storedProcedureName )
    {
        using(SqlConnection connection = new SqlConnection( _connectionString ))
        using(SqlCommand command = new SqlCommand( storedProcedureName, connection ))
        {
            command.CommandType = CommandType.StoredProcedure;
            connection.Open( );
            command.ExecuteNonQuery( );
        }
    }


    // Método para actualizar datos mediante un Stored Procedure
    public int UpdateData( string storedProcedureName, SqlParameter[ ] parameters )
    {
        using(SqlConnection connection = new SqlConnection( _connectionString ))
        {
            using(SqlCommand command = new SqlCommand( storedProcedureName, connection ))
            {
                command.CommandType = CommandType.StoredProcedure;

                if(parameters != null)
                {
                    command.Parameters.AddRange( parameters );
                }

                try
                {
                    connection.Open( );
                    int rowsAffected = command.ExecuteNonQuery( );
                    return rowsAffected;
                } catch(Exception ex)
                {
                    // Manejo de excepciones
                    throw new Exception( "Error al actualizar datos: " + ex.Message );
                }
            }
        }
    }

    // Método para borrar datos mediante un Stored Procedure
    public int DeleteData( string storedProcedureName, SqlParameter[ ] parameters )
    {
        using(SqlConnection connection = new SqlConnection( _connectionString ))
        {
            using(SqlCommand command = new SqlCommand( storedProcedureName, connection ))
            {
                command.CommandType = CommandType.StoredProcedure;

                if(parameters != null)
                {
                    command.Parameters.AddRange( parameters );
                }

                try
                {
                    connection.Open( );
                    int rowsAffected = command.ExecuteNonQuery( );
                    return rowsAffected;
                } catch(Exception ex)
                {
                    // Manejo de excepciones
                    throw new Exception( "Error al borrar datos: " + ex.Message );
                }
            }
        }
    }
}
