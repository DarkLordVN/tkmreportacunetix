using TKM.BLL;
using TKM.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;

namespace TKM.WebApp
{
    public static class SessionInfo
    {
        public static UserTicketAuthenViewModel CurrentUser
        {
            get
            {
                if (!HttpContext.Current.Request.IsAuthenticated)
                {
                    return null;
                }
                var json = HttpContext.Current.User.Identity.Name;
                var accInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<UserTicketAuthenViewModel>(json);
                return accInfo;
            }
        }
        //public static CauHinhAppViewModel CauHinhApp
        //{
        //    get
        //    {
        //        if (!HttpContext.Current.Request.IsAuthenticated)
        //        {
        //            return null;
        //        }
        //        var cauHinhService = new CauHinhAppService();
        //        var appInfo = cauHinhService.GetById();
        //        appInfo.Header = appInfo.Header.Replace("\n", "<br/>");
        //        return appInfo;
        //    }
        //}
    }
}