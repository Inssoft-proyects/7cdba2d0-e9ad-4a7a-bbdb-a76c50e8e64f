using GuanajuatoAdminUsuarios.Models;
using System.Collections.Generic;

namespace GuanajuatoAdminUsuarios.Interfaces
{
    public interface IPensionesService
    {
        public List<PensionModel> GetAllPensiones();
        public List<PensionModel> GetPensionesToGrid(string strPension, int? idDelegacion);
        public List<PensionModel> GetPensionById(int idPension);
        public List<Gruas2Model> GetGruasDisponiblesByIdPension(int idPension);
        public int CrearPension(PensionModel model);
        public int CrearPensionGruas(int idPension, List<int> gruas);
        public int EliminarPensionGruas(int idPension);
        public int EditarGrua(PensionModel model);
    }
}
