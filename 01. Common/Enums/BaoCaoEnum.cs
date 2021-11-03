using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TKM.Utils.Enums
{
    public enum BaoCaoEnum
    {
        [Description("Thống kê số lượng theo nhóm thiết bị")]
        tkslnhomthietbi = 1,
        [Description("Chi tiết thiết bị theo nhóm")]
        chitiettbtheonhom = 2,
        [Description("Tổng hợp vấn đề theo nhóm")]
        vandenhom = 3,
        [Description("Thống kê chi tiết vấn đề")]
        chitietvande = 4,
        [Description("Hiệu năng CPU máy chủ")]
        cpugroup = 5,
        [Description("Hiệu năng RAM máy chủ")]
        ramgroup = 6,
        [Description("Hiệu năng ổ cứng máy chủ")]
        hddgroup = 7,
        [Description("Băng thông máy chủ")]
        bwdgroup = 8,
        [Description("Hiệu năng CPU thiết bị")]
        cpuhost = 9,
        [Description("Hiệu năng RAM thiết bị")]
        ramhost = 10,
        [Description("Hiệu năng ổ cứng thiết bị")]
        hddhost = 11,
        [Description("Băng thông thiết bị")]
        bwdhost = 12,
        [Description("Báo cáo tổng hợp")]
        bcthall = 13,
        [Description("Đề xuất thiết bị")]
        dexuat = 14
    }
}
