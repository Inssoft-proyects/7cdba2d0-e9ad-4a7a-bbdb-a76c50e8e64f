using GuanajuatoAdminUsuarios.Models;
using System.Collections.Generic;

namespace GuanajuatoAdminUsuarios.Interfaces
{
    public interface IEstadisticasAccidentesService
    {
        public List<InfraccionesModel> GetAllInfracciones2();
        public List<BusquedaAccidentesModel> ObtenerAccidentes();
        public List<ListadoAccidentesPorAccidenteModel> AccidentesPorAccidente();
        public List<ListadoAccidentesPorVehiculoModel> AccidentesPorVehiculo();
    }
}
