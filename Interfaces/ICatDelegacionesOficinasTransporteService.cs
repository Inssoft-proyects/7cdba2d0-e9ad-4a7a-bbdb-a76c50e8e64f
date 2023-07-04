using GuanajuatoAdminUsuarios.Entity;
using GuanajuatoAdminUsuarios.Models;
using System.Collections.Generic;

namespace GuanajuatoAdminUsuarios.Interfaces
{
    public interface ICatDelegacionesOficinasTransporteService
    {
        List<CatDelegacionesOficinasTransporteModel> GetDelegacionesOficinas();

    }
}
