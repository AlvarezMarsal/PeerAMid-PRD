//if (!wcd)
//    plotWaterFallChart();

var max;

//Estimated Opportunity Waterfall* (quartile)
// Estimated Opportunity Waterfall*(Median)
// Estimated Opportunity Waterfall* (Decile)
function plotWaterFallChart(wcd, doAsync) {

    if (wcd)
        return { done: function(f) { f(); } };

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

    console.log("Calling ajax: '/api/HomeAPI/GetDecomposedSGAWaterFall'");
    return $.ajax({
            async: doAsync,
            type: "GET",
            url: "/api/HomeAPI/GetDecomposedSGAWaterFall",
            data: postData,
            dataType: "json",
            headers: config,
            // async: false,
            success: function(reportData) {
                if (reportData != null && reportData != undefined) {
                    try {
                        let l = reportData.TopQChartData.WaterfallChartItemList.length;
                        const topQValue = reportData.TopQChartData.WaterfallChartItemList[l - 1].DepartmentValue;
                        l = reportData.MedianChartData.WaterfallChartItemList.length;
                        const medianValue = reportData.MedianChartData.WaterfallChartItemList[l - 1].DepartmentValue;
                        l = reportData.TopDChartData.WaterfallChartItemList.length;
                        const topDValue = reportData.TopDChartData.WaterfallChartItemList[l - 1].DepartmentValue;
                        if (topQValue == 0 && medianValue == 0) {
                            max = Math.max(topDValue);
                        } else if (topQValue > 0 && medianValue == 0) {
                            max = Math.max(topQValue, topDValue);
                        } else if (topQValue > 0 && medianValue > 0) {
                            max = Math.max(topQValue, medianValue);
                        }
                        //Median
                        CommonWaterfallChart("divDecOpprtunityWaterfallChartMedian",
                            reportData.MedianChartData.WaterfallChartItemList,
                            false,
                            medianValue);
                        //TopQ
                        CommonWaterfallChart("divDecOpprtunityWaterfallChartBestInClass",
                            reportData.TopQChartData.WaterfallChartItemList,
                            true,
                            max);
                        //TopD
                        TopDecileWaterfallChart("divDecOpprtunityWaterfallTopDecile",
                            reportData.TopDChartData.WaterfallChartItemList);

                        //Set sumary logic for 4th(SGA functional cost) and waterfall chart
                        SetSumaryLogicForWaterfallAndSgaFunctionalCost(reportData);
                    } catch {
                        console.log("Exception in plotWaterfall");
                    }
            }
        },
        error: function(_, _, err) 
        {
            console.log("error");
            console.log(err);
        }

    });
}

//Business logic put down under this function for bind the waterfall chart.
function SetSumaryLogicForWaterfallAndSgaFunctionalCost(chartData) {
    $("#divTopDecile").hide();
    $("#divDecOpprtunityWaterfallTopDecile").hide();

    $("#divWaterfallSummary").html("");
    if (chartData != undefined &&
        chartData != null &&
        chartData.TopQChartData != undefined &&
        chartData.TopQChartData != null &&
        chartData.MedianChartData != undefined &&
        chartData.MedianChartData != null) {
        var topQValue = chartData.TopQChartData.WaterfallChartItemList[8].DepartmentValue;
        var medianValue = chartData.MedianChartData.WaterfallChartItemList[8].DepartmentValue;
        var topDValue = chartData.TopDChartData.WaterfallChartItemList[8].DepartmentValue;

        if (topQValue == 0 && medianValue == 0) {
            // case 3
            $("#divFunctionalCostSummary").html(
                analysisState.benchmarkCompany.DisplayName +
                " is a best in class performer.");
        } else if (topQValue > 0 && medianValue == 0) {
            // case 1
            $("#divFunctionalCostSummary").html(
                analysisState.benchmarkCompany.DisplayName +
                "'s decomposed  SG&A departmental costs are generally within the 2nd quartile of their peer group, and ahead of the median.  However, there remains a  " +
                numberWithCommas(Math.round(topQValue)) +
                "MM opportunity gap to the top quartile performance level.");
        } else if (topQValue > 0 && medianValue > 0) {
            // case 2
            $("#divFunctionalCostSummary").html(
                analysisState.benchmarkCompany.DisplayName +
                " is performing below Median, and they have a significant SG&A cost improvement opportunity when compared with Median to Top Quartile performers.  The SG&A opportunity could range between $" +
                numberWithCommas(Math.round(medianValue)) +
                "MM to $" +
                numberWithCommas(Math.round(topQValue)) +
                "MM.");
        }
        //Belwo logic is for waterfall chart
        if (medianValue == 0 && topQValue == 0 && topDValue == 0) {
            $("#divWaterfallSummary")
                .html(toTitleCase($("#benchmarkCompanyNameMixedCase").text()) + " is a best in class performer.");
            $("#divDecOpprtunityWaterfallChartMedian").hide();
            $("#divDecOpprtunityWaterfallChartBestInClass").hide();
            $("#divDecOpprtunityWaterfallTopDecile").hide();
            $("#div_P_DecOpprtunityWaterfallChartMedian").hide();
            $("#div_P_DecOpprtunityWaterfallChartBestInClass").hide();
            $("#div_P_DecOpprtunityWaterfallTopDecile").hide();
        } else if (medianValue == 0 && topQValue == 0 && topDValue > 0) {
            $("#divDecOpprtunityWaterfallChartMedian").hide();
            $("#divDecOpprtunityWaterfallChartBestInClass").hide();
            $("#divDecOpprtunityWaterfallTopDecile").show();
            $("#div_P_DecOpprtunityWaterfallChartMedian").hide();
            $("#div_P_DecOpprtunityWaterfallChartBestInClass").hide();
            $("#div_P_DecOpprtunityWaterfallTopDecile").show();

            var departmentNames =
                GetDepartmentTextUpTo80Percent(chartData.TopDChartData.WaterfallChartItemList, topDValue);
            $("#divWaterfallSummary")
                .html(`The SG&A cost reduction opportunity waterfall indicates the overall opportunity is pegged at $${
                    numberWithCommas(Math.round(topDValue))
                    }MM, with 80% of the opportunity coming from ${departmentNames}.`);
        } else if (medianValue == 0 && topQValue > 0 && topDValue > 0) {
            $("#divDecOpprtunityWaterfallChartMedian").hide();
            $("#divDecOpprtunityWaterfallChartBestInClass").show();
            $("#divDecOpprtunityWaterfallTopDecile").show();
            $("#div_P_DecOpprtunityWaterfallChartMedian").hide();
            $("#div_P_DecOpprtunityWaterfallChartBestInClass").show();
            $("#div_P_DecOpprtunityWaterfallTopDecile").show();
            var departmentNames =
                GetDepartmentTextUpTo80Percent(chartData.TopDChartData.WaterfallChartItemList, topDValue);
            $("#divWaterfallSummary").html(
                `The SG&A cost reduction opportunity waterfall indicates the overall opportunity ranges from $${
                numberWithCommas(Math.round(topQValue))}MM to $${numberWithCommas(Math.round(topDValue))
                }MM, with 80% of the opportunity coming from ${departmentNames}.`);
        } else if (medianValue > 0 && topQValue > 0 && topDValue > 0) {
            $("#divDecOpprtunityWaterfallChartBestInClass").show();
            $("#divDecOpprtunityWaterfallTopDecile").hide();
            $("#divDecOpprtunityWaterfallChartMedian").show();
            $("#div_P_DecOpprtunityWaterfallChartBestInClass").show();
            $("#div_P_DecOpprtunityWaterfallTopDecile").hide();
            $("#div_P_DecOpprtunityWaterfallChartMedian").show();
            var departmentNames =
                GetDepartmentTextUpTo80Percent(chartData.TopQChartData.WaterfallChartItemList, topQValue);
            $("#divWaterfallSummary").html(
                `The SG&A cost reduction opportunity waterfall indicates the overall opportunity ranges from $${
                numberWithCommas(Math.round(medianValue))}MM to $${numberWithCommas(Math.round(topQValue))
                }MM, with 80% of the opportunity coming from ${departmentNames}.`);
        }
    }
}

//Plotting amcharts data in application. There are various property of chart can set under this function(BOLD, WIDTH, TITLE etc).
function CommonWaterfallChart(chartDivId, chartData, isMedian, max) {
    //console.log("Entered CommonWaterfallChart");

    for (let i in chartData) {
        chartData[i].DepartmentValue = chartData[i].DepartmentValue.toFixed(1);
    }
    //log(chartData);
    const chart = AmCharts.makeChart(chartDivId,
        {
            "type": "serial",
            "categoryField": "DepartmentName",
            "startDuration": 0,
            "fontFamily": "Arial, Helvetica, sans-serif",
            "titles": [
                {
                    "size": 12,
                    "text": isMedian
                        ? "Incremental Departmental Gap to Top Quartile in $MM"
                        : "Incremental Departmental Gap to Median in $MM",
                    "color": "#646464"
                }
            ],
            "color": "rgb(100,100,100)",
            "categoryAxis": {
                "autoRotateAngle": 0,
                "gridPosition": "start",
                "axisColor": "#888",
                "boldLabels": true,
                "gridAlpha": 0,
                "gridColor": "#888",
                "labelRotation": 90,
                "minHorizontalGap": 0,
                "minVerticalGap": 30
            },
            "trendLines": [],
            "graphs": [
                {
                    "balloonText": "[[DepartmentValue]]",
                    "bulletBorderThickness": 0,
                    "closeField": "EndValue",
                    "columnWidth": 0.57,
                    "fillAlphas": 1,
                    "fillColors": isMedian ? "#29702A" : "#F9C20A",
                    "gapPeriod": -1,
                    "id": "AmGraph-1",
                    "labelOffset": 38,
                    "showAllValueLabels": true,
                    "labelText": "[[DepartmentValue]]",
                    "lineAlpha": 0,
                    "openField": "StartValue",
                    "title": "",
                    "type": "column",
                    "labelOffset": 5,
                    "labelPosition": "top",
                    "fontSize": 10
                }
            ],
            "guides": [],
            "valueAxes": [
                {
                    "axisTitleOffset": 9,
                    "id": "ValueAxis-1",
                    "stackType": "regular",
                    "axisAlpha": 25,
                    "gridAlpha": 0,
                    "labelsEnabled": true,
                    "showFirstLabel": true,
                    "showLastLabel": true,
                    "maximum": max,
                    "title": ""
                }
            ],
            "allLabels": [],
            "balloon": {},
            "dataProvider": chartData /*,
            "listeners": [
                {
                    "event": "drawn",
                    "method": function (e) { console.log("CommonWaterfallChart drawn event"); }
                },
                {
                    "event": "animationFinished",
                    "method": function (e) { console.log("CommonWaterfallChart  animationFinished event"); }
                },
                {
                    "event": "rendered",
                    "method": function (e) { console.log("CommonWaterfallChart  rendered event"); }
                }
            ]*/

        });
    //console.log("Exited CommonWaterfallChart");
}

//Plotting amcharts data in application. There are various property of chart can set under this function(BOLD, WIDTH, TITLE etc).
function TopDecileWaterfallChart(chartDivId, chartData) {
    //console.log("Entered TopDecileWaterfallChart");
    for (let i in chartData) {
        chartData[i].DepartmentValue = chartData[i].DepartmentValue.toFixed(1);
    }
    const chart = AmCharts.makeChart(chartDivId,
        {
            "type": "serial",
            "categoryField": "DepartmentName",
            "startDuration": 0,
            "fontFamily": "Arial, Helvetica, sans-serif",
            "titles": [
                {
                    "size": 12,
                    "text": "Incremental Departmental Gap to Top Decile in $MM"
                }
            ],
            "color": "rgb(100,100,100)",
            "categoryAxis": {
                "autoRotateAngle": 0,
                "gridPosition": "start",
                "axisColor": "#888",
                "boldLabels": true,
                "gridAlpha": 0,
                "gridColor": "#888",
                "labelRotation": 90,
                "minHorizontalGap": 0,
                "minVerticalGap": 30
            },
            "trendLines": [],
            "graphs": [
                {
                    "balloonText": "[[DepartmentValue]]",
                    "bulletBorderThickness": 0,
                    "closeField": "EndValue",
                    "columnWidth": 0.57,
                    "fillAlphas": 1,
                    "fillColors": "rgb(52,83,112)",
                    "gapPeriod": -1,
                    "id": "AmGraph-1",
                    "labelOffset": 38,
                    "showAllValueLabels": true,
                    "labelText": "[[DepartmentValue]]",
                    "lineAlpha": 0,
                    "openField": "StartValue",
                    "title": "",
                    "type": "column",
                    "labelOffset": 5,
                    "labelPosition": "top",
                    "fontSize": 10
                }
            ],
            "guides": [],
            "valueAxes": [
                {
                    "axisTitleOffset": 9,
                    "id": "ValueAxis-1",
                    "stackType": "regular",
                    "axisAlpha": 25,
                    "gridAlpha": 0,
                    "labelsEnabled": true,
                    "showFirstLabel": true,
                    "showLastLabel": true,
                    "maximum": max,
                    "title": ""
                }
            ],
            "allLabels": [],
            "balloon": {},
            "dataProvider": chartData /*,
            "listeners": [
                {
                    "event": "drawn",
                    "method": function (e) { console.log("TopDecileWaterfallChart drawn event"); }
                },
                {
                    "event": "animationFinished",
                    "method": function (e) { console.log("TopDecileWaterfallChart  animationFinished event"); }
                },
                {
                    "event": "rendered",
                    "method": function (e) { console.log("TopDecileWaterfallChart  rendered event"); }
                }
            ] */


        });
    //console.log("Exited TopDecileWaterfallChart");
}

//Calculating percent value(80% of the opportunity coming) for pointing chart.
function GetDepartmentTextUpTo80Percent(chartData, numForPercent) {
    if (typeof(numForPercent) == "string")
        numForPercent = parseFloat(numForPercent);
    var eightyPercent = numForPercent * 0.8;
    var total = 0.0;
    var done = false;
    var departments = [];

    $.each(chartData,
        function(index, item) {

            if (!done) {
                departments.push(item.DepartmentName);

                if (typeof(item.DepartmentValue) == "string")
                    total = total + parseFloat(item.DepartmentValue);
                else
                    total = total + item.DepartmentValue;

                done = (total >= eightyPercent);
            }
        });

    return OxfordComma(departments);
};

function OxfordComma(items) {
    if (items.length == 0)
        return "";
    if (items.length == 1)
        return items[0];
    if (items.length == 2)
        return items[0] + " and " + items[1];
    let result = items[0] + ", " + items[2];
    let i = 2;
    while (i < (items.length - 1))
        result = result + ", " + items[i++];
    result += " and " + items[i]; // not really the oxford comma, is it?
    return result;
}
