﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace TKM.BLL
{
    public class NhomDonViViewModel
    {
        public int ID { get; set; }
        [AllowHtml]
        [Required(ErrorMessage = "Tên nhóm đơn vị không được để trống")]
        [StringLength(250, ErrorMessage = "Tên nhóm đơn vị không được quá 250 ký tự")]
        public string TenNhom { get; set; }
        public string ListDonViThuocNhomID { get; set; }
        public string DonViTrongNhom { get; set; }

        public string NguoiTao { get; set; }
        public Nullable<System.DateTime> NgayTao { get; set; }
        public int? NguoiTaoID { get; set; }
        public int? NguoiCapNhatID { get; set; }
        public string NguoiCapNhat { get; set; }
        public Nullable<System.DateTime> NgayCapNhat { get; set; }
        public bool TrangThai { get; set; }
        //public bool CheckTrangThai { get { return (TrangThai.HasValue && TrangThai == 2) ? false : true; } set { TrangThai = value ? 1 : 2; } }
        public List<CommonDropDownList> LtsTrangThai { get; set; }
    }
    public class NhomDonViListView : PagedListBase
    {
        [AllowHtml]
        public string TuKhoa { get; set; }
        [AllowHtml]
        public string TenNhom { get; set; }
        public string DonViTrongNhom { get; set; }
        public bool? TrangThai { get; set; }
        public int? srTrangThai { get; set; }
        public List<NhomDonViViewModel> LstNhomDonVi { get; set; }
    }
}
