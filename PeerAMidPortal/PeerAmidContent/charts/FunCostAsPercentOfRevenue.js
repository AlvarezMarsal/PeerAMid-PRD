//if (!wcd) {
//    plotFunCostAsPercentOfRevenue();
//    plotFTECostAsPercentOfRevenue();
//}

//SG&A Decomposed Functional Costs Analysis*
function plotFunCostAsPercentOfRevenue(wcd, gsd, sd, doAsnyc) {

    if (wcd)
        return { done: function (f) { f(); } };

    try {
        //debugger
        const selectedPeers = analysisState.peerCompanies;
        const selectedTargetCompany = analysisState.benchmarkCompany.CompanyId;
        const benchMarkCompYear = analysisState.benchmarkCompany.FinancialYear;
        //const optionId = $("#optionId").val();
        const postData = {
            selectedPeerList: selectedPeers,
            targetCompanyId: selectedTargetCompany,
            year: benchMarkCompYear,
            optionId: 1 // optionId
        };

        console.log("Calling ajax: '/api/HomeAPI/GetFunCostAsPercentOfRevenueData'");
        return $.ajax({
            async: doAsnyc,
            type: "GET",
            url: "/api/HomeAPI/GetFunCostAsPercentOfRevenueData",
            data: postData,
            dataType: "json",
            headers: config,
            //async: false,
            success: function(reportData) {
                if (reportData != null && reportData != undefined) {
                    GenerateFunCostAsPercentOfRevenueChart(reportData);
                }
            },
            error: function(_, _, err) {
                console.log("error");
                console.log(err);
            }
        });
    } catch {
        console.log("Exception");
    }
}

//SG&A Decomposed Functional FTE Analysis*.
function plotFTECostAsPercentOfRevenue(wcd, gsd, sd, doAsnyc) {
    if (wcd) {
        return { done: function (f) { f(); } };
    }
    //debugger
    const selectedPeers = analysisState.peerCompanies;
    const selectedTargetCompany = analysisState.benchmarkCompany.CompanyId;
    const benchMarkCompYear = analysisState.benchmarkCompany.FinancialYear;
    // const optionId = $("#optionId").val();
    const postData = {
        selectedPeerList: selectedPeers,
        targetCompanyId: selectedTargetCompany,
        year: benchMarkCompYear,
        optionId: 1 // optionId
    };
    console.log("Calling ajax: '/api/HomeAPI/GetFTEDecomposedCostAsPercentOfRevenueData'");
    return $.ajax({
        async: doAsnyc,
        type: "GET",
        url: "/api/HomeAPI/GetFTEDecomposedCostAsPercentOfRevenueData",
        data: postData,
        dataType: "json",
        headers: config,
        //async: false,
        success: function(reportData) {
            if (reportData != null && reportData != undefined) {
                GenerateFTEAsPercentOfRevenueChart(reportData);
            }
        },
        error: function(_, _, err) {
            console.log("error");
            console.log(err);
        }
    });
}

//Function use for rounding data on two digits.
function DecimalRoundingTwoDigit(reportData) {
    for (let i = 0; i < reportData.length; i++) {
        reportData[i].PeerCompanyValue = reportData[i].PeerCompanyValue.toFixed(2);
    }
}

//Function use for rounding data on one digit.
function DecimalRoundingOneDigit(reportData) {
    for (let i = 0; i < reportData.length; i++) {
        reportData[i].PeerCompanyValue = reportData[i].PeerCompanyValue.toFixed(1);
    }
}

//Functional Cost % Revenue Chart data plotting function.
function GenerateFunCostAsPercentOfRevenueChart(reportData) {
    DecimalRoundingTwoDigit(reportData.FinanceData);
    DecimalRoundingTwoDigit(reportData.SalesData);
    DecimalRoundingTwoDigit(reportData.HRData);
    DecimalRoundingTwoDigit(reportData.MarketData);
    DecimalRoundingTwoDigit(reportData.ITData);
    DecimalRoundingTwoDigit(reportData.CustServData);
    DecimalRoundingTwoDigit(reportData.ProcurementData);
    DecimalRoundingTwoDigit(reportData.CSSupportServData);

    CommonChartForthAndSixth("divFunCostFinanceChart", reportData.FinanceData, false);
    CommonChartForthAndSixth("divFunCostSalesChart", reportData.SalesData, false);
    CommonChartForthAndSixth("divFunCostHRChart", reportData.HRData, false);
    CommonChartForthAndSixth("divFunCostMarketChart", reportData.MarketData, false);
    CommonChartForthAndSixth("divFunCostITChart", reportData.ITData, false);
    CommonChartForthAndSixth("divFunCostCustServChart", reportData.CustServData, false);
    CommonChartForthAndSixth("divFunCostProcurementChart", reportData.ProcurementData, false);
    CommonChartForthAndSixth("divFunCostCSSupportServChart", reportData.CSSupportServData, false);
}

//FTEs per Billion Revenue Chart data plotting function.
function GenerateFTEAsPercentOfRevenueChart(reportData) {
    DecimalRoundingOneDigit(reportData.FinanceData);
    DecimalRoundingOneDigit(reportData.SalesData);
    DecimalRoundingOneDigit(reportData.HRData);
    DecimalRoundingOneDigit(reportData.MarketData);
    DecimalRoundingOneDigit(reportData.ITData);
    DecimalRoundingOneDigit(reportData.CustServData);
    DecimalRoundingOneDigit(reportData.ProcurementData);
    DecimalRoundingOneDigit(reportData.CSSupportServData);

    CommonChartForthAndSixth("divFTEFinanceChart", reportData.FinanceData, true);
    CommonChartForthAndSixth("divFTESalesChart", reportData.SalesData, true);
    CommonChartForthAndSixth("divFTEHRChart", reportData.HRData, true);
    CommonChartForthAndSixth("divFTEMarketChart", reportData.MarketData, true);
    CommonChartForthAndSixth("divFTEITChart", reportData.ITData, true);
    CommonChartForthAndSixth("divFTECustServChart", reportData.CustServData, true);
    CommonChartForthAndSixth("divFTEProcurementChart", reportData.ProcurementData, true);
    CommonChartForthAndSixth("divFTECSSupportServChart", reportData.CSSupportServData, true);

    //Set summary logic for chart FTE
    $("#divFunctionalCostSummary")
        .html(reportData.FTESummaryLine.replace("##CompanyName##", analysisState.benchmarkCompany.DisplayName));
}

//Plotting amcharts data in application. There are various property of chart can set under this function(BOLD, WIDTH, TITLE etc).
function CommonChartForthAndSixth(chartDivId, chartData, isFTECall) {
    //console.log("entering CommonChartForthAndSixth " + chartDivId);
    if (chartData != null && chartData != undefined) {
        const chart = AmCharts.makeChart(chartDivId,
        {
                "type": "serial",
                "categoryField": "PeerCompanyDisplayName",
                "titles": [
                    {
                        "text": ""
                    }
                ],
                "angle": 30,
                "depth3D": 0,
                "startDuration": 0,
                "marginTop": 20,
                "marginBottom": 20,
                "categoryAxis": {
                    "gridPosition": "start",
                    "tickPosition": "start",
                    "gridAlpha": 0,
                    "autoWrap": true,
                    "ignoreAxisWidth": true,
                    "labelsEnabled": false,
                    "tickLength": 0,
                    "axisColor": "#000000"
                },
                "trendLines": [],
                "graphs": [
                    {
                        "balloonText": `[[category]]:[[value]]${isFTECall == true ? "" : "%"}`,
                        "fillAlphas": 1,
                        "fillColorsField": "Color",
                        "id": "AmGraph-1",
                        "labelOffset": 10,
                        "labelText": `[[PeerCompanyValue]]${isFTECall == true ? "" : "%"}`,
                        "lineAlpha": 0,
                        "minBulletSize": 6,
                        "title": "graph 1",
                        "type": "column",
                        "valueField": "PeerCompanyValue",
                        "fixedColumnWidth": 25,
                        "showAllValueLabels": true,
                        "fontSize": 10
                    }
                ],
                "guides": [],
                "valueAxes": [
                    {
                        "id": "ValueAxis-1",
                        "autoRotateAngle": -19.8,
                        "axisAlpha": 0,
                        "gridAlpha": 0,
                        "labelsEnabled": false,
                        "minimum": (isFTECall == true ? 0 : .001), // PEER-56
                        "title": ""
                    }
                ],
                "allLabels": [],
                "balloon": {},
                "legend": {
                    "enabled": false,
                    "useGraphSettings": true
                },
                "dataProvider": chartData /*,
                "listeners": [
                    {
                        "event": "drawn",
                        "method": function (e) { console.log("CommonChartForthAndSixth " + chartDivId + " drawn event"); }
                    },
                    {
                        "event": "animationFinished",
                        "method": function (e) { console.log("CommonChartForthAndSixth " + chartDivId + " animationFinished event"); }
                    },
                    {
                        "event": "rendered",
                        "method": function (e) { console.log("CommonChartForthAndSixth " + chartDivId + " rendered event"); }
                    }
                ]*/
        });
        
    }
    //console.log("Exiting CommonChartForthAndSixth" + chartDivId);
}