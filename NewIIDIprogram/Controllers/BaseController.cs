using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace NewIIDIprogram.Controllers
{
    public class BaseController : Controller
    {
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            string controller = filterContext.RouteData.Values["controller"].ToString();
            string action = filterContext.RouteData.Values["action"].ToString();

            // Allow login pages without session
            if (controller == "LoginUsers" &&
                (action == "Login" || action == "ValidateLogin"))
            {
                base.OnActionExecuting(filterContext);
                return;
            }

            // Check login
            if (Session["UserId"] == null)
            {
                filterContext.Result = new RedirectToRouteResult(
                    new RouteValueDictionary
                    {
                        { "controller", "LoginUsers" },
                        { "action", "Login" }
                    });
                return;
            }

            base.OnActionExecuting(filterContext);
        }
    }
}