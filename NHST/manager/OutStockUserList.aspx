<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/manager/adminMasterNew.Master" CodeBehind="OutStockUserList.aspx.cs" Inherits="NHST.manager.OutStockUserList" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Import Namespace="NHST.Controllers" %>
<%@ Import Namespace="NHST.Models" %>
<%@ Import Namespace="NHST.Bussiness" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:Button Style="display: none" UseSubmitBehavior="false" ID="btnSearch" runat="server" OnClick="btnSearch_Click" />
    <div id="main" class="main-full">
        <div class="row">
            <div class="content-wrapper-before bg-dark-gradient"></div>
            <div class="page-title">
                <div class="card-panel">
                    <h4 class="title no-margin" style="display: inline-block;">Danh sách yêu cầu giao</h4>

                    <div class="right-action">
                        <a href="#" class="btn" id="filter-btn">Bộ lọc</a>
                    </div>
                    <div class="right-action">
                        <a href="#addStaff" class="btn  modal-trigger waves-effect">Thêm yêu cầu giao</a>
                    </div>
                    <div class="clearfix"></div>
                    <div class="filter-wrap" style="display: block">
                        <div class="row mt-2 pt-2">
                            <div class="search-name input-field col s12 l6">
                                <asp:TextBox ID="search_name" name="txtSearchName" type="text" onkeypress="myFunction()" runat="server" />
                                <label for="search_name"><span>Usename</span></label>
                            </div>
                            <div class="input-field col s12 l6">
                                <asp:DropDownList runat="server" ID="ddlStatus">
                                    <asp:ListItem Text="Tất cả" Value="-1"></asp:ListItem>
                                    <asp:ListItem Text="Chưa xử lý" Value="0"></asp:ListItem>
                                    <asp:ListItem Text="Đã xử lý" Value="2"></asp:ListItem>
                                </asp:DropDownList>
                                <label for="status">Trạng thái</label>
                            </div>
                            <div class="input-field col s12 l6">
                                <asp:TextBox ID="rFD" runat="server" placeholder="" Type="text" class="datetimepicker from-date"></asp:TextBox>
                                <label>Từ ngày</label>
                            </div>
                            <div class="input-field col s12 l6">
                                <asp:TextBox runat="server" Type="text" placeholder="" ID="rTD" class="datetimepicker to-date"></asp:TextBox>
                                <label>Đến ngày</label>
                                <span class="helper-text" data-error="Vui lòng chọn ngày bắt đầu trước"></span>
                            </div>
                            <div class="col s12 right-align">
                                <span class="search-action btn">Lọc kết quả</span>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="list-staff col s12 section">
                <div class="list-table card-panel">
                    <table class="table responsive-table bordered highlight">
                        <thead>
                            <tr>
                                <th>ID</th>
                                <th>Username</th>
                                <th>Mã đơn hàng</th>
                                <th>Tiền cần
                                    <br />
                                    thanh toán</th>
                                <th>Mã vận đơn</th>
                                <th>Tổng kiện</th>
                                <th>Tổng cân</th>
                                <th>Trạng thái</th>
                                <th>Ghi chú</th>
                                <th>Ngày tạo</th>
                                <th>Thao tác</th>
                            </tr>
                        </thead>
                        <tbody>
                            <asp:Literal ID="ltr" runat="server" EnableViewState="false"></asp:Literal>
                        </tbody>
                    </table>
                    <div class="pagi-table float-right mt-2">
                        <%this.DisplayHtmlStringPaging1();%>
                    </div>
                    <div class="clearfix"></div>
                </div>
            </div>
        </div>
    </div>
    <asp:HiddenField ID="hdfID" runat="server" />
    <asp:HiddenField ID="hdfUserName" runat="server" />

    <div id="addStaff" class="modal">
        <div class="modal-hd">
            <span class="right"><i class="material-icons modal-close right-align">clear</i></span>
            <h4 class="no-margin center-align">Thêm yêu cầu giao</h4>
        </div>
        <div class="modal-bd">
            <div class="row">
                <div class="input-field col s12 m6">
                    <asp:TextBox runat="server" ID="txtMainOrderID" type="number"></asp:TextBox>
                    <label for="full_name">
                        Mã đơn hàng<asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidator1" ControlToValidate="txtMainOrderID" SetFocusOnError="true"
                            ValidationGroup="add" ErrorMessage="(*)" ForeColor="Red"></asp:RequiredFieldValidator></label>
                </div>

                <div class="table-code">
                    <div class="input-field col s12 m6">
                        <a href="javascript:;" class="btn add-product" style="display: inline-flex; height: 100%; align-items: center"><i class="material-icons">add</i><span>Mã vận đơn</span></a>
                    </div>
                </div>
                <div class="input-field col s12 m12">
                    <asp:TextBox runat="server" ID="txtNote" TextMode="MultiLine" type="text" CssClass="materialize-textarea"></asp:TextBox>
                    <label for="full_name">
                        Ghi chú<asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidator3" ControlToValidate="txtNote" SetFocusOnError="true"
                            ValidationGroup="add" ErrorMessage="(*)" ForeColor="Red"></asp:RequiredFieldValidator></label>
                </div>
            </div>
        </div>
        <div class="modal-ft">
            <div class="ft-wrap center-align">
                <a href="#!" class="modal-action btn modal-close waves-effect waves-green mr-2" onclick="AddRequest()">Thêm</a>
                <a class="modal-action btn orange darken-2 modal-close waves-effect waves-green ml-2">Hủy</a>
            </div>
        </div>
    </div>

    <asp:Button runat="server" ID="btnSave" Text="Lưu" CssClass="btn primary-btn" ValidationGroup="add" OnClick="btnSaveAdd_Click" UseSubmitBehavior="false" Style="display: none" />
    <asp:HiddenField ID="hdfCodeTransactionList" runat="server" />
    <asp:HiddenField ID="hdfCodeTransactionListMVD" runat="server" />

    <script>  
        function AddRequest() {
            var listmvd = "";
            $(".order-versionnew").each(function () {
                var code = $(this).find(".transactionCode").val();
                listmvd += code + "|";
            });
            $("#<%=hdfCodeTransactionListMVD.ClientID%>").val(listmvd);

            $("#<%=btnSave.ClientID%>").click();
        }

        $('.add-product').on('click', function () {
            debugger
            var tableHTML = $('.table-code');
                   <%-- var transactionCodeMainOrderCodeHTML = $("#<%=ddlMainOrderCode.ClientID%>").html().replace('selected', '');--%>
            var html = `
                <div class="order-versionnew" data-packageID="0">
                    <table class="table">
                        <tr>
                            <th scope="col" style=" width: 50%; ">
                                <div class="input-field col s12">
                                    <input class="transactionCode" type="text" value="">
                                    <label for="transactionCode">Mã vận đơn</label>
                                </div>
                            </th>
                            <th scope="col">
                                <div class="input-field col s12">
                                    <a href='javascript:;' onclick="deleteOrderCode($(this))" class=\"tooltipped\" data-position=\"top\" data-tooltip=\"Xóa\"><i class="material-icons valign-center">remove_circle</i></a>
                                </div>
                            </th>
                        </tr>
                    </table>
                </div>`;
            tableHTML.append(html);
            $('select').formSelect();
        });

        function deleteOrderCode(obj) {
            obj.parent().parent().remove();
        }
        function UpdateOutStock(ID) {
            var cf = confirm("Bạn muốn xác nhận yêu cầu này?");
            if (cf == true) {
                $.ajax({
                    type: "POST",
                    url: "/manager/OutStockUserList.aspx/UpdateStatus",
                    data: '{ID: "' + ID + '"}',
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (msg) {
                        var ret = msg.d;
                        if (ret != "none") {
                            if (ret == "ok") {
                                window.location.reload(true);
                            }
                            else
                                alert("Không thành công");
                        }
                        else
                            alert("Không thành công");
                    }
                })
            }
        }
        function myFunction() {
            if (event.which == 13 || event.keyCode == 13) {
                console.log($('#<%=search_name.ClientID%>').val());
                $('#<%=btnSearch.ClientID%>').click();
            }
        }
        $('.search-action').click(function () {
            console.log('dkm ngon');
            console.log($('#<%=search_name.ClientID%>').val());
            $('#<%=btnSearch.ClientID%>').click();
        })
        $(document).ready(function () {
            $('#search_name').autocomplete({
                data: {
                    "Apple": null,
                    "Microsoft": null,
                    "Google": 'https://placehold.it/250x250',
                    "Asgard": null
                },
            });
        });
    </script>
</asp:Content>
