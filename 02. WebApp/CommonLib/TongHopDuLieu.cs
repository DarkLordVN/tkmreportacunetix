using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using TKM.BLL;
using TKM.Services;
using TKM.Utils;

namespace TKM.WebApp
{
    public static class TongHopDuLieu
    {
        private static Thread _tongHopThread;

        private static Timer timer;
        private static void SetUpTimer()
        {
            var thamSoService = new HeThongThamSoService();
            TimeSpan alertTime = new TimeSpan(23, 59, 59);
            var timeReport = thamSoService.GetByFilter(y => y.MaThamSo.Equals("TimeReport"));
            if (timeReport != null && !string.IsNullOrEmpty(timeReport.MoTa))
            {
                var timeCheck = CommonUtils.TryParseInt(timeReport.MoTa);
                if (timeCheck > 0)
                {
                    alertTime = new TimeSpan(timeCheck);
                }
            }
            var timeNow = DateTime.Now.AddSeconds(3);
            alertTime = new TimeSpan(timeNow.Hour, timeNow.Minute, timeNow.Second);
            DateTime current = DateTime.Now;
            TimeSpan timeToGo = alertTime - current.TimeOfDay;
            OutputLog.WriteOutputLog("timeToGo - " + timeToGo.Hours + ":" + timeToGo.Minutes + ":" + timeToGo.Seconds, "checkTime");
            if (timeToGo < TimeSpan.Zero)
            {
                return;
            }
            timer = new Timer(x =>
            {
                var tsDuongDan = thamSoService.GetByFilter(y => y.MaThamSo.Equals("FolderReport") && y.TrangThai);
                if (tsDuongDan != null && tsDuongDan.ID > 0)
                {
                    ThreadTongHop(tsDuongDan.MoTa);
                }
                //ThreadTongHop();
            }, null, timeToGo, Timeout.InfiniteTimeSpan);
        }
        public static void createThreadTongHop()
        {
            SetUpTimer();
        }
        private static void ThreadTongHop(string folderPath)
        {
            if (_tongHopThread == null || (_tongHopThread != null && !_tongHopThread.IsAlive))
            {
                //var path = @"D:\Project\TKM_Report_Acunetix\Documents\FILE_REPORTS.html";
                _tongHopThread = new Thread(new ThreadStart(() => TongHop(folderPath)), 1);
                _tongHopThread.Start();
            }
        }
        public static void TongHop(string folderPath)
        {
            DirectoryInfo d = new DirectoryInfo(folderPath);
            if (d.Exists)
            {
                FileInfo[] files = d.GetFiles("*.html");
                if (files != null && files.Count() > 0)
                {
                    foreach (var file in files)
                    {
                        var pathNew = folderPath + "\\DaTongHop\\" + DateTime.Now.ToString("yyyyMMdd");
                        var dNew = new DirectoryInfo(pathNew);
                        if (!dNew.Exists)
                        {
                            Directory.CreateDirectory(pathNew);
                        }
                        var fileNew = pathNew + "\\" + file.Name;
                        TongHopChiTiet(file.FullName, fileNew);
                    }
                }
            }
            SetUpTimer();
        }
        public static void TongHopChiTiet(string rootPath, string newPath)
        {
            OutputLog.WriteOutputLog("Start Tong hop - " + DateTime.Now, "checkTime");
            HtmlDocument document = new HtmlDocument();
            //document.Load(@"D:\Project\TKM_Report_Acunetix\Documents\FILE_REPORTS_1.html");
            document.Load(rootPath);
            var innerHtml = document.DocumentNode.InnerHtml;
            string[] delimitedStr = { "<h2" };
            var lsWebsiteHtmlSplit = innerHtml.Split(delimitedStr, StringSplitOptions.None);
            if (lsWebsiteHtmlSplit != null && lsWebsiteHtmlSplit.Count() > 0)
            {
                try
                {
                    var lsReports = new List<WebsiteScanViewModel>();
                    WebsiteScanViewModel report = null;
                    for (int i = 1; i < lsWebsiteHtmlSplit.Count(); i++)
                    {
                        var websiteHtml = "<h2 " + lsWebsiteHtmlSplit[i].Replace("</body>", "").Replace("</html>", "");
                        document.LoadHtml(websiteHtml);
                        var nodeWebsite = document.DocumentNode.DescendantNodes(0).ToList();
                        report = new WebsiteScanViewModel();
                        foreach (HtmlNode item in nodeWebsite)
                        {
                            if (item.Name == "h4" && !item.HasAttributes)
                            {
                                report.ThreatLevel = item.InnerText;
                            }
                            if (item.Name == "table")
                            {
                                //Xu ly thong tin chung
                                if (item.HasClass("ax-scan-summary"))
                                {
                                    var temp = new HtmlDocument();
                                    temp.LoadHtml(item.InnerHtml);
                                    if (temp != null && temp.DocumentNode != null)
                                    {
                                        var trNodes = temp.DocumentNode.SelectNodes("//tr");
                                        if (trNodes != null && trNodes.Count > 0)
                                        {
                                            foreach (var trNode in trNodes)
                                            {
                                                var tdNodes = trNode.ChildNodes.Where(x => x.Name == "td");
                                                if (tdNodes != null && tdNodes.Count() > 1)
                                                {
                                                    var keyNode = tdNodes.ElementAt(0).InnerText;
                                                    var valNode = tdNodes.ElementAt(1).InnerText;
                                                    switch (keyNode.ToLower())
                                                    {
                                                        case "start url":
                                                            report.vmWebsite.StartURL = valNode; break;
                                                        case "host":
                                                            report.vmWebsite.Host = valNode; break;
                                                        case "server information":
                                                            report.vmWebsite.ServerInformation = valNode; break;
                                                        case "responsive":
                                                            report.vmWebsite.Responsive = valNode; break;
                                                        case "server OS":
                                                            report.vmWebsite.ServerOS = valNode; break;
                                                        case "server technologies":
                                                            report.vmWebsite.ServerTechnologies = removeHtmlEscape(valNode); break;
                                                        case "start time":
                                                            var startTime = DateTime.MinValue;
                                                            DateTime.TryParse(valNode, out startTime);
                                                            report.StartTime = startTime;
                                                            break;
                                                        case "profile":
                                                            report.ScanProfile = valNode; break;
                                                        case "scan time":
                                                            report.ScanTime = valNode;
                                                            if (!string.IsNullOrEmpty(valNode))
                                                            {
                                                                if (valNode.Contains(","))
                                                                {
                                                                    var valSplit = valNode.Split(',');
                                                                    if (valSplit != null && valSplit.Count() > 0)
                                                                    {
                                                                        foreach (var valSecond in valSplit)
                                                                        {
                                                                            report.TotalSecond += getSeconds(valSecond);
                                                                        }
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    report.TotalSecond = getSeconds(valNode);
                                                                }
                                                            }
                                                            break;
                                                        case "scan status":
                                                            report.ScanStatus = valNode; break;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                else if (item.HasClass("ax-alerts-distribution"))
                                {
                                    var temp = new HtmlDocument();
                                    temp.LoadHtml(item.InnerHtml);
                                    if (temp != null && temp.DocumentNode != null)
                                    {
                                        var trNodes = temp.DocumentNode.SelectNodes("//tr");
                                        if (trNodes != null && trNodes.Count > 0)
                                        {
                                            foreach (var trNode in trNodes)
                                            {
                                                var tdNodes = trNode.ChildNodes.Where(x => x.Name == "td");
                                                if (tdNodes != null && tdNodes.Count() > 1)
                                                {
                                                    var keyNode = tdNodes.ElementAt(0).InnerText;
                                                    var valNode = tdNodes.ElementAt(1).InnerText;
                                                    if (keyNode.ToLower().Contains("total alerts found"))
                                                    {
                                                        report.TotalAlertFound = CommonUtils.TryParseInt(valNode);
                                                    }
                                                    else if (keyNode.ToLower().Contains("high"))
                                                    {
                                                        report.HighAlert = CommonUtils.TryParseInt(valNode);
                                                    }
                                                    else if (keyNode.ToLower().Contains("medium"))
                                                    {
                                                        report.MediumAlert = CommonUtils.TryParseInt(valNode);
                                                    }
                                                    else if (keyNode.ToLower().Contains("low"))
                                                    {
                                                        report.LowAlert = CommonUtils.TryParseInt(valNode);
                                                    }
                                                    else if (keyNode.ToLower().Contains("informational"))
                                                    {
                                                        report.InforAlert = CommonUtils.TryParseInt(valNode);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                //Xu ly luu danh sach nguy co
                                else
                                {
                                    var affectedItem = new AffectedItemViewModel();
                                    var temp = new HtmlDocument();
                                    temp.LoadHtml(item.InnerHtml);
                                    if (temp != null && temp.DocumentNode != null)
                                    {
                                        var trNodes = temp.DocumentNode.SelectNodes("//tr");
                                        if (trNodes != null && trNodes.Count > 0)
                                        {
                                            foreach (var trNode in trNodes)
                                            {
                                                var tdNodes = trNode.ChildNodes.Where(x => x.Name == "td");
                                                if (tdNodes != null)
                                                {
                                                    if (tdNodes.Count() == 1)
                                                    {
                                                        var tdNode = tdNodes.ElementAt(0);
                                                        if (tdNode.HasAttributes)
                                                        {
                                                            affectedItem.Detail = tdNode.InnerText.Replace("<code style=\"white - space: pre - line\">", "").Replace("</code>", "");
                                                        }
                                                        else
                                                        {
                                                            if (tdNode.InnerText.ToLower().Equals("alert variants"))
                                                            {

                                                                affectedItem.vmAlertGroup.AlertVariants = tdNode.InnerText;
                                                            }
                                                            else
                                                            {
                                                                affectedItem.ScanPath = tdNode.InnerText;
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        var keyNode = tdNodes.ElementAt(0).InnerText;
                                                        var valNode = tdNodes.ElementAt(1).InnerText;
                                                        keyNode = removeHtmlEscape(keyNode);
                                                        valNode = removeHtmlEscape(valNode);
                                                        switch (keyNode.ToLower())
                                                        {
                                                            case "alert group":
                                                                affectedItem.vmAlertGroup.AlertName = valNode; break;
                                                            case "severity":
                                                                affectedItem.vmAlertGroup.Severity = valNode; break;
                                                            case "description":
                                                                affectedItem.vmAlertGroup.AlertDescription = valNode; break;
                                                            case "recommendations":
                                                                affectedItem.vmAlertGroup.Recommendations = valNode; break;
                                                            case "alert variants":
                                                                affectedItem.vmAlertGroup.AlertVariants = valNode; break;
                                                            case "details":
                                                                affectedItem.Detail = valNode; break;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        report.LsAffectedItem.Add(affectedItem);
                                    }
                                }
                            }
                            //Xu ly luu danh sach item
                            if (item.Name == "font")
                            {
                                report.LsScanItem.Add(new ScannedItemViewModel() { Status = (item.OuterHtml.Contains("color=\"green\"") ? true : false), FullPath = item.InnerText });
                                report.LsWebsiteItem.Add(new WebsiteItemViewModel() { Status = true, FullPath = item.InnerText });
                            }
                        }
                        //xu ly khac
                        if (report.LsScanItem != null)
                            report.TotalItemScan = report.LsScanItem.Count();
                        report.RootReportUrl = rootPath;
                        report.Status = true;
                        report.ReportHTML = websiteHtml;
                        lsReports.Add(report);
                    }

                    ////xu ly insert bao cao cuoi
                    //if (report.LsScanItem != null)
                    //    report.TotalItemScan = report.LsScanItem.Count();
                    //report.RootReportUrl = newPath;
                    //report.Status = true;
                    //lsReports.Add(report);
                    var websiteScanService = new WebsiteScanService();
                    var check = websiteScanService.Add(lsReports);
                    if (check)
                    {
                        if (File.Exists(newPath))
                        {
                            File.Delete(newPath);
                        }
                        File.Move(rootPath, newPath);
                    }
                }
                catch (Exception ex)
                {
                    OutputLog.WriteOutputLog(ex.ToString());
                }
            }
        }

        private static int getSeconds(string strTime)
        {
            try
            {
                if (!string.IsNullOrEmpty(strTime))
                {
                    var splitTime = strTime.Trim().Split(' ');
                    if (splitTime != null && splitTime.Count() > 1)
                    {
                        var heSo = 1;
                        switch (splitTime[1])
                        {
                            case "minute": heSo = 60; break;
                            case "minutes": heSo = 60; break;
                            case "hour": heSo = 3600; break;
                            case "hours": heSo = 3600; break;
                            case "day": heSo = 86400; break;
                            case "days": heSo = 86400; break;
                        }
                        var secondConvert = 0;
                        int.TryParse(splitTime[0], out secondConvert);
                        return secondConvert * heSo;
                    }
                }
                return 0;
            }
            catch
            {
                return 0;
            }
        }

        private static string removeHtmlEscape(string strValue)
        {
            if (!string.IsNullOrEmpty(strValue))
            {
                strValue = strValue.Replace("<b>", "").Replace("</b>", "");
                strValue = strValue.Replace("\n", "");
                return strValue.Trim();
            }
            return "";
        }
    }
}