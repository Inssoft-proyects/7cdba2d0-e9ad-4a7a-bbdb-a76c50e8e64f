using GuanajuatoAdminUsuarios.Models;
using System.Collections.Generic;

namespace GuanajuatoAdminUsuarios.Interfaces
{
    public interface ICatSubmarcasVehiculosService
    {
        List<CatSubmarcasVehiculosModel> ObtenerSubarcas();
        public CatSubmarcasVehiculosModel GetSubmarcaByID(int IdSubmarca);
        public int GuardarSubmarca(CatSubmarcasVehiculosModel model);
        public int UpdateSubmarca(CatSubmarcasVehiculosModel model);
    }
}
