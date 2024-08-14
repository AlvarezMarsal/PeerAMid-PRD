//let clientName = analysisState.benchmarkCompany.DisplayName;
//let selectedTargetCompanySymbol = analysisState.benchmarkCompany.Ticker;

$(document).ready(function () {

    getAnalysisState();

    showChevrons('lastTab_1020');
    showServiceDescription();
    displayBenchmarkCompanyDetails();
    $('#benchmarkCompanyNameMixedCase').html(analysisState.benchmarkCompany.DisplayName);

    var d = new Date();
    var dday = weekdays[d.getDay()];
    var dmonth = months[d.getMonth()];
    var dyear = d.getFullYear();
    var ddate = d.getDate();
    var date = dday + ", " + dmonth + " " + ddate + " " + dyear;
    $('.footerLeftContent').html(analysisState.benchmarkCompany.DisplayName + " | " + date);

    CreateCharts();
});


function CreateCharts() {

    var service = analysisState.service;
    var wcd = (service == 2) || (service == 4);

    $(".loader-div").show();
    const gsd = getGlobalStaticData();
    const sd = getSessionData();
    notifyBackEndOfBenchmarkCompany(analysisState.benchmarkCompany);

    var doAsync = false;

    plotDemographicChart(wcd, gsd, doAsync);
    plotSGAChart(wcd, doAsync);
    plotSGAPerformanceChart(wcd, doAsync);
    plotFunCostAsPercentOfRevenue(wcd, gsd, sd, doAsync);
    plotFTECostAsPercentOfRevenue(wcd, gsd, sd, doAsync);
    plotWaterFallChart(wcd, doAsync);

    $(".loader-div").hide();
}


function modifyPeers(autoOrSelf) {
    chevronNavigation('chevronPeerCompanySelection');
}



//After click on Export the Deliverable button download PPT on browser.
function downloadPPT() {

    var service = analysisState.service;
    var isSgaHighLevelCost = service == 3;

    const postData = {
        selectedPeers: analysisState.peerCompanies,
        selectedTarget: analysisState.benchmarkCompany.CompanyId,
        year: analysisState.benchmarkCompany.FinancialYear,
        service: service
    };

    downloadPPTLoader(true);
    showAdWindow(isSgaHighLevelCost);

    console.log("Calling ajax: " + '/PeerAMid/SavePPTReport?selectedTargetSymbol=');
    $.ajax({
        type: "POST",
        url: `/PeerAMid/SavePPTReport?selectedTargetSymbol=${analysisState.benchmarkCompany.Ticker}`,
        async: true,
        data: JSON.stringify(postData),
        contentType: "application/json",
        success: function (reportData) {
            downloadPPTLoader(false);
            window.location.href = `/PeerAMid/DownloadPPT?filename=${reportData.FileName}`;

        },
        error: function (err) { }
    });
}

function displayBenchmarkCompanyDetails() {
    var bmc = analysisState.benchmarkCompany;
    $('#benchmarkCompanyName').html(bmc.Name);
    $('#headerIndustryName').html(getIndustry(bmc.IndustryId).IndustryName);
    $('#benchmarkCompanySubIndustry').html(getSubIndustry(bmc.SubIndustryId).SubIndustryName);
    var n = analysisState.peerCompanies.split(',');
    var i = n.indexOf(bmc.CompanyId);
    var c = n.length;
    if (i > -1) c--;
    $('#headerPeersCount').html(c.toString());
}

function chevronNavigation(id) {
    if (id == 'chevronCompanySelection')
        window.location.href = '/PeerAMid/Search';
    else if (id == 'chevronPeerCompanySelection')
        window.location.href = '/PeerAMid/CompanyAutoSelection';
    return 0;
}

function modifyBenchmarkCompanyData() {
    analysisState.isModifyingBenchmarkCompanyData = true;
    setAnalysisState();
    window.location.href = "/PeerAMid/ModifyBenchmarkCompanyData";
}