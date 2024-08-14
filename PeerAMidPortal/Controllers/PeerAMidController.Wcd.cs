using Microsoft.Office.Interop.Excel;
using PeerAMid.Data;
using PeerAMid.Support;
using PeerAMid.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Web.Mvc;

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
    private string CreateWcd(ReportParameters parameters)
    {
        Impersonator? impersonator = null;
        if (SessionData.ImpersonateComServerExec)
        {
            try
            {
                var domain = Environment.MachineName;
                impersonator = new Impersonator("ComServerExec", domain, "@Microsoft");
                Log.Debug("Impersonating ComServerExec");
            }
            catch (Exception ex)
            {
                Log.Debug("Impersonation failed -- this may not be fatal", ex);
            }
        }
        var s = CreateWcdAsComServerExec(parameters);
        impersonator?.Dispose();
        return s;
    }


    /// <summary>
    /// </summary>
    /// <param name="parameters"></param>
    /// <returns></returns>
    private string CreateWcdAsComServerExec(ReportParameters parameters)
    {
        // Log.Info("Entering CreateWCD()");

        Application? excel = null;
        Workbook? wb = null;
        Workbooks? workbook = null;
        const string xlsmTemplateFileName = "PeerAMid-WCD.xlsm";
        const string pptxTemplateFileName = "PeerAMid-WCD.pptx";
        var visible = true; // SessionData.ExcelIsVisible;

        try
        {
            // Log.Debug("Creating excel");

            excel = new Application
            {
                DisplayAlerts = visible,
                Visible = visible
            };
            // Log.Debug("Excel created");
            while (!excel.Ready) Thread.Sleep(100);
            //excel.Visible = false;
            // Log.Debug("Excel visible set");

            //var comAddIns = excel.COMAddIns;
            //for (var i = 0; i < comAddIns.Count; i++)
            //{
            //    var cai = comAddIns.Item(i + 1);
            //    cai.Connect = false;
            //}

            // Set up the files we'll use
            var userPath = parameters.OutputFolder + "\\"; // .Server.MapPath(SharedTemplatePath + parameters.UserName + "\\");
            if (!Directory.Exists(userPath))
                Directory.CreateDirectory(userPath);
            // Log.Info("userPath = '" + userPath + "'");

            var rawFileName = GenerateDeliverableFileName(parameters, ".xlsm");

            var excelTemplateFullPath = GetFullPathOfTemplateFile(xlsmTemplateFileName);
            // Log.Info("excelTemplateFullPath = '" + excelTemplateFullPath + "'");
            var powerpointTemplateFullPath = GetFullPathOfTemplateFile(pptxTemplateFileName);
            // Log.Info("powerpointTemplateFullPath = '" + powerpointTemplateFullPath + "'");

            var fileInfo1 = new FileInfo(Path.Combine(SessionData.TemplatePath, xlsmTemplateFileName));
            // var templateXlsmPath = fileInfo.FullName;
            var fileInfo2 = new FileInfo(Path.Combine(SessionData.TemplatePath, pptxTemplateFileName));
            // var templatePptxPath = fileInfo.FullName;

            var excelUserFullPath = userPath + "\\" + rawFileName + ".xlsm";
            // Log.Info("excelTemplateFullPath = '" + excelTemplateFullPath + "'");
            var powerpointUserFullPath = userPath + "\\" + rawFileName + ".pptx";
            // Log.Info("powerpointUserFullPath = '" + powerpointUserFullPath + "'");

            System.IO.File.Copy(excelTemplateFullPath, excelUserFullPath, true);
            // Log.Debug("Copied '" + excelTemplateFullPath + "' to '" + excelUserFullPath + "'");
            System.IO.File.Copy(powerpointTemplateFullPath, powerpointUserFullPath, true);
            // Log.Debug("Copied '" + powerpointTemplateFullPath + "' to '" + powerpointUserFullPath + "'");

            // Load worksheet
            // Log.Debug("Opening Excel workbooks object");
            workbook = excel.Workbooks;

            // Log.Debug($"Opening Excel workbook file: '{excelUserFullPath}'");
            while (!excel.Ready) Thread.Sleep(100);
            wb = workbook.Open(
                excelUserFullPath,
                false,
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
            if (wb == null)
                throw new Exception($"Failed to open workbook {excelUserFullPath}");
            // Log.Debug($"Opened Excel workbook: '{excelUserFullPath}'");

            // Load up the models
            // Log.Info("Loading models");
            var workingCapitalData = _homeCore.GetWorkingCapitalData(
                parameters.SelectedTarget,
                parameters.SelectedTargetSymbol,
                parameters.SelectedPeers,
                parameters.Year,
                parameters.OptionId);
            //workingCapitalTrendData = _homeCore.GetWorkingCapitalTrendData(parameters.SelectedTarget, parameters.SelectedPeers, parameters.Year - 10, parameters.Year, 6);

            // Log.Info("Data loaded");

            wb.DoNotPromptForConvert = false;
            wb.CheckCompatibility = false;

            excel.Calculation = XlCalculation.xlCalculationManual;
            var dataWorksheet = (Worksheet)wb.Worksheets["Data"];
            try
            {
                WriteWorkingCapitalDataPage(dataWorksheet, workingCapitalData /*, parameters*/);
            }
            finally
            {
                excel.Calculation = XlCalculation.xlCalculationAutomatic;
                Release(ref dataWorksheet);
                // wb.Save();
            }

            var path = Path.GetDirectoryName(excelUserFullPath)!;
            path = Path.Combine(path, Path.GetFileNameWithoutExtension(excelUserFullPath));
            SetUpPeerGroupAnalysisPage(wb, "CccPage", workingCapitalData, CompanyInfo.Field.Ccc, true, path);
            SetUpPeerGroupAnalysisPage(wb, "DsoPage", workingCapitalData, CompanyInfo.Field.Dso, true, path);
            SetUpPeerGroupAnalysisPage(wb, "DioPage", workingCapitalData, CompanyInfo.Field.Dio, true, path);
            SetUpPeerGroupAnalysisPage(wb, "DpoPage", workingCapitalData, CompanyInfo.Field.Dpo, true, path);

            // Set up Trends page
            //var trendsWorksheet = wb.Worksheets["TrendingPage"];
            //SetUpTrendsPage(trendsWorksheet, workingCapitalTrendData);

            var bsWorksheet = (Worksheet)wb.Worksheets["BenefitSummaryPage"];
            SetUpBenefitSummaryPage(bsWorksheet, workingCapitalData, path);
            Release(ref bsWorksheet);

            var sicDescriptions = SetUpAssumptionsPage(workingCapitalData);

            var apWorksheet = (Worksheet)wb.Worksheets["Phrases"];
            var appPhrases = _homeCore.GetAppPhrases(null, "WCD", null);
            SetUpPhrasesSheet(apWorksheet, workingCapitalData, parameters, userPath, rawFileName, appPhrases, sicDescriptions);
            Release(ref apWorksheet);

            wb.Save();

            var lockWasTaken = false;
            var temp = Obj;

            // Filter output if the SG&A Diagnostic wasn't selected
            // var isFilteredOutput = parameters.Service == PeerAMidService.SgaShort;
            try
            {
                //Threading logic: Use Enter to acquire the Monitor on the object passed as the parameter. If another thread has executed an Enter on the object but has not yet executed the corresponding Exit, the current thread will block until the other thread releases the object.
                // AKA 'trying to solve threading problems by just guessing'
                // Log.Debug("Entering monitor");
                Monitor.Enter(temp, ref lockWasTaken);
                // Log.Debug("Monitor entered");

                // Log.Debug("Running 'CreatePowerPoint' macro");
                wb.Application.Run(
                    "CreatePowerPoint",
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
                    Missing.Value,
                    Missing.Value);
                // Log.Debug("Back from 'CreatePowerPoint' macro");
            }
            catch (Exception ex)
            {
                Log.Warn("COM or Locking exception ", ex);
            }
            finally
            {
                if (lockWasTaken) Monitor.Exit(temp);
            }

            //wb.Save();        // Now done by script itself
            //wb.Close();       // Now done by script itself

            _iPeerAMid.SaveRunAnalysis(rawFileName, parameters.SelectedPeers);

            return rawFileName;
        }
        catch (Exception ex)
        {
            Log.Error(ex);
            return string.Empty;
        }
        finally
        {
            ReleaseComObjects(wb, workbook, excel);
            // Log.Info("Leaving CreateWCD()");
        }
    }


    private void WriteWorkingCapitalDataPage(Worksheet ws, WorkingCapitalData wcd /*, ReportParameters parameters*/)
    {
        // Log.Info("Entering WriteWorkingCapitalDataPage()");
        // SetUpWorkingCapitalDataPage(ws);

        var nextRow = 2; // row 1 is the titles
        var companyRowAssignments = new Dictionary<int, int>();
        var targetRow = -1;

        foreach (var c in wcd.Companies)
        {
            companyRowAssignments.Add(c.UID, nextRow);
            if (c.IsTarget)
                targetRow = nextRow;
            ++nextRow;
        }

        var generations = new SortedList<int, List<int>>();
        for (var cn = CompanyInfo.MinColumnNumber; cn <= CompanyInfo.MaxColumnNumber; ++cn)
        {
            var g = CompanyInfo.ColumnGeneration(cn);
            if (!generations.TryGetValue(g, out var c))
                generations.Add(g, c = new List<int>());
            c.Add(cn);
        }


        foreach (KeyValuePair<int, List<int>> generation in generations) // (var generation = 0; generation <= CompanyInfo.MaxGeneration; ++generation)
        {
            // Log.Debug("WriteWorkingCapitalDataPage - Generation = " + generation);
            foreach (var cn in generation.Value) // (var cn = CompanyInfo.MinColumnNumber; cn <= CompanyInfo.MaxColumnNumber; ++cn)
            {
                // if (CompanyInfo.ColumnGeneration(cn) == generation)
                {
                    // Log.Debug("WriteWorkingCapitalDataPage - Column = " + cn);
                    foreach (var c in wcd.Companies)
                    {
                        //log.Append("      company = " + c.ShortName);
                        object? value = null;
                        var row = companyRowAssignments[c.UID];
                        //log.Append("      row = " + row + "   " + c.Name);
                        try
                        {
                            value = c[cn];
                            if (value == null)
                            {
                                //log.Append("Null value for row = " + row + "   col = " + cn);
                                ws.Cells[row, cn] = "#N/A";
                            }
                            else if (value is double d)
                            {
                                if (double.IsNaN(d))
                                    ws.Cells[row, cn] = -999.0;
                                else
                                    ws.Cells[row, cn] = value;
                            }
                            else
                            {
                                ws.Cells[row, cn] = value;
                            }
                        }
                        catch (Exception ex)
                        {
                            Log.Error(
                                "Exception at  col = " + cn + "   value = " + (value?.ToString() ?? "null"),
                                ex);
                            ws.Cells[row, cn] = "#N/A";
                        }
                        // value = null;
                    }
                }
            }
        }

        // ws.Parent.Save();

        for (var row = wcd.Companies.Count + 2; row < 21; ++row)
        {
            for (var column = 1; column <= CompanyInfo.ColumnNumbers.Count; ++column)
                ws.Cells[row, column].Value = string.Empty;
        }

        ws.Cells[21, 1] = "Companies:";
        ws.Cells[21, 2] = wcd.Companies.Count;

        ws.Cells[22, 1] = "Target Row:";
        ws.Cells[22, 2] = targetRow;

        ws.Cells[23, 1] = "Report:";
        var shortReport = SessionData.SelectedService ==
                          PeerAMidService.WcdShort;
        // Log.Info("Using " + (shortReport ? "SHORT" : "LONG") + " report");
        ws.Cells[23, 2] = shortReport ? "SHORT" : "LONG";
    }

    /*
    private void SetUpWorkingCapitalDataPage(Worksheet ws)
    {
        for (var i = 1; i < 200; ++i)
            ws.Cells[1, i] = "";

        foreach (var kvp in CompanyInfo.ColumnNumbers)
        {
            ws.Cells[1, kvp.Value] = kvp.Key;
        }
    }
    */
    /*
    private void SetUpTrendsPage(Worksheet ws, WorkingCapitalTrendData wctd)
    {
        var row = 2;
        ws.Cells[row, 6] = wctd.AnnualData.Count;

        foreach (var ad in wctd.AnnualData)
        {
            ws.Cells[row, 1] = ad.Value.Year;
            ws.Cells[row, 2] = Math.Round(ad.Value.CCC.GetValueOrDefault(), 0);
            ws.Cells[row, 3] = Math.Round(ad.Value.DIO.GetValueOrDefault(), 0);
            ws.Cells[row, 4] = Math.Round(ad.Value.DPO.GetValueOrDefault(), 0);
            ws.Cells[row, 5] = Math.Round(ad.Value.DSO.GetValueOrDefault(), 0);
            ++row;
        }
    }
    */

    private void SetUpBenefitSummaryPage(Worksheet ws, WorkingCapitalData wcd, string filePath)
    {
        var ccc = wcd.SummaryData["CCC"];

        var row = 7;
        ws.Cells[row, 1] = "Count";
        ws.Cells[row, 2] = ccc.SortedCompanies.Count;

        var boxPlotValues = new List<double>();
        var company = "";
        double target = 0;

        foreach (var co in ccc.SortedCompanies)
        {
            ++row;
            ws.Cells[row, 1] = co.DisplayName;
            ws.Cells[row, 2] = co.Value;
            boxPlotValues.Add(co.Value);

            if (co.Company.IsTarget)
            {
                company = co.DisplayName;
                target = co.Value;
            }

            if (co.IsMedian)
            {
                if (co.Company.IsTarget)
                    ws.Cells[row, 3] = 3;
                else
                    ws.Cells[row, 3] = 1;
            }
            else if (co.Company.IsTarget)
            {
                ws.Cells[row, 3] = 2;
            }
            else
            {
                ws.Cells[row, 3] = 0;
            }
        }

        // Build the box plot
        var filename = filePath + "_" + "BenefitSummaryPage" + "BoxPlot.bmp";
        ws.Cells[45, 2].Value = filename;
        var settings = new BoxPlot.Settings();
        settings.GraphicsSystem = BoxPlot.GraphicsSystems.Gdi;
        settings.Filename = filename;
        settings.Width = (int)ws.Cells[46, 2].Value;
        settings.Height = (int)ws.Cells[47, 2].Value;
        settings.Values.Reset(boxPlotValues);
        settings.LabeledValues.Add(target, company);
        BoxPlot.Generator.Generate(settings);
        //Log.Info($"BoxPlot.Generator.GenerateBoxPlot({settings})");
    }

    private string SetUpAssumptionsPage(WorkingCapitalData wcd)
    {
        var codes = new HashSet<string>();
        SortedList<string, string> descriptions = new SortedList<string, string>(); // description -> codes
        foreach (var co in wcd.Companies)
        {
            if (codes.Add(co.SicCode))
            {
                var desc = co.Sic2DDescription;
                var colon = desc.IndexOf(':');
                if (colon != -1)
                    desc = desc.Substring(0, colon);
                string[] words = desc.Split(' ');
                var start = char.IsDigit(words[0][0]) ? 1 : 0;
                desc = string.Join(" ", words, start, words.Length - start);

                if (descriptions.ContainsKey(desc))
                    descriptions[desc] = descriptions[desc] + ", " + co.SicCode;
                else
                    descriptions.Add(desc, co.SicCode);
            }
        }

        var b = new StringBuilder();
        foreach (KeyValuePair<string, string> kvp in descriptions)
        {
            if (b.Length > 0)
                b.AppendLine();
            b.Append(kvp.Value + " : " + kvp.Key);
        }

        return b.ToString();
    }


    private static int CompareCompanyValues(CompanyInfo a, CompanyInfo b, int fieldNumber, bool reverseOrder)
    {
        var av = (IComparable)a[fieldNumber];
        var bv = (IComparable)b[fieldNumber];

        var c = av.CompareTo(bv);
        if (c == 0)
            return 0;
        return reverseOrder ? -c : c;
    }

    private static void SetUpPeerGroupAnalysisPage(Workbook workbook, string pageName,
        WorkingCapitalData wcd, int fieldNumber, bool reverseOrder, string filePath)
    {
        Log.Info($"Entering SetUpPeerGroupAnalysisPage for {pageName}");
        Worksheet? worksheet = null;
        try
        {
            worksheet = workbook.Worksheets[pageName];
            if (worksheet == null)
            {
                Log.Error($"No worksheet named  {pageName}");
            }
            else
            {
                var companies = new List<CompanyInfo>(wcd.Companies);
                companies.Sort((a, b) => CompareCompanyValues(a, b, fieldNumber, reverseOrder));
                // var targetCompanyColor = ConfigurationManager.AppSettings["sgaTargetCompanyColor"];
                // var medianColor = ConfigurationManager.AppSettings["sgaMedianColor"];
                var row = 36;
                var i = 0;
                var boxPlotValues = new List<double>(companies.Count);
                for ( /**/; i < companies.Count; i++)
                {
                    try
                    {
                        worksheet.Cells[row, 1].Value = companies[i].DisplayName;
                        // Log.Debug($"worksheet.Cells[{row}, 1].Value = {companies[i].DisplayName}");
                        var val = companies[i][fieldNumber];
                        if (val == null)
                        {
                            val = "#N/A";
                            // Log.Debug($"worksheet.Cells[{row}, 2].Value = {val} from null");
                        }
                        else if (val is double d)
                        {
                            if (double.IsNaN(d))
                            {
                                val = "#N/A";
                                // Log.Debug($"worksheet.Cells[{row}, 2].Value = {val} from NaN");
                            }
                            else
                            {
                                val = d;
                                boxPlotValues.Add(d);
                                // Log.Debug($"worksheet.Cells[{row}, 2].Value = {val} from double");
                            }
                        }
                        else
                        {
                            val = val.ToString();
                            // Log.Debug($"worksheet.Cells[{row}, 2].Value = {val} from something else");
                        }

                        worksheet.Cells[row, 2].Value = val;
                        ++row;
                    }
                    catch (Exception ex)
                    {
                        Log.Error($"Error with {companies[i].Name}", ex);
                    }
                }

                while (i < 20)
                {
                    worksheet.Cells[row, 1].Value = "";
                    worksheet.Cells[row, 2].Value = "";
                    ++row;
                    ++i;
                }

                // Build the box plot
                var filename = filePath + "_" + pageName + "BoxPlot.bmp";
                worksheet.Cells[87, 1].Value = filename;
                var settings = new BoxPlot.Settings();
                settings.GraphicsSystem = BoxPlot.GraphicsSystems.Gdi;
                settings.Filename = filename;
                settings.Width = (int)worksheet.Cells[89, 3].Value;
                settings.Height = (int)worksheet.Cells[89, 4].Value;
                settings.Values.Reset(boxPlotValues);
                var companyName = (string)worksheet.Cells[5, 4].Value;
                var target = (double)worksheet.Cells[56, 9].Value;
                settings.LabeledValues.Add(target, companyName);
                BoxPlot.Generator.Generate(settings);
                if (settings.Exception != null)
                    throw settings.Exception;
                Log.Info($"BoxPlot.Generator.GenerateBoxPlot({settings})");
            }
        }
        catch (Exception ex)
        {
            Log.Error(pageName, ex);
        }
        finally
        {
            // Log.Info($"Leaving SetUpPeerGroupAnalysisPage for {pageName}");
            Release(ref worksheet);
        }
    }

    private static void Release(ref Worksheet? _)
    {
        /*
        if (ws != null)
        {
            try
            {
                // Log.Debug("Releasing " + ws.Name);
                Marshal.FinalReleaseComObject(ws);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
            finally
            {
                ws = null;
            }
        }
        */
    }

    private void SetUpPhrasesSheet(Worksheet sheet, WorkingCapitalData wcd, ReportParameters parameters,
        string userPath, string fileName, AppPhrases appPhrases,
        string sicDescriptions)
    {
        // Log.Info("Entering SetUpPhrasesSheet() with version " + appPhrases.Version);

        // The first row is reserved for metadata
        // A1 = appPhrases version number
        var row = 2;

        // Column 1: Logic ID (for AppPhrases only)
        // Column 2: Key
        // Column 3: Conditional Value (for AppPhrases only)
        // Column 4: Condition (for AppPhrases only)
        // Column 5: TRUE if valid, FALSE otherwise
        // Column 6: Replacement Text
        var abbreviations = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        try
        {
            // Write abbreviations first
            for (var i = 0; i < wcd.Companies.Count; ++i)
            {
                if (wcd.Companies[i].IsTarget)
                {
                    AddAbbreviation("CName", wcd.Companies[i].CompanyNameMixedCase, abbreviations);
                    AddAbbreviation("ShortName", wcd.Companies[i].ShortNameMixedCase, abbreviations);
                    AddAbbreviation("IndustryGroup", wcd.Companies[i].SubIndustry, abbreviations);
                    AddAbbreviation("DisplayName", wcd.Companies[i].DisplayName, abbreviations);
                    AddAbbreviation("ShortNameMixedCase", wcd.Companies[i].ShortNameMixedCase, abbreviations);
                    if (wcd.Companies[i].DataEntryCurrency == "USD")
                        AddAbbreviation("ExchangeRateText", "", abbreviations);
                    else
                        AddAbbreviation("ExchangeRateText", wcd.Companies[i].DataEntryExchangeRate.ToString(".0000") + " " + wcd.Companies[i].DataEntryCurrency + " = 1 USD", abbreviations);
                    break;
                }
            }

            var title = "Working Capital Diagnostics";
            if (parameters.Service.IsShortForm())
                title += " | Cash Conversion Cycle";
            AddAbbreviation("Title", title, abbreviations);

            AddAbbreviation("mmmm dd, yyyy", DateTime.Now.ToString("MMMM dd, yyyy"), abbreviations);
            AddAbbreviation("AnalysisPeriod", parameters.Year.ToString(), abbreviations);
            AddAbbreviation("Filename", fileName, abbreviations);
            AddAbbreviation("Folder", userPath, abbreviations);
            AddAbbreviation("SicDescription", sicDescriptions, abbreviations);

            var updatePhrases = true;
            if (int.TryParse(sheet.Cells[1, 1].Value?.ToString() ?? "-1", out int d))
            {
                // Log.Info("Phrase version from spreadsheet is " + d);
                if (d == appPhrases.Version)
                    // Log.Info("AppPhrases are already up-to-date");
                    updatePhrases = false;
            }

            if (updatePhrases)
            {
                sheet.Cells[1, 1].Value = appPhrases.Version.ToString();

                // The individual bits of logic
                foreach (var subject in appPhrases)
                {
                    foreach (var topic in subject)
                    {
                        foreach (var phrase in topic)
                        {
                            var column = 1;
                            try
                            {
                                AddPhrase(phrase.LogicId, phrase.SubjectAndTopic, phrase.Condition, phrase.Text, abbreviations);
                            }
                            catch (Exception ex)
                            {
                                Log.Error(
                                    "Error setting up phrase " + phrase.LogicId + " on row " + row + " col " + column,
                                    ex);
                            }
                        }
                    }
                }

                sheet.Cells[row, 2].Value = "";
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex);
        }
        // Log.Info("Leaving SetUpPhrasesSheet");

        sheet.Parent.Save();

        void AddAbbreviation(string key, string replacement, Dictionary<string, string> abbr)
        {
            abbr.Add("((" + key + "))", replacement);
            AddPhrase(0, key, "TRUE", replacement, null);
        }

        // Column 1: Logic ID (for AppPhrases only)
        // Column 2: Key
        // Column 3: Condition Text (for AppPhrases only)
        // Column 4: Conditional Value (for AppPhrases only)
        // Column 5: Replacement Text
        void AddPhrase(int logicId, string key, string condition, string replacement, Dictionary<string, string>? abbr)
        {
            sheet.Cells[row, 1].Value = logicId;
            sheet.Cells[row, 2].Value = key;
            sheet.Cells[row, 3].Value = "'" + condition;
            sheet.Cells[row, 4].Value = "=IF((" + condition + "), TRUE, FALSE)";
            var mangled =
                replacement.Replace("{", "((").Replace("}", "))"); // We stuff it into the database with '{ ... }'
            if (abbr != null)
            {
                foreach (KeyValuePair<string, string> a in abbreviations)
                {
                    if (mangled.Contains(a.Key))
                        mangled = mangled.Replace(a.Key, a.Value);
                }
            }

            sheet.Cells[row, 5].Value = "'" + mangled;
            ++row;
        }
    }

    /*
    private void RoundDemographicCharModel(DemographicChartModel dcm, int places = 0)
    {
        dcm.Minimum = Math.Round(dcm.Minimum, places, MidpointRounding.AwayFromZero);
        dcm.Percentile25 = Math.Round(dcm.Percentile25, places, MidpointRounding.AwayFromZero);
        dcm.Percentile50 = Math.Round(dcm.Percentile50, places, MidpointRounding.AwayFromZero);
        dcm.Percentile75 = Math.Round(dcm.Percentile75, places, MidpointRounding.AwayFromZero);
        dcm.Maximum = Math.Round(dcm.Maximum, places, MidpointRounding.AwayFromZero);
        dcm.StarValue = Math.Round(dcm.StarValue, places, MidpointRounding.AwayFromZero);
    }

    private void WriteSheetDemographicDataRowWCD<T>(_Worksheet ws, DemographicChartModel dcm, int row, int column, Func<decimal, T> convert)
    {
        decimal starLocationPercent;
        // Log.Info($"In WriteSheetDemographicDataRowWCD 2360 {row} {column} ");
        try
        {
            ws.Cells[row, column++].Value = convert(dcm.Minimum);
            ws.Cells[row, column++].Value = convert(dcm.Percentile25);
            ws.Cells[row, column++].Value = convert(dcm.Percentile50);
            ws.Cells[row, column++].Value = convert(dcm.Percentile75);
            ws.Cells[row, column++].Value = convert(dcm.Maximum);
            ws.Cells[row, column++].Value = convert(dcm.StarValue);
            starLocationPercent = DemographicStarLocation(dcm.Minimum, dcm.Percentile25, dcm.Percentile50, dcm.Percentile75, dcm.Maximum, dcm.StarValue);
            ws.Cells[row, column].Value = Convert.ToString(starLocationPercent) + "%";
        }
        catch (Exception ex)
        {
            Log.Error(ex);
        }
        // Log.Info($"Leaving WriteSheetDemographicDataRowWCD 2376 {row} {column} ");
    }
    */
}
