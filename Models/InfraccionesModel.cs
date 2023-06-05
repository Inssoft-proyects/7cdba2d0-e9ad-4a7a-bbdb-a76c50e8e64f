﻿using GuanajuatoAdminUsuarios.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;

namespace GuanajuatoAdminUsuarios.Models
{
    public class InfraccionesModel : EntityModel
    {
        public int idInfraccion { get; set; }
        public int? idOficial { get; set; }
        public int? idDependencia { get; set; }
        public int? idDelegacion { get; set; }
        public int? idVehiculo { get; set; }
        public int? idAplicacion { get; set; }
        public int? idGarantia { get; set; }
        public int? idEstatusInfraccion { get; set; }
        public int? idMunicipio { get; set; }
        public int? idTramo { get; set; }
        public int? idCarretera { get; set; }
        public int? idPersona { get; set; }
        public int? idPersonaInfraccion { get; set; }
        public string placasVehiculo { get; set; }
        public string folioInfraccion { get; set; }
        public DateTime fechaInfraccion { get; set; } = DateTime.Now;
        public int kmCarretera { get; set; }
        public string observaciones { get; set; }
        public string lugarCalle { get; set; }
        public string lugarNumero { get; set; }
        public string lugarColonia { get; set; }
        public string lugarEntreCalle { get; set; }
        public bool? infraccionCortesia { get; set; }
        public string NumTarjetaCirculacion { get; set; }
        public bool isPropietarioConductor { get; set; }
        public virtual VehiculoModel Vehiculo { get; set; }
        public virtual PersonaInfraccionModel PersonaInfraccion { get; set; }
        public virtual IEnumerable<MotivoInfraccionModel> MotivosInfraccion { get; set; }
        public virtual GarantiaInfraccionModel Garantia { get; set; }

    }
}
