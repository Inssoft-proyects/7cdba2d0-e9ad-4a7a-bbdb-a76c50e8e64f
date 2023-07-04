using GuanajuatoAdminUsuarios.Models;
using System.Collections.Generic;

namespace GuanajuatoAdminUsuarios.Interfaces
{
    public interface ICapturaAccidentesService
    {
         List<CapturaAccidentesModel> ObtenerAccidentes();

        public int GuardarParte1(CapturaAccidentesModel model);

        List<CapturaAccidentesModel> BuscarPorParametro(string Placa, string Serie, string Folio);


        public int ActualizarConVehiculo(int IdVehiculo, int idAccidente);

        public int AgregarValorClasificacion(int IdClasificacionAccidente, int idAccidente);

        List<CapturaAccidentesModel> ObtenerDatosGrid(int idAccidente);

        public int ClasificacionEliminar(int idAccidente);

        List<CapturaAccidentesModel> AccidentePorID(int idAccidente);

        public int AgregarValorFactorYOpcion(int IdFactorAccidente,int IdFactorOpcionAccidente,int idAccidente);

        List<CapturaAccidentesModel> ObtenerDatosGridFactor(int idAccidente);
        public int AgregarValorCausa(int IdCausaAccidente,int idAccidente);
        List<CapturaAccidentesModel> ObtenerDatosGridCausa(int idAccidente);
        public int EliminarValorFactorYOpcion(int idAccidente);


    }
}
