using GuanajuatoAdminUsuarios.Models;
using System.Collections.Generic;

namespace GuanajuatoAdminUsuarios.Interfaces
{
    public interface ILiberacionVehiculoService
    {
        List<LiberacionVehiculoModel> GetAllTopDepositos();
        List<LiberacionVehiculoModel> GetDepositos(LiberacionVehiculoBusquedaModel model);
        LiberacionVehiculoModel GetDepositoByID(int Id);
        int UpdateDeposito(LiberacionVehiculoModel model);
    }
}
