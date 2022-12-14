using MB.Extensions;
using NHST.Bussiness;
using NHST.Controllers;
using NHST.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using static NHST.WebService1;
using static Telerik.Web.UI.OrgChartStyles;

namespace NHST.manager
{
    public partial class OutStockUserList : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["userLoginSystem"] == null)
                {
                    Response.Redirect("/");
                }
                else
                {
                    string username_current = Session["userLoginSystem"].ToString();
                    tbl_Account ac = AccountController.GetByUsername(username_current);
                    if (ac.RoleID != 0 && ac.RoleID != 7 && ac.RoleID != 2)
                        Response.Redirect("/");
                    LoadData();
                }
            }
        }
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            string search = search_name.Text.Trim();
            string fd = "";
            string td = "";
            string stt = ddlStatus.SelectedValue;
            if (!string.IsNullOrEmpty(rFD.Text))
            {
                fd = rFD.Text.ToString();
            }
            if (!string.IsNullOrEmpty(rTD.Text))
            {
                td = rTD.Text.ToString();
            }
            Response.Redirect("OutStockUserList.aspx?&s=" + search + "&stt=" + stt + "&fd=" + fd + "&td=" + td);
        }
        public void LoadData()
        {
            int page = 0;
            Int32 Page = GetIntFromQueryString("Page");
            if (Page > 0)
            {
                page = Page - 1;
            }
            int status = -1;
            if (Request.QueryString["stt"] != null)
                status = Convert.ToInt32(Request.QueryString["stt"]);
            ddlStatus.SelectedValue = status.ToString();
            string search = "";
            if (!string.IsNullOrEmpty(Request.QueryString["s"]))
            {
                search = Request.QueryString["s"].ToString().Trim();
                search_name.Text = search;
            }
            string fd = Request.QueryString["fd"];
            if (!string.IsNullOrEmpty(fd))
                rFD.Text = fd;
            string td = Request.QueryString["td"];
            if (!string.IsNullOrEmpty(td))
                rTD.Text = td;
            var la = OutStockSessionController.GetDataOutStockUser(search, status, fd, td, page, 20);
            int Total = OutStockSessionController.GetTotalDataUser(search, fd, td, status);
            pagingall(la, Total, 20);
        }

        #region Pagging
        public void pagingall(List<OutStockSessionController.OutStockNew> acs, int total, int PageSize)
        {
            if (total > 0)
            {
                int TotalItems = total;
                if (TotalItems % PageSize == 0)
                    PageCount = TotalItems / PageSize;
                else
                    PageCount = TotalItems / PageSize + 1;

                Int32 Page = GetIntFromQueryString("Page");

                if (Page == -1) Page = 1;
                int FromRow = (Page - 1) * PageSize;
                int ToRow = Page * PageSize - 1;
                if (ToRow >= TotalItems)
                    ToRow = TotalItems - 1;

                StringBuilder hcm = new StringBuilder();
                for (int i = 0; i < acs.Count; i++)
                {
                    var item = acs[i];

                    string OrderCode = "";
                    if (item.OrderTransactionCode != null)
                    {
                        string[] os = item.OrderTransactionCode.Split('|');
                        if (os.Length > 0)
                        {
                            OrderCode += "<table class=\"table table-bordered table-hover\">";
                            OrderCode += "<tr>";
                            OrderCode += "<th>Mã bưu kiện</th>";
                            OrderCode += "<th>Cân nặng (kg)</th>";
                            OrderCode += "</tr>";

                            double canNang = 0;
                            for (int j = 0; j < os.Length; j++)
                            {
                                OrderCode += "<tr>";
                                if (!string.IsNullOrEmpty(os[j]) || !string.IsNullOrWhiteSpace(os[j]))
                                {
                                    OrderCode += "<td>" + os[j] + "</td>";
                                    var smallPackage = SmallPackageController.GetByOrderTransactionCode(os[j]);
                                    double compareSize = 0;
                                    double weight = Convert.ToDouble(smallPackage.Weight);

                                    double pDai = Convert.ToDouble(smallPackage.Length);
                                    double pRong = Convert.ToDouble(smallPackage.Width);
                                    double pCao = Convert.ToDouble(smallPackage.Height);
                                    if (pDai > 0 && pRong > 0 && pCao > 0)
                                    {
                                        compareSize = (pDai * pRong * pCao) / 6000;
                                    }

                                    if (weight >= compareSize)
                                    {
                                        canNang = Math.Round(weight, 5);
                                    }
                                    else
                                    {
                                        canNang += Math.Round(compareSize, 5);
                                    }
                                    OrderCode += "<td>" + canNang + "</td>";
                                    OrderCode += "<tr>";
                                }
                            }
                            OrderCode += "</table>";

                        }
                    }
                    else
                    {
                        var os = OutStockSessionPackageController.GetAllByOutStockUserID(item.ID);
                        OrderCode += "<table class=\"table table-bordered table-hover\">";
                        OrderCode += "<tr>";
                        OrderCode += "<th>Mã bưu kiện</th>";
                        OrderCode += "<th>Cân nặng (kg)</th>";
                        OrderCode += "</tr>";
                        foreach (var ro in os)
                        {
                            OrderCode += "<tr>";
                            OrderCode += "<td>" + ro.OrderTransactionCode + "</td>";
                            OrderCode += "<td>" + ro.Weight + "</td>";
                            OrderCode += "<tr>";
                        }
                        OrderCode += "</table>";
                    }
                    string StatusName = "<span class=\"badge red darken-2 white-text border-radius-2\">Yêu cầu mới<span>";
                    if (item.Status == 2)
                        StatusName = "<span class=\"badge blue darken-2 white-text border-radius-2\">Đã xử lý<span>";

                    string note = "";
                    if (item.Note != null)
                        note = item.Note;
                    hcm.Append("<tr>");
                    hcm.Append("<td>" + item.ID + "</td>");
                    hcm.Append("<td>" + item.Username + "</td>");
                    hcm.Append("<td>" + item.MainOrderString + "</td>");
                    hcm.Append("<td>" + string.Format("{0:N0}", Convert.ToDouble(item.TotalPay)) + "</td>");
                    hcm.Append("<td>" + OrderCode + "</td>");
                    hcm.Append("<td>" + item.TongKien + "</td>");
                    hcm.Append("<td>" + item.TongCan + "</td>");
                    hcm.Append("<td>" + StatusName + "</td>");
                    hcm.Append("<td>" + note + "</td>");
                    hcm.Append("<td>" + item.CreatedDate.ToString("dd/MM/yyyy HH:mm") + "</td>");

                    hcm.Append("<td>");
                    hcm.Append("<div class=\"action-table\">");
                    if (item.Status == 0)
                    {
                        hcm.Append("<a onclick=\"UpdateOutStock(" + item.ID + ")\" class=\"tooltipped\" data-position=\"top\" data-tooltip=\"Xác nhận\"><i class=\"material-icons\">edit</i><span>Xác nhận</span></a>");
                    }
                    hcm.Append("</div>");
                    hcm.Append("</td>");

                    hcm.Append("</tr>");
                }
                ltr.Text = hcm.ToString();
            }
        }
        public static Int32 GetIntFromQueryString(String key)
        {
            Int32 returnValue = -1;
            String queryStringValue = HttpContext.Current.Request.QueryString[key];
            try
            {
                if (queryStringValue == null)
                    return returnValue;
                if (queryStringValue.IndexOf("#") > 0)
                    queryStringValue = queryStringValue.Substring(0, queryStringValue.IndexOf("#"));
                returnValue = Convert.ToInt32(queryStringValue);
            }
            catch
            { }
            return returnValue;
        }
        private int PageCount;
        protected void DisplayHtmlStringPaging1()
        {
            Int32 CurrentPage = Convert.ToInt32(Request.QueryString["Page"]);
            if (CurrentPage == -1) CurrentPage = 1;
            string[] strText = new string[4] { "Trang đầu", "Trang cuối", "Trang sau", "Trang trước" };
            if (PageCount > 1)
                Response.Write(GetHtmlPagingAdvanced(6, CurrentPage, PageCount, Context.Request.RawUrl, strText));
        }
        private static string GetPageUrl(int currentPage, string pageUrl)
        {
            pageUrl = Regex.Replace(pageUrl, "(\\?|\\&)*" + "Page=" + currentPage, "");
            if (pageUrl.IndexOf("?") > 0)
            {
                if (pageUrl.IndexOf("Page=") > 0)
                {
                    int a = pageUrl.IndexOf("Page=");
                    int b = pageUrl.Length;
                    pageUrl.Remove(a, b - a);
                }
                else
                {
                    pageUrl += "&Page={0}";
                }

            }
            else
            {
                pageUrl += "?Page={0}";
            }
            return pageUrl;
        }
        public static string GetHtmlPagingAdvanced(int pagesToOutput, int currentPage, int pageCount, string currentPageUrl, string[] strText)
        {
            //Nếu Số trang hiển thị là số lẻ thì tăng thêm 1 thành chẵn
            if (pagesToOutput % 2 != 0)
            {
                pagesToOutput++;
            }

            //Một nửa số trang để đầu ra, đây là số lượng hai bên.
            int pagesToOutputHalfed = pagesToOutput / 2;

            //Url của trang
            string pageUrl = GetPageUrl(currentPage, currentPageUrl);


            //Trang đầu tiên
            int startPageNumbersFrom = currentPage - pagesToOutputHalfed; ;

            //Trang cuối cùng
            int stopPageNumbersAt = currentPage + pagesToOutputHalfed; ;

            StringBuilder output = new StringBuilder();

            //Nối chuỗi phân trang
            //output.Append("<div class=\"paging\">");
            //output.Append("<ul class=\"paging_hand\">");

            //Link First(Trang đầu) và Previous(Trang trước)
            if (currentPage > 1)
            {
                //output.Append("<li class=\"UnselectedPrev \" ><a title=\"" + strText[0] + "\" href=\"" + string.Format(pageUrl, 1) + "\">|<</a></li>");
                //output.Append("<li class=\"UnselectedPrev\" ><a title=\"" + strText[1] + "\" href=\"" + string.Format(pageUrl, currentPage - 1) + "\"><i class=\"fa fa-angle-left\"></i></a></li>");
                output.Append("<a class=\"prev-page pagi-button\" title=\"" + strText[1] + "\" href=\"" + string.Format(pageUrl, currentPage - 1) + "\">Prev</a>");
                //output.Append("<span class=\"Unselect_prev\"><a href=\"" + string.Format(pageUrl, currentPage - 1) + "\"></a></span>");
            }

            /******************Xác định startPageNumbersFrom & stopPageNumbersAt**********************/
            if (startPageNumbersFrom < 1)
            {
                startPageNumbersFrom = 1;

                //As page numbers are starting at one, output an even number of pages.  
                stopPageNumbersAt = pagesToOutput;
            }

            if (stopPageNumbersAt > pageCount)
            {
                stopPageNumbersAt = pageCount;
            }

            if ((stopPageNumbersAt - startPageNumbersFrom) < pagesToOutput)
            {
                startPageNumbersFrom = stopPageNumbersAt - pagesToOutput;
                if (startPageNumbersFrom < 1)
                {
                    startPageNumbersFrom = 1;
                }
            }
            /******************End: Xác định startPageNumbersFrom & stopPageNumbersAt**********************/

            //Các dấu ... chỉ những trang phía trước  
            if (startPageNumbersFrom > 1)
            {
                output.Append("<a href=\"" + string.Format(GetPageUrl(currentPage - 1, pageUrl), startPageNumbersFrom - 1) + "\">&hellip;</a>");
            }

            //Duyệt vòng for hiển thị các trang
            for (int i = startPageNumbersFrom; i <= stopPageNumbersAt; i++)
            {
                if (currentPage == i)
                {
                    output.Append("<a class=\"pagi-button current-active\">" + i.ToString() + "</a>");
                }
                else
                {
                    output.Append("<a class=\"pagi-button\" href=\"" + string.Format(pageUrl, i) + "\">" + i.ToString() + "</a>");
                }
            }

            //Các dấu ... chỉ những trang tiếp theo  
            if (stopPageNumbersAt < pageCount)
            {
                output.Append("<a href=\"" + string.Format(pageUrl, stopPageNumbersAt + 1) + "\">&hellip;</a>");
            }

            //Link Next(Trang tiếp) và Last(Trang cuối)
            if (currentPage != pageCount)
            {
                //output.Append("<span class=\"Unselect_next\"><a href=\"" + string.Format(pageUrl, currentPage + 1) + "\"></a></span>");
                //output.Append("<li class=\"UnselectedNext\" ><a title=\"" + strText[2] + "\" href=\"" + string.Format(pageUrl, currentPage + 1) + "\"><i class=\"fa fa-angle-right\"></i></a></li>");
                output.Append("<a class=\"next-page pagi-button\" title=\"" + strText[2] + "\" href=\"" + string.Format(pageUrl, currentPage + 1) + "\">Next</a>");
                //output.Append("<li class=\"UnselectedNext\" ><a title=\"" + strText[3] + "\" href=\"" + string.Format(pageUrl, pageCount) + "\">>|</a></li>");
            }
            //output.Append("</ul>");
            //output.Append("</div>");
            return output.ToString();
        }
        #endregion

        [WebMethod]
        public static string UpdateStatus(string ID)
        {
            if (HttpContext.Current.Session["userLoginSystem"] != null)
            {
                string username = HttpContext.Current.Session["userLoginSystem"].ToString();
                var ph = OutStockSessionController.GetByUserID(ID.ToInt(0));
                if (ph != null)
                {
                    string kq = OutStockSessionController.UpdateStatusUser(ph.ID, 2, username, DateTime.Now);
                    string[] MainOrderString = ph.MainOrderID.Split('|');
                    int MainOrderID = 0;
                    if (MainOrderString.Length > 0)
                    {
                        for (int i = 0; i < MainOrderString.Length - 1; i++)
                        {
                            MainOrderID = Convert.ToInt32(MainOrderString[i]);
                            var mo = MainOrderController.GetAllByID(MainOrderID);
                            if (mo != null)
                            {
                                MainOrderController.UpdateYCG(MainOrderID, false);
                            }
                        }
                    }

                    if (kq.ToInt(0) > 0)
                    {
                        return "ok";
                    }
                    else
                    {
                        return "none";
                    }
                }
                else
                    return null;
            }
            else
                return null;
        }

        protected void btnSaveAdd_Click(object sender, EventArgs e)
        {
            string username_current = Session["userLoginSystem"].ToString();
            var ac = AccountController.GetByUsername(username_current);
            DateTime currentDate = DateTime.Now;
            if (ac != null)
            {
                if (ac.RoleID == 0 || ac.RoleID == 2 || ac.RoleID == 9)
                {
                    int mainOrderID = Convert.ToInt32(txtMainOrderID.Text);
                    string note = txtNote.Text;
                    var mainOrder = MainOrderController.GetByID(mainOrderID);
                    if (mainOrder != null)
                    {
                        var acc = AccountController.GetByID(mainOrder.UID ?? 0);

                        double tongCan = 0;
                        int tongKien = 0;

                        string listmvd = hdfCodeTransactionListMVD.Value;
                        string[] list;
                        if (!string.IsNullOrEmpty(listmvd))
                        {
                            list = listmvd.Split('|');
                            for (int i = 0; i < list.Length; i++)
                            {
                                tongKien++;
                                var smallPackage = SmallPackageController.GetByOrderTransactionCode(list[i]);
                                if (smallPackage == null)
                                    PJUtils.ShowMessageBoxSwAlert($"Mã vận đơn {list[i]} không tồn tại", "e", true, Page);
                                if(smallPackage.Status != 3)
                                    PJUtils.ShowMessageBoxSwAlert($"Mã vận đơn {list[i]} không ở kho VN", "e", true, Page);

                                double compareSize = 0;
                                double weight = Convert.ToDouble(smallPackage.Weight);

                                double pDai = Convert.ToDouble(smallPackage.Length);
                                double pRong = Convert.ToDouble(smallPackage.Width);
                                double pCao = Convert.ToDouble(smallPackage.Height);
                                if (pDai > 0 && pRong > 0 && pCao > 0)
                                {
                                    compareSize = (pDai * pRong * pCao) / 6000;
                                }

                                if (weight >= compareSize)
                                {
                                    tongCan += Math.Round(weight, 5);
                                }
                                else
                                {
                                    tongCan += Math.Round(compareSize, 5);
                                }
                            }
                        }

                        double tienCanTT = Convert.ToDouble(mainOrder.TotalPriceVND) - Convert.ToDouble(mainOrder.Deposit);
                        string kq = OutStockUserController.Insert(acc.ID, acc.Username, tongCan, tongKien, 0, currentDate, username_current, mainOrderID.ToString(), tienCanTT, listmvd, note);
                        PJUtils.ShowMessageBoxSwAlert("Tạo yêu cầu giao thành công", "s", true, Page);
                    }
                    else
                    {
                        PJUtils.ShowMessageBoxSwAlert("Mã đơn hàng không tồn tại", "e", true, Page);
                    }
                }
            }
        }
    }
}