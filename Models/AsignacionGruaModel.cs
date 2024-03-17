﻿using Microsoft.AspNetCore.Http;
using System;

namespace GuanajuatoAdminUsuarios.Models
{
    public class AsignacionGruaModel
    {
        public string FolioSolicitud { get; set; }
        public DateTime fecha { get; set; }
        public DateTime fechaSolicitud { get; set; }
        public string Grua { get; set; }
        public string Solicitante { get; set; }
        public int idSolicitud { get; set; }
        public string vehiculoCalle { get; set; }
        public string vehiculoColonia { get; set; }
        public string carretera { get; set; }
        public string municipio { get; set; }
        public string nombreEntidad { get; set; }
        public string tipoUsuario { get; set; }
        public string nombreOficial { get; set; }
        public string apellidoPaternoOficial { get; set; }
        public string apellidoMaternoOficial { get; set; }




        public string Ubicacion
        {
            get
            {
                return nombreEntidad + "   " +
                       municipio + "<br /><br />" +
                       vehiculoColonia + "  " +
                       vehiculoCalle + "<br /><br />";
            }
        }
        public string Oficial
        {
            get
            {
                return nombreOficial + "  " + apellidoPaternoOficial + "  " + apellidoMaternoOficial;



            }
        }


        public int IdPersona { get; set; }
        public int IdVehiculo { get; set; }
        public int IdMarcaVehiculo { get; set; }
        public int IdSubmarca { get; set; }
        public int IdEntidad { get; set; }
        public int IdColor { get; set; }
        public int IdTipoVehiculo { get; set; }
        public int IdCatTipoServicio { get; set; }
        public string Marca { get; set; }
        public string Submarca { get; set; }
        public string Modelo { get; set; }
        public string Placa { get; set; }
        public string Tarjeta { get; set; }
        public DateTime VigenciaTarjeta { get; set; }
        public string Serie { get; set; }
        public string Color { get; set; }
        public string TipoServicio { get; set; }
        public string EntidadRegistro { get; set; }
        public string TipoVehiculo { get; set; }
        public string Propietario { get; set; }
        public string Motor { get; set; }
        public string NumeroEconomico { get; set; }
        public int idPropietarioGrua { get; set; }
        public int idPension { get; set; }
        public int idTramoUbicacion { get; set; }
        public string kilometro { get; set; }
        public int idInfraccion { get; set; }
        public int? idVehiculo { get; set; }
        public string folioInfraccion { get; set; }
        public DateTime fechaInfraccion { get; set; }
        public string CURP { get; set; }
        public string RFC { get; set; }
        public string observaciones { get; set; }
        public string numeroInventario { get; set; }
        public string inventarios { get; set; }
        public int IdDeposito { get; set; }
		public int estatusSolicitud { get; set; }

		public IFormFile MyFile { get; set; }


    }
}
