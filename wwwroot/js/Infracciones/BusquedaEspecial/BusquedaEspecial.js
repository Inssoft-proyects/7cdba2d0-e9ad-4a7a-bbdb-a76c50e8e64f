import { AddLoading, GetDataGrid, RemoveTramite } from './Funcionality/ConexxionPeticiones.js'

$(document).ready(() => {

    var t = document.getElementById("frmSearch")
    t.method = "POST"
    t.addEventListener("submit", Submmit)




    AddLoading("listadoInfracciones")
    var Mydata = $("#frmSearch").serialize();
    GetDataGrid(Mydata, FinishGetData)
})


function FinishGetData(view) {
    $("#listadoInfracciones").empty().append(view)
}


const Submmit = (e) => {
    e.preventDefault()
    AddLoading("listadoInfracciones")
    var Mydata = $("#frmSearch").serialize();
    GetDataGrid(Mydata, FinishGetData)
}




window.DataRequestFilter = () => {
    var Mydata = $("#frmSearch").serializeArray();
    var obj = Mydata.reduce((acc, it) => { acc[it.name] = it.value; return acc }, {})
    console.log(obj)
    return obj

}



window.TemplateCortecia = (d) => {
    console.log(d)
    //infraccionCortesia
    console.log("entro")
    if (d.infraccionCortesia) {
        return `<button disabled onclick="ShowCortesia('idInfraccion')" class='w-100 btn'><h6 class='m-0 colorWarning'><i class='icon-edit me-2'></i><b> Cortesía</b></h6></button>`
    } else {
        return `<button disabled onclick="ShowCortesia('idInfraccion')" class='w-100 btn'><h6 class='m-0 colorWarning'><i class='icon-edit me-2'></i><b> Cortesía</b></h6></button>`
    }
}


window.TemplateEditar = (d) => {
    return `<button onclick="ShowUpdate('idInfraccion ')" class='w-100 btn'><h6 class='m-0 colorPrimary'><i class='icon-edit me-2'></i><b>Ver</b></h6></button>`
}


window.TemplateExportar = (d) => {
    return `<button onclick="CancelTramite(${d.idInfraccion})" class='w-100 btn'><h6 class='m-0 colorPrimary'><b>Eliminar</b></h6></button>`
}


window.TemplateMostrar = (d) => {
    console.log(d)
    console.log(d.idInfraccion)
    return `<form   action="Mostrar">
    <input type="text" value="${d.idInfraccion}" name="id" hidden class="d-none">
    <button  class='w-100 btn'><h6 class='m-0 colorPrimary'><i class='h5 icon-pdf me-2'></i><b>Ver</b></h6></button>
    </form>`
}


function finishCancel(d) {


    var grd = $("#GridInf").data("kendoGrid")
    grd.dataSource.read()

}

window.CancelTramite = (d) => {

    var data = { id: d }

    RemoveTramite(data, finishCancel)

}
