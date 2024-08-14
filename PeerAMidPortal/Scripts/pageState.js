const localStorageKey = 'PeerAMid-anaylsis-state';
const analysisStateFormat = 1;
var analysisState = null;

function setAnalysisState() {
    var value = JSON.stringify(analysisState);
    // console.log("Saving analysis state " + value);
    localStorage.setItem(localStorageKey, value);
}

function getAnalysisState() {
    var value = localStorage.getItem(localStorageKey);
    if (value) {
        // console.log("Retrieved analysis state " + value);
        analysisState = JSON.parse(value);
        if (analysisState.format == analysisStateFormat)
            return;
        // console.log("Discarding analysis state");
    } else {
        // console.log("No analysis state");
    }
    resetAnalysisState();
}

function resetAnalysisState(service) {

    console.log("Resetting analysis state");
    var a = {};

    a.format = analysisStateFormat;
    if (typeof(service) === 'number')
        a.service = service; // 0=None,  1=SgaFull, 2=WcdFull, 3=SgaShort, 4=WcdShort, 5=RetailFull, 6=RetailShorta.service
    else
        a.service = analysisState.service;
    a.selectedRegions = "1,2,3,4,5";
    a.isNewCompany = false;
    a.isModifyingBenchmarkCompanyData = false;
    a.benchmarkCompany = {};
    a.benchmarkCompany.CompanyId = 0;       // to keep from having lots of if..else when populating the page
    a.benchmarkCompany.FinancialYear = 0;   // to keep from having lots of if..else when populating the page
    a.benchmarkCompany.Revenue = 0;         // to keep from having lots of if..else when populating the page
    a.benchmarkCompany.IndustryId = 0;      // to keep from having lots of if..else when populating the page
    a.benchmarkCompany.SubIndustryId = 0;   // to keep from having lots of if..else when populating the page
    a.peerCompanies = "";                   // the final peer list, as comma-delimited list of ids
    a.bcsp = {}; // Benchmark Company Selection Page (Search)
    a.bcsp.companySearch = "";   
    a.bcsp.tickerSearch = "";
    a.bcsp.peerSelectionMode = 0; // 0 = none selected; 1 = Allow PeerAMid to identify possible peer companies; 2 = I will select my peer companies; 3 = I will provide the latest client data
    a.bcsp.searchResults = [];
    a.psp = {};  // Peer Selection Page (CompanyAutoSelection)
    a.psp.minRevenue = -1; // defaults to benchmarkCompany.Revenue / 2
    a.psp.maxRevenue = -1; // defaults to 3 * benchmarkCompany.Revenue / 2
    a.psp.companySearch = "";   
    a.psp.tickerSearch = "";
    a.psp.selectedIndustries = ""; // comma-delimited list
    a.psp.selectedSubIndustries = "";
    a.psp.searchResults = [];   // left-hand grid
    a.psp.selectedPeers = [];   // right-hand grid
    a.psp.initialLoad = true;   // true - the searchResults are all 'suggested'; false -- the searchResults are not suggested
    a.psp.layout = "";

    analysisState = a;
    value = JSON.stringify(analysisState);
    localStorage.setItem(localStorageKey, value);
}

// It's better to call this than to set analysisState.benchmarkCompany
// directly.
function setBenchmarkCompany(bc, isNewCompany) {

    console.log("setBenchmarkCompany");
    console.log(JSON.stringify(bc));

    //if (bc.CompanyId != analysisState.benchmarkCompany.CompanyId) {
        var a = analysisState;
        a.benchmarkCompany = bc;
        a.isNewCompany = isNewCompany;
        a.psp.minRevenue = Math.floor(bc.Revenue / 2);
        a.psp.maxRevenue = Math.ceil(3 * bc.Revenue / 2);
        a.psp.companySearch = "";   
        a.psp.tickerSearch = "";
        a.psp.selectedIndustries = `${bc.IndustryId}`;
        a.psp.selectedSubIndustries = `${bc.SubIndustryId}`;
        a.psp.searchResults = [];   // left-hand grid
        a.psp.selectedPeers = [];   // right-hand grid
        a.psp.initialLoad = true;   // true - the searchResults are all 'suggested'; false -- the searchResults are not suggested
        a.psp.layout = "";
        setAnalysisState();
        return true;
    //}
    //return false;
}

// It's better to call this than to set analysisState.benchmarkCompany
// directly.
function setBenchmarkCompanyAndNotifyBackEnd(bc, isNewCompany) {

    if (setBenchmarkCompany(bc, isNewCompany)) {
        notifyBackEndOfBenchmarkCompany(bc);
    }
}

// It's better to call this than to set analysisState.benchmarkCompany
// directly.
function notifyBackEndOfBenchmarkCompany(bc) {
    var data = {
        companyId: bc.CompanyId,
        year: bc.FinancialYear
    };
    ajaxSync("/PeerAMid/SetBenchmarkCompany", data, "Post");
}




getAnalysisState();


