using GuanajuatoAdminUsuarios.Models;
using System.Collections.Generic;

namespace GuanajuatoAdminUsuarios.Interfaces
{
    public interface ICatEntidadesService
    {

        List<CatEntidadesModel> ObtenerEntidades();
        public CatEntidadesModel ObtenerEntidadesByID(int idEntidad);
        public int CrearEntidad(CatEntidadesModel model);
        public int EditarEntidad(CatEntidadesModel model);

        
    }
}
