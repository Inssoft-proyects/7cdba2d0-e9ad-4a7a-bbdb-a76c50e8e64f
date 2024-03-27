using GuanajuatoAdminUsuarios.Entity;
using GuanajuatoAdminUsuarios.Models;
using System.Collections.Generic;

namespace GuanajuatoAdminUsuarios.Interfaces
{
    public interface ICatMunicipiosService
    {
        List<CatMunicipiosModel> GetMunicipios();
        List<CatMunicipiosModel> GetMunicipiosCatalogo();

        List<CatMunicipiosModel> GetMunicipios2();
        List<CatMunicipiosModel> GetMunicipiosPorEntidad(int entidadDDlValue); 

        List<CatMunicipiosModel> GetMunicipiosPorDelegacion(int idOficina);


		List<CatMunicipiosModel> GetMunicipiosPorDelegacion2(int idOficina);


		List<CatMunicipiosModel> GetMunicipiosGuanajuato();

        public CatMunicipiosModel GetMunicipioByID(int IdMunicipio);
        public int AgregarMunicipio(CatMunicipiosModel model);
        public int EditarMunicipio(CatMunicipiosModel model);
        public int obtenerIdPorNombre(string municipio);
    }
}
