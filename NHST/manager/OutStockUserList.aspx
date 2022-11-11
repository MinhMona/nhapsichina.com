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
    <script>       
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
