using System;
using System.Linq;
using System.Collections.Generic;
using TKM.Utils;
using System.Transactions;

namespace TKM.DAO.EntityFramework
{   
	public partial class NguoiDungRepository
	{
        // Add your own data access methods.
        // This file should not get overridden
        public bool DoLogin(string username, string password, ref NguoiDung acc,ref string isLocked)
        {
            try
            {
                acc = new NguoiDung();
                acc = Repository.GetByFilter(x => x.TenDangNhap.ToUpper().Trim().Equals(username.ToUpper().Trim()) && x.MatKhau.ToUpper().Equals(password.Trim().ToUpper()));
                
                if (acc.ID > 0 && acc.TrangThai && acc.IsDeleted == false)
                {
                    return true;
                }
                if (acc.TrangThai == false)
                    isLocked = "Tài khoản của bạn đã bị khóa! Vui lòng liên hệ ban quản trị";
                else if (acc.IsDeleted)
                    isLocked = "Tên đăng nhập hoặc mật khẩu không chính xác";
                return false;
            }
            catch (Exception ex)
            {
                OutputLog.WriteOutputLog(ex);
                return false;
            }

        }
        public NguoiDung AddGetObject(NguoiDung entity)
        {
            using (TransactionScope scope = new TransactionScope())
            {
                try
                {
                    Repository.Add(entity);
                    Repository.Save();
                    scope.Complete();
                    scope.Dispose();
                    return entity;
                }
                catch (Exception ex)
                {
                    scope.Dispose();
                    OutputLog.WriteOutputLog(ex);
                    return null;
                }
            }
        }
    }
}