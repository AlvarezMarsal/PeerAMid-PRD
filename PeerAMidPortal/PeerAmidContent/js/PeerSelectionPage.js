var selectedPeersGrid;
var table2rows = [];
var table2subIndustries = {};
var allSubIndustries = null;
var minSelectedPeers = 6;
var maxSelectedPeers = 15;
const suggestedPeerColor = "#77CA00";
const lightSuggestedPeerColor = "#E7F7CD";
const additionalPeerColor = "#E0E0E0";

$(document).ready(function () {

    getGlobalStaticData();
    analysisState.psp.initialLoad = (analysisState.bcsp.peerSelectionMode == 1);

    $('#suggestedSquare').hide();
    $('#additionalSquare').hide();

    displayBenchmarkCompanyDetails();
    setPageHeaders('chevronPeerCompanySelection', null);
    setLayout("search");
    setUpPeerSearchResultsGrid();
    setUpSelectedPeersGrid();
    loadExtendedIndustryList();

    if (analysisState.psp.minRevenue > 0)
        $('#txtMinRevenue').val(analysisState.psp.minRevenue.toLocaleString());
    if (analysisState.psp.maxRevenue > 0)
        $('#txtMaxRevenue').val(analysisState.psp.maxRevenue.toLocaleString());

    // Load Company list data in dropdown.
    const options1 = {
        url: function (phrase) { return "/api/HomeAPI/GetCompanyNames"; },
        getValue: function (element) { return element.CompanyName; },
        ajaxSettings: { method: "GET", data: {}, dataType: "json", headers: config },
        preparePostData: function (data) {
            data.phrase = $("#CompanySearch").val();
            data.byTicker = false;
            data.max = 10;
            // console.log(`Calling back end:${data.phrase}`);
            return data;
        },
        listLocation: "Companies",
        list: {
                onClickEvent: function() {
                    searchPeerCompany('name');
                }

        },
        matchResponseProperty: "Tag"
    };
    $("#CompanySearch").easyAutocomplete(options1);

    const options2 = {
        url: function (phrase) { return "/api/HomeAPI/GetCompanyNames"; },
        getValue: function (element) { return element.Ticker; },
        ajaxSettings: { method: "GET", data: {}, dataType: "json", headers: config },
        preparePostData: function (data) {
            data.phrase = $("#TickerSearch").val();
            data.byTicker = true;
            data.max = 10;
            // console.log(`Calling back end:${data.phrase}`);
            return data;
        },
        listLocation: "Companies",
        list: {
            onClickEvent: function() {
                searchPeerCompany('ticker');
            }

        },
        matchResponseProperty: "Tag"
    };
    $("#TickerSearch").easyAutocomplete(options2);

    $("#CompanySearch").val(analysisState.psp.companySearch);
    $("#TickerSearch").val(analysisState.psp.tickerSearchPrefix);

    var psp = analysisState.psp;
    psp.initialLoad = psp.initialLoad && (psp.selectedPeers.length == 0) && (psp.searchResults.length == 0);
    if (psp.initialLoad && (analysisState.bcsp.peerSelectionMode == 1))
        doInitialLoad('all', continuePageLoad);
    else
        continuePageLoad();
});


function continuePageLoad() {

    var psp = analysisState.psp;

    if (psp.searchResults.length > 0) {
        refreshPeerSearchResultsGrid();
        setLayout('searchSplit');
    }

    refreshSelectedPeersGrid();
    updateSelectedCompaniesCount();
}


function doInitialLoad(type, callback) {
        
    const params = {
        uid:                analysisState.benchmarkCompany.CompanyId,
        revenueFrom:        -1,
        revenueTo:          -1,
        year:               0,
        regions:            analysisState.selectedRegions,
        industryFilter:     null,
        subIndustryFilter:  null,
        nameFilter:         null,
        tickerFilter:       null,
        bindToSessionData:  false,
        maxCompanies:       maxSelectedPeers
    };

    doSearch(type, params, callback);
}


function doSearch(type, params, callback) {

    if (!callback)
        callback = function () {};

    const uri = '/PeerAMid/GetSuggestedPeerCompanies';

    console.log(`Calling ajax: '${uri}'`);
    console.log(JSON.stringify(params));
    pleaseWait(true);
    $.ajax({
        type: "POST",
        url: uri,
        data: params,
        dataType: "json",
        success: function (result) {
            // console.log('Got ' + result.aaData.length + ' results');
            var sr = result.aaData;
            var addToSelected = (analysisState.psp.initialLoad) || ((sr.length == 1) && (analysisState.psp.selectedPeers.length < maxSelectedPeers));
            if (addToSelected) {

                if (analysisState.psp.initialLoad) {
                    analysisState.psp.selectedPeers = sr;
                    for (var i = 0; i < sr.length; ++i) {
                        sr[i].IsSuggested = true;
                        if (i <= maxSelectedPeers)
                            sr[i].IsSelected = true;
                    }
                    analysisState.psp.initialLoad = false;
                } else {
                    var hash = makeHash(analysisState.psp.selectedPeers, (c) => `${c.CompanyId},${c.FinancialYear}`);
                    var e = hash.get(sr[0]);
                    if (e === null) {
                        if (analysisState.psp.selectedPeers.length < maxSelectedPeers) {
                            sr[0].IsSelected = true;
                            analysisState.psp.selectedPeers.push(sr[0]);
                        }
                    }
                }
                // refreshSelectedPeersGrid(); // the callback does this!
                //updateSelectedCompaniesCount();
            } else {

                //console.log('not initial load');
                var hash = makeHash(analysisState.psp.searchResults, (c) => `${c.CompanyId},${c.FinancialYear}`);
                for (var i = 0; i < sr.length; ++i) {
                    var e = hash.get(sr[i]);
                    if (e === null)
                        analysisState.psp.searchResults.push(sr[i]);
                }
                if ((analysisState.psp.searchResults.length > 0) && (analysisState.psp.layout != 'searchSplit')) {
                    setLayout('searchSplit');
                }
                //refreshPeerSearchResultsGrid();
            }
            setAnalysisState();
            pleaseWait(false);
            callback();
        },
        error: function (_, _, err) {
            console.log(`Error: ${err}`);
            pleaseWait(false);
            callback();
        }
    });
}


function makeHash(data, func) {

    var hash = {
        length: 0,
        entries: {},
        getKey: func,

        set: function(v) {
            var k = this.getKey(v);
            var h = `_${k}`;
            if (!this.entries.hasOwnProperty(h))
                length++; // because entries.length is not reliable
            this.entries[h] = v;
        },

        get: function(v) {
            var k = this.getKey(v);
            return this.getByKey(k);
        },

        getByKey: function(k) {
            var h = `_${k}`;
            return this.entries.hasOwnProperty(h) ? this.entries[h] : null;
        }
    };

    for (var i = 0; i < data.length; ++i) {
        hash.set(data[i]);
    }

    return hash;
}


function goBack() {
    window.location.href = '/PeerAMid/Search';
}


function setUpPeerSearchResultsGrid() {

    const columns = kendoCompanyGrid.getColumns();
    columns[0].template = '<input type="checkbox" #= IsSelected ? checked="checked" : "" # />';
    //console.log(`Setting up PeerSearchResultsGid with ${analysisState.psp.searchResults.length}`);
    $("#peerSearchResultsGrid").kendoGrid({

        schema: kendoCompanyGrid.getSchema(),
        columns: columns,

        //selectable: false,
        pageable: false,
        resizable: true,
        sortable: true,
        noRecords: { template: "No companies match your search criteria." },

        dataSource: {
            data: analysisState.psp.searchResults,
            sort: [{ field: "Revenue", dir: "desc" } /*, { field: "CompanyName", dir: "asc" }*/],
        }, // dataSource

        dataBound: function (e) {
            kendoCompanyGrid.defaultDataBoundHandler(e);
        }, // databound
    });

    kendoCompanyGrid.attachCheckboxClickHandler($("#peerSearchResultsGrid"), "td",
        (p) => {
            //var sd = getSessionData();
            //var id = p.dataItem.CompanyId;
            // var index = -1;
            //var highest = -1;
            var searchResults = analysisState.psp.searchResults;
            //var searchResults = p.grid.dataSource.data();
            p.dataItem.IsSelected = p.checked; // updates grid's stored copy of the data
            for (let i = 0; i < searchResults.length; ++i) {
                if (searchResults[i].CompanyId === p.dataItem.CompanyId) {
                    searchResults[i].IsSelected = p.checked;
                    break;
                }
            }
        });
}


function refreshSelectedPeersGrid() {

    console.log('refreshSelectedPeersGrid');

    var selectedPeers = analysisState.psp.selectedPeers;
    var searchResults = analysisState.psp.searchResults;

    if (selectedPeers.length > 0) {
        let selected = '|';
        let i = 0;
        for (/**/; i < selectedPeers.length; ++i) {
            selected += selectedPeers[i].CompanyId;
            selected += '|';
        }
        //console.log(selected);
        i = 0;
        while (i < searchResults.length) {
            const key = `|${searchResults[i].CompanyId}|`;
            if (selected.indexOf(key) >= 0) {
                searchResults.splice(i, 1);
            } else {
                ++i;
            }
        }
    }

    $('#selectedPeersGrid').data('kendoGrid').dataSource.data(selectedPeers);
    $('#selectedPeersGrid').data('kendoGrid').refresh();

    updateFinalPeerCompanies();
}


function refreshPeerSearchResultsGrid() {

    console.log('refreshPeerSearchResultsGrid');

    var selectedPeers = analysisState.psp.selectedPeers;
    var searchResults = analysisState.psp.searchResults;

    if (selectedPeers.length > 0) {
        let selected = '|';
        let i = 0;
        for (/**/; i < selectedPeers.length; ++i) {
            selected += selectedPeers[i].CompanyId;
            selected += '|';
        }
        //console.log(selected);
        i = 0;
        while (i < searchResults.length) {
            const key = `|${searchResults[i].CompanyId}|`;
            if (selected.indexOf(key) >= 0) {
                searchResults.splice(i, 1);
            } else {
                ++i;
            }
        }
    }

    $('#peerSearchResultsGrid').data('kendoGrid').dataSource.data(searchResults);
    $('#peerSearchResultsGrid').data('kendoGrid').refresh();

}


function setUpSelectedPeersGrid() {

    //const sd = getSessionData();
    //console.log("setUpSelectedPeersGrid");
    //console.log(JSON.stringify(sd.SelectedPeerCompanies));

    const columns = kendoCompanyGrid.getColumns();
    columns[0].template = '<input type="checkbox" #= IsSelected ? checked="checked" : "" # />';

    $("#selectedPeersGrid").kendoGrid({
        schema: kendoCompanyGrid.getSchema(),
        columns: columns,

        //selectable: false,
        pageable: false,
        resizable: true,
        sortable: true,
        noRecords: { template: "You have not yet selected any companies." },

        dataSource: {
            data: analysisState.psp.selectedPeers,
            autosync: true,
            sort: [{ field: "Revenue", dir: "desc" } /*, { field: "CompanyName", dir: "asc" }*/],
        }, // dataSource

        dataBound: function (e) {
            // make headerAttributes work
            kendoCompanyGrid.defaultDataBoundHandler(e);

            // Iterate the data items and apply row styles where necessary
            const rows = e.sender.tbody.children();

            for (let j = 0; j < rows.length; j++) {
                // Get the current row.
                const row = $(rows[j]);
                // Get the dataItem of the current row.
                const dataItem = e.sender.dataItem(row);
                // console.log(JSON.stringify(dataItem));

                // Get the value of the UnitsInStock cell from the current row.
                row.addClass(dataItem.IsSuggested ? "suggestedCompanyRow" : "userSelectedRow");
                //var cell = row.children().eq(columnIndex);
                //cell.addClass(getUnitsInStockClass(units));
            }
        }, // databound
    });

    kendoCompanyGrid.attachCheckboxClickHandler($("#selectedPeersGrid"), "td",
        (p) => {
            //var sd = getSessionData();
            var id = p.dataItem.CompanyId;
            //var selectedPeers = p.grid.dataSource.data();
            var selectedPeers = analysisState.psp.selectedPeers;
            // var index = -1;
            //var highest = -1;

            if (p.checked) {
                let count = 0;
                for (var i = 0; i < selectedPeers.length; ++i) {
                    if (selectedPeers[i].IsSelected) {
                        if (++count > maxSelectedPeers) {
                            p.checked = false;
                            setTimeout(showTooManySelectedAlert);
                            return;
                        }
                    }
                }
            }

            p.dataItem.IsSelected = p.checked; // updates grid's stored copy of the data
            for (var i = 0; i < selectedPeers.length; ++i) {
                if (selectedPeers[i].CompanyId === p.dataItem.CompanyId) {
                    selectedPeers[i].IsSelected = p.checked;
                    break;
                }
            }

            setTimeout(updateSelectedCompaniesCount);
        });

}


function showTooManySelectedAlert() {
    bAlert("You already have the maximum number of peers selected.  De-select some to select more.");
}



function setLayout(layout) {

    if (isNullOrUndefined(layout))
        layout = analysisState.psp.layout;
    //console.log(layout);

    var all = byId('List');
    var allWidth = all.offsetWidth;
    var spl = byId('selectedPeersList');
    //var tso = byId('toggleSearchOpen');
    //var tsc = byId('toggleSearchClose');
    var scr = byId('searchCriteria');
    var lst = byId('searchList');
    var mtf = byId('moveToFinal');

    if (layout === "toggle-search") {
        if ((analysisState.psp.layout === "search") || (analysisState.psp.layout === "searchSplit")) {
            layout = "start";
        } else {
            layout = "search";
        }
    }

    // console.log("Setting layout to " + layout);

    if ((layout === "start") || (layout === "search")) {

        spl.style.width = Math.min(1200, allWidth) + "px";

        if (layout === "start") {
            //tso.style.display = "inline-flex";
            //tsc.style.display = "none";
            scr.style.display = "none";
        } else /*if (layout === "search") */ {
            //tso.style.display = "none";
            //tsc.style.display = "inline-flex";
            scr.style.display = "flex";
        }

        lst.style.display = "none";
        mtf.style.display = "none";

        setGridColumnWidths("selectedPeersGrid", "IsSelected", 23, "CompanyName", 75, "SubIndustryId", 125, "Ticker", 30, "Country", 35, "Revenue", 42, "FinancialYear", 20);
        setGridColumnWidths("peerSearchResultsGrid", "IsSelected", 23, "CompanyName", 75, "SubIndustryId", 125, "Ticker", 30, "Country", 35, "Revenue", 42, "FinancialYear", 20);

    } else if (layout === "searchSplit") {

        var middle = 23;
        var forTables = allWidth - middle;
        var half1 = Math.floor(forTables / 2);
        var half2 = forTables - half1;

        //console.log("all = " + allWidth + ", middle = " + middle + ", half = " + half1 + "," + half2);
        spl.style.display = "inline-flex";
        spl.style.flexBasis = Math.max(600, Math.min(1200, half1)) + "px";
        spl.style.width = "100%";
        //tso.style.display = "none";
        //tsc.style.display = "inline-flex";
        scr.style.display = "flex";
        lst.style.display = "inline-flex";
        lst.style.flexBasis = Math.max(600, Math.min(1200, half2)) + "px";
        lst.style.width = "100%";
        mtf.style.display = "inline-flex";
        mtf.style.flexBasis = middle + "px";
        mtf.style.width = "100%";
        mtf.style.flexGrow = 0;

        setGridColumnWidths("selectedPeersGrid", "IsSelected", 23, "CompanyName", 75, "SubIndustryId", 125, "Ticker", 30, "Country", 35, "Revenue", 42, "FinancialYear", 20);
        setGridColumnWidths("peerSearchResultsGrid", "IsSelected", 23, "CompanyName", 75, "SubIndustryId", 125, "Ticker", 30, "Country", 35, "Revenue", 42, "FinancialYear", 20);
    }

    analysisState.psp.layout = layout;
}


function updateSelectedCompaniesCount() {

    let auto = 0;
    let self = 0;
    let selectedPeers = analysisState.psp.selectedPeers;

    for (let i = 0; i < selectedPeers.length; ++i) {
        if (selectedPeers[i].IsSelected) {
            if (selectedPeers[i].IsSuggested)
                auto = auto + 1;
            else
                self = self + 1;
        }
    }

    const div = document.getElementById('totalCompanies');
    if ((self === 0) && (auto > 0))
        div.innerText = `Suggested Companies (${auto} / ${maxSelectedPeers})`;
    else
        div.innerText = `Selected Companies (${auto + self} / ${maxSelectedPeers})`;

    if (auto > 0)
        $('#suggestedSquare').show();
    else
        $('#suggestedSquare').hide();

    if (self > 0)
        $('#additionalSquare').show();
    else
        $('#additionalSquare').hide();
}


window.addEventListener("resize",
    () => {
        setLayout();
    });


function loadExtendedIndustryList() {

    var gsd = getGlobalStaticData();
    var selectedIndustries = commaDelimitedStringToHash(analysisState.psp.selectedIndustries);
    var selectedSubIndustries = commaDelimitedStringToHash(analysisState.psp.selectedSubIndustries);

    const industrySelectElement = document.getElementById("CompanyIndustrySearch");
    if (industrySelectElement.childElementCount < gsd.Industries.length) {

        industrySelectElement.clearChildren();
        addOption(industrySelectElement, "0", "All industries", true);
        const subIndustrySelectElement = document.getElementById("CompanySubIndustrySearch");
        subIndustrySelectElement.clearChildren();

        //console.log("Success in ajax: " + uri);
        allSubIndustries = [];
        for (let i = 0; i < gsd.Industries.length; ++i) {
            const x = gsd.Industries[i];
            var isDefaultIndustry = (x.IndustryId === analysisState.benchmarkCompany.IndustryId);
            var isSelectedIndustry = selectedIndustries[x.IndustryId];
            addOption(industrySelectElement, x.IndustryId, x.IndustryName, isDefaultIndustry, isSelectedIndustry);
            for (let j = 0; j < x.SubIndustries.length; ++j) {
                const s = x.SubIndustries[j];
                allSubIndustries.push({ value: s.SubIndustryId, text: s.SubIndustryName });
                if (isDefaultIndustry) {
                    var isDefaultSubIndustry = (s.SubIndustryId == analysisState.benchmarkCompany.SubIndustryId);
                    var isSelectedSubIndustry = selectedSubIndustries[s.SubIndustryId];
                    let d = addOption(subIndustrySelectElement, s.SubIndustryId, s.SubIndustryName, isDefaultSubIndustry, isSelectedSubIndustry);
                    if (isDefaultSubIndustry)
                        d.style.backgroundColor = lightSuggestedPeerColor;
                }
            }
        }
    }

    window.industrySearchInput = $('#CompanyIndustrySearch');
    window.industrySearchInput.select();
    window.subIndustrySearchInput = $('#CompanySubIndustrySearch');
    window.subIndustrySearchInput.multiselect(
        {
            // buttonContainer: '<div class="btn-group w-100" />',
            includeSelectAllOption: true,
            selectAllText: " All",
            numberDisplayed: 1,
            //nonSelectedText: "All selected (" + n + ")",
            inheritClass: true,
            onDropdownShown: (event) => {
                var element = window.subIndustrySearchInput;
                element._wasChanged = false;
                //pageHistory.beginOperation('subIndustriesChanged');
            },
            onDropdownHidden: (event) => {
                var element = window.subIndustrySearchInput;
                //pageHistory.endOperation(null, !element._wasChanged);
                if (element._wasChanged) {
                    analysisState.psp.selectedSubindustries = getSelectedValuesFromMultiSelect("CompanySubIndustrySearch");
                }
            },
            onChange: (event) => {
                var element = window.subIndustrySearchInput;
                element._wasChanged = true;
            }
        });

    window.industrySearchInput.change(industrySelectionChanged);

    var o = findMultiselectOption('CompanySubIndustrySearch', getSubIndustry(analysisState.benchmarkCompany.SubIndustryId));
    if (o)
        o.style.backgroundColor = lightSuggestedPeerColor;

    for (let p in table2subIndustries) {
        var o = findMultiselectOption('CompanySubIndustrySearch', p);
        if (o)
            o.style.backgroundColor = lightSuggestedPeerColor;
    }
}


function industrySelectionChanged() {
    const gsd = getGlobalStaticData();
    const industryId = parseInt($('#CompanyIndustrySearch option:selected')[0].value);
    //console.log(`industry: ${industryId}`);
    const subIndustrySelectElement = document.getElementById("CompanySubIndustrySearch");
    subIndustrySelectElement.clearChildren();
    const turnThemGreen = [];

    for (let i = 0; i < gsd.Industries.length; ++i) {
        const industry = gsd.Industries[i];
        if ((industryId == 0) || (industryId == industry.IndustryId)) {
            for (let j = 0; j < industry.SubIndustries.length; ++j) {
                const s = industry.SubIndustries[j];
                if (table2subIndustries[s.SubIndustryName]) {
                    addOption(subIndustrySelectElement, s.SubIndustryId, s.SubIndustryName, true);
                    turnThemGreen.push(s.SubIndustryName);
                } else {
                    addOption(subIndustrySelectElement, s.SubIndustryId, s.SubIndustryName, false);
                }
            }
        }
    }

    window.subIndustrySearchInput.multiselect('rebuild');

    const n = turnThemGreen.length;
    for (let i = 0; i < n; ++i) {
        const o = findMultiselectOption(CompanySubIndustrySearch, turnThemGreen[i]);
        o.style.backgroundColor = lightSuggestedPeerColor;
    }

    analysisState.psp.selectedIndustries = `${industry}`;
}


//Update session value and validation of peer selection.
function runAnalysis() {

    //var sd = getSessionData();
    [auto, self, all] = getCheckedCompanies(); // 'auto' always includes the benchmark company
    var searchResults = analysisState.psp.searchResults;
    var i = 0;
    while (i < searchResults.length) {

        const psr = searchResults[i];
        if (psr.IsSelected) {
            bAlert("You have one or more companies checked in the \"Search\" list that you haven't moved to the \"Selected\" list.",
                (x) => {
                    if (x === 2) {
                        moveToFinalList();
                        [auto, self, all] = getCheckedCompanies();
                    }
                    continueRunAnalysis(all);
                },
                undefined,
                "Ignore them",
                "Select them");
            return;
        } else {
            ++i;
        }
    }

    continueRunAnalysis(all);
    return false;
}


function continueRunAnalysis(peers) {

    if (peers.length == 0) {
        analysisState.peerCompanies = "";
    } else {
        analysisState.peerCompanies = `${peers[0]}`;
        for (var i=1; i<peers.length; ++i)
            analysisState.peerCompanies += `,${peers[i]}`;
    }

    setAnalysisState();
    const total = peers.length;

    // At least 6 company selection validation.
    if (total <= minSelectedPeers) {
        bAlert('Please select at least ' + minSelectedPeers + ' peer companies to run analysis');
    } else if (total > (maxSelectedPeers + 1)) {
        bAlert('Please select no more than ' + maxSelectedPeers + ' peer companies to run analysis');
    } else {
        window.location.href = '/PeerAMid/RunAnalysis';
    }
}


//txtMinRevenue keydown event.
//txtMaxRevenue keydown event.
$('#txtMinRevenue, #txtMaxRevenue').keydown(
    function (e) {
        if (e.keyCode === 13) {
            searchPeerCompany('all');
        }
    }
);


// Company search criteria on the basis of min and max revenue.
function searchPeerCompany(type) { // 'name', 'ticker', 'nameTicker', or 'all'

    analysisState.selectedRegions = getSelectedRegions();
    const min = $('#txtMinRevenue').val().replace(/\,/g, '') === '' ? 0 : $('#txtMinRevenue').val().replace(/\,/g, '');
    analysisState.psp.minRevenue = Number(min);
    const max = $('#txtMaxRevenue').val().replace(/\,/g, '') === '' ? 0 : $('#txtMaxRevenue').val().replace(/\,/g, '');
    analysisState.psp.maxRevenue = Number(max);

    analysisState.psp.selectedIndustries = getSelectedValuesFromMultiSelect("CompanyIndustrySearch");
    analysisState.psp.selectedSubIndustries = getSelectedValuesFromMultiSelect("CompanySubIndustrySearch");

    analysisState.psp.companySearch = $('#CompanySearch').val() || '';
    analysisState.psp.tickerSearch = $('#TickerSearch').val() || '';

    var params = {
        uid:                analysisState.benchmarkCompany.CompanyId,
        //revenueFrom:        analysisState.psp.minRevenue,
        //revenueTo:          analysisState.psp.maxRevenue,
        //year:               0,
        regions:            analysisState.selectedRegions,
        //industryFilter:     analysisState.psp.selectedIndustries,
        //subIndustryFilter:  analysisState.psp.selectedSubIndustries,
        //nameFilter:         analysisState.psp.companySearch,
        //tickerFilter:       analysisState.psp.tickerSearch,
        bindToSessionData:  false,
        maxCompanies:       100
    };

    switch (type) {

        case 'name':
            params.nameFilter = analysisState.psp.companySearch;
            params.revenueFrom = 0;
            params.revenueTo = 0;
            params.industryFilter = '*';
            params.subIndustryFilter = '*';
            params.tickerFilter = '';
            params.year = 0;
            break;

        case 'ticker':
            params.nameFilter = '';
            params.revenueFrom = 0;
            params.revenueTo = 0;
            params.industryFilter = '*';
            params.subIndustryFilter = '*';
            params.tickerFilter = analysisState.psp.tickerSearch;
            params.year = 0;
            break;

        case 'nameTicker':
            params.nameFilter = analysisState.psp.companySearch;
            params.revenueFrom = 0;
            params.revenueTo = 0;
            params.industryFilter = '*';
            params.subIndustryFilter = '*';
            params.tickerFilter = analysisState.psp.tickerSearch;
            params.year = 0;
            break;

        case 'all':
        default:
            params.nameFilter = '';
            params.revenueFrom = analysisState.psp.minRevenue;
            params.revenueTo = analysisState.psp.maxRevenue;
            params.industryFilter = analysisState.psp.selectedIndustries;
            params.subIndustryFilter = analysisState.psp.selectedSubIndustries;
            params.tickerFilter = '';
            params.year = analysisState.benchmarkCompany.FinancialYear;
            break;
    }

    /*
    const params = {
        uid:                analysisState.benchmarkCompany.CompanyId,
        revenueFrom:        analysisState.psp.minRevenue,
        revenueTo:          analysisState.psp.maxRevenue,
        year:               0,
        regions:            analysisState.selectedRegions,
        industryFilter:     analysisState.psp.selectedIndustries,
        subIndustryFilter:  analysisState.psp.selectedSubIndustries,
        nameFilter:         analysisState.psp.companySearch,
        tickerFilter:       analysisState.psp.tickerSearch,
        bindToSessionData:  false,
        maxCompanies:       100
    };
    */

    doSearch(type, params, continuePageLoad);
}

function bindFinalTable(options, callback) {
    // console.log("bindFinalTable");
    const uri = '/PeerAMid/GetAdditionalPeerCompanies';
    options.bindToSessionData = false;
    console.log(`Calling ajax: '${uri}'`);
    //console.log(JSON.stringify(options));
    $.ajax({
        type: "POST",
        url: uri,
        data: options,
        dataType: "json",
        success: function (result) {
            const c = result.aaData;
            addToFinalList(c);
            $('#CompanySearch').val('');
            $('#TickerSearch').val('');
            if (callback)
                callback();
        },
        error: function (_, _, err) {
            console.log(`Error: ${err}`);
        }
    });

}

function bindFinalTable1(company, ticker, year, callback) {
    const byCompany = (company !== null) && (company != '');
    const uri = byCompany ? '/PeerAMid/GetCompanyDetailsByName' : '/PeerAMid/GetCompanyDetailsByTicker';
    const data = { year: year };
    if (byCompany)
        data["companyName"] = company;
    else
        data["ticker"] = ticker;

    console.log(`Calling ajax: '${uri}'`);
    //console.log(JSON.stringify(options));
    $.ajax({
        type: "POST",
        url: uri,
        data: data,
        dataType: "json",
        success: function (result) {
            //console.log(result);
            //console.log(result.aaData);
            const c = result; //.aaData;
            //c.CompanyId = c.Id;
            addToFinalList(c);
            $("#CompanySearch").val('');
            //clearEasyAutocompleteList("CompanySearch");
            $("#TickerSearch").val('');
            //clearEasyAutocompleteList("TickerSearch");
            if (callback)
                callback();
        },
        error: function (_, _, err) {
            console.log(`Error: ${err}`);
        }
    });

}

//Button Click Event for add in List.
function addButtonClick() {
    const company = $('#CompanySearch').val() || '';
    if (company != null && company != "") {
        $("#divMessage").text(company + " has been added to the list.");
        $("#Modal2").modal();
        analysisState.psp.companySearch = '';
        $("#CompanySearch").val(analysisState.psp.companySearch);
        analysisState.psp.tickerSearch = '';
        $("#TickerSearch").val(analysisState.psp.tickerSearch);
    } else {
        const ticker = $('#TickerSearch').val() || '';
        if (ticker != null && ticker != "") {
            $("#divMessage").text(company + " has been added to the list.");
            $("#Modal2").modal();
            analysisState.psp.companySearch = '';
            $("#CompanySearch").val(analysisState.psp.companySearch);
            analysisState.psp.tickerSearch = '';
            $("#TickerSearch").val(analysisState.psp.tickerSearch);
        }
    }
}

//Move Selected peer company list in Companies selected list for run analysis.
function moveToFinalList() {

    var i = 0;
    var searchResults = analysisState.psp.searchResults;

    var indexes = [];
    for (var i=0; i<searchResults.length; ++i) {

        if (searchResults[i].IsSelected) {
            indexes.push(i);
        }
    }

    var toBeMoved = [];
    for (var i = indexes.length-1; i >=0; --i) {
        var j = indexes[i];
        toBeMoved.push(searchResults[j]);
        searchResults.splice(j, 1);
    }

    for (var i =0; i < toBeMoved.length; ++i) {
        analysisState.psp.selectedPeers.push(toBeMoved[i]);
    }

    continueMoveToFinalList();
}

function continueMoveToFinalList() {
    const searchResultsGrid = $("#peerSearchResultsGrid").data("kendoGrid");
    searchResultsGrid.dataSource.data(analysisState.psp.searchResults);
    const selectedPeersGrid = $('#selectedPeersGrid').data('kendoGrid');
    selectedPeersGrid.dataSource.data(analysisState.psp.selectedPeers);
    setTimeout(updateSelectedCompaniesCount);
}

function addToFinalList(companyData) {
    // console.log("addToFinalList");
    //sd = getSessionData();
    var auto, self;
    const all = getCheckedCompanies();
    var count = all.length;
    // console.log(`addToFinalList: count ${count}`);

    // If it's a single item, make an array out of it.  Hack!
    if (!Array.isArray(companyData))
        companyData = [companyData];

    const n = companyData.length;
    // console.log(`addToFinalList: n ${n}`);
    var selectedPeers = analysisState.psp.selectedPeers;
    var searchResults = analysisState.psp.searchResults;
    var alreadyInGrid = false;

    for (let i = 0; i < n; ++i) {

        const cd = companyData[i];
        const id = cd.CompanyId;

        for (let j = 0; j < selectedPeers.length; ++j) {
            if (selectedPeers[j].CompanyId === id) {
                alreadyInGrid = true;
                if (!selectedPeers[j].IsSelected && (count < maxSelectedPeers)) {
                    selectedPeers[j].IsSelected = true;
                    cd.IsSelected = true;
                    ++count;
                }
                break;
            }
        }

        if (!alreadyInGrid) {
            if (count < maxSelectedPeers) {
                cd.IsSelected = (count < maxSelectedPeers);
                ++count;
            }
            selectedPeers.push(cd);
        }

        for (let j = 0; j < searchResults.length; ++j) {
            if (searchResults[j].CompanyId === id) {
                searchResults.splice(j, 1);
                break;
            }
        }
    }

    //uploadSessionData();
    $("#selectedPeersGrid").data('kendoGrid').dataSource.data(selectedPeers);
    setTimeout(updateSelectedCompaniesCount);
}

//Update final peer selected company for run analysis.
function updateFinalPeerCompanies() {
    const [auto, self, all] = getCheckedCompanies();
    const total = all.length;

    if (total == 0) {
        analysisState.peerCompanies = "";
    } else {
        analysisState.peerCompanies = `${all[0]}`;
        for (var i=1; i<total; ++i)
            analysisState.peerCompanies += `,${all[i]}`;
    }

    setAnalysisState();
    highlightRunAnalysisButton(total > 1);
    updateNeeded = false;
}

//returns [ auto, self ], where 'auto' and 'self' are both arrays of companyIds
function getCheckedCompanies() {
    //const sd = getSessionData();

    const auto = [analysisState.benchmarkCompany.CompanyId];
    const self = [];
    const all = [analysisState.benchmarkCompany.CompanyId];
    const hash = {};
    hash[analysisState.benchmarkCompany.CompanyId] = true;

    var selectedPeers = analysisState.psp.selectedPeers;
    // table2.dump("getCheckedCompanies");
    for (let r = 0; r < selectedPeers.length; ++r) {
        const spc = selectedPeers[r];
        if (spc.IsSelected) {
            const id = spc.Id;
            if (hash[id] === undefined) {
                hash[id] = true;
                all.push(id);
                if (spc.IsAuto) {
                    //console.log("Added " + id + " to auto list");
                    auto.push(id);
                } else {
                    //console.log("Added " + id + " to self list");
                    self.push(id);
                }
            }
        }
    }

    // console.log(`getCheckedCompanies ${all.join(',')}`);
    return [auto, self, all];
}


// Highlight Run Analysis button
function highlightRunAnalysisButton(enabled) {
    const a = enabled ? "btn-outline-secondary" : "btn-secondary";
    const b = enabled ? "btn-secondary" : "btn-outline-secondary";
    $("#btnRunAnalysis").removeClass(a).addClass(b);
    // $("#btnUpdate").removeClass("btn-secondary").addClass("btn-outline-secondary");
    $("#navItemPeerCompanySelection").removeClass().addClass("innovation_tracker_history_nav-link nav-link active before-active nav-link1");
    $("#navItemConfirmAndRunAnalysis").removeClass().addClass("innovation_tracker_history_nav-link nav-link active-color nav-link1");
}

function updateButtonState(allowUndo, allowRedo) {
    // TEMPORARY!
    allowUndo = false;
    allowRedo = false;
    try {
        //debugger;
        $("#backButton").html(allowUndo ? "Undo" : "Back");
        const element = document.getElementById("redoButton");
        if (element)
            element.style.display = allowRedo ? "inline-block" : "none";
    } catch (error) {
        console.log(error);
    }
}


function displayBenchmarkCompanyDetails() {
    var bmc = analysisState.benchmarkCompany;
    $('#benchmarkCompanyName').html(bmc.Name);
    $('#benchmarkCompanyFinancialYear').html(bmc.FinancialYear);
    var rev = bmc.Revenue;
    rev = Math.floor(rev);
    rev = rev.toLocaleString();
    $('#benchmarkCompanyRevenue').html(rev);
    $('#headerIndustryName').html(getIndustry(bmc.IndustryId).IndustryName);
    $('#benchmarkCompanySubIndustry').html(getSubIndustry(bmc.SubIndustryId).SubIndustryName);
}

function chevronNavigation(id) {
    if (id == 'chevronCompanySelection')
        window.location.href = '/PeerAMid/Search';
    return 0;
}