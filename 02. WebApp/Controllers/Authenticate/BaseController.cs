using TKM.Services;
using TKM.Utils;
using System;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Web.Mvc;
using TuesPechkin;

namespace TKM.WebApp.Controllers
{
    public partial class BaseController : System.Web.Mvc.Controller
    {
        protected string Provider = "entityframework";
        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            //var strPerform = "Start Initialize " + DateTime.Now;
            if (!requestContext.HttpContext.Request.IsAuthenticated)
            {
                if (!requestContext.HttpContext.Response.IsRequestBeingRedirected)
                    requestContext.HttpContext.Response.Redirect(GetLoginUrl(requestContext));
            }
            else
            {

                var license = Encrypt.EncryptAndHash("tkm" + DateTime.Now.Year + DateTime.Now.Month);
                var keyLicense = System.Configuration.ConfigurationManager.AppSettings["license"];
                var checkLicense = Encrypt.CheckLicense(keyLicense);
                if (checkLicense)
                {
                    QuyenService service = new QuyenService();
                    var controller = requestContext.RouteData.Values["controller"].ToString().Trim().ToUpper();
                    var action = requestContext.RouteData.Values["action"].ToString().Trim().ToUpper();
                    var currentAcc = SessionInfo.CurrentUser;
                    //strPerform += "Controller: " + controller + " action: " + action + " " + DateTime.Now;
                    if (currentAcc != null && checkExclusive(controller, action) && !currentAcc.TenDangNhap.ToLower().Equals("admin"))
                    {
                        //Update lai action neu la update/delete thanh quyen cua index
                        if (action.ToUpper().Equals("ADD") || action.ToUpper().Equals("UPDATE") ||
                        action.ToUpper().Contains("DELETE"))
                        {
                            action = "INDEX";
                        }
                        if (controller.ToUpper().Equals("BAOCAOTHONGKE"))
                        {
                            action = "INDEX";
                        }
                        //strPerform += " | Check permission start " + DateTime.Now;
                        //check permission
                        var chkPermission = false;
                        var lstController = requestContext.HttpContext.Session["controller"];
                        var lstAction = requestContext.HttpContext.Session["action"];
                        if (lstController == null && lstAction == null)
                        {
                            var lstQuyens = service.GetDanhSachChucNangByUserId(SessionInfo.CurrentUser.ID);
                            lstController = ",";
                            lstAction = ",";
                            if (lstQuyens != null && lstQuyens.Count > 0)
                            {
                                foreach (var item in lstQuyens.Where(x => x.TrangThai))
                                {
                                    lstController += item.Controller.ToUpper() + ",";
                                    lstAction += item.Action.ToUpper() + ",";
                                }
                                requestContext.HttpContext.Session["controller"] = lstController;
                                requestContext.HttpContext.Session["action"] = lstAction;
                            }
                            var menuHTML = service.GetMenuHtmlByUserId(SessionInfo.CurrentUser.ID);
                            requestContext.HttpContext.Session["menu"] = menuHTML;
                        }
                        if (lstController != null && lstController.ToString().Contains("," + controller.ToUpper() + ",") && lstAction != null && lstAction.ToString().Contains("," + action.ToUpper() + ","))
                        {
                            chkPermission = true;
                        }
                        //QuyenService service = new QuyenService();
                        //var chkPermission = service.CheckPermission(currentAcc.ID, controller, action);
                        //strPerform += " | Check permission end: " + chkPermission + " " + DateTime.Now;
                        if (!chkPermission)
                        {
                            OutputLog.WriteOutputLog("User: " + currentAcc.TenDangNhap + "; Controller: " + controller + "; Action: " + action, "/LogSuDung/InvalidPermission");
                            requestContext.HttpContext.Response.Redirect("/ErrorPages/InvalidPermission");
                        }
                        OutputLog.WriteOutputLog("User: " + SessionInfo.CurrentUser.TenDangNhap + "; Date: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), "/LogSuDung/" + SessionInfo.CurrentUser.TenDangNhap);
                    }
                    base.Initialize(requestContext);

                    if (Session != null && Session["menu"] == null && SessionInfo.CurrentUser != null && SessionInfo.CurrentUser.ID > 0)
                    {
                        var menuHTML = service.GetMenuHtmlByUserId(SessionInfo.CurrentUser.ID);
                        Session["menu"] = menuHTML;
                    }
                }
                else
                {

                    requestContext.HttpContext.Response.Redirect("/ErrorPages/InvalidLicense");
                }
                //strPerform += " | Base Initialize done " + DateTime.Now;
            }
            //strPerform += "\nEnd Initialize " + DateTime.Now;
            //OutputLog.WriteOutputLog(strPerform, "/LogPerformance/");
        }
        private bool checkExclusive(string controller, string action)
        {
            //if (controller.ToUpper().Equals("HOME")
            //#region Common
            //    || action.ToUpper().Equals("INDEXDETAIL")
            //#endregion
            //#region Notification
            //    || (controller.ToUpper().Equals("LOADNOTIFICATION") && action.ToUpper().Equals("MENU"))
            //#endregion
            //#region Menu
            //    || (controller.ToUpper().Equals("MENU") && action.ToUpper().Equals("LOADMENU"))
            //#endregion
            //    )
            //    return false;
            if (!controller.ToUpper().Equals("HOME") && (
                action.ToUpper().Equals("INDEX") ||
                action.ToUpper().Equals("ADD") ||
                action.ToUpper().Equals("UPDATE") ||
                action.ToUpper().Contains("DELETE") ||
                action.ToUpper().Equals("CHOVAOSO") ||
                action.ToUpper().Equals("INSOVANBAN") ||
                action.ToUpper().Equals("BCKQTHUCHIENNHIEMVU") ||
                action.ToUpper().Equals("INDEXVIEW")
                ))
            {
                return true;
            }
            return false;
        }
        private string GetLoginUrl(System.Web.Routing.RequestContext requestContext)
        {
            var redirectUrl = requestContext.HttpContext.Server.UrlEncode(requestContext.HttpContext.Request.Url.PathAndQuery);
            return string.Format("/Authentication/Login?RedirectUrl={0}", redirectUrl);
        }

        /// <summary>
        /// Render with signle view
        /// </summary>
        /// <param name="partialViewName"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        protected virtual string RenderPartialView(string partialViewName, object model)
        {
            var strPerform = "RenderPartialView BaseController" + DateTime.Now;
            OutputLog.WriteOutputLog(strPerform, "/LogPerformance/");
            if (ControllerContext == null)
                return string.Empty;

            if (model == null)
                throw new ArgumentNullException("model");

            if (string.IsNullOrEmpty(partialViewName))
                throw new ArgumentNullException("partialViewName");

            ViewData.Model = model;
            using (var sw = new StringWriter())
            {
                ViewEngineResult viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, partialViewName);
                var viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);
                return sw.GetStringBuilder().ToString();
            }
        }

        public virtual object GetBaseObjectResult(bool isSuccess = true, string msg = "")
        {
            return new
            {
                IsSuccess = isSuccess,
                Message = msg
            };
        }
        protected override void OnException(ExceptionContext filterContext)
        {
            //Log Exception e
            OutputLog.WriteOutputLog(filterContext.Exception);
        }
    }

    public static class TuesPechkinInitializerService
    {
        private static string staticDeploymentPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "wkhtmltopdf");

        public static void CreateWkhtmltopdfPath()
        {
            if (Directory.Exists(staticDeploymentPath) == false)
            {
                Directory.CreateDirectory(staticDeploymentPath);
            }
        }

        public static IConverter converter =
            new ThreadSafeConverter(
                new RemotingToolset<PdfToolset>(
                    new WinAnyCPUEmbeddedDeployment(
                        new StaticDeployment(staticDeploymentPath)
                    )
                )
            );
    }
}
