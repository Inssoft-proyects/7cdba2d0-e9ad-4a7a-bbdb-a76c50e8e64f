using GuanajuatoAdminUsuarios.Models;
using System.Collections.Generic;

namespace GuanajuatoAdminUsuarios.Interfaces
{
    public interface ICatTramosService

    {

        List<CatTramosModel> ObtenerTamosPorCarretera(int carreteraDDValue);
        List<CatTramosModel> ObtenerTramos();

        public CatTramosModel ObtenerTramoByID(int IdTramo);
        public int CrearTramo(CatTramosModel model);
        public int EditarTramo(CatTramosModel model);

    }
}
