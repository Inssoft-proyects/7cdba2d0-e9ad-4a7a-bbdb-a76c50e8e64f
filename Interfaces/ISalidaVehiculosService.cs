using GuanajuatoAdminUsuarios.Models;
using System.Collections.Generic;

using GuanajuatoAdminUsuarios.Models;
using System.Collections.Generic;

namespace GuanajuatoAdminUsuarios.Interfaces
{
    public interface ISalidaVehiculosService
    {
        public List<SalidaVehiculosModel> ObtenerIngresos(SalidaVehiculosModel model);
        public SalidaVehiculosModel DetallesDeposito(int iDp);
        List<GruasSalidaVehiculosModel> ObtenerDatosGridGruas(int iDp);
        public CostosServicioModel CostosServicio(int idDeposito);
        public int ActualizarCostos(CostosServicioModel model);
        public int GuardarInforSalida(SalidaVehiculosModel model);


    }
}
