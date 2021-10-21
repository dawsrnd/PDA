<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Lend.aspx.cs" Inherits="PDA.Lend" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="UTF-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1, shrink-to-fit=no" />
    <meta content="ie=edge" />
    <link href="css/bootstrap.min.css" rel="stylesheet" />
    <link href="css/Styles.css" rel="stylesheet" />
    <script type="text/javascript">
        function Confirm(message) {
            var confirm_value = document.createElement("INPUT");
            confirm_value.type = "hidden";
            confirm_value.name = "confirm_value";
            if (confirm(message)) {
                confirm_value.value = "Si";
            } else {
                confirm_value.value = "No";
            }
            document.forms[0].appendChild(confirm_value);
        }
    </script>
    <style>
        tr {
            height: 10px;
        }
    </style>
    <title>Prestados</title>
</head>
<body>
    <form id="form1" runat="server" autocomplete="off">
        <asp:Panel ID="panelPage" runat="server" Visible="false">
            <div class="container-240">
                <table style="margin-left: 2px; margin-top: 2px; border-collapse: separate; margin-bottom: 2px;" class="table">
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
                                <label>Destino</label>
                                <asp:DropDownList ID="ddl_destino" Width="200px" runat="server" AppendDataBoundItems="true">
                                    <asp:ListItem>1</asp:ListItem>
                                </asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <label>Empleado</label>
                                <asp:TextBox ID="txtboxEmpleado" Width="190px" runat="server" OnTextChanged="txtboxEmpleado_TextChanged"></asp:TextBox>
                            </td>
                        </tr>
                    </tbody>
                </table>
                <asp:LinkButton ID="btnSendEmail" CssClass="btn btn-primary btn-small btn-block" OnClientClick="Confirm('Desea finalizar y enviar los resultados por EMAIL?');" OnClick="btnSendEmail_Click" runat="server">Enviar E-Mail</asp:LinkButton>
            </div>
        </asp:Panel>
    </form>
</body>
</html>
