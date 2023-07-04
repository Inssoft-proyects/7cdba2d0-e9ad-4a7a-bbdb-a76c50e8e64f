using GuanajuatoAdminUsuarios.Models;
using System.Collections.Generic;

namespace GuanajuatoAdminUsuarios.Interfaces
{
    public interface ICancelarInfraccionService
    {
        List<CancelarInfraccionModel> ObtenerInfraccionPorFolio(string FolioInfraccion);

        CancelarInfraccionModel ObtenerDetalleInfraccion(int Id);

        CancelarInfraccionModel CancelarInfraccionBD(int IdInfraccion, string OficioRevocacion);
        
    }
}
