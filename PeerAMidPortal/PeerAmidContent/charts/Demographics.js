// plotDemographicChart(wcd);

//Peer Group Demographics
function plotDemographicChart(wcd, gsd, doAsync) {

    const selectedPeers = analysisState.peerCompanies;
    const selectedTargetCompany = analysisState.benchmarkCompany.CompanyId;
    const benchMarkCompYear = analysisState.benchmarkCompany.FinancialYear;

    //const optionId = $("#optionId").val() || 0;
    const postData = {
        selectedPeerList: selectedPeers,
        targetCompanyId: selectedTargetCompany,
        year: benchMarkCompYear,
        optionId: 1 // optionId
    };

    console.log("Calling ajax: '/api/HomeAPI/GetDemographicChartData'");
    //console.log(config);
    //console.log(JSON.stringify(postData));

    return $.ajax({
        async: doAsync,
        type: "Get",
        url: "/api/HomeAPI/GetDemographicChartData",
        data: postData,
        contentType: "application/json",
        headers: config,
        success: function(demographicModel) {
            //console.log("success");
            //console.log(demographicModel);
            if ((demographicModel != null) && (demographicModel != undefined))
                GenerateDemographicChart(demographicModel, wcd, gsd);
            //$(".loader-div").hide();
        },
        error: function(_, _, err) {
            console.log("error");
            console.log(err);
        }
    });
}

//Custom logic for pointing position Min, 25Percentage, Medium, 75 percentage, Max in chart.
function GenerateDemographicChart(demographicModel, wcd, gsd) {
    // console.log("GenerateDemographicChart");
    // console.log(demographicModel);

    //debugger;
    if (demographicModel.RevenueData != undefined) {

        var v;

        try {
            v = gsd.SelectedCurrency.FormatLargeValue(Math.round(demographicModel.RevenueData.Minimum), 0);
            $("#divRevenueMin").html(v);
        } catch(e) {
        }

        
        
        try {
            v = gsd.SelectedCurrency.FormatLargeValue(Math.round(demographicModel.RevenueData.Percentile25), 0);
            $("#divRevenueP25").html(v);
        } catch(e) {
        }

        
        
        try {
            v = gsd.SelectedCurrency.FormatLargeValue(Math.round(demographicModel.RevenueData.Percentile50), 0);
            $("#divRevenueP50").html(v);
        } catch(e) {
        }

        
        
        try {
            v = gsd.SelectedCurrency.FormatLargeValue(Math.round(demographicModel.RevenueData.Percentile75), 0);
            $("#divRevenueP75").html(v);
        } catch(e) {
        }

        
        
        try {
            v = gsd.SelectedCurrency.FormatLargeValue(Math.round(demographicModel.RevenueData.Maximum), 0);
            $("#divRevenueMax").html(v);
        } catch(e) {
        }

        
        
        try {
            var RevenueStarValue = demographicModel.RevenueData.StarValue;
            var RevenueLocationPercent = DemographicStarLocation(demographicModel.RevenueData.Minimum,
                demographicModel.RevenueData.Percentile25,
                demographicModel.RevenueData.Percentile50,
                demographicModel.RevenueData.Percentile75,
                demographicModel.RevenueData.Maximum,
                demographicModel.RevenueData.StarValue);
            $("#RevenueLocationPercent")
                .html(`<span class="fas fa-star-sharp starColor"></span>$${numberWithCommas(
                    Math.round(RevenueStarValue))}MM`);
            if (RevenueLocationPercent < 0) {
                RevenueLocationPercent = 0;
            } else if (RevenueLocationPercent > 100) {
                RevenueLocationPercent = 100;
            }
            $("#RevenueLocationPercent").css("left", RevenueLocationPercent + "%");
            v = gsd.SelectedCurrency.Name;
            $("#RevenueTitle").html(`Net Revenue\n(in ${v})`);
        } catch(e) {
        }
    
    
    }

    if (demographicModel.CAGRData != undefined) {
        var hasDataForTarget = !demographicModel.CAGRData.IsCAGRDiv0;
        demographicModel.CAGRData.Minimum = demographicModel.CAGRData.Minimum.toFixed(2);
        demographicModel.CAGRData.Percentile25 = demographicModel.CAGRData.Percentile25.toFixed(2);
        demographicModel.CAGRData.Percentile50 = demographicModel.CAGRData.Percentile50.toFixed(2);
        demographicModel.CAGRData.Percentile75 = demographicModel.CAGRData.Percentile75.toFixed(2);
        demographicModel.CAGRData.Maximum = demographicModel.CAGRData.Maximum.toFixed(2);
        $("#divCAGRPMin").html(demographicModel.CAGRData.Minimum + "%");
        $("#divCAGRP25").html(demographicModel.CAGRData.Percentile25 + "%");
        $("#divCAGRP50").html(demographicModel.CAGRData.Percentile50 + "%");
        $("#divCAGR75").html(demographicModel.CAGRData.Percentile75 + "%");
        $("#divCAGRMax").html(demographicModel.CAGRData.Maximum + "%");
        var CAGRStarValue = demographicModel.CAGRData.StarValue.toFixed(2);
        if (hasDataForTarget) {
            var CAGRLocationPercent = DemographicStarLocation(demographicModel.CAGRData.Minimum,
                demographicModel.CAGRData.Percentile25,
                demographicModel.CAGRData.Percentile50,
                demographicModel.CAGRData.Percentile75,
                demographicModel.CAGRData.Maximum,
                CAGRStarValue);
            if (CAGRLocationPercent < 0) {
                CAGRLocationPercent = 0;
            } else if (CAGRLocationPercent > 100) {
                CAGRLocationPercent = 100;
            }
            $("#divCAGRLocationPercent")
                .html(`<span class="fas fa-star-sharp starColor"></span>${numberWithCommas(CAGRStarValue)}%`);
            $("#divCAGRLocationPercent").css("left", CAGRLocationPercent + "%");
            $("#CAGRTitle").html("Revenue\n3 Year CAGR");
        } else {
            $("#divCAGRLocationPercent").html("CAGR is not available for the selected client");
        }
    }

    if (demographicModel.GrossMarginData != undefined) {
        demographicModel.GrossMarginData.Minimum = demographicModel.GrossMarginData.Minimum.toFixed(2);
        demographicModel.GrossMarginData.Percentile25 = demographicModel.GrossMarginData.Percentile25.toFixed(2);
        demographicModel.GrossMarginData.Percentile50 = demographicModel.GrossMarginData.Percentile50.toFixed(2);
        demographicModel.GrossMarginData.Percentile75 = demographicModel.GrossMarginData.Percentile75.toFixed(2);
        demographicModel.GrossMarginData.Maximum = demographicModel.GrossMarginData.Maximum.toFixed(2);
        $("#divGROSSMin").html(demographicModel.GrossMarginData.Minimum + "%");
        $("#divGROSS25").html(demographicModel.GrossMarginData.Percentile25 + "%");
        $("#divGROSS50").html(demographicModel.GrossMarginData.Percentile50 + "%");
        $("#divGROSS75").html(demographicModel.GrossMarginData.Percentile75 + "%");
        $("#divGROSSMax").html(demographicModel.GrossMarginData.Maximum + "%");
        var GrossMarginStarValue = demographicModel.GrossMarginData.StarValue.toFixed(2);
        var GrossMarginLocationPercent = DemographicStarLocation(demographicModel.GrossMarginData.Minimum,
            demographicModel.GrossMarginData.Percentile25,
            demographicModel.GrossMarginData.Percentile50,
            demographicModel.GrossMarginData.Percentile75,
            demographicModel.GrossMarginData.Maximum,
            GrossMarginStarValue);
        $("#divGrossMarginLocationPercent")
            .html(`<span class="fas fa-star-sharp starColor"></span>${GrossMarginStarValue}%`);
        if (GrossMarginLocationPercent < 0) {
            GrossMarginLocationPercent = 0;
        } else if (GrossMarginLocationPercent > 100) {
            GrossMarginLocationPercent = 100;
        }

        $("#divGrossMarginLocationPercent").css("left", GrossMarginLocationPercent + "%");
        $("#GrossMarginTitle").html("Gross Margin %");
    }

    if (demographicModel.EBITDAData != undefined) {
        demographicModel.EBITDAData.Minimum = demographicModel.EBITDAData.Minimum.toFixed(2);
        demographicModel.EBITDAData.Percentile25 = demographicModel.EBITDAData.Percentile25.toFixed(2);
        demographicModel.EBITDAData.Percentile50 = demographicModel.EBITDAData.Percentile50.toFixed(2);
        demographicModel.EBITDAData.Percentile75 = demographicModel.EBITDAData.Percentile75.toFixed(2);
        demographicModel.EBITDAData.Maximum = demographicModel.EBITDAData.Maximum.toFixed(2);
        $("#divEBITDAMin").html(demographicModel.EBITDAData.Minimum + "%");
        $("#divEBITDA25").html(demographicModel.EBITDAData.Percentile25 + "%");
        $("#divEBITDA50").html(demographicModel.EBITDAData.Percentile50 + "%");
        $("#divEBITDA75").html(demographicModel.EBITDAData.Percentile75 + "%");
        $("#divEBITDAMax").html(demographicModel.EBITDAData.Maximum + "%");
        var EBITDAStarValue = demographicModel.EBITDAData.StarValue.toFixed(2);
        var EBITDALocationPercent = DemographicStarLocation(demographicModel.EBITDAData.Minimum,
            demographicModel.EBITDAData.Percentile25,
            demographicModel.EBITDAData.Percentile50,
            demographicModel.EBITDAData.Percentile75,
            demographicModel.EBITDAData.Maximum,
            EBITDAStarValue);

        $("#divEBITDALocationPercent").html(`<span class="fas fa-star-sharp starColor"></span>${EBITDAStarValue}%`);
        if (EBITDALocationPercent < 0) {
            EBITDALocationPercent = 0;
        } else if (EBITDALocationPercent > 100) {
            EBITDALocationPercent = 100;
        }
        $("#divEBITDALocationPercent").css("left", EBITDALocationPercent + "%");
        $("#EBITDATitle").html("EBITDA Margin %");
    }

    if (demographicModel.NumEmployeeData != undefined) {
        $("#divNumEmpMin").html(numberWithCommas(parseInt(demographicModel.NumEmployeeData.Minimum)));
        $("#divNumEmp25").html(numberWithCommas(parseInt(demographicModel.NumEmployeeData.Percentile25)));
        $("#divNumEmp50").html(numberWithCommas(parseInt(demographicModel.NumEmployeeData.Percentile50)));
        $("#divNumEmp75").html(numberWithCommas(parseInt(demographicModel.NumEmployeeData.Percentile75)));
        $("#divNumEmpMax").html(numberWithCommas(parseInt(demographicModel.NumEmployeeData.Maximum)));
        var NumEmpStarValue = parseInt(demographicModel.NumEmployeeData.StarValue);
        var NumEmpLocationPercent = parseInt(DemographicStarLocation(demographicModel.NumEmployeeData.Minimum,
            demographicModel.NumEmployeeData.Percentile25,
            demographicModel.NumEmployeeData.Percentile50,
            demographicModel.NumEmployeeData.Percentile75,
            demographicModel.NumEmployeeData.Maximum,
            NumEmpStarValue));
        $("#divNumEmpLocationPercent")
            .html(`<span class="fas fa-star-sharp starColor"></span>${numberWithCommas(NumEmpStarValue)}`);
        if (NumEmpLocationPercent < 0) {
            NumEmpLocationPercent = 0;
        } else if (NumEmpLocationPercent > 100) {
            NumEmpLocationPercent = 100;
        }
        $("#divNumEmpLocationPercent").css("left", NumEmpLocationPercent + "%");
        $("#NumEmpTitle").html("Number of Employees");
    }

    if (demographicModel.RevenuePerEmployeeData != undefined) {
        $("#divRevenuePerEmpMin")
            .html(`$${numberWithCommas(parseInt(demographicModel.RevenuePerEmployeeData.Minimum))}`);
        $("#divRevenuePerEmp25")
            .html(`$${numberWithCommas(parseInt(demographicModel.RevenuePerEmployeeData.Percentile25))}`);
        $("#divRevenuePerEmp50")
            .html(`$${numberWithCommas(parseInt(demographicModel.RevenuePerEmployeeData.Percentile50))}`);
        $("#divRevenuePerEmp75")
            .html(`$${numberWithCommas(parseInt(demographicModel.RevenuePerEmployeeData.Percentile75))}`);
        $("#divRevenuePerEmpMax")
            .html(`$${numberWithCommas(parseInt(demographicModel.RevenuePerEmployeeData.Maximum))}`);
        var RevenuePerEmpStarValue = parseInt(demographicModel.RevenuePerEmployeeData.StarValue);
        var RevenuePerEmpLocationPercent = parseInt(DemographicStarLocation(
            demographicModel.RevenuePerEmployeeData.Minimum,
            demographicModel.RevenuePerEmployeeData.Percentile25,
            demographicModel.RevenuePerEmployeeData.Percentile50,
            demographicModel.RevenuePerEmployeeData.Percentile75,
            demographicModel.RevenuePerEmployeeData.Maximum,
            demographicModel.RevenuePerEmployeeData.StarValue));
        $("#divRevenuePerEmpLocationPercent")
            .html(`<span class="fas fa-star-sharp starColor"></span>$${numberWithCommas(RevenuePerEmpStarValue)}`);
        if (RevenuePerEmpLocationPercent < 0) {
            RevenuePerEmpLocationPercent = 0;
        } else if (RevenuePerEmpLocationPercent > 100) {
            RevenuePerEmpLocationPercent = 100;
        }
        $("#divRevenuePerEmpLocationPercent").css("left", RevenuePerEmpLocationPercent + "%");
        $("#RevenuePerEmpTitle").html("Revenue per Employee\n(in USD)");
    }

    if (wcd && (demographicModel.CashConversionCycle != undefined)) {
        DrawDemographicBar(demographicModel.CashConversionCycle,
            "",
            " days",
            "Demographic1",
            "Cash Conversion Cycle\n(in days)");
    }

    if (wcd)
        $("#outliers").html(demographicModel.CccOutliers);
}

function DrawDemographicBar(chartModel, prefix, units, xxx, title) {
    const hxx = `#${xxx}`;
    if (chartModel != undefined) {
        
        /*
        console.log(`hxx:                     ${hxx}`);
        console.log(`prefix:                  ${prefix}`);
        console.log(`units:                   ${units}`);
        console.log(`xxx:                     ${xxx}`);
        console.log(`chartModel.Minimum:      ${chartModel.Minimum}`);
        console.log(`chartModel.Percentile25: ${chartModel.Percentile25}`);
        console.log(`chartModel.Percentile50: ${chartModel.Percentile50}`);
        console.log(`chartModel.Percentile75: ${chartModel.Percentile75}`);
        console.log(`chartModel.Maximum:      ${chartModel.Maximum}`);
        console.log(`chartModel.StarValue:    ${chartModel.StarValue}`);
        */

        $(hxx + " #Min").html(prefix + numberWithCommas(Math.round(chartModel.Minimum)) + units);
        $(hxx + " #P25").html(prefix + numberWithCommas(Math.round(chartModel.Percentile25)) + units);
        $(hxx + " #P50").html(prefix + numberWithCommas(Math.round(chartModel.Percentile50)) + units);
        $(hxx + " #P75").html(prefix + numberWithCommas(Math.round(chartModel.Percentile75)) + units);
        $(hxx + " #Max").html(prefix + numberWithCommas(Math.round(chartModel.Maximum)) + units);

        const starValue = chartModel.StarValue;
        let locationPercent = DemographicStarLocation(chartModel.Minimum,
            chartModel.Percentile25,
            chartModel.Percentile50,
            chartModel.Percentile75,
            chartModel.Maximum,
            chartModel.StarValue);

        const lpElement = $(hxx + " #LocationPercent");
        
        /*
        console.log(`hxx #LocationParent:     ${hxx + " #LocationPercent"}`);
        console.log(`locationPercent:         ${locationPercent}`);
        */

        if (locationPercent >= 75) {
            /*
            console.log('locationPercent >= 75');
            */
            lpElement.html(
                '<span>' +
                '<span>' + prefix + numberWithCommas(Math.round(starValue)) + units + '</span>' +
                '<span style="font-size: 200%; vertical-align: middle; left: 13px; position: relative; top: -2px;">&bigstar;</span>'+
                '</span>');
            lpElement.css("right", (100 - locationPercent) + "%");
        } else {
            /*
            console.log('locationPercent < 75');
            */
            lpElement.html(
                '<span>' +
                '<span style="font-size: 200%; vertical-align: middle; left: -13px; position: relative; top: -2px;">&bigstar;</span>' +
                '<span>' + prefix + numberWithCommas(Math.round(starValue)) + units + '</span>' +
                '</span>');
            lpElement.css("left", locationPercent + "%");
        }
        
        $(hxx + " #Title").text(title);

        $(hxx).show();
    } else {
        $(hxx).hide();
    }
}

function HideDemographicBar(xxx) {
    $(`#${xxx}`).hide();
}

//Custom logic what would the position of pointing star in chart.
function DemographicStarLocation(min, p25, p50, p75, max, starVal) {
    min = parseFloat(min);
    p25 = parseFloat(p25);
    p50 = parseFloat(p50);
    p75 = parseFloat(p75);
    max = parseFloat(max);
    starVal = parseFloat(starVal);

    var fixPercentage;
    var upperValue;
    var lowerValue;

    if (starVal < min) {
        return 0;
    } else if (starVal <= p25) {
        fixPercentage = 0;
        upperValue = p25;
        lowerValue = min;
    } else if (starVal <= p50) {
        fixPercentage = 25;
        upperValue = p50;
        lowerValue = p25;
    } else if (starVal <= p75) {
        fixPercentage = 50;
        upperValue = p75;
        lowerValue = p50;
    } else if (starVal <= max) {
        fixPercentage = 75;
        upperValue = max;
        lowerValue = p75;
    } else {
        return 100;
    }

    const rangeDifference = upperValue - lowerValue;
    if (rangeDifference < 0) {
        return 0;
    } else {
        const sectionDifference = 25.0;
        return fixPercentage + (starVal - lowerValue) * (sectionDifference / rangeDifference);
    }
}