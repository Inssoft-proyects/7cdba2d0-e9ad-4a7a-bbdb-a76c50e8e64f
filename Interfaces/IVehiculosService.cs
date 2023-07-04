using GuanajuatoAdminUsuarios.Models;
using System.Collections.Generic;

namespace GuanajuatoAdminUsuarios.Interfaces
{
    public interface IVehiculosService
    {
        public IEnumerable<VehiculoModel> GetAllVehiculos();
        List<VehiculoModel> GetVehiculos(VehiculoBusquedaModel modelSearch);
        public VehiculoModel GetVehiculoById(int idVehiculo);
        public VehiculoModel GetVehiculoToAnexo(VehiculoBusquedaModel model);
        public int CreateVehiculo(VehiculoModel model);
        public int UpdateVehiculo(VehiculoModel model);
    }
}
