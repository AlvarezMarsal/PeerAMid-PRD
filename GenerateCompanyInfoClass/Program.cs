using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;

namespace GenerateCompanyInfoClass
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var now = DateTime.Now;
            var sqlScriptName = now.ToString("yyyy MMdd HHmm");
            Generate(@"C:\Working-Files\PeerAMid\Working Capital Diagnostics\WCAP Devp Calculations July 18 2022 - Modified By Rick.xlsx",
                     55,
                     "C:/Working-Files/PeerAMid/PeerAMidPortal/PeerAMid.Data/CompanyInfo.Generated.cs",
                     "C:/Working-Files/PeerAMid/Database Updates/Current/" + sqlScriptName + " - Load Phrases (Generated).sql");
        }

        private static void Generate(string skSpreadsheet, int columnTitleRow, string companyInfoFile, string sqlScriptFile)
        {
            Application excel = null;
            Workbook wb = null;
            Workbooks workbooks = null;
            var message = string.Empty;
            string fileName = string.Empty;

            try
            {
                excel = new Application()
                {
                    DisplayAlerts = false,
                    Visible = false
                };

                workbooks = excel.Workbooks;
                wb = workbooks.Open(skSpreadsheet, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value);
                wb.CheckCompatibility = false;

                GenerateCompanyInfoClass(wb, skSpreadsheet, columnTitleRow, companyInfoFile);
                GenerateSqlScript(wb, skSpreadsheet, sqlScriptFile);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                ReleaseCOMObjects(wb, workbooks, excel);
            }
        }

        #region GenerateCompanyInfoClass

        private static void GenerateCompanyInfoClass(Workbook wb, string skSpreadsheet, int columnTitleRow, string generatedFile)
        {
            var sheet = wb.Worksheets["For PeerAMid WCAP Calc"];

            var info = GetColumnInfoFromSheet(sheet, columnTitleRow);

            info.Add(new FieldInfo(info, info.Count + 1, "IsTarget", null, "bool"));
            info.Add(new FieldInfo(info, info.Count + 1, "ShortName", null, "string"));
            info.Add(new FieldInfo(info, info.Count + 1, "CAGR", null, "double"));
            //info.Add(new Info { SourceColumn = 0, Formula = null, Name = "SicCode", Type = "string" });
            //info.Add(new Info { SourceColumn = 0, Formula = null, Name = "Ee", Type = "int" });
            //info.Add(new Info { SourceColumn = 0, Formula = null, Name = "Sic2D", Type = "string" });
            //info.Add(new Info { SourceColumn = 0, Formula = "EBIT / Sales", Name = "Op1", Type = "double" });
            info.Add(new FieldInfo(info, info.Count + 1, "OP1", "Om1_BO", "double"));           // OM1
            //info.Add(new Info { SourceColumn = 0, Formula = null, Name = "Mktsales", Type = "double?" });
            //info.Add(new Info { SourceColumn = 0, Formula = null, Name = "Mktta", Type = "double?" });
            //info.Add(new Info { SourceColumn = 0, Formula = null, Name = "Mktteq", Type = "double?" });
            info.Add(new FieldInfo(info, info.Count + 1, "Sic2DDescription", null, "string"));
            info.Add(new FieldInfo(info, info.Count + 1, "SubIndustry", null, "string"));
            info.Add(new FieldInfo(info, info.Count + 1, "Country", null, "string"));
            info.Add(new FieldInfo(info, info.Count + 1, "CompanyNameMixedCase", null, "string"));
            info.Add(new FieldInfo(info, info.Count + 1, "ShortNameMixedCase", null, "string"));

            GenerateCompanyInformationClass(skSpreadsheet, info, generatedFile, columnTitleRow);
        }

        private static FieldInfoCollection GetColumnInfoFromSheet(Worksheet sheet, int columnTitleRow)
        {
            var info = new FieldInfoCollection();

            /*
            info.Add(new Info { SourceColumn = 0, Formula = null, Name = "IsTarget", Type = "bool" });
            info.Add(new Info { SourceColumn = 0, Formula = null, Name = "ShortName", Type = "string" });
            info.Add(new Info { SourceColumn = 0, Formula = null, Name = "CAGR", Type = "double" });
            //info.Add(new Info { SourceColumn = 0, Formula = null, Name = "SicCode", Type = "string" });
            //info.Add(new Info { SourceColumn = 0, Formula = null, Name = "Ee", Type = "int" });
            //info.Add(new Info { SourceColumn = 0, Formula = null, Name = "Sic2D", Type = "string" });
            //info.Add(new Info { SourceColumn = 0, Formula = "EBIT / Sales", Name = "Op1", Type = "double" });
            info.Add(new Info { SourceColumn = 0, Formula = "BO56", Name = "Op1", Type = "double" });           // OM1
            //info.Add(new Info { SourceColumn = 0, Formula = null, Name = "Mktsales", Type = "double?" });
            //info.Add(new Info { SourceColumn = 0, Formula = null, Name = "Mktta", Type = "double?" });
            //info.Add(new Info { SourceColumn = 0, Formula = null, Name = "Mktteq", Type = "double?" });
            info.Add(new Info { SourceColumn = 0, Formula = null, Name = "Sic2DDescription", Type = "string" });
            info.Add(new Info { SourceColumn = 0, Formula = null, Name = "SubIndustry", Type = "string" });
            int startingIndex = info.Count;
            */

            // we have to eliminate duplicate names, by adding the column name to them
            var duplicatedNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            var col = 0;
            while (true)
            {
                ++col; // we start at 1

                // we get the name from the title cell
                var titleCell = sheet.Cells[columnTitleRow, col];
                var name = (string)titleCell?.Value?.ToString();
                if (string.IsNullOrWhiteSpace(name))
                    break;

                // Make the name something we like better
                name = ImproveName(name);
                if (info.TryToGetByName(name, out var item))
                {
                    duplicatedNames.Add(name);
                    item.Rename(item.Name + "_" + item.ColumnName);
                    name += "_" + ColumnName(col);
                }
                else if (duplicatedNames.Contains(name))
                {
                    name += "_" + ColumnName(col);
                }

                var cell = sheet.Cells[columnTitleRow + 1, col];
                if (cell == null)
                    break;

                // Figure out the type
                string type;
                switch (name)
                {
                    case "SicCode":
                    case "Sic2D":
                    case "Sic2DDescription":
                    case "SubIndustry":
                    case "Updatedon":
                        type = "string";
                        break;

                    case "Ee":
                        type = "int";
                        break;

                    case "Mktsales":
                    case "Mktta":
                    case "Mktteq":
                        type = "double?";
                        break;

                    default: // the general case
                        if (cell.Value == null)
                            type = "double";
                        else if (cell.Value is double)
                            type = "double";
                        else if (cell.Value == "NULL")
                            type = "double";
                        else
                            type = "string";
                        break;
                }

                // If the cell has a formula, we'll need it
                var formula = cell.Formula;
                if (!string.IsNullOrWhiteSpace(formula) && formula.StartsWith("="))
                {
                    if (formula.StartsWith("=+"))
                        formula = formula.Substring(2);
                    else
                        formula = formula.Substring(1);
                }
                else
                {
                    formula = null;
                }

                var newfieldInfo = info.Add(new FieldInfo(info, col, name, formula, type));

                if ((name == "DsoUq") || (name == "DioUq") || (name == "DpoUq"))
                {
                    newfieldInfo.Generation = 10;
                    newfieldInfo.OriginalFormula = "Calculate" + name + "()";
                }

                if (name == "Updatedon")
                {
                    newfieldInfo.OriginalFormula = "DateTime.Now.ToString()";
                }
            }

            return info;
        }

        private static void GenerateCompanyInformationClass(string skSpreadsheet, FieldInfoCollection info, string generatedFile, int columnTitleRow)
        {
            int maxGeneration = 0;
            foreach (var i in info)
            {
                var g = CalculateGeneration(info, i);
                if (g > maxGeneration)
                    maxGeneration = g;
            }

            ++maxGeneration;

            foreach (var i in info)
            {
                if ((i.TranslatedFormula == "AX71") || (i.TranslatedFormula == "AY71") || (i.TranslatedFormula == "AZ73"))
                    i.Generation = maxGeneration;
            }

            var b = new StringBuilder();
            b.AppendLine($"// Generated {DateTime.Now} from {skSpreadsheet}");
            b.AppendLine("using System;").AppendLine("using System.Collections.Generic;");
            b.AppendLine("using YardStick.Common;").AppendLine();
            b.AppendLine("namespace YardStick.Data.Home");
            b.AppendLine("{");
            b.AppendLine();
            b.AppendLine("    public partial class CompanyInfo");
            b.AppendLine("    {");
            b.AppendLine("        private const int COLUMN_TITLE_ROW = " + columnTitleRow + ";");
            b.AppendLine("        private const int TOP_COMPANY_ROW = COLUMN_TITLE_ROW + 1;");
            b.AppendLine("        public static string[] ColumnNames;");
            b.AppendLine("        public static SortedList<string, int> ColumnNumbers;");
            b.AppendLine("        public const int MinColumnNumber = " + info.ByIndex(0).ColumnNumber + ";");
            b.AppendLine("        public const int MaxColumnNumber = " + info.ByIndex(info.Count - 1).ColumnNumber + ";");

            b.AppendLine("        static CompanyInfo()");
            b.AppendLine("        {");
            b.AppendLine("            ColumnNames = new string[" + (info.Count + 1) + "];");

            //foreach (var i in info)
            //{
            //    b.AppendLine("            ColumnNames[" + i.TargetColumn + "] = \"" + i.Name.ToUpper() + "\";");
            //    if (i.SourceColumn != 0)
            //    {
            //        var cn = ColumnName(i.SourceColumn);
            //        oldColumnsToNewColumns.Add(cn, ColumnName(i.TargetColumn));
            //        oldColumnsToNames.Add(cn, i.Name);
            //    }
            //}

            b.AppendLine("            ColumnNumbers = new SortedList<string, int>(ColumnNames.Length);");
            b.AppendLine("            for (var i = 0; i<ColumnNames.Length; ++i)");
            b.AppendLine("                if (!string.IsNullOrEmpty(ColumnNames[i]))");
            b.AppendLine("                    ColumnNumbers.Add(ColumnNames[i], i);");
            b.AppendLine("        }");
            b.AppendLine();
            // b.AppendLine("        public readonly CrossCompanyInfo CrossCompanyInfo;");

            b.AppendLine();
            b.AppendLine("        public static class Field");
            b.AppendLine("        {");
            foreach (var i in info)
            {
                b.AppendLine("            public const int " + i.Name + " = " + i.ColumnNumber + ";");
            }
            b.AppendLine("        }");

            foreach (var i in info)
            {
                var l = b.Length;

                b.Append("        ");
                if (i.OriginalFormula == null)
                {
                    i.Generation = 0; // raw data is always Generation 0
                    b.Append(i.Visibility + " " + i.Type + " " + i.Name + " = " + i.DefaultValue + ";");
                }
                else
                {
                    b.Append("[Newtonsoft.Json.JsonIgnore] ");

                    /*
                    var type = "string";
                    var f = CleanFormula(i);
                    */

                    var type = i.Type;
                    var f = TranslateFormula(info, i, columnTitleRow + 1, 2); // this also calcultes generations

                    /*
                    if (f == "AX71")
                    {
                        f = "DSOUQ_AX71";
                        b.Append(i.Visibility + " string " + i.Name + " = \"=" + f + "\";");
                    }
                    else if (f == "AY71")
                    {
                        f = "DIOUQ_AY71";
                        b.Append(i.Visibility + " string " + i.Name + " = \"=" + f + "\";");
                    }
                    else if (f == "AZ73")
                    {
                        f = "DPOUQ_AZ73";
                        b.Append(i.Visibility + " string " + i.Name + " = \"=" + f + "\";");
                    }
                    else
                    */
                    {
                        if (i.Type == "string")
                            b.Append(i.Visibility + " " + type + " " + i.Name + " { get { " + "try { return " + f + "; } catch { return string.Empty; }}}");
                        else if (i.Type == "int")
                            b.Append(i.Visibility + " " + type + " " + i.Name + " { get { " + "try { return " + f + "; } catch { return 0; }}}");
                        else
                            b.Append(i.Visibility + " " + type + " " + i.Name + " { get { " + "var v = " + f + "; return (double.IsNaN(v) || double.IsInfinity(v)) ? double.NaN : v; }}");
                    }
                }

                var s = b.Length - l;
                var t = 80 - s;
                if (t > 0)
                    b.Append(' ', t);
                else
                    b.Append("    ");
                if (i.Comment1 != null)
                    b.Append("// " + i.Comment1 + " // Generation " + i.Generation);
                else
                    b.Append("// " + ColumnName(i.ColumnNumber) + " in SK's original spreadsheet" + " // Generation " + i.Generation);
                b.AppendLine();
            }

            b.AppendLine().AppendLine("        public const int MaxGeneration = " + maxGeneration + ";");
            /*
            b.AppendLine().AppendLine("        // Formula terms");
            foreach (var i in info)
            {
                b.AppendLine("        private " + i.Type + " " + ColumnName(i.Column) + (i.Row + 1) + " => " + i.Name + ";"); // Row+1 because the formula refers to a value in the cell just below the name cell
            }
            */

            /*
            b.AppendLine().AppendLine("        // Methods");
            b.AppendLine("        private T IF<T>(bool condition, T ifTrue, T ifFalse) => condition ? ifTrue : ifFalse;");
            b.AppendLine("        private bool ISERROR(object o) => (o == null);");
            b.AppendLine("        private bool OR(params bool[] conditions) { foreach (var c in conditions) if (c) return true; return false; }");
            b.AppendLine("        private double SUM(params double[] items) { var sum = items[0]; for (var i = 1; i<items.Length; ++i) sum += items[i]; return sum; }");
            b.AppendLine("        private double COUNT(params double[] items) => items.Length;");

            b.AppendLine("        private double PERCENTRANK(int row1, int row2, int col, int rTarget, int cTarget)");
            b.AppendLine("        {");
            b.AppendLine("            var v = DoubleFromCell(rTarget, cTarget);");
            b.AppendLine("            var d = new double[row2-row1+1];");
            b.AppendLine("            for (var i=0; i<d.Length; ++i)");
            b.AppendLine("                d[i] = DoubleFromCell(i + row1, col);");
            b.AppendLine("            Array.Sort(d);");
            b.AppendLine("            return PERCENTRANK(d,v);");
            b.AppendLine("        }");

            b.AppendLine("        private static double PERCENTRANK(double[] d, double t)");
            b.AppendLine("        {");
            b.AppendLine("            int i0=-1, i1=-1;");
            b.AppendLine("            for (int i = 0; i < d.Length; i++)");
            b.AppendLine("            {");
            b.AppendLine("                if (d[i] == t)");
            b.AppendLine("                    return ((double)i) / (d.Length - 1);");
            b.AppendLine("                if (d[i] < t)");
            b.AppendLine("                {");
            b.AppendLine("                    i0 = i;");
            b.AppendLine("                }");
            b.AppendLine("                else if (d[i] > t)");
            b.AppendLine("                { ");
            b.AppendLine("                    i1 = i;");
            b.AppendLine("                    break;");
            b.AppendLine("                }");
            b.AppendLine("            }");
            b.AppendLine("            if (i0 == -1) return 0;");
            b.AppendLine("            if (i1 == -1) return 1;");
            b.AppendLine("            double x1 = d[i0], x2 = d[i1];");
            b.AppendLine("            double y1 = ((double) i0) / (d.Length - 1);"); // PERCENTRANK(d, x1);");
            b.AppendLine("            double y2 = ((double) i1) / (d.Length - 1);"); // PERCENTRANK(d, x2);");
            b.AppendLine("            return (((x2 - t) * y1 + (t - x1) * y2)) / (x2 - x1);");
            b.AppendLine("        }");
            */

            // Access the generation (calculation order) of a column.  Raw data is Generation 0.

            b.AppendLine("        public static int ColumnGeneration(int columnNumber)");
            b.AppendLine("        {");
            b.AppendLine("            switch (columnNumber)");
            b.AppendLine("            {");
            foreach (var i in info)
            {
                b.AppendLine("                      case " + i.ColumnNumber + ": return " + i.Generation + ";    // " + i.Name + " = " + i.TranslatedFormula);
            }
            b.AppendLine("                default: throw new NotImplementedException();");
            b.AppendLine("            }");
            b.AppendLine("        }");

            b.AppendLine("        public object this[string name]");
            b.AppendLine("        {");
            b.AppendLine("             get");
            b.AppendLine("             {");
            b.AppendLine("                  switch (name)");
            b.AppendLine("                  {");
            foreach (var i in info)
            {
                b.AppendLine("                      case \"" + i.Name.ToUpper() + "\": return this." + i.Name + ";");
            }
            b.AppendLine("                     default: Log.Error(\"Bad column name: \" + name); return null;");
            b.AppendLine("                  }");
            b.AppendLine("              }");
            b.AppendLine("        }");
            b.AppendLine();

            b.AppendLine("        public object this[int i]");
            b.AppendLine("        {");
            b.AppendLine("             get");
            b.AppendLine("             {");
            b.AppendLine("                  switch (i)");
            b.AppendLine("                  {");
            foreach (var i in info)
            {
                b.AppendLine("                      case Field." + i.Name + ": return this." + i.Name + ";");
            }
            b.AppendLine("                      default: return null;");
            b.AppendLine("                  }");
            b.AppendLine("              }");
            b.AppendLine("        }");
            b.AppendLine();

            b.AppendLine("    }");
            b.AppendLine("}");
            Debug.Write(b.ToString());
            File.WriteAllText(generatedFile, b.ToString());
        }

        private static int CalculateGeneration(FieldInfoCollection allInfo, FieldInfo info)
        {
            if (info.OriginalFormula != null)
            {
                var formula = info.OriginalFormula; // TODO
                var match = refRegex.Match(formula);
                while ((match != null) && match.Success)
                {
                    var n = allInfo.ByColumnName(match.Groups[1].Value); // ColumnNameToColumnNumber(match.Groups[1].Value);
                    var g = n.Generation;
                    if (g == 0)
                        g = CalculateGeneration(allInfo, n);
                    if (g >= info.Generation)
                        info.Generation = g + 1;
                    match = refRegex.Match(formula, match.Index + match.Length);
                }
            }
            return info.Generation;
        }

        /*
        private static Info FindInfoByOldColumn(InfoCollection allInfo, string oldColumnName)
        {
            var index = Alphabet.IndexOf(oldColumnName[0]);
            if (oldColumnName.Length == 2)
            {
                index = (index + 1) * 26;
                index += Alphabet.IndexOf(oldColumnName[1]);
            }

            ++index;
            foreach (var info in allInfo)
                if (info.SourceColumn == index)
                    return info;

            return null;
        }
        */

        private static string ImproveName(string name)
        {
            var b = new StringBuilder();
            bool nextCharIsUpper = true;
            foreach (var c in name)
            {
                if (char.IsLetter(c))
                {
                    if (nextCharIsUpper)
                    {
                        b.Append(char.ToUpper(c));
                        nextCharIsUpper = false;
                    }
                    else
                    {
                        b.Append(char.ToLower(c));
                        nextCharIsUpper = false;
                    }
                }
                else if (char.IsDigit(c))
                {
                    if (b.Length > 0)
                    {
                        b.Append(c);
                        nextCharIsUpper = true;
                    }
                }
                else if (char.IsWhiteSpace(c) || (c == '_'))
                {
                    nextCharIsUpper = true;
                }
                else
                {
                    if (b.Length > 0)
                    {
                        b.Append('_');
                        nextCharIsUpper = true;
                    }
                }
            }

            return b.ToString();
        }

        private static Regex sumRegex = new Regex(@"SUM\((?<col1>\D*)(?<row1>\d*)\:(?<col2>\D*)(?<row2>\d*)\)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static Regex refRegex = new Regex(@"([A-Z][A-Z]?)56", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static Regex percentRankRegex = new Regex(@"PERCENTRANK\((?<col1>\D*)(?<row1>\d*)\:(?<col2>\D*)(?<row2>\d*),(?<col3>\D*)(?<row3>\d*)\)", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static string CleanFormula(string originalFormula)
        {
            var cleanedFormula = originalFormula.Replace("$", "");
            cleanedFormula = cleanedFormula.Replace("\" \"", Error.ToString());
            cleanedFormula = cleanedFormula.Replace("\"\"", Error.ToString());
            cleanedFormula = cleanedFormula.Replace("\"N/M\"", Error.ToString());

            return cleanedFormula;
        }

        private static string TranslateFormula(FieldInfoCollection allInfo, FieldInfo info, int firstCompanyRowSK, int firstCompanyRow)
        {
            var formula = CleanFormula(info.OriginalFormula);
            var comment = info.OriginalFormula;

            /* Translate SUM() */
            var match = sumRegex.Match(formula);
            while ((match != null) && match.Success)
            {
                var sb = new StringBuilder();
                sb.Append("(");
                var row1 = int.Parse(match.Groups[2].Value);
                var row2 = int.Parse(match.Groups[4].Value);
                var col1 = ColumnNumber(match.Groups[1].Value);
                var col2 = ColumnNumber(match.Groups[3].Value);
                for (var r = row1; r <= row2; ++r)
                {
                    for (var c = col1; c <= col2; ++c)
                    {
                        if (sb.Length > 1)
                            sb.Append("+");
                        sb.Append(ColumnName(c) + r);
                    }
                }
                sb.Append(")");
                formula = formula.Substring(0, match.Index) + sb.ToString() + formula.Substring(match.Index + match.Length);
                match = sumRegex.Match(formula);
            }

            match = percentRankRegex.Match(formula);
            while ((match != null) && match.Success)
            {
                // The formula actually comes from the row BELOW the title row
                var sb = new StringBuilder();
                sb.Append("(");
                var row1 = int.Parse(match.Groups[2].Value); // 56, currently, for the first company row
                var row2 = int.Parse(match.Groups[4].Value);
                var row3 = int.Parse(match.Groups[6].Value);
                var col1 = ColumnNumber(match.Groups[1].Value);
                var col2 = ColumnNumber(match.Groups[3].Value);
                var col3 = ColumnNumber(match.Groups[5].Value);

                if (col1 != col2)
                {
                    throw new Exception("PERCENTRANK is gonna blow");
                }

                //              sb.Append("PERCENTRANK(").Append(row1 - firstCompanyRowSK + firstCompanyRow).Append(",").Append(row2 - firstCompanyRowSK + firstCompanyRow).Append(",").Append(col1).Append(",CompanyIndex+").Append(firstCompanyRow).Append(",").Append(col3).Append(")");
                sb.Append("PERCENTRANK(").Append(row1 - firstCompanyRowSK + firstCompanyRow).Append(",").Append(row1 - firstCompanyRowSK + firstCompanyRow).Append(" + NumberOfCompanies - 1,").Append(col1).Append(", CompanyIndex+").Append(firstCompanyRow).Append(",").Append(col3).Append(")");

                sb.Append(")");
                formula = formula.Substring(0, match.Index) + sb.ToString() + formula.Substring(match.Index + match.Length);
                match = percentRankRegex.Match(formula);
            }

            // Translate cell references
            match = refRegex.Match(formula);
            while ((match != null) && match.Success)
            {
                try
                {
                    var oldColumnName = match.Groups[1].Value;
                    var i = allInfo.ByColumnName(oldColumnName);
                    formula = formula.Replace(oldColumnName + "56", i.Name);
                    match = refRegex.Match(formula);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    throw;
                }
            }

            info.TranslatedFormula = formula;
            info.Comment1 = comment;

            return info.TranslatedFormula;
        }

        private static readonly string Alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        internal static string ColumnName(int column)
        {
            if (column == 0)
                return "0";
            --column; // columns are 1-based
            if (column < 26)
                return Alphabet.Substring(column, 1);
            column -= 26;
            var first = column / 26;
            var second = column % 26;
            return Alphabet.Substring(first, 1) + Alphabet.Substring(second, 1);
        }

        internal static int ColumnNumber(string column)
        {
            var number = 0;
            for (var i = 0; i < column.Length; i++)
            {
                number *= 26;
                number += Alphabet.IndexOf(column[i]) + 1;
            }
            return number;
        }

        #endregion GenerateCompanyInfoClass

        #region Generate SQL script

        private static string Term = @"([a-zA-Z0-9_\.-]+)";
        private static string Spaces = @"\s*";
        private static string Comparison = @"([\>\<\=]+)";
        private static Regex betweenRegex = new Regex(Term + Spaces + Comparison + Spaces + Term + Spaces + Comparison + Spaces + Term, RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private const double Error = -999.0;

        private static void GenerateSqlScript(Workbook wb, string skSpreadsheet, string sqlScriptFile)
        {
            var sheet = wb.Worksheets["Text Logic"];
            var b = new StringBuilder();
            var slides = new Dictionary<string, string>();
            slides.Add("Cash Conversion Cycle", "CccPage");
            slides.Add("Days Sales Outstanding", "DsoPage");
            slides.Add("Days Inventory Outstanding", "DioPage");
            slides.Add("Days Payable Outstanding", "DpoPage");
            slides.Add("Cash Conversion Cycle UQ Benefit Summary", "BenefitSummaryPage");
            slides.Add("Liquidity Driver", "LiquidityDriversPage");
            slides.Add("Profitability Driver", "ProfitabilityDriversPage");
            slides.Add("Leverage Driver", "LeverageDriversPage");
            slides.Add("Turnover Driver", "TurnoverDriversPage");

            b.AppendLine($"-- Generated {DateTime.Now} from {skSpreadsheet}").AppendLine();
            b.AppendLine("USE [PeerAMid]").AppendLine("GO").AppendLine();
            b.AppendLine("DELETE FROM [dbo].[Phrases]").AppendLine("GO").AppendLine();

            // Skip rows without entries
            int row;
            for (row = 1; row < 1000; ++row)
            {
                var cell = sheet.Cells[row, 2]; // The id number is in column B
                if (cell == null)
                    continue;
                if (cell.Value == null)
                    continue;
                if (char.IsDigit(cell.Value.ToString()[0]))
                    break;
            }

            for (/**/; row < 1000; ++row)
            {
                var cell = sheet.Cells[row, 2]; // The id number is in column B
                if (cell == null)
                    break;
                if (cell.Value == null)
                    break;

                var id = int.Parse(cell.Value.ToString());

                string skSlide = sheet.Cells[row, 4].Value.ToString();
                var slide = slides[skSlide];
                string metric = sheet.Cells[row, 5].Value.ToString();
                metric = metric.Replace(" ", "").Replace("&", "");
                var fullMetric = slide + "_" + metric;

                string condition = sheet.Cells[row, 6].Value.ToString();

                if (condition.StartsWith("Green | ") || condition.StartsWith("Red | ") || condition.StartsWith("Yellow | ") || condition.StartsWith("Gold | "))
                {
                    var index = condition.IndexOf("|") + 2;
                    condition = condition.Substring(index);
                }
                else if (condition == "Negative Ratio")
                {
                    condition = "Target Value < 0";
                }

                condition = condition.Replace("Target Value", fullMetric);

                condition = condition.Replace("MIN", "0.00");
                condition = condition.Replace("p10", "0.10");
                condition = condition.Replace("p25", "0.25");
                condition = condition.Replace("p50", "0.50");
                condition = condition.Replace("p75", "0.75");
                condition = condition.Replace("p90", "0.90");
                condition = condition.Replace("MAX", "1.00");

                var match = betweenRegex.Match(condition);
                if ((match != null) && match.Success)
                {
                    var before = condition.Substring(0, match.Index);
                    var after = condition.Substring(match.Index + match.Length);
                    condition = before + "AND((" + match.Groups[1] + " " + match.Groups[2] + " " + match.Groups[3] + "), (" + match.Groups[3] + " " + match.Groups[4] + " " + match.Groups[5] + "))" + after;
                }

                var text = sheet.Cells[row, 7].Value.ToString().Trim();
                if (text.StartsWith("’s "))
                    text = "((ShortName))" + text;
                else if (text.StartsWith("'s "))
                    text = "((ShortName))’s " + text.Substring(3);
                else if (text.StartsWith("is ") || text.StartsWith("may ") || text.StartsWith("are ") || text.StartsWith("will "))
                    text = "((ShortName)) " + text;
                text = text.Replace("'", "''");

                b.Append("INSERT [dbo].[Phrases](LogicID, Language, App, Subject, Topic, Condition, Text) VALUES (").Append(id).Append(", NULL, N'WCD', N'").Append(slide).Append("', N'").Append(fullMetric).Append("', N'").Append(condition).Append("', N'").Append(text).Append("')");
                b.AppendLine();
            }

            b.AppendLine().AppendLine("GO").AppendLine();

            File.WriteAllText(sqlScriptFile, b.ToString());
        }

        #endregion Generate SQL script

        private static void ReleaseCOMObjects(Workbook wb, Workbooks workbook, Microsoft.Office.Interop.Excel.Application excel)
        {
            if (wb != null)
            {
                Marshal.ReleaseComObject(wb);
            }
            if (workbook != null) Marshal.ReleaseComObject(workbook);
            if (excel != null)
            {
                try
                {
                    excel.Quit();
                }
                catch
                {
                }
                int hWnd = excel.Application.Hwnd;
                GetWindowThreadProcessId((IntPtr)hWnd, out var processID);
                Process[] procs = Process.GetProcessesByName("EXCEL");
                foreach (Process p in procs)
                {
                    if (p.Id == processID)
                        p.Kill();
                }
                Marshal.FinalReleaseComObject(excel);
            }

            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        [DllImport("user32.dll")]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);
    }
}