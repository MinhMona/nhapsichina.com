<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/manager/adminMasterNew.Master" CodeBehind="import_smallpackage.aspx.cs" Inherits="NHST.manager.import_smallpackage" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Import Namespace="NHST.Controllers" %>
<%@ Import Namespace="NHST.Models" %>
<%@ Import Namespace="NHST.Bussiness" %>
<asp:Content runat="server" ContentPlaceHolderID="head">
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="ContentPlaceHolder1">
    <main id="main" class="main-full">
        <div class="row">
            <div class="content-wrapper-before bg-dark-gradient"></div>
            <div class="col s12 page-title">
                <div class="card-panel">
                    <div class="title-flex">
                        <h4 class="title no-margin">Import Mã Vận Đơn Kho Trung Quốc</h4>
                    </div>
                </div>
            </div>
            <div class="list-staff col s12 m12 l12 section">
                <div class="list-table card-panel">
                    <div class="row">
                        <div class="filter">
                            <div class="select-bao input-field col s6 m4 l4">
                                <div class="input-field inline">
                                    <asp:DropDownList ID="ddlBigpack" runat="server" CssClass="select2"
                                        DataValueField="ID" DataTextField="">
                                    </asp:DropDownList>
                                </div>
                                <a href="#addBadge" class="btn modal-trigger waves-effect">Tạo mới bao lớn</a>
                            </div>
                            <div class="input-field col s6 m4 l12">
                                <div class="lb">Chọn file</div>
                                <asp:FileUpload ID="FileUpload1" runat="server" />
                            </div>
                            <div class="input-field col s6 m4 l2">
                                <asp:Button ID="btnImport" runat="server" CssClass="btn primary-btn" Text="Import File" ValidationGroup="n" OnClick="btnImport_Click"></asp:Button>
                            </div>
                            <div class="input-field col s6 m4 l2">
                                <asp:Button ID="btnExport" runat="server" CssClass="btn primary-btn" Text="Xuất file mẫu" ValidationGroup="n" OnClick="btnExport_Click"></asp:Button>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <!-- Modal Structure -->
        <div id="addBadge" class="modal modal-big add-package">
            <div class="modal-hd">
                <span class="right"><i class="material-icons modal-close right-align">clear</i></span>
                <h4 class="no-margin center-align">Thêm bao lớn mới</h4>
            </div>
            <div class="modal-bd">
                <div class="row">

                    <div class="input-field col s12 m4">
                        <asp:TextBox runat="server" ID="txtPackageCode" CssClass="validate" placeholder="Mã bao hàng"></asp:TextBox>
                        <span class="error-info-show">
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtPackageCode" Display="Dynamic"
                                ErrorMessage="Không để trống" SetFocusOnError="true"></asp:RequiredFieldValidator>
                        </span>
                        <label for="kg_weight">Mã bao hàng</label>
                    </div>

                    <div class="input-field col s12 m4">
                        <telerik:RadNumericTextBox runat="server" CssClass="validate" Skin="MetroTouch"
                            ID="pWeight" MinValue="0" NumberFormat-DecimalDigits="2"
                            NumberFormat-GroupSizes="3" Width="100%" placeholder="Cân (kg)" Value="0">
                        </telerik:RadNumericTextBox>
                        <label for="kg_weight" class="active">Cân nặng (kg)</label>
                    </div>
                    <div class="input-field col s12 m4">
                        <telerik:RadNumericTextBox runat="server" CssClass="validate" Skin="MetroTouch"
                            ID="pVolume" MinValue="0" NumberFormat-DecimalDigits="2"
                            NumberFormat-GroupSizes="3" Width="100%" placeholder="Khối (m3)" Value="0">
                        </telerik:RadNumericTextBox>
                        <label for="m2_weigth" class="active">Khối (m<sup>3</sup>)</label>

                    </div>
                </div>
            </div>
            <div class="modal-ft">
                <div class="ft-wrap center-align">
                    <a href="javascript:;" onclick="AddBigPackage()" class="modal-action btn modal-close waves-effect waves-green mr-2 submit-button">Thêm</a>
                    <a href="javascript:;" class="modal-action btn orange darken-2 modal-close waves-effect waves-green ml-2">Hủy</a>
                </div>

            </div>
        </div>

    </main>
    <script type="text/javascript">
        function isEmpty(str) {
            return !str.replace(/^\s+/g, '').length; // boolean (`true` if field is empty)
        }
        function AddBigPackage() {
            var packageCode = $("#<%=txtPackageCode.ClientID%>").val();
            var weight = $("#<%=pWeight.ClientID%>").val();
            var Volume = $("#<%=pVolume.ClientID%>").val();
            if (!isEmpty(packageCode)) {
                $.ajax({
                    type: "POST",
                    url: "/manager/import_smallpackage.aspx/AddBigPackage",
                    data: "{PackageCode:'" + packageCode + "',Weight:'" + weight + "',Volume:'" + Volume + "'}",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (msg) {
                        var data = msg.d;
                        if (data != null) {
                            if (data != "existCode") {
                                loadBigPackage(parseInt(data));
                                location.reload(true)
                            }
                            else {
                                alert('Mã bao hàng đã tồn tài.');
                            }
                        }
                        else {
                            alert('Có lỗi trong quá trình xử lý.');
                        }
                    }
                })
            }
            else {
                alert('Không được để trống mã bao hàng!');
            }
        }
        function loadBigPackage(value) {
            $.ajax({
                type: "POST",
                url: "/manager/import_smallpackage.aspx/GetBigPackage",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (msg) {
                    var data = JSON.parse(msg.d);
                    var html = " <option value=\"0\">Chọn bao lớn</option>";
                    if (data != null) {
                        for (var i = 0; i < data.length; i++) {
                            var item = data[i];
                            html += " <option value=\"" + item.ID + "\">" + item.PackageCode + "</option>";
                        }
                    }
                    $("#ddlBigpack").html(html);
                    $("#ddlBigpack").val(value);
                    $('select').formSelect();
                }
            })
        }
    </script>
</asp:Content>