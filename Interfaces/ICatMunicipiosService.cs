﻿using GuanajuatoAdminUsuarios.Entity;
using GuanajuatoAdminUsuarios.Models;
using System.Collections.Generic;

namespace GuanajuatoAdminUsuarios.Interfaces
{
    public interface ICatMunicipiosService
    {
        List<CatMunicipiosModel> GetMunicipios();
        List<CatMunicipiosModel> GetMunicipiosPorEntidad(int entidadDDlValue);

        public CatMunicipiosModel GetMunicipioByID(int IdMunicipio);
        public int AgregarMunicipio(CatMunicipiosModel model);
        public int EditarMunicipio(CatMunicipiosModel model);
        public int obtenerIdPorNombre(string municipio);
    }
}
