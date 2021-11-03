using System;
using System.Linq;
using System.Collections.Generic;
using TKM.Utils;

namespace TKM.DAO.EntityFramework
{
    public partial class QuyenRepository
    {
        // Add your own data access methods.
        // This file should not get overridden
        //public List<Quyen> GetByNhomQuyenId(int nhomQuyenId)
        //{
        //    var nhomQuyenResp = new EFRepository<NhomQuyenQuyen>(); nhomQuyenResp.UnitOfWork = Repository.UnitOfWork;
        //    try
        //    {

        //        var check = nhomQuyenResp.GetMany(x => x.NhomQuyenID == nhomQuyenId).Select(x => x.QuyenID).ToList();
        //        var lstId = nhomQuyenResp.GetMany(x => x.NhomQuyenID == nhomQuyenId && x.Quyen.TrangThai == true).Select(x => x.QuyenID).ToList();
        //        var lstReturn = new List<Quyen>();
        //        lstReturn = Repository.GetMany(x => lstId.Contains(x.ID)).ToList();
        //        return lstReturn;
        //    }
        //    catch (Exception ex)
        //    {
        //        OutputLog.WriteOutputLog(ex);
        //        return new List<Quyen>();
        //    }
        //}
    }
}