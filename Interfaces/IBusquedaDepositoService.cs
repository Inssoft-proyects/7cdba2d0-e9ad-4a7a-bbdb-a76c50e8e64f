using GuanajuatoAdminUsuarios.Models;
using System.Collections.Generic;

namespace GuanajuatoAdminUsuarios.Interfaces
{
    public interface IBusquedaDepositoService
    {
        public List<BusquedaDepositoModel> ObtenerTodosDepositos();

        public List<BusquedaDepositoModel> ObtenerDepositos(BusquedaDepositoModel model);
        public BusquedaDepositoModel ObtenerDetalles(int idDeposito);

    }
}

