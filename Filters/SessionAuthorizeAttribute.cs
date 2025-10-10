using System;
using System.Web.Mvc;
using System.Web.Routing;

namespace StudyGroups.Filters
{
    public class SessionAuthorizeAttribute : ActionFilterAttribute
    {
        public string Roles { get; set; }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var session = filterContext.HttpContext.Session;

           
            if (session["UserID"] == null)
            {
                // Redirect to login if not authenticated
                filterContext.Result = new RedirectToRouteResult(
                    new RouteValueDictionary(new { controller = "Account", action = "Login" }));
                return;
            }

            if (!string.IsNullOrEmpty(Roles))
            {
                var userRole = session["UserRole"]?.ToString();
                var allowedRoles = Roles.Split(',');

                bool isAuthorized = false;
                foreach (var role in allowedRoles)
                {
                    if (userRole?.Equals(role.Trim(), StringComparison.OrdinalIgnoreCase) == true)
                    {
                        isAuthorized = true;
                        break;
                    }
                }

                if (!isAuthorized)
                {
                    // Redirect to unauthorized page if role doesn't match
                    filterContext.Result = new RedirectToRouteResult(
                        new RouteValueDictionary(new { controller = "Home", action = "Unauthorized" }));
                    return;
                }
            }

            base.OnActionExecuting(filterContext);
        }
    }
}