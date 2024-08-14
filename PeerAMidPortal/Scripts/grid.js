// Note: any method named 'render*' MUST return the element it created.

function Grid(id, options) {
    options = coalesce(options, { offsetWidth: 0 });

    this.id = id;
    this.div = document.getElementById(id);
    this.renderingSuspensionCount = 0;
    this.sync = {};
    this.suspendRendering();

    var forcedWidth = coalesce(options.width, 600);

    this.columns = [];
    this.columnSpacing = coalesce(options.columnSpacing, 5);
    this.rows = [];
    this.height = isNullOrUndefined(options.height) ? "100%" : (`${options.height}px`);
    this.nextRowNumber = 0;

    const div = this.div;
    div.style.boxSizing = "border-box";
    div.style.fontFamily = "Arial,Helvetica,sans-serif";
    div.style.fontWeight = "400";
    div.style.lineHeight = "18px";
    div.style.fontSize = "12px";
    //div.style.minWidth = "" + forcedWidth + "px";
    //div.style.maxWidth = "" + forcedWidth + "px";
    div.style.width = "100%";
    div.style.display = "inline-block";

    this.narrowWidth = "calc(100% - 17px)";
    this.outerElement = document.createElement("div");
    this.outerElement.style.display = "inline-block";
    this.outerElement.style.width = "100%";
    this.header = new GridHeader(this, this.narrowWidth);
    var tableWrapper = document.createElement("div");
    tableWrapper.style.maxHeight = options.height + "px";
    tableWrapper.style.overflowY = "auto";
    this.elementTable = document.createElement("table");
    this.elementTable.style.borderCollapse = "collapse";
    this.elementTable.style.tableLayout = "fixed";
    this.elementTable.width = this.outerElement.offsetWidth + "px";
    tableWrapper.appendChild(this.elementTable);

    this.body = new GridBody(this, "100%");
    this.elementTable.appendChild(this.body.element);

    this.outerElement.appendChild(this.header.outerElement);
    this.outerElement.appendChild(tableWrapper);

    this.resumeRendering();
}

// after calling this method, updates to the grid happen to a clone
// and do not go to the displayed DOM
Grid.prototype.suspendRendering = function() {
    this.renderingSuspensionCount = this.renderingSuspensionCount + 1;
};

// after calling this method, updates to the grid happen to a clone
// and do not go to the displayed DOM
Grid.prototype.resumeRendering = function() {
    if (this.renderingSuspensionCount < 1) {
        console.log("programming error");
    } else {
        this.renderingSuspensionCount = this.renderingSuspensionCount - 1;
        if (this.renderingSuspensionCount === 0) {
            this.div.clearChildren();
            this.div.appendChild(this.outerElement);
            this.synchronizeColumnWidths();
        }
    }
};

Grid.prototype._addColumn = function(type, title, weight, options) {
    this.suspendRendering();

    const column = new GridColumn(this, this.columns.length, type, title, weight, options);
    this.columns.push(column);

    this.header.outerElement.appendChild(column.outerElement);

    if (column.spacerAfter) {
        const spacer = this.createSpacer("span");
        spacer.style.backgroundColor = "rgb(52,58,64)";
        spacer.style.color = "rgb(52,58,64)";
        spacer.style.height = "100%";
        spacer.innerHTML = "&nbsp;";
        spacer.style.verticalAlign = "middle";
        this.header.outerElement.appendChild(spacer);
    }

    this.resumeRendering();
    return column;
};

Grid.prototype.addColumn = function(title, weight, options) {
    return this._addColumn(GridSupport.divTypes.readonly, title, weight, options);
};

Grid.prototype.addCheckboxColumn = function(title, weight, options) {
    options = options || {};
    options.justify = coalesce(options.justify, "center");
    options.fixedWidth = true;
    options.spacerAfter = coalesce(options.spacerAfter, false);
    return this._addColumn(GridSupport.divTypes.checkbox, title, weight, options);
};

Grid.prototype.addIndicatorColumn = function(options) {
    options = options || {};
    options.justify = coalesce(options.justify, "center");
    options.fixedWidth = true;
    options.spacerAfter = coalesce(options.spacerAfter, false);
    const col = this._addColumn(GridSupport.divTypes.spacer, "", 2, options);
    col.outerElement.innerHTML = "&nbsp;";
    return col;
};

Grid.prototype.synchronizeColumnWidths = function() {
    //if (!this.pendingSynchronizeColumnWidths) {
    //    this.pendingSynchronizeColumnWidths = setTimeout(() => { this.synchronizeColumnWidths(true); });
    //    return;
    //}

    //clearTimeout(this.pendingSynchronizeColumnWidths);
    //this.pendingSynchronizeColumnWidths = null;

    if ((this.sync.offsetWidth === this.outerElement.offsetWidth) && (this.sync.cols == this.columns.length))
        return;

    this.sync.offsetWidth = this.outerElement.offsetWidth;
    this.sync.cols = this.columns.length;

    // console.log(`Synchronizing column widths: ${this.id}`);
    this.calculateColumnWidths();

    if (this.rows.length > 0) {
        let tableRow = this.body.element.firstChild;
        for (let r = 0; r < this.rows.length; ++r) {
            const row = this.rows[r];
            for (let c = 0; c < this.columns.length; ++c) {
                const cell = row.cells[c];
                let tableCell = cell.outerElement;
                tableCell.style.width = this.columns[c].calculatedWidth + "px";
                tableCell.style.minWidth = this.columns[c].calculatedWidth + "px";
                tableCell.style.maxWidth = this.columns[c].calculatedWidth + "px";
                if (tableCell !== cell.innerElement) {
                    const inner = cell.innerElement;
                    inner.style.width = this.columns[c].calculatedWidth + "px";
                    inner.style.minWidth = this.columns[c].calculatedWidth + "px";
                    inner.style.maxWidth = this.columns[c].calculatedWidth + "px";
                }
                tableCell = tableCell.nextSibling.nextSibling;
            }
            tableRow = tableRow.nextSibling;
        }
    }
};

// constructs the grid object in html ***
Grid.prototype.calculateColumnWidths = function() {
    if (this.columns.length < 1)
        return;

    let totalSpacerWeight = 0;
    let totalFixedWeight = 0;
    let totalVariableWeight = 0;
    let variableColumns = [];

    for (let i = 0; i < this.columns.length; ++i) {
        var column = this.columns[i];
        if (column.weight > 0) {
            if (column.fixedWidth) {
                totalFixedWeight += column.weight;
                column.calculatedWidth = column.weight;
            } else {
                totalVariableWeight += column.weight;
                variableColumns.push(column);
            }

            if (this.columns[i].spacerAfter)
                totalSpacerWeight += this.columnSpacing;
        } else {
            column.calculatedWidth = 0;
        }
    }

    // console.log(`this.header.outerElement.offsetWidth = ${this.header.outerElement.offsetWidth}`);
    // console.log(`totalFixedWeight = ${totalFixedWeight}`);
    // console.log(`totalSpacerWeight = ${totalSpacerWeight}`);
    // console.log(`totalVariableWeight = ${totalVariableWeight}`);

    var w = 1.0;

    // allocate the variable widths
    if (totalVariableWeight > 0) {
        var violation = true;
        while (violation) {
            violation = false;
            var error = 0;

            var variableWeight = this.header.outerElement.offsetWidth - totalFixedWeight - totalSpacerWeight;
            w = variableWeight / totalVariableWeight;

            for (var j = 0; j < variableColumns.length; ++j) {
                var vc = variableColumns[j];
                if (vc !== null) {
                    var f = vc.weight;
                    var g = f * w;
                    f = Math.floor(g + error);
                    error = g - f;
                    vc.calculatedWidth = f;
                }
            }

            for (var j = 0; j < variableColumns.length; ++j) {
                var vc = variableColumns[j];
                if ((vc !== null) && (vc.calculatedWidth >= vc.maxWidth)) {
                    vc.calculatedWidth = vc.maxWidth;
                    violation = true;
                    totalFixedWeight += vc.calculatedWidth;
                    totalVariableWeight -= vc.weight;
                    variableColumns[j] = null;
                }
            }
        }
    }

    //assign the fixed widths
    for (let i = 0; i < this.columns.length; ++i) {
        var column = this.columns[i];
        var wpx;
        if (column.weight > 0) {
            wpx = column.calculatedWidth + "px";
        } else {
            wpx = "0";
        }

        // console.log("calculated width " + i + " " + wpx)
        column.outerElement.style.width = wpx;
        column.outerElement.style.minWidth = wpx;
        column.outerElement.style.maxWidth = wpx;
    }

    //console.log("tcw=" + totalColumnWidth + " tw=" + totalWidth);
    //this.header.outerElement.width = totalWidth + "px";
    //this.elementTable.parentElement.parentElement.parentElement.width = totalWidth + "px";
    //this.elementTable.parentElement.parentElement.width = totalWidth + "px";
    //this.elementTable.parentElement.width = totalWidth + "px";
};

Grid.prototype.addRow = function(args) {
    if ((arguments.length == 1) && Array.isArray(args))
        return this.addRowFromArray(args);
    const values = Array.from(arguments);
    return this.addRowFromArray(values);
};

Grid.prototype.addRowFromArray = function(array) {
    this.suspendRendering();
    const row = new GridRow(this, this.narrowWidth);
    row.setValuesFromArray(array);
    this.body.element.appendChild(row.element);
    this.rows.push(row);
    this.resumeRendering();
    return row;
};

Grid.prototype.setRowValues = function(row, array) { // 'contents' IS used, via 'arguments'
    let rowDiv;
    if (typeof (row) == "number") {
        rowDiv = this.body.getElementById(this.id + "-row-" + index);
    } else {
        rowDiv = row.element;
    }
    GridSupport.setRowElementValues(rowDiv, array);
};

Grid.prototype.clearRows = function() {
    this.body.element.clearChildren();
    this.nextRowNumber = 0;
    this.rows = [];
    return this;
};

Grid.prototype.handleCheckboxClick = function(checkbox) {
    // this.rows[row].cells[col].value = checkbox.isChecked; // no need
};

// This is where the user should override row-click handling
Grid.prototype.handleRowClick = function(cellDiv) {
    const rowDiv = cellDiv.parentNode;
    let child = rowDiv.firstChild;
    while (child) {
        const type = GridSupport.getCellType(child);
        if (type === GridSupport.divTypes.checkbox) {
            const value = GridSupport.getCellValue(child);
            GridSupport.setCellValue(child, !value);
            break;
        }
        child = child.nextSibling;
    }
};

Grid.prototype.removeRow = function(rowIndex) {
    this.suspendRendering();
    const div = this.rows[rowIndex].element;
    this.rows.splice(rowIndex, 1);
    if (div)
        div.parentNode.removeChild(div);
    this.resumeRendering();
};

Grid.prototype.createSpacer = function(type) {
    if (!type) type = "td";
    var spacer = document.createElement("span");
    GridSupport.setCellType(spacer, GridSupport.divTypes.spacer);
    spacer.style.display = "inline-block";
    spacer.style.width = `${this.columnSpacing}px`;
    spacer.style.minWidth = `${this.columnSpacing}px`;
    spacer.style.maxWidth = `${this.columnSpacing}px`;
    //spacer.style.overflow = "hidden";
    spacer.style.minHeight = "28px";
    spacer.style.position = "relative";
    //spacer.style.top = "10px";
    spacer.innerHTML = "&nbsp;";

    if (type != "span") {
        const s = document.createElement(type);
        s.appendChild(spacer);
        if (type === "td") {
            spacer.style.whiteSpace = "nowrap";
            s.style.width = "0px";
            s.style.whiteSpace = "nowrap";
            s.style.overflow = "hidden";
        }
        spacer = s;
    }
    return spacer;
};

Grid.prototype.dump = function(title) {
    // if (title) console.log(title);
    const n = this.rows.length;
    // console.log(`rows: ${n}`);
    for (let i = 0; i < n; ++i) {
        const values = this.rows[i].getValues();
        const m = values.length;
        let s = "";
        for (let j = 0; j < m; ++j)
            s = s + values[j] + ",";
        // console.log(s);
    }
};

///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

const GridSupport = {
    divTypes: {
        checkbox: "cb",
        readonly: "ro",
        spacer: "sp"
    },

    setCellType: function(outerElement, type) {
        outerElement.setAttribute("data-grid-type", type);
    },

    getCellType: function(outerElement) {
        return outerElement.getAttribute("data-grid-type");
    },

    removeAllChildren: function(element) {
        while (element.firstChild)
            element.removeChild(element.firstChild);
    },

    setCellElementValue: function(outerElement, value, type) {
        if (type === undefined)
            type = GridSupport.getCellType(outerElement);
        if (type === GridSupport.divTypes.readonly)
            outerElement.firstChild.innerHTML = isNullOrUndefined(value) ? "" : value.toString();
        else if (type === GridSupport.divTypes.checkbox)
            outerElement.firstChild.checked = (value === true);
        else if (type === GridSupport.divTypes.spacer); // do nothing
        else
            console.log("wtf");
    },

    getCellElementValue: function(outerElement, type) {
        if (type === undefined)
            type = GridSupport.getCellType(outerElement);
        if (type === GridSupport.divTypes.readonly)
            return outerElement.firstChild.innerText;
        if (type === GridSupport.divTypes.checkbox)
            return outerElement.firstChild.checked;
        if (type === GridSupport.divTypes.spacer)
            return null;
        console.log("wtf");
        return null;
    },

    setRowValues: function(rowElement, values) {
        let child = rowElement.firstChild;
        let index = 0;
        while (child) {
            const type = GridSupport.getCellType(child);
            if (type && (type !== GridSupport.divTypes.spacer)) {
                GridSupport.setCellElementValue(child, values[index++], type);
            }
            child = child.nextSibling;
        }
    },

    getRowValues: function(rowElement) {
        const values = [];
        let child = rowElement.firstChild;
        while (child) {
            const type = GridSupport.getCellType(child);
            if (type && (type !== GridSupport.divTypes.spacer)) {
                const value = GridSupport.getCellElementValue(child, type);
                values.push(value);
            }
            child = child.nextSibling;
        }
        return values;
    }
};

///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

function GridHeader(grid, width) {
    this.grid = grid;
    this.outerElement = document.createElement("span"); //GridSupport.createRow(width);
    this.outerElement.id = grid.id + "-header";
    this.outerElement.style.fontWeight = "400";
    this.outerElement.style.backgroundColor = "rgb(52,58,64)";
    this.outerElement.style.color = "rgb(52,58,64)";
    this.outerElement.style.width = width;
    this.outerElement.style.minWidth = width;
    this.outerElement.style.maxWidth = width;
    this.outerElement.style.height = "40px";
    this.outerElement.style.display = "inline-block";
    this.outerElement.style.overflow = "hidden";
};

///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

function GridColumn(grid, index, type, title, weight, options) {
    options = coalesce(options, {});

    options.justifyTitle = coalesce(options.justifyTitle, options.justify, "left");
    options.justifyValue = coalesce(options.justifyValue, options.justify, "left");

    if (weight === 0)
        options.spacerAfter = false;
    else
        options.spacerAfter = coalesce(options.spacerAfter, true);

    this.grid = grid;
    this.index = index;
    this.type = isNullOrUndefined(type) ? GridSupport.divTypes.readonly : type;
    this.title = isNullOrUndefined(title) ? index.toString() : title;
    this.weight = weight;
    this.maxWidth = coalesce(options.maxWidth, 5000);
    this.spacerAfter = options.spacerAfter;

    this.justifyTitle = options.justifyTitle;
    this.justifyValue = options.justifyValue;

    this.tip = options.tip;
    this.fixedWidth = isNullOrUndefined(options.fixedWidth) ? true : options.fixedWidth;

    this.outerElement = document.createElement("span");
    this.outerElement.style.display = "inline-block";
    this.outerElement.style.textAlign = this.justifyTitle;
    this.outerElement.style.fontWeight = "400";
    this.outerElement.style.backgroundColor = "rgb(52,58,64)";
    this.outerElement.style.color = "rgb(255,255,255)";
    this.outerElement.style.whiteSpace = "normal";
    this.outerElement.innerHTML = this.title;
    this.outerElement.hidden = (weight == 0);
    this.outerElement.style.height = "100%";
    this.outerElement.style.verticalAlign = "middle";
    if (this.tip)
        this.outerElement.title = tip;
    this.innerElement = this.outerElement;
}

///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

function GridBody(grid, width) {
    this.grid = grid;
    this.width = width;
    this.element = document.createElement("tbody");
}

///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

function GridRow(grid, width) {
    this.grid = grid;
    this.index = grid.nextRowNumber++;
    this.id = grid.id + "-row-" + this.index;
    this.cells = [];

    this.element = document.createElement("tr");
    this.element.id = this.id;
    this.element.style.height = "30px";
    this.element.style.borderBottom = "thin dotted #F8F8FF"; // ghost white
    this.element.setAttribute("data-grid-row", this.index.toString());

    for (let j = 0; j < grid.columns.length; ++j) {
        const cell = new GridCell(this, j);
        this.cells.push(cell);
        this.element.appendChild(cell.outerElement);
        if (grid.columns[j].spacerAfter) {
            const spacer = grid.createSpacer("td");
            this.element.appendChild(spacer);
        }
    }

    const slop = document.createElement("td");
    this.element.appendChild(slop);
}

GridRow.prototype.setValuesFromArray = function(values) {
    const grid = this.grid;
    const columns = grid.columns;
    const v = values.length;
    let i = 0;
    let j = 0;

    grid.suspendRendering();

    for (/**/; (j < v) && (i < columns.length); ++i) {
        if (columns[i].type != GridSupport.divTypes.spacer) {
            this.cells[i].setValue(values[j++]);
        }
    }
    for (/**/; i < columns.length; ++i) { // the values not provided
        if (columns[i].type != GridSupport.divTypes.spacer)
            this.cells[i].setValue(null);
    }

    grid.resumeRendering();
};

GridRow.prototype.setValuesFromArgumentsObject = function(argumentsObject) {
    const grid = this.grid;
    const columns = grid.columns;
    const v = argumentsObject.length;
    let i = 0;
    let j = 0;

    grid.suspendRendering();

    for (/**/; (j < v) && (i < columns.length); ++i) {
        if (columns[i].type != GridSupport.divTypes.spacer) {
            this.cells[i].setValue(argumentsObject[j++]);
            if (j == v)
                break;
        }
    }
    for (/**/; i < columns.length; ++i) { // the values not provided
        if (columns[i].type != GridSupport.divTypes.spacer)
            this.cells[i].setValue(null);
    }
    grid.resumeRendering();
};

GridRow.prototype.getValues = function() {
    return GridSupport.getRowValues(this.element);
};

///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

// Every cell has an outer element and an inner element.  The outer element is the immediate
// child of the GridRow it is in; the inner element has the content.
// It is not impossible for them to be the same element, but they might not be.
// There is no requirement that the inner element be a child of the outer element.

function GridCell(row, index) {
    const grid = row.grid;
    const column = grid.columns[index];

    this.grid = grid;
    this.column = column;
    this.row = row;
    this.index = index;
    this.value = "";
    this.id = row.id + "-col-" + index;

    this.outerElement = document.createElement("td");
    GridSupport.setCellType(this.outerElement, column.type);

    this.outerElement.style.width = "1px";
    this.outerElement.style.textAlign = column.justifyValue;
    var wpx = column.calculatedWidth + "px";
    this.outerElement.style.width = wpx;
    this.outerElement.style.minWidth = wpx;
    this.outerElement.style.maxWidth = wpx;

    if (column.type === GridSupport.divTypes.checkbox) {
        const checkbox = document.createElement("input");
        this.innerElement = checkbox;
        this.innerElement.style.textAlign = "center";

        checkbox.type = "checkbox";
        checkbox.id = this.id + "-checkbox";
        checkbox.checked = (this.value !== false);
        checkbox.setAttribute("data-row", this.row.index);
        checkbox.setAttribute("data-column", this.column.index);
        checkbox.onclick = function(e) {
            setTimeout(function() {
                grid.handleCheckboxClick(e.target);
            });
        };

        this.outerElement.appendChild(checkbox);
    } else if (column.type == GridSupport.divTypes.spacer) {
        this.innerElement = this.outerElement;
        this.innerElement.style.minHeight = "28px";
        this.innerElement.style.position = "relative";
        this.innerElement.style.top = "0px";
    } else {
        const span = document.createElement("span");
        this.innerElement = span;

        span.style.display = "inline-block";
        span.style.width = "100%";
        span.style.overflow = "hidden";
        span.style.textOverflow = "ellipsis";
        //span.style.paddingTop = "5px";
        //span.style.paddingBottom = "5px";
        span.style.textAlign = column.justifyValue;
        span.setAttribute("data-row", row.index);
        span.setAttribute("data-column", column.index);
        span.innerHTML = this.value;
        span.style.whiteSpace = "nowrap";
        span.onclick = function(e) {
            setTimeout(function() {
                grid.handleRowClick(e.target);
            });
        };

        span.style.width = wpx;
        span.style.minWidth = wpx;
        span.style.maxWidth = wpx;

        this.outerElement.appendChild(span);
    }

    this.outerElement.hidden = (column.weight == 0);
}

GridCell.prototype.setValue = function(newValue) {
    GridSupport.setCellElementValue(this.outerElement, newValue);
};

GridCell.prototype.getValue = function() {
    return GridSupport.getCellElementValue(this.outerElement);
};