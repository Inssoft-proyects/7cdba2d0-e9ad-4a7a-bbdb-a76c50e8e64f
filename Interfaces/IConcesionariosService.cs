using GuanajuatoAdminUsuarios.Models;
using System.Collections.Generic;

namespace GuanajuatoAdminUsuarios.Interfaces
{
    public interface IConcesionariosService
    {
        List<ConcesionariosModel> GetConcesionarios(int idOficina);
        public List<Concesionarios2Model> GetConcesionarios2ByIdDelegacion(int idDelegacion);
        public int CrearConcesionario(Concesionarios2Model model);
        public int EditarConcesionario(Concesionarios2Model model);
        public IEnumerable<Concesionarios2Model> GetAllConcesionarios(int idOficina);
        public Concesionarios2Model GetConcesionarioById(int idConcesionario);
    }
}
