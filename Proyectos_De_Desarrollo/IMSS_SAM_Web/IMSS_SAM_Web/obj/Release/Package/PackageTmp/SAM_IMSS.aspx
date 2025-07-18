<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="SAM_IMSS.aspx.cs" Inherits="IMSS_SAM_Web.SAM_IMSS" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <header>
        <link href="~/Content/styles.css" rel="stylesheet" />
    </header>

    <script>

        window.onload = function () {
            document.getElementById("spinner").style.display = "none";
            document.getElementById("overlay").style.display = "none";
        }

        function showSpinner() {
            document.getElementById("spinner").style.display = "block";
            document.getElementById("overlay").style.display = "block";
        }

    </script>
    <!-- Contenedor principal con clases de Bootstrap -->
    <div class="container mt-4">

        <div id="overlay" style="display:none; position:fixed; top:0; left:0; width:100%; height:100%; background-color:rgba(0,0,0,0.5); z-index:9998;"></div>

        <div id="spinner" style="display:none; position:fixed; top:50%; left:50%; transform: translate(-50%, -50%); z-index:9999;">
            <img src="img/Spinner.gif" alt="Procesando" />
        </div>

        <!-- Grupo 1: Vectores -->
        <div class="card">
            <div class="card-header" style="background-color: #e04f5f;">
                Vectores - Favor de seleccionar la ruta del archivo:
            </div>
            <div class="card-body">
                 <div class="mb-3">
                     <asp:CheckBox ID="chkCSV" runat="server" Text="CSV" CssClass="form-check-input" AutoPostBack="true" OnCheckedChanged="chkCSV_CheckedChanged" />
                     <asp:CheckBox ID="chkXLS" runat="server" Text="XLS" CssClass="form-check-input" AutoPostBack="true" OnCheckedChanged="chkXLS_CheckedChanged" />
                     Formato de archivo - Favor de seleccionar el formato del archivo a procesar
                 </div>
                <div class="mb-3">
                     
                    <asp:CheckBox ID="chMDValmer" runat="server" class="form-check-input" />
                    <asp:Label ID="label1" runat="server" Text="Vector MD Valmer" CssClass="form-label"></asp:Label>
                     
                    <div class="input-group">
                        <asp:FileUpload ID="fuArchivoExcelMDValmer" runat="server" Width="1000px" />
                    </div>
                </div>

                <div class="mb-3">
                   
                </div>

                <div class="mb-3">

                    <asp:CheckBox ID="chVectorPip" runat="server" class="form-check-input" />
                    <asp:Label ID="label2" runat="server" Text="Vector Analitico PiP (Formato .xls)" CssClass="form-label"></asp:Label>

                    <div class="input-group">
                        <asp:FileUpload ID="FileUploadVecPIP" runat="server" OnChanged="fuArchivoExcel_SelectedIndexChanged" />  
                        <%--<asp:TextBox ID="txFilePiP" runat="server" Enabled="false" CssClass="form-control"></asp:TextBox>--%>
                        <%--<asp:Button ID="btSelArchivoPiP" runat="server" Text="..." OnClick="btSelArchivoPiP_Click" CssClass="btn btn-secondary" />--%>
                                              
                    </div>
                </div>

                <div class="mb-3">
                    
                </div>
            </div>
        </div>

        <br />

        <!-- Grupo 2: Aladdin -->
        <div class="card">
            <div class="card-header" style="background-color: #e04f5f;">
                Aladdin - Favor de seleccionar la ruta del archivo:
            </div>
            <div class="card-body">
                <div class="mb-3">
                    
                        <asp:CheckBox ID="chArcPositions" runat="server" class="form-check-input" />
                        <asp:Label ID="label3" runat="server" Text="Archivo Positions (Layouts)" CssClass="form-label"></asp:Label>

                    <div class="input-group">
                        <asp:FileUpload ID="FileUploadPositionAladdin" runat="server" OnChanged="fuArchivoExcel_SelectedIndexChanged" />
                        <%--<asp:TextBox ID="txArchPosition" runat="server" Enabled="false" CssClass="form-control" Text="C:\Users\SAMMX\Documents\BAU\IMSS\IMSS Archivos de Carga\positions-downloadReport2025_03_15 00_10_50.xlsx"></asp:TextBox>--%>
                        <%--<asp:Button ID="btRutaPosition" runat="server" Text="..." OnClick="btRutaPosition_Click" CssClass="btn btn-secondary" />--%>
                        
                        
                    </div>
                </div>

                <div class="mb-3">
                    
                </div>

                <div class="mb-3">

                        <asp:CheckBox ID="chArchTrades" runat="server" class="form-check-input" />
                        <asp:Label ID="label4" runat="server" Text="Archivo Trades (Custodio)" CssClass="form-label"></asp:Label>

                    <div class="input-group">
                        <asp:FileUpload ID="FileUploadTrades" runat="server" OnChanged="fuArchivoExcel_SelectedIndexChanged" />
                        <%--<asp:TextBox ID="txArchTradesCustodio" runat="server" Enabled="false" CssClass="form-control" Text="C:\Users\SAMMX\Documents\BAU\IMSS\IMSS Archivos de Carga\trades-downloadReport2025_03_14 23_52_50.xlsx" ></asp:TextBox>--%>
                        <%--<asp:Button ID="btnSelTrades" runat="server" Text="..." OnClick="btnSelTrades_Click" CssClass="btn btn-secondary" />--%>                      
                        
                    </div>
                </div>

                <div class="mb-3">
                    
                </div>
            </div>
        </div>

        <br />

        <!-- Grupo 3: IMSS -->
        <div class="card">
            <div class="card-header" style="background-color: #e04f5f;">
                IMSS - Layouts
            </div>
            <div class="card-body">
                <div class="mb-3">
                    <asp:CheckBox ID="chkPosicion" runat="server" Text="Posición" Checked="false" class="form-check-input" />
                    <asp:CheckBox ID="chkTrades" runat="server" Text="Trades" Checked="false" class="form-check-input" />
                    <asp:CheckBox ID="chkValuada" runat="server" Text="Excel Posición Valuada" Checked="false" class="form-check-input" />
                    <asp:CheckBox ID="chkBBVA" runat="server" Text="Custodio BBVA" Checked="false" class="form-check-input" />
                    <asp:CheckBox ID="chkS3" runat="server" Text="Custodio S3" Checked="false" class="form-check-input" />
                    
                </div>
                 <div class="mb-3">
                     <asp:CheckBox ID="chkS3Comp" runat="server" Text="Custodio S3 Complementario" Checked="false" class="form-check-input" />
                 </div>
                <%--<div class="mb-3">
                    <asp:TextBox ID="txLayouts" runat="server" Enabled="false" CssClass="form-control" Text="C:\Users\SAMMX\Documents\BAU\IMSS\IMSS Archivos de Carga\Generados\"></asp:TextBox>
                    <asp:Button ID="btnSelCarpetaSalida" runat="server" Text="..." OnClick="btnSelCarpetaSalida_Click" CssClass="btn btn-secondary mt-2" />
                </div>--%>
            </div>
        </div>

        <br>
        
        <!-- Botón principal con clase de Bootstrap -->
        <div class="text-center">
            <asp:Button ID="btnProcesar" runat="server" Text="Procesar y Generar" OnClick="btnProcesar_Click" CssClass="btn btn-primary" />
        </div>
        


    </div> <!-- Fin del contenedor principal -->

</asp:Content>
