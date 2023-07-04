using GuanajuatoAdminUsuarios.Models;
using System.Collections.Generic;

namespace GuanajuatoAdminUsuarios.Interfaces
{
    public interface ICatFactoresAccidentesService
    {
        List<CatFactoresAccidentesModel> GetFactoresAccidentes();
        List<CatFactoresAccidentesModel> GetFactoresAccidentesActivos();

    }
}
