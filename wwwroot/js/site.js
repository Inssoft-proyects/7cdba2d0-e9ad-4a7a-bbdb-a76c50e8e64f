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
        if (element.val() == '' || element.val() == undefined) {
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