using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ShopSystem.Controllers
{
    [App_Code.CustomerLoginFilter]
    public class ShopCartController : Controller
    {
        // GET: ShopCart
        public ActionResult Index()
        {
            if (Session["Cart"] == null)
                return RedirectToAction("Index", "Home");

            HashSet<Models.CartItem> myCart =
                Session["Cart"] as HashSet<Models.CartItem>;

            int shipping = 0;
            double amount = 0;
            int total = 0;

            foreach (Models.CartItem ii in myCart)
            {
                amount += ii.SubTotal;
            }

            if (amount < 1000)
                shipping = 150;
            else if (amount < 2000)
                shipping = 80;

            total = (int)Math.Round(amount) + shipping;

            ViewData["Shipping"] = shipping;
            ViewData["Amount"] = (int)Math.Round(amount);
            ViewData["Total"] = total;

            return View();
        }

        public ActionResult AddToCart(int? pid)
        {
            if (pid == null)
                return RedirectToAction("Index", "Home");

            HashSet<Models.CartItem> myCart = null;

            if (Session["Cart"] == null)
                myCart = new HashSet<Models.CartItem>();
            else
                myCart = Session["Cart"] as HashSet<Models.CartItem>;

            Models.CartItem ci = new Models.CartItem();
            ci.ID = pid.Value;

            if (myCart.Contains(ci))
                return RedirectToAction("Index");

            DB.DB mydb = new DB.DB();
            DB.IProduct ip = new DB.Impl_Product(mydb.Connection);
            Dictionary<string, object> pp = ip.GetSingleProduct(pid.Value);

            if (pp == null)
                return RedirectToAction("Index", "Home");

            ci.ID = Convert.ToInt32(pp["proID"]);
            ci.Name = pp["proName"].ToString();
            ci.Stock = Convert.ToInt32(pp["proStock"]);
            ci.Quantity = 1;
            ci.Price = Convert.ToInt32(pp["proPrice"]);
            myCart.Add(ci);
            Session["Cart"] = myCart;
            return RedirectToAction("Index");
        }

        public ActionResult ClearCart()
        {
            Session["Cart"] = null;
            Session.Remove("Cart");
            return RedirectToAction("Index", "Home");
        }

        public ActionResult UpdateCart()
        {
            if (Session["Cart"] == null)
                return RedirectToAction("Index", "Home");

            HashSet<Models.CartItem> myCart =
                Session["Cart"] as HashSet<Models.CartItem>;

            foreach (Models.CartItem ci in myCart)
            {
                int num = int.Parse(Request.Form["pc_" + ci.ID]);
                ci.Quantity = num;
            }
            myCart.RemoveWhere(IsZero);

            if (myCart.Count == 0)
                return ClearCart();

            Session["Cart"] = myCart;
            return RedirectToAction("Index");
        }

        private bool IsZero(Models.CartItem ci)
        {
            return ((ci.Quantity) == 0);
        }

        public ActionResult CheckOut()
        {
            return View();
        }

        [HttpPost]
        public ActionResult PlaceOrder(Models.Order order)
        {
            if (Session["Cart"] == null)
                return RedirectToAction("Index", "Home");

            DB.DB mydb = new DB.DB();
            DB.ICustomer ic = new DB.Impl_Customer(mydb.Connection);
            order.custID = ic.GetCustIDByPhone(Session["Login"].ToString());

            DB.DB mydb2 = new DB.DB("SQLAdmin", "admin1234");
            DB.IOrder io = new DB.Impl_Order(mydb2.Connection);
            order.orderID = io.AddNewOrder(order.custID, order.rcptName, order.rcptPhone, order.rcptAddr, order.DTID, order.Shipping, order.orderNote);
            
            HashSet<Models.CartItem> myCart =
                Session["Cart"] as HashSet<Models.CartItem>;

            List<Dictionary<string, object>> items = new List<Dictionary<string, object>>();

            foreach (Models.CartItem ii in myCart)
            {
                Dictionary<string, object> temp = new Dictionary<string, object>();
                temp["proID"] = ii.ID;
                temp["Quantity"] = ii.Quantity;
                items.Add(temp);
            }

            DB.DB mydb3 = new DB.DB("SQLAdmin", "admin1234");
            DB.IOrder io2 = new DB.Impl_Order(mydb3.Connection);
            bool finish = io2.AddCartItems(order.orderID, items);

            if(finish)
                return RedirectToAction("FinishOrder", new { msg = "已完成訂單!請至個人中心查看!" });
            else
                return RedirectToAction("FinishOrder", new { msg = "訂單失敗!請重新下訂貨聯絡客服!" });
        }

        public ActionResult FinishOrder(string msg)
        {
            ViewData["msg"] = msg;
            return View();
        }
    }
}