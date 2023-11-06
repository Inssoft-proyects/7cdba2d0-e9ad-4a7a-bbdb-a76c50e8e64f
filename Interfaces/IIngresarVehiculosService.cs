using GuanajuatoAdminUsuarios.Models;
using System.Collections.Generic;

namespace GuanajuatoAdminUsuarios.Interfaces
{
    public interface IIngresarVehiculosService
    {
        public List<IngresoVehiculosModel> ObtenerDepositos(IngresoVehiculosModel model);
        // public int GuardarDeposito(int idDeposito);
        public IngresoVehiculosModel DetallesDeposito(int idDeposito);
        public int GuardarFechaIngreso(DatosIngresoModel model);

        
    }
}
