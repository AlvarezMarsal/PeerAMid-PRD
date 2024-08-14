// Code to execute after page is loaded
$(document).ready(function () {

    setPageHeaders('chevronDepartmentCosts'); //, "Please enter the financial data.");
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
            var ebi = this.EBITDA * 10000;
            ebi = Math.round(ebi) / 100;
            ebi = `${ebi}`;
            var gmi = this.GrossMargin * 10000;
            gmi = Math.round(gmi) / 100;
            gmi = `${gmi}`;
            addRowToTable(tb, this.FinancialYear, this.DataEntryCurrency, this.Revenue.toLocaleString(), ebi, this.Expense.toLocaleString(), gmi);
        });
}


$('#btnNoUnit').click(function () {
    $('#ddlUnitOfMeasurement').val(unit);
});


// Button btnYesUnit click event
$('#btnYesUnit').click(function() {
    unit = $('#ddlUnitOfMeasurement').val();
});

//Dropdown ddlFiscalYear change event.
//Convert From Millions CostFields.
$('#ddlFiscalYear').change(onFiscalYearChange);

function onFiscalYearChange() {

    var futureYears = analysisState.mbcdp.FutureYears;
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
        $('#txtExchangeRate').val(analysisState.benchmarkCompany.DataEntryExchangeRate);
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
        $('#txtExchangeRate').val(c.DataEntryExchangeRate);
        $('#txtRevenue').val(numberWithCommas(c.Revenue));
        $('#txtEBITDA').val(numberWithCommas(c.EBITDA));
        $('#txtExpense').val(numberWithCommas(c.SGA));
        $('#txtGrossMargin').val(numberWithCommas(c.GrossMargin * c.Revenue)); // TODO: check
        $('#txtTotalEmployees').val(numberWithCommas(c.TotalNumOfEmployee));
        $('#txtFinanceCost').val(numberWithCommas(c.SGACostFinance));
        $('#txtHumanResourceCost').val(numberWithCommas(c.SGACostHumanResources));
        $('#txtInfoTechCost').val(numberWithCommas(c.SGACostIT));
        $('#txtProcurementCost').val(numberWithCommas(c.SGACostProcurement));
        $('#txtSalesCost').val(numberWithCommas(c.SGACostSales));
        $('#txtMarketingCost').val(numberWithCommas(c.SGACostMarketing));
        $('#txtCustomerServicesCost').val(numberWithCommas(c.SGACostCustomerServices));
        $('#txtCorpServicesCost').val(numberWithCommas(c.SGACostCorporateSupportServices));
        $('#txtFinanceFTE').val(numberWithCommas(c.FTEFinance));
        $('#txtHumanResourceFTE').val(numberWithCommas(c.FTEHumanResources));
        $('#txtInfoTechFTE').val(numberWithCommas(c.FTEIT));
        $('#txtProcurementFTE').val(numberWithCommas(c.FTEProcurement));
        $('#txtSalesFTE').val(numberWithCommas(c.FTESales));
        $('#txtMarketingFTE').val(numberWithCommas(c.FTEMarketing));
        $('#txtCustomerServicesFTE').val(numberWithCommas(c.FTECustomerServices));
        $('#txtCorpServicesFTE').val(numberWithCommas(c.FTECorporateSupportServices));
    }


        /*
                if (result != null) {
                    //reqCompanyLevelData Find Event
                    $('#reqCompanyLevelData').find('input,select').not(".notIn").each(function() {
                        if (result.hasOwnProperty(this.name)) {
                            this.value = result[this.name] || '';
                        }
                    });

                    //OptionalFunctional2 Find Event
                    $('#OptionalFunctional2').find('input').each(function() {
                        if (result.hasOwnProperty(this.name)) {
                            if (result[this.name] == 0) {
                                this.value = 0;
                            } else {
                                this.value = result[this.name] || '';
                            }
                            // this.value = result[this.name] || '';
                        }
                    });
                } else {
                    //reqCompanyLevelData Find Event
                    //OptionalFunctional2 Find Event
                    $('#reqCompanyLevelData').find('input,select').not(".notIn").val('');
                    $('#OptionalFunctional2').find('input').val('');
                }
            },
            error: function() {}
        });
        */
}



// Save form data in database. 
function runAnalysis(skipDeptLevelDataValidation) {
    // Basic field validation
    var result = validateData("reqCompanyLevelData", "req");
    var optionalData = true;
    $('#isOptionalDataReq').val('1');

    // Department Data Validation
    if (!skipDeptLevelDataValidation) {
        var optionalData = validateData("OptionalFunctional2", "req");
        if (result) {
            result = optionalData;
        }
    }

    var revenue = $('#txtRevenue').val() || 0;
    if (revenue <= 0) {
        $('#txtRevenue').css("border-color", "red");
        result = false;
    }

    // Departmental Data Validation - Perform if 'skip' flag is not set to True (PEER-2)
    if (!skipDeptLevelDataValidation) {
    
        // Get Functional Department Level COST data values
        var c1 = parseFloat($('#txtFinanceCost').val());
        var c2 = parseFloat($('#txtHumanResourceCost').val());
        var c3 = parseFloat($('#txtInfoTechCost').val());
        var c4 = parseFloat($('#txtProcurementCost').val());
        var c5 = parseFloat($('#txtSalesCost').val());
        var c6 = parseFloat($('#txtMarketingCost').val());
        var c7 = parseFloat($('#txtCustomerServicesCost').val());
        var c8 = parseFloat($('#txtCorpServicesCost').val());
        var sgaC = parseFloat($('#txtExpense').val());

        // Get Functional Department Level FTE data values
        var f1 = parseFloat($('#txtFinanceFTE').val());
        var f2 = parseFloat($('#txtHumanResourceFTE').val());
        var f3 = parseFloat($('#txtInfoTechFTE').val());
        var f4 = parseFloat($('#txtProcurementFTE').val());
        var f5 = parseFloat($('#txtSalesFTE').val());
        var f6 = parseFloat($('#txtMarketingFTE').val());
        var f7 = parseFloat($('#txtCustomerServicesFTE').val());
        var f8 = parseFloat($('#txtCorpServicesFTE').val());
        var totEmp = parseFloat($('#txtTotalEmployees').val());

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

function saveAndNavigate2() {

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
        success: function (result) {
            if (result > 0) {
                window.location.href = '/PeerAMid/RunAnalysis';
            }
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
