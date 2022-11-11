using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MB.Extensions;
using NHST.Bussiness;
using NHST.Controllers;
using System.Data;
using System.IO;
using System.Text;
using System.Web.Services;
using System.Web.Script.Serialization;

namespace NHST.manager
{
    public partial class import_smallchina : System.Web.UI.Page
    {
        string currFilePath = string.Empty;
        string currFileExtension = string.Empty;  //File Extension
        protected DataTable _FileTempPlan
        {
            get { return (DataTable)this.Session["FileTempDb992"]; }
            set
            {
                this.Session["FileTempDb992"] = value;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["userLoginSystem"] == null)
                {
                    Response.Redirect("/trang-chu");
                }
                else
                {
                    _FileTempPlan = null;
                }                
            }
        }        

        private void UploadToTemp()
        {
            HttpPostedFile file = this.FileUpload1.PostedFile;
            string fileName = file.FileName;
            string tempPath = System.IO.Path.GetTempPath();
            fileName = Path.GetFileName(fileName);
            this.currFileExtension = Path.GetExtension(fileName);
            this.currFilePath = tempPath + fileName;
            file.SaveAs(this.currFilePath);
        }

        protected void btnExport_Click(object sender, EventArgs e)
        {
            StringBuilder StrExport = new StringBuilder();
            StrExport.Append(@"<html xmlns:o='urn:schemas-microsoft-com:office:office' xmlns:w='urn:schemas-microsoft-com:office:excel' xmlns='http://www.w3.org/TR/REC-html40'><head><title>Time</title>");
            StrExport.Append(@"<body lang=EN-US style='mso-element:header' id=h1><span style='mso--code:DATE'></span><div class=Section1>");
            StrExport.Append("<DIV  style='font-size:12px;'>");
            StrExport.Append("<table border=\"1\">");
            StrExport.Append("  <tr>");
            StrExport.Append("      <th><strong>STT</strong></th>");
            StrExport.Append("      <th><strong>MaVanDon</strong></th>");
            StrExport.Append("      <th><strong>CanNang</strong></th>");
            StrExport.Append("  </tr>");
            StrExport.Append("  <tr>");
            StrExport.Append("      <td>1</td>");
            StrExport.Append("      <td>123456789</td>");
            StrExport.Append("      <td>0.5</td>");
            StrExport.Append("  </tr>");
            StrExport.Append("</table>");
            StrExport.Append("</div></body></html>");
            string strFile = "import-china.xls";
            string strcontentType = "application/vnd.ms-excel";
            Response.ClearContent();
            Response.ClearHeaders();
            Response.BufferOutput = true;
            Response.ContentType = strcontentType;
            Response.AddHeader("Content-Disposition", "attachment; filename=" + strFile);
            Response.Write(StrExport.ToString());
            Response.Flush();
            Response.End();
        }

        protected void btnImport_Click(object sender, EventArgs e)
        {
            try
            {
                if (FileUpload1.HasFile)
                {
                    string username_current = Session["userLoginSystem"].ToString();
                    var obj_user = AccountController.GetByUsername(username_current);
                    DateTime currentDate = DateTime.Now;

                    UploadToTemp();
                    if (this.currFileExtension == ".xlsx" || this.currFileExtension == ".xls")
                    {
                        _FileTempPlan = PJUtils.ReadDataExcel(currFilePath, Path.GetExtension(FileUpload1.PostedFile.FileName), "Yes");// WebUtil.ReadDataFromExcel(currFilePath);
                        File.Delete((Path.GetTempPath() + FileUpload1.FileName));

                        var rs = string.Empty;
                        if (_FileTempPlan != null)
                        {
                            string checkMVD = "";
                            bool checktb = true;
                            var t = 0;
                            foreach (DataRow drRow in _FileTempPlan.Rows)
                            {
                                try
                                {
                                    int STT = 0;
                                    string sttstring = drRow["STT"].ToString().Trim();
                                    if (!string.IsNullOrEmpty(sttstring))
                                    {
                                        STT = Convert.ToInt32(sttstring);
                                    }
                                    checkMVD = drRow["MaVanDon"].ToString().Trim();
                                    double Weight = 0;
                                    if (drRow["CanNang"] != DBNull.Value)
                                    {
                                        Weight = Convert.ToDouble(drRow["CanNang"]);
                                    }                                   
                                    if (!string.IsNullOrEmpty(checkMVD))
                                    {
                                        int smallID = 0;
                                        int MainOrderID = 0;
                                        var check = SmallPackageController.GetByOrderTransactionCode(checkMVD);
                                        if (check != null)
                                        {
                                            smallID = Convert.ToInt32(check.ID);
                                            MainOrderID = Convert.ToInt32(check.MainOrderID);

                                            SmallPackageController.UpdateImportOutChina(smallID, Weight, 5, obj_user.Username, currentDate, STT);
                                            if (MainOrderID > 0)
                                            {
                                                var mo = MainOrderController.GetAllByID(Convert.ToInt32(MainOrderID));
                                                if (mo != null)
                                                {
                                                    string orderstatus = "";
                                                    int currentOrderStatus = Convert.ToInt32(mo.Status);
                                                    switch (currentOrderStatus)
                                                    {
                                                        case 0:
                                                            orderstatus = "Đơn mới";
                                                            break;
                                                        case 1:
                                                            orderstatus = "Đơn hàng hủy";
                                                            break;
                                                        case 2:
                                                            orderstatus = "Đơn đã cọc";
                                                            break;
                                                        case 3:
                                                            orderstatus = "Đơn người bán giao";
                                                            break;
                                                        case 4:
                                                            orderstatus = "Đơn chờ mua hàng";
                                                            break;
                                                        case 5:
                                                            orderstatus = "Đơn đã mua hàng";
                                                            break;
                                                        case 6:
                                                            orderstatus = "Kho Trung Quốc nhận hàng";
                                                            break;
                                                        case 7:
                                                            orderstatus = "Trên đường về Việt Nam";
                                                            break;
                                                        case 8:
                                                            orderstatus = "Trong kho Hà Nội";
                                                            break;
                                                        case 9:
                                                            orderstatus = "Đã thanh toán";
                                                            break;
                                                        case 10:
                                                            orderstatus = "Đã hoàn thành";
                                                            break;
                                                        case 11:
                                                            orderstatus = "Đang giao hàng";
                                                            break;
                                                        case 12:
                                                            orderstatus = "Đơn khiếu nại";
                                                            break;
                                                        default:
                                                            break;
                                                    }

                                                    if (mo.Status < 7)
                                                    {
                                                        MainOrderController.UpdateStatus(mo.ID, mo.UID.Value, 7);
                                                        HistoryOrderChangeController.Insert(mo.ID, obj_user.ID, obj_user.Username, obj_user.Username +
                                                        " đã import đổi trạng thái của đơn hàng ID là: " + mo.ID + ", từ: " + orderstatus + ", sang: Hàng xuất kho Trung - đang trên đường về VN", 0, currentDate);
                                                        if (mo.DateToVN == null)
                                                        {
                                                            MainOrderController.UpdateDateToVN(mo.ID, currentDate);
                                                        }
                                                    }

                                                    double FeeWeight = 0;
                                                    double FeeWeightDiscount = 0;                                                    
                                                    double returnprice = 0;
                                                    double pricePerWeight = 0;
                                                    double finalPriceOfPackage = 0;
                                                    double cannangdonggo = 0;
                                                    double TongCanNang = 0;

                                                    int warehouse = mo.ReceivePlace.ToInt(1);
                                                    int shipping = Convert.ToInt32(mo.ShippingType);
                                                    int warehouseFrom = Convert.ToInt32(mo.FromPlace);
                                                    var usercreate = AccountController.GetByID(Convert.ToInt32(mo.UID));
                                                    double ckFeeWeight = Convert.ToDouble(UserLevelController.GetByID(usercreate.LevelID.ToString().ToInt()).FeeWeight.ToString());

                                                    var smallpackage = SmallPackageController.GetByMainOrderID(MainOrderID);
                                                    if (smallpackage.Count > 0)
                                                    {
                                                        double totalWeight = 0;
                                                        foreach (var item in smallpackage)
                                                        {
                                                            double totalWeightCN = Math.Round(Convert.ToDouble(item.Weight), 5);
                                                            double totalWeightTT = 0;
                                                            double pDai = Convert.ToDouble(item.Length);
                                                            double pRong = Convert.ToDouble(item.Width);
                                                            double pCao = Convert.ToDouble(item.Height);
                                                            if (pDai > 0 && pRong > 0 && pCao > 0)
                                                            {
                                                                totalWeightTT = (pDai * pRong * pCao) / 6000;
                                                            }
                                                            if (totalWeightCN > totalWeightTT)
                                                            {
                                                                totalWeight += totalWeightCN;
                                                            }
                                                            else
                                                            {
                                                                totalWeight += totalWeightTT;
                                                            }
                                                        }
                                                        if (!string.IsNullOrEmpty(usercreate.FeeTQVNPerWeight))
                                                        {
                                                            double feetqvn = 0;
                                                            if (usercreate.FeeTQVNPerWeight.ToFloat(0) > 0)
                                                            {
                                                                feetqvn = Convert.ToDouble(usercreate.FeeTQVNPerWeight);
                                                                pricePerWeight = Convert.ToDouble(usercreate.FeeTQVNPerWeight);
                                                            }
                                                            returnprice = totalWeight * feetqvn;
                                                        }
                                                        else
                                                        {
                                                            var fee = WarehouseFeeController.GetByAndWarehouseFromAndToWarehouseAndShippingTypeAndAndHelpMoving(warehouseFrom, warehouse, shipping, false);
                                                            if (fee.Count > 0)
                                                            {
                                                                foreach (var f in fee)
                                                                {
                                                                    if (Convert.ToDouble(mo.PriceVND) > f.WeightFrom && Convert.ToDouble(mo.PriceVND) <= f.WeightTo)
                                                                    {
                                                                        pricePerWeight = Convert.ToDouble(f.Price);
                                                                        returnprice = totalWeight * Convert.ToDouble(f.Price);
                                                                        break;
                                                                    }
                                                                }
                                                            }
                                                        }                                                        

                                                        foreach (var item in smallpackage)
                                                        {
                                                            double compareweight = 0;
                                                            double compareSize = 0;

                                                            double weight = Math.Round(Convert.ToDouble(item.Weight), 5);
                                                            compareweight = weight * pricePerWeight;

                                                            double weigthTT = 0;
                                                            double pDai = Convert.ToDouble(item.Length);
                                                            double pRong = Convert.ToDouble(item.Width);
                                                            double pCao = Convert.ToDouble(item.Height);
                                                            if (pDai > 0 && pRong > 0 && pCao > 0)
                                                            {
                                                                weigthTT = (pDai * pRong * pCao) / 6000;
                                                            }
                                                            weigthTT = Math.Round(weigthTT, 5);
                                                            compareSize = weigthTT * pricePerWeight;

                                                            if (compareweight >= compareSize)
                                                            {
                                                                finalPriceOfPackage += compareweight;
                                                                SmallPackageController.UpdateTotalPrice(item.ID, compareweight);
                                                            }
                                                            else
                                                            {
                                                                finalPriceOfPackage += compareSize;
                                                                SmallPackageController.UpdateTotalPrice(item.ID, compareSize);
                                                            }
                                                        }

                                                        cannangdonggo = totalWeight;
                                                        TongCanNang = totalWeight;                                                       
                                                    }

                                                    returnprice = Math.Round(finalPriceOfPackage, 0);
                                                    FeeWeight = returnprice;
                                                    FeeWeightDiscount = FeeWeight * ckFeeWeight / 100;
                                                    FeeWeight = Math.Round(FeeWeight - FeeWeightDiscount, 0);

                                                    cannangdonggo = Math.Round(cannangdonggo, 5);
                                                    TongCanNang = Math.Round(TongCanNang, 5);

                                                    var conf = ConfigurationController.GetByTop1();

                                                    double IsPackedPrice = 0;
                                                    if (mo.IsPacked == true)
                                                    {
                                                        if (cannangdonggo > 0)
                                                        {
                                                            if (cannangdonggo <= 1)
                                                            {
                                                                IsPackedPrice = Convert.ToDouble(conf.FeeDongGoKgDau);
                                                            }
                                                            else
                                                            {
                                                                cannangdonggo = cannangdonggo - 1;
                                                                cannangdonggo = Math.Round(cannangdonggo, 5);
                                                                IsPackedPrice = Convert.ToDouble(conf.FeeDongGoKgDau) + (cannangdonggo * Convert.ToDouble(conf.FeeDongGoKgSau));
                                                            }
                                                        }
                                                    }
                                                    double IsPriceSepcial = 0;
                                                    if (mo.IsCheckSpecial1 == true)
                                                    {
                                                        if (TongCanNang > 0)
                                                        {
                                                            IsPriceSepcial = (TongCanNang * Convert.ToDouble(conf.FeeDacBiet1));
                                                        }
                                                    }
                                                    if (mo.IsCheckSpecial2 == true)
                                                    {
                                                        if (TongCanNang > 0)
                                                        {
                                                            IsPriceSepcial = (TongCanNang * Convert.ToDouble(conf.FeeDacBiet2));
                                                        }
                                                    }
                                                    if (mo.IsCheckSpecial1 == true && mo.IsCheckSpecial2 == true)
                                                    {
                                                        if (TongCanNang > 0)
                                                        {
                                                            IsPriceSepcial = (TongCanNang * (Convert.ToDouble(conf.FeeDacBiet1) + Convert.ToDouble(conf.FeeDacBiet2)));
                                                        }
                                                    }

                                                    double TotalPriceVND = Convert.ToDouble(mo.PriceVND) + Convert.ToDouble(mo.FeeBuyPro) + Convert.ToDouble(mo.InsuranceMoney) + Convert.ToDouble(mo.FeeShipCN) + FeeWeight + Convert.ToDouble(mo.IsCheckProductPrice) +
                                                    Convert.ToDouble(mo.InsuranceMoney) + Convert.ToDouble(mo.TotalFeeSupport) + Convert.ToDouble(IsPackedPrice) + Convert.ToDouble(IsPriceSepcial) + Convert.ToDouble(mo.IsFastDeliveryPrice);
                                                    TotalPriceVND = Math.Round(TotalPriceVND, 0);
                                                    MainOrderController.UpdateFeeImport(MainOrderID, FeeWeight.ToString(), IsPackedPrice.ToString(), IsPriceSepcial.ToString(), TotalPriceVND.ToString());
                                                    MainOrderController.UpdateTotalWeightandTongCanNang(MainOrderID, TongCanNang.ToString(), TongCanNang.ToString(), TongCanNang.ToString());
                                                }
                                            }                                           
                                        }
                                        else if (check == null)
                                        {
                                            SmallPackageController.InsertTroiNoiOutChina(checkMVD, Weight, obj_user.Username, currentDate);
                                        }
                                        else
                                        {
                                            checkMVD += drRow["MaVanDon"].ToString().Trim() + " - ";
                                            checktb = false;
                                        }
                                    }
                                    t++;
                                }
                                catch (Exception a)
                                {
                                    //rs = rs + drRow["Title"].ToString() + " : lỗi :" + a.Message + "<br/>";
                                }
                            }
                            if (checktb)
                            {
                                PJUtils.ShowMessageBoxSwAlert("Import thành công " + t + " mã vận đơn.", "s", true, Page);
                            }
                            else
                            {
                                PJUtils.ShowMessageBoxSwAlert(t + " Mã vận đơn thành công, trong đó có " + checkMVD + " bị trùng.", "e", true, Page);
                            }
                        }
                        else
                        {
                            PJUtils.ShowMessageBoxSwAlert("Không có dữ liệu.", "s", true, Page);
                        }
                    }
                }
                else
                {
                    PJUtils.ShowMessageBoxSwAlert("Có lỗi trong quá trình import.", "e", true, Page);
                }
            }
            catch (Exception ex)
            {

            }
        }
    }
}