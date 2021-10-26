<%@ Page Title="Reporte por linea" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="PerLine.aspx.cs" Inherits="PDA.PerLine" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        table tr a {
            opacity: 0;
            background: none;
            border: none;
            padding: 0;
            font: inherit;
            cursor: pointer;
            outline: inherit;
        }

        table tr:hover a {
            opacity: 1;
            color: red;
        }

        table a:focus {
            opacity: 1;
            outline: inherit;
        }
    </style>
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
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="d-md-flex">
        <div class="p-2 w-100">
            <div class="card mt-2 shadow-1">
                <div class="card-body">
                    <div class="d-md-flex align-items-end">
                        <div class="p-2">
                            <label class="form-label">Tipo</label>
                            <asp:DropDownList ID="ddl_type_report" runat="server" CssClass="form-select" AppendDataBoundItems="true">
                            </asp:DropDownList>
                        </div>
                        <div class="p-2">
                            <label class="form-label">Desde</label>
                            <asp:TextBox ID="txtFrom" TextMode="Date" runat="server" CssClass="form-control"></asp:TextBox>
                        </div>
                        <div class="p-2">
                            <label class="form-label">Hasta</label>
                            <asp:TextBox ID="txtTo" TextMode="Date" runat="server" CssClass="form-control"></asp:TextBox>
                        </div>
                        <div class="p-2">
                            <label class="form-label">Turno</label>
                            <asp:DropDownList ID="ddl_shift" runat="server" CssClass="form-select">
                                <asp:ListItem Text="Primero" Selected="True" Value="1"></asp:ListItem>
                                <asp:ListItem Text="Segundo" Value="2"></asp:ListItem>
                                <asp:ListItem Text="Tercero" Value="3"></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                        <div class="p-2 me-auto">
                            <asp:LinkButton ID="btnShow" runat="server" CssClass="btn blue-KL" OnClick="btnShow_Click"><i class="fas fa-search me-2"></i>Mostrar</asp:LinkButton>
                        </div>
                        <div class="p-2 ">
                            <asp:LinkButton ID="btnExcelDown" runat="server" CssClass="btn green-Excel" OnClick="btnExcelDown_Click" OnClientClick="progress();"><i class="fas fa-arrow-down me-2"></i>EXCEL DOWN</asp:LinkButton>
                        </div>
                    </div>
                    <div class="d-md-flex">
                        <div class="p-2 w-100">
                            <div class="container-fluid mt-5">
                                <div class="row">
                                    <div class="table-responsive mb-3">
                                        <div class="table-wrapper-scroll-y my-custom-scrollbar table-bom myGrid ">
                                            <asp:GridView ID="gv_report" runat="server" AutoGenerateColumns="true"
                                                EmptyDataText="No records result."
                                                ShowHeader="true"
                                                CssClass="table-bordered table table-hover table-sm" BorderStyle="None">
                                                <HeaderStyle CssClass="StickyHeader table-light" />
                                            </asp:GridView>
                                            <asp:GridView ID="gv_prestados" runat="server" AutoGenerateColumns="false"
                                                EmptyDataText="No records result."
                                                DataKeyNames="IDX"
                                                ShowHeader="true"
                                                CssClass="table-bordered table table-hover table-sm" BorderStyle="None">
                                                <HeaderStyle CssClass="StickyHeader table-light" />
                                                <Columns>
                                                    <asp:BoundField ItemStyle-Width="25%" ItemStyle-HorizontalAlign="Left" NullDisplayText=" " ReadOnly="true" ItemStyle-Wrap="false" DataField="Numero" HeaderText="Numero" />
                                                    <asp:BoundField ItemStyle-Width="25%" ItemStyle-HorizontalAlign="Left" NullDisplayText=" " ReadOnly="true" ItemStyle-Wrap="false" DataField="Origen" HeaderText="Origen" />
                                                    <asp:BoundField ItemStyle-Width="25%" ItemStyle-HorizontalAlign="Left" NullDisplayText=" " ReadOnly="true" ItemStyle-Wrap="false" DataField="Destino" HeaderText="Destino" />
                                                    <asp:BoundField ItemStyle-Width="15%" ItemStyle-HorizontalAlign="Left" NullDisplayText=" " ReadOnly="true" ItemStyle-Wrap="false" DataField="Fecha" HeaderText="Fecha" />
                                                    <asp:BoundField ItemStyle-Width="10%" ItemStyle-HorizontalAlign="Left" NullDisplayText=" " ReadOnly="true" ItemStyle-Wrap="false" DataField="Turno" HeaderText="Turno" />
                                                    <asp:TemplateField ItemStyle-Width="5%" ItemStyle-HorizontalAlign="Center" ItemStyle-Wrap="false" HeaderText="Opciones">
                                                        <ItemTemplate>
                                                            <asp:LinkButton ID="btnDelete" runat="server" CommandArgument='<%#Eval("IDX")%>' OnCommand="btnDelete_Command" OnClientClick="Confirm('Desea eliminar al empleado?');">
                                                               <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" fill="currentColor" class="bi bi-trash text-danger" viewBox="0 0 16 16">
                                                                  <path d="M5.5 5.5A.5.5 0 0 1 6 6v6a.5.5 0 0 1-1 0V6a.5.5 0 0 1 .5-.5zm2.5 0a.5.5 0 0 1 .5.5v6a.5.5 0 0 1-1 0V6a.5.5 0 0 1 .5-.5zm3 .5a.5.5 0 0 0-1 0v6a.5.5 0 0 0 1 0V6z"/>
                                                                  <path fill-rule="evenodd" d="M14.5 3a1 1 0 0 1-1 1H13v9a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2V4h-.5a1 1 0 0 1-1-1V2a1 1 0 0 1 1-1H6a1 1 0 0 1 1-1h2a1 1 0 0 1 1 1h3.5a1 1 0 0 1 1 1v1zM4.118 4 4 4.059V13a1 1 0 0 0 1 1h6a1 1 0 0 0 1-1V4.059L11.882 4H4.118zM2.5 3V2h11v1h-11z"/>
                                                                </svg>
                                                            </asp:LinkButton>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                </Columns>
                                            </asp:GridView>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <div class="modal fade " id="modalAnimation" tabindex="-1" aria-labelledby="modalAnimationLabel" aria-hidden="true">
        <div class="modal-dialog modal-dialog-centered">
            <div class="modal-content bg-transparent shadow-0">
                <div class="modal-body">
                    <div class="d-flex justify-content-center">
                        <div class="p-2">
                            <div class="sk-chase text-white">
                                <div class="sk-chase-dot"></div>
                                <div class="sk-chase-dot"></div>
                                <div class="sk-chase-dot"></div>
                                <div class="sk-chase-dot"></div>
                                <div class="sk-chase-dot"></div>
                                <div class="sk-chase-dot"></div>
                            </div>

                        </div>
                    </div>

                    <div class="d-flex justify-content-center mt-3">
                        <div class="p-2">
                            <h2 class="text-white">Cargando</h2>
                        </div>
                    </div>
                </div>

            </div>
        </div>
    </div>
    <script type="text/javascript" src="https://code.jquery.com/jquery-3.5.1.min.js"></script>
    <script type="text/javascript" src="https://cdnjs.cloudflare.com/ajax/libs/mdb-ui-kit/3.6.0/mdb.min.js"></script>
    <script type="text/javascript" src="https://cdnjs.cloudflare.com/ajax/libs/toastr.js/latest/toastr.min.js"></script>

    <script src='<%= ResolveUrl("~/js/functions.js") %>'></script>
    <script src='<%= ResolveUrl("~/js/modalLoading.js") %>'></script>
    <script>
        //VARIABLES
        var txtTo = document.getElementById('ContentPlaceHolder1_txtTo');
        var ddl_shift = document.getElementById('ContentPlaceHolder1_ddl_shift');
        var ddl_type_report = document.getElementById('ContentPlaceHolder1_ddl_type_report');
        EnableOrDisable();

        //ONITEM CHANGE SELECT
        ddl_type_report.addEventListener('change', function () {
            EnableOrDisable();
        });


        function EnableOrDisable() {
            if (ddl_type_report.value == 204) {
                txtTo.disabled = false;
                ddl_shift.disabled = true;
            }
            else {
                txtTo.disabled = true;
                txtTo.value = '';
                ddl_shift.disabled = false;
            }
        }
    </script>
</asp:Content>
