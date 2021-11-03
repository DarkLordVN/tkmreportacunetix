using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TKM.WebApp.Controllers
{
    public class ErrorPagesController : Controller
    {
        // GET: ErrorPages
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult InvalidPermission()
        {
            return View();
        }

        public ActionResult InvalidLicense()
        {
            return View();
        }
    }
}