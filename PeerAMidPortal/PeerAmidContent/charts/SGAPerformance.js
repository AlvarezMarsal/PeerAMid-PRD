//if (!wcd)
//    plotSGAPerformenceChart();

//SG&A vs. EBITDA Performance*
function plotSGAPerformanceChart(wcd, doAsync) {
    if (wcd)
        return { done: function (f) { f(); } };

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

    console.log("Calling ajax: '/api/HomeAPI/GetSGAPerformanceChartData'");
    return $.ajax({
        async: doAsync,
        type: "GET",
        url: "/api/HomeAPI/GetSGAPerformanceChartData",
        data: postData,
        dataType: "json",
        headers: config,
        //async: false,
        complete: function() {
            // $(".loader-div").hide();
        },
        success: function(reportData) {
            if (reportData != null && reportData != undefined) {
                GenerateSGAPerformanceChart(reportData);
            }
        },
        error: function(_, _, err) {
            console.log("error");
            console.log(err);
        }
    });
}

//Chart binding logic.
function GenerateSGAPerformanceChart(reportData) {
    var chartSection = $("#divPerformanceChartId");
    chartSection.html("");
    var colorClass = "";
    $.each(reportData.SGAPerformanceData,
        function(index, item) {
            colorClass = item.IsTarget ? "greenDotPDF" : "blueDotPDF";

            const leftPosition = item.XValue * 100;
            const bottomPosition = item.YValue * 100;
            const peer = item.PeerCompanyDisplayName;
            const dotItem =
                `<span data-toggle="popover" data-html="true"  data-trigger="hover" data-content="Company:<b>${item
                    .PeerCompanyDisplayName
                    }</b> <br>EBITDA Margin:<b>${item.EBITDAMarginValue}%</b><br>SGA Margin:<b>${item.SGAMarginValue
                    }%</b>" data-placement="left" class="dots popB4d${index} ${colorClass
                    }" style="left:${leftPosition}%; bottom:${bottomPosition}%;" ><span class="BGWhite">${peer
                    }</span></span>`;
            chartSection.append(dotItem);
        });

    SetPerformanceChartSummaryLogic(reportData.SGAPerformanceData);
    $('[data-toggle="popover"]').popover();
}

//Split and replace function: replace , from word in '' format.
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

//Business logic put down under this function for bind the chart.
function SetPerformanceChartSummaryLogic(performanceChartData) {
    $("#divPerformanceChartSummaryLine").val("");
    var bestInClassPerformers = "";
    var targetXValue = 0;
    var targetYValue = 0;
    var companyNames = [];

    try {
        $.each(performanceChartData,
            function(index, item) {
                if (item.IsTarget == true) {
                    targetXValue = item.XValue;
                    targetYValue = item.YValue;
                } else if (item.XValue >= 0.75 && item.YValue >= 0.75) {
                    companyNames.push(item.PeerCompanyDisplayName);
                }
            });

        if (companyNames.length == 1) {
            bestInClassPerformers = `, ${companyNames[0]} is best in class player`;
        } else if (companyNames.length > 1) {
            var isFirstCall = true;
            var counter = 0;

            $.each(companyNames,
                function(index, peerName) {
                    counter++;

                    if (isFirstCall) {
                        isFirstCall = false;
                        bestInClassPerformers = `, ${peerName}`;
                    } else {
                        if (companyNames.length == counter) {
                            bestInClassPerformers =
                                bestInClassPerformers + " and " + peerName + " are best in class players";
                        } else {
                            bestInClassPerformers = bestInClassPerformers + ", " + peerName;
                        }
                    }
                });
        }
        const targetCompany = $("#benchmarkCompanyNameMixedCase").text();
        bestInClassPerformers = toTitleCase(bestInClassPerformers);
        let phrase =
            "Optimizing SG&A spend can liberate capital for more strategic leverage within operations or drive savings to the bottom line. The top right quadrant represents top performers, and the bottom left quadrant represents companies performing below the Median.  ";

        if (targetXValue <= 0.5 && targetYValue <= 0.5) {
            phrase += targetCompany +
                " has below Median overall performance. They have significant opportunity for improvement as compared to Median and best in class performers.";
        } else if ((targetXValue >= 0.5 && targetYValue <= 0.5) || (targetXValue <= 0.5 && targetYValue >= 0.5)) {
            phrase += targetCompany +
                " has near Median overall performance. They have significant opportunity for improvement compared to best in class performers.";
        } else if (targetXValue >= 0.75 && targetYValue >= 0.75) {
            phrase += targetCompany + " has best in class overall performance.";
        } else {
            phrase += targetCompany +
                " has above Median overall performance.  They have significant opportunity for improvement compared to best in class performers.";
        }

        $("#divPerformanceChartSummaryLine").html(phrase);
    } catch (ex) {
    }
}