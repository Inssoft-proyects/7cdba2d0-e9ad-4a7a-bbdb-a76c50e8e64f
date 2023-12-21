import { Loading } from '../Templates/Templates.js'
import { GetData, RemoveData } from '../Peticiones/PeticionDatosGrid.js'

export function GetDataGrid(t,callback) {
    GetData(t, callback)   
}

export function AddLoading(id) {


    $("#"+id).empty().append(Loading())

} 



export function RemoveTramite(d, callback) {
    RemoveData(d,callback)
}


