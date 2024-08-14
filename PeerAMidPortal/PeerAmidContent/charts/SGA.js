// plotSGAChart(wcd); // Actually does the WCD chart, if we're doing WCD

// SG&A Cost as a Percent of Revenue
function plotSGAChart(wcd, doAsync) {
    // console.log("plotSGAChart");
    const selectedPeers = analysisState.peerCompanies;
    const selectedTargetCompany = analysisState.benchmarkCompany.CompanyId;
    const benchMarkCompYear = analysisState.benchmarkCompany.FinancialYear;
    const selectedTargetCompanySymbol = analysisState.benchmarkCompany.Ticker;
    //const optionId = $("#optionId").val();

    const postData = {
        selectedPeerList: selectedPeers,
        targetCompanyId: selectedTargetCompany,
        targetCompanySymbol: selectedTargetCompanySymbol,
        year: benchMarkCompYear,
        optionId: 1, // optionId,
        requireMatchingFiscalYear: false
    };
    var url = "";
    if (wcd) {
        url = "/api/HomeAPI/GetWorkingCapitalData";
    } else {
        url = "/api/HomeAPI/GetSGAChartData";
    }

    console.log(`Calling ajax: '${url}'`);
    return $.ajax({
        async: doAsync,
        type: "Get",
        url: url,
        data: postData,
        dataType: "json",
        headers: config,
        // async: false,
        success: function(reportData) {
            if (reportData != null && reportData != undefined) {
                //console.log(reportData);
                //console.log(JSON.stringify(reportData));
                if (wcd) {
                    // console.log("GenerateWCDChart1");
                    GenerateWCDChart1(reportData);
                    // console.log("GenerateWCDChart2");
                    GenerateWCDChart2(reportData.SummaryData["CCC"]);
                    //$(".loader-div").hide();
                } else {
                    $.each(reportData.ChartData.ProviderData,
                        function(index, item) {
                            // Truncate Peer Company name to the maximum amount of words that can fit in the specified length
                            //item.PeerDisplayName = item.PeerCompanyDisplayName, 20);
                        });
                    (reportData.ChartData.ProviderData).forEach(function(item) {
                        //item.PeerDisplayName = item.PeerCompanyDisplayName;
                    });
                    // console.log("GenerateSGAChart1");
                    GenerateSGAChart1(reportData);
                    // console.log("GenerateSGAChart2");
                    GenerateSGAChart2(reportData);
                }
            }
        },
        error: function(_, _, err) {
            console.log(`ERROR!${JSON.stringify(err)}`);
            console.debug(err);
        }
    });
}

// Split and replace function: replace , from word in '' format.
function splitInTwoWord(word) {
    var text = "";
    const split = word.split(" ");
    if (split.length == 1) {
        text = split[0];
    } else if (split.length > 1) {
        text = split[0] + " " + split[1].replace(",", "");
    }
    return text;
}

//Plotting amcharts data in application. There are various property of chart can set under this function(BOLD, WIDTH, TITLE etc).
function GenerateSGAChart1(reportData) {
    //console.log("Entering GenerateSGAChart1");
    //console.log(JSON.stringify(reportData));

    const chart = AmCharts.makeChart("chartdiv",
        {
            "type": "serial",
            "categoryField": "PeerCompanyDisplayName",
            "angle": 30,
            "depth3D": 0,
            "rotate": true, // Rotate chart by 90% - columns become rows  (PEER-16)
            //"columnWidth": .8,            // FOR FUTURE USE - Decrease number t oincrease space between bars
            "color": "#646464", // Update text color to match legend (PEER-16)
            "fontFamily": "Arial, Helvetica, 'sans-serif'", // Updated text font to match legend (PEER-16)
            "startDuration": 0,
            "marginTop": 50,
            "marginBottom": 60,
            "categoryAxis": {
                "autoRotateAngle": 0,
                "boldLabels": true, // (PEER-16)
                "color": "#646464", // Update label text color to match legend(PEER-16)
                //"fontSize": "12",         // FOR FUTURE USE
                "labelRotation": 90,
                "gridPosition": "start",
                "tickPosition": "end",
                "gridAlpha": 0,
            },
            "trendLines": [],
            "graphs": [
                {
                    "precision": 2,
                    "showAllValueLabels": true,
                    "balloonText": "[[PeerCompanyDisplayName]]:[[value]]%",
                    "fillAlphas": 1,
                    "fillColorsField": "BorderColor",
                    "id": "AmGraph-1",
                    "labelOffset": 10,
                    "labelText": "[[value]]%",
                    "lineAlpha": 1,
                    "minBulletSize": 6,
                    "title": "graph 1",
                    "type": "column",
                    "valueField": "PeerCompanyValue",
                    "lineColorField": "FillColor",
                    "lineThickness": 1.5,
                    //"fixedColumnWidth": 50    // Disbled to create space between rows (PEER-16)
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
                    "title": ""
                }
            ],
            "allLabels": [],
            "balloon": {},
            "legend": {
                "enabled": false,
                "useGraphSettings": true,
            },
            "dataProvider": reportData.ChartData.ProviderData,

            "listeners": [
                {
                    "event": "drawn",
                    "method": function(e) {

                        //console.log("GenerateSGAChart1 drawn event");

                        BindSNGChartDifferenceTable(reportData.DifferenceTableData);

                        if (reportData.DifferenceTableData.OutliersDescription.length > 0) {
                            $("#outliers").html(reportData.DifferenceTableData.OutliersDescription);
                        } else {
                            $("#outliersDot").hide();
                        }

                    }
                } /*,
                {
                    "event": "animationFinished",
                    "method": function (e) { console.log("GenerateSGAChart1 animationFinished event"); }
                },
                {
                    "event": "rendered",
                    "method": function (e) { console.log("GenerateSGAChart1 rendered event"); }
                }*/
            ]
        });

    //console.log("Exiting GenerateSGAChart1");
}

//Plotting amcharts data in application. There are various property of chart can set under this function(BOLD, WIDTH, TITLE etc).
function GenerateWCDChart1(workingCapitalData) {
    //console.log("Entering GenerateWCDChart1");
    //console.log(JSON.stringify(workingCapitalData));

    var ccc = workingCapitalData.SummaryData["CCC"];
    const chart = AmCharts.makeChart("chartdiv",
        {
            "type": "serial",
            "categoryField": "DisplayName",
            "angle": 30,
            "depth3D": 0,
            "rotate": true, // Rotate chart by 90% - columns become rows  (PEER-16)
            //"columnWidth": .8,            // FOR FUTURE USE - Decrease number to increase space between bars
            "color": "#646464", // Update text color to match legend (PEER-16)
            "fontFamily": "Arial, Helvetica, 'sans-serif'", // Updated text font to match legend (PEER-16)
            "startDuration": 0,
            "marginTop": 50,
            "marginBottom": 60,
            "categoryAxis": {
                "autoRotateAngle": 0,
                "boldLabels": true, // (PEER-16)
                "color": "#646464", // Update label text color to match legend(PEER-16)
                //"fontSize": "12",         // FOR FUTURE USE
                "labelRotation": 90,
                "gridPosition": "start",
                "tickPosition": "end",
                "gridAlpha": 0,
            },
            "trendLines": [],
            "graphs": [
                {
                    "precision": 0,
                    "showAllValueLabels": true,
                    "balloonText": "[[DisplayName]]:[[value]] days",
                    "fillAlphas": 1,
                    "fillColorsField": "BorderColor",
                    "id": "AmGraph-1",
                    "labelOffset": 10,
                    "labelText": "[[value]] days",
                    "lineAlpha": 1,
                    "minBulletSize": 6,
                    "title": "graph 1",
                    "type": "column",
                    "valueField": "Value",
                    "lineColorField": "FillColor",
                    //"lineThickness": 100.5,
                    //"fixedColumnWidth": 50    // Disbled to create space between rows (PEER-16)
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
                    "title": ""
                }
            ],
            "allLabels": [],
            "balloon": {},
            "legend": {
                "enabled": false,
                "useGraphSettings": true,
            },
            "dataProvider": ccc.SortedCompanies,

            "listeners": [
                {
                    "event": "drawn",
                    "method": function(e) {

                        //console.log("GenerateWCDChart1 drawn event");

                        BindSNGChartDifferenceTableWCD(ccc);

                        if (ccc.OutliersDescription.length > 0) {
                            $("#outliers").html(ccc.OutliersDescription);
                        } else {
                            $("#outliersDot").hide();
                        }
                    }
                } /*,
                {
                    "event": "animationFinished",
                    "method": function (e) { console.log("GenerateWCDChart1 animationFinished event"); }
                },
                {
                    "event": "rendered",
                    "method": function (e) { console.log("GenerateWCDChart1 rendered event"); }
                } */
            ]
        });

    // console.log("Exiting GenerateWCDChart1");
}

//Business logic put down under this function for bind the chart.
function BindSNGChartDifferenceTable(diffTableData) {
    var blankCellMark = "-";
    var targetCompany = analysisState.benchmarkCompany.DisplayName || "";
    var costTarget = diffTableData.CostTarget;
    var profileChartmessage =
        `Optimizing SG&A spend can liberate capital for more strategic leverage within operations or drive savings to the bottom line. As per reported 10-K, ${
            targetCompany} SG&A cost as a % of revenue is ${costTarget}%.`;
    $("#divIndustryGroupProfile").html(profileChartmessage);
    // var CostTopQuartile = diffTableData.CostTopQuartile;
    // var CostTopDecile = diffTableData.CostTopDecile;

    // var CostGapMedian = diffTableData.CostGapMedian + "%";
    // var CostGapTarget = blankCellMark;
    // var CostGapTopQuartile = diffTableData.CostGapTopQuartile + "%";
    // var CostGapTopDecile = diffTableData.CostGapTopDecile + "%";

    var OpportunityMedian = diffTableData.CostTarget > diffTableData.CostMedian
        ? `$${numberWithCommas(Math.round(diffTableData.OpportunityMedian))}`
        : blankCellMark;
    var OpportunityTopQuartile = diffTableData.CostTarget > diffTableData.CostTopQuartile
        ? `$${numberWithCommas(Math.round(diffTableData.OpportunityTopQuartile))}`
        : blankCellMark;
    var OpportunityTopDecile = diffTableData.CostTarget > diffTableData.CostTopDecile
        ? `$${numberWithCommas(Math.round(diffTableData.OpportunityTopDecile))}`
        : blankCellMark;

    // var OpportunityTargetMsg = blankCellMark;

    var message = "";
    if (diffTableData.CostTarget > diffTableData.CostMedian &&
        diffTableData.CostMedian > diffTableData.CostTopQuartile &&
        diffTableData.CostTopQuartile > diffTableData.CostTopDecile) {
        message = targetCompany +
            "  is performing below the Median and has SG&A opportunity when compared to Median and Top quartile. The SG&A opportunity could range between " +
            OpportunityMedian +
            "MM to " +
            OpportunityTopQuartile +
            "MM.";
    } else if (diffTableData.CostMedian == diffTableData.CostTarget &&
        diffTableData.CostTarget > diffTableData.CostTopQuartile &&
        diffTableData.CostTopQuartile > diffTableData.CostTopDecile) {
        message = targetCompany +
            " is performing at the Median level, but has SG&A opportunity when compared to Top Quartile and Top Decile. The SG&A opportunity could range between " +
            OpportunityTopQuartile +
            "MM to " +
            OpportunityTopDecile +
            "MM.";
    } else if (diffTableData.CostMedian > diffTableData.CostTarget &&
        diffTableData.CostTarget > diffTableData.CostTopQuartile &&
        diffTableData.CostTopQuartile > diffTableData.CostTopDecile) {
        message = targetCompany +
            "  is performing better than Median, but has SG&A opportunity when compared to Top Quartile and Top Decile. The SG&A opportunity could range between " +
            OpportunityTopQuartile +
            "MM to " +
            OpportunityTopDecile +
            "MM.";
    } else if (diffTableData.CostMedian > diffTableData.CostTarget &&
        diffTableData.CostTarget == diffTableData.CostTopQuartile &&
        diffTableData.CostTopQuartile > diffTableData.CostTopDecile) {
        message = targetCompany +
            "  is performing at the Top Quartile level, but has SG&A opportunity when compared to Top Decile. the SG&A opportunity could is pegged at " +
            OpportunityTopDecile +
            "MM.";
    } else if (diffTableData.CostMedian > diffTableData.CostTopQuartile &&
        diffTableData.CostTopQuartile > diffTableData.CostTarget &&
        diffTableData.CostTarget > diffTableData.CostTopDecile) {
        message = targetCompany +
            "'s overall SG&A cost performance in within the first quartile of their peer group; however, there is still a " +
            OpportunityTopDecile +
            "MM opportunity gap to attain top decile level performance.";
    } else if (diffTableData.CostMedian > diffTableData.CostTopQuartile &&
        diffTableData.CostTopQuartile > diffTableData.CostTarget &&
        diffTableData.CostTarget == diffTableData.CostTopDecile) {
        message = targetCompany + "  is a best in class performer.";
    } else if (diffTableData.CostMedian > diffTableData.CostTopQuartile &&
        diffTableData.CostTopQuartile > diffTableData.CostTopDecile &&
        diffTableData.CostTopDecile > diffTableData.CostTarget) {
        message = targetCompany + " is a best in class performer.";
    } else {
        message = targetCompany + " is a best in class performer.";
    }
    $("#SGACostPercentRevenueHeading").html(message);
};

function BindSNGChartDifferenceTableWCD(ccc) {
    const blankCellMark = " - ";
    const targetCompany = toTitleCase($("#benchmarkCompanyNameMixedCase").text()) || "";

    const oneDaySales = 2; // TODO: Get from Sandiep!
    const oneDayCost = 1; // TODO: Get from Sandiep!
    const oneDayProfit = oneDaySales - oneDayCost;

    var opportunity100 =
        ((ccc.Target.Value - ccc.P000) * oneDayProfit)
            .toFixed(0); // how much the target would gain by making it to 100% (0 is target is best)
    var opportunity090 =
        ((ccc.Target.Value - ccc.P010) * oneDayProfit)
            .toFixed(0); // how much the target would gain by making it to  90% (<0 if target is in top 10%, >=0 otherwise)
    var opportunity075 =
        ((ccc.Target.Value - ccc.P025) * oneDayProfit)
            .toFixed(0); // how much the target would gain by making it to  75% (<0 if target is in top 25%, >=0 otherwise)
    var opportunity050 =
        ((ccc.Target.Value - ccc.P050) * oneDayProfit)
            .toFixed(0); // how much the target would gain by making it to  50% (<0 if target is in top 50%, >=0 otherwise)
    var opportunity025 =
        ((ccc.Target.Value - ccc.P075) * oneDayProfit)
            .toFixed(0); // how much the target would gain by making it to  25% (<0 if target is in top 75%, >=0 otherwise)
    var opportunity010 =
        ((ccc.Target.Value - ccc.P090) * oneDayProfit)
            .toFixed(0); // how much the target would gain by making it to  10% (<0 if target is in top 90%, >=0 otherwise)
    var opportunity000 =
        ((ccc.Target.Value - ccc.P100) * oneDayProfit)
            .toFixed(0); // how much the target would gain by making it to   0% (>=0)

    opportunity100 += 0; // because sometimes we get a signed zero
    opportunity090 += 0; // because sometimes we get a signed zero
    opportunity075 += 0; // because sometimes we get a signed zero
    opportunity050 += 0; // because sometimes we get a signed zero
    opportunity025 += 0; // because sometimes we get a signed zero
    opportunity010 += 0; // because sometimes we get a signed zero
    opportunity000 += 0; // because sometimes we get a signed zero


    var message = ""; // heck of a default

    if (opportunity025 >= 0) { // The target is sucking canal water
        message = targetCompany +
            "  needs to focus on all the working capital components, where they are performing below than median. " +
            "The upper quartile comparison shows the total working capital improvement opportunity in the range of " +
            "$" +
            opportunity090 +
            "MM to " +
            "$" +
            opportunity100 +
            "MM.";
    } else if (opportunity050 >= 0) { // Target is in top 75%, but not in top 50%
        message = targetCompany +
            " is performing above the Median level, but has a working capital opportunity when compared to Top Quartile and Top Decile. This opportunity could range between " +
            "$" +
            opportunity090 +
            "MM to " +
            "$" +
            opportunity100 +
            "MM.";
    } else if (opportunity075 >= 0) { // Target is in top 50%, but not in top 25%
        message = targetCompany +
            " is performing above the Median level, but has a working capital opportunity when compared to Top Quartile and Top Decile. This opportunity could range between " +
            "$" +
            opportunity090 +
            "MM to " +
            "$" +
            opportunity100 +
            "MM.";
    } else /*if (opportunity090 > 0)*/
    { // Target is in top 50%, but not in top 25%
        message = targetCompany + " is a best-in-class performer.";
    }


    /*
        // console.log("ccc.CostTarget " + ccc.CostTarget);
        // console.log("ccc.CostMedian " + ccc.CostMedian)
        // console.log("ccc.CostTopQuartile " + ccc.CostTopQuartile)
        // console.log("ccc.CostTopDecile " + ccc.CostTopDecile)

        var tvsm = Compare(ccc.CostTarget,
            ccc.CostMedian); // 1 if target > median, -1 if target < median, 0 if equal
        var mvstq = Compare(ccc.CostMedian,
            ccc.CostTopQuartile); // 1 if median > top quartile, -1 if median < top quartile, 0 if equal
        var tqvstd =
            Compare(ccc.CostTopQuartile,
                ccc.CostTopDecile); // 1 if top quartile > top decile, -1 if top quartile < top decile, 0 if equal
        var tvstq = Compare(ccc.CostTarget,
            ccc.CostTopQuartile); // 1 if target > top quartile, -1 if target < top quartile, 0 if equal
        var tvstd = Compare(ccc.CostTarget,
            ccc.CostTopDecile); // 1 if target > top decile, -1 if target < top decile, 0 if equal

        if ((tvsm > 0) && (mvstq > 0) && (tqvstd > 0)) {
            message = targetCompany +
                "  needs to focus on all the working capital components, where they are performing below than median. " +
                "The upper quartile comparison shows the total working capital improvement opportunity in the range of " +
                opportunityMedian +
                "MM to " +
                opportunityTopQuartile +
                "MM.";
        } else if ((tvsm == 0) && (tvstq > 0) && (tqvstd > 0)) {
        } else if ((tvsm < 0) && (tvstq > 0) && (tqvstd > 0)) {
            message = targetCompany +
                " is performing at the Median level, but has a working capital when compared to Top Quartile and Top Decile. This opportunity could range between " +
                opportunityTopQuartile +
                "MM to " +
                opportunityTopDecile +
                "MM.";
        } else if ((tvsm < 0) && (tvstq == 0) && (tqvstd > 0)) {
            message = targetCompany +
                "  is performing at the Top Quartile level, but has a working capital opportunity when compared to Top Decile. This opportunity is pegged at " +
                opportunityTopDecile +
                "MM.";
        } else if ((mvstq > 0) && (tvstq < 0) && (tvstd > 0)) {
            message = targetCompany +
                " is performing better than Top Quartile, but has a working capital opportunity when compared to Top Decile. This opportunity is pegged at " +
                opportunityTopDecile +
                "MM.";
        }
    }
    */

    $("#SGACostPercentRevenueHeading").html(message);
};

function Compare(/*decimal*/ a, /*decimal*/ b) {
    if (a > b) return 1;
    if (b > a) return -1;
    return 0;
}

//Plotting amcharts data in application. There are various property of chart can set under this function(BOLD, WIDTH, TITLE etc).
function GenerateSGAChart2(reportData) {
    if (reportData.DifferenceTableData == undefined)
        return;
    //console.log("entering GenerateSGAChart2");

    var data = {
        Data1: reportData.ChartData.ProviderData[0].PeerCompanyValue.toFixed(2),
        Data2: reportData.DifferenceTableData.CostTopQuartile.toFixed(2),
        Data3: reportData.DifferenceTableData.CostMedian.toFixed(2),
        Data4: reportData.DifferenceTableData.CostBottomQuartile.toFixed(2),
        Data5: reportData.ChartData.ProviderData[reportData.ChartData.ProviderData.length - 1].PeerCompanyValue
            .toFixed(2),
        Data6: reportData.DifferenceTableData.CostTarget.toFixed(2),
        LC: "#ffffff"
    };

    var target = toTitleCase($("#benchmarkCompanyNameMixedCase").text()) || "";

    var arr = [data.Data1, data.Data2, data.Data3, data.Data4, data.Data5, data.Data6];
    var max = Math.max.apply(null, arr);
    var min = Math.min.apply(null, arr);
    if (max == arr[5]) {
        max = max + (parseFloat(arr[5]) - parseFloat(arr[4]));
    }
    min = parseFloat(min) - 0.5;
    var dtd = reportData.DifferenceTableData;
    var thickness = (dtd.CostMax - dtd.CostMin) / 100.0;

    var chart = AmCharts.makeChart("chartdiv2",
        {
            "type": "serial",
            "color": "#646464", // Update text color to match legend (PEER-16)
            "fontFamily": "Arial, Helvetica, 'sans-serif'", // Updated text font to match legend (PEER-16)
            "startDuration": 0,
            "columnWidth": 0.95,
            "depth3D": 0,
            "startEffect": "easeOutSine",
            "categoryField": "Category",
            "valueAxes": [
                {
                    "axisColor": "#AAB3B3",
                    "offset": 1,
                    "strictMinMax": true,
                    "minimum": min,
                    "maximum": max,
                    "id": "ValueAxis-1",
                    "stackType": "regular",
                    "gridThickness": 0,
                    "unit": "%",
                }
            ],

            "guides": [
                {
                    "above": true,
                    "id": "Guide-1",
                    "inside": true,
                    "label": dtd.CostTarget.toFixed(2) + "% - " + splitInTwoWord(target),
                    "lineAlpha": 1,
                    "lineColor": "#5E8AB4",
                    "lineThickness": 3,
                    "position": "left",
                    "tickLength": 15,
                    "toAngle": 0,
                    "fontSize": 9,
                    "value": dtd.CostTarget.toFixed(2),
                    "balloonText": `<span style='font-size:10px;color:#000000;'>${dtd.CostTarget.toFixed(2)}% - ${target
                        }</span>`,
                },
                /*
                {
                    "above": true,
                    "id": "Guide-2",
                    "inside": true,
                    //"label": dtd.CostMax.toFixed(2) + "%",
                    "lineAlpha": 1,
                    "lineColor": "#5E8AB4",
                    "lineThickness": 3,
                    "position": "left",
                    "tickLength": 15,
                    "toAngle": 0,
                    "fontSize": 9,
                    "value": dtd.CostMax.toFixed(2) - thickness,
                    "toValue": dtd.CostMax.toFixed(2),
                    "balloonText": "test"
                },
                */
            ],
            "categoryAxis": {
                "axisColor": "#AAB3B3",
                "labelsEnabled": false,
                "gridPosition": "start",
                "gridThickness": 1,
                "gridAlpha": 0,
                "tickPosition": "start",
                "dashLength": 0,
            },
            "trendLines": [],
            "graphs": [
                {
                    "precision": 2,
                    "fillAlphas": 1,
                    "fillColors": "#5E8AB4",
                    "lineColor": "#5E8AB4",
                    "lineThickness": 1,
                    "id": "AmGraph-1",
                    "openField": "Data1",
                    "closeField": "Data2",
                    "title": "graph 1",
                    "type": "column",
                    "labelText": "[[Data1]]% - Max",
                    "balloonText": "[[Data1]]% - Max",
                    "fontSize": 9,
                    "columnWidth": 0.01,
                    "showAllValueLabels": true,
                    "labelPosition": "inside",
                    "labelOffset": -15,
                    "lineColorField": "line1",
                    "dashLengthField": "dLF"
                },
                {
                    "precision": 2,
                    "fillAlphas": 1,
                    "fillColors": "#29702A",
                    "lineColor": "#c3c3c3",
                    "lineThickness": 1,
                    "id": "AmGraph-2",
                    "openField": "Data2",
                    "closeField": "Data3",
                    "title": "graph 2",
                    "type": "column",
                    "labelText": "[[Data2]]% - Top Quartile",
                    "balloonText": "[[Data2]]% - Top Quartile",
                    "labelColorField": "LC",
                    "labelOffset": -2,
                    "labelPosition": "inside",
                    "fontSize": 9,
                    "showAllValueLabels": true,
                    "lineColorField": "line1",
                    "dashLengthField": "dLF",
                    "bulletOffset": -25,
                    "bulletSize": 24,
                    "columnWidth": 1,
                    "customBulletField": "bullet",
                },
                {
                    "precision": 2,
                    "id": "AmGraph-3",
                    "title": "graph 3",
                    "labelText": "[[Data3]]% - Median",
                    "balloonText": "[[Data3]]% - Median",
                    "labelPosition": "inside",
                    "openField": "Data3",
                    "type": "column",
                    "closeField": "Data4",
                    "fillAlphas": 1,
                    "fillColors": "#F9C20A",
                    "lineColor": "#c3c3c3",
                    "lineThickness": 1,
                    "fontSize": 9,
                    "columnWidth": 1,
                    "showAllValueLabels": true
                },
                {
                    "precision": 2,
                    "id": "AmGraph-4",
                    "title": "graph 4",
                    "labelText": "[[Data4]]% - Bottom Quartile",
                    "balloonText": "[[Data4]]% - Bottom Quartile",
                    "labelPosition": "inside",
                    "openField": "Data4",
                    "type": "column",
                    "closeField": "Data5",
                    "fillAlphas": 1,
                    "fillColors": "#5E8AB4",
                    "lineColor": "#5E8AB4",
                    "columnWidth": 0.01,
                    "lineThickness": 1,
                    "fontSize": 9,
                    "showAllValueLabels": true
                },
                {
                    "precision": 2,
                    "fillAlphas": 0,
                    "lineThickness": 0,
                    "id": "AmGraph-5",
                    "title": "graph 5",
                    "labelText": "[[Data5]]% - Min",
                    "balloonText": "[[Data5]]% - Min",
                    "valueField": "Data5",
                    "fontSize": 9,
                    "lineColor": "#ffffff",
                    "showAllValueLabels": true,
                    "bulletSize ": 0
                },
            ],

            "balloon": {},
            "dataProvider": [data] /*,
            "listeners": [
                {
                    "event": "drawn",
                    "method": function (e) { console.log("GenerateSGAChart2 drawn event"); }
                },
                {
                    "event": "animationFinished",
                    "method": function (e) { console.log("GenerateSGAChart2 animationFinished event"); }
                },
                {
                    "event": "rendered",
                    "method": function (e) { console.log("GenerateSGAChart2 rendered event"); }
                }
            ]*/

        });
    //console.log("Exiting GenerateSGAChart2");
}

function GenerateWCDChart2(dataCollection) {

    if (dataCollection == undefined) {
        console.log("No difference data");
        return;
    }
    //console.log("Entering GenerateWCDChart2");

    var data = {
        Data1: dataCollection.P100.toFixed(0),
        Data2: dataCollection.P075.toFixed(0),
        Data3: dataCollection.P050.toFixed(0),
        Data4: dataCollection.P025.toFixed(0),
        Data5: dataCollection.P000.toFixed(0),
        Data6: dataCollection.TargetValue.toFixed(0),
        LC: "#ffffff"
    };

    // console.log(JSON.stringify(data));

    var target = toTitleCase($("#headerBenchmarkCompany").text()) || "";

    var arr = [data.Data1, data.Data2, data.Data3, data.Data4, data.Data5, data.Data6];
    var max = Math.max.apply(null, arr);
    var min = Math.min.apply(null, arr);
    if (max == arr[5]) {
        max = max + (parseFloat(arr[5]) - parseFloat(arr[4]));
    }
    min = parseFloat(min) - 0.5;

    var chart = AmCharts.makeChart("chartdiv2",
        {
            "type": "serial",
            "color": "#646464", // Update text color to match legend (PEER-16)
            "fontFamily": "Arial, Helvetica, 'sans-serif'", // Updated text font to match legend (PEER-16)
            "startDuration": 0,
            "columnWidth": 0.95,
            "depth3D": 0,
            "startEffect": "easeOutSine",
            "categoryField": "Category",
            "valueAxes": [
                {
                    "axisColor": "#AAB3B3",
                    "offset": 1,
                    "strictMinMax": true,
                    "minimum": min,
                    "maximum": max,
                    "id": "ValueAxis-1",
                    "stackType": "regular",
                    "gridThickness": 0,
                    "unit": " days",
                }
            ],

            "guides": [
                {
                    "above": true,

                    "id": "Guide-1",
                    "inside": true,
                    "label": dataCollection.Target.Value.toFixed(0) + " days - " + dataCollection.Target.DisplayName,
                    "lineAlpha": 1,
                    "lineColor": "#5E8AB4",
                    "lineThickness": 3,
                    "position": "left",
                    "tickLength": 15,
                    "toAngle": 0,
                    "fontSize": 9,
                    "value": dataCollection.Target.Value.toFixed(0) + " days",
                    "balloonText": `<span style='font-size:10px;color:#000000;'>${dataCollection.Target.Value.toFixed(0)
                        } days - ${dataCollection.Target.DisplayName}</span>`,
                }
            ],
            "categoryAxis": {
                "axisColor": "#AAB3B3",
                "labelsEnabled": false,
                "gridPosition": "start",
                "gridThickness": 1,
                "gridAlpha": 0,
                "tickPosition": "start",
                "dashLength": 0,
            },
            "trendLines": [],
            "graphs": [
                {
                    "precision": 0,
                    "fillAlphas": 1,
                    "fillColors": "#5E8AB4",
                    "lineColor": "#5E8AB4",
                    "lineThickness": 1,
                    "id": "AmGraph-1",
                    "openField": "Data1",
                    "closeField": "Data2",
                    "title": "graph 1",
                    "type": "column",
                    "labelText": "[[Data1]] days - Max",
                    "balloonText": "[[Data1]] days - Max",
                    "fontSize": 9,
                    "columnWidth": 0.01,
                    "showAllValueLabels": true,
                    "labelPosition": "inside",
                    "labelOffset": -15,
                    "lineColorField": "line1",
                    "dashLengthField": "dLF"
                },
                {
                    "precision": 0,
                    "fillAlphas": 1,
                    "fillColors": "#29702A",
                    "lineColor": "#c3c3c3",
                    "lineThickness": 1,
                    "id": "AmGraph-2",
                    "openField": "Data2",
                    "closeField": "Data3",
                    "title": "graph 2",
                    "type": "column",
                    "labelText": "[[Data2]] days - Top Quartile",
                    "balloonText": "[[Data2]] days - Top Quartile",
                    "labelColorField": "LC",
                    "labelOffset": -2,
                    "labelPosition": "inside",
                    "fontSize": 9,
                    "showAllValueLabels": true,
                    "lineColorField": "line1",
                    "dashLengthField": "dLF",
                    "bulletOffset": -25,
                    "bulletSize": 24,
                    "columnWidth": 1,
                    "customBulletField": "bullet",
                },
                {
                    "precision": 0,
                    "id": "AmGraph-3",
                    "title": "graph 3",
                    "labelText": "[[Data3]] days - Median",
                    "balloonText": "[[Data3]] days - Median",
                    "labelPosition": "inside",
                    "openField": "Data3",
                    "type": "column",
                    "closeField": "Data4",
                    "fillAlphas": 1,
                    "fillColors": "#F9C20A",
                    "lineColor": "#c3c3c3",
                    "lineThickness": 1,
                    "fontSize": 9,
                    "columnWidth": 1,
                    "showAllValueLabels": true
                },
                {
                    "precision": 0,
                    "id": "AmGraph-4",
                    "title": "graph 4",
                    "labelText": "[[Data4]] days - Bottom Quartile",
                    "balloonText": "[[Data4]] days - Bottom Quartile",
                    "labelPosition": "inside",
                    "openField": "Data4",
                    "type": "column",
                    "closeField": "Data5",
                    "fillAlphas": 1,
                    "fillColors": "#5E8AB4",
                    "lineColor": "#5E8AB4",
                    "columnWidth": 0.01,
                    "lineThickness": 1,
                    "fontSize": 9,
                    "showAllValueLabels": true
                },
                {
                    "precision": 0,
                    "fillAlphas": 0,
                    "lineThickness": 0,
                    "id": "AmGraph-5",
                    "title": "graph 5",
                    "labelText": "[[Data5]] days - Min",
                    "balloonText": "[[Data5]] days - Min",
                    "valueField": "Data5",
                    "fontSize": 9,
                    "lineColor": "#ffffff",
                    "showAllValueLabels": true,
                    "bulletSize ": 0
                },
            ],

            "balloon": {},
            "dataProvider": [data] /*,
            "listeners": [
                {
                    "event": "drawn",
                    "method": function (e) { console.log("GenerateWCDChart2 drawn event"); }
                },
                {
                    "event": "animationFinished",
                    "method": function (e) { console.log("GenerateWCDChart2 animationFinished event"); }
                },
                {
                    "event": "rendered",
                    "method": function (e) { console.log("GenerateWCDChart2 rendered event"); }
                }
            ]*/

        });
    //console.log("Exiting GenerateWCDChart2");
}

function formatDecimalInComma(value) {
    if (value != undefined) {
        return value.toFixed(2).replace(/(\d)(?=(\d{3})+\.)/g, "$1,");
    } else {
        return 0.00 .toFixed(2).replace(/(\d)(?=(\d{3})+\.)/g, "$1,");
    }
}

function numberWithCommas(x) {
    return x.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
}