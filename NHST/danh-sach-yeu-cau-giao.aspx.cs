using NHST.Controllers;
using NHST.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using static NHST.Controllers.OutStockSessionController;
using static NHST.WebService1;

namespace NHST
{
    public partial class danh_sach_yeu_cau_giao : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["userLoginSystem"] != null)
                {
                    LoadData();
                }
                else
                {
                    Response.Redirect("/dang-nhap");
                }
            }
        }

        private void LoadData()
        {
            string username = Session["userLoginSystem"].ToString();
            var ac = AccountController.GetByUsername(username);
            if (ac != null)
            {
                int page = 0;
                Int32 Page = GetIntFromQueryString("Page");
                if (Page > 0)
                {
                    page = Page - 1;
                }
                var la = OutStockSessionController.GetDataOutStockUser(username, 0, "", "", page, 20);
                int Total = OutStockSessionController.GetTotalDataUser(username, "", "", 0);
                pagingall(la, Total, 20);
            }
        }

        #region Pagging
        public void pagingall(List<OutStockSessionController.OutStockNew> acs, int total, int PageSize)
        {
            if (total > 0)
            {
                string username = Session["userLoginSystem"].ToString();
                var ac = AccountController.GetByUsername(username);
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

                    hcm.Append("<tr>");
                    hcm.Append("<td>" + item.ID + "</td>");
                    hcm.Append("<td>" + item.MainOrderString + "</td>");
                    hcm.Append("<td>" + string.Format("{0:N0}", Convert.ToDouble(item.TotalPay)) + "</td>");
                    double tienNapThem = 0;
                    if ((ac.Wallet ?? 0) < Convert.ToDouble(item.TotalPay))
                    {
                        tienNapThem = Convert.ToDouble(item.TotalPay) - (ac.Wallet ?? 0);
                    }
                    hcm.Append("<td>" + string.Format("{0:N0}", tienNapThem) + "</td>");
                    hcm.Append("<td>" + OrderCode + "</td>");
                    hcm.Append("<td>" + item.TongKien + "</td>");
                    hcm.Append("<td>" + item.TongCan + "</td>");
                    hcm.Append("<td>" + StatusName + "</td>");
                    hcm.Append("<td>" + item.CreatedDate.ToString("dd/MM/yyyy HH:mm") + "</td>");
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
    }
}