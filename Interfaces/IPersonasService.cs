using GuanajuatoAdminUsuarios.Models;
using System;
using System.Collections.Generic;

namespace GuanajuatoAdminUsuarios.Interfaces
{
    public interface IPersonasService
    {
        public IEnumerable<PersonaModel> GetAllPersonas();
        List<PersonaModel> BusquedaPersona(PersonaModel model);
        List<PersonaModel> BusquedaPersonaPagination(BusquedaPersonaModel model, Pagination pagination);
        public bool VerificarLicenciaSitteg(string numeroLicencia);
        public int InsertarDesdeServicio(LicenciaPersonaDatos personaDatos);
        public PersonaDireccionModel GetPersonaDireccionByIdPersona(int idPersona);
        public PersonaModel GetPersonaById(int idPersona);
        public PersonaInfraccionModel GetPersonaInfraccionById(int idPersona);
        
        List<PersonaModel> ObterPersonaPorIDList(int idPersona);

        IEnumerable<PersonaModel> GetAllPersonasMorales();
        IEnumerable<PersonaModel> GetAllPersonasFisicas();
        IEnumerable<PersonaModel> GetAllPersonasFisicasPagination(Pagination pagination);

        IEnumerable<PersonaModel> GetAllPersonasMorales(PersonaMoralBusquedaModel model);
        int CreatePersonaMoral(PersonaModel model);
        int UpdatePersonaMoral(PersonaModel model);
        int CreatePersonaDireccion(PersonaDireccionModel model);
        int UpdatePersonaDireccion(PersonaDireccionModel model);
        int UpdateConductores(Object model);
        PersonaModel GetPersonaTypeById(int idPersona);
        public PersonaModel BuscarPersonaSoloLicencia(string numeroLicencia);
        public int UpdatePersona(PersonaModel model);
        public int CreatePersona(PersonaModel model);

        public IEnumerable<PersonaModel> GetAllPersonasPagination(Pagination pagination);

        List<PersonaModel> GetPersonas();
        int UpdateConductor(PersonaModel model);

        List<PersonaModel> BuscarPersonasWithPagination(BusquedaPersonaModel model, Pagination pagination);

        public int ExistePersona(string licencia, string curp);

        public int InsertarPersonaDeLicencias(PersonaLicenciaModel personaDatos);
    }
}
