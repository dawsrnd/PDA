<%@ Page Title="Reporte por estacion" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="PerStation.aspx.cs" Inherits="PDA.Reportes.PerStation" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="d-md-flex">
        <div class="p-2 w-100">
            <div class="card mt-2 shadow-1">
                <div class="card-body">
                    <div class="d-md-flex align-items-end">
                        <div class="p-2">
                            <label class="form-label">Linea</label>
                            <asp:DropDownList ID="ddl_linea" runat="server" CssClass="form-select" AppendDataBoundItems="true" AutoPostBack="true" OnSelectedIndexChanged="ddl_linea_SelectedIndexChanged">
                            </asp:DropDownList>
                        </div>
                        <div class="p-2">
                            <label class="form-label">Linea</label>
                            <asp:DropDownList ID="ddl_station" runat="server" CssClass="form-select" AppendDataBoundItems="true">
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
                    </div>
                    <div class="d-md-flex mt-5">
                        <div class="p-2 w-100">
                            <h5 class="form-label text-uppercase font-weight-bold lead mb-5 text-center">Informaci&oacute;n</h5>
                            <asp:GridView ID="gv_station" runat="server"
                                CssClass="table table-bordered "
                                BorderStyle="Dotted"
                                AutoGenerateColumns="true"
                                EmptyDataText="No results">
                                <HeaderStyle CssClass="StickyHeader table-light" />
                            </asp:GridView>
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
</asp:Content>
