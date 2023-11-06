using GuanajuatoAdminUsuarios.Models;
using System.Collections.Generic;

namespace GuanajuatoAdminUsuarios.Interfaces
{
    public interface IBusquedaEspecialAccidentesService
    {
        List<BusquedaEspecialAccidentesModel> BusquedaAccidentes(BusquedaEspecialAccidentesModel model, int idOficina);
        List<BusquedaAccidentesPDFModel> BusquedaAccidentes(BusquedaAccidentesPDFModel model, int idOficina);
        public BusquedaAccidentesPDFModel ObtenerAccidentePorId(int idAccidente);
        List<BusquedaEspecialAccidentesModel> ObtenerAccidentes(BusquedaEspecialAccidentesModel model);
        public int EliminarSeleccionado(int idAccidente);
        List<BusquedaEspecialAccidentesModel> ObtenerTodosAccidentes(int idOficina);
        
    }
}
