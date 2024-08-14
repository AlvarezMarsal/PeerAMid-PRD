const globalStaticDataKey = 'PeerAMid-global';
var globalStaticData = null;
var logGsd = false;

function getGlobalStaticData() {

    if (!globalStaticData) {
        try {
            var json = localStorage.getItem(globalStaticDataKey);
            globalStaticData = JSON.parse(json);
            fixupGlobalStaticDataMethods();
            if (logGsd) console.log("Retrieved GSD from localStorage");
        } catch { }
    }

    if (globalStaticData) {
        var timeout = Date.now();
        if (timeout < globalStaticData.Timeout) {
            if (logGsd) console.log("Using GSD from localStorage");
            return globalStaticData;
        }
        if (logGsd) console.log("GSD from localStorage expired");
    }

    var promise = downloadGlobalStaticData();
    promise.then((result) => { });
    if (logGsd) console.log("Returning GSD to caller");
    return globalStaticData;
}

function downloadGlobalStaticData() {

    var version;
    if (globalStaticData)
        version = globalStaticData.CurrentVersion;
    else
        version = 0;

    var promise = new Promise(function (resolve, reject) {
        const uri = "/PeerAMid/GetGlobalStaticData";
        console.log(`Calling ajax: '${uri}'`);
        $.ajax({
            async: false,
            type: "GET",
            url: uri,
            data: { currentVersion : version },
            contentType: "application/json",
            success: function(result) {
                if (result.data.length > 0) {
                    if (logGsd) console.log("Got GSD from server");
                    globalStaticData = JSON.parse(result.data);
                    globalStaticData.Timeout = Date.now() + (10 * 60 * 1000);
                    fixupGlobalStaticData();
                    var json = JSON.stringify(globalStaticData);
                    localStorage.setItem(globalStaticDataKey, json);
                } else {
                    if (logGsd) console.log("Server ok'd GSD");
                    globalStaticData.Timeout = Date.now() + (10 * 60 * 1000);
                }
                resolve(globalStaticData);
            },
            error: function (_, _, err) {
                reject(err);
            }
        });
    });
    return promise;
}


function fixupGlobalStaticData() {
    var gsd = globalStaticData;

    gsd.IndustriesById = {};
    gsd.SubIndustriesById = {};

    for (var i = 0; i < gsd.Industries.length; ++i) {
        const ind = gsd.Industries[i];
        gsd.IndustriesById[ind.IndustryId] = ind;
        for (var j = 0; j < ind.SubIndustries.length; ++j) {
            const sub = ind.SubIndustries[j];
            gsd.SubIndustriesById[sub.SubIndustryId] = sub;
        }
    }

    const currencies = gsd.Currencies;
    gsd.Currencies = {};
    const sc = gsd.SelectedCurrency;
    gsd.SelectedCurrency = {};
    for (var i = 0; i < currencies.length; ++i) {
        const c = currencies[i];
        //console.log(JSON.stringify(c));
        const erby = c.ExchangeRateByYear;
        c.ExchangeRateByYear = {};
        for (var j = 0; j < erby.length; ++j)
            c.ExchangeRateByYear[erby[j].Year] = erby[j].ExchangeRate;
        //c.GetExchangeRate = function(y) { return ExchangeRate(y); };
        gsd.Currencies[c.Name] = c;
        if (c.Name == sc)
            gsd.SelectedCurrency = c;
    }
    //gsd.GetExchangeRate = function(c, y) { return gsd.Currencies[c].GetExchangeRate(y); };

    gsd.SelectedCurrency = gsd.Currencies[gsd.SelectedCurrency.Name];
    //console.log(`Selected currency is ${gsd.SelectedCurrency.Name}`);

    /*gsd.SelectedCurrency.FormatLargeValue = function(v, p) {
        return formatCurrency(v,
            (p === undefined) ? gsd.SelectedCurrency.LargeValueDecimalPlaces : p,
            gsd.SelectedCurrency.AmericanFormatting,
            gsd.SelectedCurrency.LargeValueFormat);
    };
    */
    /*gsd.SelectedCurrency.FormatSmallValue = function(v, p) {
        return formatCurrency(v,
            (p === undefined) ? gsd.SelectedCurrency.SmallValueDecimalPlaces : p,
            gsd.SelectedCurrency.AmericanFormatting,
            gsd.SelectedCurrency.SmallValueFormat);
    };
    */
   fixupGlobalStaticDataMethods();
}

function fixupGlobalStaticDataMethods() {
    var gsd = globalStaticData;
    const currencies = gsd.Currencies;
    for (var i = 0; i < currencies.length; ++i) {
        const c = currencies[i];
        //console.log(JSON.stringify(c));
        c.GetExchangeRate = function (y) { return ExchangeRate(y); };
    }
    gsd.GetExchangeRate = function (c, y) { return gsd.Currencies[c].GetExchangeRate(y); };
    gsd.SelectedCurrency.FormatLargeValue = function (v, p) {
        return formatCurrency(v,
            (p === undefined) ? gsd.SelectedCurrency.LargeValueDecimalPlaces : p,
            gsd.SelectedCurrency.AmericanFormatting,
            gsd.SelectedCurrency.LargeValueFormat);
    };
    gsd.SelectedCurrency.FormatSmallValue = function (v, p) {
        return formatCurrency(v,
            (p === undefined) ? gsd.SelectedCurrency.SmallValueDecimalPlaces : p,
            gsd.SelectedCurrency.AmericanFormatting,
            gsd.SelectedCurrency.SmallValueFormat);
    };
}
function getIndustry(id) {
    const gsd = getGlobalStaticData();
    var si = gsd.IndustriesById[id];
    if (si == undefined)
        return { SubIndustryId: id, Name: "Unknown" };
    return si;
}

function getSubIndustry(id) {
    const gsd = getGlobalStaticData();
    var si = gsd.SubIndustriesById[id];
    if (si === undefined)
        return { SubIndustryId: id, Name: "Unknown" };
    return si;
}

function formatCurrency(v, places, american, fmt) {
    var s = v.toFixed(places).toString();
    var left, right;
    if (places > 0) {
        const parts = s.split(".");
        left = parts[0];
        right = parts[1];
    } else {
        left = s;
        right = "";
    }
    left = left.replace(/\B(?=(\d{3})+(?!\d))/g, (american ? "," : "."));
    if (right == "")
        s = left;
    else
        s = left + (american ? "." : ",") + right;
    return fmt.replace("*", s);
}