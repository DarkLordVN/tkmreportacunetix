// SideNav Initialization
$(".button-collapse").sideNav();
//    var container = document.querySelector('.custom-scrollbar');
//    var ps = new PerfectScrollbar(container, {
//      wheelSpeed: 2,
//      wheelPropagation: true,
//      minScrollbarLength: 20
//});

// Tooltips Initialization
$(function () {
    $('[data-toggle="tooltip"]').tooltip()
})
// DataTable Văn bản liên thông đi
$(document).ready(function () {
    $(".select2").select2({
        placeholder: "",
        allowClear: true
    });

    //$(".datepicker").kendoDatePicker({
    //    //value: "",
    //    dateInput: true,
    //    format: "dd/MM/yyyy"
    //});
    //$(".datetimepicker").kendoDateTimePicker({
    //    //value: new Date(),
    //    dateInput: true,
    //    format: "dd/MM/yyyy HH:mm"
    //});

    $('.datepicker').datetimepicker({
        format: 'DD/MM/YYYY',
        icons: {
            time: 'fa fa-clock',
            date: 'fa fa-calendar',
            up: 'fa fa-chevron-up',
            down: 'fa fa-chevron-down',
            previous: 'fa fa-chevron-left',
            next: 'fa fa-chevron-right',
            today: 'fa fa-clock',
            clear: 'fa fa-trash',
            close: 'fa fa-remove'
        }
    });

    $('.datetimepicker').datetimepicker({
        format: 'DD/MM/YYYY HH:mm',
        icons: {
            time: 'fa fa-clock',
            date: 'fa fa-calendar',
            up: 'fa fa-chevron-up',
            down: 'fa fa-chevron-down',
            previous: 'fa fa-chevron-left',
            next: 'fa fa-chevron-right',
            today: 'fa fa-clock',
            clear: 'fa fa-trash',
            close: 'fa fa-remove'
        }
    });

    $(".datepicker").each(function () {
        if ($(this).val() == 'ngày / tháng / năm') {
            $(this).val("");
        }
    });
    $(".datepicker").focusout(function () {
        if ($(this).val() == 'ngày / tháng / năm') {
            $(this).val("");
        }
    });
    $(".datetimepicker").each(function () {
        if ($(this).val() == 'ngày / tháng / năm giờ:phút') {
            $(this).val("");
        }
    });
    $(".datetimepicker").focusout(function () {
        if ($(this).val() == 'ngày / tháng / năm giờ:phút') {
            $(this).val("");
        }
    });

    $("#success-alert").hide();
    $('.mdb-select').materialSelect();
    loadPaging();
    $('#dt-vblt-den').DataTable({
    	"searching": false, // false to disable search (or any other option)
    	"oLanguage": {
    		"sUrl": "/Content/vietnamese.json"
    	}
    });
    $('#dt-vblt-di').DataTable({
    	"searching": false, // false to disable search (or any other option)
    	"oLanguage": {
    		"sUrl": "/Content/vietnamese.json"
    	}
    });
    $('#dt-vblienquan').DataTable({
    	"searching": false, // false to disable search (or any other option)
    	"oLanguage": {
    		"sUrl": "/Content/vietnamese.json"
    	}
    });
    $('.dataTables_length').addClass('bs-select');
});

$("form").submit(function () {
    $('.number').each(function (index) {
        $(this).val($(this).val().replace(/[^0-9]/g, ''));
        //var val = $(this).maskMoney('unmasked')[0];
        //$(this).val(val.toString().replace('.', ''));
    });
});

function showAlert(str) {
    $("#resetPWModal").click();
    $("#success-alert").fadeTo(2000, 500).slideUp(500, function () {
        $("#lblMessage").html(str);
        $("#success-alert").slideUp(500);
    });
};

function InitClickCheckAllInTable(e) {
    var table = $(e.target).closest('table');
    $('td input:checkbox', table).prop('checked', this.checked);
};

// Tree view
function buildTreeView() {
    $('.tree li:has(ul)').addClass('parent_li').find(' > span').attr('title', 'Collapse this branch');
    $('.tree li.parent_li > span').on('click', function (e) {
        var children = $(this).parent('li.parent_li').find(' > ul > li');
        if (children.is(":visible")) {
            children.hide('fast');
            $(this).attr('title', 'Expand this branch').find(' > i').addClass('icon-plus-sign').removeClass('icon-minus-sign');
        } else {
            children.show('fast');
            $(this).attr('title', 'Collapse this branch').find(' > i').addClass('icon-minus-sign').removeClass('icon-plus-sign');
        }
        e.stopPropagation();
    });
}
