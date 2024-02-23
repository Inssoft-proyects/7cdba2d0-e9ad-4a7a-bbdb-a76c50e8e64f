using GuanajuatoAdminUsuarios.Models;
using System.Collections.Generic;

namespace GuanajuatoAdminUsuarios.Interfaces
{
    public interface ICatAutoridadesDisposicionService
    {
        List<CatAutoridadesDisposicionModel> ObtenerAutoridadesActivas();
        CatAutoridadesDisposicionModel GetAutoridadesByID(int IdAutoridadDisposicion);
        public int GuardarAutoridad(CatAutoridadesDisposicionModel autoridad);
        public int UpdateAutoridad(CatAutoridadesDisposicionModel autoridad);

    }
}
