using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ShopSystem.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login

        #region 客戶登入
        public ActionResult Index(string msg)
        {
            ViewData["msg"] = msg;

            return View();
        }

        [HttpPost]
        public ActionResult ValidLoginProcess(Models.LoginInfo li)
        {
            if(li.Phone==null)
                return RedirectToAction("Index", new { msg = "請輸入手機號碼" });
            if (li.PW == null)
                return RedirectToAction("Index", new { msg = "請輸入密碼" });

            DB.DB mydb = new DB.DB();
            DB.ICustomer ic = new DB.Impl_Customer(mydb.Connection);

            if (ic.CheckLogin(li.Phone, li.PW))
            {
                Session.Add("Login", li.Phone);
                Session.Add("LoginType", "c");

                // 取出User原先瀏覽的網頁
                HttpCookie hc = Request.Cookies["ReUrl"];
                if (hc == null)
                    return RedirectToAction("Index", "Home");
                else
                    return Redirect(hc.Value);
            }
            else
            {
                return RedirectToAction("Index", new { msg = "登入失敗!手機號碼或密碼有誤!" });
            }
        }
        #endregion

        #region 管理員登入
        public ActionResult AdminIndex(string msg)
        {
            ViewData["msg"] = msg;

            return View();
        }

        [HttpPost]
        public ActionResult AdminValidLoginProcess(Models.LoginInfo li)
        {
            if (li.Phone == null)
                return RedirectToAction("AdminIndex", new { msg = "請輸入帳號" });
            if (li.PW == null)
                return RedirectToAction("AdminIndex", new { msg = "請輸入密碼" });

            DB.DB mydb = new DB.DB();
            DB.IAdmin ia = new DB.Impl_Admin(mydb.Connection);

            if (ia.CheckLogin(li.Phone, li.PW))
            {
                Session.Add("Login", li.Phone);
                Session.Add("LoginType", "a");

                HttpCookie hc = Request.Cookies["ReUrl"];
                if (hc == null)
                    return RedirectToAction("Index", "Home");
                else
                    return Redirect(hc.Value);
            }
            else
            {
                return RedirectToAction("AdminIndex", new { msg = "登入失敗!帳號或密碼有誤!" });
            }
        }

        #endregion

        #region 登出
        public ActionResult Logout()
        {
            if (Session["Login"] != null && !string.IsNullOrEmpty(Session["Login"].ToString()))
            {
                Session.Remove("Login");
                Session.Remove("LoginType");
            }
            return RedirectToAction("Index", "Home");
        }
        #endregion

        #region 檢查登入狀況
        public string CheckValidLogin()
        {
            if (Session["Login"] == null || string.IsNullOrEmpty(Session["Login"].ToString()))
                return "f";
            return "t";
        }
        public string CheckLoginType()
        {
            if (Session["LoginType"] == null || string.IsNullOrEmpty(Session["LoginType"].ToString()))
                return "f";
            return Session["LoginType"].ToString();
        }
        #endregion

        #region 加入會員
        public ActionResult NewMemberInfo()
        {
            return View();
        }
        [HttpPost]
        public ActionResult AddNewMember(Models.MemberInfo mi)
        {
            DB.DB mydb = new DB.DB("SQLAdmin", "admin1234");
            DB.ICustomer ic = new DB.Impl_Customer(mydb.Connection);

            ic.AddNewMember(mi.Phone, mi.PW, mi.Name, mi.Email);

            return RedirectToAction("Index", "Login", new { msg = "成功加入會員，請登入!" });
        }

        public string CheckPhone(string phone)
        {
            if (phone.Length != 10 || phone.Substring(0, 2) != "09")
                return "l";
            for (int i = 2; i < 10; i++)
                if (phone[i] > '9' || phone[i] < '0')
                    return "l";

            DB.DB mydb = new DB.DB();
            DB.ICustomer ic = new DB.Impl_Customer(mydb.Connection);

            bool ans = ic.CheckPhoneDuplicate(phone);

            if (ans)
                return "t";
            else
                return "f";
        }
        #endregion

        #region 忘記密碼
        public ActionResult ForgetPassword()
        {
            return View();
        }
        #endregion
    }
}