using TuesPechkin;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using TKM.Utils;
using System.Drawing.Printing;
using System.Web.UI;
using TKM.Services;
using TKM.BLL;
using PagedList;
using TKM.DAO.EntityFramework;
using Novacode;
using Newtonsoft.Json;

namespace TKM.WebApp.Controllers
{
    public class BaoCaoController : BaseController
    {
        JqueryUploadController _upload;
        private BaoCaoService _baoCaoService;
        #region Constructor
        public BaoCaoController()
        {
            if (_baoCaoService == null) _baoCaoService = new BaoCaoService();
        }
        #endregion

        #region BaoCaoTongHop
        public JsonResult GetDataChart(BaoCaoTongHopListView viewModel)
        {
            string vTotalLoi = "", vTongThietBi = "", vGroupName = "";
            if (viewModel != null)
            {
                /////Tong thiet bi
                var lstData = getDataTongThietBiTheoNhom(viewModel);
                if (lstData != null && lstData.Count > 0)
                {
                    vTotalLoi = lstData.Select(x => x.totalLoi).ToList().Aggregate("", (current, next) => current + "," + next).Trim(',');
                    vTongThietBi = lstData.Select(x => x.totalThietBi).ToList().Aggregate("", (current, next) => current + "," + next).Trim(',');
                    vGroupName = lstData.Select(x => x.name).ToList().Aggregate("", (current, next) => current + "," + next).Trim(',');
                }
                viewModel.TongThietBiLoi = vTotalLoi;
                viewModel.TongThietBi = vTongThietBi;
                viewModel.ColumnName = vGroupName;

                /////Tong Van De
                var lstChuaXuLy = new List<int?>();
                string vChuaXuLy = "", vDaXuLy = "";
                var lstData1 = getDataTongVanDeTheoNhom(viewModel);
                if (lstData1 != null && lstData1.Count > 0)
                {
                    vChuaXuLy = lstData1.Select(x => x.chuaxuly).ToList().Aggregate("", (current, next) => current + "," + next).Trim(',');
                    vDaXuLy = lstData1.Select(x => x.daxuly).ToList().Aggregate("", (current, next) => current + "," + next).Trim(',');
                    vGroupName = lstData1.Select(x => x.groupname).ToList().Aggregate("", (current, next) => current + "," + next).Trim(',');
                }
                viewModel.TongChuaXuLy = vChuaXuLy;
                viewModel.TongDaXuLy = vDaXuLy;
                viewModel.LstGroup = vGroupName;
            }
            var result = this.Json(JsonConvert.SerializeObject(viewModel), JsonRequestBehavior.AllowGet);
            return result;
        }
        public ActionResult BaoCaoTongHop()
        {
            var viewModel = new BaoCaoTongHopListView();
            viewModel.LstGroupData = _baoCaoService.getDDLNhomThietBi();
            return View(viewModel);
        }

        [HttpPost]
        public string ExportWordBaoCaoTongHop(BaoCaoTongHopListView viewModel)
        {
            var doc = initWordDocument();
            initWord(ref doc, getTitleReport(Utils.Enums.BaoCaoEnum.bcthall));
            if (!string.IsNullOrEmpty(viewModel.BaoCao) && !viewModel.BaoCao.Equals(","))
            {
                var lstBaoCao = viewModel.BaoCao.Split(',');
                if (lstBaoCao != null && lstBaoCao.Count() > 0)
                {
                    var index = 1;
                    foreach (var item in lstBaoCao.Where(x => !string.IsNullOrEmpty(x)))
                    {
                        Utils.Enums.BaoCaoEnum enumBaoCao = Utils.Enums.BaoCaoEnum.cpugroup;
                        var bullet = "";
                        if (item.Contains("hngroup") || item.Contains("hnhost"))
                        {
                            if (item.Contains("hngroup"))
                            {
                                viewModel.GroupBy = "group";
                            }
                            else
                            {
                                viewModel.GroupBy = "host";
                            }
                            viewModel.KeyType = item.Replace("hngroup", "").Replace("hnhost", "");
                        }
                        else
                        {
                            if (item.Contains("tkslnhomthietbi"))
                            {
                                enumBaoCao = Utils.Enums.BaoCaoEnum.tkslnhomthietbi;
                                if (!string.IsNullOrEmpty(viewModel.ChartTongThietBiNhom))
                                {
                                    _upload = new JqueryUploadController();
                                    string base64 = viewModel.ChartTongThietBiNhom.Substring(viewModel.ChartTongThietBiNhom.IndexOf(',') + 1);
                                    insertHeader(ref doc, index + "a. " + getHeaderTongThietBiNhom());
                                    insertWordChart(ref doc, base64);
                                    bullet = "b";
                                }
                            }
                            else if (item.Contains("tkcttheonhom"))
                            {
                                enumBaoCao = Utils.Enums.BaoCaoEnum.chitiettbtheonhom;
                            }
                            else if (item.Contains("thvdn"))
                            {
                                enumBaoCao = Utils.Enums.BaoCaoEnum.vandenhom;
                                if (!string.IsNullOrEmpty(viewModel.ChartTongSLVanDe))
                                {
                                    _upload = new JqueryUploadController();
                                    string base64 = viewModel.ChartTongSLVanDe.Substring(viewModel.ChartTongSLVanDe.IndexOf(',') + 1);
                                    insertHeader(ref doc, index + "a. " + getHeaderTongThietBiNhom());
                                    insertWordChart(ref doc, base64);
                                    bullet = "b";
                                }
                            }
                            else if (item.Contains("chitietvdn"))
                            {
                                enumBaoCao = Utils.Enums.BaoCaoEnum.chitietvande;
                            }
                        }
                        var lstCol = new List<string>();
                        var tblData = new DataTable();
                        var header = index + bullet + ". " + getHeaderBaoCaoTongHop(false, getTitleReport(enumBaoCao, viewModel.KeyType, viewModel.GroupBy));
                        getTableColumn(enumBaoCao, viewModel, ref lstCol, ref tblData);
                        insertHeader(ref doc, header);
                        insertWordDataTable(tblData, lstCol, ref doc);
                        index++;
                    }
                }
            }
            return saveToWordNew(doc, "BaoCaoTongHop");
        }

        [HttpPost]
        public string ExportPDFBaoCaoTongHop(BaoCaoTongHopListView viewModel)
        {
            var htmlData = new StringBuilder();
            if (!string.IsNullOrEmpty(viewModel.BaoCao) && !viewModel.BaoCao.Equals(","))
            {
                var lstBaoCao = viewModel.BaoCao.Split(',');
                if (lstBaoCao != null && lstBaoCao.Count() > 0)
                {
                    var index = 1;
                    foreach (var item in lstBaoCao.Where(x => !string.IsNullOrEmpty(x)))
                    {
                        htmlData.Append(Utils.ExportPDF.getPageBreak());
                        Utils.Enums.BaoCaoEnum enumBaoCao = Utils.Enums.BaoCaoEnum.cpugroup;
                        var bullet = "";
                        if (item.Contains("hngroup") || item.Contains("hnhost"))
                        {
                            if (item.Contains("hngroup"))
                            {
                                viewModel.GroupBy = "group";
                            }
                            else
                            {
                                viewModel.GroupBy = "host";
                            }
                            viewModel.KeyType = item.Replace("hngroup", "").Replace("hnhost", "");
                        }
                        else
                        {
                            if (item.Contains("tkslnhomthietbi"))
                            {
                                enumBaoCao = Utils.Enums.BaoCaoEnum.tkslnhomthietbi;
                                if (!string.IsNullOrEmpty(viewModel.ChartTongThietBiNhom))
                                {
                                    htmlData.Append(getChartHTML(index + bullet + "a. " + getHeaderTongThietBiNhom(), viewModel.ChartTongThietBiNhom));
                                    bullet = "b";
                                }
                            }
                            else if (item.Contains("tkcttheonhom"))
                            {
                                enumBaoCao = Utils.Enums.BaoCaoEnum.chitiettbtheonhom;
                            }
                            else if (item.Contains("thvdn"))
                            {
                                enumBaoCao = Utils.Enums.BaoCaoEnum.vandenhom;
                                if (!string.IsNullOrEmpty(viewModel.ChartTongSLVanDe))
                                {
                                    htmlData.Append(getChartHTML(index + bullet + "a. " + getHeaderTongVDNhom(), viewModel.ChartTongSLVanDe));
                                    bullet = "b";
                                }
                            }
                            else if (item.Contains("chitietvdn"))
                            {
                                enumBaoCao = Utils.Enums.BaoCaoEnum.chitietvande;
                            }
                        }
                        var lstCol = new List<string>();
                        var tblData = new DataTable();
                        var header = getHeaderBaoCaoTongHop(false, getTitleReport(enumBaoCao, viewModel.KeyType, viewModel.GroupBy));
                        getTableColumn(enumBaoCao, viewModel, ref lstCol, ref tblData);
                        htmlData.Append("<h2><b>" + index + bullet + ". " + header + "</b></h2>");
                        htmlData.Append(ExportPDF.pdfBang(lstCol, tblData));

                        index++;
                    }
                }
            }
            var html = genHtmlForm(getTitleReport(Utils.Enums.BaoCaoEnum.bcthall), htmlData);
            return saveToPDF(html, "BaoCaoTongHop");
        }

        private string getHeaderBaoCaoTongHop(bool isChart = true, string header = "")
        {
            if (isChart)
            {
                return "Biểu đồ " + header;
            }
            else
            {
                return "Chi tiết " + header;
            }
        }
        #endregion

        #region DeXuatThietBi

        public ActionResult DeXuatThietBi()
        {
            var viewModel = new BaoCaoTongHopListView();
            viewModel.LstGroupData = _baoCaoService.getDDLNhomThietBi();
            return View(viewModel);
        }

        [HttpPost]
        public string ExportWordDeXuatThietBi(BaoCaoTongHopListView viewModel)
        {
            var doc = initWordDocument();
            initWord(ref doc, "Đề xuất nâng cấp thiết bị");
            if (!string.IsNullOrEmpty(viewModel.BaoCao) && !viewModel.BaoCao.Equals(","))
            {
                var lstBaoCao = viewModel.BaoCao.Split(',');
                if (lstBaoCao != null && lstBaoCao.Count() > 0)
                {
                    var index = 1;
                    foreach (var item in lstBaoCao.Where(x => !string.IsNullOrEmpty(x)))
                    {
                        Utils.Enums.BaoCaoEnum enumBaoCao = Utils.Enums.BaoCaoEnum.dexuat;
                        var bullet = "";
                        viewModel.GroupBy = "host";
                        viewModel.KeyType = item.Replace("hngroup", "").Replace("hnhost", "");

                        var lstCol = new List<string>();
                        var tblData = new DataTable();
                        var lstDeXuat = new List<DeXuatNhomThietBi_Result>();
                        var header = index + bullet + ". " + getHeaderDeXuatThietBi(false, getTitleReport(enumBaoCao, viewModel.KeyType, viewModel.GroupBy));
                        getTableColumn(enumBaoCao, viewModel, ref lstCol, ref tblData, ref lstDeXuat);
                        if (lstDeXuat != null && lstDeXuat.Count > 0)
                        {
                            insertHeader(ref doc, header);
                            var lstGroup = lstDeXuat.GroupBy(x => new { x.groupname, x.name, x.ip, x.itemname }).ToList();
                            for (var j = 1; j <= lstGroup.Count; j++)
                            {
                                insertHeader(ref doc, "      + Thiết bị " + j + ": " + lstGroup[j - 1].First().name, 14D, true);
                                insertHeader(ref doc, "           Thuộc nhóm: " + lstGroup[j - 1].First().groupname, 14D, false);
                                insertHeader(ref doc, "           IP: " + lstGroup[j - 1].First().ip, 14D, false);
                            }
                            insertHeader(ref doc, "Thông tin chi tiết");
                            insertWordDataTable(tblData, lstCol, ref doc);
                            index++;
                        }
                    }
                }
            }
            return saveToWordNew(doc, "DeXuatThietBi");
        }

        [HttpPost]
        public string ExportPDFDeXuatThietBi(BaoCaoTongHopListView viewModel)
        {
            var htmlData = new StringBuilder();
            if (!string.IsNullOrEmpty(viewModel.BaoCao) && !viewModel.BaoCao.Equals(","))
            {
                var lstBaoCao = viewModel.BaoCao.Split(',');
                if (lstBaoCao != null && lstBaoCao.Count() > 0)
                {
                    var index = 1;
                    foreach (var item in lstBaoCao.Where(x => !string.IsNullOrEmpty(x)))
                    {
                        htmlData.Append(Utils.ExportPDF.getPageBreak());
                        //Utils.Enums.BaoCaoEnum enumBaoCao = Utils.Enums.BaoCaoEnum.cpugroup;
                        //var bullet = "";
                        //if (item.Contains("hngroup") || item.Contains("hnhost"))
                        //{
                        //    if (item.Contains("hngroup"))
                        //    {
                        //        viewModel.GroupBy = "group";
                        //    }
                        //    else
                        //    {
                        //        viewModel.GroupBy = "host";
                        //    }
                        //    viewModel.KeyType = item.Replace("hngroup", "").Replace("hnhost", "");
                        //}
                        //else
                        //{
                        //    if (item.Contains("tkslnhomthietbi"))
                        //    {
                        //        enumBaoCao = Utils.Enums.BaoCaoEnum.tkslnhomthietbi;
                        //        if (!string.IsNullOrEmpty(viewModel.ChartTongThietBiNhom))
                        //        {
                        //            htmlData.Append(getChartHTML(index + bullet + "a. " + getHeaderTongThietBiNhom(), viewModel.ChartTongThietBiNhom));
                        //            bullet = "b";
                        //        }
                        //    }
                        //    else if (item.Contains("tkcttheonhom"))
                        //    {
                        //        enumBaoCao = Utils.Enums.BaoCaoEnum.chitiettbtheonhom;
                        //    }
                        //    else if (item.Contains("thvdn"))
                        //    {
                        //        enumBaoCao = Utils.Enums.BaoCaoEnum.vandenhom;
                        //        if (!string.IsNullOrEmpty(viewModel.ChartTongSLVanDe))
                        //        {
                        //            htmlData.Append(getChartHTML(index + bullet + "a. " + getHeaderTongVDNhom(), viewModel.ChartTongSLVanDe));
                        //            bullet = "b";
                        //        }
                        //    }
                        //    else if (item.Contains("chitietvdn"))
                        //    {
                        //        enumBaoCao = Utils.Enums.BaoCaoEnum.chitietvande;
                        //    }
                        //}

                        //var lstCol = new List<string>();
                        //var tblData = new DataTable();
                        //var header = getHeaderDeXuatThietBi(false, getTitleReport(enumBaoCao, viewModel.KeyType, viewModel.GroupBy));
                        //getTableColumn(enumBaoCao, viewModel, ref lstCol, ref tblData);
                        //htmlData.Append("<h2><b>" + index + bullet + ". " + header + "</b></h2>");
                        //htmlData.Append(ExportPDF.pdfBang(lstCol, tblData));

                        //index++;

                        Utils.Enums.BaoCaoEnum enumBaoCao = Utils.Enums.BaoCaoEnum.dexuat;
                        var bullet = "";
                        viewModel.GroupBy = "host";
                        viewModel.KeyType = item.Replace("hngroup", "").Replace("hnhost", "");

                        var lstCol = new List<string>();
                        var tblData = new DataTable();
                        var lstDeXuat = new List<DeXuatNhomThietBi_Result>();
                        var header = getHeaderDeXuatThietBi(false, getTitleReport(enumBaoCao, viewModel.KeyType, viewModel.GroupBy));
                        getTableColumn(enumBaoCao, viewModel, ref lstCol, ref tblData, ref lstDeXuat);
                        if (lstDeXuat != null && lstDeXuat.Count > 0)
                        {
                            htmlData.Append("<h2><b>" + index + bullet + ". " + header + "</b></h2>");
                            var lstGroup = lstDeXuat.GroupBy(x => new { x.groupname, x.name, x.ip, x.itemname }).ToList();
                            for (var j = 1; j <= lstGroup.Count; j++)
                            {
                                htmlData.Append("<h3><b>" + "      + Thiết bị " + j + ": " + lstGroup[j - 1].First().name + "</b></h3>");
                                htmlData.Append("<h3 style='font-weight:normal;'>" + "           Thuộc nhóm: " + lstGroup[j - 1].First().groupname + "</h3>");
                                htmlData.Append("<h3 style='font-weight:normal;'>" + "           IP: " + lstGroup[j - 1].First().ip + "</h3>");
                            }
                            htmlData.Append("<h2><b>Thông tin chi tiết</b></h2>");
                            htmlData.Append(ExportPDF.pdfBang(lstCol, tblData));
                            index++;
                        }
                    }
                }
            }
            var html = genHtmlForm(getTitleReport(Utils.Enums.BaoCaoEnum.bcthall), htmlData);
            return saveToPDF(html, "DeXuatThietBi");
        }

        private string getHeaderDeXuatThietBi(bool isChart = true, string header = "")
        {
            if (isChart)
            {
                return "Biểu đồ " + header;
            }
            else
            {
                return "Chi tiết " + header;
            }
        }


        private List<DeXuatNhomThietBi_Result> getDataDeXuatThietBi(BaoCaoTongHopListView viewModel)
        {
            if (viewModel.PageNumber == null) viewModel.PageNumber = 1;
            if (viewModel.PageSize == null) viewModel.PageSize = 20;
            DateTime? tuNgay = null, denNgay = null;
            if (viewModel.TuNgay != null)
            {
                tuNgay = ConvertDateTime.ConvertToDate(viewModel.TuNgay);
            }
            if (viewModel.DenNgay != null)
            {
                denNgay = ConvertDateTime.ConvertToDate(viewModel.DenNgay);
            }
            viewModel.GetDetail = 1;
            var lstData = _baoCaoService.deXuatThietBi(viewModel.LstGroup == null ? "" : viewModel.LstGroup, viewModel.TenThietBi == null ? "" : viewModel.TenThietBi, viewModel.GroupBy, viewModel.GroupTime, viewModel.KeyType, viewModel.GetDetail, tuNgay, denNgay, 0, 0, viewModel.ColumnName == null ? "" : viewModel.ColumnName, viewModel.OrderBy == null ? "" : viewModel.OrderBy);
            return lstData;
        }
        #endregion

        #region TongVanDeTheoNhom
        public ActionResult Index(int? trangThai, int? pnum, int? psize)
        {
            var viewModel = new BaoCaoTongHopListView()
            {
                PageNumber = pnum ?? 1,
                PageSize = psize ?? 10,
                HasChart = true,
                HasTableData = true
            };
            viewModel.LstGroupData = _baoCaoService.getDDLNhomThietBi();

            return View(viewModel);
        }

        public ActionResult IndexDetail(BaoCaoTongHopListView viewModel)
        {
            if (viewModel != null)
            {
                var lstChuaXuLy = new List<int?>();
                string vChuaXuLy = "", vDaXuLy = "", vGroupName = "";
                var lstData = getDataTongVanDeTheoNhom(viewModel);
                if (lstData != null && lstData.Count > 0)
                {
                    if (viewModel.PageSize != null && viewModel.PageNumber != null && viewModel.PageSize > 0 && viewModel.PageNumber > 0)
                    {
                        viewModel.LstResultDataCountProblemByStatus = lstData.Skip((viewModel.PageNumber.Value - 1) * viewModel.PageSize.Value).Take(viewModel.PageSize.Value).ToList();
                    }
                    vChuaXuLy = lstData.Select(x => x.chuaxuly).ToList().Aggregate("", (current, next) => current + "," + next).Trim(',');
                    vDaXuLy = lstData.Select(x => x.daxuly).ToList().Aggregate("", (current, next) => current + "," + next).Trim(',');
                    vGroupName = lstData.Select(x => x.groupname).ToList().Aggregate("", (current, next) => current + "," + next).Trim(',');
                }
                viewModel.TongChuaXuLy = vChuaXuLy;
                viewModel.TongDaXuLy = vDaXuLy;
                viewModel.LstGroup = vGroupName;
                var total = lstData.Count;
                viewModel.TotalItem = total;
                if (viewModel.PageNumber > 0 && viewModel.PageSize > 0)
                {
                    int[] totals = new int[total];
                    ViewBag.PagedList = totals.ToPagedList((int)viewModel.PageNumber, (int)viewModel.PageSize);
                }
            }
            return PartialView(viewModel);
        }

        private List<CountProblemByStatus_Result> getDataTongVanDeTheoNhom(BaoCaoTongHopListView viewModel)
        {
            if (viewModel.PageNumber == null) viewModel.PageNumber = 1;
            if (viewModel.PageSize == null) viewModel.PageSize = 20;
            DateTime? tuNgay = null, denNgay = null;
            if (viewModel.TuNgay != null)
            {
                tuNgay = ConvertDateTime.ConvertToDate(viewModel.TuNgay);
            }
            if (viewModel.DenNgay != null)
            {
                denNgay = ConvertDateTime.ConvertToDate(viewModel.DenNgay);
            }
            viewModel.GroupBy = "group";
            var lstData = _baoCaoService.countProblemByGroup(viewModel.LstGroup == null ? "" : viewModel.LstGroup, viewModel.srTrangThai.HasValue ? viewModel.srTrangThai.Value.ToString() : "", viewModel.GroupBy, tuNgay, denNgay, 0, 0, viewModel.ColumnName, viewModel.OrderBy);
            return lstData;
        }

        //private StringBuilder genTongVDNhomHTML(BaoCaoTongHopListView viewModel, int widthChart = 0, bool isPDF = false)
        //{
        //    var htmlData = new StringBuilder();
        //    if (viewModel.HasChart.HasValue && viewModel.HasChart.Value && !string.IsNullOrEmpty(viewModel.ChartImage))
        //    {
        //        htmlData.Append(getChartHTML("Chi tiết", viewModel.ChartImage, widthChart));
        //    }
        //    if (viewModel.HasTableData.HasValue && viewModel.HasTableData.Value)
        //    {
        //        var lstData = getDataTongVanDeTheoNhom(viewModel);
        //        if (lstData != null)
        //        {
        //            var lstCol = new List<string>() { "Nhóm thiết bị", "Số lượng vấn đề chưa xử lý", "Số lượng vấn đề đã xử lý", "Tổng số lượng vấn đề" };
        //            var tblData = ObjectUtils.ConvertToDatatable<CountProblemByStatus_Result>(lstData);
        //            htmlData.Append(getTableHTML("Chi tiết", lstCol, tblData));
        //        }
        //    }
        //    return htmlData;
        //}

        [HttpPost]
        public string ExportWordTongVDNhom(BaoCaoTongHopListView viewModel)
        {
            var doc = initWordDocument();
            initWord(ref doc, getTitleReport(Utils.Enums.BaoCaoEnum.vandenhom));
            if (viewModel.HasChart.HasValue && viewModel.HasChart.Value && !string.IsNullOrEmpty(viewModel.ChartImage))
            {
                _upload = new JqueryUploadController();
                string base64 = viewModel.ChartImage.Substring(viewModel.ChartImage.IndexOf(',') + 1);
                insertHeader(ref doc, getHeaderTongThietBiNhom());
                insertWordChart(ref doc, base64);
            }
            if (viewModel.HasTableData.HasValue && viewModel.HasTableData.Value)
            {
                var lstCol = new List<string>();
                var tblData = new DataTable();
                var header = getHeaderTongVDNhom(false);
                getTableColumn(Utils.Enums.BaoCaoEnum.vandenhom, viewModel, ref lstCol, ref tblData);
                insertHeader(ref doc, header);
                insertWordDataTable(tblData, lstCol, ref doc);
            }
            return saveToWordNew(doc, "TongVanDeNhom");
        }

        [HttpPost]
        public string ExportPDFTongVDNhom(BaoCaoTongHopListView viewModel)
        {
            var htmlData = new StringBuilder();
            if (viewModel.HasChart.HasValue && viewModel.HasChart.Value && !string.IsNullOrEmpty(viewModel.ChartImage))
            {
                htmlData.Append(getChartHTML(getHeaderTongVDNhom(), viewModel.ChartImage));
            }
            if (viewModel.HasTableData.HasValue && viewModel.HasTableData.Value)
            {
                var lstCol = new List<string>();
                var tblData = new DataTable();
                var header = getHeaderTongVDNhom(false);
                getTableColumn(Utils.Enums.BaoCaoEnum.vandenhom, viewModel, ref lstCol, ref tblData);
                htmlData.Append("<h2><b>" + header + "</b></h2>");
                htmlData.Append(ExportPDF.pdfBang(lstCol, tblData));
            }
            var html = genHtmlForm(getTitleReport(Utils.Enums.BaoCaoEnum.tkslnhomthietbi), htmlData);
            return saveToPDF(html, "TongVanDeNhom");
        }
        private string getHeaderTongVDNhom(bool isChart = true)
        {
            if (isChart)
            {
                return "Biểu đồ Tổng số lượng vấn đề của nhóm thiết bị";
            }
            else
            {
                return "Chi tiết Tổng số lượng vấn đề của nhóm thiết bị";
            }
        }
        #endregion

        #region TongThietBiTheoNhom

        public ActionResult TongThietBiNhom(int? trangThai, int? pnum, int? psize)
        {
            var viewModel = new BaoCaoTongHopListView()
            {
                PageNumber = pnum ?? 1,
                PageSize = psize ?? 10,
                HasChart = true,
                HasTableData = true
            };
            viewModel.LstGroupData = _baoCaoService.getDDLNhomThietBi();

            return View(viewModel);
        }
        public ActionResult TongThietBiNhomDetail(BaoCaoTongHopListView viewModel)
        {
            if (viewModel != null)
            {
                string vTotalLoi = "", vTongThietBi = "", vGroupName = "";
                var lstData = getDataTongThietBiTheoNhom(viewModel);
                if (lstData != null && lstData.Count > 0)
                {
                    if (viewModel.PageSize != null && viewModel.PageNumber != null && viewModel.PageSize > 0 && viewModel.PageNumber > 0)
                    {
                        viewModel.LstResultDataCountHostsByGroup = lstData.Skip((viewModel.PageNumber.Value - 1) * viewModel.PageSize.Value).Take(viewModel.PageSize.Value).ToList();
                    }
                    vTotalLoi = lstData.Select(x => x.totalLoi).ToList().Aggregate("", (current, next) => current + "," + next).Trim(',');
                    vTongThietBi = lstData.Select(x => x.totalThietBi).ToList().Aggregate("", (current, next) => current + "," + next).Trim(',');
                    vGroupName = lstData.Select(x => x.name).ToList().Aggregate("", (current, next) => current + "," + next).Trim(',');
                }
                viewModel.TongThietBiLoi = vTotalLoi;
                viewModel.TongThietBi = vTongThietBi;
                viewModel.LstGroup = vGroupName;
                var total = lstData.Count;
                viewModel.TotalItem = total;
                if (viewModel.PageNumber > 0 && viewModel.PageSize > 0)
                {
                    int[] totals = new int[total];
                    ViewBag.PagedList = totals.ToPagedList((int)viewModel.PageNumber, (int)viewModel.PageSize);
                }
            }
            return PartialView(viewModel);
        }

        private List<CountHostsByGroup_Result> getDataTongThietBiTheoNhom(BaoCaoTongHopListView viewModel)
        {
            if (viewModel.PageNumber == null) viewModel.PageNumber = 1;
            if (viewModel.PageSize == null) viewModel.PageSize = 20;
            DateTime? tuNgay = null, denNgay = null;
            if (viewModel.TuNgay != null)
            {
                tuNgay = ConvertDateTime.ConvertToDate(viewModel.TuNgay);
            }
            if (viewModel.DenNgay != null)
            {
                denNgay = ConvertDateTime.ConvertToDate(viewModel.DenNgay);
            }
            var lstData = _baoCaoService.countHostsByGroup(viewModel.LstGroup == null ? "" : viewModel.LstGroup, viewModel.srTrangThai.HasValue ? viewModel.srTrangThai.Value.ToString() : "", tuNgay, denNgay, 0, 0, viewModel.ColumnName, viewModel.OrderBy);
            return lstData;
        }

        //[HttpPost]
        //public void ExportWordTongThietBiNhom(TongThietBiNhomListView viewModel)
        //{
        //    var html = genTongThietBiNhomHTML(viewModel, "Báo cáo Tổng số lượng thiết bị của nhóm thiết bị", 700);
        //    saveToWord(html, "TongThietBiNhom");
        //}


        [HttpPost]
        public string ExportWordTongThietBiNhom(BaoCaoTongHopListView viewModel)
        {
            var path = CommonUtils.GetPath("~/Assets/Template/ReportTemplate.docx");
            DocX doc = DocX.Load(path);
            initWord(ref doc, getTitleReport(Utils.Enums.BaoCaoEnum.tkslnhomthietbi));
            if (viewModel.HasChart.HasValue && viewModel.HasChart.Value && !string.IsNullOrEmpty(viewModel.ChartImage))
            {
                _upload = new JqueryUploadController();
                string base64 = viewModel.ChartImage.Substring(viewModel.ChartImage.IndexOf(',') + 1);
                insertHeader(ref doc, getHeaderTongThietBiNhom());
                insertWordChart(ref doc, base64);
            }
            if (viewModel.HasTableData.HasValue && viewModel.HasTableData.Value)
            {
                var lstCol = new List<string>();
                var tblData = new DataTable();
                var header = getHeaderTongThietBiNhom(false);
                getTableColumn(Utils.Enums.BaoCaoEnum.tkslnhomthietbi, viewModel, ref lstCol, ref tblData);
                insertHeader(ref doc, header);
                insertWordDataTable(tblData, lstCol, ref doc);
            }
            return saveToWordNew(doc, "TongThietBiNhom");
        }

        [HttpPost]
        public string ExportPDFTongThietBiNhom(BaoCaoTongHopListView viewModel)
        {
            var htmlData = new StringBuilder();
            if (viewModel.HasChart.HasValue && viewModel.HasChart.Value && !string.IsNullOrEmpty(viewModel.ChartImage))
            {
                htmlData.Append(getChartHTML(getHeaderTongThietBiNhom(), viewModel.ChartImage));
            }
            if (viewModel.HasTableData.HasValue && viewModel.HasTableData.Value)
            {
                var lstCol = new List<string>();
                var tblData = new DataTable();
                var header = getHeaderTongThietBiNhom(false);
                getTableColumn(Utils.Enums.BaoCaoEnum.tkslnhomthietbi, viewModel, ref lstCol, ref tblData);
                htmlData.Append("<h2><b>" + header + "</b></h2>");
                htmlData.Append(ExportPDF.pdfBang(lstCol, tblData));
            }
            var html = genHtmlForm(getTitleReport(Utils.Enums.BaoCaoEnum.tkslnhomthietbi), htmlData);
            return saveToPDF(html, "TongThietBiNhom");
        }

        private string getHeaderTongThietBiNhom(bool isChart = true)
        {
            if (isChart)
            {
                return "Biểu đồ Tổng số lượng thiết bị";
            }
            else
            {
                return "Chi tiết Tổng số lượng thiết bị";
            }
        }

        #endregion

        #region ChiTietThietBiNhom

        public ActionResult ChiTietThietBiNhom(int? trangThai, int? pnum, int? psize)
        {
            var viewModel = new BaoCaoTongHopListView()
            {
                PageNumber = pnum ?? 1,
                PageSize = psize ?? 10,
                HasChart = true,
                HasTableData = true
            };
            viewModel.LstGroupData = _baoCaoService.getDDLNhomThietBi();

            return View(viewModel);
        }

        public ActionResult ChiTietThietBiNhomDetail(BaoCaoTongHopListView viewModel)
        {
            if (viewModel != null)
            {
                string vTotalLoi = "", vTongThietBi = "", vGroupName = "";
                var lstData = getDataChiTietThietBiTheoNhom(viewModel);
                if (lstData != null && lstData.Count > 0)
                {
                    if (viewModel.PageSize != null && viewModel.PageNumber != null && viewModel.PageSize > 0 && viewModel.PageNumber > 0)
                    {
                        viewModel.LstResultDataHostsByGroup = lstData.Skip((viewModel.PageNumber.Value - 1) * viewModel.PageSize.Value).Take(viewModel.PageSize.Value).ToList();
                    }
                }
                var total = lstData.Count;
                viewModel.TotalItem = total;
                if (viewModel.PageNumber > 0 && viewModel.PageSize > 0)
                {
                    int[] totals = new int[total];
                    ViewBag.PagedList = totals.ToPagedList((int)viewModel.PageNumber, (int)viewModel.PageSize);
                }
            }
            return PartialView(viewModel);
        }

        private List<HostsByGroup_Result> getDataChiTietThietBiTheoNhom(BaoCaoTongHopListView viewModel)
        {
            if (viewModel.PageNumber == null) viewModel.PageNumber = 1;
            if (viewModel.PageSize == null) viewModel.PageSize = 20;
            var lstData = _baoCaoService.chiTietThietBiTheoNhom(viewModel.LstGroup == null ? "" : viewModel.LstGroup, viewModel.TenThietBi, 0, 0, viewModel.ColumnName, viewModel.OrderBy);
            return lstData;
        }

        //private StringBuilder genChiTietThietBiTheoNhomHTML(BaoCaoTongHopListView viewModel, string tenBaoCao, int widthChart = 0, bool isPDF = false)
        //{
        //    var htmlChart = new StringBuilder();
        //    var htmlChiTiet = new StringBuilder();
        //    var lstData = getDataChiTietThietBiTheoNhom(viewModel);
        //    if (lstData != null)
        //    {
        //        htmlChiTiet.Append("<h2><b>Chi tiết</b></h2>");
        //        var lstCol = new List<string>() { "Nhóm thiết bị", "Tên thiết bị", "IP", "Số lượng thành phần quản lý", "Số lượng item thiết bị", "Số lượng triggers giám sát" };
        //        var tblData = ObjectUtils.ConvertToDatatable<HostsByGroup_Result>(lstData);
        //        htmlChiTiet.Append(ExportPDF.pdfBang(lstCol, tblData));
        //    }
        //    return genHtmlForm(tenBaoCao, htmlChart, htmlChiTiet, isPDF);
        //}

        [HttpPost]
        public string ExportWordChiTietThietBiNhom(BaoCaoTongHopListView viewModel)
        {
            var doc = initWordDocument();
            initWord(ref doc, getTitleReport(Utils.Enums.BaoCaoEnum.chitiettbtheonhom));
            //if (viewModel.HasChart.HasValue && viewModel.HasChart.Value && !string.IsNullOrEmpty(viewModel.ChartImage))
            //{
            //    _upload = new JqueryUploadController();
            //    string base64 = viewModel.ChartImage.Substring(viewModel.ChartImage.IndexOf(',') + 1);
            //    insertHeader(ref doc, getHeaderTongThietBiNhom());
            //    insertWordChart(ref doc, base64);
            //}
            //if (viewModel.HasTableData.HasValue && viewModel.HasTableData.Value)
            //{
            var lstCol = new List<string>();
            var tblData = new DataTable();
            var header = getHeaderChiTietThietBiNhom(false);
            getTableColumn(Utils.Enums.BaoCaoEnum.chitiettbtheonhom, viewModel, ref lstCol, ref tblData);
            insertHeader(ref doc, header);
            insertWordDataTable(tblData, lstCol, ref doc);
            //}
            return saveToWordNew(doc, "ChiTietThietBiNhom");
        }

        [HttpPost]
        public string ExportPDFChiTietThietBiNhom(BaoCaoTongHopListView viewModel)
        {
            var htmlData = new StringBuilder();
            //if (viewModel.HasChart.HasValue && viewModel.HasChart.Value && !string.IsNullOrEmpty(viewModel.ChartImage))
            //{
            //    htmlData.Append(getChartHTML(getHeaderTongThietBiNhom(), viewModel.ChartImage));
            //}
            //if (viewModel.HasTableData.HasValue && viewModel.HasTableData.Value)
            //{
            var lstCol = new List<string>();
            var tblData = new DataTable();
            var header = getHeaderChiTietThietBiNhom(false);
            getTableColumn(Utils.Enums.BaoCaoEnum.chitiettbtheonhom, viewModel, ref lstCol, ref tblData);
            htmlData.Append("<h2><b>" + header + "</b></h2>");
            htmlData.Append(ExportPDF.pdfBang(lstCol, tblData));
            //}
            var html = genHtmlForm(getTitleReport(Utils.Enums.BaoCaoEnum.chitiettbtheonhom), htmlData);
            return saveToPDF(html, "ChiTietThietBiNhom");
        }
        private string getHeaderChiTietThietBiNhom(bool isChart = false)
        {
            if (isChart)
            {
                return "Biểu đồ Thiết bị theo nhóm";
            }
            else
            {
                return "Chi tiết Thiết bị theo nhóm";
            }
        }
        #endregion

        #region ChiTietVanDe

        public ActionResult ChiTietVanDe(int? trangThai, int? pnum, int? psize)
        {
            var viewModel = new BaoCaoTongHopListView()
            {
                PageNumber = pnum ?? 1,
                PageSize = psize ?? 10,
                HasChart = true,
                HasTableData = true
            };
            viewModel.LstGroupData = _baoCaoService.getDDLNhomThietBi();

            return View(viewModel);
        }

        public ActionResult ChiTietVanDeDetail(BaoCaoTongHopListView viewModel)
        {
            if (viewModel != null)
            {
                var lstData = getDataChiTietVanDe(viewModel);
                if (lstData != null && lstData.Count > 0)
                {
                    if (viewModel.PageSize != null && viewModel.PageNumber != null && viewModel.PageSize > 0 && viewModel.PageNumber > 0)
                    {
                        viewModel.LstResultDataProblemDetail = lstData.Skip((viewModel.PageNumber.Value - 1) * viewModel.PageSize.Value).Take(viewModel.PageSize.Value).ToList();
                    }
                }
                var total = lstData.Count;
                viewModel.TotalItem = total;
                if (viewModel.PageNumber > 0 && viewModel.PageSize > 0)
                {
                    int[] totals = new int[total];
                    ViewBag.PagedList = totals.ToPagedList((int)viewModel.PageNumber, (int)viewModel.PageSize);
                }
            }
            return PartialView(viewModel);
        }

        private List<GetProblemDetail_Result> getDataChiTietVanDe(BaoCaoTongHopListView viewModel)
        {
            if (viewModel.PageNumber == null) viewModel.PageNumber = 1;
            if (viewModel.PageSize == null) viewModel.PageSize = 20;
            DateTime? tuNgay = null, denNgay = null;
            if (viewModel.TuNgay != null)
            {
                tuNgay = ConvertDateTime.ConvertToDate(viewModel.TuNgay);
            }
            if (viewModel.DenNgay != null)
            {
                denNgay = ConvertDateTime.ConvertToDate(viewModel.DenNgay);
            }
            var dataChart = "";
            var lstData = _baoCaoService.getProblem(viewModel.LstGroup == null ? "" : viewModel.LstGroup, "", "host", viewModel.CapDo, tuNgay, denNgay, 0, 0, viewModel.ColumnName, viewModel.OrderBy, ref dataChart, false);
            return lstData;
        }

        //private StringBuilder genChiTietVanDeHTML(BaoCaoTongHopListView viewModel, string tenBaoCao, int widthChart = 0, bool isPDF = false)
        //{
        //    var htmlChart = new StringBuilder();
        //    var htmlChiTiet = new StringBuilder();
        //    var lstData = getDataChiTietVanDe(viewModel);
        //    if (lstData != null)
        //    {
        //        htmlChiTiet.Append("<h2><b>Chi tiết</b></h2>");
        //        var lstCol = new List<string>() { "Nhóm thiết bị", "Tên thiết bị", "IP", "Vấn đề", "Thời gian xuất hiện", "Thời gian tồn tại", "Mức độ nghiêm trọng", "Trạng thái" };
        //        var tblData = ObjectUtils.ConvertToDatatable<GetProblemDetail_Result>(lstData);
        //        tblData.Columns.RemoveAt(0);
        //        tblData.Columns.RemoveAt(0);
        //        tblData.Columns.RemoveAt(0);
        //        tblData.Columns.RemoveAt(0);
        //        htmlChiTiet.Append(ExportPDF.pdfBang(lstCol, tblData));
        //    }
        //    return genHtmlForm(tenBaoCao, htmlChart, htmlChiTiet, isPDF);
        //}

        [HttpPost]
        public string ExportWordChiTietVanDe(BaoCaoTongHopListView viewModel)
        {
            ////var html = genChiTietVanDeHTML(viewModel, "Thống kê danh sách vấn đề của thiết bị", 700);
            ////saveToWord(html, "ChiTietVanDe");
            //var path = CommonUtils.GetPath("~/Assets/Template/ReportTemplate.docx");
            //DocX doc = DocX.Load(path);
            //initWord(ref doc, "Thống kê danh sách vấn đề của thiết bị");
            ////if (viewModel.HasChart.HasValue && viewModel.HasChart.Value && !string.IsNullOrEmpty(viewModel.ChartImage))
            ////{
            ////    _upload = new JqueryUploadController();
            ////    string base64 = viewModel.ChartImage.Substring(viewModel.ChartImage.IndexOf(',') + 1);
            ////    insertWordChart(ref doc, base64);
            ////}
            ////if (viewModel.HasTableData.HasValue && viewModel.HasTableData.Value)
            ////{
            //var lstData = getDataChiTietVanDe(viewModel);
            ////var lstData = getDataHieuNangNhomTheoNhom(viewModel);
            //if (lstData != null && lstData.Count > 0)
            //{
            //    var lstCol = new List<string>() { "Nhóm thiết bị", "Tên thiết bị", "IP", "Vấn đề", "Thời gian xuất hiện", "Thời gian tồn tại", "Mức độ nghiêm trọng", "Trạng thái" };
            //    var tblData = ObjectUtils.ConvertToDatatable<GetProblemDetail_Result>(lstData);
            //    tblData.Columns.RemoveAt(0);
            //    tblData.Columns.RemoveAt(0);
            //    tblData.Columns.RemoveAt(0);
            //    tblData.Columns.RemoveAt(0);
            //    insertHeader(ref doc, "Thống kê danh sách vấn đề của thiết bị");
            //    insertWordDataTable(tblData, lstCol, ref doc);
            //}
            ////}
            var doc = initWordDocument();
            initWord(ref doc, getTitleReport(Utils.Enums.BaoCaoEnum.chitietvande));
            //if (viewModel.HasChart.HasValue && viewModel.HasChart.Value && !string.IsNullOrEmpty(viewModel.ChartImage))
            //{
            //    _upload = new JqueryUploadController();
            //    string base64 = viewModel.ChartImage.Substring(viewModel.ChartImage.IndexOf(',') + 1);
            //    insertHeader(ref doc, getHeaderTongThietBiNhom());
            //    insertWordChart(ref doc, base64);
            //}
            //if (viewModel.HasTableData.HasValue && viewModel.HasTableData.Value)
            //{
            var lstCol = new List<string>();
            var tblData = new DataTable();
            var header = getHeaderChiTietVanDe(false);
            getTableColumn(Utils.Enums.BaoCaoEnum.chitietvande, viewModel, ref lstCol, ref tblData);
            insertHeader(ref doc, header);
            insertWordDataTable(tblData, lstCol, ref doc);
            //}
            return saveToWordNew(doc, "ChiTietVanDe");
        }

        [HttpPost]
        public string ExportPDFChiTietVanDe(BaoCaoTongHopListView viewModel)
        {
            var htmlData = new StringBuilder();
            //if (viewModel.HasChart.HasValue && viewModel.HasChart.Value && !string.IsNullOrEmpty(viewModel.ChartImage))
            //{
            //    htmlData.Append(getChartHTML(getHeaderTongThietBiNhom(), viewModel.ChartImage));
            //}
            //if (viewModel.HasTableData.HasValue && viewModel.HasTableData.Value)
            //{
            var lstCol = new List<string>();
            var tblData = new DataTable();
            var header = getHeaderChiTietVanDe(false);
            getTableColumn(Utils.Enums.BaoCaoEnum.chitietvande, viewModel, ref lstCol, ref tblData);
            htmlData.Append("<h2><b>" + header + "</b></h2>");
            htmlData.Append(ExportPDF.pdfBang(lstCol, tblData));
            //}
            var html = genHtmlForm(getTitleReport(Utils.Enums.BaoCaoEnum.chitietvande), htmlData);
            return saveToPDF(html, "ChiTietVanDe");
        }
        private string getHeaderChiTietVanDe(bool isChart = true)
        {
            if (isChart)
            {
                return "Biểu đồ Danh sách vấn đề của thiết bị";
            }
            else
            {
                return "Chi tiết Danh sách vấn đề của thiết bị";
            }
        }
        #endregion

        #region HieuNangNhom

        public ActionResult HieuNangNhom(int? trangThai, int? pnum, int? psize, string keytype = "cpu", string grouptime = "month", string groupby = "group")
        {
            var viewModel = new BaoCaoTongHopListView()
            {
                PageNumber = pnum ?? 1,
                PageSize = psize ?? 10,
                HasChart = true,
                HasTableData = true,
                GroupBy = groupby,
                GroupTime = grouptime,
                KeyType = keytype
            };
            viewModel.LstGroupData = _baoCaoService.getDDLNhomThietBi();
            viewModel.LstFilterMonth = _baoCaoService.getDDLFilterTimeHieuNang();
            viewModel.LstFilterWeek = _baoCaoService.getDDLFilterTimeHieuNang("week");
            viewModel.LstFilterQuarter = _baoCaoService.getDDLFilterTimeHieuNang("quarter");
            viewModel.LstFilterYear = _baoCaoService.getDDLFilterTimeHieuNang("year");

            return View(viewModel);
        }

        public ActionResult HieuNangNhomDetail(BaoCaoTongHopListView viewModel)
        {
            if (viewModel != null)
            {
                var lstChuaXuLy = new List<int?>();
                string vChuaXuLy = "", vDaXuLy = "", vGroupName = "";
                var lstData = getDataHieuNangNhomTheoNhom(viewModel);
                if (lstData != null && lstData.Count > 0)
                {
                    if (viewModel.PageSize != null && viewModel.PageNumber != null && viewModel.PageSize > 0 && viewModel.PageNumber > 0)
                    {
                        viewModel.LstResultDataHieuNangNhomThietBi = lstData.Skip((viewModel.PageNumber.Value - 1) * viewModel.PageSize.Value).Take(viewModel.PageSize.Value).ToList();
                    }
                    //vChuaXuLy = lstData.Select(x => x.chuaxuly).ToList().Aggregate("", (current, next) => current + "," + next).Trim(',');
                    //vDaXuLy = lstData.Select(x => x.daxuly).ToList().Aggregate("", (current, next) => current + "," + next).Trim(',');
                    //vGroupName = lstData.Select(x => x.groupname).ToList().Aggregate("", (current, next) => current + "," + next).Trim(',');
                }
                var total = lstData.Count;
                viewModel.TotalItem = total;
                if (viewModel.PageNumber > 0 && viewModel.PageSize > 0)
                {
                    int[] totals = new int[total];
                    ViewBag.PagedList = totals.ToPagedList((int)viewModel.PageNumber, (int)viewModel.PageSize);
                }
            }
            return PartialView(viewModel);
        }

        private List<HieuNangNhomThietBi_Result> getDataHieuNangNhomTheoNhom(BaoCaoTongHopListView viewModel)
        {
            if (viewModel.PageNumber == null) viewModel.PageNumber = 1;
            if (viewModel.PageSize == null) viewModel.PageSize = 20;
            DateTime? tuNgay = null, denNgay = null;
            if (viewModel.TuNgay != null)
            {
                tuNgay = ConvertDateTime.ConvertToDate(viewModel.TuNgay);
            }
            if (viewModel.DenNgay != null)
            {
                denNgay = ConvertDateTime.ConvertToDate(viewModel.DenNgay);
            }
            viewModel.GetDetail = 1;
            var lstData = _baoCaoService.hieuNangThietBi(viewModel.LstGroup == null ? "" : viewModel.LstGroup, viewModel.TenThietBi == null ? "" : viewModel.TenThietBi, viewModel.GroupBy, viewModel.GroupTime, viewModel.KeyType, viewModel.GetDetail, tuNgay, denNgay, 0, 0, viewModel.ColumnName == null ? "" : viewModel.ColumnName, viewModel.OrderBy == null ? "" : viewModel.OrderBy);
            return lstData;
        }

        //private StringBuilder genHieuNangNhomHTML(BaoCaoTongHopListView viewModel, string tenBaoCao, int widthChart = 0, bool isPDF = false)
        //{
        //    var htmlChart = new StringBuilder();
        //    var htmlChiTiet = new StringBuilder();
        //    //if (viewModel.HasChart.HasValue && viewModel.HasChart.Value && !string.IsNullOrEmpty(viewModel.ChartImage))
        //    //{
        //    //    _upload = new JqueryUploadController();
        //    //    string base64 = viewModel.ChartImage.Substring(viewModel.ChartImage.IndexOf(',') + 1);
        //    //    base64 = base64.Trim('\0');
        //    //    byte[] chartData = Convert.FromBase64String(base64);
        //    //    var filePath = _upload.getImagesPath(chartData, widthChart);
        //    //    htmlChart.Append("<h2><b>Biểu đồ</b></h2>");
        //    //    htmlChart.Append(ExportPDF.pdfAnh(filePath, "width: 100%;"));
        //    //}
        //    //if (viewModel.HasTableData.HasValue && viewModel.HasTableData.Value)
        //    //{
        //    var lstData = getDataHieuNangNhomTheoNhom(viewModel);
        //    if (lstData != null)
        //    {
        //        htmlChiTiet.Append("<h2><b>Chi tiết</b></h2>");
        //        var lstCol = new List<string>() { "Nhóm thiết bị", "Tên thiết bị", "IP", "Tên item", "Thời gian", (viewModel.KeyType.Equals("cpu") ? "CPU nhỏ nhất" : viewModel.KeyType.Equals("ram") ? "RAM sử dụng thấp nhất" : viewModel.KeyType.Equals("hdd") ? "Ổ cứng sử dụng đầu kỳ" : "Băng thông sử dụng nhỏ nhất"), (viewModel.KeyType.Equals("cpu") ? "CPU trung bình" : viewModel.KeyType.Equals("ram") ? "RAM sử dụng trung bình" : viewModel.KeyType.Equals("hdd") ? "Ổ cứng sử dụng giữa kỳ" : "Băng thông sử dụng trung bình"), (viewModel.KeyType.Equals("cpu") ? "CPU lớn nhất" : viewModel.KeyType.Equals("ram") ? "RAM sử dụng lớn nhất" : viewModel.KeyType.Equals("hdd") ? "Ổ cứng sử dụng cuối kỳ" : "Băng thông sử dụng lớn nhất") };
        //        if (viewModel.GroupBy.Equals("group"))
        //        {
        //            lstCol = new List<string>() { "Nhóm thiết bị", "Tên item", "Thời gian", (viewModel.KeyType.Equals("cpu") ? "CPU nhỏ nhất" : viewModel.KeyType.Equals("ram") ? "RAM sử dụng thấp nhất" : viewModel.KeyType.Equals("hdd") ? "Ổ cứng sử dụng đầu kỳ" : "Băng thông sử dụng nhỏ nhất"), (viewModel.KeyType.Equals("cpu") ? "CPU trung bình" : viewModel.KeyType.Equals("ram") ? "RAM sử dụng trung bình" : viewModel.KeyType.Equals("hdd") ? "Ổ cứng sử dụng giữa kỳ" : "Băng thông sử dụng trung bình"), (viewModel.KeyType.Equals("cpu") ? "CPU lớn nhất" : viewModel.KeyType.Equals("ram") ? "RAM sử dụng lớn nhất" : viewModel.KeyType.Equals("hdd") ? "Ổ cứng sử dụng cuối kỳ" : "Băng thông sử dụng lớn nhất") };
        //        }
        //        var tblData = ObjectUtils.ConvertToDatatable<HieuNangNhomThietBi_Result>(lstData);
        //        tblData.Columns.RemoveAt(0);
        //        tblData.Columns.RemoveAt(0);
        //        tblData.Columns.RemoveAt(0);
        //        if (viewModel.GroupBy.Equals("group"))
        //        {
        //            tblData.Columns.RemoveAt(1);
        //            tblData.Columns.RemoveAt(1);
        //        }
        //        htmlChiTiet.Append(ExportPDF.pdfBang(lstCol, tblData));
        //    }
        //    //}
        //    return genHtmlForm(tenBaoCao, htmlChart, htmlChiTiet, isPDF);
        //}

        [HttpPost]
        public string ExportWordHieuNangNhom(BaoCaoTongHopListView viewModel)
        {
            var doc = initWordDocument();
            initWord(ref doc, getTitleReport(Utils.Enums.BaoCaoEnum.bwdgroup, viewModel.KeyType, viewModel.GroupBy));
            //if (viewModel.HasChart.HasValue && viewModel.HasChart.Value && !string.IsNullOrEmpty(viewModel.ChartImage))
            //{
            //    _upload = new JqueryUploadController();
            //    string base64 = viewModel.ChartImage.Substring(viewModel.ChartImage.IndexOf(',') + 1);
            //    insertHeader(ref doc, getHeaderTongThietBiNhom());
            //    insertWordChart(ref doc, base64);
            //}
            //if (viewModel.HasTableData.HasValue && viewModel.HasTableData.Value)
            //{
            var lstCol = new List<string>();
            var tblData = new DataTable();
            var header = getHeaderHieuNangNhom(false, getTitleReport(Utils.Enums.BaoCaoEnum.cpugroup, viewModel.KeyType, viewModel.GroupBy));
            getTableColumn(Utils.Enums.BaoCaoEnum.cpugroup, viewModel, ref lstCol, ref tblData);
            insertHeader(ref doc, header);
            insertWordDataTable(tblData, lstCol, ref doc);
            //}
            return saveToWordNew(doc, "HieuNangNhom");
        }

        [HttpPost]
        public string ExportPDFHieuNangNhom(BaoCaoTongHopListView viewModel)
        {
            //var html = genHieuNangNhomHTML(viewModel, getReportTitle(viewModel.KeyType) + (viewModel.GroupBy.Equals("group") ? "nhóm thiết bị" : "thiết bị"), 0, true);
            var htmlData = new StringBuilder();
            //if (viewModel.HasChart.HasValue && viewModel.HasChart.Value && !string.IsNullOrEmpty(viewModel.ChartImage))
            //{
            //    htmlData.Append(getChartHTML(getHeaderTongThietBiNhom(), viewModel.ChartImage));
            //}
            //if (viewModel.HasTableData.HasValue && viewModel.HasTableData.Value)
            //{
            var lstCol = new List<string>();
            var tblData = new DataTable();
            var header = getHeaderHieuNangNhom(false, getTitleReport(Utils.Enums.BaoCaoEnum.cpugroup, viewModel.KeyType, viewModel.GroupBy));
            getTableColumn(Utils.Enums.BaoCaoEnum.cpugroup, viewModel, ref lstCol, ref tblData);
            htmlData.Append("<h2><b>" + header + "</b></h2>");
            htmlData.Append(ExportPDF.pdfBang(lstCol, tblData));
            //}
            var html = genHtmlForm(getHeaderHieuNangNhom(false, getTitleReport(Utils.Enums.BaoCaoEnum.cpugroup, viewModel.KeyType, viewModel.GroupBy)), htmlData);
            return saveToPDF(html, "HieuNangNhom");
        }

        private string getHeaderHieuNangNhom(bool isChart = true, string header = "")
        {
            if (isChart)
            {
                return "Biểu đồ " + header;
            }
            else
            {
                return "Chi tiết " + header;
            }
        }

        #endregion

        //public ActionResult TongThietBiTheoNhom()
        //{
        //    return View();
        //}

        //public ActionResult ChiTietThietBiTheoNhom()
        //{
        //    return View();
        //}
        public ActionResult HieuNangTheoNhom()
        {
            return View();
        }
        public ActionResult HieuNangTheoKhoangTG()
        {
            return View();
        }

        #region Common
        public string getTitleReport(Utils.Enums.BaoCaoEnum enumBaoCao, string keyType = "", string isGroup = "")
        {
            var title = "";
            if (!string.IsNullOrEmpty(keyType))
            {
                title = "Báo cáo băng thông đường truyền theo ";
                if (enumBaoCao == Utils.Enums.BaoCaoEnum.dexuat)
                {
                    title = "Đề xuất nâng cấp băng thông ";
                }
                if (keyType.Equals("cpu"))
                {
                    title = "Báo cáo hiệu năng CPU theo ";
                    if (enumBaoCao == Utils.Enums.BaoCaoEnum.dexuat)
                    {
                        title = "Đề xuất nâng cấp CPU ";
                    }
                }
                else if (keyType.Equals("ram"))
                {
                    title = "Báo cáo hiệu năng RAM theo ";
                    if (enumBaoCao == Utils.Enums.BaoCaoEnum.dexuat)
                    {
                        title = "Đề xuất nâng cấp RAM ";
                    }
                }
                else if (keyType.Equals("hdd"))
                {
                    title = "Báo cáo hiệu năng ổ cứng theo ";
                    if (enumBaoCao == Utils.Enums.BaoCaoEnum.dexuat)
                    {
                        title = "Đề xuất nâng cấp ổ cứng ";
                    }
                }
                if (isGroup.Equals("group"))
                {
                    title += "nhóm thiết bị";
                }
                else { title += "thiết bị"; }
            }
            if (enumBaoCao == Utils.Enums.BaoCaoEnum.tkslnhomthietbi)
            {
                title = "Báo cáo Tổng số lượng thiết bị của nhóm thiết bị";
            }
            else if (enumBaoCao == Utils.Enums.BaoCaoEnum.chitiettbtheonhom)
            {
                title = "Báo cáo giám sát thông tin thiết bị";
            }
            else if (enumBaoCao == Utils.Enums.BaoCaoEnum.vandenhom)
            {
                title = "Báo cáo Tổng số lượng vấn đề của nhóm thiết bị";
            }
            else if (enumBaoCao == Utils.Enums.BaoCaoEnum.chitietvande)
            {
                title = "Thống kê danh sách vấn đề của thiết bị";
            }
            else if (enumBaoCao == Utils.Enums.BaoCaoEnum.bcthall)
            {
                title = "Báo cáo tổng hợp";
            }
            return title;
        }

        public void getTableColumn(TKM.Utils.Enums.BaoCaoEnum enumBaoCao, BaoCaoTongHopListView viewModel, ref List<string> lstCol, ref DataTable tblData, ref List<DeXuatNhomThietBi_Result> lstDeXuat)
        {
            if (enumBaoCao == Utils.Enums.BaoCaoEnum.dexuat)
            {
                var lstData = getDataDeXuatThietBi(viewModel);
                if (lstData != null)
                {
                    lstDeXuat = lstData;
                    lstCol = new List<string>() { "Nhóm thiết bị", "Tên thiết bị", "IP", "Tên item", "Thời gian", (viewModel.KeyType.Equals("cpu") ? "CPU trung bình" : viewModel.KeyType.Equals("ram") ? "RAM sử dụng trung bình" : viewModel.KeyType.Equals("hdd") ? "Ổ cứng sử dụng trung bình" : "Băng thông sử dụng trung bình") };
                    tblData = ObjectUtils.ConvertToDatatable<DeXuatNhomThietBi_Result>(lstData);
                    tblData.Columns.RemoveAt(0);
                    tblData.Columns.RemoveAt(0);
                    tblData.Columns.RemoveAt(0);
                    tblData.Columns.RemoveAt(0);
                    tblData.Columns.RemoveAt(5);
                    tblData.Columns.RemoveAt(6);
                }
            }
        }

        public void getTableColumn(TKM.Utils.Enums.BaoCaoEnum enumBaoCao, BaoCaoTongHopListView viewModel, ref List<string> lstCol, ref DataTable tblData)
        {
            if (enumBaoCao == Utils.Enums.BaoCaoEnum.tkslnhomthietbi)
            {
                var lstData = getDataTongThietBiTheoNhom(viewModel);
                if (lstData != null)
                {
                    lstCol = new List<string>() { "Nhóm thiết bị", "Số lượng thiết bị không gặp vấn đề", "Số lượng thiết bị đang gặp vấn đề", "Tổng số lượng thiết bị" };
                    tblData = ObjectUtils.ConvertToDatatable<CountHostsByGroup_Result>(lstData);
                }
            }
            else if (enumBaoCao == Utils.Enums.BaoCaoEnum.chitiettbtheonhom)
            {
                var lstData = getDataChiTietThietBiTheoNhom(viewModel);
                if (lstData != null && lstData.Count > 0)
                {
                    lstCol = new List<string>() { "Nhóm thiết bị", "Tên thiết bị", "IP", "Số lượng thành phần quản lý", "Số lượng item thiết bị", "Số lượng triggers giám sát" };
                    tblData = ObjectUtils.ConvertToDatatable<HostsByGroup_Result>(lstData);
                }
            }
            else if (enumBaoCao == Utils.Enums.BaoCaoEnum.vandenhom)
            {
                var lstData = getDataTongVanDeTheoNhom(viewModel);
                if (lstData != null && lstData.Count > 0)
                {
                    lstCol = new List<string>() { "Nhóm thiết bị", "Số lượng vấn đề chưa xử lý", "Số lượng vấn đề đã xử lý", "Tổng số lượng vấn đề" };
                    tblData = ObjectUtils.ConvertToDatatable<CountProblemByStatus_Result>(lstData);
                    tblData.Columns.RemoveAt(0);
                    tblData.Columns.RemoveAt(0);
                    tblData.Columns.RemoveAt(1);
                    tblData.Columns.RemoveAt(1);
                }
            }
            else if (enumBaoCao == Utils.Enums.BaoCaoEnum.chitietvande)
            {
                var lstData = getDataChiTietVanDe(viewModel);
                if (lstData != null && lstData.Count > 0)
                {
                    lstCol = new List<string>() { "Nhóm thiết bị", "Tên thiết bị", "IP", "Vấn đề", "Thời gian xuất hiện", "Thời gian tồn tại", "Mức độ nghiêm trọng", "Trạng thái" };
                    tblData = ObjectUtils.ConvertToDatatable<GetProblemDetail_Result>(lstData);
                    tblData.Columns.RemoveAt(0);
                    tblData.Columns.RemoveAt(0);
                    tblData.Columns.RemoveAt(0);
                    tblData.Columns.RemoveAt(0);
                }
            }
            else
            {
                var lstData = getDataHieuNangNhomTheoNhom(viewModel);
                if (lstData != null)
                {
                    lstCol = new List<string>() { "Nhóm thiết bị", "Tên thiết bị", "IP", "Tên item", "Thời gian", (viewModel.KeyType.Equals("cpu") ? "CPU nhỏ nhất" : viewModel.KeyType.Equals("ram") ? "RAM sử dụng thấp nhất" : viewModel.KeyType.Equals("hdd") ? "Ổ cứng sử dụng đầu kỳ" : "Băng thông sử dụng nhỏ nhất"), (viewModel.KeyType.Equals("cpu") ? "CPU trung bình" : viewModel.KeyType.Equals("ram") ? "RAM sử dụng trung bình" : viewModel.KeyType.Equals("hdd") ? "Ổ cứng sử dụng giữa kỳ" : "Băng thông sử dụng trung bình"), (viewModel.KeyType.Equals("cpu") ? "CPU lớn nhất" : viewModel.KeyType.Equals("ram") ? "RAM sử dụng lớn nhất" : viewModel.KeyType.Equals("hdd") ? "Ổ cứng sử dụng cuối kỳ" : "Băng thông sử dụng lớn nhất") };
                    if (viewModel.GroupBy.Equals("group"))
                    {
                        lstCol = new List<string>() { "Nhóm thiết bị", "Tên item", "Thời gian", (viewModel.KeyType.Equals("cpu") ? "CPU nhỏ nhất" : viewModel.KeyType.Equals("ram") ? "RAM sử dụng thấp nhất" : viewModel.KeyType.Equals("hdd") ? "Ổ cứng sử dụng đầu kỳ" : "Băng thông sử dụng nhỏ nhất"), (viewModel.KeyType.Equals("cpu") ? "CPU trung bình" : viewModel.KeyType.Equals("ram") ? "RAM sử dụng trung bình" : viewModel.KeyType.Equals("hdd") ? "Ổ cứng sử dụng giữa kỳ" : "Băng thông sử dụng trung bình"), (viewModel.KeyType.Equals("cpu") ? "CPU lớn nhất" : viewModel.KeyType.Equals("ram") ? "RAM sử dụng lớn nhất" : viewModel.KeyType.Equals("hdd") ? "Ổ cứng sử dụng cuối kỳ" : "Băng thông sử dụng lớn nhất") };
                    }
                    lstCol.Add("Hiệu năng");
                    tblData = ObjectUtils.ConvertToDatatable<HieuNangNhomThietBi_Result>(lstData);
                    if (tblData.Columns.Contains("HieuNang"))
                    {
                        tblData.Columns["HieuNang"].SetOrdinal(tblData.Columns.Count - 1);
                    }
                    tblData.Columns.RemoveAt(0);
                    tblData.Columns.RemoveAt(0);
                    tblData.Columns.RemoveAt(0);
                    tblData.Columns.RemoveAt(0);
                    if (viewModel.GroupBy.Equals("group"))
                    {
                        tblData.Columns.RemoveAt(1);
                        tblData.Columns.RemoveAt(1);
                    }
                }
            }
        }

        public StringBuilder getChartHTML(string title, string chartImage, int widthChart = 0)
        {
            var htmlChart = new StringBuilder();
            _upload = new JqueryUploadController();
            string base64 = chartImage.Substring(chartImage.IndexOf(',') + 1);
            base64 = base64.Trim('\0');
            byte[] chartData = Convert.FromBase64String(base64);
            var filePath = _upload.getImagesPath(chartData, widthChart);
            htmlChart.Append("<h2><b>" + title + "</b></h2>");
            htmlChart.Append(ExportPDF.pdfAnh(filePath, "width: 100%;"));
            return htmlChart;
        }

        public StringBuilder getTableHTML(string title, List<string> lstCol, DataTable tblData)
        {
            var htmlChart = new StringBuilder();
            htmlChart.Append("<h2><b>" + title + "</b></h2>");
            htmlChart.Append(ExportPDF.pdfBang(lstCol, tblData));
            return htmlChart;
        }

        public void initWord(ref DocX doc, string reportTitle)
        {
            var topRightHeader = getAppSetting("TopRightHeader", true);
            var subTitle = getAppSetting("SubTitle", true);
            var bottomCover = getAppSetting("BottomCoverPage", true);
            doc.AddCustomProperty(new CustomProperty("TopRightCover", topRightHeader));
            doc.AddCustomProperty(new CustomProperty("SubTitle", subTitle));
            doc.AddCustomProperty(new CustomProperty("BottomCover", bottomCover));
            doc.AddCustomProperty(new CustomProperty("TitleReport", reportTitle));
            doc.AddCustomProperty(new CustomProperty("NguoiKy", SessionInfo.CurrentUser.HoVaTen));
        }
        public void insertHeader(ref DocX doc, string title, Double fontSize = 18D, bool isBold = true)
        {
            var headerFormat = new Novacode.Formatting();
            headerFormat.Size = fontSize;
            headerFormat.Position = 12;
            headerFormat.Bold = isBold;
            var header = doc.InsertParagraph(title, false, headerFormat);
        }
        public void insertWordChart(ref DocX doc, string base64, float widthExpect = 640)
        {
            base64 = base64.Trim('\0');
            byte[] chartData = Convert.FromBase64String(base64);
            var filePath = _upload.getImagesPath(chartData);
            var imgInit = System.Drawing.Image.FromFile(filePath);
            using (var ms = new MemoryStream())
            {
                imgInit.Save(ms, imgInit.RawFormat);
                ms.Seek(0, SeekOrigin.Begin);
                var img = doc.AddImage(ms);
                var pic1 = img.CreatePicture();
                var p = doc.InsertParagraph();
                var ratio = widthExpect / (float)imgInit.Width;
                var width = (int)(imgInit.Width * ratio);
                var height = (int)(imgInit.Height * ratio);
                pic1.Width = width;
                pic1.Height = height;
                p.InsertPicture(pic1);
            }
        }
        public void insertWordDataTable(DataTable tblInit, List<string> lstCol, ref DocX doc)
        {
            var tblData = doc.InsertTable(tblInit.Rows.Count + 1, lstCol.Count + 1);
            tblData.Design = TableDesign.TableGrid;
            var tableTitle = new Novacode.Formatting();
            tableTitle.FontColor = Color.White;
            tableTitle.Bold = true;
            tblData.Rows[0].Cells[0].InsertParagraph("STT", false, tableTitle).Alignment = Alignment.center;
            tblData.Rows[0].Cells[0].FillColor = Color.DarkSlateBlue;
            for (int i = 1; i <= lstCol.Count; i++)
            {
                var colName = lstCol[i - 1].Replace("|center", "");
                tblData.Rows[0].Cells[i].InsertParagraph(colName, false, tableTitle).Alignment = Alignment.center;
                tblData.Rows[0].Cells[i].FillColor = Color.DarkSlateBlue;
            }

            for (var i = 1; i <= tblInit.Rows.Count; i++)
            {
                tblData.Rows[i].Cells[0].InsertParagraph(i + "", false).Alignment = Alignment.center;
                tblData.Rows[0].Cells[0].FillColor = Color.DarkSlateBlue;
                for (var j = 0; j < lstCol.Count; j++)
                {
                    if (lstCol[j].Contains("|center"))
                    {
                        tblData.Rows[i].Cells[j + 1].InsertParagraph(tblInit.Rows[i - 1][j] + "", false).Alignment = Alignment.center;
                    }
                    else
                    {
                        tblData.Rows[i].Cells[j + 1].InsertParagraph(tblInit.Rows[i - 1][j] + "", false);
                    }
                }
            }
        }
        private string getAppSetting(string key, bool isWord = false)
        {
            var val = "";
            try
            {
                var createDate = DateTime.Now;
                val = System.Configuration.ConfigurationManager.AppSettings[key];
                val = val.Replace("[ngay]", createDate.Day + "");
                val = val.Replace("[thang]", createDate.Month + "");
                val = val.Replace("[nam]", createDate.Year + "");
                val = val.Replace("[gio]", createDate.Hour + "");
                val = val.Replace("[phut]", createDate.Minute + "");
                val = val.Replace("[giay]", createDate.Second + "");
                val = val.Replace("[n]", "\n");
                val = val.Replace("[username]", SessionInfo.CurrentUser.TenDangNhap);
                val = val.Replace("[fullname]", SessionInfo.CurrentUser.HoVaTen);
                if (isWord)
                {
                    val = val.Replace("<br/>", "\n                                                                                                                                              ");
                    val = val.Replace("<h5 style='font-size: 18px;'>", "");
                    val = val.Replace("<b>", "");
                    val = val.Replace("</b>", "");
                    val = val.Replace("</h5>", "");
                }
            }
            catch (Exception ex)
            {
                OutputLog.WriteOutputLog(ex.ToString());
            }
            return val;
        }

        //private StringBuilder genHtmlForm(string tenBaoCao, StringBuilder htmlData, StringBuilder htmlData1, bool isPDF = true)
        //{
        //    var htmlStr = new StringBuilder();
        //    htmlStr.Append(ExportPDF.KhoiTao());
        //    var path = CommonUtils.GetPath("~/Assets/images/tkm-logo.png");
        //    var topRightHeader = getAppSetting("TopRightHeader");
        //    var subTitle = getAppSetting("SubTitle");
        //    var bottomCover = getAppSetting("BottomCoverPage");
        //    htmlStr.Append(ExportPDF.pdfTrangBia(path, topRightHeader, tenBaoCao, subTitle, bottomCover, isPDF));
        //    htmlStr.Append(htmlData);
        //    htmlStr.Append(ExportPDF.KetThuc());
        //    return htmlStr;
        //}

        private StringBuilder genHtmlForm(string tenBaoCao, StringBuilder htmlData, bool isPDF = true)
        {
            var htmlStr = new StringBuilder();
            htmlStr.Append(ExportPDF.KhoiTao());
            var path = CommonUtils.GetPath("~/Assets/images/tkm-logo.png");
            var topRightHeader = getAppSetting("TopRightHeader");
            var subTitle = getAppSetting("SubTitle");
            var bottomCover = getAppSetting("BottomCoverPage");
            var urlRoot = Request.Url.OriginalString.Replace(Request.Url.PathAndQuery, "");
            htmlStr.Append(ExportPDF.pdfTrangBia(path, topRightHeader, tenBaoCao, subTitle, bottomCover, isPDF, SessionInfo.CurrentUser.HoVaTen, urlRoot));
            htmlStr.Append(htmlData);
            htmlStr.Append(ExportPDF.KetThuc());
            return htmlStr;
        }

        //private void saveToWord(StringBuilder htmlStr, string fName)
        //{
        //    try
        //    {
        //        Response.Clear();
        //        Response.Charset = "";
        //        //Response.ContentType = "application/msword";
        //        Response.ContentType = "application/vnd.ms-word";
        //        Response.AddHeader("Content-Disposition", "attachment;filename=" + fName + ".doc");
        //        StringBuilder strHTMLContent = new StringBuilder();
        //        strHTMLContent.Append(htmlStr);
        //        Response.Write(strHTMLContent);
        //        Response.End();
        //        Response.Flush();
        //    }
        //    catch (Exception ex)
        //    {
        //        OutputLog.WriteOutputLog("BaoCao/saveToWord: " + ex.ToString());
        //    }
        //}

        private DocX initWordDocument()
        {
            var path = CommonUtils.GetPath("~/Assets/Template/ReportTemplate.docx");
            DocX doc = DocX.Load(path);
            return doc;
        }
        private string saveToWordNew(DocX doc, string fName)
        {
            var pathReturn = "";
            try
            {
                var folder = "/TempFile/Word";
                var folderFull = CommonUtils.GetPath("~" + folder);
                if (!Directory.Exists(folderFull))
                    Directory.CreateDirectory(folderFull);
                var fileName = string.Format("/Test" + "_{0:yyyyMMddHHmmss}", DateTime.Now) + ".docx";
                var filePath = folderFull + fileName;
                doc.SaveAs(filePath);
                pathReturn = folder + fileName;
            }
            catch (Exception ex)
            {
                OutputLog.WriteOutputLog("BaoCao/saveToWord: " + ex.ToString());
            }
            return pathReturn;
        }

        private string saveToPDF(StringBuilder htmlStr, string fName)
        {
            var pathReturn = "";
            try
            {
                string footer = getAppSetting("LeftFooter");
                var document = new HtmlToPdfDocument
                {
                    GlobalSettings =
                    {
                        ProduceOutline = true,
                        DocumentTitle = "Pretty Websites",
                        PaperSize = PaperKind.A4, // Implicit conversion to PechkinPaperSize
                        Margins = { All = 1.375, Unit = Unit.Centimeters }
                    },
                    Objects = {
                        new ObjectSettings { HtmlText = htmlStr.ToString(),  WebSettings = new WebSettings { EnableIntelligentShrinking = true,DefaultEncoding="UTF-8", PrintMediaType =true }
                        , FooterSettings = new FooterSettings{LeftText = footer, RightText = "Trang [page] / [topage]", FontSize = 10, FontName = "Times New Roman", ContentSpacing = 6 }
                        }
                    }
                };
                IConverter converter = new ThreadSafeConverter(
                    new RemotingToolset<PdfToolset>(
                        new WinAnyCPUEmbeddedDeployment(
                            new TempFolderDeployment())));
                byte[] result = TuesPechkinInitializerService.converter.Convert(document);

                var folder = "/TempFile/PDF";
                var folderFull = CommonUtils.GetPath("~" + folder);
                if (!Directory.Exists(folderFull))
                    Directory.CreateDirectory(folderFull);
                var dt = DateTime.Now;
                var fileName = string.Format("/" + fName + "_{0:yyyyMMddHHmmss}", DateTime.Now) + ".pdf";
                var filePath = folderFull + fileName;
                System.IO.File.WriteAllBytes(filePath, result);
                pathReturn = folder + fileName;
            }
            catch (Exception ex)
            {
                OutputLog.WriteOutputLog("Export PDF: " + ex.ToString());
            }
            return pathReturn;
        }
        #endregion

        //#region TestSection
        //private void saveToWord1(string htmlStr)
        //{
        //    var path = CommonUtils.GetPath("~/2021/chartSave.png");
        //    Byte[] bytesimg = System.IO.File.ReadAllBytes(path);
        //    //String file = Convert.ToBase64String(bytesimg);
        //    //string filePath = Server.MapPath("~/2021/chartSave.png");
        //    //System.IO.File.WriteAllBytes(filePath, bytesimg);
        //    _upload = new JqueryUploadController();
        //    var filePath = _upload.getImagesPath(bytesimg);
        //    var filePathResize = _upload.getImagesPath(bytesimg, 300);


        //    var timeStr = DateTime.Now.ToString("yyyyMMddHHmm");
        //    var htmlString = new StringBuilder();

        //    string tenFile = "tesst";
        //    string widthtable = "";
        //    string baseUrl = "";
        //    try
        //    {
        //        string headerUrl = CommonUtils.GetPath("~/2021/test.html");
        //        DataTable tblData = new DataTable();
        //        tblData.Columns.Add("stt");
        //        tblData.Columns.Add("col1");
        //        tblData.Columns.Add("col2");
        //        for (int i = 0; i < 500; i++)
        //        {
        //            var row = tblData.NewRow();
        //            row[0] = i + 1;
        //            row[1] = "data-col1-" + i;
        //            row[2] = "Col2-col2-" + i;
        //            tblData.Rows.Add(row);
        //        }
        //        List<string> lstCol = new List<string>() { "width: 50px;|STT", "Cột 1", "Cột 2" };
        //        htmlString.Append(ExportPDF.KhoiTao());

        //        path = CommonUtils.GetPath("~/Assets/images/tkm-logo.png");
        //        htmlString.Append(ExportPDF.pdfTrangBia(path, "<h5><b>CÔNG TY CỔ PHẦN CÔNG NGHỆ TKM VIỆT NAM</b></h5><p><b>Trụ sở chính:</b> số 28 , lô 14, Trung Yên 11, Trung Hòa, Cầu Giấy, Hà Nội</p><p><b>Điện thoại:</b> (04) 558 9970 - <b>Fax:</b> (04) 558 9971</p><p><b>Email:</b> info@tkmgroup.com.vn    - <b>Website:</b> https://tkmgroup.com.vn</p>", "Báo cáo tổng hợp", "Hệ thống quản lý và giám sát SYSMAN", DateTime.Now.Year.ToString()));
        //        htmlString.Append("<h2><b>1. Báo cáo hiệu năng thiết bị</b></h2>");
        //        htmlString.Append(ExportPDF.pdfAnh(filePath, "width: 100%;"));
        //        htmlString.Append(ExportPDF.pdfAnh(filePathResize, "width: 100%;"));
        //        htmlString.Append(ExportPDF.pdfBang(lstCol, tblData));
        //        htmlString.Append(ExportPDF.KetThuc());

        //        Response.Clear();
        //        Response.Charset = "";
        //        Response.ContentType = "application/msword";
        //        string strFileName = "docName" + ".doc";
        //        Response.AddHeader("Content-Disposition", "attachment;filename=" + strFileName);

        //        StringBuilder strHTMLContent = new StringBuilder();
        //        strHTMLContent.Append(htmlString.ToString());

        //        Response.Write(strHTMLContent);
        //        Response.End();
        //        Response.Flush();
        //    }
        //    catch (Exception ex)
        //    {
        //        OutputLog.WriteOutputLog("Export PDF: " + ex.ToString());
        //    }
        //}

        //private void saveToWord1()
        //{
        //    try
        //    {
        //        var path = CommonUtils.GetPath("~/2021/ReportTemplate.docx");
        //        DocX doc = DocX.Load(path);
        //        path = CommonUtils.GetPath("~/2021/chartSave.png");
        //        var logo = System.Drawing.Image.FromFile(path);
        //        using (var ms = new MemoryStream())
        //        {
        //            logo.Save(ms, logo.RawFormat);
        //            ms.Seek(0, SeekOrigin.Begin);
        //            var img = doc.AddImage(ms);
        //            var pic1 = img.CreatePicture();
        //            var p = doc.InsertParagraph();
        //            pic1.Width = 300;
        //            pic1.Height = 300;
        //            p.InsertPicture(pic1);
        //            //p.InsertParagraphBeforeSelf(template.InsertParagraph());
        //        }

        //        var folder = "/TempFile/PDF";
        //        var folderFull = CommonUtils.GetPath("~" + folder);
        //        var dt = DateTime.Now;
        //        var fileName = string.Format("/Test" + "_{0:yyyyMMddHHmmss}", dt) + ".docx";
        //        var filePath = folderFull + fileName;
        //        doc.SaveAs(filePath);
        //    }
        //    catch (Exception ex)
        //    {
        //        OutputLog.WriteOutputLog("BaoCao/saveToWord: " + ex.ToString());
        //    }
        //}
        //private void saveToPDF111()
        //{
        //    var path = CommonUtils.GetPath("~/2021/chartSave.png");
        //    Byte[] bytesimg = System.IO.File.ReadAllBytes(path);
        //    //String file = Convert.ToBase64String(bytesimg);
        //    //string filePath = Server.MapPath("~/2021/chartSave.png");
        //    //System.IO.File.WriteAllBytes(filePath, bytesimg);
        //    _upload = new JqueryUploadController();
        //    var filePath = _upload.getImagesPath(bytesimg);
        //    var filePathResize = _upload.getImagesPath(bytesimg, 300);


        //    var timeStr = DateTime.Now.ToString("yyyyMMddHHmm");
        //    var htmlString = new StringBuilder();

        //    string tenFile = "tesst";
        //    string widthtable = "";
        //    string baseUrl = "";
        //    try
        //    {
        //        string headerUrl = CommonUtils.GetPath("~/2021/test.html");
        //        DataTable tblData = new DataTable();
        //        tblData.Columns.Add("stt");
        //        tblData.Columns.Add("col1");
        //        tblData.Columns.Add("col2");
        //        for (int i = 0; i < 500; i++)
        //        {
        //            var row = tblData.NewRow();
        //            row[0] = i + 1;
        //            row[1] = "data-col1-" + i;
        //            row[2] = "Col2-col2-" + i;
        //            tblData.Rows.Add(row);
        //        }
        //        List<string> lstCol = new List<string>() { "width: 50px;|STT", "Cột 1", "Cột 2" };
        //        htmlString.Append(ExportPDF.KhoiTao());

        //        path = CommonUtils.GetPath("~/Assets/images/tkm-logo.png");
        //        htmlString.Append(ExportPDF.pdfTrangBia(path, "<h5><b>CÔNG TY CỔ PHẦN CÔNG NGHỆ TKM VIỆT NAM</b></h5><p><b>Trụ sở chính:</b> số 28 , lô 14, Trung Yên 11, Trung Hòa, Cầu Giấy, Hà Nội</p><p><b>Điện thoại:</b> (04) 558 9970 - <b>Fax:</b> (04) 558 9971</p><p><b>Email:</b> info@tkmgroup.com.vn    - <b>Website:</b> https://tkmgroup.com.vn</p>", "Báo cáo tổng hợp", "Hệ thống quản lý và giám sát SYSMAN", DateTime.Now.Year.ToString()));
        //        htmlString.Append("<h2><b>1. Báo cáo hiệu năng thiết bị</b></h2>");
        //        htmlString.Append(ExportPDF.pdfAnh(filePath, "width: 100%;"));
        //        htmlString.Append(ExportPDF.pdfAnh(filePathResize, "width: 100%;"));
        //        htmlString.Append(ExportPDF.pdfBang(lstCol, tblData));
        //        htmlString.Append(ExportPDF.KetThuc());


        //        //htmlString = new StringBuilder("<html xmlns:o='urn:schemas-microsoft-com:office:office' xmlns:w='urn:schemas-microsoft-com:office:word' xmlns='http://www.w3.org/TR/REC-html40'><head><meta charset='utf-8'></head><body style='margin: 0; font-family: 'Times New Roman',Roboto,Arial,'Noto Sans',sans-serif; font-size: 1rem; font-weight: 400; line-height: 1.5; color: #212529; text-align: left; background-color: #fff;'><div style='page-break-after: always;'><table><tbody><tr><td style='width: 35%;'><img src='https://tkmgroup.com.vn/wp-content/uploads/2020/02/tkm-logo.png' /></td><td colspan='2' style='text-align: right;'><h5><b>CÔNG TY CỔ PHẦN CÔNG NGHỆ TKM VIỆT NAM</b></h5><p style='margin: 2px 0;'><b>Trụ sở chính:</b> số 28 , lô 14, Trung Yên 11, Trung Hòa, Cầu Giấy, Hà Nội</p><p style='margin: 2px 0;'><b>Điện thoại:</b> (04) 558 9970 - <b>Fax:</b> (04) 558 9971</p><p style='margin: 2px 0;'><b>Email:</b> info@tkmgroup.com.vn - <b>Website:</b> https://tkmgroup.com.vn/</p></div></div></td></tr></tbody></table><hr style='border-top: 1px solid rgba(0,0,0,.1);width: 100%;'/><div style='width: 100%; margin-bottom: 1rem; text-align: center;'><br/><br/><br/><br/><br/><br/><br/><br/><br/><br/><br/><br/><br/><br/><br/><br/><br/><br/><h1>Báo cáo tổng hợp</h1><h2>Hệ thống quản lý và giám sát SYSMAN</h2><br/><br/><br/><br/><br/><br/><br/><br/><br/><br/><br/><br/><br/><br/><br/><br/><br/><br/><br/><br/><br/><br/><br/><br/><b>2021</b></div></div></div><table style='border: 1px solid #dee2e6; width: 100%;'><tbody><tr style='background-color: #5c6bc0; color: #fff;font-weight: bold;'><th style='width: 50px; padding: 8px;border-bottom: 2px solid #dee2e6;border: 1px solid #dee2e6;'>STT</th><th style='padding: 8px;border-bottom: 2px solid #dee2e6;border: 1px solid #dee2e6;'>Text</th></tr><tr><td style='padding: 8px;border: 1px solid #dee2e6;'>1</td><td style='padding: 8px;border: 1px solid #dee2e6;'>Test</td></tr><tr><td style='padding: 8px;border: 1px solid #dee2e6;'>2</td><td style='padding: 8px;border: 1px solid #dee2e6;'><b>bold</b></td></tr></tbody></table> <img style='page-break-inside: avoid;' src='https://tkmgroup.com.vn/wp-content/uploads/2020/02/tkm-logo.png'></img></div></body></html>");
        //        //Response.Clear();
        //        //Response.Buffer = true;
        //        //Response.AddHeader("content-disposition", "attachment;filename=Gridtesst.doc");
        //        //Response.Charset = "";
        //        //Response.ContentType = "application/vnd.ms-word";
        //        //Response.Output.Write(htmlString.ToString());
        //        //Response.Flush();
        //        //Response.End();

        //        Response.Clear();
        //        Response.Charset = "";
        //        Response.ContentType = "application/msword";
        //        string strFileName = "docName" + ".doc";
        //        Response.AddHeader("Content-Disposition", "attachment;filename=" + strFileName);

        //        StringBuilder strHTMLContent = new StringBuilder();
        //        //strHTMLContent.Append("<html xmlns:v=\"urn:schemas-microsoft-com:vml\" xmlns:o=\"urn:schemas-microsoft-com:office:office\" xmlns:x=\"urn:schemas-microsoft-com:office:word\" xmlns=\"http://www.w3.org/TR/REC-html40\"><head></head><body>");
        //        strHTMLContent.Append(htmlString.ToString());
        //        //strHTMLContent.Append("</body></html>");

        //        Response.Write(strHTMLContent);
        //        Response.End();
        //        Response.Flush();





        //        var document = new HtmlToPdfDocument
        //        {
        //            GlobalSettings =
        //            {
        //                ProduceOutline = true,
        //                DocumentTitle = "Pretty Websites",
        //                PaperSize = PaperKind.A4, // Implicit conversion to PechkinPaperSize
        //                Margins =
        //                {
        //                    All = 1.375,
        //                    Unit = Unit.Centimeters
        //                }
        //            },
        //            Objects = {
        //                new ObjectSettings { HtmlText = htmlString.ToString(),  WebSettings = new WebSettings { EnableIntelligentShrinking = true,DefaultEncoding="UTF-8", PrintMediaType =true }
        //                , FooterSettings = new FooterSettings{LeftText = "Công ty TKM – Lưu hành\nBM_[TKM]_BCTK[SYSMAN] –1.1", RightText = "Trang [page] / [topage]", FontSize = 10, FontName = "Times New Roman",ContentSpacing = 6 }
        //                }
        //            }
        //        };
        //        IConverter converter = new ThreadSafeConverter(
        //            new RemotingToolset<PdfToolset>(
        //                new WinAnyCPUEmbeddedDeployment(
        //                    new TempFolderDeployment())));
        //        byte[] result = TuesPechkinInitializerService.converter.Convert(document);
        //        //byte[] result = converter.Convert(document);

        //        filePath = Server.MapPath("~/2021/htmlToPDF.pdf");
        //        System.IO.File.WriteAllBytes(filePath, result);

        //        //HtmlToPdf converter = new HtmlToPdf();
        //        //ExportPDF.setConfigCommon(ref converter);
        //        //ExportPDF.setFooter(ref converter, headerUrl);
        //        //DataTable tblData = new DataTable();
        //        //tblData.Columns.Add("stt");
        //        //tblData.Columns.Add("col1");
        //        //tblData.Columns.Add("col2");
        //        //for(int i = 0; i<10; i++)
        //        //{
        //        //    var row = tblData.NewRow();
        //        //    row[0] = i + 1;
        //        //    row[1] = "data-col1-" + i;
        //        //    row[2] = "Col2-col2-" + i;
        //        //    tblData.Rows.Add(row);
        //        //}
        //        //List<string> lstCol = new List<string>() { "width: 50px;|STT", "Cột 1", "Cột 2" };
        //        //htmlString.Append(ExportPDF.KhoiTao());
        //        //htmlString.Append(ExportPDF.pdfBang(lstCol, tblData));
        //        //htmlString.Append(ExportPDF.KetThuc());

        //        //StringReader sr = new StringReader(htmlStr);
        //        //Document pdfDoc = new Document(PageSize.A4, 10f, 10f, 10f, 0f);
        //        //HTMLWorker htmlparser = new HTMLWorker(pdfDoc);
        //        //using (MemoryStream memoryStream = new MemoryStream())
        //        //{
        //        //    //PdfWriter writer = PdfWriter.GetInstance(pdfDoc, memoryStream);
        //        //    //pdfDoc.Open();
        //        //    //pdfDoc.HtmlStyleClass = "table {border: 1px solid #dee2e6;width: 100%;}table thead {background-color: #5c6bc0!important;color: #fff;}.table thead th {vertical-align: middle;}.table-bordered thead td, .table-bordered thead th {border-bottom: 2px solid #dee2e6;}table th {font-weight: bold !important;}.table-bordered td, .table-bordered th {border: 1px solid #dee2e6;}table.table td, table.table th {padding-top: 8px;padding-bottom: 8px;padding-left: 8px;padding-right: 8px;}.table td {font-weight: 400;color: #4f4f4f;} img { display: block; page-break-before: always; page-break-inside: avoid; }";
        //        //    //var styleCSS = new StyleSheet();
        //        //    //var dictionCSS = new Dictionary<string, string>();
        //        //    //dictionCSS.Add("border", "1px solid #dee2e6");
        //        //    //styleCSS.ApplyStyle("table thead", dictionCSS);
        //        //    //htmlparser.SetStyleSheet(styleCSS);
        //        //    //htmlparser.Parse(sr);
        //        //    //pdfDoc.Close();

        //        //    PdfWriter writer = PdfWriter.GetInstance(pdfDoc, memoryStream);
        //        //    pdfDoc.Open();
        //        //    XMLWorkerHelper.GetInstance().ParseXHtml(writer, pdfDoc, sr);
        //        //    pdfDoc.Close();

        //        //    byte[] bytes = memoryStream.ToArray();
        //        //    var filepath = Server.MapPath("~/2021/htmltopdf.pdf");
        //        //    System.IO.File.WriteAllBytes(filepath, bytes);
        //        //    memoryStream.Close();
        //        //}




        //        //PdfDocument doc = converter.ConvertHtmlString(htmlString.ToString(), baseUrl);

        //        //byte[] pdf = doc.Save();
        //        //doc.Close();
        //        //filePath = Server.MapPath("~/2021/htmlToPDF.pdf");
        //        //System.IO.File.WriteAllBytes(filePath, pdf);
        //    }
        //    catch (Exception ex)
        //    {
        //        OutputLog.WriteOutputLog("Export PDF: " + ex.ToString());
        //    }
        //    //// return resulted pdf document
        //    //FileResult fileResult = new FileContentResult(pdf, "application/pdf");
        //    //fileResult.FileDownloadName = "PDF-" + tenFile + "-" + timeStr + ".pdf";
        //}
        //#endregion
    }
}