using GuanajuatoAdminUsuarios.Models;
using System.Collections.Generic;

namespace GuanajuatoAdminUsuarios.Interfaces
{
    public interface IPersonasService
    {
        public IEnumerable<PersonaModel> GetAllPersonas();
        public PersonaDireccionModel GetPersonaDireccionByIdPersona(int idPersona);
        public PersonaModel GetPersonaById(int idPersona);
        IEnumerable<PersonaModel> GetAllPersonasMorales();
        IEnumerable<PersonaModel> GetAllPersonasMorales(PersonaMoralBusquedaModel model);
        int CreatePersonaMoral(PersonaModel model);
        int UpdatePersonaMoral(PersonaModel model);
        int CreatePersonaDireccion(PersonaDireccionModel model);
        int UpdatePersonaDireccion(PersonaDireccionModel model);
        PersonaModel GetPersonaTypeById(int idPersona);

        public int UpdatePersona(PersonaModel model);
        public int CreatePersona(PersonaModel model);
    }
}
