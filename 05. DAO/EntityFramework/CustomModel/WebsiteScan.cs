using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TKM.DAO.EntityFramework;

namespace TKM.DAO.EntityFramework
{
    public partial class WebsiteScan
    {
        public Website eWebsite { get; set; }
        public List<WebsiteItem> LsWebsiteItem { get; set; }
        public List<ScannedItem> LsScannedItem { get; set; }
        public List<AlertGroup> LsAlertGroup { get; set; }
        public List<AffectedItem> LsAffectedItem { get; set; }
    }

    public partial class AffectedItem
    {
        public AlertGroup eAlertGroup { get; set; }
    }
}
