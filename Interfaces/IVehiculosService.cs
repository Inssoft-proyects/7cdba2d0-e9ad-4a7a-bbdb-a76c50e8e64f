using GuanajuatoAdminUsuarios.Models;
using System.Collections.Generic;

namespace GuanajuatoAdminUsuarios.Interfaces
{
    public interface IVehiculosService
    {
        public IEnumerable<VehiculoModel> GetAllVehiculos();
        public IEnumerable<VehiculoModel> GetAllVehiculosPagination(Pagination pagination);
        List<VehiculoModel> GetVehiculos(VehiculoBusquedaModel modelSearch);
        List<VehiculoModel> GetVehiculosPagination(VehiculoBusquedaModel modelSearch, Pagination pagination);
        public VehiculoModel GetVehiculoById(int idVehiculo);
        public VehiculoModel GetVehiculoToAnexo(VehiculoBusquedaModel model);
        public int CreateVehiculo(VehiculoModel model);
        public int UpdateVehiculo(VehiculoModel model);
    }
}
