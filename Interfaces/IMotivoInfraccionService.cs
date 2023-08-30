using GuanajuatoAdminUsuarios.Models;
using System.Collections.Generic;

namespace GuanajuatoAdminUsuarios.Interfaces
{
    public interface IMotivoInfraccionService
    {

        public CatMotivosInfraccionModel GetMotivoByID(int IdCatMotivoInfraccion);
        public List<CatMotivosInfraccionModel> GetMotivos();
        public List<CatMotivosInfraccionModel> GetCatMotivos();
        public int CrearMotivo(CatMotivosInfraccionModel motivo);
        public int UpdateMotivo(CatMotivosInfraccionModel motivo);
        public int DeleteMotivo(CatMotivosInfraccionModel model);
    }
}
