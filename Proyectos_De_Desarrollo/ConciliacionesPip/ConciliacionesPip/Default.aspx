<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_Default" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Conciliaciones</title>
    <link href="Content/bootstrap.min.css" rel="stylesheet" />
    <link href="Content/Site.css" rel="stylesheet" />
    <script src="Scripts/jquery-3.4.1.min.js"></script>
    <script src="Scripts/bootstrap.min.js"></script>
</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePartialRendering="true" />

        <div class="container mt-5">
            <div class="row mb-3">
                <div class="col-md-12">
                    <h2 class="text-center">Carga de Archivos Conciliación</h2>
                    <hr />
                </div>
            </div>

            <div class="row">
                <!-- Panel de botones -->
                <div class="col-md-4">
                    <asp:Button ID="btnProcessPosicionAladdin" runat="server" Text="Cargar Posición Aladdin"
                        CssClass="btn btn-primary btn-block mb-2"
                        OnClick="btnProcessPosicionAladdin_Click"
                        CausesValidation="false" UseSubmitBehavior="false" />

                    <asp:Button ID="btnProcessPosicionAladdin_Reporto" runat="server" Text="Cargar Posición Aladdin Reporto"
                        CssClass="btn btn-success btn-block mb-2"
                        OnClick="btnProcessPosicionAladdin_Reporto_Click"
                        CausesValidation="false" UseSubmitBehavior="false" />

                    <asp:FileUpload ID="fuArchivoValuaciones" runat="server" CssClass="form-control mb-2" />
                    <asp:Button ID="btnCargarValuacionesUnificadas" runat="server"
                        Text="Cargar Valuaciones BSMX y S3"
                        CssClass="btn btn-primary btn-block"
                        OnClick="btnCargarValuacionesUnificadas_Click"
                        CausesValidation="false" UseSubmitBehavior="false" />

                    <asp:Button ID="btnProcessVectAnali" runat="server" Text="Cargar Vector Analítico"
                        CssClass="btn btn-info btn-block mb-2"
                        OnClick="btnProcessVectAnali_Click"
                        CausesValidation="false" UseSubmitBehavior="false" />

                    <asp:Button ID="btnConfirmarCarga" runat="server" Text="Confirmar Carga"
                        CssClass="btn btn-dark btn-block mt-4"
                        OnClick="btnConfirmarCarga_Click"
                        Visible="false"
                        CausesValidation="false" UseSubmitBehavior="false" />
                </div>

                <!-- Panel de vista previa de archivos -->
                <div class="col-md-8">
                    <asp:UpdatePanel ID="upArchivosPreview" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <asp:Panel ID="pnlArchivosPreview" runat="server" CssClass="border rounded p-3 bg-light">
                                <h4>Archivos a cargar:</h4>
                                <asp:Label ID="lblArchivoEnProceso" runat="server" CssClass="font-weight-bold text-primary mb-3"></asp:Label>

                                <asp:Repeater ID="rptArchivos" runat="server">
                                    <HeaderTemplate>
                                        <table class="table table-striped table-bordered">
                                            <thead class="thead-dark">
                                                <tr>
                                                    <th>Archivo</th>
                                                    <th>Fuente</th>
                                                    <th>Número de registros</th>
                                                    <th>Progreso</th>
                                                </tr>
                                            </thead>
                                            <tbody>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <tr>
                                            <td><%# Eval("Nombre") %></td>
                                            <td><%# Eval("Fuente") %></td>
                                            <td><%# Eval("Registros") %></td>
                                            <td>
                                                <div class="progress">
                                                    <div class="progress-bar" role="progressbar"
                                                         style='<%# "width:" + Eval("Progreso") + "%;" %>'
                                                         aria-valuenow='<%# Eval("Progreso") %>'
                                                         aria-valuemin="0" aria-valuemax="100">
                                                        <%# Eval("Progreso") %>%
                                                    </div>
                                                </div>
                                            </td>
                                        </tr>
                                    </ItemTemplate>
                                    <FooterTemplate>
                                            </tbody>
                                        </table>
                                    </FooterTemplate>
                                </asp:Repeater>
                            </asp:Panel>

                            <asp:Panel ID="pnlResultados" runat="server" CssClass="alert alert-success mt-3" Visible="false">
                                <asp:Label ID="lblResultados" runat="server" Text="Resultados de carga aparecerán aquí." />
                            </asp:Panel>

                            <!-- Timer para procesar archivos uno por uno -->
                            <asp:Timer ID="tmrProcesoArchivos" runat="server" Interval="1500" OnTick="tmrProcesoArchivos_Tick" Enabled="false" />
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="tmrProcesoArchivos" EventName="Tick" />
                        </Triggers>
                    </asp:UpdatePanel>
                </div>
            </div>
        </div>
    </form>
</body>
</html>
