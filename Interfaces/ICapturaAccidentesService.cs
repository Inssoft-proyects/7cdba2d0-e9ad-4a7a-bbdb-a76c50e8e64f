using GuanajuatoAdminUsuarios.Models;
using System.Collections.Generic;

namespace GuanajuatoAdminUsuarios.Interfaces
{
    public interface ICapturaAccidentesService
    {
         List<CapturaAccidentesModel> ObtenerAccidentes();

        public int GuardarParte1(CapturaAccidentesModel model);
        public CapturaAccidentesModel ObtenerAccidentePorId(int idAccidente);

        List<CapturaAccidentesModel> BuscarPorParametro(string Placa, string Serie, string Folio);


        public int ActualizarConVehiculo(int IdVehiculo, int idAccidente);
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
        List<CapturaAccidentesModel> BusquedaPersonaInvolucrada(BusquedaInvolucradoModel model);
        public int AgregarPersonaInvolucrada(int idPersonaInvolucrado, int idAccidente);
        List<CapturaAccidentesModel> VehiculosInvolucrados(int IdAccidente);
        CapturaAccidentesModel ObtenerConductorPorId(int IdPersona);
        public int GuardarComplementoVehiculo(CapturaAccidentesModel model,int IdVehiculo,int idAccidente);

        
    }

}
