using Microsoft.Office.Interop.Excel;
using Microsoft.Practices.EnterpriseLibrary.Data;
using PeerAMid.Business;
using PeerAMid.Data;
using PeerAMid.Support;
using PeerAMid.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Web.Mvc;
using DataTable = System.Data.DataTable;

#nullable enable

namespace YardStickPortal;

/// <summary>
///     Maximum function of application put under below class(Create new company, Edit company, Search data, Save data,
///     GetTargetCompanyDetails, BenchmarkCompanySearchResults..etc).
///     Session related all changes(Data insert in session, Clear data from session) only handle in PeerAMidController
///     class.
///     PPT, Excel creation code written PeerAMidController class.
///     When any request come from UI end then first request handle by below class and then we are calling API which is
///     internally calling data layer for fetch and save data in database.
/// </summary>
[Authorize]
public partial class PeerAMidController : Controller
{
    /// <summary>
    ///     After click on Export the Deliverable button Download PPT and redirect request on SavePPTReport function.
    /// </summary>
    /// <param name="parameters"></param>
    /// <returns></returns>
    private string CreateSga(ReportParameters parameters)
    {
        Impersonator? impersonator = null;
        try
        {
            var impersonationSetting = ConfigurationManager.AppSettings.GetForThisMachine("Impersonation", "");
            string? impersonationUserName = null;
            string? impersonationPassword = null;
            string impersonationDomain = Environment.MachineName;

            Log.Debug($"Impersonation setting \"{impersonationSetting}\"");
            if (string.IsNullOrEmpty(impersonationSetting) || (impersonationSetting == "true"))
            {
                impersonationUserName = "ComServerExec";
                impersonationPassword = "@Microsoft";
            }
            else if (impersonationSetting == "false")
            {
                // No impersonation
            }
            else
            {
                if (impersonationSetting![0] == '#')
                    impersonationSetting = CryptoHelper.DecryptEx(impersonationSetting);
                var parts = impersonationSetting.Split(':');
                if (parts.Length == 2)
                {
                    impersonationUserName = parts[0];
                    impersonationPassword = parts[1];
                    if (impersonationUserName.Contains("\\"))
                    {
                        parts = impersonationUserName.Split('\\');
                        impersonationDomain = parts[0];
                        impersonationUserName = parts[1];
                    }
                    else
                    {
                    }
                }
            }

            if (impersonationUserName != null)
            {
                impersonator = new Impersonator(impersonationUserName, impersonationDomain, impersonationPassword ?? "");
                Log.Debug($"Impersonating {impersonationDomain}.{impersonationUserName}");
            }
        }
        catch (Exception ex)
        {
            Log.Debug("Impersonation failed -- this may not be fatal", ex);
        }

        var s = CreateSgaAsComServerExec(parameters);
        impersonator?.Dispose();
        return s;
    }

    private string CreateSgaAsComServerExec(ReportParameters parameters)
    {
        Log.Debug($"Running SGA as {Environment.UserName} {Environment.UserInteractive}");

        Application? excel = null;
        Workbook? wb = null;
        Workbooks? workbook = null;
        var excelUserFullPath = "";
        var rawFileName = "";
        var excelVisible = ConfigurationManager.AppSettings.GetForThisMachine("ExcelVisible", false);
        var excelAlerts = ConfigurationManager.AppSettings.GetForThisMachine("ExcelAlerts", false);

        try
        {
            excel = new Application
            {
                DisplayAlerts = excelAlerts,
                Visible = excelVisible
            };

            var userPath = parameters.OutputFolder;
            if (!userPath.EndsWith("\\"))
                userPath += "\\";
            if (!Directory.Exists(userPath))
                Directory.CreateDirectory(userPath);
            // Log.Info("userPath = '" + userPath + "'");
            rawFileName = GenerateDeliverableFileName(parameters, ".xlsm");

            var excelTemplateFullPath = GetFullPathOfTemplateFile("PeerAMid-SGA.xlsm");
            // Log.Info("excelTemplateFullPath = '" + excelTemplateFullPath + "'");
            var powerpointTemplateFullPath = GetFullPathOfTemplateFile("PeerAMid-SGA.pptx");
            // Log.Info("powerpointTemplateFullPath = '" + powerpointTemplateFullPath + "'");

            excelUserFullPath = userPath + rawFileName + ".xlsm";
            // Log.Info("excelTemplateFullPath = '" + excelTemplateFullPath + "'");
            var powerpointUserFullPath = userPath + rawFileName + ".pptx";
            // Log.Info("powerpointUserFullPath = '" + powerpointUserFullPath + "'");

            System.IO.File.Copy(excelTemplateFullPath, excelUserFullPath, true);
            Log.Debug("Copied '" + excelTemplateFullPath + "' to '" + excelUserFullPath + "'");
            System.IO.File.Copy(powerpointTemplateFullPath, powerpointUserFullPath, true);
            Log.Debug("Copied '" + powerpointTemplateFullPath + "' to '" + powerpointUserFullPath + "'");

            // Log.Info("OPENING Excel Workbooks Object");
            workbook = excel.Workbooks;
            Log.Info($"Opening Excel Workbook File: {excelUserFullPath}");
            Exception? exception = new Exception("File not copied");
            for (var i = 0; i < 40; ++i)
            {
                try
                {
                    if (System.IO.File.Exists(excelUserFullPath))
                    {
        	            wb = workbook.Open(
                            excelUserFullPath /*,
                            Missing.Value,
                            Missing.Value,
                            Missing.Value,
                            Missing.Value,
                            Missing.Value,
                            Missing.Value,
                            Missing.Value,
                            Missing.Value,
                            Missing.Value,
                            Missing.Value,
                            Missing.Value,
                            Missing.Value,
                            Missing.Value,
                            Missing.Value */);
                        exception = null;
                        break;
                    }
                    else
                    {
                        Thread.Sleep(250);
                    }
                }
                catch (Exception ex)
                {
                    exception = ex;
                    Thread.Sleep(250);
                }
            }

            if (exception != null)
                throw exception;

            Log.Info($"Opened Excel Workbook: {excelUserFullPath}");
            //Thread.Sleep(1000*10);
            // Log.Info("Making first COM call ...");
            wb!.DoNotPromptForConvert = false;
            // Log.Info($"Setting CheckCompatibility...");
            wb.CheckCompatibility = false;

            // Log.Info($"CheckCompatibility SET");
            var optionId = 1; // parameters.OptionId;
            Log.Info("setting optionid = " + optionId);

            var demographicModel = _homeCore.GetDemographicChartData(
                    parameters.SelectedTarget,
                    parameters.SelectedPeers,
                    parameters.Year,
                    optionId);
            var sGAmodel = _homeCore.GetSgaChartData(
                    parameters.SelectedTarget,
                    parameters.SelectedTargetSymbol,
                    parameters.SelectedPeers,
                    parameters.Year,
                    optionId);
            var sgaPerformancemodel = _homeCore.GetPerformanceChartData(
                    parameters.SelectedTarget,
                    parameters.SelectedPeers,
                    parameters.Year,
                    optionId);
            var funCostmodel = _homeCore.GetFunCostAsPercentOfRevenueData(
                    parameters.SelectedTarget,
                    parameters.SelectedPeers,
                    parameters.Year,
                    optionId);
            var ftEmodel = _homeCore.GetFTEDecomposedCostAsPercentOfRevenueData(
                    parameters.SelectedTarget,
                    parameters.SelectedPeers,
                    parameters.Year,
                    optionId);
            var waterFallTopModel = _homeCore.GetDecomposedWaterFallTopQ(
                    parameters.SelectedTarget,
                    parameters.SelectedPeers,
                    parameters.Year,
                    optionId);
            var waterFalMediumlModel = _homeCore.GetDecomposedWaterFallMedian(
                    parameters.SelectedTarget,
                    parameters.SelectedPeers,
                    parameters.Year,
                    optionId);
            var waterFalTopDecileModel = _homeCore.GetDecomposedWaterFallTopD(
                    parameters.SelectedTarget,
                    parameters.SelectedPeers,
                    parameters.Year,
                    optionId);
            var companyList = _homeCore.GetFinalPeerAndTargetCompaniesList(
                    parameters.SelectedTarget,
                    parameters.SelectedPeers,
                    parameters.Year);

            //foreach (var aaa in sGAmodel.ChartData.ProviderData)
            //    Log.Debug($"UPON RETURN {aaa.PeerCompanyName} : {aaa.Ranking}");

            // Log.Info($"Accessing Charts Data.....");
            var w = wb.Worksheets["Charts Data"];
            // Log.Info($"Charts Data ACCESSED");
            // Log.Info($"Modifying 'File Name' cell.....");
            w.Range["FileName"].Value = rawFileName;
            // Log.Info($"'File Name' cell populated.....");
            w.Range["IndustryGroup"] = parameters.Industry;
            w.Range["SubIndustryGroup"].Value = parameters.SubIndustry;
            w.Range["SubIndustryGroupName"].Value = parameters.SubIndustryName; // PEER-17
            var benchmarkCompanyName = parameters.BenchmarkCompanyName;
            var benchmarkCompanyDisplayName = parameters.BenchmarkCompanyDisplayName;
            w.Cells[4, 62].Value = benchmarkCompanyDisplayName;
            w.Range["ReportName"].Value = parameters.Service.GetReportTitle(); // PEER-2
            w.Range["CName"].Value = benchmarkCompanyDisplayName;
            w.Range["Revenue"].Value = parameters.Revenue;
            w.Range["AnalysisPeriod"].Value = "FY" + parameters.Year;
            WriteSheetDemographicDataSga(w, demographicModel);

            //foreach (var aaa in sGAmodel.ChartData.ProviderData)
            //    Log.Debug($"BEFORE DATATABLE RETURN {aaa.PeerCompanyName} : {aaa.Ranking}");

            var sgaModel0 = sGAmodel!.ChartData!.ProviderData.Select(
                    a => new
                    {
                        a.PeerCompanyName,
                        a.PeerCompanyValue,
                        a.Ranking,
                        a.IsTarget,
                        a.IsOutlier,
                        a.PeerCompanyDisplayName
                    })
                .OrderBy(a => a.PeerCompanyValue)
                .ToList();

            //foreach (var aa in sgaModel0)
            //    Log.Debug("BEFORE WRITESHEETDATA:" + $"{aa.PeerCompanyName} : {aa.Ranking}");

            WriteSheetData(w, ToDataTable(sgaModel0), 3, 0);
            WriteSheetSgaDiffTableData(w, sGAmodel.DifferenceTableData!);
            w.Cells[4, 11].Value = sgaModel0.Last().PeerCompanyValue / 100;
            w.Cells[4, 8].Value = sgaModel0.First().PeerCompanyValue / 100;
            WriteSheetCompanyTable(w, companyList);
            w.Cells[5, 59].Value = GetSgaMessageSummary(parameters, sGAmodel, out var cost1, out var cost2);

            if (cost2 == null && cost1 != null)
                w.Cells[1, 47].Value = cost1 + "MM";                                // w.Cells[1, 47].Value = "$" + String.Format("{0:n0}", Convert.ToDecimal(cost1)) +"MM";
            else if (cost2 != null)
                w.Cells[1, 47].Value = cost1 + "MM" + " to " + cost2 + "MM";        // w.Cells[1, 47].Value = "$" + String.Format("{0:n0}", Convert.ToDecimal(cost1)) + "MM " + " to " + "$" + String.Format("{0:n0}", Convert.ToDecimal(cost2)) + "MM";

            // In the spreadsheet, 'Short Name',  'SG&A CAGR - Percentile',   'EBITDA CAGR -Percentile' columns
            var sgaPerformanceModel = sgaPerformancemodel!.SGAPerformanceData.Select(
                    a => new
                    {
                        a.PeerCompanyDisplayName,
                        a.XValue,
                        a.YValue,
                        a.IsTarget
                    })
                .ToList();
            WriteSheetSgaPerformanceData(w, ToDataTable(sgaPerformanceModel), 3, 12, 4);



            w.Cells[6, 59].Value = GetSgaPerformanceMessage(parameters, sgaPerformancemodel);

            var funCost0 = funCostmodel!.FinanceData.Select(
                    a => new
                    {
                        a.PeerCompanyName,
                        PeerCompanyValue = a.PeerCompanyValue / 100,
                        a.PeerCompanyDisplayName
                    })
                .ToList();
            var funCost1 = funCostmodel.HRData.Select(
                    a => new
                    {
                        a.PeerCompanyName,
                        PeerCompanyValue = a.PeerCompanyValue / 100,
                        a.PeerCompanyDisplayName
                    })
                .ToList();
            var funCost2 = funCostmodel.ITData.Select(
                    a => new
                    {
                        a.PeerCompanyName,
                        PeerCompanyValue = a.PeerCompanyValue / 100,
                        a.PeerCompanyDisplayName
                    })
                .ToList();
            var funCost3 = funCostmodel.MarketData.Select(
                    a => new
                    {
                        a.PeerCompanyName,
                        PeerCompanyValue = a.PeerCompanyValue / 100,
                        a.PeerCompanyDisplayName
                    })
                .ToList();
            var funCost4 = funCostmodel.ProcurementData.Select(
                    a => new
                    {
                        a.PeerCompanyName,
                        PeerCompanyValue = a.PeerCompanyValue / 100,
                        a.PeerCompanyDisplayName
                    })
                .ToList();
            var funCost5 = funCostmodel.SalesData.Select(
                    a => new
                    {
                        a.PeerCompanyName,
                        PeerCompanyValue = a.PeerCompanyValue / 100,
                        a.PeerCompanyDisplayName
                    })
                .ToList();
            var funCost6 = funCostmodel.CustServData.Select(
                    a => new
                    {
                        a.PeerCompanyName,
                        PeerCompanyValue = a.PeerCompanyValue / 100,
                        a.PeerCompanyDisplayName
                    })
                .ToList();
            var funCost7 = funCostmodel.CSSupportServData.Select(
                    a => new
                    {
                        a.PeerCompanyName,
                        PeerCompanyValue = a.PeerCompanyValue / 100,
                        a.PeerCompanyDisplayName
                    })
                .ToList();
            // Log.Info("Launching write process for FUNCTIONAL Cost Data..... ");
            /*                    NotParallel.Invoke(
                                        () => writeSheetFunCostData(w, ToDataTable(FunCost0), 3), () => writeSheetFunCostData(w, ToDataTable(FunCost1), 4),
                                        () => writeSheetFunCostData(w, ToDataTable(FunCost2), 5), () => writeSheetFunCostData(w, ToDataTable(FunCost3), 6),
                                        () => writeSheetFunCostData(w, ToDataTable(FunCost4), 7), () => writeSheetFunCostData(w, ToDataTable(FunCost5), 8),
                                        () => writeSheetFunCostData(w, ToDataTable(FunCost6), 9), () => writeSheetFunCostData(w, ToDataTable(FunCost7), 10)
                                    );
            */
            WriteSheetFunCostData(w, ToDataTable(funCost0), 3);
            WriteSheetFunCostData(w, ToDataTable(funCost1), 4);
            WriteSheetFunCostData(w, ToDataTable(funCost2), 5);
            WriteSheetFunCostData(w, ToDataTable(funCost3), 6);
            WriteSheetFunCostData(w, ToDataTable(funCost4), 7);
            WriteSheetFunCostData(w, ToDataTable(funCost5), 8);
            WriteSheetFunCostData(w, ToDataTable(funCost6), 9);
            WriteSheetFunCostData(w, ToDataTable(funCost7), 10);
            // Log.Info("COMPLETED write process for FUNCTIONAL Cost Data");

            var fteCost0 = ftEmodel!.FinanceData.Select(
                    a => new
                    {
                        a.PeerCompanyName,
                        a.PeerCompanyValue,
                        a.PeerCompanyDisplayName
                    })
                .ToList();
            var fteCost1 = ftEmodel.HRData.Select(
                    a => new
                    {
                        a.PeerCompanyName,
                        a.PeerCompanyValue,
                        a.PeerCompanyDisplayName
                    })
                .ToList();
            var fteCost2 = ftEmodel.ITData.Select(
                    a => new
                    {
                        a.PeerCompanyName,
                        a.PeerCompanyValue,
                        a.PeerCompanyDisplayName
                    })
                .ToList();
            var fteCost3 = ftEmodel.MarketData.Select(
                    a => new
                    {
                        a.PeerCompanyName,
                        a.PeerCompanyValue,
                        a.PeerCompanyDisplayName
                    })
                .ToList();
            var fteCost4 = ftEmodel.ProcurementData.Select(
                    a => new
                    {
                        a.PeerCompanyName,
                        a.PeerCompanyValue,
                        a.PeerCompanyDisplayName
                    })
                .ToList();
            var fteCost5 = ftEmodel.SalesData.Select(
                    a => new
                    {
                        a.PeerCompanyName,
                        a.PeerCompanyValue,
                        a.PeerCompanyDisplayName
                    })
                .ToList();
            var fteCost6 = ftEmodel.CustServData.Select(
                    a => new
                    {
                        a.PeerCompanyName,
                        a.PeerCompanyValue,
                        a.PeerCompanyDisplayName
                    })
                .ToList();
            var fteCost7 = ftEmodel.CSSupportServData.Select(
                    a => new
                    {
                        a.PeerCompanyName,
                        a.PeerCompanyValue,
                        a.PeerCompanyDisplayName
                    })
                .ToList();
            // Log.Info("Launching write process for FTE Cost Data.... ");
            /*                    NotParallel.Invoke(
                                        () => writeSheetFunCostData(w, ToDataTable(FTECost0), 13), () => writeSheetFunCostData(w, ToDataTable(FTECost1), 14),
                                        () => writeSheetFunCostData(w, ToDataTable(FTECost2), 15), () => writeSheetFunCostData(w, ToDataTable(FTECost3), 16),
                                        () => writeSheetFunCostData(w, ToDataTable(FTECost4), 17), () => writeSheetFunCostData(w, ToDataTable(FTECost5), 18),
                                        () => writeSheetFunCostData(w, ToDataTable(FTECost6), 19), () => writeSheetFunCostData(w, ToDataTable(FTECost7), 20)
                                    );*/
            WriteSheetFunCostData(w, ToDataTable(fteCost0), 13);
            WriteSheetFunCostData(w, ToDataTable(fteCost1), 14);
            WriteSheetFunCostData(w, ToDataTable(fteCost2), 15);
            WriteSheetFunCostData(w, ToDataTable(fteCost3), 16);
            WriteSheetFunCostData(w, ToDataTable(fteCost4), 17);
            WriteSheetFunCostData(w, ToDataTable(fteCost5), 18);
            WriteSheetFunCostData(w, ToDataTable(fteCost6), 19);
            WriteSheetFunCostData(w, ToDataTable(fteCost7), 20);
            // Log.Info("COMPLETED write process for FTE Cost Data");

            w.Cells[7, 59].Value = ftEmodel.FTESummaryLine!.Replace("##CompanyName##", benchmarkCompanyName);
            var str = GetFteMessage(parameters, waterFallTopModel!, waterFalMediumlModel!);
            if (!string.IsNullOrEmpty(str)) w.Cells[7, 59].Value = str;

            WriteSheetWaterFallTopQ(w, waterFallTopModel, 25);
            WriteSheetWaterFallTopQ(w, waterFalMediumlModel, 24);
            WriteSheetWaterFallTopQ(w, waterFalTopDecileModel, 66);
            w.Cells[8, 59].Value = GetWaterFallMessage(
                waterFallTopModel!,
                waterFalMediumlModel!,
                waterFalTopDecileModel!,
                out var waterfallFlag);
            w.Cells[2, 82].Value = waterfallFlag;

            var profileChartMessage =
                "Optimizing SG&A spend can liberate capital for more strategic leverage within operations or drive savings to the bottom line. As per reported 10-K, " +
                benchmarkCompanyName +
                " SG&A cost as a % of revenue is " +
                sGAmodel.DifferenceTableData!.CostTarget +
                "%";
            profileChartMessage += $" and their industry is {parameters.SubIndustryName}."; // PEER-17
            w.Range["SGAIndustryGroupProfileMessage"].Value = profileChartMessage;

            WriteSheetCurrencyInfo(w, parameters.Currency, parameters.ExchangeRate);

            try
            {
                Marshal.ReleaseComObject(w);
            }
            catch // (Exception ex)
            {
                // Log.Warn("Releasing worksheet", ex);
            }

            var lockWasTaken = false;
            var temp = Obj;

            // Filter output if the SG&A Diagnostic wasn't selected (PEER-2)
            var isFilteredOutput = parameters.Service == PeerAMidService.SgaShort;
            try
            {
                wb.Save();
                //Threading logic: Use Enter to acquire the Monitor on the object passed as the parameter. If another thread has executed an Enter on the object but has not yet executed the corresponding Exit, the current thread will block until the other thread releases the object.
                Monitor.Enter(temp, ref lockWasTaken);

                // Log.Info("Running 'FormatCharts' macro");
                wb.Application.Run(
                    "FormatCharts",
                    isFilteredOutput,
                    Missing.Value,
                    Missing.Value,
                    Missing.Value,
                    Missing.Value,
                    Missing.Value,
                    Missing.Value,
                    Missing.Value,
                    Missing.Value,
                    Missing.Value,
                    Missing.Value,
                    Missing.Value,
                    Missing.Value,
                    Missing.Value,
                    Missing.Value,
                    Missing.Value,
                    Missing.Value,
                    Missing.Value,
                    Missing.Value,
                    Missing.Value,
                    Missing.Value,
                    Missing.Value,
                    Missing.Value,
                    Missing.Value,
                    Missing.Value,
                    Missing.Value,
                    Missing.Value,
                    Missing.Value,
                    Missing.Value,
                    Missing.Value);
                Log.Info("Back from 'FormatCharts' macro");
                wb.Save();
            }
            catch (Exception ex)
            {
                Log.Warn("COM or Locking exception", ex.ToString());
            }
            finally
            {
                if (lockWasTaken) Monitor.Exit(temp);
            }

            wb.Close();

            _iPeerAMid.SaveRunAnalysis(rawFileName, parameters.SelectedPeers);
            Log.Debug("Done with " + rawFileName);

            /*
            try
            {
                var a = System.IO.File.GetAttributes(excelUserFullPath);
                a |= FileAttributes.ReadOnly;
                System.IO.File.SetAttributes(excelUserFullPath, a);
            }
            catch
            {
            }
            */
        }
        catch (Exception ex)
        {
            Log.Error(ex);
            rawFileName = string.Empty;
        }
        finally
        {
            if (!string.IsNullOrEmpty(excelUserFullPath))
            {
                var excelLog = Path.Combine(Path.GetDirectoryName(excelUserFullPath), Path.GetFileNameWithoutExtension(excelUserFullPath) + ".txt");
                Log.DebugImportFile("EXCEL ", excelLog);
            }

            try
            {
                ReleaseComObjects(wb, workbook, excel);
                // Log.Info("Excel cleaned up");
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }

        return rawFileName;
    }

    private string GetFullPathOfTemplateFile(string filename)
    {
        var serverFilename = Path.Combine(SessionData.TemplatePath, filename);
        var path = Path.GetFullPath(serverFilename);
        // Log.Debug($"Mapped '{filename}' to '{path}'");
        return path;
    }

    /// <summary>
    ///     According business requirement write the message in PPT under WaterFall chart.
    /// </summary>
    /// <param name="waterFallTopModel"></param>
    /// <param name="waterFalMediumlModel"></param>
    /// <param name="waterFallTopDecileModel"></param>
    /// <param name="waterfallFlag"></param>
    /// <returns></returns>
    private string GetWaterFallMessage(SGAWaterfallModel waterFallTopModel, SGAWaterfallModel waterFalMediumlModel,
        SGAWaterfallModel waterFallTopDecileModel, out int waterfallFlag)
    {
        waterfallFlag = 0;
        var waterfallSummary = string.Empty;
        if (waterFallTopModel != null && waterFalMediumlModel != null)
        {
            var topQValue = Convert.ToDouble(waterFallTopModel.WaterfallChartItemList[8].DepartmentValue);
            var medianValue = Convert.ToDouble(waterFalMediumlModel.WaterfallChartItemList[8].DepartmentValue);
            var topDValue = Convert.ToDouble(waterFallTopDecileModel.WaterfallChartItemList[8].DepartmentValue);
            //if (topQValue == 0 && medianValue == 0)
            //{
            //    // case 3
            //    //w.Cells[7, 59]="As far as decomposed SG&A expenses are concerned, " + SessionData.BenchmarkCompanyName + " is a best in class performer";
            //    var departmentNames = GetDepartmentTextUpTo80Percent(waterFallTopDecileModel.WaterfallChartItemList, topDValue);
            //    WaterfallSummary = "The SG&A cost reduction opportunity waterfall indicates that " + SessionData.BenchmarkCompanyName + " is performing better than median and top quartile, the top decile SG&A opportunity is pegged at $" + topDValue + "MM, the 80% of the opportunity comes from " + departmentNames + '.';
            //    waterfallFlag = 1;
            //}
            //else if (topQValue > 0 && medianValue == 0)
            //{
            //    // case 1
            //    //$('#divFunctionalCostSummary').html('As far as decomposed SG&A expenses are concerned, ' + $('#headerBenchmarkCompany').html() + ' is performing better than median, but when compared with top quartile performers, they do have significant SG&A cost improvement opportunity to the tune of $' + topQValue + 'MM');
            //    var departmentNames = GetDepartmentTextUpTo80Percent(waterFallTopModel.WaterfallChartItemList, topQValue);
            //    WaterfallSummary = "The SG&A cost reduction opportunity waterfall indicates the overall opportunity of $" + topDValue + "MM, the 80% of the opportunity comes from " + departmentNames + ".";
            //    waterfallFlag = 2;
            //}
            //else if (topQValue > 0 && medianValue > 0)
            //{
            //    // case 2
            //    //$('#divFunctionalCostSummary').html('As far as decomposed SG&A expenses are concerned, ' + $('#headerBenchmarkCompany').html() + ' is performing worse than median, and they do have significant SG&A cost improvement opportunity when compared with median to top quartile performers, the SG&A opportunity could range between $' + medianValue + 'MM to $' + topQValue + 'MM');
            //    var departmentNames = GetDepartmentTextUpTo80Percent(waterFallTopModel.WaterfallChartItemList, topQValue);
            //    WaterfallSummary = "The SG&A cost reduction opportunity waterfall indicates the overall opportunity that ranges from $" + medianValue + "MM to $" + topQValue + "MM, the 80% of the opportunity comes from " + departmentNames + ".";
            //    waterfallFlag = 3;
            //}
            if (medianValue == 0 && topQValue == 0 && topDValue == 0)
            {
                waterfallSummary = SessionData.BenchmarkCompany.Name +
                                   " is a best in class performer.";
                waterfallFlag = 0;
            }
            else if (medianValue == 0 && topQValue == 0 && topDValue > 0)
            {
                var departmentNames = GetDepartmentTextUpTo80Percent(
                    waterFallTopDecileModel.WaterfallChartItemList,
                    topDValue);
                waterfallSummary =
                    "The SG&A cost reduction opportunity waterfall indicates the overall opportunity is pegged at $" +
                    $"{Math.Round(topDValue):n0}" +
                    "MM, with 80 % of the opportunity coming from " +
                    departmentNames +
                    ".";
                waterfallFlag = 1;
            }
            else if (medianValue == 0 && topQValue > 0 && topDValue > 0)
            {
                var departmentNames = GetDepartmentTextUpTo80Percent(
                    waterFallTopModel.WaterfallChartItemList,
                    topQValue);
                waterfallSummary =
                    "The SG&A cost reduction opportunity waterfall indicates the overall opportunity ranges from $" +
                    $"{Math.Round(topQValue):n0}" +
                    "MM to $" +
                    $"{Math.Round(topDValue):n0}" +
                    "MM, with 80% of the opportunity coming from " +
                    departmentNames +
                    ".";
                waterfallFlag = 2;
            }
            else if (medianValue > 0 && topQValue > 0 && topDValue > 0)
            {
                var departmentNames = GetDepartmentTextUpTo80Percent(
                    waterFallTopModel.WaterfallChartItemList,
                    topQValue);
                waterfallSummary =
                    "The SG&A cost reduction opportunity waterfall indicates the overall opportunity ranges from $" +
                    $"{Math.Round(medianValue):n0}" +
                    "MM to $" +
                    $"{Math.Round(topQValue):n0}" +
                    "MM, with 80% of the opportunity coming from " +
                    departmentNames +
                    ".";
                waterfallFlag = 3;
            }
        }

        return waterfallSummary;
    }

    /// <summary>
    ///     According business requirement write the message in PPT.
    /// </summary>
    /// <param name="parameters"></param>
    /// <param name="waterFallTopModel"></param>
    /// <param name="waterFalMediumlModel"></param>
    /// <returns></returns>
    private string GetFteMessage(ReportParameters parameters, SGAWaterfallModel waterFallTopModel,
        SGAWaterfallModel waterFalMediumlModel)
    {
        var fteSummary = string.Empty;
        if (waterFallTopModel != null && waterFalMediumlModel != null)
        {
            var topQValue = Convert.ToDouble(waterFallTopModel.WaterfallChartItemList[8].DepartmentValue);
            var medianValue = Convert.ToDouble(waterFalMediumlModel.WaterfallChartItemList[8].DepartmentValue);
            //var textInfo = new CultureInfo("en-US", false).TextInfo;
            var selectedTargetName = parameters.BenchmarkCompanyDisplayName;
            if (topQValue == 0 && medianValue == 0)
            {
                // case 3
                fteSummary = "As far as decomposed SG&A expenses are concerned, " +
                             selectedTargetName +
                             " is a best in class performer.";
            }
            else if (topQValue > 0 && medianValue == 0)
            {
                // case 1
                fteSummary = selectedTargetName + "'s decomposed SG&A departmental costs are generally within the 2nd quartile of their peer group, and ahead of the median.  However, there remains a " +
                             string.Format("{0:n0}", Math.Round(topQValue)) + "MM opportunity gap to the top quartile performance level.";
            }
            //var departmentNames = GetDepartmentTextUpTo80Percent(waterFallTopModel.WaterfallChartItemList, topQValue);
            //FTESummary = "The SG&A cost reduction opportunity waterfall indicates the overall opportunity of $" + topQValue + "MM, the 80% of the opportunity comes from " + departmentNames + ".";
            else if (topQValue > 0 && medianValue > 0)
            {
                // case 2
                fteSummary = selectedTargetName + "'s decomposed SG&A departmental costs are generally below the median, " +
                             "and they do have significant SG&A cost improvement opportunity when compared with Median to Top Quartile performers.  The SG&A opportunity could range between $" +
                             string.Format("{0:n0}", Math.Round(medianValue)) +
                             "MM to $" +
                             string.Format("{0:n0}", Math.Round(topQValue)) +
                             "MM.";
            }
            //var departmentNames = GetDepartmentTextUpTo80Percent(waterFallTopModel.WaterfallChartItemList, topQValue);
            //FTESummary = "The SG&A cost reduction opportunity waterfall indicates the overall opportunity that ranges from $" + medianValue + "MM to $" + topQValue + "MM, the 80% of the opportunity comes from " + departmentNames + ".";
        }

        return fteSummary;
    }

    /// <summary>
    ///     Calculating percent value(80% of the opportunity coming) for pointing chart.
    /// </summary>
    /// <param name="chartData"></param>
    /// <param name="numForPercent"></param>
    /// <returns></returns>
    public string GetDepartmentTextUpTo80Percent(List<SGAWaterfallItemModel> chartData, double total)
    {
        return GetDepartmentTextUpTo(chartData, total * 0.8, out _);
    }


    /// <summary>
    ///     Calculating percent value(80% of the opportunity coming) for pointing chart.
    /// </summary>
    /// <param name="chartData"></param>
    /// <param name="numForPercent"></param>
    /// <returns></returns>
    public string GetDepartmentTextUpTo(List<SGAWaterfallItemModel> chartData, double target, out double actualSum)
    {
        var departments = new List<string>();
        var sum = 0.0;

        foreach (var item in chartData)
        {
            departments.Add(item.DepartmentName!);
            sum += Convert.ToDouble(item.DepartmentValue!);
            if (sum >= target)
                break;
        }

        actualSum = sum;
        return StringExtensionMethods.OxfordComma(departments);
    }

    /// <summary>
    ///     According business requirement write the message in PPT under SGAPerformance chart.
    /// </summary>
    /// <param name="parameters"></param>
    /// <param name="sgaPerformanceModel"></param>
    /// <returns></returns>
    private string GetSgaPerformanceMessage(ReportParameters parameters, SGAPerformanceModel sgaPerformanceModel)
    {
        var bestInClassPerformers = "";
        decimal targetXValue = 0;
        decimal targetYValue = 0;
        var companyNames = new ArrayList();

        foreach (var item in sgaPerformanceModel.SGAPerformanceData)
        {
            if (item.IsTarget)
            {
                targetXValue = item.XValue;
                targetYValue = item.YValue;
            }
            else if (Convert.ToDouble(item.XValue) >= 0.75 && Convert.ToDouble(item.YValue) >= 0.75)
            {
                companyNames.Add(item.PeerCompanyDisplayName);
            }
        }

        if (companyNames.Count == 1)
        {
            bestInClassPerformers = ", " + companyNames[0] + " is best in class player";
        }
        else if (companyNames.Count > 1)
        {
            var isFirstCall = true;
            var counter = 0;
            foreach (var companyName in companyNames)
            {
                counter++;

                if (isFirstCall)
                {
                    isFirstCall = false;
                    bestInClassPerformers = ", " + companyName;
                }
                else
                {
                    if (companyNames.Count == counter)
                    {
                        bestInClassPerformers =
                            bestInClassPerformers + " and " + companyName + " are best in class players";
                    }
                    else
                    {
                        bestInClassPerformers = bestInClassPerformers + ", " + companyName;
                    }
                }
            }
        }

        var targetCompany = parameters.BenchmarkCompanyDisplayName;
        var targetXValueDouble = Convert.ToDouble(targetXValue);
        var targetYValueDouble = Convert.ToDouble(targetYValue);
        var performanceChartSummaryLine = "Optimizing SG&A spend can liberate capital for more strategic leverage within operations or drive savings to the bottom line. The top right quadrant represents top performers, " +
                                          "and the bottom left quadrant represents companies performing below the Median.  ";

        var left = targetXValueDouble <= 0.5;
        var bottom = targetYValueDouble <= 0.5;

        if (targetXValueDouble >= 0.75 && targetYValueDouble >= 0.75)
        {
            performanceChartSummaryLine += targetCompany + " has best in class overall performance.";
        }
        else if (left)
        {
            if (!bottom)
                performanceChartSummaryLine += targetCompany + "'s SG&A performance is within top quartile, however EBITDA is below Median keeping them from being a top performer overall";
            else
                performanceChartSummaryLine += targetCompany + " has near Median overall performance. They have significant opportunity for improvement as compared to best in class performers.";
        }
        else
        {
            performanceChartSummaryLine += targetCompany + " has above Median overall performance, but they do have opportunity for improvement as compared to best in class performers.";
        }

        return performanceChartSummaryLine;
    }

    /*
    /// <summary>
    ///     Remove temporary files from server (PEER-20)
    /// </summary>
    /// <param name="filePaths">A list containing the full paths of files to remove</param>
    private void RemoveFiles(List<string> filePaths)
    {
        // Loop through file name list and and delete each file. Log any exceptions that
        // may occur, but continue on to the next file.
        foreach (var filePath in filePaths)
        {
            try
            {
                // Delete file
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
            }
            catch (IOException io)
            {
                Log.Warn($"Could not delete the file [{filePath}] due to IOException\r\n[{PagePath}]", io);
            }
        }
    }
    */

    /// <summary>
    ///     Write business data in Excel sheet for WaterFall chart and sheet (Macro) will ran and create PPT.
    /// </summary>
    /// <param name="w"></param>
    /// <param name="waterFallModel"></param>
    /// <param name="column"></param>
    private void WriteSheetWaterFallTopQ(_Worksheet w, SGAWaterfallModel waterFallModel, int column)
    {
        var model = waterFallModel.WaterfallChartItemList;
        foreach (var item in model)
        {
            switch (item.DepartmentName)
            {
                case "Finance":
                    w.Cells[3, column].Value = item.DepartmentValue;
                    break;

                case "HR":
                    w.Cells[4, column].Value = item.DepartmentValue;
                    break;

                case "IT":
                    w.Cells[5, column].Value = item.DepartmentValue;
                    break;

                case "Marketing":
                    w.Cells[6, column].Value = item.DepartmentValue;
                    break;

                case "Procurement":
                    w.Cells[7, column].Value = item.DepartmentValue;
                    break;

                case "Sales":
                    w.Cells[8, column].Value = item.DepartmentValue;
                    break;

                case "Customer Service":
                    w.Cells[9, column].Value = item.DepartmentValue;
                    break;

                case "Corporate Support":
                    w.Cells[10, column].Value = item.DepartmentValue;
                    break;
            }
        }
    }

    /// <summary>
    ///     Write data in excel sheet.
    /// </summary>
    /// <param name="w"></param>
    /// <param name="sgaDifferenceTableData"></param>
    private void WriteSheetSgaDiffTableData(_Worksheet w, DifferenceTableModel<decimal> sgaDifferenceTableData)
    {
        w.Cells[3, 5].Value = sgaDifferenceTableData.CostTopQuartile / 100;
        w.Cells[3, 6].Value = sgaDifferenceTableData.CostMedian / 100;
        w.Cells[3, 7].Value = sgaDifferenceTableData.CostBottomQuartile / 100;
        w.Cells[4, 8].Value = sgaDifferenceTableData.CostTopDecile / 100;
        w.Cells[4, 11].Value = sgaDifferenceTableData.CostBottomDecile / 100;
        w.Cells[25, 1].Value = sgaDifferenceTableData.OutliersDescription;
    }

    /// <summary>
    ///     Write data in excel sheet.
    /// </summary>
    /// <param name="workSheet"></param>
    /// <param name="companyModel"></param>
    private void WriteSheetCompanyTable(_Worksheet workSheet, List<Company> companyModel)
    {
        var counter = 3;
        // var textInfo = new CultureInfo("en-US", false).TextInfo;
        // var BenchmarkCompanyName = textInfo.ToTitleCase(parameters.BenchmarkCompanyName.ToLower());

        foreach (var item in companyModel)
        {
            workSheet.Cells[counter, 76].Value = item.DisplayName;
            workSheet.Cells[counter, 77].Value = "'" + item.SubIndustryId;
            workSheet.Cells[counter, 78].Value = item.GetSubIndustryName();
            counter++;
        }

        counter = 31;
        foreach (var item in companyModel)
        {
            var column = 76;
            workSheet.Cells[counter, column++].Value = item.DisplayName;
            workSheet.Cells[counter, column++].Value = "'" + item.SicCode;
            workSheet.Cells[counter, column++].Value = item.GetSubIndustryName()!.Substring(5);
            workSheet.Cells[counter, column++].Value = item.Country;
            workSheet.Cells[counter, column++].Value = "'" + item.DataYear;
            var rev = Math.Round(item.Revenue, 0);
            workSheet.Cells[counter, column++].Value = $"'{rev:n0}";
            ++counter;
        }
    }

    /// <summary>
    ///     Write data in excel sheet, that use for write data in ppt after ran macro.
    /// </summary>
    /// <param name="ws"></param>
    /// <param name="dt"></param>
    /// <param name="row"></param>
    private void WriteSheetFunCostData(_Worksheet ws, DataTable dt, int row)
    {
        System.Data.DataRow[] row1 = dt.Select("PeerCompanyName='Target'");
        System.Data.DataRow[] row2 = dt.Select("PeerCompanyName='Median'");
        System.Data.DataRow[] row3 = dt.Select("PeerCompanyName='Top Quartile'");
        ws.Cells[row, 19].Value = row1[0]["PeerCompanyValue"];
        ws.Cells[row, 20].Value = row2[0]["PeerCompanyValue"];
        ws.Cells[row, 21].Value = row3[0]["PeerCompanyValue"];
    }

    /// <summary>
    ///     Convert data in table format, which is coming from DB.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="items"></param>
    /// <returns></returns>
    public DataTable ToDataTable<T>(List<T> items)
    {
        var dataTable = new DataTable(typeof(T).Name);
        //Get all the properties by using reflection
        var props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
        foreach (var prop in props)
            //Setting column names as Property names
            dataTable.Columns.Add(prop.Name);
        // Log.Info(prop.Name);
        foreach (var item in items)
        {
            object[] values = new object[props.Length];
            for (var i = 0; i < props.Length; i++) values[i] = props[i].GetValue(item, null);
            dataTable.Rows.Add(values);
        }

        return dataTable;
    }

    /// <summary>
    ///     Writing data in excel sheet.
    /// </summary>
    /// <param name="ws"></param>
    /// <param name="dt"></param>
    /// <param name="drawRow"></param>
    /// <param name="drawColumn"></param>
    private void WriteSheetData(_Worksheet ws, DataTable dt, int drawRow, int drawColumn)
    {
        // a.PeerCompanyName, a.PeerCompanyValue, a.Ranking, a.IsTarget
        //var sgaTargetCompanyColor = ConfigurationManager.AppSettings["sgaTargetCompanyColor"];
        //var sgaMedianColor = ConfigurationManager.AppSettings["sgaMedianColor"];
        for (var row = 0; row < dt.Rows.Count; row++)
        {
            ws.Cells[drawRow + row, drawColumn + 1].Value = dt.Rows[row][5];
            ws.Cells[drawRow + row, drawColumn + 2].Value = dt.Rows[row][1] + "%";
            var isTarget = Convert.ToString(dt.Rows[row][3]);
            var isOutlier = Convert.ToString(dt.Rows[row][4]);
            var ranking = Convert.ToString(dt.Rows[row][2]);

            // Log.Debug("row " + row + ": " + "isTarget " + isTarget + " ranking " + ranking);
            // Log.Debug("name " + dt.Rows[row][0] + " isTarget " + isTarget + " isOutlier " + isOutlier);
            if (isOutlier == "True")
                ws.Cells[drawRow + row, 3].Value = "Outlier";
            else
                ws.Cells[drawRow + row, 3].Value = ranking + (isTarget == "True" ? "Target" : "");

            /*
            if (isTarget == "true")
            {
                if (Convert.ToString(dt.Rows[row]["ColorFirst"]) == sgaMedianColor)
                {
                    ws.Cells[drawRow + row, 3].Value = "MedianTarget";
                }
                else
                {
                    ws.Cells[drawRow + row, 3].Value = "Target";
                }
            }
            else if (ranking == sGADifferenceTableData.CostTopQuartile)
            {
                ws.Cells[drawRow + row, 3].Value = "TopQ";
            }
            else if (Convert.ToString(dt.Rows[row]["Color"]) == sgaMedianColor)
            {
                ws.Cells[drawRow + row, 3].Value = "Median";
            }
            */
            /*
             for (int column = 0; column < dt.Columns.Count; column++)
             {
                 if (column == 0 || column == 1)
                 {
                     ws.Cells[drawRow + row, drawColumn + column + 1].Value = (column == 1) ? dt.Rows[row][column] + "%" : SplitInTwoWord(Convert.ToString(dt.Rows[row][column]));
                 }
                 if (Convert.ToString(dt.Rows[row]["IsTarget"]) == "true")
                 {
                     if (Convert.ToString(dt.Rows[row]["ColorFirst"]) == sgaMedianColor)
                     {
                         ws.Cells[drawRow + row, 3].Value = "MedianTarget";
                     }
                     else
                     {
                         ws.Cells[drawRow + row, 3].Value = "Target";
                     }
                 }
                 else if (Convert.ToDecimal(dt.Rows[row]["PeerCompanyValue"]) == sGADifferenceTableData.CostTopQuartile)
                 {
                     ws.Cells[drawRow + row, 3].Value = "TopQ";
                 }
                 else if (Convert.ToString(dt.Rows[row]["Color"]) == sgaMedianColor)
                 {
                     ws.Cells[drawRow + row, 3].Value = "Median";
                 }
             }
             */
        }
    }

    /*
    /// <summary>
    ///     Split data(Company) name.
    /// </summary>
    /// <param name="word"></param>
    /// <returns></returns>
    private string SplitInTwoWord(string word)
    {
        var text = "";
        var split = word.Split(' ');
        if (split.Length == 1)
        {
            text = split[0];
        }
        else if (split.Length > 1)
        {
            text = split[0] + ' ' + split[1].Replace(",", "");
        }
        return text;
    }
    */

    /// <summary>
    ///     Write SGAPerformence chart data in Excel sheet.
    /// </summary>
    /// <param name="ws"></param>
    /// <param name="dt"></param>
    /// <param name="drawRow"></param>
    /// <param name="drawColumn"></param>
    /// <param name="columnCount"></param>
    private void WriteSheetSgaPerformanceData(_Worksheet ws, DataTable dt, int drawRow, int drawColumn, int columnCount = -1)
    {
        if (columnCount < 0)
            columnCount = dt.Columns.Count;

        for (var row = 0; row < dt.Rows.Count; row++)
        {
            for (var column = 0; column < columnCount; column++)
                ws.Cells[drawRow + row, drawColumn + column + 1].Value = dt.Rows[row][column];
        }
    }

    /// <summary>
    ///     Writing data in excel sheet.
    /// </summary>
    /// <param name="ws"></param>
    /// <param name="model"></param>
    private void WriteSheetDemographicDataSga(_Worksheet ws, DemographicModel model)
    {
        var rd = model.RevenueData!;
        rd.Minimum = Math.Round(rd.Minimum, MidpointRounding.AwayFromZero);
        rd.Percentile25 = Math.Round(rd!.Percentile25, MidpointRounding.AwayFromZero);
        rd.Percentile50 = Math.Round(rd!.Percentile50, MidpointRounding.AwayFromZero);
        rd.Percentile75 = Math.Round(rd!.Percentile75, MidpointRounding.AwayFromZero);
        rd.Maximum = Math.Round(rd!.Maximum, MidpointRounding.AwayFromZero);
        rd.StarValue = Math.Round(rd!.StarValue);
        ws.Cells[4, 47].Value = rd!.Minimum;
        ws.Cells[4, 48].Value = rd!.Percentile25;
        ws.Cells[4, 49].Value = rd.Percentile50;
        ws.Cells[4, 50].Value = rd.Percentile75;
        ws.Cells[4, 51].Value = rd.Maximum;
        ws.Cells[4, 52].Value = rd.StarValue;
        var startlocationPercent1 = DemographicStarLocation(
            rd.Minimum,
            rd.Percentile25,
            rd.Percentile50,
            rd.Percentile75,
            rd.Maximum,
            rd.StarValue);

        ws.Cells[4, 53].Value = Convert.ToString(startlocationPercent1) + "%";

        var cagr = model.CAGRData!;
        ws.Cells[5, 47].Value = cagr.IsCAGRDiv0 ? "NA" : Convert.ToString(cagr.Minimum) + "%";
        ws.Cells[5, 48].Value = cagr.IsCAGRDiv0 ? "NA" : Convert.ToString(cagr.Percentile25) + "%";
        ws.Cells[5, 49].Value = cagr.IsCAGRDiv0 ? "NA" : Convert.ToString(cagr.Percentile50) + "%";
        ws.Cells[5, 50].Value = cagr.IsCAGRDiv0 ? "NA" : Convert.ToString(cagr.Percentile75) + "%";
        ws.Cells[5, 51].Value = cagr.IsCAGRDiv0 ? "NA" : Convert.ToString(cagr.Maximum) + "%";
        ws.Cells[5, 52].Value = Convert.ToString(cagr.StarValue) + "%";
        var startlocationPercent2 = DemographicStarLocation(
            cagr.Minimum,
            cagr.Percentile25,
            cagr.Percentile50,
            cagr.Percentile75,
            cagr.Maximum,
            cagr.StarValue);

        ws.Cells[5, 53].Value = Convert.ToString(startlocationPercent2) + "%";
        if (cagr.IsCAGRDiv0)
        {
            ws.Cells[5, 52].Value = "CAGR is not available for the selected client";
            ws.Cells[5, 53].Value = "NA";
        }

        var gmr = model.GrossMarginData!;
        ws.Cells[6, 47].Value = Convert.ToString(gmr.Minimum) + "%";
        ws.Cells[6, 48].Value = Convert.ToString(gmr.Percentile25) + "%";
        ws.Cells[6, 49].Value = Convert.ToString(gmr.Percentile50) + "%";
        ws.Cells[6, 50].Value = Convert.ToString(gmr.Percentile75) + "%";
        ws.Cells[6, 51].Value = Convert.ToString(gmr.Maximum) + "%";
        ws.Cells[6, 52].Value = gmr.StarValue + "%";

        var startlocationPercent3 = DemographicStarLocation(
            gmr.Minimum,
            gmr.Percentile25,
            gmr.Percentile50,
            gmr.Percentile75,
            gmr.Maximum,
            gmr.StarValue);

        ws.Cells[6, 53].Value = Convert.ToString(startlocationPercent3) + "%";

        var ed = model.EBITDAData!;
        ws.Cells[7, 47].Value = Convert.ToString(ed.Minimum) + "%";
        ws.Cells[7, 48].Value = Convert.ToString(ed.Percentile25) + "%";
        ws.Cells[7, 49].Value = Convert.ToString(ed.Percentile50) + "%";
        ws.Cells[7, 50].Value = Convert.ToString(ed.Percentile75) + "%";
        ws.Cells[7, 51].Value = Convert.ToString(ed.Maximum) + "%";
        ws.Cells[7, 52].Value = Convert.ToString(ed.StarValue) + "%";

        var startlocationPercent4 = DemographicStarLocation(
            ed.Minimum,
            ed.Percentile25,
            ed.Percentile50,
            ed.Percentile75,
            ed.Maximum,
            ed.StarValue);

        ws.Cells[7, 53].Value = Convert.ToString(startlocationPercent4) + "%";

        var ned = model.NumEmployeeData!;
        ws.Cells[8, 47].Value = ned.Minimum;
        ws.Cells[8, 48].Value = ned.Percentile25;
        ws.Cells[8, 49].Value = ned.Percentile50;
        ws.Cells[8, 50].Value = ned.Percentile75;
        ws.Cells[8, 51].Value = ned.Maximum;
        ws.Cells[8, 52].Value = ned.StarValue;
        var startlocationPercent5 = DemographicStarLocation(
            ned.Minimum,
            ned.Percentile25,
            ned.Percentile50,
            ned.Percentile75,
            ned.Maximum,
            ned.StarValue);

        ws.Cells[8, 53].Value = Convert.ToString(startlocationPercent5) + "%";

        var rpe = model.RevenuePerEmployeeData!;
        ws.Cells[9, 47].Value = rpe.Minimum;
        ws.Cells[9, 48].Value = rpe.Percentile25;
        ws.Cells[9, 49].Value = rpe.Percentile50;
        ws.Cells[9, 50].Value = rpe.Percentile75;
        ws.Cells[9, 51].Value = rpe.Maximum;
        ws.Cells[9, 52].Value = rpe.StarValue;
        var startlocationPercent6 = DemographicStarLocation(
            rpe.Minimum,
            rpe.Percentile25,
            rpe.Percentile50,
            rpe.Percentile75,
            rpe.Maximum,
            rpe.StarValue);
        startlocationPercent6 = Math.Round(startlocationPercent6, 3);
        ws.Cells[9, 53].Value = Convert.ToString(startlocationPercent6) + "%";
    }

    /*
    /// <summary>
    ///     Previously use for write data in excel sheet. (Not in use)
    /// </summary>
    /// <param name="ws"></param>
    /// <param name="model"></param>
    private void WriteSheetFunCostAsPerRevenueData(_Worksheet ws, FunctionalRevenueModel model)
        => ws.Cells[3, 19].Value = model.FinanceData;
    */

    /// <summary>
    ///     Write business data in Excel sheet for WaterFall chart and sheet (Macro) will ran and create PPT.
    /// </summary>
    /// <param name="w"></param>
    /// <param name="currency"></param>
    /// <param name="exchangeRate"></param>
    private static void WriteSheetCurrencyInfo(_Worksheet w, Currency currency, double exchangeRate)
    {
        var column = 1;
        /*w.Cells[30, */
        column++ /*].Value = currency.Name*/;
        /*w.Cells[30, */
        column++ /*].Value = currency.Description*/;
        /*w.Cells[30, */
        column++ /*].Value = currency.SmallValueLabel*/;
        /*w.Cells[30, */
        column++ /*].Value = currency.SmallValueFormat*/;
        /*w.Cells[30, */
        column++ /*].Value = currency.SmallValueDecimalPlaces*/;
        /*w.Cells[30, */
        column++ /*].Value = currency.LargeValueDivisor*/;
        /*w.Cells[30, */
        column++ /*].Value = currency.LargeValueLabel*/;
        /*w.Cells[30, */
        column++ /*].Value = currency.LargeValueFormat*/;
        /*w.Cells[30, */
        column++ /*].Value = currency.LargeValueDecimalPlaces*/;
        /*w.Cells[30, */
        column++ /*].Value = currency.AmericanFormatting*/;
        /*w.Cells[30, */
        column++ /*].Value = currency.CanBeUsedForReports*/;
        if (exchangeRate > 0)
        {
            var er = 1.0 / exchangeRate;
            w.Cells[30, column++].Value = currency.Name == "USD" ? "" : "1 USD = " + er.ToString(".0000") + " " + currency.Name;
        }
    }

    /// <summary>
    ///     Excel is COM object which is currently we are using in application, so every time after complete the execution
    ///     process of excel need to release this COM object from application.
    ///     .Net feature for avoiding garbage collection in application, need to remove instances of classes and component.
    /// </summary>
    /// <param name="wb"></param>
    /// <param name="workbooks"></param>
    /// <param name="excel"></param>
    private void ReleaseComObjects(Workbook? wb, Workbooks? workbooks, Application? excel)
    {
        if (wb != null) Marshal.ReleaseComObject(wb);
        // Log.Info("Released workbook");
        if (workbooks != null) Marshal.ReleaseComObject(workbooks);
        // Log.Info("Released workbook");
        if (excel != null)
        {
            try
            {
                excel.DisplayAlerts = false;
                excel.Quit();
                Thread.Sleep(1000);
                // Log.Info("Quit excel");
            }
            catch
            {
            }

            Marshal.ReleaseComObject(excel);
            // Log.Info("Released excel");

            /*
            try
            {
                IntPtr hWnd = new IntPtr(excel.Application.Hwnd);
                // Log.Info("Excel hwnd " + hWnd.ToString());
                DestroyWindow(hWnd);
                // SendMessage(hWnd, 0x0016, 0, 0x40000001); // WM_ENDSESSION, marked critical

                //GetWindowThreadProcessId((IntPtr)hWnd, out var processID);
                // Log.Info("Excel processID " + processID);
                //Process proc = Process.GetProcessById((int)processID); //.GetProcessesByName("EXCEL");
                //if (proc == null)
                //{
                //    // Log.Info("Did not get process");
                //}
                //else
                //{
                //    proc.Kill();
                //}
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
            */
        }

        GC.Collect();
        GC.WaitForPendingFinalizers();
    }

    /// <summary>
    ///     Download PPT on server folder.
    /// </summary>
    /// <param name="filename"></param>
    /// <returns></returns>
    [CustomAuthorization]
    public ActionResult DownloadPpt(string? filename)
    {
        if (string.IsNullOrEmpty(filename))
            filename = SessionData.PreviousPptFileName;
        // Log.Info("Entering DownloadPPT with '" + filename + "'");
        var mappedFileName = SessionData.Instance.User.Folder + "\\" + filename + ".pptx";
        //Server.MapPath(SharedTemplatePath                 + SessionData.User.Name + "\\" + filename + ".pptx");
        var fileContentType = "application/vnd.openxmlformats-officedocument.presentationml.presentation";
        if (System.IO.File.Exists(mappedFileName))
        {
            using var fs = System.IO.File.OpenRead(mappedFileName);
            var ppt = new byte[fs.Length];
            fs.Read(ppt, 0, ppt.Length);
            return File(ppt, fileContentType, filename + ".pptx");
        }

        Log.Error("Error while downloading\r\nFile not found: " + mappedFileName);
        return Json(
            new
            {
                Error = "File not found.",
                Filename = filename,
                MappedFilename = mappedFileName
            },
            JsonRequestBehavior.AllowGet);
    }

    /// <summary>
    ///     According business requirement write the message in PPT under SGA chart.
    /// </summary>
    /// <param name="parameters"></param>
    /// <param name="model"></param>
    /// <param name="cost1"></param>
    /// <param name="cost2"></param>
    /// <returns></returns>
    private string GetSgaMessageSummary(ReportParameters parameters, SGAModel model, out string? cost1, out string? cost2)
    {
        string message;
        var selectedTargetName = /*textInfo.ToTitleCase(*/ parameters.BenchmarkCompanyDisplayName /*.ToLower())*/;

        var dtd = model.DifferenceTableData!;
        if (dtd.CostTarget > dtd.CostMedian &&
            dtd.CostMedian > dtd.CostTopQuartile &&
            dtd.CostTopQuartile > dtd.CostTopDecile)
        {
            //regex
            //(cost\d) = (Convert.*);
            //$1 =  String.Format("{0:n0}", $2);
            cost1 = "$" + $"{Math.Round(dtd.OpportunityMedian):n0}";
            cost2 = "$" + $"{Math.Round(dtd.OpportunityTopQuartile):n0}";
            message = selectedTargetName +
                      " is performing below the Median and has SG&A opportunity when compared to Median and Top Quartile. The SG&A opportunity could range between " +
                      cost1 +
                      "MM to " +
                      cost2 +
                      "MM.";
        }
        else if (dtd.CostMedian == dtd.CostTarget &&
                 dtd.CostTarget > dtd.CostTopQuartile &&
                 dtd.CostTopQuartile > dtd.CostTopDecile)
        {
            cost1 = "$" + $"{Math.Round(dtd.OpportunityTopQuartile):n0}";
            cost2 = "$" + $"{Math.Round(dtd.OpportunityTopDecile):n0}";
            message = selectedTargetName +
                      " is performing at the Median level, but has SG&A opportunity when compared to Top Quartile and Top Decile. The SG&A opportunity could range between " +
                      cost1 +
                      "MM to " +
                      cost2 +
                      "MM.";
        }
        else if (dtd.CostMedian > dtd.CostTarget &&
                 dtd.CostTarget > dtd.CostTopQuartile &&
                 dtd.CostTopQuartile > dtd.CostTopDecile)
        {
            cost1 = "$" + $"{Math.Round(dtd.OpportunityTopQuartile):n0}";
            cost2 = "$" + $"{Math.Round(dtd.OpportunityTopDecile):n0}";
            message = selectedTargetName +
                      " is performing better than Median, but has SG&A opportunity when compared to Top Quartile and Top Decile. The SG&A opportunity could range between " +
                      cost1 +
                      "MM to " +
                      cost2 +
                      "MM.";
        }
        else if (dtd.CostMedian > dtd.CostTarget &&
                 dtd.CostTarget == dtd.CostTopQuartile &&
                 dtd.CostTopQuartile > dtd.CostTopDecile)
        {
            cost1 = "$" + $"{Math.Round(dtd.OpportunityTopDecile):n0}";
            cost2 = null;
            message = selectedTargetName +
                      " is performing at the Top Quartile level, but has SG&A opportunity when compared to Top Decile. the SG&A opportunity could is pegged at " +
                      cost1 +
                      "MM.";
        }
        else if (dtd.CostMedian > dtd.CostTopQuartile &&
                 dtd.CostTopQuartile > dtd.CostTarget &&
                 dtd.CostTarget > dtd.CostTopDecile)
        {
            cost1 = "$" + $"{Math.Round(dtd.OpportunityTopDecile):n0}";
            cost2 = null;
            message = selectedTargetName + "'s overall SG&A cost performance in within the first quartile of their peer group; however, there is still a " +
                      cost1 + "MM opportunity gap to attain top decile level performance.";
        }
        else if (dtd.CostMedian > dtd.CostTopQuartile &&
                 dtd.CostTopQuartile > dtd.CostTarget &&
                 dtd.CostTarget == dtd.CostTopDecile)
        {
            cost1 = null;
            cost2 = null;
            message = selectedTargetName + " is a best in class performer.";
        }
        else if (dtd.CostMedian > dtd.CostTopQuartile &&
                 dtd.CostTopQuartile > dtd.CostTopDecile &&
                 dtd.CostTopDecile > dtd.CostTarget)
        {
            cost1 = null;
            cost2 = null;
            message = selectedTargetName + " is a best in class performer.";
        }
        else
        {
            cost1 = null;
            cost2 = null;
            message = selectedTargetName + " is a best in class performer.";
        }

        return message;
    }
}
