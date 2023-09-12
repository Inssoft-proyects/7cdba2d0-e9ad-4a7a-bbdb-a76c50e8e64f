using GuanajuatoAdminUsuarios.Entity;
using GuanajuatoAdminUsuarios.Models;
using System.Collections.Generic;

namespace GuanajuatoAdminUsuarios.Interfaces
{
    public interface ICapturaAccidentesService
    {
         List<CapturaAccidentesModel> ObtenerAccidentes(int idOficina);

        public int GuardarParte1(CapturaAccidentesModel model, int idOficina);
        public CapturaAccidentesModel ObtenerAccidentePorId(int idAccidente, int idOficina);

        List<CapturaAccidentesModel> BuscarPorParametro(string Placa, string Serie, string Folio);


        public int ActualizarConVehiculo(int IdVehiculo, int idAccidente,int IdPersona, string Placa, string Serie);
        public int BorrarVehiculoAccidente(int idAccidente, int idVehiculo);
        public int InsertarConductor(int IdVehiculo, int idAccidente, int IdPersona);
        public int AgregarValorClasificacion(int IdClasificacionAccidente, int idAccidente);

        List<CapturaAccidentesModel> ObtenerDatosGrid(int idAccidente);

        public int ClasificacionEliminar(int idAccidente);

        List<CapturaAccidentesModel> AccidentePorID(int idAccidente);

        public int AgregarValorFactorYOpcion(int IdFactorAccidente,int IdFactorOpcionAccidente,int idAccidente);

        List<CapturaAccidentesModel> ObtenerDatosGridFactor(int idAccidente);
        public int AgregarValorCausa(int IdCausaAccidente,int idAccidente);
        public int EditarValorCausa(int IdCausaAccidente,int idAccidente, int IdCausaAccidenteEdit);

        List<CapturaAccidentesModel> ObtenerDatosGridCausa(int idAccidente);
        public int EliminarValorFactorYOpcion(int idAccidente);
        public int EliminarCausaBD(int IdCausaAccidente,int idAccidente);
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

        int AgregarDatosFinales(DatosAccidenteModel datosAccidente, int armasValue, int drogasValue, int valoresValue, int prendasValue, int otrosValue, int idAccidente);
        int EliminarInvolucradoAcc(int IdVehiculoInvolucrado,int IdPropietarioInvolucrado,int IdAccidente);
        public int RegistrarInfraccion(NuevaInfraccionModel model);
        public string ObtenerDescripcionCausaDesdeBD(int idAccidente);
        public DatosAccidenteModel ObtenerDatosFinales(int idAccidente);


    }

}
