using GuanajuatoAdminUsuarios.Models;
using System.Collections.Generic;

namespace GuanajuatoAdminUsuarios.Interfaces
{
    public interface ICatFactoresAccidentesService
    {
        List<CatFactoresAccidentesModel> GetFactoresAccidentes();
        List<CatFactoresAccidentesModel> GetFactoresAccidentesActivos();
        CatFactoresAccidentesModel GetFactorByID(int IdFactorAccidente);
        public int GuardarFactor(CatFactoresAccidentesModel factor);
        public int UpdateFactor(CatFactoresAccidentesModel factor);

    }
}
