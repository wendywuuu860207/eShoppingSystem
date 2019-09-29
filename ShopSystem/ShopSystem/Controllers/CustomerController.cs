using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ShopSystem.Controllers
{
    [App_Code.CustomerLoginFilter]
    public class CustomerController : Controller
    {
        // GET: Customer
        public ActionResult Index()
        {
            #region 列出訂單
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            DB.DB mydb = new DB.DB();
            DB.IOrder io = new DB.Impl_Order(mydb.Connection);
            DataTable tt = io.GetOrderInfoByCustPhone(Session["Login"].ToString());
            if (tt != null)
            {
                foreach (DataRow r in tt.Rows)
                {
                    sb.Append("<tr>");
                    sb.Append(string.Format("<td align=\"center\"><a style=\"text-decoration:underline\" href=\"/Customer/OrderDetail?oid={0}\">{1}</a></td>", r["orderID"].ToString(), r["orderID"].ToString()));
                    sb.Append(string.Format("<td align=\"center\">{0}</td>", r["rcptName"].ToString()));
                    sb.Append(string.Format("<td align=\"center\">{0}</td>", r["rcptPhone"].ToString()));
                    sb.Append(string.Format("<td>{0}</td>", r["rcptAddr"].ToString()));
                    sb.Append(string.Format("<td>{0}</td>", r["DTName"].ToString()));
                    sb.Append(string.Format("<td>{0}</td>", r["Shipping"].ToString()));
                    sb.Append(string.Format("<td>{0}</td>", r["orderState"].ToString()));
                    sb.Append(string.Format("<td>{0}</td>", r["orderNote"].ToString()));
                    sb.Append(string.Format("<td>{0}</td>", r["orderTime"].ToString()));
                    sb.Append("</tr>");
                }
            }
            ViewData["orderList"] = sb.ToString();
            #endregion

            return View();
        }
        public ActionResult OrderDetail(int? oid)
        {
            if (oid == null)
                RedirectToAction("Index");

            #region 列出訂單明細
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            DB.DB mydb = new DB.DB();
            DB.IOrder io = new DB.Impl_Order(mydb.Connection);
            DataTable tt = io.GetOrderDetail(oid.Value);
            int totalPrice = 0;
            if (tt != null)
            {
                foreach (DataRow r in tt.Rows)
                {
                    totalPrice += (int)r["proPrice"] * (int)r["quantity"];
                    sb.Append("<tr>");
                    sb.Append(string.Format("<td>{0}</td>", r["proName"].ToString()));
                    sb.Append(string.Format("<td>{0}</td>", r["proPrice"].ToString()));
                    sb.Append(string.Format("<td>{0}</td>", r["quantity"].ToString()));
                    sb.Append("</tr>");
                }
            }
            ViewData["orderID"] = oid.ToString();
            ViewData["totalPrice"] = totalPrice.ToString();
            ViewData["orderDetailList"] = sb.ToString();
            #endregion

            return View();
        }

        public ActionResult ChangePW(string msg)
        {
            ViewData["msg"] = msg;
            return View();
        }
        [HttpPost]
        public ActionResult UpdatePW(Models.MemberInfo Mi)
        {
            if(!CheckLI(Session["Login"].ToString(), Mi.oldPW))
                return RedirectToAction("ChangePW", new { msg = "無效的舊密碼!" });
            else if (Mi.oldPW == Mi.PW)
                return RedirectToAction("ChangePW", new { msg = "新舊密碼不能相同!" });

            DB.DB mydb = new DB.DB("SQLAdmin","admin1234");
            DB.ICustomer ic = new DB.Impl_Customer(mydb.Connection);
            ic.ChangePassword(Session["Login"].ToString(), Mi.PW);
            return RedirectToAction("ChangePW", new { msg = "密碼變更成功!" });
        }
        public bool CheckLI(string phone, string pw)
        {
            DB.DB mydb = new DB.DB();
            DB.ICustomer ic = new DB.Impl_Customer(mydb.Connection);
            return ic.CheckLogin(phone, pw);
        }

        public ActionResult ContectService()
        {
            return View();
        }
    }
}