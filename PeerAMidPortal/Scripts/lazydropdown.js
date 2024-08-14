function addOption(selectElement, value, text, isDefault, selected) {
    const o = document.createElement("option");
    o.innerHTML = text;
    o.value = value.toString();
    o.selected = isNullOrUndefined(selected) ? false : selected;
    selectElement.appendChild(o);
    return o;
}

// Returns them as a comma-delimited string
function getSelectedValuesFromMultiSelect(id) {
    const r = $(`#${id} option:selected`); //.map(function (a, item) { return item.value; });
    var s = "";
    for (let i = 0; i < r.length; ++i) {
        if (s.length > 0) s += ",";
        s += r[i].value;
    }
    //console.log(`getSelectedValuesFromMultiSelect: ${id} ${s}`);
    return s;
}

// Returns them as an array
function getSelectedValuesFromMultiSelectAsArray(id) {
    const r = $(`#${id} option:selected`); //.map(function (a, item) { return item.value; });
    var s = [];
    for (let i = 0; i < r.length; ++i) {
        s.push(r[i].value);
    }
    //console.log(`getSelectedValuesFromMultiSelect: ${id} ${s}`);
    return s;
}

// Returns them as a hash
function getSelectedValuesFromMultiSelectAsHash(id, func) {
    if (!func)
        func = identityFunction;
    const r = $(`#${id} option:selected`); //.map(function (a, item) { return item.value; });
    var s = {};
    for (let i = 0; i < r.length; ++i) {
        s[r] = func(r[i].value);
    }
    //console.log(`getSelectedValuesFromMultiSelect: ${id} ${s}`);
    return s;
}

function identityFunction(x) {
    return x;
}

function findMultiselectOption(idMultiSelect, optionText) {
    const top = document.getElementById(idMultiSelect);
    const sib = top.nextElementSibling;
    const firstKid = sib.firstElementChild;
    const secondKid = firstKid.nextElementSibling;

    let li = secondKid.firstElementChild;
    while (li) {
        const a = li.firstElementChild;
        const label = a.firstElementChild;
        if (label.textContent.trim() == optionText)
            return label;
        li = li.nextElementSibling;
    }

    return null;
}