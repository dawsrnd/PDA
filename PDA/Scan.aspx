<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Scan.aspx.cs" Inherits="PDA.Scan" %>


<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="UTF-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1, shrink-to-fit=no" />
    <meta content="ie=edge" />
    <link href="css/bootstrap.min.css" rel="stylesheet" />
    <link href="css/Styles.css" rel="stylesheet" />
    <style>
        tr {
            height: 10px;
        }
    </style>
    <title>Scan</title>
</head>
<body>
    <form id="form1" runat="server" autocomplete="off">
        <asp:HiddenField ID="hdfEstacion" runat="server" />
        <asp:HiddenField ID="hdfOrden" runat="server" />
        <asp:HiddenField ID="hdfTipo" runat="server" />
        <asp:Panel ID="panelPage" runat="server" Visible="false">
            <div class="container-240">
                <table style="margin-left: 2px; margin-top: 2px; border-collapse: separate; margin-bottom:2px;" class="table">
                    <tbody>
                        <tr style="background-color: #0073b7;">
                            <td>
                                <asp:Label ID="lblTotal" runat="server" ForeColor="White"></asp:Label>
                            </td>
                            <td style="text-align: right;">
                                <asp:Label ID="lblShift" runat="server" ForeColor="White"></asp:Label>
                            </td>
                        </tr>
                    </tbody>
                </table>
                <table style="margin-left: 2px; border-collapse: separate;" class="table table-bordered">
                    <tbody>
                        <tr>
                            <td>
                                <asp:DropDownList ID="ddl_linea" Width="200px" runat="server" AppendDataBoundItems="true" AutoPostBack="true" OnSelectedIndexChanged="ddl_linea_SelectedIndexChanged" />
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:TextBox ID="txtEmpleado" Width="190px" runat="server" OnTextChanged="txtEmpleado_TextChanged" placeholder="Numero"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:GridView ID="gv_estacion" runat="server"
                                    CssClass="table table-bordered font-small"
                                    BorderStyle="None"
                                    AutoGenerateColumns="false"
                                    EmptyDataText="No results"
                                    OnSelectedIndexChanged="gv_estacion_SelectedIndexChanged"
                                    DataKeyNames="orden,estacion,tipo">
                                    <Columns>
                                        <asp:BoundField ItemStyle-Width="33%" ItemStyle-HorizontalAlign="Center" NullDisplayText=" " ReadOnly="true" ItemStyle-Wrap="false" DataField="estacion" HeaderText="Estaci&oacute;n" />
                                        <asp:BoundField ItemStyle-Width="33%" ItemStyle-HorizontalAlign="Center" NullDisplayText=" " ReadOnly="true" ItemStyle-Wrap="false" ItemStyle-CssClass="font-small-table" DataField="tipo" HeaderText="Tipo" />
                                        <asp:ButtonField Text="Elegir" CommandName="Select" />
                                    </Columns>
                                </asp:GridView>
                            </td>
                        </tr>
                    </tbody>
                </table>
            </div>
        </asp:Panel>
    </form>
</body>
</html>
