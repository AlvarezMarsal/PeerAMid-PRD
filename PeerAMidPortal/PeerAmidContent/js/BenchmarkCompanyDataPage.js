
$('#isOptionalDataReq').val('1');

// Save for data in database. Procedure name  Proc_SaveUpdateFunctionalData.
function savePartialDataAndNavigate(skipDeptLevelDataValidation) {
    // Basic field validation
    var result = validateData("formPartialDataByUser", "req");

// Departmental Data Validation - Perform if 'skip' flag is not set to True (PEER-2)
if (!skipDeptLevelDataValidation) {
        // Get Functional Department Level FTE data values
        var f1 = parseFloat($('#txtFinanceFTE').val());
var f2 = parseFloat($('#txtHumanResourceFTE').val());
var f3 = parseFloat($('#txtInfoTechFTE').val());
var f4 = parseFloat($('#txtProcurementFTE').val());
var f5 = parseFloat($('#txtSalesFTE').val());
var f6 = parseFloat($('#txtMarketingFTE').val());
var f7 = parseFloat($('#txtCustomerServicesFTE').val());
var f8 = parseFloat($('#txtCorpServicesFTE').val());
var totEmp = parseFloat($('#txtTotalNumOfEmployee').val());

        // Validate FTE Counts
        if ((f1 + f2 + f3 + f4 + f5 + f6 + f7 + f8) > totEmp) {
    result = false;
bAlert("Sum of functional FTEs should always be less than or equal to the Total Employees.");
        }
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

        // Validate Cost Totals
        if ((c1 + c2 + c3 + c4 + c5 + c6 + c7 + c8) > sgaC) {
    result = false;
bAlert("SG&A functional costs must be less than or equal to total SG&A cost.");
        }
    }

// Submit form data as long as there are no validation errors
if (result) {
    $('#isOptionalDataReq').val('1');
let formData = $('#formPartialDataByUser').serialize();
console.log("Calling ajax '/api/ActualDataCollection/SaveActualClientDataCollection'");
$.ajax({
    type: "Post",
url: '/api/ActualDataCollection/SaveActualClientDataCollection',
beforeSend: function() {
    $('.loader-div').show();
            },
complete: function() {
    $('.loader-div').hide();
            },
data: formData,
dataType: "json",
headers: config,
success: function(result) {
                if (result > 0) {
    bAlert("Company data updated successfully",
        function () {
            window.location.href = '/PeerAMid/CompanyAutoSelection';
        });
                } else {
    bAlert("Some error occurred");
                }
            },
error: function() { }
        });
    }

}

/*
*  Validate the specified Form field
*
*  formId - ID of the container that holds the form field
*  className - Name of field to validate
*
*/
function validateData(formId, className) {
    let flag = true;
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
