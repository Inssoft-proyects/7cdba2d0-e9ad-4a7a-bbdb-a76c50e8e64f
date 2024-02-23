using GuanajuatoAdminUsuarios.Entity;
using GuanajuatoAdminUsuarios.Models;
using System;
using System.Collections.Generic;

namespace GuanajuatoAdminUsuarios.Interfaces
{
    public interface ICapturaAccidentesService
    {
         List<CapturaAccidentesModel> ObtenerAccidentes(int idOficina);
        List<CapturaAccidentesModel> ObtenerAccidentesPagination(int idOficina, Pagination pagination);

        public int GuardarParte1(CapturaAccidentesModel model, int idOficina,string nombreOficina="NRA");
        public CapturaAccidentesModel ObtenerAccidentePorId(int idAccidente, int idOficina);

        List<CapturaAccidentesModel> BuscarPorParametro(string Placa, string Serie, string Folio);


        public int ActualizarConVehiculo(int IdVehiculo, int idAccidente,int IdPersona, string Placa, string Serie);
        public int BorrarVehiculoAccidente(int idAccidente, int idVehiculo);
        public int InsertarConductor(int IdVehiculo, int idAccidente, int IdPersona);
        public int ActualizaInfoAccidente(int idAccidente, DateTime Fecha, TimeSpan Hora, int IdMunicipio, int IdCarretera, int IdTramo, int Kilometro);

		public int AgregarValorClasificacion(int IdClasificacionAccidente, int idAccidente);

        List<CapturaAccidentesModel> ObtenerDatosGrid(int idAccidente);

        public int ClasificacionEliminar(int idAccidente, int IdClasificacionAccidente);

        List<CapturaAccidentesModel> AccidentePorID(int idAccidente);

        public int AgregarValorFactorYOpcion(int IdFactorAccidente,int IdFactorOpcionAccidente,int idAccidente);
        int EditarFactorOpcion(int IdFactorAccidente, int IdFactorOpcionAccidente, int IdAccidenteFactorOpcion);
        List<CapturaAccidentesModel> ObtenerDatosGridFactor(int idAccidente);
        public int AgregarValorCausa(int IdCausaAccidente,int idAccidente);
        public void ActualizaIndiceCuasa(int idAccidenteCausa, int indice);
        public void RecalcularIndex(int IdAccidente);
        public int EditarValorCausa(int IdCausaAccidente,int idAccidenteCausa);

        List<CapturaAccidentesModel> ObtenerDatosGridCausa(int idAccidente);
        public int EliminarValorFactorYOpcion(int IdAccidenteFactorOpcion);
        public int EliminarRegistroInfraccion(int IdInfraccion);
        public int EliminarCausaBD(int IdCausaAccidente,int idAccidente, int idAccidenteCausa);
        public int GuardarDescripcion(int idAccidente,string descripcionCausa);
        List<CapturaAccidentesModel> BusquedaPersonaInvolucrada(BusquedaInvolucradoModel model, string server= null);

        public int AgregarPersonaInvolucrada(int idPersonaInvolucrado, int idAccidente);
        List<CapturaAccidentesModel> VehiculosInvolucrados(int IdAccidente);
        CapturaAccidentesModel InvolucradoSeleccionado(int idAccidente,int IdVehiculoInvolucrado,int IdPropietarioInvolucrado);
        CapturaAccidentesModel ObtenerConductorPorId(int IdPersona);
        public int GuardarComplementoVehiculo(CapturaAccidentesModel model,int IdVehiculo,int idAccidente);
        int AgregarMontoV(MontoModel model);
        List<CapturaAccidentesModel> InfraccionesVehiculosAccidete(int idAccidente);
        public int RelacionAccidenteInfraccion(int IdVehiculo,int idAccidente,int IdInfraccion);
        List<CapturaAccidentesModel> InfraccionesDeAccidente(int idAccidente);
       public int RelacionPersonaVehiculo(int IdPersona,int idAccidente,int IdVehiculoInvolucrado);
        public int ActualizarInvolucrado(CapturaAccidentesModel model,int idAccidente);
        List<CapturaAccidentesModel> InvolucradosAccidente(int idAccidente);
        int AgregarFechaHoraIngreso(FechaHoraIngresoModel model,int idAccidente);
        int GuardarDatosPrevioInfraccion(int idAccidente, string montoCamino, string montoCarga, string montoPropietarios, string montoOtros);
        int AgregarDatosFinales(DatosAccidenteModel datosAccidente, int armasValue, int drogasValue, int valoresValue, int prendasValue, int otrosValue, int idAccidente, int convenioValue);
        int EliminarInvolucradoAcc(int IdVehiculoInvolucrado,int IdPropietarioInvolucrado,int IdAccidente);

        public int EliminarInvolucrado(int idPersona);

        public int EditarInvolucrado(CapturaAccidentesModel model);
        public int RegistrarInfraccion(NuevaInfraccionModel model, int idDependencia);
        public string ObtenerDescripcionCausaDesdeBD(int idAccidente);
        public DatosAccidenteModel ObtenerDatosFinales(int idAccidente);
        public bool ValidarFolio(string folioInfraccion, int idDependencia);

		CapturaAccidentesModel ObtenerDetallePersona(int Id);
		CapturaAccidentesModel DatosInvolucradoEdicion(int Id);

		




	}

}
