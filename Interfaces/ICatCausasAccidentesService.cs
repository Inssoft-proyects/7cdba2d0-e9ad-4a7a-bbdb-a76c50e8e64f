using GuanajuatoAdminUsuarios.Models;
using System.Collections.Generic;

namespace GuanajuatoAdminUsuarios.Interfaces
{
    public interface ICatCausasAccidentesService
    {
        List<CatCausasAccidentesModel> ObtenerCausasActivas();
        public CatCausasAccidentesModel ObtenerCausaByID(int IdCausaAccidente);
        public int CrearCausa(CatCausasAccidentesModel model);
        public int EditarCausa(CatCausasAccidentesModel model);

    }
}
