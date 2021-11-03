using System.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using TKM.WebApp.Controllers;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Hosting;

namespace TKM.WebApp
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            ////v1
            ////AreaRegistration.RegisterAllAreas();
            ////FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            ////RouteConfig.RegisterRoutes(RouteTable.Routes);
            //v2
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Add(new RazorViewEngine());
            TuesPechkinInitializerService.CreateWkhtmltopdfPath();
        }
        protected void Application_BeginRequest()
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
            Response.Cache.SetNoStore();
        }
    }
}