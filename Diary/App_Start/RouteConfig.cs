﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Diary
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "HashTags",
                url: "Hashtags",
                defaults: new { controller = "HashTags", action = "Index" }
            );

            routes.MapRoute(
                name: "HashTagsPopular",
                url: "Hashtags/Popular",
                defaults: new { controller = "HashTags", action = "Index", popular = true }
            );

            routes.MapRoute(
                name: "Categories",
                url: "Categories/{action}/{id}",
                defaults: new { controller = "Categories", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Security",
                url: "Security/{action}",
                defaults: new { controller = "Security", action = "Login" }
            );

            routes.MapRoute(
                name: "BlogPosts",
                url: "{action}/{id}",
                defaults: new { controller = "Posts", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
