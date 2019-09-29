using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ShopSystem.Controllers
{
    [App_Code.AdminLoginFilter]
    public class AdminController : Controller
    {
        // GET: Admin
        public ActionResult Index()
        {
            return View();
        }

        #region 管理商品
        public ActionResult ManageProduct()
        {
            #region 列出商品
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            DB.DB mydb = new DB.DB();
            DB.IProduct ip = new DB.Impl_Product(mydb.Connection);
            DataTable tt = ip.GetProducts();
            if (tt != null)
            {
                foreach (DataRow r in tt.Rows)
                {
                    sb.Append("<tr>");
                    sb.Append(string.Format("<td align=\"center\"><a style=\"text-decoration:underline\" href=\"/Admin/Product?pid={0}\">{1}</a></td>", r["proID"].ToString(), r["proID"].ToString()));
                    sb.Append(string.Format("<td align=\"center\">{0}</td>", r["proName"].ToString()));
                    sb.Append(string.Format("<td align=\"center\">{0}</td>", r["PTID"].ToString()));
                    sb.Append(string.Format("<td align=\"center\">{0}</td>", r["proPrice"].ToString()));
                    sb.Append(string.Format("<td align=\"center\">{0}</td>", r["proStock"].ToString()));
                    sb.Append(string.Format("<td align=\"center\">{0}</td>", r["proState"].ToString()));
                    sb.Append(string.Format("<td align=\"center\">{0}</td>", r["proPic"].ToString()));
                    sb.Append(string.Format("<td>{0}</td>", r["proNote"].ToString()));
                    sb.Append("</tr>");
                }
            }
            ViewData["products"] = sb.ToString();
            #endregion

            return View();
        }
        public ActionResult Product(int? pid)
        {
            if (pid == null)
                pid = 1;
            
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            DB.DB mydb = new DB.DB();
            DB.IProduct ip = new DB.Impl_Product(mydb.Connection);
            Dictionary<string, object> dp = ip.GetSingleProduct(pid.Value);

            if (dp == null)
                RedirectToAction("Product");

            Models.ProductInfo pi = new Models.ProductInfo();
            pi.ID = (int)dp["proID"];
            pi.Name = dp["proName"].ToString();
            pi.PTID = (int)dp["PTID"];
            pi.Note = dp["proNote"].ToString();
            pi.Price = Convert.ToInt32(dp["proPrice"]);
            pi.Stock = Convert.ToInt32(dp["proStock"]);
            pi.State = Convert.ToInt32(dp["proState"]);
            pi.Pic = dp["proPic"].ToString();

            return View(pi);
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult UpdateProduct(Models.ProductInfo product)
        {
            DB.DB mydb = new DB.DB("SQLAdmin", "admin1234");
            DB.IProduct ip = new DB.Impl_Product(mydb.Connection);
            ip.UpdateProduct(product.ID, product.Name, product.PTID, product.Price, product.Note, product.Stock, product.State, product.Pic);
            return RedirectToAction("ManageProduct");
        }
        public ActionResult NewProduct()
        {
            return View();
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult AddProduct(Models.ProductInfo product)
        {
            DB.DB mydb = new DB.DB("SQLAdmin", "admin1234");
            DB.IProduct ip = new DB.Impl_Product(mydb.Connection);
            ip.AddNewProduct(product.Name, product.PTID, product.Price, product.Note, product.Stock, product.State, product.Pic);
            return RedirectToAction("ManageProduct");
        }
        #endregion

        #region 管理訂單
        public ActionResult ManageOrder()
        {
            #region 列出訂單
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            DB.DB mydb = new DB.DB();
            DB.IOrder io = new DB.Impl_Order(mydb.Connection);
            DataTable tt = io.GetOrderInfo();
            if (tt != null)
            {
                foreach (DataRow r in tt.Rows)
                {
                    sb.Append("<tr>");
                    sb.Append(string.Format("<td align=\"center\">{0}</td>", r["orderID"].ToString()));
                    sb.Append(string.Format("<td align=\"center\">{0}</td>", r["custID"].ToString()));
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
        [HttpPost]
        public ActionResult ChageOrderState(Models.Order order)
        {
            DB.DB mydb = new DB.DB("SQLAdmin", "admin1234");
            DB.IOrder io = new DB.Impl_Order(mydb.Connection);
            io.UpdateOrder(order.orderID, order.orderState);
            return RedirectToAction("ManageOrder");
        }
        #endregion

        #region 管理客服
        public ActionResult ManageService()
        {
            return View();
        }
        #endregion
    }
}