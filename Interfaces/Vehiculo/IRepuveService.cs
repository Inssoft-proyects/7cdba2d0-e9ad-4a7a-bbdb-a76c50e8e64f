using GuanajuatoAdminUsuarios.RESTModels;
using System.Collections.Generic;

namespace GuanajuatoAdminUsuarios.Interfaces
{
    public interface IRepuveService
    {
        List<RepuveConsgralResponseModel> ConsultaGeneral(RepuveConsgralRequestModel model);
        List<RepuveConsRoboResponseModel> ConsultaRobo(RepuveConsgralRequestModel model);

	}
}
