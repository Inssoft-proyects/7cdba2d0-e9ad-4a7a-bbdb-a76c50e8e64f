using GuanajuatoAdminUsuarios.Entity;
using GuanajuatoAdminUsuarios.Models;
using System.Collections.Generic;

namespace GuanajuatoAdminUsuarios.Interfaces
{
    public interface IDepositosService
    {
        string GuardarSolicitud(SolicitudDepositoModel model,int idOficina,string nombreOficina);
        SolicitudDepositoModel ObtenerSolicitudPorID(int Isol);
        int ActualizarSolicitud(int? Isol,SolicitudDepositoModel model);
        int CompletarSolicitud(SolicitudDepositoModel model);
        SolicitudDepositoModel ImportarInfraccion(string folioBusquedaInfraccion);
        List<SolicitudDepositoModel> ObtenerServicios();

    }
}
