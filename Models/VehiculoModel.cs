﻿using GuanajuatoAdminUsuarios.Entity;
using System;
using System.ComponentModel.DataAnnotations;
using System.Drawing;

namespace GuanajuatoAdminUsuarios.Models
{
    public class VehiculoModel : EntityModel
    {
        public int idVehiculo { get; set; }
        public string placas { get; set; }
        public string serie { get; set; }
        public string tarjeta { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime vigenciaTarjeta { get; set; }
        public int idMarcaVehiculo { get; set; }
        public int idSubmarca { get; set; }
        public int idSubmarcaUpdated { get; set; }
        public int idTipoVehiculo { get; set; }
        public string modelo { get; set; }
        public int idColor { get; set; }
        public int idEntidad { get; set; }
        public int idCatTipoServicio { get; set; }
        public string propietario { get; set; }
        public string numeroEconomico { get; set; }
        public string paisManufactura { get; set; }
        public int? idPersona { get; set; }

        public string marca { get; set; }
        public string submarca { get; set; }
        public string tipoVehiculo { get; set; }
        public string color { get; set; }
        public string entidadRegistro { get; set; }
        public string tipoServicio { get; set; }

        public string motor { get; set; }
        public int? capacidad { get; set; }
        public string poliza { get; set; }
        public bool? carga { get; set; }
        public string otros { get; set; }

        /// <summary>
        /// Estatus para saber si se encontro en Sitteg, Registro Estatal o no 
        /// </summary>
        public int? encontradoEn { get; set; }

        public PersonaMoralBusquedaModel PersonaMoralBusquedaModel { get; set; }

        //public string RFC { get; set; }
        //public string RazonSoccial { get; set; }

        public virtual PersonaModel Persona { get; set; }

        //public virtual PersonaModel PersonaUpdate { get; set; }

    }
}
