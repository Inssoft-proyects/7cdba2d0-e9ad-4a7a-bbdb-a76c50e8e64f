using GuanajuatoAdminUsuarios.Entity;
using GuanajuatoAdminUsuarios.Models;
using System.Collections.Generic;

namespace GuanajuatoAdminUsuarios.Interfaces
{
    public interface ICatDelegacionesOficinasTransporteService
    {
        List<CatDelegacionesOficinasTransporteModel> GetDelegacionesOficinas();
        List<CatDelegacionesOficinasTransporteModel> GetDelegacionesOficinasActivos();

       public string GetDelegacionOficinaById(int idOficina);
        public int EditarDelegacion(CatDelegacionesOficinasTransporteModel model);



    }
}
