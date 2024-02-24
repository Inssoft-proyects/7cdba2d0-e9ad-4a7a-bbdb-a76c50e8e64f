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
        console.log(x.controlName, element.val())
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

/**
 * Valida controles si son requeridos y coloca
 * el foco en el primer control con error
 * @param {*} controlsValidate 
 * @returns 
 */
function isControlsValidWithFocus(controlsValidate, withFocus = true) {
    var isValid = true;
    var firstElementWithError;
    controlsValidate.forEach(x => {
        var element = $('#' + x.controlName);
        var validElement = true;
        var validators = x.validators ?? ['required'];
        //Si el campo es un input remueve el estilo de error
        if (x.isInput)
            element.removeClass("errorData");
        //Si el campo es tipo dropdown se remueve el estilo de error
        if (x.isDropDown)
            element.closest('.k-dropdown').removeClass("errorData");

        if (validators.includes('required')) {
            if (element.val() === '' || element.val() === undefined) {
                isValid = false;
                validElement = false;
            }
        }
        if (validators.includes('phoneValidator') && element.val() != '' && element.val() != undefined) {
            if (!isValidPhone(element.val())) {
                isValid = false;
                validElement = false;
            }
        }
        if (validators.includes('emailValidator') && element.val() != '' && element.val() != undefined) {
            if (!isValidEmail(element.val())) {
                isValid = false;
                validElement = false;

            }
        }
        if (!validElement) {
            //Se agrega el estilo de error en caso no se haya encontrado un valor
            if (x.isInput)
                element.addClass("errorData");
            if (x.isDropDown)
                element.closest('.k-dropdown').addClass("errorData");
            if (!firstElementWithError)
                firstElementWithError = x;
        }
    });

    //En caso de existir un componente requerido con error y se debe establecer el foco
    if (firstElementWithError && withFocus) {
        //Si el control es de tipo input
        if (firstElementWithError.isInput)
            $('#' + firstElementWithError.controlName).focus();
        //Si el control es de tipo dropdown se establece el foco y se expande la lista
        if (firstElementWithError.isDropDown)
            $('#' + firstElementWithError.controlName).focus().click();

    }

    return { isValid, firstElementWithError };
}

function addOnLostFocusRequiredControls(controlsValidate) {
    controlsValidate.forEach(x => {
        var element = $('#' + x.controlName);
        var validators = x.validators ?? ['required'];
        element.on('focusout', () => {
            var isValid = true;

            if (validators.includes('required')) {
                //Si el control no tiene valor se agrega el estilo de error
                if (element.val() === '' || element.val() === undefined) {
                    isValid = false;
                }
            }
            if (validators.includes('phoneValidator') && element.val() != '' && element.val() != undefined) {
                if (!isValidPhone(element.val())) {
                    isValid = false;
                }
            }
            if (validators.includes('emailValidator') && element.val() != '' && element.val() != undefined) {
                if (!isValidEmail(element.val())) {
                    isValid = false;
                }
            }
            if (!isValid) {
                if (x.isInput)
                    element.addClass("errorData");
                if (x.isDropDown)
                    element.closest('.k-dropdown').addClass("errorData");
            }
            else {
                //Si el campo es un input remueve el estilo de error
                if (x.isInput)
                    element.removeClass("errorData");
                //Si el campo es tipo dropdown se remueve el estilo de error
                if (x.isDropDown)
                    element.closest('.k-dropdown').removeClass("errorData");
            }
        })
    });
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

