using GuanajuatoAdminUsuarios.Models;
using System.Collections.Generic;

namespace GuanajuatoAdminUsuarios.Interfaces
{
    public interface IInfraccionesService
    {
        public decimal GetUmas();
        public List<InfraccionesModel> GetAllInfracciones2();
        List<InfraccionesModel> GetAllInfracciones();
        List<InfraccionesModel> GetAllInfracciones(InfraccionesBusquedaModel model);
        InfraccionesModel GetInfraccionById(int IdInfraccion);
        public List<MotivoInfraccionModel> GetMotivosInfraccionByIdInfraccion(int idInfraccion);
        public GarantiaInfraccionModel GetGarantiaById(int idGarantia);
        public PersonaInfraccionModel GetPersonaInfraccionById(int idPersonaInfraccion);
        public int CrearPersonaInfraccion(int idPersona);
        public int CrearGarantiaInfraccion(GarantiaInfraccionModel model);
        public int ModificarGarantiaInfraccion(GarantiaInfraccionModel model);
        public int CrearMotivoInfraccion(MotivoInfraccionModel model);
        public int EliminarMotivoInfraccion(int idMotivoInfraccion);
        public InfraccionesModel GetInfraccion2ById(int idInfraccion);
        public int CrearInfraccion(InfraccionesModel model);
        public int ModificarInfraccion(InfraccionesModel model);
        int ModificarInfraccionPorCortesia(InfraccionesModel model);


    }
}
