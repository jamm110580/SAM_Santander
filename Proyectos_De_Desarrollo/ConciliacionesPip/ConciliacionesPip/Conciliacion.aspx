<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Conciliacion.aspx.cs" Inherits="Conciliacion" Title="Conciliación" MasterPageFile="~/Site.Master" %>

<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="System.Data.SqlClient" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <style>
        .hero-container {
            position: relative;
            background-image: url('img/Fondo_Oficina.jpg');
            background-size: cover;
            background-position: center;
            min-height: 90vh;
            display: flex;
            flex-direction: column; /* Para que el contenido fluya hacia abajo */
            align-items: flex-start; /* Alinea al inicio verticalmente */
            justify-content: flex-start; /* Evita centrado vertical */
            padding: 40px;
            color: white;
            text-shadow: 2px 2px 4px rgba(0,0,0,0.7);
        }

        .hero-title {
            font-size: 2.5em;
            font-weight: bold;
            text-align: center;
            background-color: rgba(0, 0, 0, 0.7);
            color: white;
            padding: 20px;
            margin-bottom: 20px;
            border-radius: 8px;
            width: 100%;
        }

        footer {
            width: 100%;
            background-color: rgba(0,0,0,0.6);
            color: white;
            text-align: center;
            padding: 10px 0;
            position: relative;
        }

        .btn-custom {
            font-size: 1.2em;
            padding: 12px 30px;
            margin-bottom: 20px;
        }

         .filtros-bloque {
        display: flex;
        gap: 20px;
        align-items: flex-end;
        flex-wrap: wrap;
        margin-bottom: 20px;
    }

    .filtros-container {
        background-color: rgba(255, 255, 255, 0.9);
        padding: 20px;
        border-radius: 8px;
        display: flex;
        gap: 20px;
        align-items: flex-end;
        box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
    }

    .filtro-item label {
        font-weight: bold;
        color: #212529;
        margin-bottom: 5px;
        display: block;
    }

    .btn-custom {
        font-size: 1.2em;
        padding: 12px 30px;
        white-space: nowrap;
    }

        /* Contenedor con scroll vertical para el GridView */
        .grid-view {
            width: 90%;
            overflow-x: auto; /* Scroll horizontal si la tabla es ancha */
            max-height: 700px; /* Ajusta la altura que quieras */
            overflow-y: auto; /* Scroll vertical */
            border: 1px solid #ddd;
            background-color: white; /* Para mejor lectura */
        }

            .grid-view table {
                width: 100%; /* Para que ocupe todo el contenedor */
                border-collapse: collapse;
                table-layout: fixed; /* Crucial para respetar anchos fijos */
            }

            .grid-view th {
                background-color: #212529;
                color: white; /* Puedes dejar blanco para header si quieres, o cambiar a negro también */
                text-align: center;
                padding: 10px;
                word-wrap: break-word;
                text-shadow: none !important; /* Quitamos la sombra si existía */
            }

            .grid-view td {
                text-align: center;
                padding: 10px;
                border: 1px solid #dee2e6;
                overflow: hidden;
                text-overflow: ellipsis;
                white-space: nowrap;
                color: black !important; /* Aquí el cambio principal */
                text-shadow: none !important;
            }

            .grid-view tr:nth-child(even) {
                background-color: #f8f9fa;
            }

            .grid-view tr:hover {
                background-color: #e9ecef;
            }
    </style>

    <div class="hero-container">

        <div>

            <div class="hero-title">Conciliación Mensual</div>


        </div>
        <div style="margin-bottom: 20px;">

           <div class="filtros-bloque">
    <div class="filtros-container">
        <div class="filtro-item">
            <label for="ddlMes">Mes de conciliación:</label>
            <asp:DropDownList ID="ddlMes" runat="server" CssClass="form-control" Style="min-width: 180px;"></asp:DropDownList>
        </div>
        <div class="filtro-item">
            <label for="ddlAnio">Año:</label>
            <asp:DropDownList ID="ddlAnio" runat="server" CssClass="form-control" Style="min-width: 120px;"></asp:DropDownList>
        </div>
    </div>

    <asp:Button ID="btnConciliacion" runat="server" Text="Generar Conciliación" CssClass="btn btn-danger btn-custom" OnClick="btnConciliacion_Click" />
</div>

        </div>

        <div class="grid-view">


            <asp:GridView ID="gridViewResultados" runat="server" CssClass="table table-striped" AutoGenerateColumns="false"
                AllowPaging="true" PageSize="12"
                OnPageIndexChanging="gridViewResultados_PageIndexChanging"
                OnRowDataBound="gridViewResultados_RowDataBound">

                <Columns>
                    <asp:BoundField ItemStyle-Width="70px" HeaderStyle-Width="70px" DataField="Origen" HeaderText="Origen" />
                    <asp:BoundField ItemStyle-Width="170px" HeaderStyle-Width="170px" DataField="ISIN_Banco" HeaderText="ISIN" />
                    <asp:BoundField ItemStyle-Width="100px" HeaderStyle-Width="100px" DataField="Fecha_Banco" HeaderText="Fecha" />
                    <asp:BoundField ItemStyle-Width="100px" HeaderStyle-Width="100px" DataField="Portafolio_Banco" HeaderText="Portafolio" />
                    <asp:BoundField ItemStyle-Width="50px" HeaderStyle-Width="50px" DataField="Tipo_Banco" HeaderText="Tipo" />
                    <asp:BoundField ItemStyle-Width="100px" HeaderStyle-Width="100px" DataField="Emisora_Banco" HeaderText="Emisora" />
                    <asp:BoundField ItemStyle-Width="100px" HeaderStyle-Width="100px" DataField="Serie_Banco" HeaderText="Serie" />
                    <asp:BoundField ItemStyle-Width="150px" HeaderStyle-Width="150px" DataField="Tipo_Valor_SAM" HeaderText="Tipo Valor SAM" />
                    <asp:BoundField ItemStyle-Width="100px" HeaderStyle-Width="100px" DataField="Titulo_Aladdin" HeaderText="Título Aladdin" DataFormatString="{0:N2}" />
                    <asp:BoundField ItemStyle-Width="100px" HeaderStyle-Width="100px" DataField="Unsettled" HeaderText="Unsettled" DataFormatString="{0:N2}" />
                    <asp:BoundField ItemStyle-Width="100px" HeaderStyle-Width="100px" DataField="Vector_Precio_Sucio" HeaderText="Vector Precio Sucio" DataFormatString="{0:N6}" />
                    <asp:BoundField ItemStyle-Width="100px" HeaderStyle-Width="100px" DataField="Vector_Precio_Limpio" HeaderText="Vector Precio Limpio" DataFormatString="{0:N6}" />
                    <asp:BoundField ItemStyle-Width="150px" HeaderStyle-Width="150px" DataField="Valor_Banco" HeaderText="Valor Banco" DataFormatString="{0:N2}" />
                    <asp:BoundField ItemStyle-Width="150px" HeaderStyle-Width="150px" DataField="Valor_SAM" HeaderText="Valor SAM" DataFormatString="{0:N2}" />
                    <asp:BoundField ItemStyle-Width="100px" HeaderStyle-Width="100px" DataField="Diferencia" HeaderText="Diferencia" DataFormatString="{0:N2}" />
                    <asp:BoundField ItemStyle-Width="100px" HeaderStyle-Width="100px" DataField="%_Diferencia" HeaderText="% Diferencia" DataFormatString="{0:N2}" />
                </Columns>
            </asp:GridView>
        </div>
    </div>
</asp:Content>
