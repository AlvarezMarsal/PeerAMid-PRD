$(document).ready(function() {

    if ($("#tableSelectRow").length) {
        $("#tableSelectRow").DataTable({
            "searching": false,
            //"ordering": false,
            "info": false,
            "order": [[4, "desc"]],
            "columnDefs": [{ orderable: false, "targets": [0, 2, 5] }],
            //"paging":false,
            "language": {
                "emptyTable":
                    "There is no peer company in benchmark company revenue range. Please add peer companies on your own."
            }
        });

        $("#tableSelectRow tbody").on("click",
            "tr",
            function(event) {
                clickRow(this);
            });
    }
    if ($("#finalPeerTable").length) {
        $("#finalPeerTable").DataTable({
            "searching": false,
            "ordering": false,
            "info": false,
            "paging": false,
            //"order": [[4, 'desc']],
            //"columnDefs": [{ orderable: false, "targets": [0, 2, 5] }],
        });

        $("#finalPeerTable tbody").on("click",
            "tr",
            function(event) {
                clickRow(this);
            });
    }
    if ($("#finalUserSelectPeerTable").length) {
        $("#finalUserSelectPeerTable").DataTable({
            "searching": false,
            "ordering": false,
            "info": false,
            "paging": false,
            //"order": [[4, 'desc']],
            // "columnDefs": [{ orderable: false, "targets": [0, 2, 5] }],
        });

        $("#finalUserSelectPeerTable tbody").on("click",
            "tr",
            function(event) {
                const checkBox = $(this).find("input:checkbox");
                if (event.target.type !== "checkbox") {
                    if ($(checkBox).prop("checked") == true) {
                        $(checkBox).prop("checked", false);
                    } else {
                        $(checkBox).prop("checked", true);
                    }
                }
            });
    }

    function clickRow(tr) {
        const checkBox = $(tr).find("input:checkbox");
        if (event.target.type !== "checkbox") {
            if ($(checkBox).prop("checked") == true) {
                $(checkBox).prop("checked", false);
                $(tr).removeClass("selected");
            } else {
                $(checkBox).prop("checked", true);
                $(tr).addClass("selected");
            }
        } else {
            if ($(checkBox).prop("checked") == true) {
                $(tr).addClass("selected");
            } else {
                $(tr).removeClass("selected");
            }
        }
    }
    // Company/industry search autocomplete
    //var values =["Pellentesque Euismod per nascetur", "Dignissim nunc", "Varius felis", "Euismod per nascetur", "Pellentesque Euismod per nascetur", "Dignissim nunc", "Varius felis", "Euismod per nascetur", "Pellentesque Euismod per nascetur", "Dignissim nunc", "Industry 1", "Industry 2", "Industry 3", "Industry 4", "Industry 5", "Industry 6", "Industry 7", "Industry 8", "Industry 9", "Industry 10"]

    //var options = {
    //				data: values,
    //				list:{match: {enabled: true}}
    //			  };
    //if($("#CompanyIndustrySearch").length){
    //	$("#CompanyIndustrySearch").easyAutocomplete(options);
    //}

    //range slider
    if ($(".rangeSlider").length) {
        $(".rangeSlider").slider();
    }

    //Tooltip
    if ($('[data-toggle="tooltip"]').length) {
        $('[data-toggle="tooltip"]').tooltip();
    }

    //Optional data enable
    if ($(".TCheck").length) {
        var myContainerName;
        $(".TCheck").click(function() {
            myContainerName = $(this).closest("section").next().attr("id");
            if ($(this).is(":checked")) {
                $("#TandCconfirm").modal("show");
            } else {
                $(`#${myContainerName}`).collapse("hide");
            }
        });

        $("#TnCAgree").click(function() {
            //$('#OptionalFunctional').find('.decimal').val('');
            $(`#${myContainerName}`).collapse("show");
        });

        $("#TCdisagree").click(function() {
            // $('#OptionalFunctional').find('.decimal').val('');
            $(".TCheck").prop("checked", false);
        });
    }

    //page url test for Browsing and opening section according to previous selection
    const url = location.href.split("/").pop().replace(location.hash, "");
    const hash = window.location.hash;
});