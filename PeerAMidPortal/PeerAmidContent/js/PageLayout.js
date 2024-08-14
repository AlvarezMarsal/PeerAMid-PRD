var autoLogOffTime;    
var autoLogOffInterval;
var autoLogOffTimeOut


function resetAutoLogOff() {
    const now = Date.now();
    // console.warn("resetAutoLogOff at " + now);
    autoLogOffTime = now + (autoLogOffTimeOut * 1000);
    // console.warn("autoLogOffTime is " + autoLogOffTime);
}

function checkForAutoLogoff() {
    const now = Date.now();
    // console.warn("checkForAutoLogoff at " + now);
    const diff = autoLogOffTime - now;
    if (diff < 1) {
        // console.log("logging off ");
        resetAutoLogOff();
        window.location = '/Account/Logout';
    } else {
        const e = diff / 1000;
        //console.log("log off in " + e + " seconds");
    }
}

var selectedRegionsControl = null;
var industrySearchInput = null;
var subIndustrySearchInput = null;

function selectService(service) {
    //console.log("selectService " + service);
    resetAnalysisState(service);
    const uri = '/PeerAMid/SelectService';
    console.log(`Calling ajax: ${uri}`);
    $.ajax({
        type: "GET",
        url: uri,
        data: { selectedService: service },
        dataType: "json",
        //async: false,
        success: function () {
            if (sessionData)
                sessionData.SelectedService = service;
        },
        error: function (_, _, err) {
            console.log("error");
            console.log(err);
        }
    }); 
}


function setupRegionsSelector(rs) {
    const gsd = getGlobalStaticData();

    //var select = document.getElementById('selectedRegions');
    const n = gsd.AllRegions.length;
    for (let i = 0; i < n; ++i) {
        const option = document.createElement("option");
        option.value = gsd.AllRegions[i].Id.toString();
        option.text = gsd.AllRegions[i].Name + "  (" + numberWithCommas(gsd.AllRegions[i].NumberOfCompanies) + " companies)";
        rs.appendChild(option);
    }

    var all =  ` All (${numberWithCommas(gsd.TotalCompanies)} companies)`;
    const sr = $('#selectedRegions');
    sr.multiselect({
        buttonContainer: '<div class="btn-group w-100" />',
        includeSelectAllOption: true,
        selectAllText: " " + all,
        numberDisplayed: 1,
        nonSelectedText: "All regions",
        allSelectedText: "All regions",
        inheritClass: true,
        selectAllNumber : false,
    });

    setSelectedRegions(analysisState.selectedRegions);
}


// Code to execute after page is loaded
$(document).ready(function() {

    getSessionData();
    autoLogOffTimeOut = sessionData.AutoLogOffTimeout;
    autoLogOffTime = Date.now() + (autoLogOffTimeOut * 1000);    

    if (autoLogOffInterval)
        clearInterval(autoLogOffInterval);
    resetAutoLogOff();
    autoLogOffInterval = setInterval(checkForAutoLogoff, 30000); // every 30 seconds

    const url = location.href.split("/");
    const path = url[3] || '';
    const li = $('ul.navbar-nav li');
    li.find('a.nav-link').removeClass("active");
    switch (path) {
    case "PeerAMid":
    case "HighLevelCost":
        $('ul.navbar-nav li:eq(0)').find('a.nav-link').addClass("active");
        break;
    case "Account":
        $('ul.navbar-nav li:eq(3)').find('a.nav-link').addClass("active");
        break;
    }
    //To grey-out menu Dropdown-items
    $('a.dropdown-item[href="javascript:void(0)"]').css("color", "#D0D0D0");
    $('a.dropdown-item[href="javascript:void(0)"]').css("cursor", "default");

    //console.log('setting up selected regions ' + '@SessionData.Instance.SelectedRegions');
    const rs = document.getElementById("selectedRegions");
    if (rs)
        setupRegionsSelector(rs);
});

function getSelectedRegions() {
    var s = getSelectedRegionsWithoutCallingBackEnd();
    AjaxCallWithOutURL('POST', '/PeerAMid/SetSessionVariables', { type: 9, searchAdditional: s }); // sets Session.SelectedRegions
    return s;
}

function getSelectedRegionsWithoutCallingBackEnd() {
    const r = $('#selectedRegions option:selected'); //.map(function (a, item) { return item.value; });
    var s = "";
    for (let i = 0; i < r.length; ++i) {
        if (s.length > 0) s += ",";
        s += r[i].value;
    }
    // console.log("getSelectedRegions");
    return s;
}


function getSelectedRegionsArray() {
    const r = $('#selectedRegions option:selected'); //.map(function (a, item) { return item.value; });
    const s = [];
    for (let i = 0; i < r.length; ++i) {
        s.push(r[i].value);
    }
    return s;
}

function setSelectedRegions(regions) {
    const r = $('#selectedRegions');
    setMultiSelectOptions(r, regions.split(','));
}

function setGridColumnWidth(gridId, columnName, width) {
    $(`#${gridId} .k-grid-header-wrap`).find("colgroup col").eq(columnName).width(width + "px");
    $(`#${gridId} .k-grid-content`).find("colgroup col").eq(columnName).width(width + "px");
}

function setGridColumnWidths(/*arguments*/) {
    var index = 0;
    var total = 0;
    const gridId = arguments[index++];
    while (index < arguments.length) {
        const columnName = arguments[index++];
        const width = arguments[index++];
        setGridColumnWidth(gridId, columnName, width);
        total += width;
    }
    $(`#${gridId} .k-grid`).width(total);
}



function setSessionVariables(t, s1, s2, s3, success) {
    if ((s1 === undefined) || (s1 === null)) s1 = "";
    if ((s2 === undefined) || (s2 === null)) s2 = "";
    if ((s3 === undefined) || (s3 === null)) s3 = "";
    const d = { type: t, searchText1: s1, searchText2: s2, searchAdditional: s3 };
    if ((success === undefined) || (success == null))
        success = function(_) {};
    const u = '/PeerAMid/SetSessionVariables';
    console.log(`Calling ajax: ${u}`);
    $.ajax({
        type: 'post',
        url: u,
        data: d,
        dataType: "json",
        success: function(r) { success(r) },
        error: function() {}
    });

}


function createHashFromCsv(csv) {
    const hash = { count: 0 };
    if ((csv === null) || (csv === undefined))
        return hash;
    const parts = csv.split(',');
    for (let i = 0; i < parts.length; ++i) {
        const part = parts[i].trim();
        if (part.length > 0) {
            hash[part] = true;
            hash.count++;
        }
    }
    return hash;
}

function showServiceDescription() {

    var title = "";
    switch (analysisState.service) {
        case 1: title = 'SG&A Full Functional Diagnostics'; break;
        case 2: title = 'Working Capital Full Diagnostics'; break;
        case 3: title = 'SG&A Cost High Level Diagnostics'; break;
        case 4: title = 'Cash Conversion Cycle Diagnostics'; break;
        case 5: title = 'Retail Diagnostic'; break;
        case 6: title = 'Retail High Level Cost Diagnostic'; break;
    }

    var e = document.getElementById('ServiceDescription');
    e.childNodes[0].textContent = title;
    if (title == "")
        e.parentElement.setAttribute("style", "display:none;");
    else
        e.parentElement.setAttribute("style", "display:block;");
}


function setPageHeaders(chevron, instructions) {

    showServiceDescription();
    showChevrons(chevron);
    var e = document.getElementById('InstructionsParent');
    if (instructions) {
        e.style.display = "block";
        $('#Instructions').html(instructions).show();
    }
    else {
        e.style.display = "none";
    }
}

// Note -- this function is usually overriden by the page logic
function chevronNavigation(id) {
    if (id == 'chevronCompanySelection')
        window.location.href = '/PeerAMid/Search';
    else if (id == 'chevronPeerCompanySelection')
        window.location.href = '/PeerAMid/CompanyAutoSelection';
    else if (id == 'chevronDepartmentCosts')
        window.location.href = '/PeerAMid/NewCompany';
    else if (id == 'lastTab_1020')
        window.location.href = '/PeerAMid/RunAnalysis';
    return 0;
}