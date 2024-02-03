// Write your Javascript code.
function hideLoading() {
    $('#loading').css('display', 'none');
}

function showLoading() {
    $('#loading').css('display', 'block');
}

var isValidPhone = function (phone) {
    var regex = /^[0-9]+$/;
    return regex.test(phone);
};

var isValidEmail = function (email) {
    var regex = /^\w+([.-_+]?\w+)*\w+([.-]?\w+)*(\.\w{2,10})+$/;
    return regex.test(email);
};

function isControlsValid(controlsValidate) {
    var isValid = true;
    var isFirst = false;

    controlsValidate.forEach(x => {
        var element = $('#' + x.controlName);
        element.removeClass("errorData");
        if (element.val() === '' || element.val() === undefined) {
            element.addClass("errorData");
            if (!isFirst) {
                //element.focus();
            }
            isValid = false;
            isFirst = true;
        }
    });

    return isValid;
}

function isControlsValidDropDown(controlsValidate) {
    var isValid = true;
    var isFirst = false;

    controlsValidate.forEach(x => {
        var element = $('#' + x.controlName);
        element.closest('.k-dropdown').removeClass("errorData");
        if (element.val() === '' || element.val() === undefined) {
            element.closest('.k-dropdown').addClass("errorData");
            if (!isFirst) {
                //element.focus();
            }
            isValid = false;
            isFirst = true;
        }
    });

    return isValid;
}


$(document).ready(function () {
   
    $(".navbar-nav li").on("click", function () {
        var dataId = $(this).attr("data-id");
        localStorage.setItem("menuId", dataId);
        //alert("Id del men�: " + dataId);
    });

});


function errorMessage(option, message) {
    switch (option) {
        case 1:
            toastr.error(messages.serverError);
            break;
        case 2:
            toastr.error(messages.formError);
            break;
        case 3:
            toastr.error(message);
            break;
        default:
            toastr.error(message ? message : messages.formError);
            break;
    }
}

