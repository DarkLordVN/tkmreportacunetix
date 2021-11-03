using TKM.DAO.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;

namespace TKM.BLL
{
    public static class AutoMapper
    {
        private static MapperConfiguration _mapperConfiguration;
        private static IMapper _mapper;

        #region Constructor
        public static IMapper Mapper
        {
            get
            {
                if (_mapper == null)
                    Init();
                return _mapper;
            }
        }
        public static MapperConfiguration MapperConfiguration
        {
            get
            {
                return _mapperConfiguration;
            }
        }

        //Map extension
        public static TDestination MapTo<TSource, TDestination>(this TSource source)
        {
            return AutoMapper.Mapper.Map<TSource, TDestination>(source);
        }

        public static TDestination MapTo<TSource, TDestination>(this TSource source, TDestination destination)
        {
            return Mapper.Map(source, destination);
        }
        #endregion

        public static void Init()
        {
            _mapperConfiguration = new MapperConfiguration(cfg =>
            {
                //Quyen
                cfg.CreateMap<Quyen, QuyenViewModel>();
                cfg.CreateMap<QuyenViewModel, Quyen>();
                //NguoiDung
                cfg.CreateMap<NguoiDung, NguoiDungViewModel>();
                cfg.CreateMap<NguoiDungViewModel, NguoiDung>();
                //NhatKyHeThong
                cfg.CreateMap<NhatKyHeThong, NhatKyHeThongViewModel>();
                cfg.CreateMap<NhatKyHeThongViewModel, NhatKyHeThong>();
                //NhomQuyen
                cfg.CreateMap<NhomQuyen, NhomQuyenViewModel>();
                cfg.CreateMap<NhomQuyenViewModel, NhomQuyen>();
                //NhomNguoiDung
                cfg.CreateMap<NhomNguoiDung, NhomNguoiDungViewModel>();
                cfg.CreateMap<NhomNguoiDungViewModel, NhomNguoiDung>();
               
                //ThongBao
                cfg.CreateMap<ThongBao, ThongBaoViewModel>();
                cfg.CreateMap<ThongBaoViewModel, ThongBao>();
                //HeThongThamSo
                cfg.CreateMap<HeThongThamSo, HeThongThamSoViewModel>();
                cfg.CreateMap<HeThongThamSoViewModel, HeThongThamSo>();
                //AffectedItem
                cfg.CreateMap<AffectedItem, AffectedItemViewModel>();
                cfg.CreateMap<AffectedItemViewModel, AffectedItem>();
                //AlertGroup
                cfg.CreateMap<AlertGroup, AlertGroupViewModel>();
                cfg.CreateMap<AlertGroupViewModel, AlertGroup>();
                //ScannedItem
                cfg.CreateMap<ScannedItem, ScannedItemViewModel>();
                cfg.CreateMap<ScannedItemViewModel, ScannedItem>();
                //TuDienVietHoa
                cfg.CreateMap<TuDienVietHoa, TuDienVietHoaViewModel>();
                cfg.CreateMap<TuDienVietHoaViewModel, TuDienVietHoa>();
                //WebsiteScan
                cfg.CreateMap<WebsiteScan, WebsiteScanViewModel>();
                cfg.CreateMap<WebsiteScanViewModel, WebsiteScan>();
                //Website
                cfg.CreateMap<Website, WebsiteViewModel>();
                cfg.CreateMap<WebsiteViewModel, Website>();
                //WebsiteItem
                cfg.CreateMap<WebsiteItem, WebsiteItemViewModel>();
                cfg.CreateMap<WebsiteItemViewModel, WebsiteItem>();
            });
            _mapper = _mapperConfiguration.CreateMapper();
        }

        #region QuyenEntity

        public static QuyenViewModel ToModel(this Quyen entity)
        {
            return entity.MapTo<Quyen, QuyenViewModel>();
        }

        public static Quyen ToEntity(this QuyenViewModel model)
        {
            return model.MapTo<QuyenViewModel, Quyen>();
        }

        public static List<QuyenViewModel> ToListModel(this List<Quyen> lEntity)
        {
            var lModel = new List<QuyenViewModel>();
            if (lEntity != null && lEntity.Count > 0)
            {
                foreach (var entity in lEntity)
                {
                    lModel.Add(entity.MapTo<Quyen, QuyenViewModel>());
                }
            }
            return lModel;
        }

        public static List<Quyen> ToListEntity(this List<QuyenViewModel> lModel)
        {
            var lEntity = new List<Quyen>();
            if (lModel != null && lModel.Count > 0)
            {
                foreach (var model in lModel)
                {
                    lEntity.Add(model.MapTo<QuyenViewModel, Quyen>());
                }
            }
            return lEntity;
        }
        #endregion

        #region NguoiDungEntity

        public static NguoiDungViewModel ToModel(this NguoiDung entity)
        {
            var vm = entity.MapTo<NguoiDung, NguoiDungViewModel>();
            return vm;
        }

        public static NguoiDung ToEntity(this NguoiDungViewModel model)
        {
            var entity = model.MapTo<NguoiDungViewModel, NguoiDung>();
            if (string.IsNullOrEmpty(entity.MatKhau)) entity.MatKhau = "123123";
            return entity;
        }


        public static List<NguoiDungViewModel> ToListModel(this List<NguoiDung> lEntity)
        {
            var lModel = new List<NguoiDungViewModel>();
            if (lEntity != null && lEntity.Count > 0)
            {
                foreach (var entity in lEntity)
                {
                    lModel.Add(entity.ToModel());
                }
            }
            return lModel;
        }

        public static List<NguoiDung> ToListEntity(this List<NguoiDungViewModel> lModel)
        {
            var lEntity = new List<NguoiDung>();
            if (lModel != null && lModel.Count > 0)
            {
                foreach (var model in lModel)
                {
                    lEntity.Add(model.MapTo<NguoiDungViewModel, NguoiDung>());
                }
            }
            return lEntity;
        }
        #endregion

        #region NhatKyHeThongEntity

        public static NhatKyHeThongViewModel ToModel(this NhatKyHeThong entity)
        {
            var item = entity.MapTo<NhatKyHeThong, NhatKyHeThongViewModel>();
            //item.TenNguoiDung = entity.NguoiDung != null ? entity.NguoiDung.HoVaTen : "";
            return item;
        }

        public static NhatKyHeThong ToEntity(this NhatKyHeThongViewModel model)
        {
            return model.MapTo<NhatKyHeThongViewModel, NhatKyHeThong>();
        }

        public static List<NhatKyHeThongViewModel> ToListModel(this List<NhatKyHeThong> lEntity)
        {
            var lModel = new List<NhatKyHeThongViewModel>();
            if (lEntity != null && lEntity.Count > 0)
            {
                foreach (var entity in lEntity)
                {
                    var item = entity.MapTo<NhatKyHeThong, NhatKyHeThongViewModel>();
                    //item.TenNguoiDung = entity.NguoiDung != null ? entity.NguoiDung.HoVaTen : "";
                    lModel.Add(item);
                }
            }
            return lModel;
        }

        public static List<NhatKyHeThong> ToListEntity(this List<NhatKyHeThongViewModel> lModel)
        {
            var lEntity = new List<NhatKyHeThong>();
            if (lModel != null && lModel.Count > 0)
            {
                foreach (var model in lModel)
                {
                    lEntity.Add(model.MapTo<NhatKyHeThongViewModel, NhatKyHeThong>());
                }
            }
            return lEntity;
        }
        #endregion
        
        #region NhomQuyenEntity

        public static NhomQuyenViewModel ToModel(this NhomQuyen entity)
        {
            return entity.MapTo<NhomQuyen, NhomQuyenViewModel>();
        }

        public static NhomQuyen ToEntity(this NhomQuyenViewModel model)
        {
            return model.MapTo<NhomQuyenViewModel, NhomQuyen>();
        }

        public static List<NhomQuyenViewModel> ToListModel(this List<NhomQuyen> lEntity)
        {
            var lModel = new List<NhomQuyenViewModel>();
            if (lEntity != null && lEntity.Count > 0)
            {
                foreach (var entity in lEntity)
                {
                    lModel.Add(entity.MapTo<NhomQuyen, NhomQuyenViewModel>());
                }
            }
            return lModel;
        }

        public static List<NhomQuyen> ToListEntity(this List<NhomQuyenViewModel> lModel)
        {
            var lEntity = new List<NhomQuyen>();
            if (lModel != null && lModel.Count > 0)
            {
                foreach (var model in lModel)
                {
                    lEntity.Add(model.MapTo<NhomQuyenViewModel, NhomQuyen>());
                }
            }
            return lEntity;
        }
        #endregion
      
        #region NhomNguoiDungEntity

        public static NhomNguoiDungViewModel ToModel(this NhomNguoiDung entity)
        {
            return entity.MapTo<NhomNguoiDung, NhomNguoiDungViewModel>();
        }

        public static NhomNguoiDung ToEntity(this NhomNguoiDungViewModel model)
        {
            return model.MapTo<NhomNguoiDungViewModel, NhomNguoiDung>();
        }

        public static List<NhomNguoiDungViewModel> ToListModel(this List<NhomNguoiDung> lEntity)
        {
            var lModel = new List<NhomNguoiDungViewModel>();
            if (lEntity != null && lEntity.Count > 0)
            {
                foreach (var entity in lEntity)
                {
                    lModel.Add(entity.MapTo<NhomNguoiDung, NhomNguoiDungViewModel>());
                }
            }
            return lModel;
        }

        public static List<NhomNguoiDung> ToListEntity(this List<NhomNguoiDungViewModel> lModel)
        {
            var lEntity = new List<NhomNguoiDung>();
            if (lModel != null && lModel.Count > 0)
            {
                foreach (var model in lModel)
                {
                    lEntity.Add(model.MapTo<NhomNguoiDungViewModel, NhomNguoiDung>());
                }
            }
            return lEntity;
        }
        #endregion
        
        #region HeThongThamSoEntity

        public static HeThongThamSoViewModel ToModel(this HeThongThamSo entity)
        {
            return entity.MapTo<HeThongThamSo, HeThongThamSoViewModel>();
        }

        public static HeThongThamSo ToEntity(this HeThongThamSoViewModel model)
        {
            return model.MapTo<HeThongThamSoViewModel, HeThongThamSo>();
        }

        public static List<HeThongThamSoViewModel> ToListModel(this List<HeThongThamSo> lEntity)
        {
            var lModel = new List<HeThongThamSoViewModel>();
            if (lEntity != null && lEntity.Count > 0)
            {
                foreach (var entity in lEntity)
                {
                    lModel.Add(entity.MapTo<HeThongThamSo, HeThongThamSoViewModel>());
                }
            }
            return lModel;
        }

        public static List<HeThongThamSo> ToListEntity(this List<HeThongThamSoViewModel> lModel)
        {
            var lEntity = new List<HeThongThamSo>();
            if (lModel != null && lModel.Count > 0)
            {
                foreach (var model in lModel)
                {
                    lEntity.Add(model.MapTo<HeThongThamSoViewModel, HeThongThamSo>());
                }
            }
            return lEntity;
        }
        #endregion
        
        #region ThongBaoEntity

        public static ThongBaoViewModel ToModel(this ThongBao entity)
        {
            var item = entity.MapTo<ThongBao, ThongBaoViewModel>();
            item.ChucNang = item.ChucNangID != 0 && item.ChucNangID != null ? TKM.Utils.ObjectUtils.GetDescription((TKM.Utils.Enums.LoaiLienKetVanBan)item.ChucNangID) : "Thông báo";
            return item;
        }

        public static ThongBao ToEntity(this ThongBaoViewModel model)
        {
            return model.MapTo<ThongBaoViewModel, ThongBao>();
        }

        public static List<ThongBaoViewModel> ToListModel(this List<ThongBao> lEntity)
        {
            var lModel = new List<ThongBaoViewModel>();
            if (lEntity != null && lEntity.Count > 0)
            {
                foreach (var entity in lEntity)
                {
                    var item = entity.MapTo<ThongBao, ThongBaoViewModel>();
                    item.ChucNang = item.ChucNangID != 0 && item.ChucNangID != null ? TKM.Utils.ObjectUtils.GetDescription((TKM.Utils.Enums.ChucNang)item.ChucNangID) : "Thông báo";
                    lModel.Add(item);
                }
            }
            return lModel;
        }

        public static List<ThongBao> ToListEntity(this List<ThongBaoViewModel> lModel)
        {
            var lEntity = new List<ThongBao>();
            if (lModel != null && lModel.Count > 0)
            {
                foreach (var model in lModel)
                {
                    lEntity.Add(model.MapTo<ThongBaoViewModel, ThongBao>());
                }
            }
            return lEntity;
        }
        #endregion

        #region ThongBaoAttachEntity

        public static ThongBaoAttachViewModel ToModel(this ThongBaoAttach entity)
        {
            return entity.MapTo<ThongBaoAttach, ThongBaoAttachViewModel>();
        }

        public static ThongBaoAttach ToEntity(this ThongBaoAttachViewModel model)
        {
            return model.MapTo<ThongBaoAttachViewModel, ThongBaoAttach>();
        }

        public static List<ThongBaoAttachViewModel> ToListModel(this List<ThongBaoAttach> lEntity)
        {
            var lModel = new List<ThongBaoAttachViewModel>();
            if (lEntity != null && lEntity.Count > 0)
            {
                foreach (var entity in lEntity)
                {
                    lModel.Add(entity.MapTo<ThongBaoAttach, ThongBaoAttachViewModel>());
                }
            }
            return lModel;
        }

        public static List<ThongBaoAttach> ToListEntity(this List<ThongBaoAttachViewModel> lModel)
        {
            var lEntity = new List<ThongBaoAttach>();
            if (lModel != null && lModel.Count > 0)
            {
                foreach (var model in lModel)
                {
                    lEntity.Add(model.MapTo<ThongBaoAttachViewModel, ThongBaoAttach>());
                }
            }
            return lEntity;
        }
        #endregion

        #region AffectedItemEntity

        public static AffectedItemViewModel ToModel(this AffectedItem entity)
        {
            var item = entity.MapTo<AffectedItem, AffectedItemViewModel>();
            return item;
        }

        public static AffectedItem ToEntity(this AffectedItemViewModel model)
        {
            return model.MapTo<AffectedItemViewModel, AffectedItem>();
        }

        public static List<AffectedItemViewModel> ToListModel(this List<AffectedItem> lEntity)
        {
            var lModel = new List<AffectedItemViewModel>();
            if (lEntity != null && lEntity.Count > 0)
            {
                foreach (var entity in lEntity)
                {
                    var item = entity.MapTo<AffectedItem, AffectedItemViewModel>();
                   lModel.Add(item);
                }
            }
            return lModel;
        }

        public static List<AffectedItem> ToListEntity(this List<AffectedItemViewModel> lModel)
        {
            var lEntity = new List<AffectedItem>();
            if (lModel != null && lModel.Count > 0)
            {
                foreach (var model in lModel)
                {
                    var entity = model.MapTo<AffectedItemViewModel, AffectedItem>();
                    entity.eAlertGroup = model.vmAlertGroup.ToEntity();
                    lEntity.Add(entity);
                }
            }
            return lEntity;
        }
        #endregion

        #region AlertGroupEntity

        public static AlertGroupViewModel ToModel(this AlertGroup entity)
        {
            var item = entity.MapTo<AlertGroup, AlertGroupViewModel>();
            return item;
        }

        public static AlertGroup ToEntity(this AlertGroupViewModel model)
        {
            return model.MapTo<AlertGroupViewModel, AlertGroup>();
        }

        public static List<AlertGroupViewModel> ToListModel(this List<AlertGroup> lEntity)
        {
            var lModel = new List<AlertGroupViewModel>();
            if (lEntity != null && lEntity.Count > 0)
            {
                foreach (var entity in lEntity)
                {
                    var item = entity.MapTo<AlertGroup, AlertGroupViewModel>();
                    lModel.Add(item);
                }
            }
            return lModel;
        }

        public static List<AlertGroup> ToListEntity(this List<AlertGroupViewModel> lModel)
        {
            var lEntity = new List<AlertGroup>();
            if (lModel != null && lModel.Count > 0)
            {
                foreach (var model in lModel)
                {
                    lEntity.Add(model.MapTo<AlertGroupViewModel, AlertGroup>());
                }
            }
            return lEntity;
        }
        #endregion

        #region ScannedItemEntity

        public static ScannedItemViewModel ToModel(this ScannedItem entity)
        {
            var item = entity.MapTo<ScannedItem, ScannedItemViewModel>();
            return item;
        }

        public static ScannedItem ToEntity(this ScannedItemViewModel model)
        {
            return model.MapTo<ScannedItemViewModel, ScannedItem>();
        }

        public static List<ScannedItemViewModel> ToListModel(this List<ScannedItem> lEntity)
        {
            var lModel = new List<ScannedItemViewModel>();
            if (lEntity != null && lEntity.Count > 0)
            {
                foreach (var entity in lEntity)
                {
                    var item = entity.MapTo<ScannedItem, ScannedItemViewModel>();
                    lModel.Add(item);
                }
            }
            return lModel;
        }

        public static List<ScannedItem> ToListEntity(this List<ScannedItemViewModel> lModel)
        {
            var lEntity = new List<ScannedItem>();
            if (lModel != null && lModel.Count > 0)
            {
                foreach (var model in lModel)
                {
                    lEntity.Add(model.MapTo<ScannedItemViewModel, ScannedItem>());
                }
            }
            return lEntity;
        }
       
        #endregion

        #region TuDienVietHoaEntity

        public static TuDienVietHoaViewModel ToModel(this TuDienVietHoa entity)
        {
            var item = entity.MapTo<TuDienVietHoa, TuDienVietHoaViewModel>();
            return item;
        }

        public static TuDienVietHoa ToEntity(this TuDienVietHoaViewModel model)
        {
            return model.MapTo<TuDienVietHoaViewModel, TuDienVietHoa>();
        }

        public static List<TuDienVietHoaViewModel> ToListModel(this List<TuDienVietHoa> lEntity)
        {
            var lModel = new List<TuDienVietHoaViewModel>();
            if (lEntity != null && lEntity.Count > 0)
            {
                foreach (var entity in lEntity)
                {
                    var item = entity.MapTo<TuDienVietHoa, TuDienVietHoaViewModel>();
                    lModel.Add(item);
                }
            }
            return lModel;
        }

        public static List<TuDienVietHoa> ToListEntity(this List<TuDienVietHoaViewModel> lModel)
        {
            var lEntity = new List<TuDienVietHoa>();
            if (lModel != null && lModel.Count > 0)
            {
                foreach (var model in lModel)
                {
                    lEntity.Add(model.MapTo<TuDienVietHoaViewModel, TuDienVietHoa>());
                }
            }
            return lEntity;
        }
        #endregion

        #region WebsiteScanEntity

        public static WebsiteScanViewModel ToModel(this WebsiteScan entity)
        {
            var item = entity.MapTo<WebsiteScan, WebsiteScanViewModel>();
            return item;
        }

        public static WebsiteScan ToEntity(this WebsiteScanViewModel model)
        {
            return model.MapTo<WebsiteScanViewModel, WebsiteScan>();
        }

        public static List<WebsiteScanViewModel> ToListModel(this List<WebsiteScan> lEntity)
        {
            var lModel = new List<WebsiteScanViewModel>();
            if (lEntity != null && lEntity.Count > 0)
            {
                foreach (var entity in lEntity)
                {
                    var item = entity.MapTo<WebsiteScan, WebsiteScanViewModel>();
                    lModel.Add(item);
                }
            }
            return lModel;
        }

        public static List<WebsiteScan> ToListEntity(this List<WebsiteScanViewModel> lModel)
        {
            var lEntity = new List<WebsiteScan>();
            if (lModel != null && lModel.Count > 0)
            {
                foreach (var model in lModel)
                {
                    lEntity.Add(model.MapTo<WebsiteScanViewModel, WebsiteScan>());
                }
            }
            return lEntity;
        }
        #endregion

        #region WebsiteEntity

        public static WebsiteViewModel ToModel(this Website entity)
        {
            var item = entity.MapTo<Website, WebsiteViewModel>();
            return item;
        }

        public static Website ToEntity(this WebsiteViewModel model)
        {
            return model.MapTo<WebsiteViewModel, Website>();
        }

        public static List<WebsiteViewModel> ToListModel(this List<Website> lEntity)
        {
            var lModel = new List<WebsiteViewModel>();
            if (lEntity != null && lEntity.Count > 0)
            {
                foreach (var entity in lEntity)
                {
                    var item = entity.MapTo<Website, WebsiteViewModel>();
                    lModel.Add(item);
                }
            }
            return lModel;
        }

        public static List<Website> ToListEntity(this List<WebsiteViewModel> lModel)
        {
            var lEntity = new List<Website>();
            if (lModel != null && lModel.Count > 0)
            {
                foreach (var model in lModel)
                {
                    lEntity.Add(model.MapTo<WebsiteViewModel, Website>());
                }
            }
            return lEntity;
        }
        #endregion

        #region WebsiteItemEntity

        public static WebsiteItemViewModel ToModel(this WebsiteItem entity)
        {
            var item = entity.MapTo<WebsiteItem, WebsiteItemViewModel>();
            return item;
        }

        public static WebsiteItem ToEntity(this WebsiteItemViewModel model)
        {
            return model.MapTo<WebsiteItemViewModel, WebsiteItem>();
        }

        public static List<WebsiteItemViewModel> ToListModel(this List<WebsiteItem> lEntity)
        {
            var lModel = new List<WebsiteItemViewModel>();
            if (lEntity != null && lEntity.Count > 0)
            {
                foreach (var entity in lEntity)
                {
                    var item = entity.MapTo<WebsiteItem, WebsiteItemViewModel>();
                    lModel.Add(item);
                }
            }
            return lModel;
        }

        public static List<WebsiteItem> ToListEntity(this List<WebsiteItemViewModel> lModel, int websiteID, int websiteScanID)
        {
            var lEntity = new List<WebsiteItem>();
            if (lModel != null && lModel.Count > 0)
            {
                foreach (var model in lModel)
                {
                    var eItem = model.MapTo<WebsiteItemViewModel, WebsiteItem>();
                    eItem.WebsiteID = websiteID;
                    eItem.LastScanID = websiteScanID;
                    eItem.Status = true;
                    lEntity.Add(eItem);
                }
            }
            return lEntity;
        }
        public static List<WebsiteItem> ToListEntity(this List<WebsiteItemViewModel> lModel)
        {
            var lEntity = new List<WebsiteItem>();
            if (lModel != null && lModel.Count > 0)
            {
                foreach (var model in lModel)
                {
                    var eItem = model.MapTo<WebsiteItemViewModel, WebsiteItem>();
                    lEntity.Add(eItem);
                }
            }
            return lEntity;
        }
        #endregion
        
    }
}