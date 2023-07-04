function _set_combo_catalog(catalog, parameter, htmlName, isRequired, container, functionName, multiple = "false") {

    $.ajax({
        type: "POST",
        url: '/GenericComponents/GetComboByCatalog',
        dataType: "html",
        data: {
            "htmlName": htmlName,
            "isRequired": isRequired,
            "catalog": catalog + ",",
            "parameter": parameter + ",",
            "functionName": functionName,
            "multiple": multiple
        },
        success: function (data) {
            $("#" + container).html(data);
            
        }
    });
}


function sitteg_warning(msg) {
    set_toastr_options(2000);
    toastr.warning(msg, 'Alerta');
}

function sitteg_info(msg) {
    set_toastr_options(2500);
    toastr.info(msg, "Información");
}

function sitteg_success(msg) {
    set_toastr_options(2500);
    toastr.success(msg, "Alerta");
}


function set_toastr_options(timeout) {
    toastr.options = {
        "closeButton": true,
        "debug": false,
        "progressBar": false,
        "positionClass": "toast-top-right",
        "onclick": null,
        "timeOut": timeout,
        "extendedTimeOut": "1000",
        "showEasing": "swing",
        "hideEasing": "linear",
        "showMethod": "fadeIn",
        "hideMethod": "fadeOut"
    };
}