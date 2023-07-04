using GuanajuatoAdminUsuarios.Models;
using System.Collections.Generic;

namespace GuanajuatoAdminUsuarios.Interfaces
{
    public interface ICatFactoresOpcionesAccidentesService
    {
        List<CatFactoresOpcionesAccidentesModel> ObtenerOpcionesPorFactor(int factorDDValue);

    }
}
