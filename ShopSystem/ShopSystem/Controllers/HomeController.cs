using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ShopSystem.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            #region 光顧數量
            //記錄光顧數量
            string path = Server.MapPath("~/App_Data/VisitCount.txt");
            string text = System.IO.File.ReadAllText(path);
            int nn = int.Parse(text) + 1;
            System.IO.File.WriteAllText(path, nn.ToString());

            //印出光顧數量
            System.Text.StringBuilder sb1 = new System.Text.StringBuilder();
            for (int i = 1; i <= 5 - nn.ToString().Length; i++)
                sb1.Append("<img src='/images/number/0.gif' />");
            for (int i = 0; i < nn.ToString().Length; i++)
                sb1.Append(string.Format("<img src='/images/number/{0}.gif' />", nn.ToString().Substring(i, 1)));

            ViewData["VisitNum"] = sb1.ToString();
            #endregion

            #region 列出商品
            DB.DB mydb = new DB.DB();
            DB.IProduct ip = new DB.Impl_Product(mydb.Connection);
            DataTable tt = ip.GetProductList();

            List<Models.ProductInfo> ps = new List<Models.ProductInfo>();
            if (tt != null)
            {
                foreach (DataRow r in tt.Rows)
                {
                    Models.ProductInfo pi = new Models.ProductInfo();
                    pi.ID = (int)r["proID"];
                    pi.Name = r["proName"].ToString();
                    pi.PTName = r["PTName"].ToString();
                    pi.Pic = r["proPic"].ToString();
                    ps.Add(pi);
                }
            }
            #endregion

            #region 列出分類
            if (Session["proTypes"] == null)
            {
                System.Text.StringBuilder sb2 = new System.Text.StringBuilder();
                sb2.Append("<table>");
                DB.DB mydb2 = new DB.DB();
                DB.IProduct ip2 = new DB.Impl_Product(mydb2.Connection);
                DataTable tt2 = ip2.GetProductType();
                if (tt2 != null)
                {
                    foreach (DataRow r in tt2.Rows)
                    {
                        sb2.Append(string.Format("<tr><td><a style=\"text-align:center\" href=\"/Home/ProductType?type={0}\">", r["PTID"].ToString()));
                        sb2.Append(string.Format("<img src='/images/shop/{0}' />", r["PTPic"].ToString()));
                        sb2.Append("</a></td></tr>");
                    }
                }
                sb2.Append("</table>");
                Session["proTypes"] = sb2.ToString();
            }
            ViewData["proTypes"] = Session["proTypes"];
            #endregion

            return View(ps);
        }
        public ActionResult ProductType(int? type, int? page)
        {
            if (page == null) page = 1;
            if (type == null) type = 0;

            DB.DB mydb = new DB.DB("SQLAdmin", "admin1234");
            DB.IProduct ip = new DB.Impl_Product(mydb.Connection);
            DataTable tt = ip.GetProductListByType(type.Value);
            
            if (tt == null) tt = new DataTable();
            int count = tt.Rows.Count;

            int totalPage = 1;
            if (count > 5)
                totalPage = (int)Math.Ceiling(count / 5.0);

            if (page > totalPage)
                return RedirectToAction("ProductType", new { type });

            List<Models.ProductInfo> ps = new List<Models.ProductInfo>();
            foreach (DataRow r in tt.Rows)
            {
                int countID = Convert.ToInt32(r["countID"]);
                if (countID <= 5 * (page.Value - 1) || countID > 5 * page.Value)
                    continue;

                Models.ProductInfo pi = new Models.ProductInfo();
                pi.ID = (int)r["proID"];
                pi.Name = r["proName"].ToString();
                pi.Note = r["proNote"].ToString();
                pi.PTName = r["PTName"].ToString();
                pi.Price = Convert.ToInt32(r["proPrice"]);
                pi.Stock = Convert.ToInt32(r["proStock"]);
                pi.Pic = r["proPic"].ToString();
                ps.Add(pi);
            }


            //頁碼
            System.Text.StringBuilder sb2 = new System.Text.StringBuilder();
            for (int i = 1; i <= totalPage; i++)
            {
                if (i == page.Value)
                    sb2.Append(i).Append("&nbsp;&nbsp;");
                else
                    sb2.Append(string.Format("<a style=\"text-decoration:underline\" href='{0}'>{1}</a>&nbsp;&nbsp;", Url.Action("ProductType", new { type, page = i }), i));
            }
            ViewData["PageLink"] = sb2.ToString();
            ViewData["proTypes"] = Session["proTypes"];

            return View(ps);
        }
        public ActionResult ProductSearch(string proName, int? page)
        {
            if (page == null) page = 1;
            DataTable tt;
            if (proName == null)
                tt = new DataTable();
            else
            {
                DB.DB mydb = new DB.DB("SQLAdmin", "admin1234");
                DB.IProduct ip = new DB.Impl_Product(mydb.Connection);
                tt = ip.SearchProductsByName(proName);
            }

            if (page == null) page = 1;
            if (tt == null) tt = new DataTable();
            int count = tt.Rows.Count;

            int totalPage = 1;
            if (count > 5)
                totalPage = (int)Math.Ceiling(count / 5.0);

            if (page > totalPage)
                return RedirectToAction("ProductSearch", new { proName });

            List<Models.ProductInfo> ps = new List<Models.ProductInfo>();
            foreach (DataRow r in tt.Rows)
            {
                int countID = Convert.ToInt32(r["countID"]);
                if (countID <= 5 * (page.Value - 1) || countID > 5 * page.Value)
                    continue;

                Models.ProductInfo pi = new Models.ProductInfo();
                pi.ID = (int)r["proID"];
                pi.Name = r["proName"].ToString();
                pi.Note = r["proNote"].ToString();
                pi.PTName = r["PTName"].ToString();
                pi.Price = Convert.ToInt32(r["proPrice"]);
                pi.Stock = Convert.ToInt32(r["proStock"]);
                pi.Pic = r["proPic"].ToString();
                ps.Add(pi);
            }
            
            //頁碼
            System.Text.StringBuilder sb2 = new System.Text.StringBuilder();
            for (int i = 1; i <= totalPage; i++)
            {
                if (i == page.Value)
                    sb2.Append(i).Append("&nbsp;&nbsp;");
                else
                    sb2.Append(string.Format("<a style=\"text-decoration:underline\" href='{0}'>{1}</a>&nbsp;&nbsp;", Url.Action("ProductSearch", new { proName, page = i }), i));
            }
            ViewData["PageLink"] = sb2.ToString();
            ViewData["proTypes"] = Session["proTypes"];

            return View(ps);
        }
        [HttpPost]
        public ActionResult SentSearch(string Search)
        {
            return RedirectToAction("ProductSearch", new { proName = Search });
        }
        public ActionResult Product(int? pid)
        {
            if (pid == null)
                pid = 1;

            DB.DB mydb = new DB.DB();
            DB.IProduct ip = new DB.Impl_Product(mydb.Connection);
            Dictionary<string, object> dp = ip.GetSingleProduct(pid.Value);

            if (dp == null || (int)dp["proState"] == 0)
                RedirectToAction("Product");

            Models.ProductInfo pi = new Models.ProductInfo();
            pi.ID = (int)dp["proID"];
            pi.Name = dp["proName"].ToString();
            pi.PTName = dp["PTName"].ToString();
            pi.Note = dp["proNote"].ToString();
            pi.Price = Convert.ToInt32(dp["proPrice"]);
            pi.Stock = Convert.ToInt32(dp["proStock"]);
            pi.Pic = dp["proPic"].ToString();

            ViewData["proTypes"] = Session["proTypes"];

            return View(pi);
        }
    }
}