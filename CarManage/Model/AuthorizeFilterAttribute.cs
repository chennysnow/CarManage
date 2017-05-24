using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using FrameSystemInfo;

namespace Marep.Web
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class AuthorizeFilterAttribute : ActionFilterAttribute
    {
        int i = 0;
        string html = "<html xmlns=\"http://www.w3.org/1999/xhtml\"><head><meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\"/><title></title></head><body></body></html><script type=\"text/javascript\">  top.document.location.href='/'; </script>";
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext == null)
            {
                throw new ArgumentNullException("filterContext");
            }
            var path = filterContext.HttpContext.Request.Path;
            if ( path.ToString().ToLower().IndexOf("account/login")>=0 )
                return;

            //忽略对Login登录页的权限判定
            //  object[] attrs = filterContext.ActionDescriptor.GetCustomAttributes(typeof(ViewPageAttribute), true);
            //  bool isViewPage = filterContext.ActionDescriptor.IsDefined(typeof(ViewPageAttribute), inherit: true)
            //                        || filterContext.ActionDescriptor.ControllerDescriptor.IsDefined(typeof(ViewPageAttribute), inherit: true);
            // var isViewPage = attrs.Length == 1;
            //当前Action请求是否为具体的功能页
            //  filterContext.Result = new HttpUnauthorizedResult();//直接URL输入的页面地址跳转到登陆页
            
            AuthorizeCore(filterContext);
            base.OnActionExecuting(filterContext);
          //      ViewData 

        }
        //权限判断业务逻辑
        protected virtual bool AuthorizeCore(ActionExecutingContext filterContext)
        {

            // ILog log = Log4Net.Log();
            if (filterContext.HttpContext == null)
            {
                throw new ArgumentNullException("httpContext");
            }
            var dname = filterContext.HttpContext.User.Identity.Name;
            OnlineUser ou = FrameSysteminfo.Instance.online.GetValues(dname);
            if (ou == null)
            {
                i = -3;
                return false;
            }
            var path = filterContext.HttpContext.Request.Path;
            if (path == "/index".ToLower())
            {
                filterContext.HttpContext.Response.Redirect("/");
                return true;
            }

            var controllerName = filterContext.RouteData.Values["controller"].ToString();
            var actionName = filterContext.RouteData.Values["action"].ToString();

            if (controllerName.ToLower() == "share")
            {
                filterContext.HttpContext.Response.Redirect("/");
                return true;
            }
                   
            return true;
        }
    }
    /// <summary>
    /// 表示当前Action请求为一个具体的功能页面
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class ViewPageAttribute : Attribute
    {
    }

    

}
