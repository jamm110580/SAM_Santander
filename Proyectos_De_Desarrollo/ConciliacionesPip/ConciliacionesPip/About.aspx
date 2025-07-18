<%@ Page Title="About" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="About.aspx.cs" Inherits="About" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    
    <div class="container header">
        <h1 class="text-center">Sistema de Gestión de Valores</h1>
        <div class="text-center mt-3">
            <asp:Button ID="btnConciliacion" runat="server" Text="Conciliación" CssClass="btn btn-danger btn-lg" OnClick="btnConciliacion_Click" />
        </div>
    </div>
    
    <div class="container result-container">
        <asp:GridView ID="gridViewResultados" runat="server" CssClass="table table-striped grid-view" AutoGenerateColumns="false" 
            AllowPaging="True" PageSize="10" OnPageIndexChanging="gridViewResultados_PageIndexChanging">
            <Columns>
                <asp:BoundField DataField="ISIN_ALADDIN" HeaderText="ISIN Aladdin" />
                <asp:BoundField DataField="Isin_PIP" HeaderText="ISIN PIP" />
                <asp:BoundField DataField="Orig_Face" HeaderText="Original Face" />
                <asp:BoundField DataField="Precio_limpio_ALADDIN" HeaderText="Precio Limpio Aladdin" />
                <asp:BoundField DataField="Precio_Limpio_PIP" HeaderText="Precio Limpio PIP" />
                <asp:BoundField DataField="ALADDIN_Orig_Face_ALADDIN" HeaderText="Aladdin Orig Face" />
            </Columns>
        </asp:GridView>
    </div>
</asp:Content>
