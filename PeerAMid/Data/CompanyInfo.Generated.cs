// Generated 9/29/2022 4:14:50 PM from C:\Working-Files\PeerAMid\Working Capital Diagnostics\WCAP Devp Calculations July 18 2022 - Modified By Rick.xlsx
using System;
using System.Collections.Generic;
using PeerAMid.Support;

namespace PeerAMid.Data
{
    public partial class CompanyInfo
    {
        private const int COLUMN_TITLE_ROW = 55;
        private const int TOP_COMPANY_ROW = COLUMN_TITLE_ROW + 1;
        public static string[] ColumnNames;
        public static SortedList<string, int> ColumnNumbers;
        public const int MinColumnNumber = 1;
        public const int MaxColumnNumber = 139;

        static CompanyInfo()
        {
            ColumnNames = new string[140];
            ColumnNumbers = new SortedList<string, int>(ColumnNames.Length);
            for (var i = 0; i < ColumnNames.Length; ++i)
                if (!string.IsNullOrEmpty(ColumnNames[i]))
                    ColumnNumbers.Add(ColumnNames[i], i);
        }

        public static class Field
        {
            public const int SqlQry = 1;
            public const int Factid = 2;
            public const int Coname = 3;
            public const int Uid_D = 4;
            public const int Sic2D = 5;
            public const int SicCode = 6;
            public const int SgaMult = 7;
            public const int Yscid = 8;
            public const int CikId = 9;
            public const int Rev1 = 10;
            public const int Sga1 = 11;
            public const int Ga1M = 12;
            public const int Sgm1 = 13;
            public const int Ee = 14;
            public const int Revperee = 15;
            public const int CashEquiv = 16;
            public const int CurrAssets = 17;
            public const int TotalAssets = 18;
            public const int CurrLiab = 19;
            public const int Cogs = 20;
            public const int Ar = 21;
            public const int Inventory = 22;
            public const int Ap = 23;
            public const int TLiab = 24;
            public const int TotalEquity = 25;
            public const int Mktcap = 26;
            public const int Teqliab = 27;
            public const int Retainede = 28;
            public const int Wcta_AC = 29;
            public const int Cashcl_AD = 30;
            public const int Cashta_AE = 31;
            public const int Cashsale_AF = 32;
            public const int Ccratio_AG = 33;
            public const int Cata_AH = 34;
            public const int ClEq_AI = 35;
            public const int Qrat_AJ = 36;
            public const int Qta_AK = 37;
            public const int Invca_AL = 38;
            public const int Wcta_AM = 39;
            public const int Cashcl_AN = 40;
            public const int Cashta_AO = 41;
            public const int Cashsale_AP = 42;
            public const int Ccratio_AQ = 43;
            public const int Cata_AR = 44;
            public const int ClEq_AS = 45;
            public const int Qrat_AT = 46;
            public const int Qta_AU = 47;
            public const int Invca_AV = 48;
            public const int LiquidityScore = 49;
            public const int Dso = 50;
            public const int Dio = 51;
            public const int Dpo = 52;
            public const int Ccc = 53;
            public const int Ebitda1 = 54;
            public const int Em1 = 55;
            public const int EbitdaPercentileRanking = 56;
            public const int Gp = 57;
            public const int Ni = 58;
            public const int Ebit = 59;
            public const int Gppe = 60;
            public const int Nppe = 61;
            public const int Roa_BJ = 62;
            public const int Roe_BK = 63;
            public const int Gm1_BL = 64;
            public const int Np1_BM = 65;
            public const int Sgam_BN = 66;
            public const int Om1_BO = 67;
            public const int Ebitta_BP = 68;
            public const int Roa_BQ = 69;
            public const int Roe_BR = 70;
            public const int Gm1_BS = 71;
            public const int Np1_BT = 72;
            public const int Sgam_BU = 73;
            public const int Om1_BV = 74;
            public const int Ebitta_BW = 75;
            public const int ProfitabilityScore = 76;
            public const int Wcap = 77;
            public const int Wcap1 = 78;
            public const int Wcsales = 79;
            public const int AvgWorkingCapital = 80;
            public const int NetSales_Avg_Wcap_CC = 81;
            public const int Iturns_CD = 82;
            public const int Faturn_CE = 83;
            public const int Taturn_CF = 84;
            public const int Eqturns_CG = 85;
            public const int Invsales_CH = 86;
            public const int Rturns_CI = 87;
            public const int Qasales_CJ = 88;
            public const int Casales_CK = 89;
            public const int NetSales_Avg_Wcap_CL = 90;
            public const int Iturns_CM = 91;
            public const int Faturn_CN = 92;
            public const int Taturn_CO = 93;
            public const int Eqturns_CP = 94;
            public const int Invsales_CQ = 95;
            public const int Rturns_CR = 96;
            public const int Qasales_CS = 97;
            public const int Casales_CT = 98;
            public const int TurnoverScore = 99;
            public const int Dbteq_CV = 100;
            public const int Dbtta_CW = 101;
            public const int Fnmulti_CX = 102;
            public const int Faeqltl_CY = 103;
            public const int Reta_CZ = 104;
            public const int Dbteq_DA = 105;
            public const int Dbtta_DB = 106;
            public const int Fnmulti_DC = 107;
            public const int Faeqltl_DD = 108;
            public const int Reta_DE = 109;
            public const int LeverageScore = 110;
            public const int CompositeScore = 111;
            public const int Createdby = 112;
            public const int Createdon = 113;
            public const int Updatedby = 114;
            public const int Updatedon = 115;
            public const int Datayear = 116;
            public const int Uid_DM = 117;
            public const int OneDaySales = 118;
            public const int OneDayCogs = 119;
            public const int DsoUq = 120;
            public const int DpoUq = 121;
            public const int DioUq = 122;
            public const int Dso_Sales_ = 123;
            public const int Dpo_Cogs_ = 124;
            public const int Dio_Cogs_ = 125;
            public const int Twc_BasedOnCogs_ = 126;
            public const int ActualRoce = 127;
            public const int CapitalEmployed_Ta_Cl_ = 128;
            public const int NewRoceWithTwcImpprovement = 129;
            public const int TwcScope = 130;
            public const int IsTarget = 131;
            public const int ShortName = 132;
            public const int CAGR = 133;
            public const int OP1 = 134;
            public const int Sic2DDescription = 135;
            public const int SubIndustry = 136;
            public const int Country = 137;
            public const int CompanyNameMixedCase = 138;
            public const int ShortNameMixedCase = 139;
        }

        public double SqlQry = 0;                                               // A in SK's original spreadsheet // Generation 0
        public double Factid = 0;                                               // B in SK's original spreadsheet // Generation 0
        public string Coname = null;                                            // C in SK's original spreadsheet // Generation 0
        public double Uid_D = 0;                                                // D in SK's original spreadsheet // Generation 0
        public string Sic2D = null;                                             // E in SK's original spreadsheet // Generation 0
        public string SicCode = null;                                           // F in SK's original spreadsheet // Generation 0
        public double SgaMult = 0;                                              // G in SK's original spreadsheet // Generation 0
        public double Yscid = 0;                                                // H in SK's original spreadsheet // Generation 0
        public double CikId = 0;                                                // I in SK's original spreadsheet // Generation 0
        public double Rev1 = 0;                                                 // J in SK's original spreadsheet // Generation 0
        public double Sga1 = 0;                                                 // K in SK's original spreadsheet // Generation 0
        public double Ga1M = 0;                                                 // L in SK's original spreadsheet // Generation 0
        public double Sgm1 = 0;                                                 // M in SK's original spreadsheet // Generation 0
        public int Ee = 0;                                                      // N in SK's original spreadsheet // Generation 0
        public double Revperee = 0;                                             // O in SK's original spreadsheet // Generation 0
        public double CashEquiv = 0;                                            // P in SK's original spreadsheet // Generation 0
        public double CurrAssets = 0;                                           // Q in SK's original spreadsheet // Generation 0
        public double TotalAssets = 0;                                          // R in SK's original spreadsheet // Generation 0
        public double CurrLiab = 0;                                             // S in SK's original spreadsheet // Generation 0
        public double Cogs = 0;                                                 // T in SK's original spreadsheet // Generation 0
        public double Ar = 0;                                                   // U in SK's original spreadsheet // Generation 0
        public double Inventory = 0;                                            // V in SK's original spreadsheet // Generation 0
        public double Ap = 0;                                                   // W in SK's original spreadsheet // Generation 0
        public double TLiab = 0;                                                // X in SK's original spreadsheet // Generation 0
        public double TotalEquity = 0;                                          // Y in SK's original spreadsheet // Generation 0
        public double Mktcap = 0;                                               // Z in SK's original spreadsheet // Generation 0
        public double Teqliab = 0;                                              // AA in SK's original spreadsheet // Generation 0
        public double Retainede = 0;                                            // AB in SK's original spreadsheet // Generation 0
        [Newtonsoft.Json.JsonIgnore] public double Wcta_AC { get { var v = IF(ISERROR((CurrAssets - CurrLiab) / TotalAssets), -999, (CurrAssets - CurrLiab) / TotalAssets); return (double.IsNaN(v) || double.IsInfinity(v)) ? double.NaN : v; } }    // IF(ISERROR((Q56-S56)/R56)," ",(Q56-S56)/R56) // Generation 1
        [Newtonsoft.Json.JsonIgnore] public double Cashcl_AD { get { var v = IF(ISERROR(CashEquiv / CurrLiab), -999, (CashEquiv / CurrLiab)); return (double.IsNaN(v) || double.IsInfinity(v)) ? double.NaN : v; } }    // IF(ISERROR(P56/S56)," ",(P56/S56)) // Generation 1
        [Newtonsoft.Json.JsonIgnore] public double Cashta_AE { get { var v = IF(ISERROR(CashEquiv / TotalAssets), -999, (CashEquiv / TotalAssets)); return (double.IsNaN(v) || double.IsInfinity(v)) ? double.NaN : v; } }    // IF(ISERROR(P56/R56)," ",(P56/R56)) // Generation 1
        [Newtonsoft.Json.JsonIgnore] public double Cashsale_AF { get { var v = IF(ISERROR(CashEquiv / Rev1), -999, (CashEquiv / Rev1)); return (double.IsNaN(v) || double.IsInfinity(v)) ? double.NaN : v; } }    // IF(ISERROR(P56/J56)," ",(P56/J56)) // Generation 1
        [Newtonsoft.Json.JsonIgnore] public double Ccratio_AG { get { var v = IF(ISERROR(CurrAssets / CurrLiab), -999, (CurrAssets / CurrLiab)); return (double.IsNaN(v) || double.IsInfinity(v)) ? double.NaN : v; } }    // IF(ISERROR(Q56/S56)," ",(Q56/S56)) // Generation 1
        [Newtonsoft.Json.JsonIgnore] public double Cata_AH { get { var v = IF(ISERROR(CurrAssets / TotalAssets), -999, (CurrAssets / TotalAssets)); return (double.IsNaN(v) || double.IsInfinity(v)) ? double.NaN : v; } }    // IF(ISERROR(Q56/R56)," ",(Q56/R56)) // Generation 1
        [Newtonsoft.Json.JsonIgnore] public double ClEq_AI { get { var v = IF(ISERROR(CurrLiab / TotalEquity), -999, (CurrLiab / TotalEquity)); return (double.IsNaN(v) || double.IsInfinity(v)) ? double.NaN : v; } }    // IF(ISERROR(S56/Y56)," ",(S56/Y56)) // Generation 1
        [Newtonsoft.Json.JsonIgnore] public double Qrat_AJ { get { var v = IF(ISERROR((CurrAssets - Inventory) / CurrLiab), -999, (CurrAssets - Inventory) / CurrLiab); return (double.IsNaN(v) || double.IsInfinity(v)) ? double.NaN : v; } }    // IF(ISERROR((Q56-V56)/S56)," ",(Q56-V56)/S56) // Generation 1
        [Newtonsoft.Json.JsonIgnore] public double Qta_AK { get { var v = IF(ISERROR((CurrAssets - Inventory) / TotalAssets), -999, (CurrAssets - Inventory) / TotalAssets); return (double.IsNaN(v) || double.IsInfinity(v)) ? double.NaN : v; } }    // IF(ISERROR((Q56-V56)/R56)," ",(Q56-V56)/R56) // Generation 1
        [Newtonsoft.Json.JsonIgnore] public double Invca_AL { get { var v = IF(ISERROR(Inventory / CurrAssets), -999, (Inventory / CurrAssets)); return (double.IsNaN(v) || double.IsInfinity(v)) ? double.NaN : v; } }    // IF(ISERROR(V56/Q56)," ",(V56/Q56)) // Generation 1
        [Newtonsoft.Json.JsonIgnore] public double Wcta_AM { get { var v = IF(ISERROR((PERCENTRANK(2, 2 + NumberOfCompanies - 1, 29, CompanyIndex + 2, 29))), -999, (PERCENTRANK(2, 2 + NumberOfCompanies - 1, 29, CompanyIndex + 2, 29))); return (double.IsNaN(v) || double.IsInfinity(v)) ? double.NaN : v; } }    // IF(ISERROR(PERCENTRANK($AC$56:$AC$67,AC56))," ", PERCENTRANK($AC$56:$AC$67,AC56)) // Generation 2
        [Newtonsoft.Json.JsonIgnore] public double Cashcl_AN { get { var v = IF(ISERROR((PERCENTRANK(2, 2 + NumberOfCompanies - 1, 30, CompanyIndex + 2, 30))), -999, (PERCENTRANK(2, 2 + NumberOfCompanies - 1, 30, CompanyIndex + 2, 30))); return (double.IsNaN(v) || double.IsInfinity(v)) ? double.NaN : v; } }    // IF(ISERROR(PERCENTRANK($AD$56:$AD$67,AD56))," ", PERCENTRANK($AD$56:$AD$67,AD56)) // Generation 2
        [Newtonsoft.Json.JsonIgnore] public double Cashta_AO { get { var v = IF(ISERROR((PERCENTRANK(2, 2 + NumberOfCompanies - 1, 31, CompanyIndex + 2, 31))), -999, (PERCENTRANK(2, 2 + NumberOfCompanies - 1, 31, CompanyIndex + 2, 31))); return (double.IsNaN(v) || double.IsInfinity(v)) ? double.NaN : v; } }    // IF(ISERROR(PERCENTRANK($AE$56:$AE$67,AE56))," ", PERCENTRANK($AE$56:$AE$67,AE56)) // Generation 2
        [Newtonsoft.Json.JsonIgnore] public double Cashsale_AP { get { var v = IF(ISERROR((PERCENTRANK(2, 2 + NumberOfCompanies - 1, 32, CompanyIndex + 2, 32))), -999, (PERCENTRANK(2, 2 + NumberOfCompanies - 1, 32, CompanyIndex + 2, 32))); return (double.IsNaN(v) || double.IsInfinity(v)) ? double.NaN : v; } }    // IF(ISERROR(PERCENTRANK($AF$56:$AF$67,AF56))," ", PERCENTRANK($AF$56:$AF$67,AF56)) // Generation 2
        [Newtonsoft.Json.JsonIgnore] public double Ccratio_AQ { get { var v = IF(ISERROR((PERCENTRANK(2, 2 + NumberOfCompanies - 1, 33, CompanyIndex + 2, 33))), -999, (PERCENTRANK(2, 2 + NumberOfCompanies - 1, 33, CompanyIndex + 2, 33))); return (double.IsNaN(v) || double.IsInfinity(v)) ? double.NaN : v; } }    // IF(ISERROR(PERCENTRANK($AG$56:$AG$67,AG56))," ", PERCENTRANK($AG$56:$AG$67,AG56)) // Generation 2
        [Newtonsoft.Json.JsonIgnore] public double Cata_AR { get { var v = IF(ISERROR((PERCENTRANK(2, 2 + NumberOfCompanies - 1, 34, CompanyIndex + 2, 34))), -999, (PERCENTRANK(2, 2 + NumberOfCompanies - 1, 34, CompanyIndex + 2, 34))); return (double.IsNaN(v) || double.IsInfinity(v)) ? double.NaN : v; } }    // IF(ISERROR(PERCENTRANK($AH$56:$AH$67,AH56))," ", PERCENTRANK($AH$56:$AH$67,AH56)) // Generation 2
        [Newtonsoft.Json.JsonIgnore] public double ClEq_AS { get { var v = IF(ISERROR(1 - (PERCENTRANK(2, 2 + NumberOfCompanies - 1, 35, CompanyIndex + 2, 35))), -999, 1 - (PERCENTRANK(2, 2 + NumberOfCompanies - 1, 35, CompanyIndex + 2, 35))); return (double.IsNaN(v) || double.IsInfinity(v)) ? double.NaN : v; } }    // IF(ISERROR(1-PERCENTRANK($AI$56:$AI$67,AI56))," ", 1-PERCENTRANK($AI$56:$AI$67,AI56)) // Generation 2
        [Newtonsoft.Json.JsonIgnore] public double Qrat_AT { get { var v = IF(ISERROR((PERCENTRANK(2, 2 + NumberOfCompanies - 1, 36, CompanyIndex + 2, 36))), -999, (PERCENTRANK(2, 2 + NumberOfCompanies - 1, 36, CompanyIndex + 2, 36))); return (double.IsNaN(v) || double.IsInfinity(v)) ? double.NaN : v; } }    // IF(ISERROR(PERCENTRANK($AJ$56:$AJ$67,AJ56))," ", PERCENTRANK($AJ$56:$AJ$67,AJ56)) // Generation 2
        [Newtonsoft.Json.JsonIgnore] public double Qta_AU { get { var v = IF(ISERROR((PERCENTRANK(2, 2 + NumberOfCompanies - 1, 37, CompanyIndex + 2, 37))), -999, (PERCENTRANK(2, 2 + NumberOfCompanies - 1, 37, CompanyIndex + 2, 37))); return (double.IsNaN(v) || double.IsInfinity(v)) ? double.NaN : v; } }    // IF(ISERROR(PERCENTRANK($AK$56:$AK$67,AK56))," ", PERCENTRANK($AK$56:$AK$67,AK56)) // Generation 2
        [Newtonsoft.Json.JsonIgnore] public double Invca_AV { get { var v = IF(ISERROR(1 - (PERCENTRANK(2, 2 + NumberOfCompanies - 1, 38, CompanyIndex + 2, 38))), -999, 1 - (PERCENTRANK(2, 2 + NumberOfCompanies - 1, 38, CompanyIndex + 2, 38))); return (double.IsNaN(v) || double.IsInfinity(v)) ? double.NaN : v; } }    // IF(ISERROR(1-PERCENTRANK($AL$56:$AL$67,AL56))," ", 1-PERCENTRANK($AL$56:$AL$67,AL56)) // Generation 2
        [Newtonsoft.Json.JsonIgnore] public double LiquidityScore { get { var v = SUM(Wcta_AM, Cashcl_AN, Cashta_AO, Cashsale_AP, Ccratio_AQ, Cata_AR, ClEq_AS, Qrat_AT, Qta_AU, Invca_AV) / COUNT(Wcta_AM, Cashcl_AN, Cashta_AO, Cashsale_AP, Ccratio_AQ, Cata_AR, ClEq_AS, Qrat_AT, Qta_AU, Invca_AV); return (double.IsNaN(v) || double.IsInfinity(v)) ? double.NaN : v; } }    // SUM(AM56,AN56,AO56,AP56,AQ56,AR56,AS56,AT56,AU56,AV56)/COUNT(AM56,AN56,AO56,AP56,AQ56,AR56,AS56,AT56,AU56,AV56) // Generation 3
        public double Dso = 0;                                                  // AX in SK's original spreadsheet // Generation 0
        [Newtonsoft.Json.JsonIgnore] public double Dio { get { var v = (Inventory / Cogs) * 365; return (double.IsNaN(v) || double.IsInfinity(v)) ? double.NaN : v; } }    // (V56/T56)*365 // Generation 1
        [Newtonsoft.Json.JsonIgnore] public double Dpo { get { var v = (Ap / Cogs) * 365; return (double.IsNaN(v) || double.IsInfinity(v)) ? double.NaN : v; } }    // (W56/T56)*365 // Generation 1
        [Newtonsoft.Json.JsonIgnore] public double Ccc { get { var v = Dso + Dio - Dpo; return (double.IsNaN(v) || double.IsInfinity(v)) ? double.NaN : v; } }    // AX56+AY56-AZ56 // Generation 2
        public double Ebitda1 = 0;                                              // BB in SK's original spreadsheet // Generation 0
        public double Em1 = 0;                                                  // BC in SK's original spreadsheet // Generation 0
        [Newtonsoft.Json.JsonIgnore] public double EbitdaPercentileRanking { get { var v = IF(ISERROR((PERCENTRANK(2, 2 + NumberOfCompanies - 1, 55, CompanyIndex + 2, 55))), -999, (PERCENTRANK(2, 2 + NumberOfCompanies - 1, 55, CompanyIndex + 2, 55))); return (double.IsNaN(v) || double.IsInfinity(v)) ? double.NaN : v; } }    // IF(ISERROR(PERCENTRANK($BC$56:$BC$67,BC56))," ", PERCENTRANK($BC$56:$BC$67,BC56)) // Generation 1
        public double Gp = 0;                                                   // BE in SK's original spreadsheet // Generation 0
        public double Ni = 0;                                                   // BF in SK's original spreadsheet // Generation 0
        public double Ebit = 0;                                                 // BG in SK's original spreadsheet // Generation 0
        public double Gppe = 0;                                                 // BH in SK's original spreadsheet // Generation 0
        public double Nppe = 0;                                                 // BI in SK's original spreadsheet // Generation 0
        [Newtonsoft.Json.JsonIgnore] public double Roa_BJ { get { var v = IF(ISERROR(Ni / TotalAssets), -999, (Ni / TotalAssets)); return (double.IsNaN(v) || double.IsInfinity(v)) ? double.NaN : v; } }    // IF(ISERROR(BF56/R56)," ",(BF56/R56)) // Generation 1
        [Newtonsoft.Json.JsonIgnore] public double Roe_BK { get { var v = IF(ISERROR(Ni / TotalEquity), -999, (Ni / TotalEquity)); return (double.IsNaN(v) || double.IsInfinity(v)) ? double.NaN : v; } }    // IF(ISERROR(BF56/Y56)," ",(BF56/Y56)) // Generation 1
        [Newtonsoft.Json.JsonIgnore] public double Gm1_BL { get { var v = IF(ISERROR(Gp / Rev1), -999, (Gp / Rev1)); return (double.IsNaN(v) || double.IsInfinity(v)) ? double.NaN : v; } }    // IF(ISERROR(BE56/J56)," ",(BE56/J56)) // Generation 1
        [Newtonsoft.Json.JsonIgnore] public double Np1_BM { get { var v = IF(ISERROR(Ni / Rev1), -999, (Ni / Rev1)); return (double.IsNaN(v) || double.IsInfinity(v)) ? double.NaN : v; } }    // IF(ISERROR(BF56/J56)," ",(BF56/J56)) // Generation 1
        [Newtonsoft.Json.JsonIgnore] public double Sgam_BN { get { var v = IF(ISERROR(Sga1 / Rev1), -999, (Sga1 / Rev1)); return (double.IsNaN(v) || double.IsInfinity(v)) ? double.NaN : v; } }    // IF(ISERROR(K56/J56)," ",(K56/J56)) // Generation 1
        [Newtonsoft.Json.JsonIgnore] public double Om1_BO { get { var v = IF(ISERROR(Ebit / Rev1), -999, (Ebit / Rev1)); return (double.IsNaN(v) || double.IsInfinity(v)) ? double.NaN : v; } }    // IF(ISERROR(BG56/J56)," ",(BG56/J56)) // Generation 1
        [Newtonsoft.Json.JsonIgnore] public double Ebitta_BP { get { var v = IF(ISERROR(Ebit / TotalAssets), -999, (Ebit / TotalAssets)); return (double.IsNaN(v) || double.IsInfinity(v)) ? double.NaN : v; } }    // IF(ISERROR(BG56/R56)," ",(BG56/R56)) // Generation 1
        [Newtonsoft.Json.JsonIgnore] public double Roa_BQ { get { var v = IF(ISERROR((PERCENTRANK(2, 2 + NumberOfCompanies - 1, 62, CompanyIndex + 2, 62))), -999, (PERCENTRANK(2, 2 + NumberOfCompanies - 1, 62, CompanyIndex + 2, 62))); return (double.IsNaN(v) || double.IsInfinity(v)) ? double.NaN : v; } }    // IF(ISERROR(PERCENTRANK($BJ$56:$BJ$67,BJ56))," ", PERCENTRANK($BJ$56:$BJ$67,BJ56)) // Generation 2
        [Newtonsoft.Json.JsonIgnore] public double Roe_BR { get { var v = IF(ISERROR((PERCENTRANK(2, 2 + NumberOfCompanies - 1, 63, CompanyIndex + 2, 63))), -999, (PERCENTRANK(2, 2 + NumberOfCompanies - 1, 63, CompanyIndex + 2, 63))); return (double.IsNaN(v) || double.IsInfinity(v)) ? double.NaN : v; } }    // IF(ISERROR(PERCENTRANK($BK$56:$BK$67,BK56))," ", PERCENTRANK($BK$56:$BK$67,BK56)) // Generation 2
        [Newtonsoft.Json.JsonIgnore] public double Gm1_BS { get { var v = IF(ISERROR((PERCENTRANK(2, 2 + NumberOfCompanies - 1, 64, CompanyIndex + 2, 64))), -999, (PERCENTRANK(2, 2 + NumberOfCompanies - 1, 64, CompanyIndex + 2, 64))); return (double.IsNaN(v) || double.IsInfinity(v)) ? double.NaN : v; } }    // IF(ISERROR(PERCENTRANK($BL$56:$BL$67,BL56))," ", PERCENTRANK($BL$56:$BL$67,BL56)) // Generation 2
        [Newtonsoft.Json.JsonIgnore] public double Np1_BT { get { var v = IF(ISERROR((PERCENTRANK(2, 2 + NumberOfCompanies - 1, 65, CompanyIndex + 2, 65))), -999, (PERCENTRANK(2, 2 + NumberOfCompanies - 1, 65, CompanyIndex + 2, 65))); return (double.IsNaN(v) || double.IsInfinity(v)) ? double.NaN : v; } }    // IF(ISERROR(PERCENTRANK($BM$56:$BM$67,BM56))," ", PERCENTRANK($BM$56:$BM$67,BM56)) // Generation 2
        [Newtonsoft.Json.JsonIgnore] public double Sgam_BU { get { var v = IF(ISERROR(1 - (PERCENTRANK(2, 2 + NumberOfCompanies - 1, 66, CompanyIndex + 2, 66))), -999, 1 - (PERCENTRANK(2, 2 + NumberOfCompanies - 1, 66, CompanyIndex + 2, 66))); return (double.IsNaN(v) || double.IsInfinity(v)) ? double.NaN : v; } }    // IF(ISERROR(1-PERCENTRANK($BN$56:$BN$67,BN56))," ", 1-PERCENTRANK($BN$56:$BN$67,BN56)) // Generation 2
        [Newtonsoft.Json.JsonIgnore] public double Om1_BV { get { var v = IF(ISERROR((PERCENTRANK(2, 2 + NumberOfCompanies - 1, 67, CompanyIndex + 2, 67))), -999, (PERCENTRANK(2, 2 + NumberOfCompanies - 1, 67, CompanyIndex + 2, 67))); return (double.IsNaN(v) || double.IsInfinity(v)) ? double.NaN : v; } }    // IF(ISERROR(PERCENTRANK($BO$56:$BO$67,BO56))," ", PERCENTRANK($BO$56:$BO$67,BO56)) // Generation 2
        [Newtonsoft.Json.JsonIgnore] public double Ebitta_BW { get { var v = IF(ISERROR((PERCENTRANK(2, 2 + NumberOfCompanies - 1, 68, CompanyIndex + 2, 68))), -999, (PERCENTRANK(2, 2 + NumberOfCompanies - 1, 68, CompanyIndex + 2, 68))); return (double.IsNaN(v) || double.IsInfinity(v)) ? double.NaN : v; } }    // IF(ISERROR(PERCENTRANK($BP$56:$BP$67,BP56))," ", PERCENTRANK($BP$56:$BP$67,BP56)) // Generation 2
        [Newtonsoft.Json.JsonIgnore] public double ProfitabilityScore { get { var v = SUM(Roa_BQ, Roe_BR, Gm1_BS, Np1_BT, Sgam_BU, Om1_BV, Ebitta_BW) / COUNT(Roa_BQ, Roe_BR, Gm1_BS, Np1_BT, Sgam_BU, Om1_BV, Ebitta_BW); return (double.IsNaN(v) || double.IsInfinity(v)) ? double.NaN : v; } }    // SUM(BQ56,BR56,BS56,BT56,BU56,BV56,BW56)/COUNT(BQ56,BR56,BS56,BT56,BU56,BV56,BW56) // Generation 3
        public double Wcap = 0;                                                 // BY in SK's original spreadsheet // Generation 0
        public double Wcap1 = 0;                                                // BZ in SK's original spreadsheet // Generation 0
        [Newtonsoft.Json.JsonIgnore] public double Wcsales { get { var v = Wcap / Rev1; return (double.IsNaN(v) || double.IsInfinity(v)) ? double.NaN : v; } }    // BY56/J56 // Generation 1
        [Newtonsoft.Json.JsonIgnore] public double AvgWorkingCapital { get { var v = Ar + Inventory - Ap; return (double.IsNaN(v) || double.IsInfinity(v)) ? double.NaN : v; } }    // U56+V56-W56 // Generation 1
        [Newtonsoft.Json.JsonIgnore] public double NetSales_Avg_Wcap_CC { get { var v = IF(ISERROR(Rev1 / AvgWorkingCapital), -999, (Rev1 / AvgWorkingCapital)); return (double.IsNaN(v) || double.IsInfinity(v)) ? double.NaN : v; } }    // IF(ISERROR(J56/CB56)," ",(J56/CB56)) // Generation 2
        [Newtonsoft.Json.JsonIgnore] public double Iturns_CD { get { var v = IF(ISERROR(Cogs / Inventory), -999, (Cogs / Inventory)); return (double.IsNaN(v) || double.IsInfinity(v)) ? double.NaN : v; } }    // IF(ISERROR(T56/V56)," ",(T56/V56)) // Generation 1
        [Newtonsoft.Json.JsonIgnore] public double Faturn_CE { get { var v = IF(ISERROR(Rev1 / Nppe), -999, (Rev1 / Nppe)); return (double.IsNaN(v) || double.IsInfinity(v)) ? double.NaN : v; } }    // IF(ISERROR(J56/BI56)," ",(J56/BI56)) // Generation 1
        [Newtonsoft.Json.JsonIgnore] public double Taturn_CF { get { var v = IF(ISERROR(Rev1 / TotalAssets), -999, (Rev1 / TotalAssets)); return (double.IsNaN(v) || double.IsInfinity(v)) ? double.NaN : v; } }    // IF(ISERROR(J56/R56)," ",(J56/R56)) // Generation 1
        [Newtonsoft.Json.JsonIgnore] public double Eqturns_CG { get { var v = IF(ISERROR(Rev1 / TotalEquity), -999, (Rev1 / TotalEquity)); return (double.IsNaN(v) || double.IsInfinity(v)) ? double.NaN : v; } }    // IF(ISERROR(J56/Y56)," ",(J56/Y56)) // Generation 1
        [Newtonsoft.Json.JsonIgnore] public double Invsales_CH { get { var v = IF(ISERROR(Inventory / Rev1), -999, (Inventory / Rev1)); return (double.IsNaN(v) || double.IsInfinity(v)) ? double.NaN : v; } }    // IF(ISERROR(V56/J56)," ",(V56/J56)) // Generation 1
        [Newtonsoft.Json.JsonIgnore] public double Rturns_CI { get { var v = IF(ISERROR(Rev1 / Ar), -999, (Rev1 / Ar)); return (double.IsNaN(v) || double.IsInfinity(v)) ? double.NaN : v; } }    // IF(ISERROR(J56/U56)," ",(J56/U56)) // Generation 1
        [Newtonsoft.Json.JsonIgnore] public double Qasales_CJ { get { var v = IF(ISERROR((CurrAssets - Inventory) / Rev1), -999, (CurrAssets - Inventory) / Rev1); return (double.IsNaN(v) || double.IsInfinity(v)) ? double.NaN : v; } }    // IF(ISERROR((Q56-V56)/J56)," ",(Q56-V56)/J56) // Generation 1
        [Newtonsoft.Json.JsonIgnore] public double Casales_CK { get { var v = IF(ISERROR(Rev1 / CurrAssets), -999, (Rev1 / CurrAssets)); return (double.IsNaN(v) || double.IsInfinity(v)) ? double.NaN : v; } }    // IF(ISERROR(J56/Q56)," ",(J56/Q56)) // Generation 1
        [Newtonsoft.Json.JsonIgnore] public double NetSales_Avg_Wcap_CL { get { var v = IF(ISERROR((PERCENTRANK(2, 2 + NumberOfCompanies - 1, 81, CompanyIndex + 2, 81))), -999, (PERCENTRANK(2, 2 + NumberOfCompanies - 1, 81, CompanyIndex + 2, 81))); return (double.IsNaN(v) || double.IsInfinity(v)) ? double.NaN : v; } }    // IF(ISERROR(PERCENTRANK($CC$56:$CC$67,CC56))," ", PERCENTRANK($CC$56:$CC$67,CC56)) // Generation 3
        [Newtonsoft.Json.JsonIgnore] public double Iturns_CM { get { var v = IF(ISERROR((PERCENTRANK(2, 2 + NumberOfCompanies - 1, 82, CompanyIndex + 2, 82))), -999, (PERCENTRANK(2, 2 + NumberOfCompanies - 1, 82, CompanyIndex + 2, 82))); return (double.IsNaN(v) || double.IsInfinity(v)) ? double.NaN : v; } }    // IF(ISERROR(PERCENTRANK($CD$56:$CD$67,CD56))," ", PERCENTRANK($CD$56:$CD$67,CD56)) // Generation 2
        [Newtonsoft.Json.JsonIgnore] public double Faturn_CN { get { var v = IF(ISERROR((PERCENTRANK(2, 2 + NumberOfCompanies - 1, 83, CompanyIndex + 2, 83))), -999, (PERCENTRANK(2, 2 + NumberOfCompanies - 1, 83, CompanyIndex + 2, 83))); return (double.IsNaN(v) || double.IsInfinity(v)) ? double.NaN : v; } }    // IF(ISERROR(PERCENTRANK($CE$56:$CE$67,CE56))," ", PERCENTRANK($CE$56:$CE$67,CE56)) // Generation 2
        [Newtonsoft.Json.JsonIgnore] public double Taturn_CO { get { var v = IF(ISERROR((PERCENTRANK(2, 2 + NumberOfCompanies - 1, 84, CompanyIndex + 2, 84))), -999, (PERCENTRANK(2, 2 + NumberOfCompanies - 1, 84, CompanyIndex + 2, 84))); return (double.IsNaN(v) || double.IsInfinity(v)) ? double.NaN : v; } }    // IF(ISERROR(PERCENTRANK($CF$56:$CF$67,CF56))," ", PERCENTRANK($CF$56:$CF$67,CF56)) // Generation 2
        [Newtonsoft.Json.JsonIgnore] public double Eqturns_CP { get { var v = IF(ISERROR((PERCENTRANK(2, 2 + NumberOfCompanies - 1, 85, CompanyIndex + 2, 85))), -999, (PERCENTRANK(2, 2 + NumberOfCompanies - 1, 85, CompanyIndex + 2, 85))); return (double.IsNaN(v) || double.IsInfinity(v)) ? double.NaN : v; } }    // IF(ISERROR(PERCENTRANK($CG$56:$CG$67,CG56))," ", PERCENTRANK($CG$56:$CG$67,CG56)) // Generation 2
        [Newtonsoft.Json.JsonIgnore] public double Invsales_CQ { get { var v = IF(ISERROR(1 - (PERCENTRANK(2, 2 + NumberOfCompanies - 1, 86, CompanyIndex + 2, 86))), -999, 1 - (PERCENTRANK(2, 2 + NumberOfCompanies - 1, 86, CompanyIndex + 2, 86))); return (double.IsNaN(v) || double.IsInfinity(v)) ? double.NaN : v; } }    // IF(ISERROR(1-PERCENTRANK($CH$56:$CH$67,CH56))," ", 1-PERCENTRANK($CH$56:$CH$67,CH56)) // Generation 2
        [Newtonsoft.Json.JsonIgnore] public double Rturns_CR { get { var v = IF(ISERROR((PERCENTRANK(2, 2 + NumberOfCompanies - 1, 87, CompanyIndex + 2, 87))), -999, (PERCENTRANK(2, 2 + NumberOfCompanies - 1, 87, CompanyIndex + 2, 87))); return (double.IsNaN(v) || double.IsInfinity(v)) ? double.NaN : v; } }    // IF(ISERROR(PERCENTRANK($CI$56:$CI$67,CI56))," ", PERCENTRANK($CI$56:$CI$67,CI56)) // Generation 2
        [Newtonsoft.Json.JsonIgnore] public double Qasales_CS { get { var v = IF(ISERROR((PERCENTRANK(2, 2 + NumberOfCompanies - 1, 88, CompanyIndex + 2, 88))), -999, (PERCENTRANK(2, 2 + NumberOfCompanies - 1, 88, CompanyIndex + 2, 88))); return (double.IsNaN(v) || double.IsInfinity(v)) ? double.NaN : v; } }    // IF(ISERROR(PERCENTRANK($CJ$56:$CJ$67,CJ56))," ", PERCENTRANK($CJ$56:$CJ$67,CJ56)) // Generation 2
        [Newtonsoft.Json.JsonIgnore] public double Casales_CT { get { var v = IF(ISERROR((PERCENTRANK(2, 2 + NumberOfCompanies - 1, 89, CompanyIndex + 2, 89))), -999, (PERCENTRANK(2, 2 + NumberOfCompanies - 1, 89, CompanyIndex + 2, 89))); return (double.IsNaN(v) || double.IsInfinity(v)) ? double.NaN : v; } }    // IF(ISERROR(PERCENTRANK($CK$56:$CK$67,CK56))," ", PERCENTRANK($CK$56:$CK$67,CK56)) // Generation 2
        [Newtonsoft.Json.JsonIgnore] public double TurnoverScore { get { var v = SUM(NetSales_Avg_Wcap_CL, Iturns_CM, Faturn_CN, Taturn_CO, Eqturns_CP, Invsales_CQ, Rturns_CR, Qasales_CS, Casales_CT) / COUNT(NetSales_Avg_Wcap_CL, Iturns_CM, Faturn_CN, Taturn_CO, Eqturns_CP, Invsales_CQ, Rturns_CR, Qasales_CS, Casales_CT); return (double.IsNaN(v) || double.IsInfinity(v)) ? double.NaN : v; } }    // SUM(CL56,CM56,CN56,CO56,CP56,CQ56,CR56,CS56,CT56)/COUNT(CL56,CM56,CN56,CO56,CP56,CQ56,CR56,CS56,CT56) // Generation 4
        [Newtonsoft.Json.JsonIgnore] public double Dbteq_CV { get { var v = IF(ISERROR(TLiab / TotalEquity), -999, (TLiab / TotalEquity)); return (double.IsNaN(v) || double.IsInfinity(v)) ? double.NaN : v; } }    // IF(ISERROR(X56/Y56)," ",(X56/Y56)) // Generation 1
        [Newtonsoft.Json.JsonIgnore] public double Dbtta_CW { get { var v = IF(ISERROR(TLiab / TotalAssets), -999, (TLiab / TotalAssets)); return (double.IsNaN(v) || double.IsInfinity(v)) ? double.NaN : v; } }    // IF(ISERROR(X56/R56)," ",(X56/R56)) // Generation 1
        [Newtonsoft.Json.JsonIgnore] public double Fnmulti_CX { get { var v = IF(ISERROR(TotalAssets / TotalEquity), -999, (TotalAssets / TotalEquity)); return (double.IsNaN(v) || double.IsInfinity(v)) ? double.NaN : v; } }    // IF(ISERROR(R56/Y56)," ",(R56/Y56)) // Generation 1
        [Newtonsoft.Json.JsonIgnore] public double Faeqltl_CY { get { var v = IF(ISERROR(Nppe / (TLiab + TotalEquity)), -999, (Nppe / (TLiab + TotalEquity))); return (double.IsNaN(v) || double.IsInfinity(v)) ? double.NaN : v; } }    // IF(ISERROR(BI56/(X56+Y56))," ",(BI56/(X56+Y56))) // Generation 1
        [Newtonsoft.Json.JsonIgnore] public double Reta_CZ { get { var v = IF(ISERROR(Retainede / TotalAssets), -999, (Retainede / TotalAssets)); return (double.IsNaN(v) || double.IsInfinity(v)) ? double.NaN : v; } }    // IF(ISERROR(AB56/R56)," ",(AB56/R56)) // Generation 1
        [Newtonsoft.Json.JsonIgnore] public double Dbteq_DA { get { var v = IF(ISERROR(1 - (PERCENTRANK(2, 2 + NumberOfCompanies - 1, 100, CompanyIndex + 2, 100))), -999, 1 - (PERCENTRANK(2, 2 + NumberOfCompanies - 1, 100, CompanyIndex + 2, 100))); return (double.IsNaN(v) || double.IsInfinity(v)) ? double.NaN : v; } }    // IF(ISERROR(1-PERCENTRANK($CV$56:$CV$67,CV56))," ", 1-PERCENTRANK($CV$56:$CV$67,CV56)) // Generation 2
        [Newtonsoft.Json.JsonIgnore] public double Dbtta_DB { get { var v = IF(ISERROR(1 - (PERCENTRANK(2, 2 + NumberOfCompanies - 1, 101, CompanyIndex + 2, 101))), -999, 1 - (PERCENTRANK(2, 2 + NumberOfCompanies - 1, 101, CompanyIndex + 2, 101))); return (double.IsNaN(v) || double.IsInfinity(v)) ? double.NaN : v; } }    // IF(ISERROR(1-PERCENTRANK($CW$56:$CW$67,CW56))," ", 1-PERCENTRANK($CW$56:$CW$67,CW56)) // Generation 2
        [Newtonsoft.Json.JsonIgnore] public double Fnmulti_DC { get { var v = IF(ISERROR(1 - (PERCENTRANK(2, 2 + NumberOfCompanies - 1, 102, CompanyIndex + 2, 102))), -999, 1 - (PERCENTRANK(2, 2 + NumberOfCompanies - 1, 102, CompanyIndex + 2, 102))); return (double.IsNaN(v) || double.IsInfinity(v)) ? double.NaN : v; } }    // IF(ISERROR(1-PERCENTRANK($CX$56:$CX$67,CX56))," ", 1-PERCENTRANK($CX$56:$CX$67,CX56)) // Generation 2
        [Newtonsoft.Json.JsonIgnore] public double Faeqltl_DD { get { var v = IF(ISERROR((PERCENTRANK(2, 2 + NumberOfCompanies - 1, 103, CompanyIndex + 2, 103))), -999, (PERCENTRANK(2, 2 + NumberOfCompanies - 1, 103, CompanyIndex + 2, 103))); return (double.IsNaN(v) || double.IsInfinity(v)) ? double.NaN : v; } }    // IF(ISERROR(PERCENTRANK($CY$56:$CY$67,CY56))," ", PERCENTRANK($CY$56:$CY$67,CY56)) // Generation 2
        [Newtonsoft.Json.JsonIgnore] public double Reta_DE { get { var v = IF(ISERROR((PERCENTRANK(2, 2 + NumberOfCompanies - 1, 104, CompanyIndex + 2, 104))), -999, (PERCENTRANK(2, 2 + NumberOfCompanies - 1, 104, CompanyIndex + 2, 104))); return (double.IsNaN(v) || double.IsInfinity(v)) ? double.NaN : v; } }    // IF(ISERROR(PERCENTRANK($CZ$56:$CZ$67,CZ56))," ", PERCENTRANK($CZ$56:$CZ$67,CZ56)) // Generation 2
        [Newtonsoft.Json.JsonIgnore] public double LeverageScore { get { var v = SUM(Dbteq_DA, Dbtta_DB, Fnmulti_DC, Faeqltl_DD, Reta_DE) / COUNT(Dbteq_DA, Dbtta_DB, Fnmulti_DC, Faeqltl_DD, Reta_DE); return (double.IsNaN(v) || double.IsInfinity(v)) ? double.NaN : v; } }    // SUM(DA56,DB56,DC56,DD56,DE56)/COUNT(DA56,DB56,DC56,DD56,DE56) // Generation 3
        [Newtonsoft.Json.JsonIgnore] public double CompositeScore { get { var v = SUM(LiquidityScore, ProfitabilityScore, TurnoverScore, LeverageScore) / COUNT(LiquidityScore, ProfitabilityScore, TurnoverScore, LeverageScore); return (double.IsNaN(v) || double.IsInfinity(v)) ? double.NaN : v; } }    // SUM(AW56,BX56,CU56,DF56)/COUNT(AW56,BX56,CU56,DF56) // Generation 5
        public double Createdby = 0;                                            // DH in SK's original spreadsheet // Generation 0
        public double Createdon = 0;                                            // DI in SK's original spreadsheet // Generation 0
        public double Updatedby = 0;                                            // DJ in SK's original spreadsheet // Generation 0
        [Newtonsoft.Json.JsonIgnore] public string Updatedon { get { try { return DateTime.Now.ToString(); } catch { return string.Empty; } } }    // DateTime.Now.ToString() // Generation 0
        public double Datayear = 0;                                             // DL in SK's original spreadsheet // Generation 0
        public double Uid_DM = 0;                                               // DM in SK's original spreadsheet // Generation 0
        [Newtonsoft.Json.JsonIgnore] public double OneDaySales { get { var v = Rev1 / 365; return (double.IsNaN(v) || double.IsInfinity(v)) ? double.NaN : v; } }    // J56/365 // Generation 1
        [Newtonsoft.Json.JsonIgnore] public double OneDayCogs { get { var v = Cogs / 365; return (double.IsNaN(v) || double.IsInfinity(v)) ? double.NaN : v; } }    // T56/365 // Generation 1
        [Newtonsoft.Json.JsonIgnore] public double DsoUq { get { var v = CalculateDsoUq(); return (double.IsNaN(v) || double.IsInfinity(v)) ? double.NaN : v; } }    // CalculateDsoUq() // Generation 10
        [Newtonsoft.Json.JsonIgnore] public double DpoUq { get { var v = CalculateDpoUq(); return (double.IsNaN(v) || double.IsInfinity(v)) ? double.NaN : v; } }    // CalculateDpoUq() // Generation 10
        [Newtonsoft.Json.JsonIgnore] public double DioUq { get { var v = CalculateDioUq(); return (double.IsNaN(v) || double.IsInfinity(v)) ? double.NaN : v; } }    // CalculateDioUq() // Generation 10
        [Newtonsoft.Json.JsonIgnore] public double Dso_Sales_ { get { var v = IF(Dso - DsoUq > 0, (Dso - DsoUq) * OneDaySales, 0); return (double.IsNaN(v) || double.IsInfinity(v)) ? double.NaN : v; } }    // IF(AX56-DP56>0,(AX56-DP56)*DN56,0) // Generation 11
        [Newtonsoft.Json.JsonIgnore] public double Dpo_Cogs_ { get { var v = IF(DpoUq - Dpo > 0, (DpoUq - Dpo) * OneDayCogs, 0); return (double.IsNaN(v) || double.IsInfinity(v)) ? double.NaN : v; } }    // IF(DQ56-AZ56>0,(DQ56-AZ56)*DO56,0) // Generation 11
        [Newtonsoft.Json.JsonIgnore] public double Dio_Cogs_ { get { var v = IF(Dio - DioUq > 0, (Dio - DioUq) * OneDayCogs, 0); return (double.IsNaN(v) || double.IsInfinity(v)) ? double.NaN : v; } }    // IF(AY56-DR56>0,(AY56-DR56)*DO56,0) // Generation 11
        [Newtonsoft.Json.JsonIgnore] public double Twc_BasedOnCogs_ { get { var v = (Dso_Sales_ + Dpo_Cogs_ + Dio_Cogs_); return (double.IsNaN(v) || double.IsInfinity(v)) ? double.NaN : v; } }    // SUM(DS56:DU56) // Generation 12
        [Newtonsoft.Json.JsonIgnore] public double ActualRoce { get { var v = Ebit / CapitalEmployed_Ta_Cl_; return (double.IsNaN(v) || double.IsInfinity(v)) ? double.NaN : v; } }    // BG56/DX56 // Generation 2
        [Newtonsoft.Json.JsonIgnore] public double CapitalEmployed_Ta_Cl_ { get { var v = TotalAssets - CurrLiab; return (double.IsNaN(v) || double.IsInfinity(v)) ? double.NaN : v; } }    // R56-S56 // Generation 1
        [Newtonsoft.Json.JsonIgnore] public double NewRoceWithTwcImpprovement { get { var v = IF(OR(ActualRoce < 0, Twc_BasedOnCogs_ > CapitalEmployed_Ta_Cl_), -999, Ebit / (CapitalEmployed_Ta_Cl_ - Ebit)); return (double.IsNaN(v) || double.IsInfinity(v)) ? double.NaN : v; } }    // IF(OR(DW56<0,DV56>DX56),"N/M",BG56/(DX56-BG56)) // Generation 13
        [Newtonsoft.Json.JsonIgnore] public double TwcScope { get { var v = Twc_BasedOnCogs_ / (Ar + Inventory + Ap); return (double.IsNaN(v) || double.IsInfinity(v)) ? double.NaN : v; } }    // DV56/SUM(U56:W56) // Generation 13
        public bool IsTarget = false;                                           // EA in SK's original spreadsheet // Generation 0
        public string ShortName = null;                                         // EB in SK's original spreadsheet // Generation 0
        public double CAGR = 0;                                                 // EC in SK's original spreadsheet // Generation 0
        [Newtonsoft.Json.JsonIgnore] public double OP1 { get { var v = Om1_BO; return (double.IsNaN(v) || double.IsInfinity(v)) ? double.NaN : v; } }    // Om1_BO // Generation 0
        public string Sic2DDescription = null;                                  // EE in SK's original spreadsheet // Generation 0
        public string SubIndustry = null;                                       // EF in SK's original spreadsheet // Generation 0
        public string Country = null;                                           // EG in SK's original spreadsheet // Generation 0
        public string CompanyNameMixedCase = null;                              // EH in SK's original spreadsheet // Generation 0
        public string ShortNameMixedCase = null;                                // EI in SK's original spreadsheet // Generation 0

        public const int MaxGeneration = 14;

        public static int ColumnGeneration(int columnNumber)
        {
            switch (columnNumber)
            {
                case 1: return 0;    // SqlQry =
                case 2: return 0;    // Factid =
                case 3: return 0;    // Coname =
                case 4: return 0;    // Uid_D =
                case 5: return 0;    // Sic2D =
                case 6: return 0;    // SicCode =
                case 7: return 0;    // SgaMult =
                case 8: return 0;    // Yscid =
                case 9: return 0;    // CikId =
                case 10: return 0;    // Rev1 =
                case 11: return 0;    // Sga1 =
                case 12: return 0;    // Ga1M =
                case 13: return 0;    // Sgm1 =
                case 14: return 0;    // Ee =
                case 15: return 0;    // Revperee =
                case 16: return 0;    // CashEquiv =
                case 17: return 0;    // CurrAssets =
                case 18: return 0;    // TotalAssets =
                case 19: return 0;    // CurrLiab =
                case 20: return 0;    // Cogs =
                case 21: return 0;    // Ar =
                case 22: return 0;    // Inventory =
                case 23: return 0;    // Ap =
                case 24: return 0;    // TLiab =
                case 25: return 0;    // TotalEquity =
                case 26: return 0;    // Mktcap =
                case 27: return 0;    // Teqliab =
                case 28: return 0;    // Retainede =
                case 29: return 1;    // Wcta_AC = IF(ISERROR((CurrAssets-CurrLiab)/TotalAssets),-999,(CurrAssets-CurrLiab)/TotalAssets)
                case 30: return 1;    // Cashcl_AD = IF(ISERROR(CashEquiv/CurrLiab),-999,(CashEquiv/CurrLiab))
                case 31: return 1;    // Cashta_AE = IF(ISERROR(CashEquiv/TotalAssets),-999,(CashEquiv/TotalAssets))
                case 32: return 1;    // Cashsale_AF = IF(ISERROR(CashEquiv/Rev1),-999,(CashEquiv/Rev1))
                case 33: return 1;    // Ccratio_AG = IF(ISERROR(CurrAssets/CurrLiab),-999,(CurrAssets/CurrLiab))
                case 34: return 1;    // Cata_AH = IF(ISERROR(CurrAssets/TotalAssets),-999,(CurrAssets/TotalAssets))
                case 35: return 1;    // ClEq_AI = IF(ISERROR(CurrLiab/TotalEquity),-999,(CurrLiab/TotalEquity))
                case 36: return 1;    // Qrat_AJ = IF(ISERROR((CurrAssets-Inventory)/CurrLiab),-999,(CurrAssets-Inventory)/CurrLiab)
                case 37: return 1;    // Qta_AK = IF(ISERROR((CurrAssets-Inventory)/TotalAssets),-999,(CurrAssets-Inventory)/TotalAssets)
                case 38: return 1;    // Invca_AL = IF(ISERROR(Inventory/CurrAssets),-999,(Inventory/CurrAssets))
                case 39: return 2;    // Wcta_AM = IF(ISERROR((PERCENTRANK(2,2 + NumberOfCompanies - 1,29, CompanyIndex+2,29))),-999, (PERCENTRANK(2,2 + NumberOfCompanies - 1,29, CompanyIndex+2,29)))
                case 40: return 2;    // Cashcl_AN = IF(ISERROR((PERCENTRANK(2,2 + NumberOfCompanies - 1,30, CompanyIndex+2,30))),-999, (PERCENTRANK(2,2 + NumberOfCompanies - 1,30, CompanyIndex+2,30)))
                case 41: return 2;    // Cashta_AO = IF(ISERROR((PERCENTRANK(2,2 + NumberOfCompanies - 1,31, CompanyIndex+2,31))),-999, (PERCENTRANK(2,2 + NumberOfCompanies - 1,31, CompanyIndex+2,31)))
                case 42: return 2;    // Cashsale_AP = IF(ISERROR((PERCENTRANK(2,2 + NumberOfCompanies - 1,32, CompanyIndex+2,32))),-999, (PERCENTRANK(2,2 + NumberOfCompanies - 1,32, CompanyIndex+2,32)))
                case 43: return 2;    // Ccratio_AQ = IF(ISERROR((PERCENTRANK(2,2 + NumberOfCompanies - 1,33, CompanyIndex+2,33))),-999, (PERCENTRANK(2,2 + NumberOfCompanies - 1,33, CompanyIndex+2,33)))
                case 44: return 2;    // Cata_AR = IF(ISERROR((PERCENTRANK(2,2 + NumberOfCompanies - 1,34, CompanyIndex+2,34))),-999, (PERCENTRANK(2,2 + NumberOfCompanies - 1,34, CompanyIndex+2,34)))
                case 45: return 2;    // ClEq_AS = IF(ISERROR(1-(PERCENTRANK(2,2 + NumberOfCompanies - 1,35, CompanyIndex+2,35))),-999, 1-(PERCENTRANK(2,2 + NumberOfCompanies - 1,35, CompanyIndex+2,35)))
                case 46: return 2;    // Qrat_AT = IF(ISERROR((PERCENTRANK(2,2 + NumberOfCompanies - 1,36, CompanyIndex+2,36))),-999, (PERCENTRANK(2,2 + NumberOfCompanies - 1,36, CompanyIndex+2,36)))
                case 47: return 2;    // Qta_AU = IF(ISERROR((PERCENTRANK(2,2 + NumberOfCompanies - 1,37, CompanyIndex+2,37))),-999, (PERCENTRANK(2,2 + NumberOfCompanies - 1,37, CompanyIndex+2,37)))
                case 48: return 2;    // Invca_AV = IF(ISERROR(1-(PERCENTRANK(2,2 + NumberOfCompanies - 1,38, CompanyIndex+2,38))),-999, 1-(PERCENTRANK(2,2 + NumberOfCompanies - 1,38, CompanyIndex+2,38)))
                case 49: return 3;    // LiquidityScore = SUM(Wcta_AM,Cashcl_AN,Cashta_AO,Cashsale_AP,Ccratio_AQ,Cata_AR,ClEq_AS,Qrat_AT,Qta_AU,Invca_AV)/COUNT(Wcta_AM,Cashcl_AN,Cashta_AO,Cashsale_AP,Ccratio_AQ,Cata_AR,ClEq_AS,Qrat_AT,Qta_AU,Invca_AV)
                case 50: return 0;    // Dso =
                case 51: return 1;    // Dio = (Inventory/Cogs)*365
                case 52: return 1;    // Dpo = (Ap/Cogs)*365
                case 53: return 2;    // Ccc = Dso+Dio-Dpo
                case 54: return 0;    // Ebitda1 =
                case 55: return 0;    // Em1 =
                case 56: return 1;    // EbitdaPercentileRanking = IF(ISERROR((PERCENTRANK(2,2 + NumberOfCompanies - 1,55, CompanyIndex+2,55))),-999, (PERCENTRANK(2,2 + NumberOfCompanies - 1,55, CompanyIndex+2,55)))
                case 57: return 0;    // Gp =
                case 58: return 0;    // Ni =
                case 59: return 0;    // Ebit =
                case 60: return 0;    // Gppe =
                case 61: return 0;    // Nppe =
                case 62: return 1;    // Roa_BJ = IF(ISERROR(Ni/TotalAssets),-999,(Ni/TotalAssets))
                case 63: return 1;    // Roe_BK = IF(ISERROR(Ni/TotalEquity),-999,(Ni/TotalEquity))
                case 64: return 1;    // Gm1_BL = IF(ISERROR(Gp/Rev1),-999,(Gp/Rev1))
                case 65: return 1;    // Np1_BM = IF(ISERROR(Ni/Rev1),-999,(Ni/Rev1))
                case 66: return 1;    // Sgam_BN = IF(ISERROR(Sga1/Rev1),-999,(Sga1/Rev1))
                case 67: return 1;    // Om1_BO = IF(ISERROR(Ebit/Rev1),-999,(Ebit/Rev1))
                case 68: return 1;    // Ebitta_BP = IF(ISERROR(Ebit/TotalAssets),-999,(Ebit/TotalAssets))
                case 69: return 2;    // Roa_BQ = IF(ISERROR((PERCENTRANK(2,2 + NumberOfCompanies - 1,62, CompanyIndex+2,62))),-999, (PERCENTRANK(2,2 + NumberOfCompanies - 1,62, CompanyIndex+2,62)))
                case 70: return 2;    // Roe_BR = IF(ISERROR((PERCENTRANK(2,2 + NumberOfCompanies - 1,63, CompanyIndex+2,63))),-999, (PERCENTRANK(2,2 + NumberOfCompanies - 1,63, CompanyIndex+2,63)))
                case 71: return 2;    // Gm1_BS = IF(ISERROR((PERCENTRANK(2,2 + NumberOfCompanies - 1,64, CompanyIndex+2,64))),-999, (PERCENTRANK(2,2 + NumberOfCompanies - 1,64, CompanyIndex+2,64)))
                case 72: return 2;    // Np1_BT = IF(ISERROR((PERCENTRANK(2,2 + NumberOfCompanies - 1,65, CompanyIndex+2,65))),-999, (PERCENTRANK(2,2 + NumberOfCompanies - 1,65, CompanyIndex+2,65)))
                case 73: return 2;    // Sgam_BU = IF(ISERROR(1-(PERCENTRANK(2,2 + NumberOfCompanies - 1,66, CompanyIndex+2,66))),-999, 1-(PERCENTRANK(2,2 + NumberOfCompanies - 1,66, CompanyIndex+2,66)))
                case 74: return 2;    // Om1_BV = IF(ISERROR((PERCENTRANK(2,2 + NumberOfCompanies - 1,67, CompanyIndex+2,67))),-999, (PERCENTRANK(2,2 + NumberOfCompanies - 1,67, CompanyIndex+2,67)))
                case 75: return 2;    // Ebitta_BW = IF(ISERROR((PERCENTRANK(2,2 + NumberOfCompanies - 1,68, CompanyIndex+2,68))),-999, (PERCENTRANK(2,2 + NumberOfCompanies - 1,68, CompanyIndex+2,68)))
                case 76: return 3;    // ProfitabilityScore = SUM(Roa_BQ,Roe_BR,Gm1_BS,Np1_BT,Sgam_BU,Om1_BV,Ebitta_BW)/COUNT(Roa_BQ,Roe_BR,Gm1_BS,Np1_BT,Sgam_BU,Om1_BV,Ebitta_BW)
                case 77: return 0;    // Wcap =
                case 78: return 0;    // Wcap1 =
                case 79: return 1;    // Wcsales = Wcap/Rev1
                case 80: return 1;    // AvgWorkingCapital = Ar+Inventory-Ap
                case 81: return 2;    // NetSales_Avg_Wcap_CC = IF(ISERROR(Rev1/AvgWorkingCapital),-999,(Rev1/AvgWorkingCapital))
                case 82: return 1;    // Iturns_CD = IF(ISERROR(Cogs/Inventory),-999,(Cogs/Inventory))
                case 83: return 1;    // Faturn_CE = IF(ISERROR(Rev1/Nppe),-999,(Rev1/Nppe))
                case 84: return 1;    // Taturn_CF = IF(ISERROR(Rev1/TotalAssets),-999,(Rev1/TotalAssets))
                case 85: return 1;    // Eqturns_CG = IF(ISERROR(Rev1/TotalEquity),-999,(Rev1/TotalEquity))
                case 86: return 1;    // Invsales_CH = IF(ISERROR(Inventory/Rev1),-999,(Inventory/Rev1))
                case 87: return 1;    // Rturns_CI = IF(ISERROR(Rev1/Ar),-999,(Rev1/Ar))
                case 88: return 1;    // Qasales_CJ = IF(ISERROR((CurrAssets-Inventory)/Rev1),-999,(CurrAssets-Inventory)/Rev1)
                case 89: return 1;    // Casales_CK = IF(ISERROR(Rev1/CurrAssets),-999,(Rev1/CurrAssets))
                case 90: return 3;    // NetSales_Avg_Wcap_CL = IF(ISERROR((PERCENTRANK(2,2 + NumberOfCompanies - 1,81, CompanyIndex+2,81))),-999, (PERCENTRANK(2,2 + NumberOfCompanies - 1,81, CompanyIndex+2,81)))
                case 91: return 2;    // Iturns_CM = IF(ISERROR((PERCENTRANK(2,2 + NumberOfCompanies - 1,82, CompanyIndex+2,82))),-999, (PERCENTRANK(2,2 + NumberOfCompanies - 1,82, CompanyIndex+2,82)))
                case 92: return 2;    // Faturn_CN = IF(ISERROR((PERCENTRANK(2,2 + NumberOfCompanies - 1,83, CompanyIndex+2,83))),-999, (PERCENTRANK(2,2 + NumberOfCompanies - 1,83, CompanyIndex+2,83)))
                case 93: return 2;    // Taturn_CO = IF(ISERROR((PERCENTRANK(2,2 + NumberOfCompanies - 1,84, CompanyIndex+2,84))),-999, (PERCENTRANK(2,2 + NumberOfCompanies - 1,84, CompanyIndex+2,84)))
                case 94: return 2;    // Eqturns_CP = IF(ISERROR((PERCENTRANK(2,2 + NumberOfCompanies - 1,85, CompanyIndex+2,85))),-999, (PERCENTRANK(2,2 + NumberOfCompanies - 1,85, CompanyIndex+2,85)))
                case 95: return 2;    // Invsales_CQ = IF(ISERROR(1-(PERCENTRANK(2,2 + NumberOfCompanies - 1,86, CompanyIndex+2,86))),-999, 1-(PERCENTRANK(2,2 + NumberOfCompanies - 1,86, CompanyIndex+2,86)))
                case 96: return 2;    // Rturns_CR = IF(ISERROR((PERCENTRANK(2,2 + NumberOfCompanies - 1,87, CompanyIndex+2,87))),-999, (PERCENTRANK(2,2 + NumberOfCompanies - 1,87, CompanyIndex+2,87)))
                case 97: return 2;    // Qasales_CS = IF(ISERROR((PERCENTRANK(2,2 + NumberOfCompanies - 1,88, CompanyIndex+2,88))),-999, (PERCENTRANK(2,2 + NumberOfCompanies - 1,88, CompanyIndex+2,88)))
                case 98: return 2;    // Casales_CT = IF(ISERROR((PERCENTRANK(2,2 + NumberOfCompanies - 1,89, CompanyIndex+2,89))),-999, (PERCENTRANK(2,2 + NumberOfCompanies - 1,89, CompanyIndex+2,89)))
                case 99: return 4;    // TurnoverScore = SUM(NetSales_Avg_Wcap_CL,Iturns_CM,Faturn_CN,Taturn_CO,Eqturns_CP,Invsales_CQ,Rturns_CR,Qasales_CS,Casales_CT)/COUNT(NetSales_Avg_Wcap_CL,Iturns_CM,Faturn_CN,Taturn_CO,Eqturns_CP,Invsales_CQ,Rturns_CR,Qasales_CS,Casales_CT)
                case 100: return 1;    // Dbteq_CV = IF(ISERROR(TLiab/TotalEquity),-999,(TLiab/TotalEquity))
                case 101: return 1;    // Dbtta_CW = IF(ISERROR(TLiab/TotalAssets),-999,(TLiab/TotalAssets))
                case 102: return 1;    // Fnmulti_CX = IF(ISERROR(TotalAssets/TotalEquity),-999,(TotalAssets/TotalEquity))
                case 103: return 1;    // Faeqltl_CY = IF(ISERROR(Nppe/(TLiab+TotalEquity)),-999,(Nppe/(TLiab+TotalEquity)))
                case 104: return 1;    // Reta_CZ = IF(ISERROR(Retainede/TotalAssets),-999,(Retainede/TotalAssets))
                case 105: return 2;    // Dbteq_DA = IF(ISERROR(1-(PERCENTRANK(2,2 + NumberOfCompanies - 1,100, CompanyIndex+2,100))),-999, 1-(PERCENTRANK(2,2 + NumberOfCompanies - 1,100, CompanyIndex+2,100)))
                case 106: return 2;    // Dbtta_DB = IF(ISERROR(1-(PERCENTRANK(2,2 + NumberOfCompanies - 1,101, CompanyIndex+2,101))),-999, 1-(PERCENTRANK(2,2 + NumberOfCompanies - 1,101, CompanyIndex+2,101)))
                case 107: return 2;    // Fnmulti_DC = IF(ISERROR(1-(PERCENTRANK(2,2 + NumberOfCompanies - 1,102, CompanyIndex+2,102))),-999, 1-(PERCENTRANK(2,2 + NumberOfCompanies - 1,102, CompanyIndex+2,102)))
                case 108: return 2;    // Faeqltl_DD = IF(ISERROR((PERCENTRANK(2,2 + NumberOfCompanies - 1,103, CompanyIndex+2,103))),-999, (PERCENTRANK(2,2 + NumberOfCompanies - 1,103, CompanyIndex+2,103)))
                case 109: return 2;    // Reta_DE = IF(ISERROR((PERCENTRANK(2,2 + NumberOfCompanies - 1,104, CompanyIndex+2,104))),-999, (PERCENTRANK(2,2 + NumberOfCompanies - 1,104, CompanyIndex+2,104)))
                case 110: return 3;    // LeverageScore = SUM(Dbteq_DA,Dbtta_DB,Fnmulti_DC,Faeqltl_DD,Reta_DE)/COUNT(Dbteq_DA,Dbtta_DB,Fnmulti_DC,Faeqltl_DD,Reta_DE)
                case 111: return 5;    // CompositeScore = SUM(LiquidityScore,ProfitabilityScore,TurnoverScore,LeverageScore)/COUNT(LiquidityScore,ProfitabilityScore,TurnoverScore,LeverageScore)
                case 112: return 0;    // Createdby =
                case 113: return 0;    // Createdon =
                case 114: return 0;    // Updatedby =
                case 115: return 0;    // Updatedon = DateTime.Now.ToString()
                case 116: return 0;    // Datayear =
                case 117: return 0;    // Uid_DM =
                case 118: return 1;    // OneDaySales = Rev1/365
                case 119: return 1;    // OneDayCogs = Cogs/365
                case 120: return 10;    // DsoUq = CalculateDsoUq()
                case 121: return 10;    // DpoUq = CalculateDpoUq()
                case 122: return 10;    // DioUq = CalculateDioUq()
                case 123: return 11;    // Dso_Sales_ = IF(Dso-DsoUq>0,(Dso-DsoUq)*OneDaySales,0)
                case 124: return 11;    // Dpo_Cogs_ = IF(DpoUq-Dpo>0,(DpoUq-Dpo)*OneDayCogs,0)
                case 125: return 11;    // Dio_Cogs_ = IF(Dio-DioUq>0,(Dio-DioUq)*OneDayCogs,0)
                case 126: return 12;    // Twc_BasedOnCogs_ = (Dso_Sales_+Dpo_Cogs_+Dio_Cogs_)
                case 127: return 2;    // ActualRoce = Ebit/CapitalEmployed_Ta_Cl_
                case 128: return 1;    // CapitalEmployed_Ta_Cl_ = TotalAssets-CurrLiab
                case 129: return 13;    // NewRoceWithTwcImpprovement = IF(OR(ActualRoce<0,Twc_BasedOnCogs_>CapitalEmployed_Ta_Cl_),-999,Ebit/(CapitalEmployed_Ta_Cl_-Ebit))
                case 130: return 13;    // TwcScope = Twc_BasedOnCogs_/(Ar+Inventory+Ap)
                case 131: return 0;    // IsTarget =
                case 132: return 0;    // ShortName =
                case 133: return 0;    // CAGR =
                case 134: return 0;    // OP1 = Om1_BO
                case 135: return 0;    // Sic2DDescription =
                case 136: return 0;    // SubIndustry =
                case 137: return 0;    // Country =
                case 138: return 0;    // CompanyNameMixedCase =
                case 139: return 0;    // ShortNameMixedCase =
                default: throw new NotImplementedException();
            }
        }

        public object this[string name]
        {
            get
            {
                switch (name)
                {
                    case "SQLQRY": return this.SqlQry;
                    case "FACTID": return this.Factid;
                    case "CONAME": return this.Coname;
                    case "UID_D": return this.Uid_D;
                    case "SIC2D": return this.Sic2D;
                    case "SICCODE": return this.SicCode;
                    case "SGAMULT": return this.SgaMult;
                    case "YSCID": return this.Yscid;
                    case "CIKID": return this.CikId;
                    case "REV1": return this.Rev1;
                    case "SGA1": return this.Sga1;
                    case "GA1M": return this.Ga1M;
                    case "SGM1": return this.Sgm1;
                    case "EE": return this.Ee;
                    case "REVPEREE": return this.Revperee;
                    case "CASHEQUIV": return this.CashEquiv;
                    case "CURRASSETS": return this.CurrAssets;
                    case "TOTALASSETS": return this.TotalAssets;
                    case "CURRLIAB": return this.CurrLiab;
                    case "COGS": return this.Cogs;
                    case "AR": return this.Ar;
                    case "INVENTORY": return this.Inventory;
                    case "AP": return this.Ap;
                    case "TLIAB": return this.TLiab;
                    case "TOTALEQUITY": return this.TotalEquity;
                    case "MKTCAP": return this.Mktcap;
                    case "TEQLIAB": return this.Teqliab;
                    case "RETAINEDE": return this.Retainede;
                    case "WCTA_AC": return this.Wcta_AC;
                    case "CASHCL_AD": return this.Cashcl_AD;
                    case "CASHTA_AE": return this.Cashta_AE;
                    case "CASHSALE_AF": return this.Cashsale_AF;
                    case "CCRATIO_AG": return this.Ccratio_AG;
                    case "CATA_AH": return this.Cata_AH;
                    case "CLEQ_AI": return this.ClEq_AI;
                    case "QRAT_AJ": return this.Qrat_AJ;
                    case "QTA_AK": return this.Qta_AK;
                    case "INVCA_AL": return this.Invca_AL;
                    case "WCTA_AM": return this.Wcta_AM;
                    case "CASHCL_AN": return this.Cashcl_AN;
                    case "CASHTA_AO": return this.Cashta_AO;
                    case "CASHSALE_AP": return this.Cashsale_AP;
                    case "CCRATIO_AQ": return this.Ccratio_AQ;
                    case "CATA_AR": return this.Cata_AR;
                    case "CLEQ_AS": return this.ClEq_AS;
                    case "QRAT_AT": return this.Qrat_AT;
                    case "QTA_AU": return this.Qta_AU;
                    case "INVCA_AV": return this.Invca_AV;
                    case "LIQUIDITYSCORE": return this.LiquidityScore;
                    case "DSO": return this.Dso;
                    case "DIO": return this.Dio;
                    case "DPO": return this.Dpo;
                    case "CCC": return this.Ccc;
                    case "EBITDA1": return this.Ebitda1;
                    case "EM1": return this.Em1;
                    case "EBITDAPERCENTILERANKING": return this.EbitdaPercentileRanking;
                    case "GP": return this.Gp;
                    case "NI": return this.Ni;
                    case "EBIT": return this.Ebit;
                    case "GPPE": return this.Gppe;
                    case "NPPE": return this.Nppe;
                    case "ROA_BJ": return this.Roa_BJ;
                    case "ROE_BK": return this.Roe_BK;
                    case "GM1_BL": return this.Gm1_BL;
                    case "NP1_BM": return this.Np1_BM;
                    case "SGAM_BN": return this.Sgam_BN;
                    case "OM1_BO": return this.Om1_BO;
                    case "EBITTA_BP": return this.Ebitta_BP;
                    case "ROA_BQ": return this.Roa_BQ;
                    case "ROE_BR": return this.Roe_BR;
                    case "GM1_BS": return this.Gm1_BS;
                    case "NP1_BT": return this.Np1_BT;
                    case "SGAM_BU": return this.Sgam_BU;
                    case "OM1_BV": return this.Om1_BV;
                    case "EBITTA_BW": return this.Ebitta_BW;
                    case "PROFITABILITYSCORE": return this.ProfitabilityScore;
                    case "WCAP": return this.Wcap;
                    case "WCAP1": return this.Wcap1;
                    case "WCSALES": return this.Wcsales;
                    case "AVGWORKINGCAPITAL": return this.AvgWorkingCapital;
                    case "NETSALES_AVG_WCAP_CC": return this.NetSales_Avg_Wcap_CC;
                    case "ITURNS_CD": return this.Iturns_CD;
                    case "FATURN_CE": return this.Faturn_CE;
                    case "TATURN_CF": return this.Taturn_CF;
                    case "EQTURNS_CG": return this.Eqturns_CG;
                    case "INVSALES_CH": return this.Invsales_CH;
                    case "RTURNS_CI": return this.Rturns_CI;
                    case "QASALES_CJ": return this.Qasales_CJ;
                    case "CASALES_CK": return this.Casales_CK;
                    case "NETSALES_AVG_WCAP_CL": return this.NetSales_Avg_Wcap_CL;
                    case "ITURNS_CM": return this.Iturns_CM;
                    case "FATURN_CN": return this.Faturn_CN;
                    case "TATURN_CO": return this.Taturn_CO;
                    case "EQTURNS_CP": return this.Eqturns_CP;
                    case "INVSALES_CQ": return this.Invsales_CQ;
                    case "RTURNS_CR": return this.Rturns_CR;
                    case "QASALES_CS": return this.Qasales_CS;
                    case "CASALES_CT": return this.Casales_CT;
                    case "TURNOVERSCORE": return this.TurnoverScore;
                    case "DBTEQ_CV": return this.Dbteq_CV;
                    case "DBTTA_CW": return this.Dbtta_CW;
                    case "FNMULTI_CX": return this.Fnmulti_CX;
                    case "FAEQLTL_CY": return this.Faeqltl_CY;
                    case "RETA_CZ": return this.Reta_CZ;
                    case "DBTEQ_DA": return this.Dbteq_DA;
                    case "DBTTA_DB": return this.Dbtta_DB;
                    case "FNMULTI_DC": return this.Fnmulti_DC;
                    case "FAEQLTL_DD": return this.Faeqltl_DD;
                    case "RETA_DE": return this.Reta_DE;
                    case "LEVERAGESCORE": return this.LeverageScore;
                    case "COMPOSITESCORE": return this.CompositeScore;
                    case "CREATEDBY": return this.Createdby;
                    case "CREATEDON": return this.Createdon;
                    case "UPDATEDBY": return this.Updatedby;
                    case "UPDATEDON": return this.Updatedon;
                    case "DATAYEAR": return this.Datayear;
                    case "UID_DM": return this.Uid_DM;
                    case "ONEDAYSALES": return this.OneDaySales;
                    case "ONEDAYCOGS": return this.OneDayCogs;
                    case "DSOUQ": return this.DsoUq;
                    case "DPOUQ": return this.DpoUq;
                    case "DIOUQ": return this.DioUq;
                    case "DSO_SALES_": return this.Dso_Sales_;
                    case "DPO_COGS_": return this.Dpo_Cogs_;
                    case "DIO_COGS_": return this.Dio_Cogs_;
                    case "TWC_BASEDONCOGS_": return this.Twc_BasedOnCogs_;
                    case "ACTUALROCE": return this.ActualRoce;
                    case "CAPITALEMPLOYED_TA_CL_": return this.CapitalEmployed_Ta_Cl_;
                    case "NEWROCEWITHTWCIMPPROVEMENT": return this.NewRoceWithTwcImpprovement;
                    case "TWCSCOPE": return this.TwcScope;
                    case "ISTARGET": return this.IsTarget;
                    case "SHORTNAME": return this.ShortName;
                    case "CAGR": return this.CAGR;
                    case "OP1": return this.OP1;
                    case "SIC2DDESCRIPTION": return this.Sic2DDescription;
                    case "SUBINDUSTRY": return this.SubIndustry;
                    case "COUNTRY": return this.Country;
                    case "COMPANYNAMEMIXEDCASE": return this.CompanyNameMixedCase;
                    case "SHORTNAMEMIXEDCASE": return this.ShortNameMixedCase;
                    default: Log.Error("Bad column name: " + name); return null;
                }
            }
        }

        public object this[int i]
        {
            get
            {
                switch (i)
                {
                    case Field.SqlQry: return this.SqlQry;
                    case Field.Factid: return this.Factid;
                    case Field.Coname: return this.Coname;
                    case Field.Uid_D: return this.Uid_D;
                    case Field.Sic2D: return this.Sic2D;
                    case Field.SicCode: return this.SicCode;
                    case Field.SgaMult: return this.SgaMult;
                    case Field.Yscid: return this.Yscid;
                    case Field.CikId: return this.CikId;
                    case Field.Rev1: return this.Rev1;
                    case Field.Sga1: return this.Sga1;
                    case Field.Ga1M: return this.Ga1M;
                    case Field.Sgm1: return this.Sgm1;
                    case Field.Ee: return this.Ee;
                    case Field.Revperee: return this.Revperee;
                    case Field.CashEquiv: return this.CashEquiv;
                    case Field.CurrAssets: return this.CurrAssets;
                    case Field.TotalAssets: return this.TotalAssets;
                    case Field.CurrLiab: return this.CurrLiab;
                    case Field.Cogs: return this.Cogs;
                    case Field.Ar: return this.Ar;
                    case Field.Inventory: return this.Inventory;
                    case Field.Ap: return this.Ap;
                    case Field.TLiab: return this.TLiab;
                    case Field.TotalEquity: return this.TotalEquity;
                    case Field.Mktcap: return this.Mktcap;
                    case Field.Teqliab: return this.Teqliab;
                    case Field.Retainede: return this.Retainede;
                    case Field.Wcta_AC: return this.Wcta_AC;
                    case Field.Cashcl_AD: return this.Cashcl_AD;
                    case Field.Cashta_AE: return this.Cashta_AE;
                    case Field.Cashsale_AF: return this.Cashsale_AF;
                    case Field.Ccratio_AG: return this.Ccratio_AG;
                    case Field.Cata_AH: return this.Cata_AH;
                    case Field.ClEq_AI: return this.ClEq_AI;
                    case Field.Qrat_AJ: return this.Qrat_AJ;
                    case Field.Qta_AK: return this.Qta_AK;
                    case Field.Invca_AL: return this.Invca_AL;
                    case Field.Wcta_AM: return this.Wcta_AM;
                    case Field.Cashcl_AN: return this.Cashcl_AN;
                    case Field.Cashta_AO: return this.Cashta_AO;
                    case Field.Cashsale_AP: return this.Cashsale_AP;
                    case Field.Ccratio_AQ: return this.Ccratio_AQ;
                    case Field.Cata_AR: return this.Cata_AR;
                    case Field.ClEq_AS: return this.ClEq_AS;
                    case Field.Qrat_AT: return this.Qrat_AT;
                    case Field.Qta_AU: return this.Qta_AU;
                    case Field.Invca_AV: return this.Invca_AV;
                    case Field.LiquidityScore: return this.LiquidityScore;
                    case Field.Dso: return this.Dso;
                    case Field.Dio: return this.Dio;
                    case Field.Dpo: return this.Dpo;
                    case Field.Ccc: return this.Ccc;
                    case Field.Ebitda1: return this.Ebitda1;
                    case Field.Em1: return this.Em1;
                    case Field.EbitdaPercentileRanking: return this.EbitdaPercentileRanking;
                    case Field.Gp: return this.Gp;
                    case Field.Ni: return this.Ni;
                    case Field.Ebit: return this.Ebit;
                    case Field.Gppe: return this.Gppe;
                    case Field.Nppe: return this.Nppe;
                    case Field.Roa_BJ: return this.Roa_BJ;
                    case Field.Roe_BK: return this.Roe_BK;
                    case Field.Gm1_BL: return this.Gm1_BL;
                    case Field.Np1_BM: return this.Np1_BM;
                    case Field.Sgam_BN: return this.Sgam_BN;
                    case Field.Om1_BO: return this.Om1_BO;
                    case Field.Ebitta_BP: return this.Ebitta_BP;
                    case Field.Roa_BQ: return this.Roa_BQ;
                    case Field.Roe_BR: return this.Roe_BR;
                    case Field.Gm1_BS: return this.Gm1_BS;
                    case Field.Np1_BT: return this.Np1_BT;
                    case Field.Sgam_BU: return this.Sgam_BU;
                    case Field.Om1_BV: return this.Om1_BV;
                    case Field.Ebitta_BW: return this.Ebitta_BW;
                    case Field.ProfitabilityScore: return this.ProfitabilityScore;
                    case Field.Wcap: return this.Wcap;
                    case Field.Wcap1: return this.Wcap1;
                    case Field.Wcsales: return this.Wcsales;
                    case Field.AvgWorkingCapital: return this.AvgWorkingCapital;
                    case Field.NetSales_Avg_Wcap_CC: return this.NetSales_Avg_Wcap_CC;
                    case Field.Iturns_CD: return this.Iturns_CD;
                    case Field.Faturn_CE: return this.Faturn_CE;
                    case Field.Taturn_CF: return this.Taturn_CF;
                    case Field.Eqturns_CG: return this.Eqturns_CG;
                    case Field.Invsales_CH: return this.Invsales_CH;
                    case Field.Rturns_CI: return this.Rturns_CI;
                    case Field.Qasales_CJ: return this.Qasales_CJ;
                    case Field.Casales_CK: return this.Casales_CK;
                    case Field.NetSales_Avg_Wcap_CL: return this.NetSales_Avg_Wcap_CL;
                    case Field.Iturns_CM: return this.Iturns_CM;
                    case Field.Faturn_CN: return this.Faturn_CN;
                    case Field.Taturn_CO: return this.Taturn_CO;
                    case Field.Eqturns_CP: return this.Eqturns_CP;
                    case Field.Invsales_CQ: return this.Invsales_CQ;
                    case Field.Rturns_CR: return this.Rturns_CR;
                    case Field.Qasales_CS: return this.Qasales_CS;
                    case Field.Casales_CT: return this.Casales_CT;
                    case Field.TurnoverScore: return this.TurnoverScore;
                    case Field.Dbteq_CV: return this.Dbteq_CV;
                    case Field.Dbtta_CW: return this.Dbtta_CW;
                    case Field.Fnmulti_CX: return this.Fnmulti_CX;
                    case Field.Faeqltl_CY: return this.Faeqltl_CY;
                    case Field.Reta_CZ: return this.Reta_CZ;
                    case Field.Dbteq_DA: return this.Dbteq_DA;
                    case Field.Dbtta_DB: return this.Dbtta_DB;
                    case Field.Fnmulti_DC: return this.Fnmulti_DC;
                    case Field.Faeqltl_DD: return this.Faeqltl_DD;
                    case Field.Reta_DE: return this.Reta_DE;
                    case Field.LeverageScore: return this.LeverageScore;
                    case Field.CompositeScore: return this.CompositeScore;
                    case Field.Createdby: return this.Createdby;
                    case Field.Createdon: return this.Createdon;
                    case Field.Updatedby: return this.Updatedby;
                    case Field.Updatedon: return this.Updatedon;
                    case Field.Datayear: return this.Datayear;
                    case Field.Uid_DM: return this.Uid_DM;
                    case Field.OneDaySales: return this.OneDaySales;
                    case Field.OneDayCogs: return this.OneDayCogs;
                    case Field.DsoUq: return this.DsoUq;
                    case Field.DpoUq: return this.DpoUq;
                    case Field.DioUq: return this.DioUq;
                    case Field.Dso_Sales_: return this.Dso_Sales_;
                    case Field.Dpo_Cogs_: return this.Dpo_Cogs_;
                    case Field.Dio_Cogs_: return this.Dio_Cogs_;
                    case Field.Twc_BasedOnCogs_: return this.Twc_BasedOnCogs_;
                    case Field.ActualRoce: return this.ActualRoce;
                    case Field.CapitalEmployed_Ta_Cl_: return this.CapitalEmployed_Ta_Cl_;
                    case Field.NewRoceWithTwcImpprovement: return this.NewRoceWithTwcImpprovement;
                    case Field.TwcScope: return this.TwcScope;
                    case Field.IsTarget: return this.IsTarget;
                    case Field.ShortName: return this.ShortName;
                    case Field.CAGR: return this.CAGR;
                    case Field.OP1: return this.OP1;
                    case Field.Sic2DDescription: return this.Sic2DDescription;
                    case Field.SubIndustry: return this.SubIndustry;
                    case Field.Country: return this.Country;
                    case Field.CompanyNameMixedCase: return this.CompanyNameMixedCase;
                    case Field.ShortNameMixedCase: return this.ShortNameMixedCase;
                    default: return null;
                }
            }
        }
    }
}