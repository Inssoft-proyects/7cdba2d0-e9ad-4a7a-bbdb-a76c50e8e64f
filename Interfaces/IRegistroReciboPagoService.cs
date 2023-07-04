using GuanajuatoAdminUsuarios.Entity;
using GuanajuatoAdminUsuarios.Models;
using System.Collections.Generic;

namespace GuanajuatoAdminUsuarios.Interfaces
{
    public interface IRegistroReciboPagoService
    {
        List<RegistroReciboPagoModel> ObtInfracciones(string FolioInfraccion);
        RegistroReciboPagoModel ObtenerDetallePorId(int Id);


        
    }
}
