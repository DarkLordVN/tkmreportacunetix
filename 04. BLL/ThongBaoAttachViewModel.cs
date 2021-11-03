using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace TKM.BLL
{
    public class ThongBaoAttachViewModel
    {
        public int ID { get; set; }
        public int ThongBaoID { get; set; }
        public string LinkFile { get; set; }
        public string NameFile { get; set; }
        public string ReplaceName { get; set; }
        public string Size { get; set; }
        public string Code { get; set; }
        public DateTime NgayTao { get; set; }
        public bool TrangThai { get; set; }

    }
    public class ThongBaoAttachListView : PagedListBase
    {
        public string TuKhoa { get; set; }
        public List<ThongBaoAttachViewModel> LstThongBaoAttach { get; set; }
    }
}
