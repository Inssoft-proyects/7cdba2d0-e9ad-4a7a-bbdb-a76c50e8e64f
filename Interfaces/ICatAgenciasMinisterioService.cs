using GuanajuatoAdminUsuarios.Models;
using System.Collections.Generic;

namespace GuanajuatoAdminUsuarios.Interfaces
{
    public interface ICatAgenciasMinisterioService
    {
        List<CatAgenciasMinisterioModel> ObtenerAgenciasActivas();
        List<CatAgenciasMinisterioModel> ObtenerAgenciasActivasPorDelegacion(int idOficina);

        

    }
}
