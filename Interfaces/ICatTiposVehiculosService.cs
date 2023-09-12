using Example.WebUI.Controllers;
using GuanajuatoAdminUsuarios.Entity;
using GuanajuatoAdminUsuarios.Models;
using System.Collections.Generic;

namespace GuanajuatoAdminUsuarios.Interfaces
{
    public interface ICatTiposVehiculosService
    {
        List<TiposVehiculosModel> GetTiposVehiculos();
        public int obtenerIdPorTipo(string categoria);

    }
}
