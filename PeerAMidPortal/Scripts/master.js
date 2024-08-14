// Author Abhay Mandal (Lead Engineer - The Smartcube)

$(function() {
    //login
    //if($("#back-btn").length){
    //	$("#back-btn").click(function(){
    //		$("#ForgotForm").hide()
    //		$("#form_login").show()
    //	})
    //	$("#forget-password").click(function(){
    //		$("#ForgotForm").show()
    //		$("#form_login").hide()
    //	})
    //}

    ////Slimscroll for Macro Indicator & Indices Performance
    //if($(".scroll1").length){
    //	$('.scroll1').mCustomScrollbar({
    //		theme:"dark",
    //		scrollbarPosition:"outside"
    //	});
    //}

    ////Slimscroll for News
    //if($(".scroll2").length){
    //	$('.scroll2').mCustomScrollbar({
    //		theme:"dark",
    //		scrollbarPosition:"outside"
    //	});
    //}

    ////Slimscroll for Business Description
    //if($("#BusDec").length){
    //	$('#BusDec').mCustomScrollbar({
    //		theme:"dark",
    //		scrollbarPosition:"outside"
    //	});
    //}

    ////Slimscroll for Credit Thesis
    //if($(".tableScroll").length){
    //	$('.tableScroll').mCustomScrollbar({
    //				axis:"yx",
    //				//scrollButtons:{enable:true},
    //				theme:"dark",
    //				scrollbarPosition:"outside"
    //			});
    //}

    ////Slimscroll for Big Tables
    //if($("#CreThe").length){
    //	$('#CreThe').mCustomScrollbar({
    //		theme:"dark",
    //		scrollbarPosition:"outside"
    //	});
    //}

    ////Mobile Menu
    //if($(".mMenuBtn").length){
    //	$(".mMenuBtn").click(function(){
    //		$(".whiteBase, .mobileMenu").addClass("viewM")
    //	})
    //	$(".closeMmenu, .mobileMenu").click(function(){
    //		$(".whiteBase, .mobileMenu").removeClass("viewM")
    //	})
    //}

    //// autocomplete
    //var options = {data: ["Nucor Corporation", "Abbott Laboratories", "Norsk Hydro ASA", "Volkswagen AG", "Anglo American PLC", "Bristol-Myers Squibb", "Merck & Co.", "Starbucks Corp", "Target Corporation Corporation", "Wal-mart Stores"]	};
    //$("#company").easyAutocomplete(options);

    ////tooltip
    //if($('[data-toggle="tooltip"]').length){
    //	$('[data-toggle="tooltip"]').tooltip()
    //}

    //// multiselect menw
    //if($(".mSelect").length){
    //	$('.mSelect').multiselect({
    //	  	includeSelectAllOption: true,
    //		 enableFiltering: true,
    //	  	buttonText: function(options, select) {
    //	  	    if (options.length === 0) {
    //	  	        return 'None selected';
    //	  	    } else if (options.prevObject.length == options.length){
    //	  	      return 'All selected';
    //	  	    } else if (options.length > 0) {
    //	  	        return options.length +  ' selected';
    //	  	   }
    //	  	}
    //	  });
    //}

    // multiselect menw 2 for display text above
    if ($(".mSelect2").length) {
        $(".mSelect2").multiselect({
            includeSelectAllOption: true,
            enableFiltering: true,
            numberDisplayed: 5
        });
    }

    //data table for company list
    // if($("#companyList").length){
    // 	$("#companyList").dataTable({
    // 		//"ordering": false,
    // 		"lengthChange": false,
    // 		"order": [[ 3, "desc" ]],
    // 		"info":     false,
    // 		"responsive": true,
    // 		"searching": false,
    // 		"fnDrawCallback": function( settings ) {
    // 			if($(".mSelect").length){
    // 				// enbling toggle again to avoid bubbling
    // 				$('body .dropdown-toggle').dropdown();
    // 			}
    // 		}
    // 	})
    // }

    //mobile filter show hide
    if ($(".mobileFilter").length) {
        $(".mobileFilter").click(function() {
            event.preventDefault();
            $(this).toggleClass("active");
            targetDiv = $(this).attr("href");
            $(targetDiv).slideToggle();
        });
    }

    //remove section
    if ($(".remove").length) {
        $(".remove").click(function() {
            $(this).closest(".whiteBox").closest("[class^=col-]").remove();
        });
    }

    //filters
    if ($(".filterMenu").length) {
        $(".filterOpen").click(function() {
            $(this).closest(".filterMenu").find(".filterBody").slideDown();
        });
        $(".filterClose").click(function() {
            $(this).closest(".filterBody").fadeOut();
        });
    }

    //Company Management Add Edit
    if ($("#ShowAddEdit").length) {
        $("#ShowAddEdit").click(function() {
            $("#OpenAddEdit").slideDown();
            $("#step1").show();
            $("#fetch").show();
        });

        $(".closeAddEdit").click(function() {
            $("#OpenAddEdit").slideUp();
            $("#step2").slideUp();
        });

        $("#fetch").click(function() {
            $("#step2").slideDown();
        });

        $("#Reset").click(function() {
            $("#step2").slideUp();
        });

        $(".edit").click(function() {
            $("#fetch").hide();
            $("#step2").show();
            $("#step1").show();
            $("#OpenAddEdit").slideDown();
        });
    }

    //end of ready function
})