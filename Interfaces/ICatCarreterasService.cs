using GuanajuatoAdminUsuarios.Models;
using System.Collections.Generic;

namespace GuanajuatoAdminUsuarios.Interfaces
{
    public interface ICatCarreterasService
    {

        List<CatCarreterasModel> ObtenerCarreteras();
        public CatCarreterasModel ObtenerCarreteraByID(int IdCarretera);
        public int CrearCarretera(CatCarreterasModel model);
       public int EditarCarretera(CatCarreterasModel model);




    }
}
