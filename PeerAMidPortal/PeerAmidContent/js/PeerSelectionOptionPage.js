//Redirection on CompanyAutoSelection.cshtml page or CompanySelfSelection.cshtml
function navigateURL() {

    //updateFirstButton();
    const subOption = $("input[name='suboption']:checked").val();
    if (subOption == "a1") {
        analysisState.bcsp.peerSelectionMode = 1;
        setAnalysisState();
        window.location.href = "/PeerAMid/CompanyAutoSelection";
    } else if (subOption == "a2") {
        analysisState.bcsp.peerSelectionMode = 2;
        setAnalysisState();
        window.location.href = "/PeerAMid/CompanyAutoSelection";
    } else if (subOption == "a3") {
        analysisState.bcsp.peerSelectionMode = 3;
        setAnalysisState();
        window.location.href = "/PeerAMid/BenchmarkCompanyLatestData";
    }
    return false;
}


$(document).ready(function () {

    getAnalysisState();
    displayBenchmarkCompanyDetails();

    if (analysisState.service == 3) {
        $('#a1').hide();
    } else {
        $('#a1').show();
    }

    if (analysisState.bcsp.peerSelectionMode == 1) {
        $('#a1').prop('checked', true);
    } else if (analysisState.bcsp.peerSelectionMode == 2) {
        $('#a2').prop('checked', true);
    } else if (analysisState.bcsp.peerSelectionMode == 3) {
        $('#a3').prop('checked', true);
    }

    //setPageHeaders('chevronPeerCompanySelection', null);
});


function displayBenchmarkCompanyDetails() {

    var bmc = analysisState.benchmarkCompany;
    $('#benchmarkCompanyName').html(bmc.Name);
    $('#benchmarkCompanyFinancialYear').html(bmc.FinancialYear);
    $('#benchmarkCompanyCurrency').html(bmc.DataEntryCurrency);

    var rev = bmc.Revenue;
    rev = Math.floor(rev);
    rev = rev.toLocaleString();
    $('#benchmarkCompanyRevenue').html(rev);

    var ebi = fractionToPercentage(bmc.EbitdaMargin, 2); // * 10000;
    //ebi = Math.round(ebi) / 100;
    //ebi = `${ebi}%`;
    $('#benchmarkCompanyEbitda').html(ebi);

    var exp = bmc.Expense;
    exp = Math.floor(exp);
    exp = exp.toLocaleString();
    $('#benchmarkCompanySga').html(exp);

    var gm = fractionToPercentage(bmc.GrossMargin, 2);
    //gm = Math.round(gm) / 100;
    //gm = `${gm}%`;
    $('#benchmarkCompanyGrossMargin').html(gm);

    var emp = bmc.TotalEmployees;
    $('#benchmarkCompanyEmployees').html(emp);
}


function navigateBack() {

    if (analysisState.isNewCompany) {
        window.location.href = "/PeerAMid/EditNewCompany";
    } else {
        window.location.href = "/PeerAMid/Search";
    }
}