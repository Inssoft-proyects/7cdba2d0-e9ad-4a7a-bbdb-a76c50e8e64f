using GuanajuatoAdminUsuarios.Models;
using System.Collections.Generic;

namespace GuanajuatoAdminUsuarios.Interfaces
{
    public interface IPadronDepositosGruasService
    {

        List<PadronDepositosGruasModel> GetAllPadronDepositosGruas();
        List<PadronDepositosGruasModel> GetPadronDepositosGruas(PadronDepositosGruasBusquedaModel model);
        List<PensionModel> GetPensiones();
    }
}
