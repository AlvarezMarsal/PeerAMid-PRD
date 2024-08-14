var kendoCompanyGrid =
{
    getSchema: function (isCompanyModel, selection) {
        const f = {
            IsSelected: { type: "boolean" },
            CompanyName: { type: "string", editable: false },
            SubIndustryId: { type: "string", editable: false },
            Ticker: { type: "string", editable: false },
            Country: { type: "string", editable: false },
            Revenue: { type: "number", editable: false },
            FinancialYear: { type: "number", editable: false },
            CompanyId: { type: "string", editable: false },
            IsSuggested: { type: "boolean", editable: false }
        };

        if (isCompanyModel) {
            f.CompanyName = f.Name;
            f.CompanyId = f.Id;
            f.Name = undefined;
            f.Id = undefined;
        }

        const schema = {
            model: {
                fields: f
            } // model
        }; // schema

        return schema;
    },

    getColumns: function () {
        const columns = [
            {
                field: "IsSelected",
                template: '<input type="radio" #= IsSelected ? checked="checked" : "" # />',
                width: "23px",
                minResizableWidth: "23px",
                maxResizableWidth: "23px",
                title: "Selected",
                attributes: { style: 'text-align: center' },
                headerAttributes: { style: 'text-align: center; justify-content: center' },
                sortable: { allowUnsort: false }
            },
            {
                field: "CompanyName",
                title: "Company Name",
                width: "85px",
                sortable: { allowUnsort: false }
            },
            {
                //field: "SubIndustry",
                title: "SubIndustry",
                width: "115px",
                template: "#=getSubIndustry(SubIndustryId).SubIndustryName#",
                sortable: { allowUnsort: false }
            },
            {
                field: "Ticker",
                title: "Ticker",
                width: "52px",
                maxResizableWidth: "52px",
                sortable: { allowUnsort: false }
            },
            {
                field: "Country",
                title: "Country",
                width: "48px",
                maxResizableWidth: "48px",
                sortable: { allowUnsort: false }
            },
            {
                field: "Revenue",
                title: "Revenue <small>($USD MM)</small>",
                width: "47px",
                maxResizableWidth: "47px",
                attributes: { style: 'text-align: right' },
                headerAttributes: { style: 'text-align: right; justify-content: right' },
                format: "{0:c0}",
                sortable: { allowUnsort: false }
            },
            {
                field: "FinancialYear",
                title: "FY",
                width: "20px",
                maxResizableWidth: "20px",
                attributes: { style: 'text-align: right' },
                headerAttributes: { style: 'text-align: right; justify-content: right' },
                sortable: { allowUnsort: false }
            },
            { field: "CompanyId", hidden: true },
            { field: "IsSuggested", hidden: true }
        ];

        return columns;
    },

    defaultDataBoundHandler: function (e) {
        // make headerAttributes work
        e.sender.element.find(".k-grid-header").find("thead th").each(function (i, el) {
            $(el).find(".k-cell-inner").css("display", "inline-flex");
        });

    }, // databound

    attachCheckboxClickHandler: function (gridObject, tag, func) {
        gridObject.on("click",
            tag,
            function (e) {

                //console.log("clicked", e.ctrlKey, e.altKey, e.shiftKey);
                const p = { grid: gridObject.data("kendoGrid") };
                p.tr = $(this).closest("tr");
                p.dataItem = p.grid.dataItem(p.tr);
                if (tag == "td") {
                    var td = $(this)[0];
                    p.input = td.firstElementChild;
                } else {
                    p.input = $(this)[0];
                }
                if (p.input == null) {
                    //console.log("p.input == null");
                } else {
                    if (p.input.type == 'radio')
                        p.checked = p.input.value === 'on';
                    else
                        p.checked = p.input.checked;
                    const wasChecked = p.checked;

                    func(p);

                    if (p.input.type == 'radio')
                        p.input.value = p.checked ? 'on' : 'off';
                    else
                        p.input.checked = p.checked;
                }
            });
    },

    // Updates a single row in a kendo grid without firing a databound event.
    // This is needed since otherwise the entire grid will be redrawn.
    drawRow: function (grid, row) {
        const dataItem = grid.dataItem(row);
        if (dataItem) {
            const rowChildren = $(row).children('td[role="gridcell"]');
            for (let i = 0; i < grid.columns.length; i++) {
                kendoCompanyGrid._drawCell(grid, i, dataItem, rowChildren);
            }
        }
    },

    drawCell: function (grid, row, col) {
        const dataItem = grid.dataItem(row);
        if (dataItem) {
            const rowChildren = $(row).children('td[role="gridcell"]');
            kendoCompanyGrid._drawCell(grid, col, dataItem, rowChildren);
        }
    },

    _drawCell: function (grid, col, dataItem, rowChildren) {

        const column = grid.columns[col];
        const template = column.template;
        const cell = rowChildren.eq(col);

        if (template !== undefined) {
            const kendoTemplate = kendo.template(template);

            // Render using template
            cell.html(kendoTemplate(dataItem));
        } else {
            const fieldValue = dataItem[column.field];

            const format = column.format;
            const values = column.values;

            if (values !== undefined && values != null) {
                // use the text value mappings (for enums)
                for (let j = 0; j < values.length; j++) {
                    const value = values[j];
                    if (value.value == fieldValue) {
                        cell.html(value.text);
                        break;
                    }
                }
            } else if (format !== undefined) {
                // use the format
                cell.html(kendo.format(format, fieldValue));
            } else {
                // Just dump the plain old value
                cell.html(fieldValue);
            }
        }
    }

    /*
    kendo.ui.Grid.fn.expandToFit = function() {
        var $gridHeaderTable = this.thead.closest('table');
        var gridDataWidth = $gridHeaderTable.width();
        var gridWrapperWidth = $gridHeaderTable.closest('.k-grid-headerwrap').innerWidth();
        // Since this is called after column auto-fit, reducing size
        // of columns would overflow data.
        if (gridDataWidth >= gridWrapperWidth) {
            return;
        }
        var $headerCols = $gridHeaderTable.find('colgroup > col');
        var $tableCols = this.table.find('colgroup > col');
        var sizeFactor = (gridWrapperWidth / gridDataWidth);
        $headerCols.add($tableCols).not('.k-group-col').each(function () {
            var currentWidth = $(this).width();
            var newWidth = (currentWidth * sizeFactor);
            $(this).css({
                width: newWidth
            });
        });
    }//expandToFit
    */

};