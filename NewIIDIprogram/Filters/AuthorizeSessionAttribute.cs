using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NewIIDIprogram.Filters
{
    public class AuthorizeSessionAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            string controller = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;
            string action = filterContext.ActionDescriptor.ActionName;

            // Allow login actions without session
            if (controller == "LoginUsers" &&
                (action == "Login" || action == "ValidateLogin"))
            {
                return;
            }

            var email = filterContext.HttpContext.Session["Email"];
            var validOtp = filterContext.HttpContext.Session["ValidOtp"];

            if (email == null || validOtp == null)
            {
                filterContext.Result = new RedirectToRouteResult(
                    new System.Web.Routing.RouteValueDictionary
                    {
                        { "Controller", "LoginUsers" },
                        { "Action", "Login" }
                    }
                );
            }

            base.OnActionExecuting(filterContext);
        }
    }
}
