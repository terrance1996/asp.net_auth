using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;

namespace TaskAuth
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.MapPageRoute("defaultRoute", "", "~/HomePage.aspx");
            routes.MapPageRoute("homeRoute", "home", "~/HomePage.aspx");
            routes.MapPageRoute("loginRoute", "login", "~/LoginForm.aspx");
            routes.MapPageRoute("registerRoute", "register", "~/RegisterationForm.aspx");
            routes.MapPageRoute("activateRoute", "activate", "~/ActivateAccount.aspx");
            routes.MapPageRoute("forgotpwdRoute", "forgotpwd", "~/ForgetPasswordForm.aspx");
            routes.MapPageRoute("resetpwdRoute", "resetpwd", "~/ResetPasswordForm.aspx");
        }
    }
}