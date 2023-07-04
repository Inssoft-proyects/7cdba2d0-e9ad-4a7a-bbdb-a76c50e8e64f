using GuanajuatoAdminUsuarios.Models;
using System.Collections.Generic;

namespace GuanajuatoAdminUsuarios.Interfaces
{
    public interface IPlacaServices
    {
        List<PlacaModel> GetPlacasByDelegacionId(int DelegacionId);
    }
}
