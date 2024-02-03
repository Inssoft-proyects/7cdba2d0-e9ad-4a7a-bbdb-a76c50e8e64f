using GuanajuatoAdminUsuarios.Models;
using System.Collections.Generic;

namespace GuanajuatoAdminUsuarios.Interfaces
{
    public interface IPensionesService
    {
        public List<PensionModel> GetAllPensiones(int idOficina);
        public List<PensionModel> GetPensionesByDelegacion(int delegacionDDValue);
        public List<PensionModel> GetPensionesToGrid(string strPension, int? idOficina);
        public List<PensionModel> GetPensionById(int idPension, int idOficina);
        public List<Gruas2Model> GetGruasDisponiblesByIdPension(int idPension,int idOficina);
        public int CrearPension(PensionModel model);
        public int CrearPensionGruas(int idPension, List<int> gruas);
        public int EliminarPensionGruas(int idPension);
        public int EditarGrua(PensionModel model);
		public string GetPensionLogin(int idPension);

	}
}
