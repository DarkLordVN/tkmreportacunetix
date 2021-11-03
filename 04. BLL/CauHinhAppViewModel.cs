using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace TKM.BLL
{
    public class CauHinhAppViewModel
    {
        public int CauHinhAppId { get; set; }
        public string LogoPath { get; set; }
        public string Header { get; set; }
        public string Footer { get; set; }
        public string ListImage { get; set; }
        public List<HttpPostedFileBase> LstImageFile { get; set; }
        public HttpPostedFileBase LogoFile { get; set; }
    }
}
