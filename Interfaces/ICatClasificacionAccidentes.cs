using GuanajuatoAdminUsuarios.Models;
using System.Collections.Generic;

namespace GuanajuatoAdminUsuarios.Interfaces
{
    public interface ICatClasificacionAccidentes
    {
        List<CatClasificacionAccidentesModel> ObtenerClasificacionesActivas();
        
    }
}
