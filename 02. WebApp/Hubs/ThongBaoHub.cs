using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using TKM.Services;
using TKM.BLL;

namespace TKM.WebApp.Hubs
{
    [HubName("thongBaoHub")]
    public class ThongBaoHub : Hub
    {
        private NguoiDungService _nguoiDungService;
        private ThongBaoService _thongBaoService;
        public ThongBaoHub()
        {
            if (_nguoiDungService == null) _nguoiDungService = new NguoiDungService();
            if (_thongBaoService == null) _thongBaoService = new ThongBaoService();
        }
        public void UploadNotification(string lstid = "")
        {
            var lstnguoinhan = "";
            var lstidMoiNhat = "";
            var id = 0;
            var objThongBao = new ThongBaoViewModel();
            if (!string.IsNullOrEmpty(lstid) && lstid.Contains(","))
            {
                var arrID = lstid.Split(',');
                foreach (var itemID in arrID)
                {
                    if (!string.IsNullOrEmpty(itemID))
                    {
                        id = Convert.ToInt32(itemID);
                        objThongBao = _thongBaoService.GetById(id);
                        if (objThongBao != null)
                        {
                            lstnguoinhan += objThongBao.LstNguoiNhanID + "[--]";
                            lstidMoiNhat += itemID + ","; 
                        }
                    }
                }
            }

            Clients.All.reloadNotification(lstnguoinhan, lstidMoiNhat);
        }
        public void UploadNotificationRight(int idNew = 0)
        {
            var thongtinnguoigui = " [--] [--] [--] [--] [--] [--] ";
            var objThongBao = _thongBaoService.GetById(idNew);
            if (objThongBao != null)
            {
                var objNguoiGui = _nguoiDungService.GetById(objThongBao.NguoiTaoID.Value);
                if (objNguoiGui != null)
                {
                    thongtinnguoigui = objNguoiGui.AnhDaiDien + "[--]" + objNguoiGui.HoVaTen + "[--]" + objNguoiGui.TenChucVu + "[--]" + objThongBao.NoiDung + "[--]" + TKM.Utils.ConvertDateTime.ConvertDateTimeToTimeString(DateTime.Now) + "[--]" + idNew;
                }
            }

            Clients.All.reloadNotificationRight(thongtinnguoigui, objThongBao.NoiDung, "," + objThongBao.LstNguoiNhanID + ",");
        }
    }
}