using GuanajuatoAdminUsuarios.Models;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace GuanajuatoAdminUsuarios.Interfaces
{
    public interface IAsignacionGruasService
    {
        public List<AsignacionGruaModel> ObtenerTodasSolicitudes();
        
        public List<AsignacionGruaModel> BuscarSolicitudes(AsignacionGruaModel model);
        List<AsignacionGruaModel> BuscarPorParametro(string Placa, string Serie);
        public AsignacionGruaModel BuscarSolicitudPord(int iSo);
        public int ActualizarDatos(AsignacionGruaModel selectedRowData,int iSo);
        List<AsignacionGruaModel> ObtenerInfracciones(string folioInfraccion);
        public int UpdateDatosGrua(IFormCollection formData, int abanderamiento,int arrastre,int salvamento,int iSo);
        public List<SeleccionGruaModel> BusquedaGruaTabla(int iSo);
        public int AgregarObs(AsignacionGruaModel formData,int iSo);
        public int InsertarInventario(byte[] imageData, int iSo, string numeroInventario);
        public SeleccionGruaModel ObtenerAsignacionPorId(int idAsignacion);
        public int EliminarGrua(int idAsignacion);
        public int EditarDatosGrua(IFormCollection formData, int abanderamiento, int arrastre, int salvamento);


    }

}
