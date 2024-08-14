var sessionData = null;

function getSessionData() {

    if (sessionData) {
        var now = Date.now();
        if (sessionData.timeout > now)
            return sessionData;
    }

    var promise = _downloadSessionData();
    promise.then(function (result) { },
                 function (reject) { });
    return sessionData;
}

function _downloadSessionData() {
    var promise = new Promise(function (resolve, reject) {
        const uri = "/PeerAMid/GetSessionData";
        console.log(`Calling ajax: '${uri}'`);
        $.ajax({
            async: false,
            type: "GET",
            url: uri,
            data: {},
            contentType: "application/json",
            success: function (result) {
                sessionData = result;
                var now = Date.now();
                sessionData.timeout = new Date(now + 10 * 60 * 1000);
                resolve(sessionData);
            },
            error: function (_, _, err) {
                reject(err)
            }
        });
    });
    return promise;
}


function uploadSessionData() {
    const uri = "/PeerAMid/SetSessionVariables";
    //console.log("Calling ajax: '" + uri + "'");
    const s1 = JSON.stringify(sessionData.PeerSearchResults);
    //console.log(s1);
    const s2 = JSON.stringify(sessionData.SelectedPeerCompanies);
    //console.log(s2);
    AjaxCallWithOutURL("post",
        uri,
        { searchText1: s1, searchText2: s2, type: 16, searchAdditional: window.location.href });
    return sessionData;
}


function setMultiSelectOptions(multiSelectElement, selectedItems) {
    // debugger;
    multiSelectElement.val(selectedItems);
    multiSelectElement.multiselect("refresh");
}

