using GuanajuatoAdminUsuarios.Models;
using System.Collections.Generic;

namespace GuanajuatoAdminUsuarios.Interfaces
{
    public interface ICatMarcasVehiculosService
    {
        List<CatMarcasVehiculosModel> ObtenerMarcas();
        public CatMarcasVehiculosModel GetMarcaByID(int IdMarcaVehiculo);
       public int GuardarMarca(CatMarcasVehiculosModel model);
        public int UpdateMarca(CatMarcasVehiculosModel marca);

    }
}
