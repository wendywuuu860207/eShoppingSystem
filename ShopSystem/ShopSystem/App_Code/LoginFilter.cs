using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ShopSystem.App_Code
{
    public class CustomerLoginFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            UrlHelper uu = new UrlHelper(filterContext.HttpContext.Request.RequestContext);

            if (filterContext.HttpContext.Session["Login"] == null
                || string.IsNullOrEmpty(filterContext.HttpContext.Session["Login"].ToString()))
            {
                // 暫存目前原本要瀏覽的網頁
                string ww = filterContext.HttpContext.Request.RawUrl;
                HttpCookie hc = new HttpCookie("ReUrl");
                hc.Value = ww;
                filterContext.HttpContext.Response.Cookies.Add(hc);

                filterContext.HttpContext.Response.Redirect(uu.Action("Index", "Login"));
            }
            else if (filterContext.HttpContext.Session["LoginType"] == null
                || filterContext.HttpContext.Session["LoginType"].ToString() != "c")
                filterContext.HttpContext.Response.Redirect(uu.Action("Index", "Home"));
        }
    }
    public class AdminLoginFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            UrlHelper uu = new UrlHelper(filterContext.HttpContext.Request.RequestContext);

            if (filterContext.HttpContext.Session["Login"] == null
                || string.IsNullOrEmpty(filterContext.HttpContext.Session["Login"].ToString()))
            {
                // 暫存目前原本要瀏覽的網頁
                string ww = filterContext.HttpContext.Request.RawUrl;
                HttpCookie hc = new HttpCookie("ReUrl");
                hc.Value = ww;
                filterContext.HttpContext.Response.Cookies.Add(hc);

                filterContext.HttpContext.Response.Redirect(uu.Action("Index", "Login"));
            }
            else if (filterContext.HttpContext.Session["LoginType"] == null
                || filterContext.HttpContext.Session["LoginType"].ToString() != "a")
                filterContext.HttpContext.Response.Redirect(uu.Action("Index", "Home"));
        }
    }
}