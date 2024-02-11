using GuanajuatoAdminUsuarios.Entity;
using GuanajuatoAdminUsuarios.Models;
using GuanajuatoAdminUsuarios.RESTModels;
using System;
using System.Collections.Generic;

namespace GuanajuatoAdminUsuarios.Interfaces
{
    public interface IInfraccionesService
    {
        bool UpdateFolio(string id, string folio);
        public decimal GetUmas();
        public List<InfraccionesModel> GetAllInfracciones2();

        public List<EstadisticaInfraccionMotivosModel> GetAllEstadisticasInfracciones(int idOficina, int idDependencia);
        public List<EstadisticaInfraccionMotivosModel> GetAllMotivosPorInfraccion(int idOficina, int idDependencia);

        
        public List<InfoInfraccion> GetAllInfraccionesEstadisticasGrid(int idDependencia);
        List<InfraccionesModel> GetAllInfracciones(int idOficina, int idDependencia);
        public List<InfraccionesModel> GetAllInfraccionesByFolioInfraccion(string FolioInfraccion);
        public List<InfraccionesModel> GetAllInfraccionesByReciboPago(string ReciboPago);
        List<InfraccionesModel> GetAllInfracciones(InfraccionesBusquedaModel model,int idOficina, int idDependencia);
		List<InfraccionesModel> GetAllInfraccionesPagination(InfraccionesBusquedaModel model, int idOficina, int idDependencia, Pagination pagination);
		List<InfraccionesModel> GetAllInfraccionesBusquedaEspecial(InfraccionesBusquedaEspecialModel model, int idOficina, int idDependencia);
		List<InfraccionesModel> GetAllInfraccionesBusquedaEspecialPagination(InfraccionesBusquedaEspecialModel model, int idOficina, int idDependencia, Pagination pagination);

		InfraccionesModel GetInfraccionById(int IdInfraccion, int idDependencia);
        public InfraccionesReportModel GetInfraccionReportById(int IdInfraccion, int idDependencia);
        public List<MotivosInfraccionVistaModel> GetMotivosInfraccionByIdInfraccion(int idInfraccion);
        public GarantiaInfraccionModel GetGarantiaById(int idGarantia);
        public PersonaInfraccionModel GetPersonaInfraccionById(int idPersonaInfraccion);
        public int CrearPersonaInfraccion(int idInfraccion, int idPersona);
        public int CrearGarantiaInfraccion(GarantiaInfraccionModel model,int idInf);
        public int ModificarGarantiaInfraccion(GarantiaInfraccionModel model, int idInf);
        public int CrearMotivoInfraccion(MotivoInfraccionModel model);
        public int EliminarMotivoInfraccion(int idMotivoInfraccion);
        public InfraccionesModel GetInfraccion2ById(int idInfraccion, int idDependencia);

        public bool CancelTramite(string id);
        public int ActualizarEstatusCortesia(int idInfraccion,int  cortesiaInt);


        public NuevaInfraccionModel GetInfraccionAccidenteById(int idInfraccion, int idDependencia);
        
		public bool ValidarFolio(string folioInfraccion, int idDependencia);
        public int CrearInfraccion(InfraccionesModel model, int idDependencia);

		public int ModificarInfraccion(InfraccionesModel model);
        int ModificarInfraccionPorCortesia(InfraccionesModel model);

        public int InsertarImagenEnInfraccion( string rutaInventario, int idInfraccion);
        public List<InfraccionesResumen> GetInfraccionesLicencia(string numLicencia, string CURP);
        public List<InfraccionesModel> GetAllAccidentes2();

        public int  GuardarReponse(CrearMultasTransitoChild MT_CrearMultasTransito_res, int idInfraccion);
        public int ModificarEstatusInfraccion(int idInfraccion, int idEstatusInfraccion);

        public decimal getUMAValue();

    }
}
