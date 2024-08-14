// Code to execute after page is loaded
$(document).ready(function () {

    setPageHeaders('chevronDepartmentCosts'); //, "Please enter the financial data.");

    if (analysisState.isModifyingBenchmarkCompanyData) {
        $("#PastDataLabel").hide();
        $("#TableHeader").hide();
    } else {
        $("#PastDataLabel").show();
        $("#TableHeader").show();
    }

    if (analysisState.service == 3) { // SgaShort -- no depts
        $("#DepartmentLevelHeader").hide();
        $("#DepartmentLevelBody").hide();
    } else {
        $("#DepartmentLevelHeader").show();
        $("#DepartmentLevelBody").show();
    }

    load1();

});

function load1() {

    getGlobalStaticData();

    var bmc = analysisState.benchmarkCompany;
    $('#benchmarkCompanyName').html(bmc.Name);
    $('#hdnCompanyId').val(bmc.CompanyId);

    var uri = '/PeerAMid/GetBenchmarkCompanyData';
    console.log(`Calling ajax: ${uri}`);

    $.ajax({
        async: false,
        type: "Get",
        url: uri,
        data: { companyId: bmc.Id, year: bmc.FinancialYear },
        headers: config,
        success: function (result) {
            analysisState.mbcdp = {}
            analysisState.mbcdp.FutureYears = result.FutureYears;
            analysisState.mbcdp.CompanyYears = result.CompanyYears;
            load2();
        },
        error: function (a, b, err) {
            analysisState.mbcdp = {}
            analysisState.mbcdp.FutureYears = null;
            analysisState.mbcdp.CompanyYears = null;
            console.log(err);
            load2();
        }
    });
}

function load2() {

    var bmc = analysisState.benchmarkCompany;
    var futureYears = analysisState.mbcdp.FutureYears;
    var companyYears = analysisState.mbcdp.CompanyYears;

    $.each(futureYears,
        function () {
            //console.log(result);
            $("#ddlFiscalYear").append($('<option></option>').val(this).html(this));
        });
    $(`#ddlFiscalYear option[value='${bmc.FinancialYear}'`).attr("selected", "selected").change();


    /* foreach(var item in Model.CompanyList)
    {
        <tr>
            <td>@item.FinancialYear</td>
            <td>@item.DataEntryCurrency</td>
            <td>@Convert.ToInt64(item.Revenue).ToString("#,##0")</td>
            <td>@string.Format("{0:P}", item.EBITDA)</td>
            <td>@Convert.ToInt64(item.Expense).ToString("#,##0")</td>
            <td>@string.Format("{0:P}", item.GrossMargin)</td>
        </tr>
    }*/

    var tb = $('#companyYearTableBody');

    $.each(companyYears,
        function () {
            var rev, exp, ebi, gmi;
            if (this.IsActualData) {
                rev = numberWithCommas(this.Revenue, 0);
                exp = numberWithCommas(this.Expense, 0);
                ebi = fractionToPercentage(this.EbitdaMargin, 2);
                gmi = fractionToPercentage(this.GrossMargin / this.Revenue, 2);
            } else {
                rev = numberWithCommas(this.Revenue, 0);
                exp = numberWithCommas(this.Expense, 0);
                ebi = fractionToPercentage(this.EbitdaMargin, 2);
                gmi = fractionToPercentage(this.GrossMargin, 2);
            }
            addRowToTable(tb, this.FinancialYear, this.DataEntryCurrency, rev, ebi, exp, gmi);
            console.log("companyYear: " + JSON.stringify(this));
        });

    var currYear = analysisState.benchmarkCompany.FinancialYear;
    const gsd = globalStaticData;

    $.each(gsd.Currencies,
        function (index, value) {
            // console.log(value);
            $("#ddlCurrency").append($('<option></option>').val(value.Name).html(value.Name));
        });
    var currency = analysisState.benchmarkCompany.DataEntryCurrency;
    $(`#ddlCurrency option[value=${currency}]`).attr("selected", "selected");

    /*
    console.log("Calling ajax /api/HomeAPI/GetFutureYearList");
    $.ajax({
        type: "Get",
        url: '/api/HomeAPI/GetFutureYearList',
        data: { industoryId: this.value },
        dataType: "json",
        headers: config,
        success: function (result) {
            $("#ddlFiscalYear").empty();
            if (result.length > 0) {

                $.each(result,
                    function () {
                        // console.log(result);
                        $("#ddlFiscalYear").append($('<option></option>').val(this).html(this));
                    });
            }
            $(`#ddlFiscalYear option[value=${currYear}]`).attr("selected", "selected");
            continueLoadingPage1();
        },
        error: function () { }
    });
    */
}

/*
function continueLoadingPage1() {

    var bmc = analysisState.benchmarkCompany;
    var startdate = globalStaticData.CurrentFinancialYear;
    $('#ddlUnitOfMeasurement').val(bmc.DataEntryUnitOfMeasure);
    bindFY($('#ddlFiscalYear'), startdate, Number(startdate) + Number(0), 0);
    //bindCurrency("ddlCurrency", '@(Model.CompanyModel == null ? "" : Model.CompanyModel.DataEntryCurrency)');
    //var unit = '@Model.CompanyModel.DataEntryUnitOfMeasure';
    var exchangeRate = bmc.DataEntryExchangeRate;
    $('#txtExchangeRate').val(exchangeRate.toFixed(4));
    //$('#ddlUnitOfMeasurement').change(function () {
    //    //unit = $('#ddlUnitOfMeasurement').val();
    //    $("#Modal1").modal();
    //})

    var url = '/PeerAMid/ReallyGetBenchmarkCompanyLatestData';
    console.log("Calling ajax " + url);
    $.ajax({
        type: "Get",
        url: url,
        data: { uid: bmc.CompanyId, year: bmc.FinancialYear },
        dataType: "json",
        headers: config,
        success: function (result) {
            continueLoadingPage2(result);
        },
        error: function () { }
    });

}

function continueLoadingPage2(data) {

    var tbody = $('#yearByYearData');
    data.forEach(function (d) {
        console.log(JSON.stringify(d));
        var tr = $('<tr>');
        var r = numberWithCommas(d.Value.Revenue);
        var e = Math.round(d.Value.EbitdaMargin * 10000) / 100;
        var x = numberWithCommas(d.Value.Expense);
        var g = numberWithCommas(d.Value.GrossMargin);
        tr.append(`<td>${d.Value.FinancialYear}</td><td>${d.Value.DataEntryCurrency}</td><td>${r}</td><td>${e}</td><td>${x}</td><td>${g}</td>`);
        tbody.append(tr);
    });

}
*/

//  btnNoUnit Click Event
$('#btnNoUnit').click(function() {
    $('#ddlUnitOfMeasurement').val(unit);
});

//  btnYesUnit Click Event
$('#btnYesUnit').click(function() {
    unit = $('#ddlUnitOfMeasurement').val();
});

//Dropdown ddlFiscalYear change event.
//Convert From Millions CostFields.
$('#ddlFiscalYear').change(onFiscalYearChange);

function onFiscalYearChange() {

    // var futureYears = analysisState.mbcdp.FutureYears;
    var companyYears = analysisState.mbcdp.CompanyYears;
    var chosenYear = $('#ddlFiscalYear').val();
    if (chosenYear != '') {

        chosenYear = parseInt(chosenYear);
        var cyi = -1;
        for (var i = 0; i < companyYears.length; ++i) {
            if (companyYears[i].FinancialYear == chosenYear) {
                cyi = i;
                break;
            }
        }
    }

    if (cyi == -1) {

        var unit = analysisState.benchmarkCompany.DataEntryUnitOfMeasure;
        $('#ddlUnitOfMeasurement').val(unit);
        $('#ddlCurrrency').val(analysisState.benchmarkCompany.DataEntryCurrency);
        $('#txtExchangeRate').val(analysisState.benchmarkCompany.DataEntryExchangeRate.toFixed(4));
        $('#txtRevenue').val('');
        $('#txtEBITDA').val('');
        $('#txtExpense').val('');
        $('#txtGrossMargin').val('');
        $('#txtTotalEmployees').val('');
        $('#txtFinanceCost').val('');
        $('#txtHumanResourceCost').val('');
        $('#txtInfoTechCost').val('');
        $('#txtProcurementCost').val('');
        $('#txtSalesCost').val('');
        $('#txtMarketingCost').val('');
        $('#txtCustomerServicesCost').val('');
        $('#txtCorpServicesCost').val('');
        $('#txtFinanceFTE').val('');
        $('#txtHumanResourceFTE').val('');
        $('#txtInfoTechFTE').val('');
        $('#txtProcurementFTE').val('');
        $('#txtSalesFTE').val('');
        $('#txtMarketingFTE').val('');
        $('#txtCustomerServicesFTE').val('');
        $('#txtCorpServicesFTE').val('');
    } else {

        var c = companyYears[i];

        var unit = c.DataEntryUnitOfMeasure;
        $('#ddlUnitOfMeasurement').val(unit);
        $('#ddlCurrrency').val(c.DataEntryCurrency);
        $('#txtExchangeRate').val(c.DataEntryExchangeRate.toFixed(4));
        $('#txtRevenue').val(numberWithCommas(c.Revenue));
        $('#txtEBITDA').val(numberWithCommas(c.EBITDA));
        $('#txtExpense').val(numberWithCommas(c.SGA));
        $('#txtTotalEmployees').val(numberWithCommas(c.TotalNumOfEmployee));

        if (analysisState.service != 3) { // SgaShort -- no depts

            if (c.IsActualData) {
                // From ClientActualData
                $('#txtGrossMargin').val(numberWithCommas(c.GrossMargin));
                $('#txtFinanceCost').val(numberWithCommas(c.SGACostFinance * c.Revenue));
                $('#txtHumanResourceCost').val(numberWithCommas(c.SGACostHumanResources * c.Revenue));
                $('#txtInfoTechCost').val(numberWithCommas(c.SGACostIT * c.Revenue));
                $('#txtProcurementCost').val(numberWithCommas(c.SGACostProcurement * c.Revenue));
                $('#txtSalesCost').val(numberWithCommas(c.SGACostSales * c.Revenue));
                $('#txtMarketingCost').val(numberWithCommas(c.SGACostMarketing * c.Revenue));
                $('#txtCustomerServicesCost').val(numberWithCommas(c.SGACostCustomerServices * c.Revenue));
                $('#txtCorpServicesCost').val(numberWithCommas(c.SGACostCorporateSupportServices * c.Revenue));
                $('#txtFinanceFTE').val(numberWithCommas(c.FTEFinance));
                $('#txtHumanResourceFTE').val(numberWithCommas(c.FTEHumanResources));
                $('#txtInfoTechFTE').val(numberWithCommas(c.FTEIT));
                $('#txtProcurementFTE').val(numberWithCommas(c.FTEProcurement));
                $('#txtSalesFTE').val(numberWithCommas(c.FTESales));
                $('#txtMarketingFTE').val(numberWithCommas(c.FTEMarketing));
                $('#txtCustomerServicesFTE').val(numberWithCommas(c.FTECustomerServices));
                $('#txtCorpServicesFTE').val(numberWithCommas(c.FTECorporateSupportServices));
            } else {
                // From FACTS
                $('#txtGrossMargin').val(numberWithCommas(c.GrossMargin * c.Revenue)); 
                $('#txtFinanceCost').val(numberWithCommas(c.SGACostFinance * c.Revenue));
                $('#txtHumanResourceCost').val(numberWithCommas(c.SGACostHumanResources * c.Revenue));
                $('#txtInfoTechCost').val(numberWithCommas(c.SGACostIT * c.Revenue));
                $('#txtProcurementCost').val(numberWithCommas(c.SGACostProcurement * c.Revenue));
                $('#txtSalesCost').val(numberWithCommas(c.SGACostSales * c.Revenue));
                $('#txtMarketingCost').val(numberWithCommas(c.SGACostMarketing * c.Revenue));
                $('#txtCustomerServicesCost').val(numberWithCommas(c.SGACostCustomerServices * c.Revenue));
                $('#txtCorpServicesCost').val(numberWithCommas(c.SGACostCorporateSupportServices * c.Revenue));
                $('#txtFinanceFTE').val(numberWithCommas(c.FTEFinance));
                $('#txtHumanResourceFTE').val(numberWithCommas(c.FTEHumanResources));
                $('#txtInfoTechFTE').val(numberWithCommas(c.FTEIT));
                $('#txtProcurementFTE').val(numberWithCommas(c.FTEProcurement));
                $('#txtSalesFTE').val(numberWithCommas(c.FTESales));
                $('#txtMarketingFTE').val(numberWithCommas(c.FTEMarketing));
                $('#txtCustomerServicesFTE').val(numberWithCommas(c.FTECustomerServices));
                $('#txtCorpServicesFTE').val(numberWithCommas(c.FTECorporateSupportServices));

                var tot = c.SGACostFinance + c.SGACostHumanResources + c.SGACostIT + c.SGACostProcurement + c.SGACostSales + c.SGACostMarketing + c.SGACostCustomerServices + c.SGACostCorporateSupportServices;
                console.log("tot = " + tot);
                var totrev = tot * c.Revenue;
                console.log("totrev = " + totrev);
            }

        }
    }
}



// Save form data in database. 
function saveCompletedDataAndNavigate() {

    var skipDeptLevelDataValidation = (analysisState.service == 3); // SgaShort
    var result = validateData("reqCompanyLevelData", "req", skipDeptLevelDataValidation);

    var revenue = $('#txtRevenue').val().replaceAll(",", "") || 0;
    if (revenue <= 0) {
        $('#txtRevenue').css("border-color", "red");
        result = false;
    }

    var totEmp = parseFloat($('#txtTotalEmployees').val().replaceAll(",", ""));
    if (totEmp <= 0) {
        $('#txtTotalEmployees').css("border-color", "red");
        result = false;
    }

    var sgaC = parseFloat($('#txtExpense').val().replaceAll(",", ""));

    // Departmental Data Validation - Perform if 'skip' flag is not set to True (PEER-2)
    if (!skipDeptLevelDataValidation) {

        // Get Functional Department Level COST data values
        var c1 = parseFloat($('#txtFinanceCost').val().replaceAll(",", ""));
        var c2 = parseFloat($('#txtHumanResourceCost').val().replaceAll(",", ""));
        var c3 = parseFloat($('#txtInfoTechCost').val().replaceAll(",", ""));
        var c4 = parseFloat($('#txtProcurementCost').val().replaceAll(",", ""));
        var c5 = parseFloat($('#txtSalesCost').val().replaceAll(",", ""));
        var c6 = parseFloat($('#txtMarketingCost').val().replaceAll(",", ""));
        var c7 = parseFloat($('#txtCustomerServicesCost').val().replaceAll(",", ""));
        var c8 = parseFloat($('#txtCorpServicesCost').val().replaceAll(",", ""));

        // Get Functional Department Level FTE data values
        var f1 = parseFloat($('#txtFinanceFTE').val().replaceAll(",", ""));
        var f2 = parseFloat($('#txtHumanResourceFTE').val().replaceAll(",", ""));
        var f3 = parseFloat($('#txtInfoTechFTE').val().replaceAll(",", ""));
        var f4 = parseFloat($('#txtProcurementFTE').val().replaceAll(",", ""));
        var f5 = parseFloat($('#txtSalesFTE').val().replaceAll(",", ""));
        var f6 = parseFloat($('#txtMarketingFTE').val().replaceAll(",", ""));
        var f7 = parseFloat($('#txtCustomerServicesFTE').val().replaceAll(",", ""));
        var f8 = parseFloat($('#txtCorpServicesFTE').val().replaceAll(",", ""));

        // Validate FTE Counts
        if ((f1 + f2 + f3 + f4 + f5 + f6 + f7 + f8) > totEmp) {
            result = false;
            bAlert("Sum of functional FTEs should always be less than or equal to the Total Employees.");
        }

        // Validate Cost Totals
        if ((c1 + c2 + c3 + c4 + c5 + c6 + c7 + c8) > sgaC) {
            result = false;
            bAlert("SG&A functional costs must be less than or equal to total SG&A cost.");
        }
    }

    // Submit form data as long as there are no validation errors
    if (result) {
        confirmTermsAndConditions(saveCompletedDataAndNavigate2);
    }
}

function saveCompletedDataAndNavigate2() {

    var exchangeRate = parseFloat($('#txtExchangeRate').val());
    if (exchangeRate < 0.000001)
        exchangeRate = 1;
    analysisState.fullCompanyData = $('#formCompleteDataByUser').serializeArray();
    var formData = $('#formCompleteDataByUser').serialize();
    formData = "CompanyId=" + analysisState.benchmarkCompany.CompanyId + "&IsOptionalData=1&" + formData;
    //console.log(formData);
    //formData += `&ExchangeRate=${exchangeRate.toFixed(4)}`;
    console.log("Calling ajax: '/PeerAMid/SaveActualLatestDataCollectionNew'");
    $.ajax({
        type: "Post",
        url: '/PeerAMid/SaveActualLatestDataCollectionNew',
        beforeSend: function() {
            $('.loader-div').show();
        },
        complete: function() {
            $('.loader-div').hide();
        },
        data: formData,
        dataType: "text",
        headers: config,
        success: function(result) {
            var bmc = JSON.parse(result);
            console.log("bmc: " + JSON.stringify(bmc, '\n'));
            console.log("analysisState.benchmarkCompany: " + JSON.stringify(analysisState.benchmarkCompany, '\n'));
            analysisState.benchmarkCompany.DataYear = bmc.Year;
            analysisState.benchmarkCompany.Ebitda = bmc.Ebitda;
            analysisState.benchmarkCompany.EBITDA = bmc.Ebitda;
            analysisState.benchmarkCompany.EbitdaMargin = bmc.EbitdaMargin;
            analysisState.benchmarkCompany.Expense = bmc.Expense;
            analysisState.benchmarkCompany.FinancialYear = bmc.Year;
            analysisState.benchmarkCompany.IsActualData = true;
            analysisState.benchmarkCompany.GrossMargin = bmc.GrossMargin;
            analysisState.benchmarkCompany.Revenue = bmc.Revenue;
            analysisState.benchmarkCompany.SGA = bmc.Expense;
            analysisState.benchmarkCompany.TotalEmployees = bmc.TotalNumOfEmployee;
            analysisState.benchmarkCompany.Year = bmc.Year;
            analysisState.bcsp.peerSelectionMode = 0; // not 3, which would just bring us back here
            setAnalysisState();
            if (analysisState.isModifyingBenchmarkCompanyData)
                window.location.href = '/PeerAMid/CompanyAutoSelection';
            else if (analysisState.peerCompanies.length == 0) {
                window.location.href = '/PeerAMid/PeerSelectionOption';
            } else if (analysisState.peerCompanies.length < 6)
                window.location.href = '/PeerAMid/CompanyAutoSelection';
            else
                window.location.href = '/PeerAMid/RunAnalysis';
        },
        error: function () { analysisState.fullCompanyData = []; }
    });
}

/*
*  Validate the specified Form field
*
*  formId - ID of the container that holds the form field
*  className - Name of field to validate
*
*/
function validateData(formId, className) {
    var flag = true;
    const controls = $(`#${formId}`).find(`.${className}`);
    if (controls.length > 0) {
        $.each(controls,
            function() {
                if (this.value == '') {
                    $(this).css("border-color", "red");
                    flag = false;
                } else {
                    $(this).css("border-color", "");
                }
            });
        return flag;
    }
}

/*
*  Expand/Collapse container next to specified checkbox control
*
*  checkbox - ID of checkbox field
*
*/
function expandCollapse(checkbox) {
    myContainerName = $(checkbox).closest('section').next().attr('id');
    if ($(checkbox).is(":checked")) {
        $(`#${myContainerName}`).collapse('show');
    } else {
        $(`#${myContainerName}`).collapse('hide');
    }
}


function backButton() {
//    if (analysisState.service == 3) { // SgaShort -- no depts
        window.location.href = "/PeerAMid/PeerSelectionOption";
//    } else {
//        window.location.href = "/PeerAMid/PeerSelectionOption";
//    }
}

function nextButton() {
    //if (analysisState.isModifyingBenchmarkCompanyData) {
        saveCompletedDataAndNavigate();
    //} else {
    //    saveCompletedDataAndNavigate(false);
    //}
}