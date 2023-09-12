using GuanajuatoAdminUsuarios.Interfaces;

namespace GuanajuatoAdminUsuarios.Services
{
    public class SalidaVehiculosService : ISalidaVehiculosService
    {
        private readonly ISqlClientConnectionBD _sqlClientConnectionBD;
        public SalidaVehiculosService(ISqlClientConnectionBD sqlClientConnectionBD)
        {
            _sqlClientConnectionBD = sqlClientConnectionBD;
        }
    }
}
