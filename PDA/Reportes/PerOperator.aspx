<%@ Page Title="Reporte por operador" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="PerOperator.aspx.cs" Inherits="PDA.PerOperator" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="d-md-flex">
        <div class="p-2 w-100">
            <div class="card mt-2 shadow-1">
                <div class="card-body">
                    <div class="d-md-flex align-items-end">
                        <div class="p-2">
                            <div class="form-outline">
                                <asp:TextBox ID="txtEmpleado" runat="server" class="form-control"></asp:TextBox>
                                <label class="form-label" for="form12">Numero de empleado</label>
                            </div>

                        </div>
                        <div class="p-2">
                            <label class="form-label">Desde</label>
                            <asp:TextBox ID="txtFromOperador" TextMode="Date" runat="server" CssClass="form-control"></asp:TextBox>
                        </div>
                        <div class="p-2">
                            <label class="form-label">Hasta</label>
                            <asp:TextBox ID="txtToOperador" TextMode="Date" runat="server" CssClass="form-control"></asp:TextBox>
                        </div>
                        <div class="p-2 me-auto">
                            <asp:LinkButton ID="btnShowOperador" runat="server" CssClass="btn blue-KL" OnClick="btnShowOperador_Click" OnClientClick="progress();"><i class="fas fa-search me-2"></i>Mostrar</asp:LinkButton>
                        </div>
                    </div>
                    <div class="d-md-flex mt-5">
                        <div class="p-2 me-auto">
                            <h5 class="form-label text-uppercase font-weight-bold lead mb-2 text-center">Informaci&oacute;n</h5>
                            <div class="d-md-flex mt-5">
                                <div class="p-2">
                                    <div class="avatar mx-4 w-100 white d-flex ">
                                        <asp:Image ID="imgTrabajador" Visible="false" runat="server" CssClass="rounded-5 img-fluid z-depth-1" Width="150px" Height="140px" ImageUrl="~/src/no-image-available.jpg" />
                                    </div>
                                </div>
                                <div class="p-2">
                                    <asp:ListView ID="ListView1" runat="server">
                                        <ItemTemplate>
                                            <table>
                                                <tbody>
                                                    <tr>
                                                        <td>
                                                            <label class="form-label text-uppercase lead mb-2"><%# Eval("nombre") %></label>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>
                                                            <label class="form-label font-italic lead mb-2"><%# Eval("puesto") %></label>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>
                                                            <label class="form-label mb-2">Fecha de ingreso: <%# Eval("ingreso", "{0:dd/MM/yyyy}") %></label>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>
                                                            <label class="form-label mb-2">Turno: <%# Eval("turno") %></label>
                                                        </td>
                                                    </tr>
                                                </tbody>
                                            </table>
                                        </ItemTemplate>
                                        <EmptyDataTemplate>
                                            <p>Sin informacion.</p>
                                        </EmptyDataTemplate>
                                    </asp:ListView>
                                </div>
                            </div>
                        </div>
                        <div class="p-2">
                            <h5 class="form-label text-uppercase font-weight-bold lead mb-2 text-center">Defectos</h5>
                            <div class="d-md-flex mt-5">
                                <div class="p-2 w-100">
                                    <asp:GridView ID="gv_defectos2" runat="server"
                                        CssClass="table-small"
                                        BorderStyle="Dotted"
                                        AutoGenerateColumns="true"
                                        EmptyDataText="No results">
                                        <HeaderStyle CssClass="StickyHeader table-light" />
                                    </asp:GridView>
                                </div>
                            </div>
                            <div class="d-md-flex mt-2">
                                <div class="p-2 ">
                                    <asp:GridView ID="gv_defectos" runat="server"
                                        CssClass="table-small"
                                        BorderStyle="Dotted"
                                        AutoGenerateColumns="true"
                                        EmptyDataText="No results">
                                        <HeaderStyle HorizontalAlign="Center" CssClass="StickyHeader table-light" />
                                        <RowStyle HorizontalAlign="Center" />
                                    </asp:GridView>
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
</asp:Content>
