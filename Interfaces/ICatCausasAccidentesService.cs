using GuanajuatoAdminUsuarios.Models;
using System.Collections.Generic;

namespace GuanajuatoAdminUsuarios.Interfaces
{
    public interface ICatCausasAccidentesService
    {
        List<CatCausasAccidentesModel> ObtenerCausasActivas();

    }
}
