using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TKM.DAO.EntityFramework;

namespace TKM.DAO.EntityFramework
{
    public partial class NguoiDung
    {
        private static EFUnitOfWork oUnitOfWork = new EFUnitOfWork();
        //private readonly DmChucVuRepository _dmChucVuRespnew = new DmChucVuRepository(new EFRepository<DmChucVu>(), oUnitOfWork);
        //public string ChucVuVaTen {
        //    get
        //    {
        //        var ten = this.HoVaTen;
        //        var objChucVu = _dmChucVuRespnew.GetById(this.ChucVuID.Value);
        //        if (objChucVu != null)
        //            ten = objChucVu.TenChucVu + " " + this.HoVaTen;
        //        return ten;
        //    }
        //}
    }
}
