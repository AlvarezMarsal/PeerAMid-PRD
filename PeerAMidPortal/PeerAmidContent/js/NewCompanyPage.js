/*

    $('#isOptionalDataReq').val('1');
    var startdate = '@SessionData.Instance.CurrentFinancialYear';

    var subIndustry = $('#ddlSubIndustry');
*/

var originalIndustry = null;
var originalSubIndustry = null;

$(document).ready(function () {

    setPageHeaders('chevronCompanySelection');

    const gsd = getGlobalStaticData();

    $("#ddlFiscalYear").empty();
    $("#ddlFiscalYear").append('<option value="">Select</option>');


    $("#ddlCurrency").empty();
    const currency = 'USD';

    //console.log(JSON.stringify(gsd));
    $.each(gsd.Currencies,
        function (index, value) {
            //console.log(value);
            $("#ddlCurrency").append($('<option></option>').val(value.Name).html(value.Name));
        });
    $(`#ddlCurrency option[value=${currency}]`).attr("selected", "selected");

    const exchangeRate = 1;
    $('#txtExchangeRate').val(exchangeRate.toFixed(4));

    $.each(gsd.Industries,
        function (index, value) {
            //console.log(value);
            $("#ddlIndustry").append($('<option></option>').val(value.IndustryId).html(value.IndustryName));
        });

    $('#TnCAgree').click(function () {
        saveAndNavigate2();
    });

    console.log("Calling ajax /api/HomeAPI/GetFutureYearList");
    $.ajax({
        type: "Get",
        url: '/api/HomeAPI/GetFutureYearList',
        data: { industoryId: this.value },
        dataType: "json",
        headers: config,
        success: function (result) {
            if (result.length > 0) {
                $.each(result,
                    function () {
                        //console.log(result);
                        $("#ddlFiscalYear").append($('<option></option>').val(this).html(this));
                    });
            }
            continueLoading();
        },
        error: function () { }
    });
});


function continueLoading() {
    if (analysisState.benchmarkCompany.CompanyId > 0)
        loadBenchmarkCompanyData();
}

//Load sub industry data in dropdown. procedure name Proc_GetSubIndustryList.
$('#ddlIndustry').change(function () {
    onIndustryChange(this);
});

function onIndustryChange(element) {
    var subIndustry = $('#ddlSubIndustry');
    subIndustry.empty();
    subIndustry.append('<option value="">Select</option>');
    if (this.value != '') {
        var val = element.value;
        const gsd = globalStaticData;
        for (let i = 0; i < gsd.Industries.length; ++i) {
            const industry = gsd.Industries[i];
            if (industry.IndustryId == val) {
                const sub = industry.SubIndustries;
                $.each(sub,
                    function () {
                        subIndustry.append($('<option></option>').val(this.SubIndustryId).html(this.SubIndustryName));
                    }
                );
            }
        }
    }
}


//Save new company’s actual data in database. procedure name
function saveAndNavigate(skipDeptLevelDataValidation) {
    // Basic field validation
    var result = validateData("reqCompanyLevelData", "req");
    // -- Perform basic field validation on deparmental data, unless 'skip' flag is set to True (PEER-14)
    optionalData = false;
    if (!skipDeptLevelDataValidation) {
        optionalData = validateData("OptionalFunctional", "req");
    } else {
        optionalData = true;
    }
    $('#isOptionalDataReq').val('1');

    var exchangeRate = $('#txtExchangeRate').val() || 1;
    if (txtExchangeRate <= 0) {
        $('#txtRevenue').css("border-color", "red");
        result = false;
    }

    var revenue = $('#txtRevenue').val() || 0;
    if (revenue <= 0) {
        $('#txtRevenue').css("border-color", "red");
        result = false;
    }

    // Departmental Data Validation - Perform unless 'skip' flag is set to True (PEER-14)
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

    // Save data when no error occurred
    if (result && optionalData) {
        $("#TandCconfirm").modal("show");
    }
    return false;
}

function saveAndNavigate2() {
    analysisState.fullCompanyData = $('#formAddCompany').serializeArray();
    var formData = $('#formAddCompany').serialize();
    console.log("Calling ajax /PeerAMid/SaveActualDataCollectionNew");
    // console.log(JSON.stringify(formData));
    $.ajax({
        type: "Post",
        url: '/PeerAMid/SaveActualDataCollectionNew',
        data: JSON.stringify(formData),
        dataType: "text",
        //headers: config,
        success: function (json) {
            var response = JSON.parse(json);
            // console.log(JSON.stringify(response));
            if (response.CompanyId > 0) {                     
                setBenchmarkCompanyAndNotifyBackEnd(response, true);
                setAnalysisState();
                navigateToPeerSelection();
            } else if (response.CompanyId == -1) {
                analysisState.fullCompanyData = [];
                bAlert("Company data already exists");
            } else {
                analysisState.fullCompanyData = [];
                bAlert("Company data not created");
            }
        },
        error: function (a, b, err) {
            console.log(JSON.stringify(b));
            bAlert(`Could not save Company data:${JSON.stringify(err)}`);
        }
    });
}


//Update session value.
// Get Benchmark Company data and redirect in Peer selection option page. 
function navigateToPeerSelection(uid, year) {

    window.location.href = '/PeerAMid/PeerSelectionOption';
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
            function () {
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

 // Note -- this function is usually overriden by the page logic
function chevronNavigation(id) {
    if (id == 'chevronCompanySelection')
        window.location.href = '/PeerAMid/Search';
//    else if (id == 'chevronPeerCompanySelection')
//        window.location.href = '/PeerAMid/CompanyAutoSelection';
//    else if (id == 'chevronDepartmentCosts')
//        window.location.href = '/PeerAMid/NewCompany';
//    else if (id == 'lastTab_1020')
//        window.location.href = '/PeerAMid/RunAnalysis';
    return 0;
}

function loadBenchmarkCompanyData() {

    var bmc = analysisState.benchmarkCompany;
    //originalIndustry = bmc.IndustryId;
    //originalSubIndustry = bmc.SubIndustryId;

    //$('#ddlIndustry').val(bmc.IndustryId);
    //$('#ddlUnitOfMeasurement').val(bmc.DataEntryUnitOfMeasure);
    //$('#ddlCompanyType').val(bmc.CompanyType);
    //$('#ddlSubIndustry').val(bmc.SubIndustryId);
    //$('#ddlCurrency').val(bmc.DataEntryCurrency);
    //$('#txtExchangeRate').val(bmc.DataEntryExchangeRate);

    var sid = null;
    var yid = null;
    for (var i = 0; i < analysisState.fullCompanyData.length; ++i) {
        var item = analysisState.fullCompanyData[i];
        if (item.name == "SubIndustryId") {
            sid = item.value;
            continue;
        }
        else if (item.name == "YearId") {
            yid = item.value;
            continue;
        }
        var selector = '[name="' + item.name + '"]';
        $(selector).val(item.value);
    }
    onIndustryChange(document.getElementById('ddlIndustry'));
    var selector = '[name="' + "SubIndustryId" + '"]';
    $(selector).val(sid);

    selector = '[name="' + "YearId" + '"]';
    $(selector).val(yid).change();

}

function updateModel() {
    $("#divMessage1").text('All the financial cost data for this company will be reset as you have selected to update industry/sub-industry. Do you want to continue?');
    $('.btnNoUnit').addClass("btnNoIndustry");
    $('.btnYesUnit').addClass("btnYesIndustry");
    $("#Modal1").modal();
}

function clearControls() {
    $("#OptionalFunctional").find('input[id$="Cost"]').val('');
    $('#ddlCurrency').val('');
    $('#ddlFiscalYear').val('');
    $('#ddlUnitOfMeasurement').val('Millions');
    $('#txtRevenue').val('');
    $('#txtEBITDA').val('');
    $('#txtExpense').val('');
    $('#txtGrossProfit').val('');
    $('#txtTotalEmployees').val('');
    $('#ddlCurrency').val('USD');
    //$('#reqCompanyLevelData').children('section[class="row mb-2 pb-2"]:nth-of-type(3)').find('input').val('');
}

    // $('#ddlSubIndustry').change(function () {
    //    updateModel();
    // })
//Click on No Button
function btnNoClickIndustry() {
    $('#ddlIndustry').val(originalIndustry);
    $('#ddlSubIndustry').val(originalSubIndustry);
    //$('#ddlIndustry').val(modelIndustry);
    subIndustryList(originalIndustry);
    $('#btnNoUnit').removeClass("btnNoIndustry");
    $('#btnYesUnit').removeClass("btnYesIndustry");
    $('#ddlSubIndustry').val(modelSubIndustry);
    $("#divMessage1").text('Data under Cost fields will be considered as per updated unit. Do you want to continue?');
}

//Click on Yes Button
function btnYesClickIndustry() {
    originalIndustry = $('#ddlIndustry').val();
    originalSubIndustry = $('#ddlSubIndustry').val();
    $('#btnNoUnit').removeClass("btnNoIndustry");
    $('#btnYesUnit').removeClass("btnYesIndustry");
    clearControls();
    $("#divMessage1").text('Data under Cost fields will be considered as per updated unit. Do you want to continue?');
}

/*
    //btnNoUnit click event
    $('#btnNoUnit').click(function () {
        if (($('#btnNoUnit').attr('class')).indexOf('Industry') != -1) {
            btnNoClickIndustry();
        } else {
            $('#ddlUnitOfMeasurement').val(unit);
        }
    })

    //btnYesUnit click event
    $('#btnYesUnit').click(function () {
        if (($('#btnNoUnit').attr('class')).indexOf('Industry') != -1) {
            btnYesClickIndustry();
        } else {
            unit = $('#ddlUnitOfMeasurement').val();
        }

    });

*/