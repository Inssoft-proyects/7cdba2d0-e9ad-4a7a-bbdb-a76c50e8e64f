using GuanajuatoAdminUsuarios.Models;
using System.Collections.Generic;

namespace GuanajuatoAdminUsuarios.Interfaces
{
    public interface ICatClasificacionAccidentes
    {
        List<CatClasificacionAccidentesModel> GetClasificacionAccidentes();

        List<CatClasificacionAccidentesModel> ObtenerClasificacionesActivas();

        public CatClasificacionAccidentesModel GetClasificacionAccidenteByID(int IdClasificacionAccidente);
        public int CrearClasificacionAccidente(CatClasificacionAccidentesModel model);
        public int EditarClasificacionAccidente(CatClasificacionAccidentesModel model);

    }
}
