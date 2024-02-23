using GuanajuatoAdminUsuarios.Models;
using System.Collections.Generic;

namespace GuanajuatoAdminUsuarios.Interfaces
{
    public interface ICatAutoridadesEntregaService
    {
        List<CatAutoridadesEntregaModel> ObtenerAutoridadesActivas();
        CatAutoridadesEntregaModel GetAutoridadesByID(int IdAutoridadEntrega);
        public int GuardarAutoridad(CatAutoridadesEntregaModel autoridad);
        public int UpdateAutoridad(CatAutoridadesEntregaModel autoridad);

    }
}
