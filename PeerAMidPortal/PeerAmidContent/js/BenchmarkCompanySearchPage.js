
var benchmarkCandidates = [];
var currentlySelectedData = null;
//var currentlySelectedBenchmarkCandidate = null;

$(document).ready(function() {

    // restore the previous state of th page
    if (!sessionData)
        getSessionData();

    if (sessionData.ResetAnalysisOnBenchmarkCompanySelectionPage) {
        // var service = analysisState.service;
        resetAnalysisState();
        // analysisState.service = service;
    }

    setPageHeaders('chevronCompanySelection','Please identify the company you would like to benchmark');

    if (analysisState.service == 1) {
        $('#allowAddNewCompany').show();
    } else {
         $('#teaseAddNewCompany').show();
    }

    var benchmarkCompany = analysisState.benchmarkCompany;
    benchmarkCandidates = analysisState.bcsp.searchResults;
    currentlySelectedBenchmarkCandidate = benchmarkCompany;

    for (var i = 0; i < benchmarkCandidates.length; ++i) {
        if (benchmarkCandidates[i].CompanyId == benchmarkCompany.CompanyId) {
            benchmarkCandidates[i].IsSelected = true;
            break;
        }
    }

    setUpBenchmarkCandidateGrid();
    //setupRegionsSelector(document.getElementById("selectedRegions"));

    $('input[type="radio"]').change(function () {
        $('#NextButton').css("color", "white");
    });

    displayBenchmarkCompany(benchmarkCompany);

    $('#exampleInputCompany').val(analysisState.bcsp.companySearch);
    $('#exampleInputTicker').val(analysisState.bcsp.tickerSearch);
    setSelectedRegions(analysisState.selectedRegions);
    $('#hdnCompanyId').val(benchmarkCompany.CompanyId);                     // TODO -- dispense with
    $('#hdnCompanyYear').val(benchmarkCompany.FinancialYear.toString());    // TODO -- dispense with

    if (benchmarkCandidates.length > 0) {
        $("#table1").data('kendoGrid').dataSource.data(benchmarkCandidates);
        $('#companySearch').show();
        $('#companySelectionOption').show();
        $('.desc').show();
    }

    var psm = analysisState.bcsp.peerSelectionMode;
    if (psm == 0)
        $("#NextButton").css("color", "darkgray");
    else
        $("#PeerSelectionMode" + psm).prop('checked', true);
});

function setUpBenchmarkCandidateGrid() {

    $("#table1").kendoGrid({
        schema: kendoCompanyGrid.getSchema(),
        columns: kendoCompanyGrid.getColumns(),

        //selectable: false,
        pageable: false,
        resizable: true,
        sortable: true,
        editable: "inline",
        //noRecords: "You have not yet selected any companies.",

        dataSource: {
            data: benchmarkCandidates,
            autosync: true,
            sort: [{ field: "Revenue", dir: "desc" } /*, { field: "CompanyName", dir: "asc" }*/],
        }, // dataSource

        dataBound: function(e) {
            kendoCompanyGrid.defaultDataBoundHandler(e);
        }, // databound
    });

    setGridColumnWidths("table1", "IsSelected", 23, "CompanyName", 85, "SubIndustryId", 115, "Ticker", 52, "Country", 48, "Revenue", 47, "FinancialYear", 20);

    kendoCompanyGrid.attachCheckboxClickHandler($("#table1"), "input",
        (p) => {
            if (currentlySelectedData && (currentlySelectedData.dataItem.CompanyId === p.dataItem.CompanyId)) {
                if (p.dataItem.IsSelected) {
                    p.dataItem.IsSelected = false;
                    p.checked = false;
                    p.input.checked = false;
                    $('#companySelectionOption').hide();
                } else {
                    p.dataItem.IsSelected = true;
                    benchmarkCompanySelected(p.dataItem);
                    p.checked = true;
                    p.input.checked = true;
                }
            } else {
                if (p.dataItem.IsSelected) {
                    currentlySelectedData = null;
                    p.dataItem.IsSelected = false;
                    p.checked = false;
                    $('#companySelectionOption').hide();
                } else {
                    if (currentlySelectedData != null) {
                        currentlySelectedData.dataItem.IsSelected = false;
                        currentlySelectedData.input.checked = false;
                    }
                    p.dataItem.IsSelected = true;
                    currentlySelectedData = { dataItem: p.dataItem, input: p.input };
                    benchmarkCompanySelected(p.dataItem);
                    p.checked = true;
                }
            }
        });
}

function scrollDown() {
    $('html, body').animate({ scrollTop: '250px' }, 1600);
}

// Radio button Display code.
function showNestedRadioButtons() {

    $(".desc").css("display", "block");
}

// Radio button hide code.
function hideNestedRadioButtons() {

    $(".desc").css("display", "none");
}

//Request redirect to CompanyAutoSelection.cshtml
function navigateURL(event1) {

    analysisState.selectedRegions = getSelectedRegionsWithoutCallingBackEnd();
    analysisState.bcsp.companySearch = $('#exampleInputCompany').val() || '';
    analysisState.bcsp.tickerSearch = $('#exampleInputTicker').val() || '';
    const option = $("input[name='option']:checked").val();
    analysisState.bcsp.peerSelectionMode = (option == "4") ? 1 : ((option == "5") ? 2 : 3);

    setAnalysisState();
    notifyBackEndOfBenchmarkCompany(analysisState.benchmarkCompany);

    let href = null;

    switch (analysisState.bcsp.peerSelectionMode) {
      
        case 0: // not selected
            break; 

        case 1: // 1 = Allow PeerAMid to identify possible peer companies
        case 2: // 2 = I will select my peer companies
            href = "/PeerAMid/CompanyAutoSelection";
            break;        
            
        /*
        case 2:
            UNUSED href = "/PeerAMid/BenchmarkCompanyData#PartialDataByUser";
            break;
        */

        case 3: // 3 = I will provide the latest client data
            analysisState.isModifyingBenchmarkCompanyData = false;
            notifyBackEndOfBenchmarkCompany(analysisState.benchmarkCompany);
            href = "/PeerAMid/BenchmarkCompanyLatestData";
            //href = "/PeerAMid/BenchmarkCompanyLatestData#CompleteDataByUser";
            break;
    }

    if (href != null) {
        setSessionVariables(20,
            null,
            null,
            null,
            (r) => { // clear selected and search lists, industry and subIndustry
                // console.log(`Navigating to ${href}`);
                window.location.href = href;
            });
    }
}


//Text box keydown event.
$(function() {
    $('input[type="text"]').keydown(function(e) {
        if (e.keyCode == 13) {
            navigateToSearchResult();
            event.preventDefault();
        }
    });
});

//After click on search button search company data and bind in Company grid list.Procedure name Proc_GetAllcompanyList
function navigateToSearchResult() {

    analysisState.bcsp.companySearch = $('#exampleInputCompany').val() || '';
    analysisState.bcsp.tickerSearch = $('#exampleInputTicker').val() || '';
    analysisState.selectedRegions = getSelectedRegions();

    if (analysisState.bcsp.companySearch != '' || analysisState.bcsp.tickerSearch != '') {
        //Show #companySearch
        $('#companySearch').show();
        //Hide #companySelectionOption
        $('#companySelectionOption').hide();
        console.log("Calling ajax: " + '/PeerAMid/BenchmarkCompanySearchResults');
        $.ajax({
            type: "POST",
            url: '/PeerAMid/BenchmarkCompanySearchResults',
            data: { cSearch: analysisState.bcsp.companySearch, tSearch: analysisState.bcsp.tickerSearch, regions: analysisState.selectedRegions },
            dataType: "json",
            success: function(result) {
                benchmarkCandidates = result.aaData;
                analysisState.bcsp.searchResults = benchmarkCandidates;
                $("#table1").data('kendoGrid').dataSource.data(benchmarkCandidates);
            },
            error: function(_, _, err) {
                console.log(err);
                benchmarkCandidates = [];
                $("#table1").data('kendoGrid').dataSource.data(benchmarkCandidates);
            }
        });
    } else {
        bAlert("Please enter company name or ticker to search");
    }

}


function benchmarkCompanySelected(benchmarkCompany) {
    //console.log(JSON.stringify(benchmarkCompany));
    $('#hdnCompanyId').val(benchmarkCompany.Id);
    $('#hdnCompanyYear').val(benchmarkCompany.FinancialYear.toString());
    $('#companySelectionOption').show();
    $('#companySelectionOption').find("input:radio").prop("checked", false);
    $('#NextButton').css("color", "darkgray");
    $('.desc').hide();
    //console.log("benchmarkCompanySelected");

    $.ajax({
        type: "GET",
        url: '/PeerAMid/GetCompanyDetails',
        data: { companyId: benchmarkCompany.Id, year: benchmarkCompany.DataYear || '0' },
        dataType: "json",
        success: function(bc) {
            if (bc === undefined) {
                analysisState.benchmarkCompany = {};
                console.log("Not the benchmark company!");
            } else {
                setBenchmarkCompany(bc, false);
                displayBenchmarkCompany(analysisState.benchmarkCompany);
            }
        },
        error: function() 
        {
            analysisState.benchmarkCompany = {};
            console.log("Error");
        }
    });
}

function displayBenchmarkCompany(benchmarkCompany) {
    if (benchmarkCompany.hasOwnProperty("CompanyName") && benchmarkCompany.CompanyName) {
        $('#txtCompanyName').text(benchmarkCompany.CompanyName);
        $('#txtFiscalYear').text(benchmarkCompany.FinancialYear);
        $('#txtCurrency1').text(benchmarkCompany.ReportingCurrency);
        $('#txtCurrency2').text('USD');
            // console.log(result.ExchangeRate);
        $('#txtExRate').text(benchmarkCompany.DataEntryExchangeRate.toFixed(4));
        $('#txtRevenue').text(numberWithCommas(Math.round(benchmarkCompany.Revenue)));
        $('#txtEBITDA').text((benchmarkCompany.EbitdaMargin * 100).toFixed(2) + '%');
        $('#txtExpense').text(numberWithCommas(Math.round(benchmarkCompany.Expense)));
        $('#txtGrossMargin').text((benchmarkCompany.GrossMargin * 100).toFixed(2) + '%');
        $('#txtTotalEmployees').text(numberWithCommas(benchmarkCompany.TotalEmployees));
    }
}

function chevronNavigation(id) {
    return 0;
}