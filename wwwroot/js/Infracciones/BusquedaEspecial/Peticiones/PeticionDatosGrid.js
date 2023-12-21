
export function GetData(data, Succes, Fail) {
    //showLoading();
    $.ajax({
        url: './GetDataBusquedaEspecial',
        type: 'POST',
        data: data,
        success: Succes || function (result) {
            console.log(result);
            hideLoading();
        },
        error: Fail || function (result) {
            console.log(result);
            hideLoading();
        }
    });


}


export function RemoveData(data, Succes, Fail) {
    showLoading();
    $.ajax({
        url: './RemoveData',
        type: 'POST',
        data: data,
        success: Succes || function (result) {
            console.log(result);
            hideLoading();
        },
        error: Fail || function (result) {
            console.log(result);
            hideLoading();
        }
    });


}