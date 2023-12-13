using GuanajuatoAdminUsuarios.Models;
using System.Collections.Generic;

namespace GuanajuatoAdminUsuarios.Interfaces
{
    public interface ICatSubtipoServicio
    {
        List<CatSubtipoServicioModel> GetSubtipoPorTipo(int tipoServicioDDlValue);

    }
}
