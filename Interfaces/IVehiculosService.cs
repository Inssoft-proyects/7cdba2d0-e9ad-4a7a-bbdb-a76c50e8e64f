using GuanajuatoAdminUsuarios.Models;
using GuanajuatoAdminUsuarios.RESTModels;
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
        int BuscarPorParametro(string Placa, string Serie, string Folio);
        VehiculoModel GetModles(VehiculoBusquedaModel model);
        public VehiculoModel GetVehiculoToAnexo(VehiculoBusquedaModel model);
        public int CreateVehiculo(VehiculoModel model);
        public int UpdateVehiculo(VehiculoModel model);

        public List<VehiculoModel> GetVehiculoPropietario(VehiculoBusquedaModel model);
    }
}
