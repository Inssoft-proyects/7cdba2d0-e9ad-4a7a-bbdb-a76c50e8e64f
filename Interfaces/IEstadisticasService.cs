using GuanajuatoAdminUsuarios.Models;
using System.Collections.Generic;

namespace GuanajuatoAdminUsuarios.Interfaces
{
    public interface IEstadisticasService 
    {
        public List<InfraccionesModel> GetAllInfraccionesEstadisticas(IncidenciasBusquedaModel modelBusqueda);
    }
}
