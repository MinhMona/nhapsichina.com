<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="danh-sach-yeu-cau-giao.aspx.cs" MasterPageFile="~/UserMasterNew.Master" Inherits="NHST.danh_sach_yeu_cau_giao" %>

<asp:Content runat="server" ContentPlaceHolderID="head"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="ContentPlaceHolder1">
    <div id="main">
        <div class="row">
            <div class="content-wrapper-before blue-grey lighten-5"></div>
            <div class="col s12">
                <div class="container">
                    <div class="all">
                        <div class="card-panel mt-3 no-shadow">
                            <div class="row">
                                <div class="col s12">
                                    <div class="page-title mt-2 center-align">
                                        <h4>Danh sách yêu cầu giao</h4>
                                    </div>
                                </div>
                            </div>
                            <div class="row mt-2">
                                <div class="col s12">
                                    <table class="table responsive-table   highlight bordered  centered">
                                        <thead>
                                            <tr>
                                                <th>ID</th>
                                                <th>Mã đơn hàng</th>
                                                <th>Tiền cần
                                                <br />
                                                    thanh toán</th>
                                                <th>Tiền cần
                                                <br />
                                                    nạp thêm</th>
                                                <th>Mã vận đơn</th>
                                                <th>Tổng kiện</th>
                                                <th>Tổng cân</th>
                                                <th>Trạng thái</th>
                                                <th>Ngày tạo</th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                            <asp:Literal ID="ltr" runat="server"></asp:Literal>

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
                </div>
            </div>
        </div>
    </div>
</asp:Content>
