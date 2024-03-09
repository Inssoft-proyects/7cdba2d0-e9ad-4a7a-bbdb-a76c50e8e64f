﻿using GuanajuatoAdminUsuarios.Models;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace GuanajuatoAdminUsuarios.Interfaces
{
    public interface IAsignacionGruasService
    {
        public List<AsignacionGruaModel> ObtenerTodasSolicitudes();
        
        public List<AsignacionGruaModel> BuscarSolicitudes(AsignacionGruaModel model);
        List<AsignacionGruaModel> BuscarPorParametro(string Placa, string Serie);
        public AsignacionGruaModel BuscarSolicitudPord(int iSo,string folio, int idOficina,int idDependencia);
        public AsignacionGruaModel DatosInfraccionAsociada(string folioSolicitud);

        public int ActualizarDatos(AsignacionGruaModel selectedRowData,int iDep);
        List<AsignacionGruaModel> ObtenerInfracciones(string folioInfraccion);
        public int UpdateDatosGrua(IFormCollection formData, int abanderamiento,int arrastre,int salvamento,int iDep,int iSo);
        public int InsertDatosGrua(IFormCollection formData, int abanderamiento, int arrastre, int salvamento, int iDep, int iSo, string horaInicioInsert, string horaArriboInsert, string horaTerminoInsert);
        public List<SeleccionGruaModel> BusquedaGruaTabla(int iDep);
        public int AgregarObs(AsignacionGruaModel formData,int iDep);
        public int InsertarInventario(string archivoInventario, int iDep, string numeroInventario);
        public SeleccionGruaModel ObtenerAsignacionPorId(int idAsignacion);
        public int EliminarGrua(int idAsignacion);
        public int EditarDatosGrua(IFormCollection formData, int abanderamiento, int arrastre, int salvamento);


    }

}
