using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Globalization;
using System.Web.UI.WebControls;

public partial class Conciliacion : System.Web.UI.Page
{
    protected void Page_Load( object sender, EventArgs e )
    {
        if(!IsPostBack)
        {
            LlenarMeses( );
            LlenarAnios( );
        }
    }

    private void LlenarMeses( )
    {
        ddlMes.Items.Clear( );

        for(int i = 1; i <= 12; i++)
        {
            string mesNombre = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName( i );
            ddlMes.Items.Add( new ListItem( mesNombre, i.ToString( ) ) );
        }

        // Puedes seleccionar el mes actual por default
        ddlMes.SelectedValue = DateTime.Now.Month.ToString( );
    }

    private void LlenarAnios( )
    {
        ddlAnio.Items.Clear( );

        int anioActual = DateTime.Now.Year;
        int anioInicio = anioActual - 5;
        int anioFin = anioActual + 5;

        for(int i = anioInicio; i <= anioFin; i++)
        {
            ddlAnio.Items.Add( new ListItem( i.ToString( ), i.ToString( ) ) );
        }

        ddlAnio.SelectedValue = anioActual.ToString( );
    }

    protected void btnConciliacion_Click( object sender, EventArgs e )
    {
        CargarResultados( );
    }

    private void CargarResultados( )
    {
        string connectionString = ConfigurationManager.ConnectionStrings[ "MyDatabaseConnection" ].ConnectionString;
        DatabaseHelper databaseHelper = new DatabaseHelper( connectionString );

        SqlParameter[ ] parameters = null;

        if(btnConciliacion.Text == "Generar Conciliación" || btnConciliacion.Text == "Mostrar todas")
        {
            parameters = new SqlParameter[ ]
            {
            new SqlParameter("@TipoValor", SqlDbType.VarChar) { Value = "ConciliacionCompleta" }
            };
            btnConciliacion.Text = "Mostrar solo diferencias";
        } else if(btnConciliacion.Text == "Mostrar solo diferencias")
        {
            parameters = new SqlParameter[ ]
            {
            new SqlParameter("@TipoValor", SqlDbType.VarChar) { Value = "ConciliacioSoloDiferencias" }
            };
            btnConciliacion.Text = "Mostrar todas";
        }

        if(parameters != null)
        {
            DataSet ds = databaseHelper.ExecuteQuery( "sp_Select_conciliacion", parameters );

            if(ds != null && ds.Tables.Count > 0 && ds.Tables[ 0 ].Rows.Count > 0)
            {
                DataTable dt = ds.Tables[ 0 ];

                // Asignamos el DataSource completo (GridView se encarga de paginar)
                gridViewResultados.DataSource = dt;
                gridViewResultados.DataBind( );
            } else
            {
                gridViewResultados.DataSource = null;
                gridViewResultados.DataBind( );
            }
        }
    }


    protected void gridViewResultados_RowDataBound( object sender, System.Web.UI.WebControls.GridViewRowEventArgs e )
    {
        // Aquí puedes aplicar lógica adicional por fila si quieres
    }

    protected void gridViewResultados_PageIndexChanging( object sender, GridViewPageEventArgs e )
    {
        gridViewResultados.PageIndex = e.NewPageIndex;
        CargarResultados( );
    }
}
