﻿using System.Web.Mvc;
using System.Web.Routing;

namespace BasicAuthHttpModule
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute("Default", "{controller}/{action}/{id}", new {action = "Index", id = UrlParameter.Optional}
                );
        }
    }
}