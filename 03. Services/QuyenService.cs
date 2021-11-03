using TKM.BLL;
using TKM.DAO;
using TKM.DAO.EntityFramework;
using TKM.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace TKM.Services
{
    public class QuyenService
    {
        private EFUnitOfWork oUnitOfWork = new EFUnitOfWork();
        private readonly QuyenRepository _quyenResp;
        private readonly NhomQuyenRepository _nhomQuyenResp;
        private readonly NguoiDungRepository _nguoiDungResp;
        public QuyenService()
        {
            _quyenResp = new QuyenRepository(new EFRepository<Quyen>(), oUnitOfWork);
            _nhomQuyenResp = new NhomQuyenRepository(new EFRepository<NhomQuyen>(), oUnitOfWork);
            _nguoiDungResp = new NguoiDungRepository(new EFRepository<NguoiDung>(), oUnitOfWork);
        }
        public string GetMenuHtml(int nhomId)
        {
            try
            {
                //var leResult = _quyenResp.GetByNhomQuyenId(nhomId);
                var leResult = getByNhomQuyenId(nhomId);
                StringBuilder strMenu = new StringBuilder();
                BuildMenuHtml(0, leResult, strMenu, 0);
                return strMenu.ToString();
            }
            catch (Exception ex)
            {
                OutputLog.WriteOutputLog(ex);
                return null;
            }
        }
        public string GetMenuHtmlByUserId(int userId)
        {
            try
            {
                StringBuilder strMenu = new StringBuilder();
                if (userId > 0)
                {
                    var leResult = getChucNangByUserId(userId);
                    BuildMenuHtml(0, leResult, strMenu, 0);
                }
                return strMenu.ToString();
            }
            catch (Exception ex)
            {
                OutputLog.WriteOutputLog(ex);
                return null;
            }
        }

        public bool CheckPermission(int userId, string controller, string action)
        {
            var lstChucNang = GetDanhSachChucNangByUserId(userId);
            if (lstChucNang != null && lstChucNang.Count > 0)
            {
                if (lstChucNang.Where(x => x.TrangThai && x.Controller.ToUpper().Equals(controller) && x.Action.ToUpper().Equals(action)).Count() > 0) return true;
            }
            return false;
        }

        public List<QuyenViewModel> GetDanhSachChucNangByUserId(int userId)
        {
            try
            {
                var leResult = getChucNangByUserId(userId);
                if (leResult != null)
                    return leResult.ToListModel();
                return null;
            }
            catch (Exception ex)
            {
                OutputLog.WriteOutputLog(ex);
                return null;
            }
        }
        private List<Quyen> getChucNangByUserId(int userId)
        {
            try
            {
                if (userId > 0)
                {
                    var eUser = _nguoiDungResp.GetById(userId);
                    if (eUser != null)
                    {
                        var leResult = new List<Quyen>();
                        var lstCheck = new List<int>();
                        //if (eUser.NhomQuyenID.HasValue)
                        //{
                        //    leResult.AddRange(getByNhomQuyenId(eUser.NhomQuyenID.Value));
                        //    lstCheck.Add(eUser.NhomQuyenID.Value);
                        //}
                        if (!string.IsNullOrEmpty(eUser.NhomQuyenList))
                        {
                            var lstNhomID = eUser.NhomQuyenList.Split(',');
                            if (lstNhomID != null && lstNhomID.Count() > 0)
                            {
                                foreach (var item in lstNhomID)
                                {
                                    if (!string.IsNullOrEmpty(item))
                                    {
                                        var nhomId = int.Parse(item);
                                        if (!lstCheck.Contains(nhomId))
                                        {
                                            lstCheck.Add(nhomId);
                                            leResult.AddRange(getByNhomQuyenId(nhomId));
                                        }
                                    }
                                }
                            }
                        }
                        return leResult.Distinct().ToList();
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                OutputLog.WriteOutputLog(ex);
                return null;
            }
        }

        public List<QuyenViewModel> GetDanhSachQuyen(int nhomId)
        {
            try
            {
                var leResult = getByNhomQuyenId(nhomId);
                leResult = leResult.Where(x => "HoSoVanPhongPham".Equals(x.Controller)).ToList();
                return leResult.ToListModel();
            }
            catch (Exception ex)
            {
                OutputLog.WriteOutputLog(ex);
                return null;
            }
        }
        public List<QuyenDetailViewModel> GetDdlList(int currID, int khoaChaId = 0, string iconPhanCap = "--")
        {
            try
            {
                var totalItem = 0;
                var leResult = _quyenResp.GetList(x => x.IsDeleted == false, 0, 0, ref totalItem);
                var leReturn = new List<QuyenDetailViewModel>();
                BuildHierachy(khoaChaId, leResult, ref leReturn, null, iconPhanCap, 1, false, currID);
                return leReturn;
            }
            catch (Exception ex)
            {
                OutputLog.WriteOutputLog(ex);
                return null;
            }
        }

        public List<QuyenDetailViewModel> GetList(string tuKhoa, string tenQuyen, string maQuyen, bool? trangThai, int pageIndex, int pageSize, ref int totalItem, string columnName, string orderBy)
        {
            try
            {
                var leReturn = new List<QuyenDetailViewModel>();
                //Expression<Func<Quyen, bool>> where = x => x.IsDeleted == false && ((!String.IsNullOrEmpty(tenQuyen)) ? x.TenQuyen.ToLower().Trim().Contains(tenQuyen.ToLower().Trim()) : true && (!String.IsNullOrEmpty(maQuyen)) ? x.MaQuyen.ToLower().Trim().Contains(maQuyen.ToLower().Trim()) : true) && (trangThai != null ? x.TrangThai == trangThai : true);
                //var leResult = _quyenService.GetList(where, pageIndex, pageSize, ref totalItem,
                //     //Filter theo column
                //     x => (string.IsNullOrEmpty(columnName) ? null :
                //         columnName.Equals("MaQuyen") ? x.MaQuyen :
                //         columnName.Equals("TenQuyen") ? x.TenQuyen : null),
                //     //Order by
                //     orderBy == null || (!string.IsNullOrEmpty(orderBy) && orderBy.Equals("desc")) ? true : false);
                var lstData = _quyenResp.GetList(g => g.IsDeleted == false).OrderBy(g => g.KhoaChaID).ThenBy(g => g.ThuTu).ToList();
                var lstDataFull = _quyenResp.GetList(g => g.IsDeleted == false).OrderBy(g => g.ID);

                if (!string.IsNullOrEmpty(tuKhoa))
                {
                    lstData = lstData.Where(g => (!string.IsNullOrEmpty(g.TenQuyen) && g.TenQuyen.Trim().ToLower().Contains(tuKhoa.ToLower().Trim()))
                    || g.ThuTu.ToString().ToLower().Trim().Contains(tuKhoa.ToLower().Trim())).ToList();
                }
                if (!string.IsNullOrEmpty(tenQuyen))
                {
                    lstData = lstData.Where(g => !string.IsNullOrEmpty(g.TenQuyen) && g.TenQuyen.Trim().ToLower().Contains(tenQuyen.ToLower().Trim())).ToList();
                }
                if (!string.IsNullOrEmpty(maQuyen))
                {
                    lstData = lstData.Where(g => !string.IsNullOrEmpty(g.MaQuyen) && g.MaQuyen.Trim().ToLower().Contains(maQuyen.ToLower().Trim())).ToList();
                }
                if (trangThai != null)
                {
                    lstData = lstData.Where(g => g.TrangThai == trangThai).ToList();
                }
                //DataSearch
                if (!string.IsNullOrEmpty(tenQuyen) || !string.IsNullOrEmpty(maQuyen) || !string.IsNullOrEmpty(tuKhoa) || trangThai != null)
                {

                }
                else
                    lstData = CommonUtils.CreateLevel(lstData);
                //lstData = _entities.Database.SqlQuery<Quyen>("Select * from Quyen where ID IN (" + String.Join(",", lstFinalID) + ")").ToList();
                //lstData = CommonUtils.CreateLevel(lstData);
                Func<Quyen, object> orderCol = x => (string.IsNullOrEmpty(columnName) ? (x.NgayCapNhat.HasValue ? x.NgayCapNhat.Value.ToString("yyyyMMddHHmmss") : x.NgayTao.Value.ToString("yyyyMMddHHmmss")) :
                    
                     columnName.Equals("TenQuyen") ? x.TenQuyen : (x.NgayCapNhat.HasValue ? x.NgayCapNhat.Value.ToString("yyyyMMddHHmmss") : x.NgayTao.Value.ToString("yyyyMMddHHmmss")));
                var isDecending = !string.IsNullOrEmpty(orderBy) && orderBy.Equals("desc") ? true : false;
                if (columnName != null)
                {
                    if (!isDecending) lstData = lstData.OrderBy(orderCol).ToList();
                    else lstData = lstData.OrderByDescending(orderCol).ToList();
                }
                //if (leResult != null && leResult.Count > 0)
                //{
                //    BuildHierachy(0, leResult, ref leReturn);
                //}
                totalItem = lstData.Count();
                lstData = lstData.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                foreach (var entity in lstData)
                {
                    var vm = new QuyenDetailViewModel();
                    vm.TenQuyen = entity.TenQuyen;
                    vm.Level = entity.Level;
                    vm.ID = entity.ID;
                    vm.ThuTu = entity.ThuTu;
                    vm.HienThiMenu = entity.IsMenu;
                    vm.TrangThai = entity.TrangThai;
                    leReturn.Add(vm);
                }
                //var lstIDByDataKey = lstData.Select(g => g.ID).ToArray();
                //var lstFinalID = new List<int>();
                //foreach (var itemID in lstIDByDataKey)
                //{
                //    lstFinalID.AddRange(CommonUtils.GetAllParent(lstDataFull.ToList(), itemID));
                //}
                //if (lstFinalID.Count == 0)
                //    leReturn = null;
                //else
                //{

                //}

                return leReturn;
            }
            catch (Exception ex)
            {
                OutputLog.WriteOutputLog(ex);
                return null;
            }
        }
        public List<QuyenViewModel> GetList(Expression<Func<Quyen, bool>> where)
        {
            try
            {
                var leReturn = _quyenResp.GetList(where);
                return leReturn.ToListModel();
            }
            catch (Exception ex)
            {
                OutputLog.WriteOutputLog(ex);
                return null;
            }
        }
        public List<QuyenViewModel> GetListMaQuyen(string maQuyen, int pageIndex, int pageSize, ref int totalItem)
        {
            try
            {
                Expression<Func<Quyen, bool>> where = x => ((!string.IsNullOrEmpty(maQuyen)) ? x.MaQuyen.ToLower().Trim().Contains(maQuyen.ToLower().Trim()) : true && x.IsDeleted == false);
                var leReturn = _quyenResp.GetList(where, pageIndex, pageSize, ref totalItem);
                foreach(var item in leReturn)
                {
                    var lstNhomQuyenQuyen = item.NhomQuyenQuyens.ToList();
                    if(lstNhomQuyenQuyen != null && lstNhomQuyenQuyen.Count > 0)
                    {
                        foreach (var eLienKet in lstNhomQuyenQuyen)
                        {
                            eLienKet.QuyenChiTiet = "";
                        }
                    }
                }
                return leReturn.ToListModel();
            }
            catch (Exception ex)
            {
                OutputLog.WriteOutputLog(ex);
                return null;
            }
        }

        public QuyenViewModel GetByFilter(Expression<Func<Quyen, bool>> where)
        {
            try
            {
                var eReturn = _quyenResp.GetByFilter(where);
                return eReturn.ToModel();
            }
            catch (Exception ex)
            {
                OutputLog.WriteOutputLog(ex);
                return null;
            }
        }

        public int GetMaxThuTu()
        {
            try
            {
                return _quyenResp.GetList(x => x.TrangThai).Select(x => x.ThuTu).Max() ?? 0;
            }
            catch (Exception ex)
            {
                OutputLog.WriteOutputLog(ex);
                return 0;
            }
        }
        public bool GetCheckTrungThuTu(int so, int id = 0)
        {
            try
            {
                return _quyenResp.CheckByFilter(x => (id > 0 ? x.ID == id : true) && x.ThuTu == so);
            }
            catch (Exception ex)
            {
                OutputLog.WriteOutputLog(ex);
                return false;
            }
        }
        public QuyenViewModel GetById(int id)
        {
            try
            {
                var eReturn = _quyenResp.GetById(id);
                return eReturn.ToModel();
            }
            catch (Exception ex)
            {
                OutputLog.WriteOutputLog(ex);
                return null;
            }
        }
        public bool Update(QuyenViewModel model)
        {
            try
            {
                var entity = model.ToEntity();
                return _quyenResp.Update(entity);
            }
            catch (Exception ex)
            {
                OutputLog.WriteOutputLog(ex);
                return false;
            }
        }
        public bool UpdateThuTu(int soThuTu)
        {
            try
            {
                var lstUpdateThuTu = _quyenResp.GetList(x => x.ThuTu != null && x.ThuTu >= soThuTu).OrderBy(x => x.ThuTu).ToList();
                if (lstUpdateThuTu != null && lstUpdateThuTu.Count() > 0)
                {
                    lstUpdateThuTu.ForEach(x => x.ThuTu = x.ThuTu + 1);
                }
                return _quyenResp.Update(lstUpdateThuTu);
            }
            catch (Exception ex)
            {
                OutputLog.WriteOutputLog(ex);
                return false;
            }
        }

        public bool Add(QuyenViewModel model)
        {
            try
            {
                var entity = model.ToEntity();
                return _quyenResp.Add(entity);
            }
            catch (Exception ex)
            {
                OutputLog.WriteOutputLog(ex);
                return false;
            }
        }

        public bool DeleteList(List<int> listId)
        {
            try
            {
                return _quyenResp.DeleteList(listId);
            }
            catch (Exception ex)
            {
                OutputLog.WriteOutputLog(ex);
                return false;
            }
        }
        public bool Delete(int id)
        {
            try
            {
                return _quyenResp.Delete(id);
            }
            catch (Exception ex)
            {
                OutputLog.WriteOutputLog(ex);
                return false;
            }
        }

        #region Build tree
        public void BuildHierachy(int khoaChaId, List<Quyen> lstAll, ref List<QuyenDetailViewModel> lstReturn, string phanCapStyle = null, string iconPhanCapDDL = null, int deepLv = 1, bool showChild = true, int currentId = 0)
        {
            //Check nếu list trả về null thì khởi tạo
            if (lstReturn == null) lstReturn = new List<QuyenDetailViewModel>();
            //Get 
            var results = lstAll.Where(x => x.KhoaChaID == khoaChaId).OrderBy(x => x.ThuTu).ThenBy(x => x.TenQuyen);
            if (results.Any())
            {
                //Get icon phân cấp nếu không phải root
                var icon = string.Empty;
                if (deepLv > 1)
                    icon = getIconPhanCapLevel(deepLv, phanCapStyle, iconPhanCapDDL);
                foreach (var eQuyen in results)
                {
                    var item = new QuyenDetailViewModel()
                    {
                        ID = eQuyen.ID,
                        KhoaChaID = eQuyen.KhoaChaID,
                        MaQuyen = eQuyen.MaQuyen,
                        TenQuyen = icon + eQuyen.TenQuyen,
                        HienThiMenu = eQuyen.IsMenu,
                        DataLevel = deepLv,
                        TrangThai = eQuyen.TrangThai,
                        ThuTu = eQuyen.ThuTu,
                        HasChild = lstAll.Where(x => x.KhoaChaID == eQuyen.ID).Count() > 0 ? true : false
                    };
                    if (item.ID != currentId) lstReturn.Add(item);
                    if (!showChild && currentId == item.ID) { continue; }
                    BuildHierachy(item.ID, lstAll, ref lstReturn, phanCapStyle, iconPhanCapDDL, (deepLv + 1), showChild, currentId);
                }
            }
        }

        private string getIconPhanCapLevel(int deepLv, string phanCapStyle, string iconPhanCapDLL)
        {
            if (!string.IsNullOrEmpty(phanCapStyle))
            {
                string newLevelIcon = phanCapStyle.Replace(": 10px", string.Format(": {0}px", deepLv * 10));
                return newLevelIcon;
            }
            else
            {
                string newIcon = string.Empty;
                for (int i = 1; i < deepLv; i++)
                {
                    newIcon += iconPhanCapDLL;
                }
                return newIcon;
            }
        }

        private void BuildMenuHtml(int khoaChaId, List<Quyen> lstAll, StringBuilder strBuilder, int dataLevel)
        {
            //Get 
            var results = lstAll.Where(x => x.KhoaChaID == khoaChaId && x.IsMenu).OrderBy(x => x.ThuTu).ThenBy(x => x.TenQuyen);
            if (results.Any())
            {
                foreach (var eQuyen in results)
                {
                    var hasChild = lstAll.Where(x => x.KhoaChaID == eQuyen.ID).Count() > 0;
                    strBuilder.Append("<li>");
                    strBuilder.Append("<a class=\"collapsible-header waves-effect arrow-r\"" + (hasChild ? "" : " href=\"" + (string.IsNullOrEmpty(eQuyen.Controller) ? "#" : ("/" + eQuyen.Controller + (string.IsNullOrEmpty(eQuyen.Action) ? "" : "/" + eQuyen.Action)))) + "\"><i class=\"" + eQuyen.IconPath + " mr-2\"></i>" + eQuyen.TenQuyen + (hasChild ? "<i class=\"fas fa-angle-down rotate-icon\"></i>" : "") +"</a>");
                    if (hasChild && dataLevel == 0)
                    {
                        strBuilder.Append("<div class=\"collapsible-body\"><ul>");
                    }
                    if (dataLevel < 2) BuildMenuHtml(eQuyen.ID, lstAll, strBuilder, dataLevel + 1);
                    if (hasChild && dataLevel == 0)
                    {
                        strBuilder.Append("</ul></div>");
                    }
                    if (dataLevel == 0) strBuilder.Append("</li>");
                }
            }
        }

        private List<Quyen> getByNhomQuyenId(int nhomQuyenId)
        {
            try
            {
                NhomQuyenService ser = new NhomQuyenService();
                var lstId = ser.getQuyenByNhomQuyenId(nhomQuyenId);
                var lstReturn = new List<Quyen>();
                if(lstId != null && lstId.Count > 0) lstReturn = _quyenResp.GetList(x => lstId.Contains(x.ID)).ToList();
                return lstReturn;
            }
            catch (Exception ex)
            {
                OutputLog.WriteOutputLog(ex);
                return new List<Quyen>();
            }
        }
        #endregion
    }
}
