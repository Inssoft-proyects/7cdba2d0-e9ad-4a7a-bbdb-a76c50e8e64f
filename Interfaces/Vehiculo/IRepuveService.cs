using GuanajuatoAdminUsuarios.RESTModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GuanajuatoAdminUsuarios.Interfaces
{
    public interface IRepuveService
    {
        List<RepuveConsgralResponseModel> ConsultaGeneral(RepuveConsgralRequestModel model);
        Task<List<RepuveRoboModel>> ConsultaRobo(RepuveConsgralRequestModel model);

	}
}
