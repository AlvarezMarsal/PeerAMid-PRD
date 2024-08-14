var validNumber = new RegExp(/^\d*\.?\d*$/);
var lastValid = "";
var validNegativeNumber = new RegExp(/^-?\d*\.?\d*$/);
var lastValidNegative = "";
var config = { /*'Authorization': "vOuzTZH04SDfWy0XQ49wSQ"*/ };
const tenToThe = [1, 10, 100, 1000, 10000, 100000, 1000000];

function AjaxCall(t, u, d, nu) {
    console.log(`Calling ajax: ${u}`);
    $.ajax({
        type: t,
        url: u,
        data: d,
        dataType: "json",
        success: function(r) {
            if (r == 1) {
                window.location.href = nu;
            }
        },
        error: function() {}
    });
}

function AjaxCallWithOutURL(t, u, d) {
    console.log(`Calling ajax: ${u}`);
    $.ajax({ type: t, url: u, data: d, dataType: "json", success: function(r) {}, error: function() {} });
}

function validateNumber(elem) {
    if (validNumber.test(elem.value)) {
        lastValid = elem.value;
    } else {
        elem.value = lastValid;
    }
}

function validateNegativeNumber(elem) {
    if (validNegativeNumber.test(elem.value)) {
        lastValidNegative = elem.value;
    } else {
        elem.value = lastValidNegative;
    }
}

function validateNavigate() {
    var flag = true;
    const controls = $(".req");
    if (controls.length > 0) {
        $.each(controls,
            function() {
                if (this.value == "") {
                    $(this).css("border-color", "red");
                    flag = false;
                } else {
                    $(this).css("border-color", "");
                }
            });
        return flag;
    }
}

$(".decimal").on("input", function() { validateNumber(this); });
$(".negativeDecimal").on("input", function() { validateNegativeNumber(this); });
$(".numeric").on("input", function(event) { this.value = this.value.replace(/[^0-9]/g, ""); });
$("input[type=text].req,select.req").blur(function (event) {
    (this.value == "") ? $(this).css("border-color", "red") : $(this).css("border-color", "");
}); /*validateNavigate();*/
$.ajaxSetup({ statusCode: { 401: function() { window.location.href = "/PeerAMid/Index" } } });

function bindFY(ddlyear, selectedYear, sY, lY) {
    const currentYear = sY;
    const lastFiveYear = currentYear - lY;
    for (let year = currentYear; year >= lastFiveYear; year--) {
        if (selectedYear == year) {
            ddlyear.append($(`<option value=${year} selected>${year}</option>`));
        } else {
            ddlyear.append($(`<option value=${year}>${year}</option>`));
        }
    }
}

function remove(array, element) {
    const index = array.indexOf(element);
    if (index !== -1) {
        array.splice(index, 1);
    }
}

function getallcontrolsInDatatable(dataTableId) {
    const oTable = $(`#${dataTableId}`).dataTable();
    const rowcollection = oTable.$(".call-checkbox:checked", { "page": "all" });
    var data = [];
    rowcollection.each(function(index, elem) { data.push($(elem).val()); });
    return data;
}

function numberWithCommas(x, places) {
    if (x === undefined)
        return "";
    if (x === null)
        return "";
    var negative = (x < 0);
    if (negative)
        x *= -1;

    var v;
    if ((places === undefined) || (places < 1)) {
        var s = Math.round(x).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
        v = (negative ? "-" : "") + s;
        // console.log("numberWithCommas(" + x + ") --> " + v);
        return v;
    }

    var f = Math.floor(x);
    var s = f.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
    var a = (x - s) * tenToThe[places];
    var b = a.toString();

    var i = b.indexOf('.');
    if (i < 0)
        v = (negative ? "-" : "") + s + "." + "0000000".substring(0, places);
    else
        v = (negative ? "-" : "") + s + b.substring(i);

    // console.log("numberWithCommas(" + x + "," + places + ") --> " + v);

    return v;
}

function fractionToPercentage(x, places) {
    if (x === undefined)
        return "0%";
    if (x === null)
        return "0%";
    var negative = (x < 0);
    if (negative)
        x *= -1;

    x = x * 100;

    var v;
    if ((places === undefined) || (places < 1)) {
        var s = Math.round(x).toString() + "%";
        v = (negative ? "-" : "") + s;
        console.log("fractionToPercentage(" + x + ") --> " + v);
        return v;
    }

    var s = x.toFixed(places);
    v = (negative ? "-" : "") + s + "%";

    console.log("fractionToPercentage(" + x + "," + places + ") --> " + v);

    return v;
}


/*
function bindCurrency(id, selected) {
    var currArr = ["USD", "EUR", "GBP"];
    $.each(currArr,
        function (index, value) {
            if (value == selected) {
                $("#" + id).append($("<option value=" + value + " selected>" + value + "</option>"));
            } else {
                $("#" + id).append($("<option value=" + value + ">" + value + "</option>"));
            }
        });
}
*/

var adWindowIsVisible = false;
var swirlyIsVisible = false;

/* Called when download is started or completed */
function downloadPPTLoader(isVisible) {
    swirlyIsVisible = isVisible;
    setOverlayState();
}

function showAdWindow(isVisible) {
    adWindowIsVisible = isVisible;
    setOverlayState();
}

function setOverlayState() {
    const loaderDiv = $(".loader-div");
    const holder = $(".holder");
    const availableDownloadsDiv = $(".available-downloads-div");

    if (swirlyIsVisible) {
        $(".loadText").html("Please wait while we are downloading....");
        loaderDiv.show();
        holder.show();
        loaderDiv.outerHeight("100vh");

        if (adWindowIsVisible) {
            availableDownloadsDiv.show();
        } else {
            availableDownloadsDiv.hide();
        }
    } else { // swirly hidden
        if (adWindowIsVisible) {
            loaderDiv.show();
            holder.hide();
            availableDownloadsDiv.show();
            loaderDiv.outerHeight(availableDownloadsDiv.outerHeight() + availableDownloadsDiv.position().top);
        } else {
            loaderDiv.hide();
        }
    }
}

var pleaseWaitTimer = null;

function pleaseWait(state) {
    if (state) {
        if (pleaseWaitTimer) // already waiting
            return;
        pleaseWaitTimer = setTimeout(() => { // wait a bit
            pleaseWaitTimer = null;
            const loaderDiv = $(".loader-div");
            const holder = $(".holder");
            $(".loadText").html("Please wait....");
            loaderDiv.show();
            holder.show();
            loaderDiv.outerHeight("100vh");
        }, 100);
    } else {
        if (pleaseWaitTimer) {
            clearTimeout(pleaseWaitTimer);
        } else {
            $(".loader-div").hide();
        }
    }
}

function toTitleCase(str) {
    return str;
}

function oldToTitleCase(str) {
    return str.replace(
        /\w\S*/g,
        function(txt) {
            return txt.charAt(0).toUpperCase() + txt.substr(1).toLowerCase();
        }
    );
}

function getCurrentFiscalYear() {
    //get current date
    const today = new Date();

    //get current month
    const curMonth = today.getMonth();

    var fiscalYr = "";
    if (curMonth > 3) { //
        const nextYr1 = (today.getFullYear() + 1).toString();
        fiscalYr = today.getFullYear().toString();
    } else {
        const nextYr2 = today.getFullYear().toString();
        fiscalYr = (today.getFullYear() - 1).toString();
    }

    return fiscalYr;
}

// Truncate space delimited string to contain the maximum amount of words,
// within the specified string length.
function truncateWordString(string, stringLength) {
    var text = "", prevText = "";
    const split = string.split(" ");
    const maxLength = (stringLength == null || isNaN(stringLength)) ? 30 : stringLength;

    var i = 0;
    prevText = text = split[i++];
    while (text.length < maxLength && i < split.length) {
        prevText = text;
        text = text + " " + split[i++].replace(",", "");
    }
    return text;
}

// Truncate space delimited string to contain the maximum amount of words,
// within the specified string length.

var uselessCompanyNameSuffixes = [
    "& CO",
    "CO",
    "COMPANY",
    "AG",
    ", INC.",
    ",INC.",
    "INC.",
    ", INC",
    ",INC",
    "INC",
    "CORP",
    "PLC",
    "S.A.",
    "S. A."
];

function truncateCompanyName(name, stringLength) {
    const n = uselessCompanyNameSuffixes.length;
    for (let i = 0; i < n; ++i)
        if (name.endsWith(uselessCompanyNameSuffixes[i]))
            name = name.substr(0, name.length - uselessCompanyNameSuffixes[i].length);
    return truncateWordString(name, stringLength);
}


function hashToString(hash) {
    var a = Object.keys(hash);
    if (a.length == 0)
        return "";
    var s = a[0].toString();
    for (var i=1; i<a.length; ++i)
        s += "," + a[i].toString();
    return s;
}

function commaDelimitedStringToHash(s) {
    var hash = {};
    var parts = s.split(',');
    for (var i=0; i<parts.length; ++i)
        hash[parts[i].trim()] = true;
    return hash;
}

const weekdays = ["Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday"];
const months = ["January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"];

function bAlert(message, okFunctionCallback, headerText, btnText, btnText2) {

    if (okFunctionCallback != null && okFunctionCallback != undefined) {

        $("#btnText").unbind('click');
        $("#btnText").click(function () {
            $('.modal').modal('hide');
            okFunctionCallback(1);
        });
        $("#btnText2").unbind('click');
        $("#btnText2").click(function () {
            $('.modal').modal('hide');
            okFunctionCallback(2);
        });
    }
    if ((headerText != undefined) && (headerText != null)) {
        $('#divHeaderMessage').html(headerText);
    } else {
        $('#divHeaderMessage').html('');
    }

    if (message != undefined) {
        $('#divMessage').html(message);
    }
    if (btnText != undefined) {
        $('#btnText').html(btnText);
    } else {
        $('#btnText').html("Ok");
    }
    if (btnText2 != undefined) {
        $('#btnText2').html(btnText2);
        $('#btnText2').show();
    } else {
        $('#btnText2').hide();
    }

    $('.modal').modal('show');
}

function confirmTermsAndConditions(callback) {
    doModalDialog({
        text: 'By clicking on the "Agree" button, you are agreeing to the <a href = "#"> Terms & Conditions</a>',
        ok : callback,
        okText : 'Agree',
        cancelText : 'Disagree',
        cancel : callback
    });
}

function doModalDialog(options) {

    var o = Object.assign({}, options);

    if (!o.hasOwnProperty('text') || (o.text == null)) {
        o.text = 'Proceed?';
    }

    if (!o.hasOwnProperty('okText') || (o.okText == null)) {
        o.okText = 'Ok';
    }

    if (!o.hasOwnProperty('cancelText') || (o.cancelText == null)) {
        o.cancelText = 'Cancel';
    }

    if (!o.hasOwnProperty('ok') || (o.ok == null))
        o.ok = nop;

    if (o.hasOwnProperty('cancel') && (o.cancel != null)) {
        o.allowCancel = true;
    } else {
        o.allowCancel = false;
        o.cancel = nop;
    }

    if (o.block) {
        var promise = new Promise(function (resolve, reject) {
            o.ok = function (r) { o.ok(r); resolve(r);  }
            o.cancel = function (r) { o.cancel(r); resolve(r); }
            _doModalDialogHelper(o);
        });
        return promise.then(function (result) { return result })
    } else {
        _doModalDialogHelper(o);
    }
}

function _doModalDialogHelper(options) {

    if (options.hasOwnProperty('title') && (options.title != null)) {
        $('#divHeaderMessage').show();
        $('#divHeaderMessage').html(options.title);
    } else {
        $('#divHeaderMessage').hide();
    }

    $('#divMessage').html(options.text);

    $('#btnText').html(options.okText);
    $("#btnText").unbind('click');
    $("#btnText").click(function () {
            doModalDialogHelperDone(options.okText);
            options.ok(options.okText);
        });

    if (options.allowCancel) {
        $('#btnText2').show();
        $('#btnText2').html(options.cancelText);
    } else {
        $('#btnText2').hide();
    }

    $("#btnText2").unbind('click');
    $("#btnText2").click(function () {
        doModalDialogHelperDone(options.cancelText);
        options.cancel(options.cancelText);
    });

    $('.modal').modal('show');
}

function doModalDialogHelperDone() { $('.modal').modal('hide'); }

function addRowToTable(tbody, data1, data2, data3, data4, data5, data6, data7, data8, data9, data10) {
    var tr = tbody.append($("<tr>"));
    if (data1 !== undefined) tr.append($("<td>").html(data1));
    if (data2 !== undefined) tr.append($("<td>").html(data2));
    if (data3 !== undefined) tr.append($("<td>").html(data3));
    if (data4 !== undefined) tr.append($("<td>").html(data4));
    if (data5 !== undefined) tr.append($("<td>").html(data5));
    if (data6 !== undefined) tr.append($("<td>").html(data6));
    if (data7 !== undefined) tr.append($("<td>").html(data7));
    if (data8 !== undefined) tr.append($("<td>").html(data8));
    if (data9 !== undefined) tr.append($("<td>").html(data9));
    if (data10 !== undefined) tr.append($("<td>").html(data10));
    return tr;
}