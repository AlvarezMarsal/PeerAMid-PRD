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
    var targetXValue = 0;
    var targetYValue = 0;
    var companyNames = [];

    try {
        $.each(performanceChartData,
            function(index, item) {
                if (item.IsTarget == true) {
                    targetXValue = item.XValue;
                    targetYValue = item.YValue;
                }
                if ((item.XValue >= 0.75) && (item.YValue >= 0.75)) {
                    companyNames.push(item.PeerCompanyDisplayName);
                }
            });

        let bestInClassPhrase = OxfordComma(companyNames);
        if (companyNames.length == 1) {
            bestInClassPhrase += " is a best-in-class player and is managing its SG&A and Profitability effectively.";
        } else if (companyNames.length > 1) {
            bestInClassPhrase += " are best-in-class players and are managing their SG&A and Profitability effectively.";
        }

        const targetCompany = $("#benchmarkCompanyNameMixedCase").text();
        //let oldInitialPhrase =
        //    "Optimizing SG&A spend can liberate capital for more strategic leverage within operations or drive savings to the bottom line. The top right quadrant represents top performers, and the bottom left quadrant represents companies performing below the Median.";
        let initialPhrase = "";
        let conditionPhrase = "";

        if (targetXValue < 0.25) {

            if (targetYValue < 0.25) {
                // SK Condition 1
                conditionPhrase = targetCompany + " is in the Bottom Quartile for both SG&A efficiency and EBITDA performance, indicating significant challenges in both cost management and profitability. There is substantial room for improvement across the board.";
            } else if (targetYValue < 0.50) {
                // SK Condition 2
                conditionPhrase = targetCompany + " is in the Bottom Quartile for SG&A efficiency but near Median for EBITDA performance. While profitability is close to average, cost management practices require considerable enhancement.";
            } else if (targetYValue < 0.75) {
                // SK Condition 5
                conditionPhrase = targetCompany + "'s SG&A efficiency is in the Bottom Quartile, but their EBITDA performance is above Median. While profitability is strong, cost management practices need substantial improvement.";
            } else { // (targetYValue >= 0.75)
                // SK Condition 15
                conditionPhrase = targetCompany + " has bottom-quartile SG&A efficiency, indicating high costs relative to peers. However, their EBITDA performance is best-in-class, suggesting that despite high costs, the company is able to generate strong profitability. This may indicate an opportunity to improve cost efficiency without compromising profitability.";
            }

        } else if (targetXValue < 0.50) {

            if (targetYValue < 0.25) {
                // SK Condition 3
                conditionPhrase = targetCompany + " is near Median for SG&A efficiency but in the Bottom Quartile for EBITDA performance. Profitability is a significant concern, despite relatively average cost management.";
            } else if (targetYValue < 0.50) {
                // SK Condition 4
                conditionPhrase = targetCompany + " has near Median overall performance. They have significant opportunities for improvement as compared to best-in-class performers.";
            } else if (targetYValue < 0.75) {
                // SK Condition 6
                conditionPhrase = targetCompany + " is near Median in SG&A efficiency and above Median in EBITDA performance. Overall, they are performing better than average but have room for improvement.";
            } else { // (targetYValue >= 0.75) {
                // SK Condition 16
                conditionPhrase = targetCompany + " has near-median SG&A efficiency but achieves best-in-class EBITDA performance. This indicates that while there may be some room for improvement in cost management, the company is excelling in profitability, possibly due to strong revenue generation or other operational efficiencies.";
            }

        } else if (targetXValue < 0.75) {

            if (targetYValue < 0.25) {
                // SK Condition 7
                conditionPhrase = targetCompany + "'s SG&A efficiency is above Median, but their EBITDA performance is in the Bottom Quartile. Despite relatively good cost management, profitability is a critical area needing improvement.";
            } else if (targetYValue < 0.50) {
                // SK Condition 8
                conditionPhrase = targetCompany + " has above Median SG&A efficiency, but EBITDA performance is near Median. They are performing better than average in cost management, but profitability still has room for enhancement.";
            } else if (targetYValue < 0.75) {
                // SK Condition 9
                conditionPhrase = targetCompany + " has above Median overall performance. They are performing well, but there is still opportunity for improvement as compared to best-in-class performers.";
            } else { // (targetYValue >= 0.75) {
                // SK Condition 10
                conditionPhrase = targetCompany + "'s SG&A efficiency is above Median, and EBITDA performance is best-in-class. They are strong in both cost management and profitability, but further improvements could lead to top quartile SG&A performance.";
            }
        } else { // (targetXValue >= 0.75) {

            if (targetYValue < 0.25) {
                // SK Condition 11
                conditionPhrase = targetCompany + "'s SG&A efficiency is best-in-class, but their EBITDA performance is in the Bottom Quartile. Despite excellent cost management, profitability is a significant concern and needs to be addressed.";
            } else if (targetYValue < 0.50) {
                // SK Condition 12
                conditionPhrase = targetCompany + " has best-in-class SG&A efficiency, but EBITDA performance is near Median. They excel in cost management but need to improve profitability to match their cost efficiency.";
            } else if (targetYValue < 0.75) {
                // SK Condition 13
                conditionPhrase = targetCompany + " has best-in-class SG&A efficiency, and their EBITDA performance is above Median. They are strong in cost management and have good profitability, but there is potential to elevate their performance even further.";
            } else { // (targetYValue >= 0.75) {
                // SK Condition 14
                conditionPhrase = targetCompany + " has best-in-class overall performance. They excel in both cost management and profitability, positioning them as a leader in the industry.";
            }
        }

        let phrase = initialPhrase;
        if (phrase.length > 0)
            phrase += "<br>";
        if (bestInClassPhrase.length > 0) {
            phrase += bestInClassPhrase + "<br>";
        }
        phrase += conditionPhrase;

        $("#divPerformanceChartSummaryLine").html(phrase);

    } catch (ex) {
    }
}