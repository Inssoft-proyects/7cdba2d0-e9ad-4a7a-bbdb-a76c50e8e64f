using GuanajuatoAdminUsuarios.Models;
using System.Collections.Generic;

namespace GuanajuatoAdminUsuarios.Interfaces
{
    public interface IReporteAsignacionService
    {
        List<ReporteAsignacionModel> GetAllReporteAsignaciones();
        List<ReporteAsignacionModel> GetAllReporteAsignaciones(ReporteAsignacionBusquedaModel model);
    }
}
