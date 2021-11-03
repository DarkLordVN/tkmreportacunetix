using TKM.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TKM.WebApp.Controllers
{
    public class MenuController : BaseController
    {
        private QuyenService _quyenService;
        public MenuController()
        {
            if (_quyenService == null)
            {
                _quyenService = new QuyenService();
            }
        }
        public ActionResult LoadMenu()
        {
            if (SessionInfo.CurrentUser != null)
            {
                //var str = _quyenService.GetMenuHtml(SessionInfo.CurrentUser.NhomQuyenID);
                var str = _quyenService.GetMenuHtmlByUserId(SessionInfo.CurrentUser.ID);
                @ViewBag.Html = str;
            }
            return PartialView("_Menu");
        }
    }
}