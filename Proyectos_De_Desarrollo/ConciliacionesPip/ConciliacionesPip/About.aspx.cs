using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class About : Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }


    protected void btnConciliacion_Click( object sender, EventArgs e )
    {
        // Llama al método para llenar el GridView
        CargarResultados( );
    }

    private void CargarResultados( )
    {
        string connectionString = ConfigurationManager.ConnectionStrings[ "MyDatabaseConnection" ].ConnectionString;
        using(SqlConnection connection = new SqlConnection( connectionString ))
        {
            using(SqlCommand command = new SqlCommand( "sp_Select_conciliacion", connection ))
            {
                command.CommandType = CommandType.StoredProcedure;
                SqlDataAdapter adapter = new SqlDataAdapter( command );
                DataSet ds = new DataSet( );
                adapter.Fill( ds );

                gridViewResultados.DataSource = ds;
                gridViewResultados.DataBind( );
            }
        }
    }

    protected void gridViewResultados_PageIndexChanging( object sender, GridViewPageEventArgs e )
    {
        gridViewResultados.PageIndex = e.NewPageIndex; // Cambia la página
        CargarResultados( ); // Vuelve a cargar los resultados
    }

}