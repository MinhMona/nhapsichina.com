using NHST.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NHST.Controllers
{
    public class OutStockUserController
    {
        public static string Insert(int UID, string Username, double tongCan, int tongKien, int Status,
    DateTime CreatedDate, string CreatedBy, string mainOrderID, double tongTien, string orderTransactionCode, string note)
        {
            using (var dbe = new NHSTEntities()) //now wrapping the context in a using to ensure it is disposed
            {
                tbl_OutStockUser o = new tbl_OutStockUser();
                o.UID = UID;
                o.Username = Username;
                o.TongCan = tongCan;
                o.TongKien = tongKien;
                o.Status = Status;
                o.CreatedDate = CreatedDate;
                o.CreatedBy = CreatedBy;
                o.MainOrderID = mainOrderID;
                o.TotalPrice = tongTien;
                o.OrderTransactionCode = orderTransactionCode;
                o.Note = note;
                dbe.tbl_OutStockUser.Add(o);
                int kq = dbe.SaveChanges();
                string k = o.ID.ToString();
                return k;
            }

        }
    }
}